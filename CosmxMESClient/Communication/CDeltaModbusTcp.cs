using CosmxMESClient.interfaceConfig;
using CosmxMESClient.Services;
using HslCommunication.ModBus;
using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace CosmxMESClient.Communication {
    public class CDeltaModbusTcp:IPlcCommunication, IDisposable {
        #region 事件
        public event EventHandler<DataReadEventArgs> DataRead;
        public event EventHandler<DataWriteEventArgs> DataWriteCompleted;
        public event EventHandler<HeartbeatEventArgs> HeartbeatStatusChanged;
        public event Action<string> WriteLog;
        protected virtual void OnDataRead( DataReadEventArgs e ) {
            DataRead?.Invoke(this,e);
            }

        protected virtual void OnDataWriteCompleted( DataWriteEventArgs e ) {
            DataWriteCompleted?.Invoke(this,e);
            }

        protected virtual void OnHeartbeatStatusChanged( HeartbeatEventArgs e ) {
            HeartbeatStatusChanged?.Invoke(this,e);
            }
        #endregion

        #region 自动读取
        private Timer _autoReadTimer;
        private ConcurrentDictionary<string, AutoReadConfig> _autoReadConfigs = new ConcurrentDictionary<string, AutoReadConfig>();
        private DateTime _nextReadTime = DateTime.MaxValue;
        #endregion

        #region 自动读取分组优化
        private class AutoReadGroup {
            public TypeCode DataType {
                get; set;
                }
            public int ReadIntervalMs {
                get; set;
                }
            public List<AutoReadConfig> Configs { get; set; } = new List<AutoReadConfig>( );
            public List<string> AllAddresses => Configs.SelectMany(c => c.Addresses).ToList( );
            public DateTime NextReadTime {
                get; set;
                }
            }

        private ConcurrentDictionary<string, AutoReadGroup> _autoReadGroups = new ConcurrentDictionary<string, AutoReadGroup>();
        #endregion

        #region 私有字段和属性
        private static ModbusTcpNet _modbusClient;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private bool _disposed = false;
        private string _lastError = string.Empty;
        private IPEndPoint _localEndPoint;
        public Encoding StringEncoding { get; set; } = Encoding.ASCII;

        #region 端序配置
        private ByteOrderEnum _byteOrder = ByteOrderEnum.BigEndian;
        public ByteOrderEnum ByteOrder {
            get => _byteOrder;
            set {
                _byteOrder=value;
                UpdateHslByteOrder( );
                }
            }
        #endregion
        #region 字符串端序配置
        private StringByteOrderEnum _stringByteOrder = StringByteOrderEnum.RegisterByteSwap;

        public StringByteOrderEnum StringByteOrder {
            get => _stringByteOrder;
            set => _stringByteOrder=value;
            }
        #endregion
        // 连接参数
        private string _ipAddress;
        private int _port;
        private byte _slaveId;
        private int _timeout = 1000;

        public string IpAddress {
            get => _ipAddress;
            set {
                if (!_IsConnected)
                    _ipAddress=value;
                }
            }

        public IPEndPoint IPEnd {
            get => _localEndPoint;
            set {
                if (!_IsConnected)
                    _localEndPoint=value;
                }
            }

        public int Port {
            get => _port;
            set {
                if (!_IsConnected)
                    _port=value;
                }
            }

        public byte SlaveId {
            get => _slaveId;
            set {
                if (!_IsConnected)
                    _slaveId=value;
                if (_modbusClient!=null)
                    _modbusClient.Station=value;
                }
            }

        public int Timeout {
            get => _timeout;
            set {
                _timeout=value;
                _lock.EnterWriteLock( );
                try {
                    if (_modbusClient!=null) {
                        _modbusClient.ConnectTimeOut=value;
                        _modbusClient.ReceiveTimeOut=value;
                        }
                    }
                finally {
                    _lock.ExitWriteLock( );
                    }
                }
            }

        public string LastError => _lastError;
        #endregion

        #region 心跳机制
        private Timer _heartbeatTimer;
        private int _heartbeatInterval = 30000;
        private int _heartbeatTimeout = 5000;
        private int _heartbeatRetryCount = 3;
        private int _currentRetryCount = 0;
        private bool _heartbeatEnabled = true;
        private ushort _heartbeatAddress = 0;

        public int HeartbeatInterval {
            get => _heartbeatInterval;
            set {
                _heartbeatInterval=value;
                if (_heartbeatTimer!=null) {
                    _heartbeatTimer.Interval=value;
                    }
                }
            }

        public bool HeartbeatEnabled {
            get => _heartbeatEnabled;
            set => _heartbeatEnabled=value;
            }

        public ushort HeartbeatAddress {
            get => _heartbeatAddress;
            set => _heartbeatAddress=value;
            }

        bool IPlcCommunication.IsConnected => _IsConnected;
        private bool _IsConnected = _modbusClient != null ? _modbusClient.IpAddressPing() == IPStatus.Success : false;
        #endregion

        #region 构造函数
        public CDeltaModbusTcp( string ipAddress,int port,byte slaveId,
                              string ipAddressEnd = null,int portEnd = 0,
                              ByteOrderEnum byteOrder = ByteOrderEnum.BigEndian ) {
            _ipAddress=ipAddress;
            _port=port;
            _slaveId=slaveId;
            _byteOrder=byteOrder;
            if (IPAddress.TryParse(ipAddress,out IPAddress _IPEnd)) {
                IPEnd=new IPEndPoint(_IPEnd,port);
                }

            if (!string.IsNullOrEmpty(ipAddressEnd)&&portEnd>0) {
                if (IPAddress.TryParse(ipAddressEnd,out IPAddress localIP)) {
                    _localEndPoint=new IPEndPoint(localIP,portEnd);
                    }
                }
            InitializeAutoReadHandlers( );
            InitializeHeartbeat( );
            }

        public CDeltaModbusTcp( string ipAddress,int port,byte slaveId,bool isBigEndian )
            : this(ipAddress,port,slaveId,null,0,isBigEndian ? ByteOrderEnum.BigEndian : ByteOrderEnum.LittleEndian) {
            }

        public CDeltaModbusTcp( ) : this("192.168.1.100",502,1) { }
        #endregion

        #region 心跳管理
        private void InitializeHeartbeat( ) {
            _heartbeatTimer=new Timer(_heartbeatInterval);
            _heartbeatTimer.Elapsed+=OnHeartbeatElapsed;
            _heartbeatTimer.AutoReset=true;
            _heartbeatTimer.Enabled=_heartbeatEnabled;
            }

        private void OnHeartbeatElapsed( object sender,ElapsedEventArgs e ) {
            if (!_IsConnected||!_heartbeatEnabled)
                return;

            ThreadPool.QueueUserWorkItem(state => ExecuteHeartbeat( ));
            }

        private void ExecuteHeartbeat( ) {
            try {
                _lock.EnterReadLock( );
                try {
                    if (!_IsConnected)
                        return;
                    var result = _modbusClient.ReadInt16(_heartbeatAddress.ToString());
                    if (!result.IsSuccess)
                        throw new Exception(result.Message);
                    }
                finally {
                    _lock.ExitReadLock( );
                    }

                _currentRetryCount=0;
                OnHeartbeatStatusChanged(new HeartbeatEventArgs(true,"心跳检测成功",0));
                }
            catch (Exception ex) {
                _currentRetryCount++;
                if (_currentRetryCount>=_heartbeatRetryCount) {
                    OnHeartbeatStatusChanged(new HeartbeatEventArgs(false,
                        $"心跳检测失败，连接可能已断开: {ex.Message}",_currentRetryCount));
                    //AttemptReconnect( );
                    }
                else {
                    OnHeartbeatStatusChanged(new HeartbeatEventArgs(true,
                        $"心跳检测暂时失败 ({_currentRetryCount}/{_heartbeatRetryCount}): {ex.Message}",
                        _currentRetryCount));
                    }
                }
            }

        private async void AttemptReconnect( ) {
            const int maxReconnectAttempts = 3;
            const int reconnectDelay = 3000; // 3秒

            for (int attempt = 1; attempt<=maxReconnectAttempts; attempt++) {
                try {
                    _lock.EnterWriteLock( );
                    try {
                        CloseConnectionUnderLock( );
                        await Task.Delay(reconnectDelay);

                        if (InitializeUnderLock( )) {
                            if (TestConnection( )) {
                                _currentRetryCount=0;
                                OnHeartbeatStatusChanged(new HeartbeatEventArgs(true,
                                    $"连接自动恢复成功 (第{attempt}次尝试)",0));
                                return;
                                }
                            }
                        }
                    finally {
                        _lock.ExitWriteLock( );
                        }
                    }
                catch (Exception ex) {
                    OnHeartbeatStatusChanged(new HeartbeatEventArgs(false,
                        $"自动重连失败 (第{attempt}/{maxReconnectAttempts}次): {ex.Message}",
                        _currentRetryCount));
                    }
                }
            }

        private bool TestConnection( ) {
            try {
                var result = _modbusClient.ReadInt16(_heartbeatAddress.ToString());
                return result.IsSuccess;
                }
            catch {
                return false;
                }
            }
        #endregion

        #region 连接管理
        public bool Initialize( ) {
            LoggingService.Info($"开始初始化PLC连接: {_ipAddress}:{_port}, 站号: {_slaveId}");

            _lock.EnterWriteLock( );
            try {
                return InitializeUnderLock( );
                }
            finally {
                _lock.ExitWriteLock( );
                }
            }

        private bool InitializeUnderLock( ) {
            try {
                if (_IsConnected) {
                    LoggingService.Info("PLC连接已存在，先关闭现有连接");
                    CloseUnderLock( );
                    }

                _modbusClient=new ModbusTcpNet(_ipAddress,_port,_slaveId);
                _modbusClient.ConnectTimeOut=_timeout;
                _modbusClient.ReceiveTimeOut=_timeout;
                UpdateHslByteOrder( );

                if (_localEndPoint!=null) {
                    // HSL 不直接支持绑定本地端点，可能需自定义 TcpClient
                    // 这里暂时忽略，如需支持需扩展
                    }

                var result = _modbusClient.ConnectServer();
                if (!result.IsSuccess) {
                    LoggingService.Error($"PLC连接失败: {result.Message}");
                    _lastError=result.Message;
                    return false;
                    }


                _IsConnected=true;
                _lastError=string.Empty;

                if (_heartbeatAddress>0&&_heartbeatEnabled) {
                    StartHeartbeatService( );
                    }
                LoggingService.PLCCommunication($"PLC连接成功: {_ipAddress}:{_port}");
                return true;
                }
            catch (Exception ex) {
                LoggingService.Error("PLC连接初始化异常",ex);
                _lastError=ex.Message;
                return false;
                }
            }

        private void UpdateHslByteOrder( ) {
            if (_modbusClient!=null) {
                switch (_byteOrder) {
                    case ByteOrderEnum.BigEndian:
                        _modbusClient.ByteTransform.DataFormat=HslCommunication.Core.DataFormat.ABCD;
                        break;
                    case ByteOrderEnum.LittleEndian:
                        _modbusClient.ByteTransform.DataFormat=HslCommunication.Core.DataFormat.DCBA;
                        break;
                    case ByteOrderEnum.BigEndianByteSwap:
                        _modbusClient.ByteTransform.DataFormat=HslCommunication.Core.DataFormat.BADC;
                        break;
                    case ByteOrderEnum.LittleEndianByteSwap:
                        _modbusClient.ByteTransform.DataFormat=HslCommunication.Core.DataFormat.CDAB;
                        break;
                    }
                }
            }

        public void Close( ) {
            _lock.EnterWriteLock( );
            try {
                CloseUnderLock( );
                }
            finally {
                _lock.ExitWriteLock( );
                }
            }

        private void CloseUnderLock( ) {
            try {
                StopHeartbeatService( );
                CloseConnectionUnderLock( );
                }
            catch (Exception ex) {
                _lastError=ex.Message;
                LogException(ex,"Close");
                }
            }

        private void CloseConnectionUnderLock( ) {
            _modbusClient?.ConnectClose( );
            _modbusClient=null;
            // _IsConnected=false;
            _lastError=string.Empty;
            }
        #endregion

        #region 基础数据读写
        public bool ReadRegister( string address,ref int value,int power = 1 ) {
            if (!_IsConnected||!TryParseAddress(address,out ushort modbusAddress))
                return false;

            _lock.EnterReadLock( );
            try {
                var result = _modbusClient.ReadInt16(modbusAddress.ToString());
                if (!result.IsSuccess) {
                    _lastError=result.Message;
                    return false;
                    }
                // 应用缩放系数
                value=power==1 ? result.Content : result.Content/power;
                }
            catch (Exception ex) {
                HandleReadError(ex,$"ReadRegister({address})");
                return false;
                }
            finally {
                _lock.ExitReadLock( );
                }

            _lastError=string.Empty;
            OnDataRead(new DataReadEventArgs(address,value,TypeCode.Int16,IPEnd));
            return true;
            }

        public bool WriteRegister( string address,int value,int power = 1 ) {
            if (!_IsConnected||!TryParseAddress(address,out ushort modbusAddress))
                return false;

            // 应用缩放系数并检查范围
            int scaledValue = power == 1 ? value : value * power;
            if (scaledValue<short.MinValue||scaledValue>short.MaxValue) {
                _lastError=$"Value {value} after scaling by {power} is out of short range";
                return false;
                }

            _lock.EnterWriteLock( );
            try {
                var result = _modbusClient.Write(modbusAddress.ToString(), (short)scaledValue);
                if (!result.IsSuccess) {
                    _lastError=result.Message;
                    return false;
                    }
                _lastError=string.Empty;
                }
            catch (Exception ex) {
                HandleWriteError("WriteRegister",new List<string> { address },value,ex);
                return false;
                }
            finally {
                _lock.ExitWriteLock( );
                }

            OnDataWriteCompleted(new DataWriteEventArgs("WriteRegister",new List<string> { address },value,true));
            return true;
            }

        public bool ReadInt32( string address,ref int value,int power = 1 ) {
            if (!_IsConnected||!TryParseAddress(address,out ushort modbusAddress))
                return false;

            _lock.EnterReadLock( );
            try {
                var result = _modbusClient.ReadInt32(modbusAddress.ToString());
                if (!result.IsSuccess) {
                    _lastError=result.Message;
                    return false;
                    }
                // 应用缩放系数
                value=power==1 ? result.Content : result.Content/power;
                }
            catch (Exception ex) {
                HandleReadError(ex,$"ReadInt32({address})");
                return false;
                }
            finally {
                _lock.ExitReadLock( );
                }

            _lastError=string.Empty;
            OnDataRead(new DataReadEventArgs(address,value,TypeCode.Int32,IPEnd));
            return true;
            }

        public bool WriteInt32( string address,int value,int power = 1 ) {
            if (!_IsConnected||!TryParseAddress(address,out ushort modbusAddress))
                return false;

            // 应用缩放系数
            int scaledValue = power == 1 ? value : value * power;

            _lock.EnterWriteLock( );
            try {
                var result = _modbusClient.Write(modbusAddress.ToString(), scaledValue);
                if (!result.IsSuccess) {
                    _lastError=result.Message;
                    return false;
                    }
                _lastError=string.Empty;
                }
            catch (Exception ex) {
                HandleWriteError("WriteInt32",new List<string> { address },value,ex);
                return false;
                }
            finally {
                _lock.ExitWriteLock( );
                }

            OnDataWriteCompleted(new DataWriteEventArgs("WriteInt32",new List<string> { address },value,true));
            return true;
            }

        public bool ReadFloat( string address,ref float value,int power = 1 ) {
            if (!_IsConnected||!TryParseAddress(address,out ushort modbusAddress))
                return false;

            _lock.EnterReadLock( );
            try {
                var result = _modbusClient.ReadFloat(modbusAddress.ToString());
                if (!result.IsSuccess) {
                    _lastError=result.Message;
                    return false;
                    }
                // 应用缩放系数
                value=power==1 ? result.Content : result.Content/power;
                }
            catch (Exception ex) {
                HandleReadError(ex,$"ReadFloat({address})");
                return false;
                }
            finally {
                _lock.ExitReadLock( );
                }

            _lastError=string.Empty;
            OnDataRead(new DataReadEventArgs(address,value,TypeCode.Single,IPEnd));
            return true;
            }

        public bool WriteFloat( string address,float value,int power = 1 ) {
            if (!_IsConnected||!TryParseAddress(address,out ushort modbusAddress))
                return false;

            // 应用缩放系数
            float scaledValue = power == 1 ? value : value * power;

            _lock.EnterWriteLock( );
            try {
                var result = _modbusClient.Write(modbusAddress.ToString(), scaledValue);
                if (!result.IsSuccess) {
                    _lastError=result.Message;
                    return false;
                    }
                _lastError=string.Empty;
                }
            catch (Exception ex) {
                HandleWriteError("WriteFloat",new List<string> { address },value,ex);
                return false;
                }
            finally {
                _lock.ExitWriteLock( );
                }

            OnDataWriteCompleted(new DataWriteEventArgs("WriteFloat",new List<string> { address },value,true));
            return true;
            }

        public bool ReadDouble( string address,ref double value,int power = 1 ) {
            if (!_IsConnected||!TryParseAddress(address,out ushort modbusAddress))
                return false;

            _lock.EnterReadLock( );
            try {
                var result = _modbusClient.ReadDouble(modbusAddress.ToString());
                if (!result.IsSuccess) {
                    _lastError=result.Message;
                    return false;
                    }
                // 应用缩放系数
                value=power==1 ? result.Content : result.Content/power;
                }
            catch (Exception ex) {
                HandleReadError(ex,$"ReadDouble({address})");
                return false;
                }
            finally {
                _lock.ExitReadLock( );
                }

            _lastError=string.Empty;
            OnDataRead(new DataReadEventArgs(address,value,TypeCode.Double,IPEnd));
            return true;
            }

        public bool WriteDouble( string address,double value,int power = 1 ) {
            if (!_IsConnected||!TryParseAddress(address,out ushort modbusAddress))
                return false;

            // 应用缩放系数
            double scaledValue = power == 1 ? value : value * power;

            _lock.EnterWriteLock( );
            try {
                var result = _modbusClient.Write(modbusAddress.ToString(), scaledValue);
                if (!result.IsSuccess) {
                    _lastError=result.Message;
                    return false;
                    }
                _lastError=string.Empty;
                }
            catch (Exception ex) {
                HandleWriteError("WriteDouble",new List<string> { address },value,ex);
                return false;
                }
            finally {
                _lock.ExitWriteLock( );
                }

            OnDataWriteCompleted(new DataWriteEventArgs("WriteDouble",new List<string> { address },value,true));
            return true;
            }

        public bool ReadRegisterBit( string address,ref bool value ) {
            if (!_IsConnected||!TryParseExtendedAddress(address,out ushort modbusAddress,out byte bitIndex))
                return false;

            _lock.EnterReadLock( );
            try {
                var result = _modbusClient.ReadBool($"{modbusAddress}.{bitIndex}");
                if (!result.IsSuccess) {
                    _lastError=result.Message;
                    return false;
                    }
                value=result.Content;
                }
            catch (Exception ex) {
                HandleReadError(ex,$"ReadRegisterBit({address})");
                return false;
                }
            finally {
                _lock.ExitReadLock( );
                }

            _lastError=string.Empty;
            OnDataRead(new DataReadEventArgs(address,value,TypeCode.Boolean,IPEnd));
            return true;
            }

        public bool WriteRegisterBit( string address,bool value ) {
            if (!_IsConnected||!TryParseExtendedAddress(address,out ushort modbusAddress,out byte bitIndex))
                return false;

            _lock.EnterWriteLock( );
            try {
                var result = _modbusClient.Write($"{modbusAddress}.{bitIndex}", value);
                if (!result.IsSuccess) {
                    _lastError=result.Message;
                    return false;
                    }
                _lastError=string.Empty;
                }
            catch (Exception ex) {
                HandleWriteError("WriteRegisterBit",new List<string> { address },value,ex);
                return false;
                }
            finally {
                _lock.ExitWriteLock( );
                }

            OnDataWriteCompleted(new DataWriteEventArgs("WriteRegisterBit",new List<string> { address },value,true));
            return true;
            }
        #endregion

        #region 批量数据操作
        public bool BatchReadInt( List<string> addresses,ref int[] values,int power = 1 ) {
            // 先读取为 short[]，再转换为 int[]
            short[] shortValues = new short[values.Length];
            return BatchReadByType(addresses,ref shortValues,TypeCode.Int16,power,
                ( client,startAddr,length ) => client.ReadInt16(startAddr,(ushort) length),
                ( result,index ) => result.Content[index]);
            }

        public bool BatchWriteInt( List<string> addresses,int[] values,int power = 1 ) {
            return BatchWriteByType(addresses,values,TypeCode.Int16,power,
                ( client,startAddr,data ) => client.Write(startAddr,data.Select(v => (short) v).ToArray( )));
            }

        public bool BatchReadInt32( List<string> addresses,ref int[] values,int power = 1 ) {
            return BatchReadByType(addresses,ref values,TypeCode.Int32,power,
                ( client,startAddr,length ) => client.ReadInt32(startAddr,(ushort) length),
                ( result,index ) => result.Content[index]);
            }

        public bool BatchWriteInt32( List<string> addresses,int[] values,int power = 1 ) {
            return BatchWriteByType(addresses,values,TypeCode.Int32,power,
                ( client,startAddr,data ) => client.Write(startAddr,data));
            }

        public bool BatchReadFloat( List<string> addresses,ref float[] values,int power = 1 ) {
            return BatchReadByType(addresses,ref values,TypeCode.Single,power,
                ( client,startAddr,length ) => client.ReadFloat(startAddr,(ushort) length),
                ( result,index ) => result.Content[index]);
            }

        public bool BatchWriteFloat( List<string> addresses,float[] values,int power = 1 ) {
            return BatchWriteByType(addresses,values,TypeCode.Single,power,
                ( client,startAddr,data ) => client.Write(startAddr,data));
            }

        public bool BatchReadDouble( List<string> addresses,ref double[] values,int power = 1 ) {
            return BatchReadByType(addresses,ref values,TypeCode.Double,power,
                ( client,startAddr,length ) => client.ReadDouble(startAddr,(ushort) length),
                ( result,index ) => result.Content[index]);
            }

        public bool BatchWriteDouble( List<string> addresses,double[] values,int power = 1 ) {
            return BatchWriteByType(addresses,values,TypeCode.Double,power,
                ( client,startAddr,data ) => client.Write(startAddr,data));
            }

        public bool BatchReadString( List<string> addresses,int maxStringLength,ref string[] texts ) {
            if (!_IsConnected||addresses==null||addresses.Count==0)
                return false;

            texts=new string[addresses.Count];

            _lock.EnterReadLock( );
            try {
                for (int i = 0; i<addresses.Count; i++) {
                    var result = _modbusClient.ReadString(addresses[i], (ushort)maxStringLength);
                    if (!result.IsSuccess) {
                        _lastError=result.Message;
                        return false;
                        }
                    string originalText = result.Content?.TrimEnd('\0', ' ').Trim();
                    texts[i]=FixStringOrder(originalText);
                    }

                _lastError=string.Empty;
                OnDataRead(new DataReadEventArgs(string.Join(", ",addresses),texts,TypeCode.String,IPEnd,true,addresses.Count));
                return true;
                }
            catch (Exception ex) {
                HandleReadError(ex,$"BatchReadString({string.Join(", ",addresses)})");
                return false;
                }
            finally {
                _lock.ExitReadLock( );
                }
            }

        public bool BatchWriteString( List<string> addresses,string[] texts,int maxStringLength ) {
            if (!_IsConnected||addresses==null||texts==null||addresses.Count!=texts.Length)
                return false;

            _lock.EnterWriteLock( );
            try {
                for (int i = 0; i<addresses.Count; i++) {
                    string correctedText = ReverseStringOrder(texts[i]);
                    var result = _modbusClient.Write(addresses[i], correctedText);
                    if (!result.IsSuccess) {
                        _lastError=result.Message;
                        return false;
                        }
                    }

                OnDataWriteCompleted(new DataWriteEventArgs("BatchWriteString",addresses,texts,true));
                return true;
                }
            catch (Exception ex) {
                HandleWriteError("BatchWriteString",addresses,texts,ex);
                return false;
                }
            finally {
                _lock.ExitWriteLock( );
                }
            }

        public bool BatchReadRegisterBits( List<string> addresses,ref bool[] values ) {
            if (!_IsConnected||addresses==null||addresses.Count==0)
                return false;

            values=new bool[addresses.Count];

            _lock.EnterReadLock( );
            try {
                for (int i = 0; i<addresses.Count; i++) {
                    var result = _modbusClient.ReadBool(addresses[i]);
                    if (!result.IsSuccess) {
                        _lastError=result.Message;
                        return false;
                        }
                    values[i]=result.Content;
                    }

                _lastError=string.Empty;
                OnDataRead(new DataReadEventArgs(string.Join(", ",addresses),values,TypeCode.Boolean,IPEnd,true,addresses.Count));
                return true;
                }
            catch (Exception ex) {
                HandleReadError(ex,$"BatchReadRegisterBits({string.Join(", ",addresses)})");
                return false;
                }
            finally {
                _lock.ExitReadLock( );
                }
            }


        public bool BatchWriteRegisterBits( List<string> addresses,bool[] values ) {
            if (!_IsConnected||addresses==null||values==null||addresses.Count!=values.Length)
                return false;

            _lock.EnterWriteLock( );
            try {
                for (int i = 0; i<addresses.Count; i++) {
                    var result = _modbusClient.Write(addresses[i], values[i]);
                    if (!result.IsSuccess) {
                        _lastError=result.Message;
                        return false;
                        }
                    }

                OnDataWriteCompleted(new DataWriteEventArgs("BatchWriteRegisterBits",addresses,values,true));
                return true;
                }
            catch (Exception ex) {
                HandleWriteError("BatchWriteRegisterBits",addresses,values,ex);
                return false;
                }
            finally {
                _lock.ExitWriteLock( );
                }
            }
        #endregion

        #region 位操作
        public bool ReadCoil( string address,ref bool value ) {
            if (!_IsConnected||!TryParseCoilAddress(address,out ushort coilAddress))
                return false;

            _lock.EnterReadLock( );
            try {
                var result = _modbusClient.ReadCoil(coilAddress.ToString());
                if (!result.IsSuccess) {
                    _lastError=result.Message;
                    return false;
                    }
                value=result.Content;
                }
            catch (Exception ex) {
                HandleReadError(ex,$"ReadCoil({address})");
                return false;
                }
            finally {
                _lock.ExitReadLock( );
                }

            _lastError=string.Empty;
            OnDataRead(new DataReadEventArgs(address,value,TypeCode.Boolean,IPEnd));
            return true;
            }

        public bool WriteCoil( string address,bool value ) {
            if (!_IsConnected||!TryParseCoilAddress(address,out ushort coilAddress))
                return false;

            _lock.EnterWriteLock( );
            try {
                var result = _modbusClient.Write(coilAddress.ToString(), value);
                if (!result.IsSuccess) {
                    _lastError=result.Message;
                    return false;
                    }
                _lastError=string.Empty;
                }
            catch (Exception ex) {
                HandleWriteError("WriteCoil",new List<string> { address },value,ex);
                return false;
                }
            finally {
                _lock.ExitWriteLock( );
                }

            OnDataWriteCompleted(new DataWriteEventArgs("WriteCoil",new List<string> { address },value,true));
            return true;
            }

        public bool ReadCoils( string address,int length,ref bool[] values ) {
            if (!_IsConnected||!TryParseCoilAddress(address,out ushort coilAddress))
                return false;

            _lock.EnterReadLock( );
            try {
                var result = _modbusClient.ReadCoil(coilAddress.ToString(), (ushort)(short)length);
                if (!result.IsSuccess) {
                    _lastError=result.Message;
                    return false;
                    }
                values=result.Content;
                }
            catch (Exception ex) {
                HandleReadError(ex,$"ReadCoils({address})");
                return false;
                }
            finally {
                _lock.ExitReadLock( );
                }

            _lastError=string.Empty;
            OnDataRead(new DataReadEventArgs(address,values,TypeCode.Boolean,IPEnd,true,length));
            return true;
            }

        public bool WriteCoils( string address,bool[] values ) {
            if (!_IsConnected||!TryParseCoilAddress(address,out ushort coilAddress))
                return false;

            _lock.EnterWriteLock( );
            try {
                var result = _modbusClient.Write(coilAddress.ToString(), values);
                if (!result.IsSuccess) {
                    _lastError=result.Message;
                    return false;
                    }
                _lastError=string.Empty;
                }
            catch (Exception ex) {
                HandleWriteError("WriteCoils",new List<string> { address },values,ex);
                return false;
                }
            finally {
                _lock.ExitWriteLock( );
                }

            OnDataWriteCompleted(new DataWriteEventArgs("WriteCoils",new List<string> { address },values,true));
            return true;
            }
        #endregion

        #region 字符串操作
        public bool ReadString( string address,ref string text,int maxLength = 20 ) {
            if (!_IsConnected||!TryParseAddress(address,out ushort modbusAddress))
                return false;

            _lock.EnterReadLock( );
            try {
                var result = _modbusClient.ReadString(modbusAddress.ToString(), (ushort)maxLength);
                if (!result.IsSuccess) {
                    _lastError=result.Message;
                    return false;
                    }

                string originalText = result.Content?.TrimEnd('\0', ' ').Trim();
                text=FixStringOrder(originalText);
                }
            catch (Exception ex) {
                HandleReadError(ex,$"ReadString({address})");
                return false;
                }
            finally {
                _lock.ExitReadLock( );
                }

            _lastError=string.Empty;
            OnDataRead(new DataReadEventArgs(address,text,TypeCode.String,IPEnd));
            return true;
            }

        public bool WriteString( string address,string text ) {
            if (!_IsConnected||string.IsNullOrEmpty(text)||!TryParseAddress(address,out ushort modbusAddress))
                return false;

            _lock.EnterWriteLock( );
            try {
                // 反转字符串顺序以便正确写入
                string correctedText = ReverseStringOrder(text);

                var result = _modbusClient.Write(modbusAddress.ToString(), correctedText);
                if (!result.IsSuccess) {
                    _lastError=result.Message;
                    return false;
                    }
                _lastError=string.Empty;
                }
            catch (Exception ex) {
                HandleWriteError("WriteString",new List<string> { address },text,ex);
                return false;
                }
            finally {
                _lock.ExitWriteLock( );
                }

            OnDataWriteCompleted(new DataWriteEventArgs("WriteString",new List<string> { address },text,true));
            return true;
            }
        #endregion
        #region 通用的批量读取方法
        /// <summary>
        /// 通用的批量读取方法
        /// </summary>
        /// <summary>
        /// 通用的批量读取方法（支持power参数）
        /// </summary>
        private bool BatchReadByType<T>( List<string> addresses,ref T[] values,TypeCode dataType,int power,
            Func<ModbusTcpNet,string,int,HslCommunication.OperateResult<T[]>> bulkReadFunc,
            Func<HslCommunication.OperateResult<T[]>,int,T> getValueFunc ) {
            if (!_IsConnected||addresses==null||addresses.Count==0)
                return false;

            values=new T[addresses.Count];
            var addressGroups = GroupContinuousAddresses(addresses, dataType);

            _lock.EnterReadLock( );
            try {
                foreach (var group in addressGroups) {
                    if (ShouldUseBulkRead(group)) {
                        if (!BulkReadGroup(group,addresses,values,power,dataType,bulkReadFunc,getValueFunc))
                            return false;
                        }
                    else {
                        if (!SingleReadGroup(group,addresses,values,dataType,power))
                            return false;
                        }
                    }

                _lastError=string.Empty;
                OnDataRead(new DataReadEventArgs(string.Join(", ",addresses),values,dataType,IPEnd,true,addresses.Count));
                return true;
                }
            catch (Exception ex) {
                HandleReadError(ex,$"BatchRead{dataType}({string.Join(", ",addresses)})");
                return false;
                }
            finally {
                _lock.ExitReadLock( );
                }
            }
        private bool BulkReadGroup<T>( AddressGroup group,List<string> addresses,T[] values,int power,TypeCode typeCode,
            Func<ModbusTcpNet,string,int,HslCommunication.OperateResult<T[]>> bulkReadFunc,
            Func<HslCommunication.OperateResult<T[]>,int,T> getValueFunc ) {
            try {
                var result = bulkReadFunc(_modbusClient, group.StartAddress.ToString(), group.Length);
                if (!result.IsSuccess) {
                    _lastError=result.Message;
                    return false;
                    }

                for (int i = 0; i<group.OriginalIndices.Count; i++) {
                    int originalIndex = group.OriginalIndices[i];
                    T rawValue = getValueFunc(result, i);

                    // 应用缩放系数
                    if (power!=1) {
                        if (typeCode==TypeCode.Int16) {
                            rawValue=(T) (object) ( Convert.ToInt16(rawValue)/power );
                            }
                        else if (typeCode==TypeCode.Int32) {
                            rawValue=(T) (object) ( Convert.ToInt32(rawValue)/power );
                            }
                        else if (typeCode==TypeCode.Single) {
                            rawValue=(T) (object) ( Convert.ToSingle(rawValue)/power );
                            }
                        else if (typeCode==TypeCode.Double) {
                            rawValue=(T) (object) ( Convert.ToDouble(rawValue)/power );
                            }
                        }

                    values[originalIndex]=rawValue;
                    }

                return true;
                }
            catch (Exception ex) {
                _lastError=$"批量读取{_dataTypeInfo[group.DataType].Description}地址 {group.StartAddress}-{group.EndAddress} 失败: {ex.Message}";
                return false;
                }
            }

        private bool SingleReadGroup<T>( AddressGroup group,List<string> addresses,T[] values,TypeCode dataType,int power ) {
            foreach (int originalIndex in group.OriginalIndices) {
                bool success = false;
                string address = addresses[originalIndex];

                try {
                    object rawValue = null;

                    switch (dataType) {
                        case TypeCode.Int16:
                            var result16 = _modbusClient.ReadInt16(address);
                            if (result16.IsSuccess) {
                                rawValue=result16.Content;
                                success=true;
                                }
                            break;
                        case TypeCode.Int32:
                            var result32 = _modbusClient.ReadInt32(address);
                            if (result32.IsSuccess) {
                                rawValue=result32.Content;
                                success=true;
                                }
                            break;
                        case TypeCode.Single:
                            var resultFloat = _modbusClient.ReadFloat(address);
                            if (resultFloat.IsSuccess) {
                                rawValue=resultFloat.Content;
                                success=true;
                                }
                            break;
                        case TypeCode.Double:
                            var resultDouble = _modbusClient.ReadDouble(address);
                            if (resultDouble.IsSuccess) {
                                rawValue=resultDouble.Content;
                                success=true;
                                }
                            break;
                        default:
                            _lastError=$"不支持的数据类型: {dataType}";
                            return false;
                        }

                    if (!success) {
                        _lastError=$"读取地址 {address} 失败";
                        return false;
                        }

                    // 应用缩放系数
                    if (power!=1) {
                        if (dataType==TypeCode.Int16||dataType==TypeCode.Int32) {
                            rawValue=Convert.ToInt32(rawValue)/power;
                            }
                        else if (dataType==TypeCode.Single) {
                            rawValue=Convert.ToSingle(rawValue)/power;
                            }
                        else if (dataType==TypeCode.Double) {
                            rawValue=Convert.ToDouble(rawValue)/power;
                            }
                        }

                    values[originalIndex]=(T) Convert.ChangeType(rawValue,typeof(T));
                    }
                catch (Exception ex) {
                    _lastError=$"读取地址 {address} 异常: {ex.Message}";
                    return false;
                    }
                }
            return true;
            }
        #endregion
        #region 通用的批量写入方法
        /// <summary>
        /// 通用的批量写入方法（支持power参数）
        /// </summary>
        private bool BatchWriteByType<T>( List<string> addresses,T[] values,TypeCode dataType,int power,
            Func<ModbusTcpNet,string,T[],HslCommunication.OperateResult> bulkWriteFunc ) {
            if (!_IsConnected||addresses==null||values==null||addresses.Count!=values.Length)
                return false;

            var addressGroups = GroupContinuousAddresses(addresses, dataType);

            _lock.EnterWriteLock( );
            try {
                foreach (var group in addressGroups) {
                    if (ShouldUseBulkRead(group)) {
                        if (!BulkWriteGroup(group,addresses,values,power,dataType, bulkWriteFunc))
                            return false;
                        }
                    else {
                        if (!SingleWriteGroup(group,addresses,values,dataType,power))
                            return false;
                        }
                    }

                OnDataWriteCompleted(new DataWriteEventArgs($"BatchWrite{dataType}",addresses,values,true));
                return true;
                }
            catch (Exception ex) {
                HandleWriteError($"BatchWrite{dataType}",addresses,values,ex);
                return false;
                }
            finally {
                _lock.ExitWriteLock( );
                }
            }

        private bool BulkWriteGroup<T>( AddressGroup group,List<string> addresses,T[] values,int power,TypeCode dataType,
            Func<ModbusTcpNet,string,T[],HslCommunication.OperateResult> bulkWriteFunc ) {
            try {
                T[] writeData = new T[group.Length];
                for (int i = 0; i<group.OriginalIndices.Count; i++) {
                    int originalIndex = group.OriginalIndices[i];
                    T value = values[originalIndex];

                    // 应用缩放系数
                    if (power!=1) {
                        if (dataType==TypeCode.Int16) {
                            value=(T) (object) ( Convert.ToInt16(value)*power );
                            }
                        else if (dataType==TypeCode.Int32) {
                            value=(T) (object) ( Convert.ToInt32(value)*power );
                            }
                        else if (dataType==TypeCode.Single) {
                            value=(T) (object) ( Convert.ToSingle(value)*power );
                            }
                        else if (dataType==TypeCode.Double) {
                            value=(T) (object) ( Convert.ToDouble(value)*power );
                            }
                        }

                    writeData[i]=value;
                    }

                var result = bulkWriteFunc(_modbusClient, group.StartAddress.ToString(), writeData);
                if (!result.IsSuccess) {
                    _lastError=result.Message;
                    return false;
                    }

                return true;
                }
            catch (Exception ex) {
                _lastError=$"批量写入{_dataTypeInfo[group.DataType].Description}地址 {group.StartAddress}-{group.EndAddress} 失败: {ex.Message}";
                return false;
                }
            }
        private bool SingleWriteGroup<T>( AddressGroup group,List<string> addresses,T[] values,TypeCode dataType,int power ) {
            foreach (int originalIndex in group.OriginalIndices) {
                bool success = false;
                string address = addresses[originalIndex];
                T value = values[originalIndex];

                try {
                    // 应用缩放系数
                    object scaledValue = value;
                    if (power!=1) {
                        if (dataType==TypeCode.Int16) {
                            scaledValue=Convert.ToInt16(value)*power;
                            }
                        else if (dataType==TypeCode.Int32) {
                            scaledValue=Convert.ToInt32(value)*power;
                            }
                        else if (dataType==TypeCode.Single) {
                            scaledValue=Convert.ToSingle(value)*power;
                            }
                        else if (dataType==TypeCode.Double) {
                            scaledValue=Convert.ToDouble(value)*power;
                            }
                        }

                    switch (dataType) {
                        case TypeCode.Int16:
                            var result16 = _modbusClient.Write(address, (short)Convert.ToInt32(scaledValue));
                            success=result16.IsSuccess;
                            break;
                        case TypeCode.Int32:
                            var result32 = _modbusClient.Write(address, Convert.ToInt32(scaledValue));
                            success=result32.IsSuccess;
                            break;
                        case TypeCode.Single:
                            var resultFloat = _modbusClient.Write(address, Convert.ToSingle(scaledValue));
                            success=resultFloat.IsSuccess;
                            break;
                        case TypeCode.Double:
                            var resultDouble = _modbusClient.Write(address, Convert.ToDouble(scaledValue));
                            success=resultDouble.IsSuccess;
                            break;
                        default:
                            _lastError=$"不支持的数据类型: {dataType}";
                            return false;
                        }

                    if (!success) {
                        _lastError=$"写入地址 {address} 失败";
                        return false;
                        }
                    }
                catch (Exception ex) {
                    _lastError=$"写入地址 {address} 异常: {ex.Message}";
                    return false;
                    }
                }
            return true;
            }
        #endregion
        #region 自动读取系统
        private Dictionary<TypeCode, Func<AutoReadConfig, bool>> _autoReadHandlers;

        private void InitializeAutoReadHandlers( ) {
            _autoReadHandlers=new Dictionary<TypeCode,Func<AutoReadConfig,bool>>
                {
                [TypeCode.Int16]=config => {
                    int value = 0;
                    return ReadRegister(config.Addresses[0],ref value,config.Power);
                },
                [TypeCode.Int32]=config => {
                    int value = 0;
                    return ReadInt32(config.Addresses[0],ref value,config.Power);
                },
                [TypeCode.Single]=config => {
                    float value = 0;
                    return ReadFloat(config.Addresses[0],ref value,config.Power);
                },
                [TypeCode.Double]=config => {
                    double value = 0;
                    return ReadDouble(config.Addresses[0],ref value,config.Power);
                },
                [TypeCode.Boolean]=config => {
                    bool value = false;
                    if (config.Addresses[0].Contains('.'))
                        return ReadRegisterBit(config.Addresses[0],ref value);
                    else
                        return ReadCoil(config.Addresses[0],ref value);
                },
                [TypeCode.String]=config => {
                    string text = null;
                    return ReadString(config.Addresses[0],ref text,config.MaxLength);
                }
                };
            }

        public void StartAutoRead( ) {
            _lock.EnterWriteLock( );
            try {
                // 检查定时器是否已经存在并且已经订阅了事件
                if (_autoReadTimer!=null&&_autoReadTimer.Enabled)
                    return;

                // 如果定时器存在但未启用，先清理资源
                if (_autoReadTimer!=null) {
                    _autoReadTimer.Elapsed-=OnAutoReadCheck; // 确保取消之前的订阅
                    _autoReadTimer.Dispose( );
                    _autoReadTimer=null;
                    }

                _autoReadTimer=new Timer(100);
                _autoReadTimer.Elapsed+=OnAutoReadCheck;
                _autoReadTimer.AutoReset=true;
                _autoReadTimer.Start( );
                RecalculateNextReadTime( );
                }
            finally {
                _lock.ExitWriteLock( );
                }
            }

        public void StopAutoRead( ) {
            _lock.EnterWriteLock( );
            try {
                if (_autoReadTimer!=null) {
                    _autoReadTimer.Stop( );
                    _autoReadTimer.Dispose( );
                    _autoReadTimer=null;
                    }

                if (_autoReadConfigs!=null) {
                    _autoReadConfigs.Clear( );
                    }

                _nextReadTime=DateTime.MaxValue;
                }
            finally {
                _lock.ExitWriteLock( );
                }
            }

        private void OnAutoReadCheck( object sender,ElapsedEventArgs e ) {
            if (!_IsConnected)
                return;

            var now = DateTime.Now;
            bool anyRead = false;

            foreach (var group in _autoReadGroups.Values) {
                if (now>=group.NextReadTime) {
                    ThreadPool.QueueUserWorkItem(state => ProcessAutoReadGroup(group));
                    anyRead=true;

                    // 更新组的下次读取时间
                    group.NextReadTime=now.AddMilliseconds(group.ReadIntervalMs);
                    }
                }

            if (anyRead) {
                RecalculateNextReadTime( );
                }
            }

        private bool ProcessAutoReadGroup( AutoReadGroup group ) {
            try {
                // 收集组内所有地址
                var allAddresses = group.AllAddresses;
                if (allAddresses.Count==0)
                    return false;

                // 检查地址连续性
                if (IsContinuousAddresses(allAddresses,group.DataType)&&allAddresses.Count>1) {
                    // 使用批量读取
                    return BulkReadAutoReadGroup(group,allAddresses);
                    }
                else {
                    // 逐个读取
                    return SingleReadAutoReadGroup(group);
                    }
                }
            catch (Exception ex) {
                LogException(ex,$"ProcessAutoReadGroup({group.DataType})");
                return false;
                }
            }

        private bool BulkReadAutoReadGroup( AutoReadGroup group,List<string> addresses ) {
            try {
                bool success = false;

                switch (group.DataType) {
                    case TypeCode.Int16:
                        int[] int16Values = new int[addresses.Count];
                        success=BatchReadInt(addresses,ref int16Values,GetAveragePower(group.Configs));
                        if (success) {
                            DistributeBulkReadResults(group,addresses,int16Values);
                            }
                        break;

                    case TypeCode.Int32:
                        int[] int32Values = new int[addresses.Count];
                        success=BatchReadInt32(addresses,ref int32Values,GetAveragePower(group.Configs));
                        if (success) {
                            DistributeBulkReadResults(group,addresses,int32Values);
                            }
                        break;

                    case TypeCode.Single:
                        float[] floatValues = new float[addresses.Count];
                        success=BatchReadFloat(addresses,ref floatValues,GetAveragePower(group.Configs));
                        if (success) {
                            DistributeBulkReadResults(group,addresses,floatValues);
                            }
                        break;

                    case TypeCode.Double:
                        double[] doubleValues = new double[addresses.Count];
                        success=BatchReadDouble(addresses,ref doubleValues,GetAveragePower(group.Configs));
                        if (success) {
                            DistributeBulkReadResults(group,addresses,doubleValues);
                            }
                        break;

                    default:
                        // 不支持批量读取的数据类型，回退到逐个读取
                        return SingleReadAutoReadGroup(group);
                    }

                return success;
                }
            catch (Exception ex) {
                LogException(ex,"BulkReadAutoReadGroup");
                return false;
                }
            }

        private int GetAveragePower( List<AutoReadConfig> configs ) {
            if (configs.Count==0)
                return 1;
            return (int) configs.Average(c => c.Power);
            }

        private void DistributeBulkReadResults<T>( AutoReadGroup group,List<string> addresses,T[] values ) {
            int addressIndex = 0;

            foreach (var config in group.Configs) {
                foreach (var address in config.Addresses) {
                    if (addressIndex<values.Length) {
                        // 触发数据读取事件
                        OnDataRead(new DataReadEventArgs(address,values[addressIndex],group.DataType,IPEnd));
                        addressIndex++;
                        }
                    }

                // 更新配置的最后读取时间
                config.LastReadTime=DateTime.Now;
                }
            }

        private bool SingleReadAutoReadGroup( AutoReadGroup group ) {
            bool overallSuccess = true;

            foreach (var config in group.Configs) {
                foreach (var address in config.Addresses) {
                    bool success = ProcessSingleAutoRead(address, config);
                    if (!success)
                        overallSuccess=false;
                    }

                config.LastReadTime=DateTime.Now;
                }

            return overallSuccess;
            }

        private bool ProcessSingleAutoRead( string address,AutoReadConfig config ) {
            try {
                bool success = false;
                object value = null;
                TypeCode valueType = config.DataType;

                switch (config.DataType) {
                    case TypeCode.Int16:
                        int int16Value = 0;
                        success=ReadRegister(address,ref int16Value,config.Power);
                        value=int16Value;
                        break;

                    case TypeCode.Int32:
                        int int32Value = 0;
                        success=ReadInt32(address,ref int32Value,config.Power);
                        value=int32Value;
                        break;

                    case TypeCode.Single:
                        float floatValue = 0;
                        success=ReadFloat(address,ref floatValue,config.Power);
                        value=floatValue;
                        break;

                    case TypeCode.Double:
                        double doubleValue = 0;
                        success=ReadDouble(address,ref doubleValue,config.Power);
                        value=doubleValue;
                        break;

                    case TypeCode.Boolean:
                        bool boolValue = false;
                        if (address.Contains('.')) {
                            // 寄存器位读取
                            success=ReadRegisterBit(address,ref boolValue);
                            }
                        else {
                            // 线圈读取
                            success=ReadCoil(address,ref boolValue);
                            }
                        value=boolValue;
                        break;

                    case TypeCode.String:
                        string stringValue = null;
                        success=ReadString(address,ref stringValue,config.MaxLength);
                        value=stringValue;
                        break;

                    default:
                        LoggingService.Error($"不支持的自动读取数据类型: {config.DataType}");
                        return false;
                    }

                if (success) {
                    // 触发数据读取事件
                    OnDataRead(new DataReadEventArgs(address,value,valueType,IPEnd));

                    // 记录调试信息
                    //if (WriteLog==null) {
                       // LoggingService.Error($"自动读取成功: 地址={address}, 值={value}, 类型={valueType}");
                       // }
                    }
                else {
                    LoggingService.Error($"自动读取失败，地址: {address}, 数据类型: {config.DataType}");

                    // 触发错误事件
                    OnDataWriteCompleted(new DataWriteEventArgs(
                        "AutoRead",
                        new List<string> { address },
                        null,
                        false,
                        $"自动读取失败: {_lastError}"
                    ));
                    }

                return success;
                }
            catch (Exception ex) {
                LogException(ex,$"ProcessSingleAutoRead({address})");

                // 触发错误事件
                OnDataWriteCompleted(new DataWriteEventArgs(
                    "AutoRead",
                    new List<string> { address },
                    null,
                    false,
                    $"自动读取异常: {ex.Message}"
                ));

                return false;
                }
            }


        public bool AddOrUpdateAutoRead( string key,string address,TypeCode dataType,
                                      int readIntervalMs = 1000,int power = 1,int length = 1,
                                      int maxLength = 20,bool isBulkRead = true ) {
            try {
                var config = new AutoReadConfig
                    {
                    Key = key,
                    Addresses = new List<string> { address },
                    DataType = dataType,
                    ReadIntervalMs = readIntervalMs,
                    Power = power,
                    Length = length,
                    MaxLength = maxLength,
                    IsBulkRead = isBulkRead,
                    LastReadTime = DateTime.Now.AddMilliseconds(-readIntervalMs)
                    };

                _autoReadConfigs.AddOrUpdate(key,config,( k,oldValue ) => config);
                // 添加到配置字典

                // 重新分组
                RegroupAutoReadConfigs();

                RecalculateNextReadTime( );
                return true;
                }
            catch (Exception ex) {
                LogException(ex,$"AddOrUpdateAutoRead({key})");
                return false;
                }
            }

        public bool AddOrUpdateAutoRead( string key,List<string> addresses,TypeCode dataType,
                               int readIntervalMs = 1000,int power = 1,int length = 1,
                               int maxLength = 10,bool isBulkRead = true ) {
            try {
                var config = new AutoReadConfig
                    {
                    Key = key,
                    Addresses = addresses,
                    DataType = dataType,
                    ReadIntervalMs = readIntervalMs,
                    Power = power,
                    Length = length,
                    MaxLength = maxLength,
                    IsBulkRead = isBulkRead,
                    LastReadTime = DateTime.Now.AddMilliseconds(-readIntervalMs)
                    };

                // 添加到配置字典
                _autoReadConfigs.AddOrUpdate(key,config,( k,oldValue ) => config);

                // 重新分组
                RegroupAutoReadConfigs( );
                RecalculateNextReadTime( );
                return true;
                }
            catch (Exception ex) {
                LogException(ex,$"AddOrUpdateAutoRead({key})");
                return false;
                }
            }

        private void RegroupAutoReadConfigs( ) {
            _autoReadGroups.Clear( );

            foreach (var config in _autoReadConfigs.Values) {
                string groupKey = GenerateGroupKey(config);

                if (!_autoReadGroups.TryGetValue(groupKey,out var group)) {
                    group=new AutoReadGroup
                        {
                        DataType=config.DataType,
                        ReadIntervalMs=config.ReadIntervalMs
                        };
                    _autoReadGroups[groupKey]=group;
                    }

                group.Configs.Add(config);

                // 计算下一次读取时间（取组内最早需要读取的时间）
                DateTime nextRead = config.LastReadTime.AddMilliseconds(config.ReadIntervalMs);
                if (nextRead<group.NextReadTime||group.NextReadTime==default) {
                    group.NextReadTime=nextRead;
                    }
                }
            }

        public bool RemoveAutoRead( string key ) {
            bool result = _autoReadConfigs.TryRemove(key, out _);
            if (result) {
                RecalculateNextReadTime( );
                }
            return result;
            }

        public bool RemoveAutoReadAll( ) {
            try {
                _autoReadConfigs.Clear( );
                RecalculateNextReadTime( );
                return true;
                }
            catch (Exception ex) {
                LogException(ex,"RemoveAutoReadAll");
                return false;
                }
            }

        public List<string> GetAutoReadKeys( ) {
            return _autoReadConfigs.Keys.ToList( );
            }

        public AutoReadConfig GetAutoReadConfig( string key ) {
            return _autoReadConfigs.TryGetValue(key,out var config) ? config : null;
            }

        public bool ExecuteAutoReadNow( string key ) {
            if (!_autoReadConfigs.TryGetValue(key,out var config))
                return false;

            return ProcessAutoReadConfig(config);
            }

        private bool ProcessAutoReadConfig( AutoReadConfig config ) {
            try {
                if (config.Addresses.Count==0)
                    return false;

                if (config.Addresses.Count==1) {
                    if (_autoReadHandlers.TryGetValue(config.DataType,out var handler)) {
                        return handler(config);
                        }
                    }
                else {
                    // 批量读取处理
                    foreach (var address in config.Addresses) {
                        if (config.DataType==TypeCode.Int16) {
                            int value = 0;
                            if (!ReadRegister(address,ref value,config.Power))
                                return false;
                            }
                        else if (config.DataType==TypeCode.Int32) {
                            int value = 0;
                            if (!ReadInt32(address,ref value,config.Power))
                                return false;
                            }
                        else if (config.DataType==TypeCode.Single) {
                            float value = 0;
                            if (!ReadFloat(address,ref value,config.Power))
                                return false;
                            }
                        else if (config.DataType==TypeCode.Double) {
                            double value = 0;
                            if (!ReadDouble(address,ref value,config.Power))
                                return false;
                            }
                        else if (config.DataType==TypeCode.Boolean) {
                            bool value = false;
                            if (address.Contains('.')) {
                                if (!ReadRegisterBit(address,ref value))
                                    return false;
                                }
                            else {
                                if (!ReadCoil(address,ref value))
                                    return false;
                                }
                            }
                        else if (config.DataType==TypeCode.String) {
                            string text = null;
                            if (!ReadString(address,ref text,config.MaxLength))
                                return false;
                            }
                        }
                    return true;
                    }

                return false;
                }
            catch (Exception ex) {
                LogException(ex,$"AutoRead for {config.Key}");
                return false;
                }
            }
        private void RecalculateNextReadTime( ) {
            if (_autoReadGroups.Count==0) {
                _nextReadTime=DateTime.MaxValue;
                return;
                }

            DateTime minNextTime = DateTime.MaxValue;
            foreach (var group in _autoReadGroups.Values) {
                if (group.NextReadTime<minNextTime) {
                    minNextTime=group.NextReadTime;
                    }
                }

            _nextReadTime=minNextTime;

            // 动态调整定时器间隔
            if (_autoReadTimer!=null&&_nextReadTime!=DateTime.MaxValue) {
                double millisecondsUntilNextRead = (_nextReadTime - DateTime.Now).TotalMilliseconds;
                if (millisecondsUntilNextRead>0) {
                    _autoReadTimer.Interval=Math.Max(100,millisecondsUntilNextRead);
                    }
                }
            }

        #endregion

        #region 心跳控制方法
        public void StartHeartbeatService( ) {
            if (_heartbeatAddress==0) {
                WriteLog("警告: 未设置心跳地址，使用默认地址100");
                _heartbeatAddress=100;
                }

            HeartbeatEnabled=true;
            _heartbeatTimer?.Start( );
            _currentRetryCount=0;
            }

        public void StopHeartbeatService( ) {
            HeartbeatEnabled=false;
            _heartbeatTimer?.Stop( );
            }

        public void SetHeartbeatAddress( string address ) {
            if (!_IsConnected||!TryParseAddress(address,out ushort modbusAddress))
                return;
            HeartbeatAddress=modbusAddress;
            }

        public void SetHeartbeatInterval( int intervalMs ) {
            HeartbeatInterval=intervalMs;
            }

        public HeartbeatStatus GetHeartbeatStatus( ) {
            return new HeartbeatStatus
                {
                IsEnabled=_heartbeatEnabled,
                Interval=_heartbeatInterval,
                LastRetryCount=_currentRetryCount,
                IsConnectionAlive=_IsConnected&&_currentRetryCount==0,
                LastCheckTime=DateTime.Now
                };
            }
        #endregion

        #region 辅助方法和资源管理
        private bool TryParseAddress( string deviceName,out ushort modbusAddress ) {
            modbusAddress=0;
            if (string.IsNullOrEmpty(deviceName))
                return false;

            if (ushort.TryParse(deviceName,out ushort address)) {
                modbusAddress=address;
                return true;
                }
            return false;
            }

        private bool TryParseCoilAddress( string deviceName,out ushort coilAddress ) {
            coilAddress=0;
            if (string.IsNullOrEmpty(deviceName))
                return false;

            if (deviceName.StartsWith("Y")&&ushort.TryParse(deviceName.Substring(1),out ushort address)) {
                coilAddress=address;
                return true;
                }
            return false;
            }

        private bool TryParseExtendedAddress( string deviceName,out ushort modbusAddress,out byte bitIndex ) {
            modbusAddress=0;
            bitIndex=0;

            if (string.IsNullOrEmpty(deviceName))
                return false;

            if (deviceName.Contains('.')) {
                string[] parts = deviceName.Split('.');
                if (parts.Length==2&&
                    ushort.TryParse(parts[0],out modbusAddress)&&
                    byte.TryParse(parts[1],out bitIndex)&&
                    bitIndex<16) {
                    return true;
                    }
                return false;
                }

            return TryParseAddress(deviceName,out modbusAddress);
            }

        private void LogException( Exception ex,string operation ) {
            LoggingService.Error($"CDeltaModbusTcp 错误于 {operation}",ex);
            }

        private void HandleReadError( Exception ex,string operation ) {
            _lastError=ex.Message;
            LoggingService.Error($"读取操作失败: {operation}",ex);
            }

        private void HandleWriteError( string operation,List<string> addresses,object values,Exception ex ) {
            _lastError=ex.Message;
            LoggingService.Error($"写入操作失败: {operation}",ex);
            OnDataWriteCompleted(new DataWriteEventArgs(operation,addresses,values,false,ex.Message));
            }

        protected virtual void Dispose( bool disposing ) {
            if (_disposed)
                return;

            if (disposing) {
                StopAutoRead( );
                StopHeartbeatService( );
                if (_lock.TryEnterWriteLock(TimeSpan.FromSeconds(2))) {
                    try {
                        CloseConnectionUnderLock( );
                        _modbusClient?.Dispose( );
                        }
                    finally {
                        _lock.ExitWriteLock( );
                        }
                    }
                _heartbeatTimer?.Dispose( );
                _autoReadTimer?.Dispose( );
                _lock.Dispose( );
                }

            _disposed=true;
            }

        public void Dispose( ) {
            Dispose(true);
            GC.SuppressFinalize(this);
            }

        ~CDeltaModbusTcp( ) {
            Dispose(false);
            }
        #endregion

        #region 数据类型步长定义
        private class DataTypeInfo {
            public TypeCode TypeCode {
                get; set;
                }
            public int RegisterCount {
                get; set;
                } // 占用的寄存器数量
            public string Description {
                get; set;
                }
            }

        private static readonly Dictionary<TypeCode, DataTypeInfo> _dataTypeInfo = new Dictionary<TypeCode, DataTypeInfo>
        {
            { TypeCode.Int16, new DataTypeInfo { TypeCode = TypeCode.Int16, RegisterCount = 1, Description = "16位整数" } },
            { TypeCode.Int32, new DataTypeInfo { TypeCode = TypeCode.Int32, RegisterCount = 2, Description = "32位整数" } },
            { TypeCode.Single, new DataTypeInfo { TypeCode = TypeCode.Single, RegisterCount = 2, Description = "单精度浮点数" } },
            { TypeCode.Double, new DataTypeInfo { TypeCode = TypeCode.Double, RegisterCount = 4, Description = "双精度浮点数" } },
            { TypeCode.Boolean, new DataTypeInfo { TypeCode = TypeCode.Boolean, RegisterCount = 1, Description = "布尔值" } },
            { TypeCode.String, new DataTypeInfo { TypeCode = TypeCode.String, RegisterCount = 2, Description = "字符串" } } // 每个字符占1个寄存器
        };

        private int GetRegisterCount( TypeCode dataType ) {
            return _dataTypeInfo.TryGetValue(dataType,out var info) ? info.RegisterCount : 1;
            }
        #endregion

        #region 地址连续性判断辅助类（按数据类型区分）
        public class AddressGroup {
            public ushort StartAddress {
                get; set;
                }
            public ushort EndAddress {
                get; set;
                }
            public List<int> OriginalIndices { get; set; } = new List<int>( );
            public TypeCode DataType {
                get; set;
                }
            public int RegisterCount {
                get; set;
                }
            public int Length => ( EndAddress-StartAddress )/RegisterCount+1;
            public int TotalRegisters => Length*RegisterCount;
            }

        /// <summary>
        /// 根据数据类型将地址列表分组为连续地址块
        /// </summary>
        public List<AddressGroup> GroupContinuousAddresses( List<string> addresses,TypeCode dataType ) {
            var groups = new List<AddressGroup>();

            if (addresses==null||addresses.Count==0)
                return groups;

            int registerCount = GetRegisterCount(dataType);

            // 解析所有地址并排序
            var parsedAddresses = new List<(ushort address, int index)>();
            for (int i = 0; i<addresses.Count; i++) {
                if (TryParseAddress(addresses[i],out ushort modbusAddress)) {
                    parsedAddresses.Add((modbusAddress, i));
                    }
                }

            if (parsedAddresses.Count==0)
                return groups;

            // 按地址排序
            parsedAddresses=parsedAddresses.OrderBy(x => x.address).ToList( );

            // 分组连续地址（考虑数据类型占用的寄存器数量）
            AddressGroup currentGroup = null;
            foreach (var item in parsedAddresses) {
                if (currentGroup==null) {
                    currentGroup=new AddressGroup
                        {
                        StartAddress=item.address,
                        EndAddress=(ushort) ( item.address+registerCount-1 ),
                        OriginalIndices={ item.index },
                        DataType=dataType,
                        RegisterCount=registerCount
                        };
                    }
                else {
                    // 计算期望的下一个起始地址
                    ushort expectedNextAddress = (ushort)(currentGroup.EndAddress + 1);

                    if (item.address==expectedNextAddress) {
                        // 地址连续，扩展当前组
                        currentGroup.EndAddress=(ushort) ( item.address+registerCount-1 );
                        currentGroup.OriginalIndices.Add(item.index);
                        }
                    else {
                        // 地址不连续，创建新组
                        groups.Add(currentGroup);
                        currentGroup=new AddressGroup
                            {
                            StartAddress=item.address,
                            EndAddress=(ushort) ( item.address+registerCount-1 ),
                            OriginalIndices={ item.index },
                            DataType=dataType,
                            RegisterCount=registerCount
                            };
                        }
                    }
                }

            if (currentGroup!=null) {
                groups.Add(currentGroup);
                }

            return groups;
            }

        /// <summary>
        /// 判断是否应该使用批量读取（考虑数据类型和连续地址数量）
        /// </summary>
        private bool ShouldUseBulkRead( AddressGroup group ) {
            // 根据数据类型和连续数量动态判断
            // 对于占用寄存器多的数据类型，可以降低阈值
            int threshold = group.RegisterCount > 2 ? 2 : 3;
            return group.Length>=threshold;
            }
        #endregion

        #region 字符串顺序修正方法
        /// <summary>
        /// 修正读取的字符串顺序
        /// </summary>
        private string FixStringOrder( string originalString ) {
            if (string.IsNullOrEmpty(originalString))
                return originalString;

            switch (_stringByteOrder) {
                case StringByteOrderEnum.Normal:
                    return originalString;

                case StringByteOrderEnum.RegisterByteSwap:
                    return SwapBytesInString(originalString);

                case StringByteOrderEnum.WordSwap:
                    return SwapWordsInString(originalString);

                case StringByteOrderEnum.FullSwap:
                    return SwapBytesInString(SwapWordsInString(originalString));

                default:
                    return originalString;
                }
            }

        /// <summary>
        /// 反转字符串顺序以便写入
        /// </summary>
        private string ReverseStringOrder( string text ) {
            if (string.IsNullOrEmpty(text))
                return text;

            switch (_stringByteOrder) {
                case StringByteOrderEnum.Normal:
                    return text;

                case StringByteOrderEnum.RegisterByteSwap:
                    return SwapBytesInString(text); // 交换操作是对称的

                case StringByteOrderEnum.WordSwap:
                    return SwapWordsInString(text); // 交换操作是对称的

                case StringByteOrderEnum.FullSwap:
                    return SwapBytesInString(SwapWordsInString(text)); // 交换操作是对称的

                default:
                    return text;
                }
            }

        /// <summary>
        /// 寄存器内字节交换（每2个字符交换位置）
        /// 示例： "ABCD" -> "BADC"
        /// </summary>
        private string SwapBytesInString( string str ) {
            if (string.IsNullOrEmpty(str)||str.Length<2)
                return str;

            char[] chars = str.ToCharArray();
            for (int i = 0; i<chars.Length; i+=2) {
                if (i+1<chars.Length) {
                    char temp = chars[i];
                    chars[i]=chars[i+1];
                    chars[i+1]=temp;
                    }
                }
            return new string(chars);
            }

        /// <summary>
        /// 寄存器间交换（每4个字符交换前2个和后2个）
        /// 示例： "ABCD" -> "CDAB"
        /// </summary>
        private string SwapWordsInString( string str ) {
            if (string.IsNullOrEmpty(str)||str.Length<4)
                return str;

            char[] chars = str.ToCharArray();
            for (int i = 0; i<chars.Length; i+=4) {
                if (i+3<chars.Length) {
                    // 交换前2个字符和后2个字符
                    char temp1 = chars[i];
                    char temp2 = chars[i + 1];
                    chars[i]=chars[i+2];
                    chars[i+1]=chars[i+3];
                    chars[i+2]=temp1;
                    chars[i+3]=temp2;
                    }
                }
            return new string(chars);
            }
        #endregion

        #region 端序测试和诊断
        public bool TestByteOrder( string testAddress = "0" ) {
            LoggingService.Info($"开始端序测试，地址: {testAddress}, 当前端序: {_byteOrder}");

            if (!_IsConnected||!TryParseAddress(testAddress,out ushort modbusAddress)) {
                LoggingService.Error("端序测试失败: PLC未连接或地址解析失败");
                return false;
                }

            var sb = new StringBuilder();
            sb.AppendLine($"端序测试开始 (地址: {testAddress}, 端序: {_byteOrder})");
            bool allTestsPassed = true;

            try {
                // 测试 int16
                short testValue16 = 0x1234;
                if (!WriteRegister(testAddress,testValue16)) {
                    sb.AppendLine("16位整数写入失败");
                    allTestsPassed=false;
                    }
                else {
                    int readValue16 = 0;
                    if (!ReadRegister(testAddress,ref readValue16)) {
                        sb.AppendLine("16位整数读取失败");
                        allTestsPassed=false;
                        }
                    else {
                        bool success16 = (short)readValue16 == testValue16;
                        sb.AppendLine($"16位整数测试: {( success16 ? "通过" : "失败" )} (写入: 0x{testValue16:X4}, 读取: 0x{readValue16:X4})");
                        allTestsPassed&=success16;
                        }
                    }

                // 测试 int32
                int testValue32 = 0x12345678;
                if (!WriteInt32(testAddress,testValue32)) {
                    sb.AppendLine("32位整数写入失败");
                    allTestsPassed=false;
                    }
                else {
                    int readValue32 = 0;
                    if (!ReadInt32(testAddress,ref readValue32)) {
                        sb.AppendLine("32位整数读取失败");
                        allTestsPassed=false;
                        }
                    else {
                        bool success32 = readValue32 == testValue32;
                        sb.AppendLine($"32位整数测试: {( success32 ? "通过" : "失败" )} (写入: 0x{testValue32:X8}, 读取: 0x{readValue32:X8})");
                        allTestsPassed&=success32;
                        }
                    }

                // 测试 float
                float testValueFloat = 123.456f;
                if (!WriteFloat(testAddress,testValueFloat)) {
                    sb.AppendLine("32位浮点数写入失败");
                    allTestsPassed=false;
                    }
                else {
                    float readValueFloat = 0;
                    if (!ReadFloat(testAddress,ref readValueFloat)) {
                        sb.AppendLine("32位浮点数读取失败");
                        allTestsPassed=false;
                        }
                    else {
                        bool successFloat = Math.Abs(testValueFloat - readValueFloat) < 0.001f;
                        sb.AppendLine($"32位浮点数测试: {( successFloat ? "通过" : "失败" )} (写入: {testValueFloat:F4}, 读取: {readValueFloat:F4})");
                        allTestsPassed&=successFloat;
                        }
                    }

                // 测试 double
                double testValueDouble = 123456.789;
                int power = 1000;
                if (!WriteDouble(testAddress,testValueDouble,power)) {
                    sb.AppendLine("64位双精度数写入失败");
                    allTestsPassed=false;
                    }
                else {
                    double readValueDouble = 0;
                    if (!ReadDouble(testAddress,ref readValueDouble,power)) {
                        sb.AppendLine("64位双精度数读取失败");
                        allTestsPassed=false;
                        }
                    else {
                        bool successDouble = Math.Abs(testValueDouble - readValueDouble) < 0.001;
                        sb.AppendLine($"64位双精度数测试: {( successDouble ? "通过" : "失败" )} (写入: {testValueDouble:F4}, 读取: {readValueDouble:F4})");
                        allTestsPassed&=successDouble;
                        }
                    }

                // 测试 string
                string testString = "ABCD";
                if (!WriteString(testAddress,testString)) {
                    sb.AppendLine("字符串写入失败");
                    allTestsPassed=false;
                    }
                else {
                    string readString = "";
                    if (!ReadString(testAddress,ref readString,testString.Length)) {
                        sb.AppendLine("字符串读取失败");
                        allTestsPassed=false;
                        }
                    else {
                        bool successString = readString == testString;
                        sb.AppendLine($"字符串测试: {( successString ? "通过" : "失败" )} (写入: \"{testString}\", 读取: \"{readString}\")");
                        allTestsPassed&=successString;
                        }
                    }

                // 测试 bool
                bool testValueBool = true;
                string bitAddress = $"{testAddress}.0";
                if (!WriteRegisterBit(bitAddress,testValueBool)) {
                    sb.AppendLine("位写入失败");
                    allTestsPassed=false;
                    }
                else {
                    bool readValueBool = false;
                    if (!ReadRegisterBit(bitAddress,ref readValueBool)) {
                        sb.AppendLine("位读取失败");
                        allTestsPassed=false;
                        }
                    else {
                        bool successBool = readValueBool == testValueBool;
                        sb.AppendLine($"位测试: {( successBool ? "通过" : "失败" )} (写入: {testValueBool}, 读取: {readValueBool})");
                        allTestsPassed&=successBool;
                        }
                    }
                LoggingService.Info("端序测试完成");
                }
            catch (Exception ex) {
                LoggingService.Error("端序测试异常",ex);
                allTestsPassed=false;
                }

            _lastError=sb.ToString( );
            WriteLog(_lastError);
            return allTestsPassed;
            }

        public string GetByteOrderInfo( ) {
            var sb = new StringBuilder();
            sb.AppendLine($"当前端序: {_byteOrder}");
            sb.AppendLine($"描述: {GetByteOrderDescription(_byteOrder)}");

            // 测试端序转换
            float testFloat = 123.456f;
            var result = _modbusClient.Write("0", testFloat);
            if (result.IsSuccess) {
                var registers = _modbusClient.ReadInt16("0", 2).Content;
                sb.AppendLine($"示例 float ({testFloat:F4}) 寄存器: [0x{registers[0]:X4}, 0x{registers[1]:X4}]");
                }

            return sb.ToString( );
            }

        private string GetByteOrderDescription( ByteOrderEnum order ) {
            switch (order) {
                case ByteOrderEnum.BigEndian:
                    return "大端序 (ABCD -> ABCD)";
                case ByteOrderEnum.LittleEndian:
                    return "小端序 (ABCD -> DCBA)";
                case ByteOrderEnum.BigEndianByteSwap:
                    return "大端序字节交换 (ABCD -> BADC)";
                case ByteOrderEnum.LittleEndianByteSwap:
                    return "小端序字节交换 (ABCD -> CDAB)";
                default:
                    return "未知";
                }
            }

        public string VerifyByteOrderSettings( ) {
            var sb = new StringBuilder();
            sb.AppendLine($"当前端序配置: {_byteOrder}");
            sb.AppendLine($"HSL DataFormat: {_modbusClient?.ByteTransform.DataFormat}");

            // 测试端序
            float testFloat = 123.456f;
            var result = _modbusClient.Write("0", testFloat);
            if (result.IsSuccess) {
                var registers = _modbusClient.ReadInt16("0", 2).Content;
                sb.AppendLine($"示例 float ({testFloat:F4}) 寄存器: [0x{registers[0]:X4}, 0x{registers[1]:X4}]");
                }

            return sb.ToString( );
            }

        public bool AutoDetectByteOrder( string testAddress = "0" ) {
            if (!_IsConnected||!TryParseAddress(testAddress,out ushort modbusAddress))
                return false;

            var originalOrder = _byteOrder;
            var orders = Enum.GetValues(typeof(ByteOrderEnum)).Cast<ByteOrderEnum>();
            var results = new Dictionary<ByteOrderEnum, string>();

            foreach (var order in orders) {
                ByteOrder=order;
                _lastError=string.Empty;
                if (TestByteOrder(testAddress)) {
                    results[order]=$"成功 (端序: {order})";
                    }
                else {
                    results[order]=$"失败: {_lastError}";
                    }
                }

            var successfulOrder = results.FirstOrDefault(r => r.Value.StartsWith("成功")).Key;
            if (successfulOrder!=default) {
                ByteOrder=successfulOrder;
                _lastError=$"自动检测到端序: {successfulOrder}\n测试结果:\n{string.Join("\n",results.Select(r => $"{r.Key}: {r.Value}"))}";
                WriteLog(_lastError);
                return true;
                }

            ByteOrder=originalOrder;
            _lastError=$"无法自动检测端序，请手动设置\n测试结果:\n{string.Join("\n",results.Select(r => $"{r.Key}: {r.Value}"))}";
            WriteLog(_lastError);
            return false;
            }
        #endregion

        private string GenerateGroupKey( AutoReadConfig config ) {
            // 按照扫描时间、地址类型和连续性进行分组
            return $"{config.DataType}_{config.ReadIntervalMs}";
            }

        private bool IsContinuousAddresses( List<string> addresses,TypeCode dataType ) {
            if (addresses.Count<=1)
                return true;

            var parsedAddresses = new List<ushort>();
            foreach (var address in addresses) {
                if (TryParseAddress(address,out ushort modbusAddress)) {
                    parsedAddresses.Add(modbusAddress);
                    }
                else {
                    return false;
                    }
                }

            // 排序并检查连续性
            parsedAddresses.Sort( );
            int registerCount = GetRegisterCount(dataType);

            for (int i = 1; i<parsedAddresses.Count; i++) {
                ushort expectedAddress = (ushort)(parsedAddresses[i - 1] + registerCount);
                if (parsedAddresses[i]!=expectedAddress) {
                    return false;
                    }
                }

            return true;
            }
        }
    }
