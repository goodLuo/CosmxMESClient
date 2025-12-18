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

namespace CosmxMESClient.Communication
{
    public class CDeltaModbusTcp : IPlcCommunication, IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CDeltaModbusTcp));

        #region 事件
        public event EventHandler<DataReadEventArgs> DataRead;
        public event EventHandler<DataWriteEventArgs> DataWriteCompleted;
        public event EventHandler<HeartbeatEventArgs> HeartbeatStatusChanged;
        public event Action<string> WriteLog;
        protected virtual void OnDataRead(DataReadEventArgs e)
        {
            DataRead?.Invoke(this, e);
        }

        protected virtual void OnDataWriteCompleted(DataWriteEventArgs e)
        {
            DataWriteCompleted?.Invoke(this, e);
        }

        protected virtual void OnHeartbeatStatusChanged(HeartbeatEventArgs e)
        {
            HeartbeatStatusChanged?.Invoke(this, e);
        }
        #endregion

        #region 自动读取
        private Timer _autoReadTimer;
        private ConcurrentDictionary<string, AutoReadConfig> _autoReadConfigs = new ConcurrentDictionary<string, AutoReadConfig>();
        private DateTime _nextReadTime = DateTime.MaxValue;
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
        public ByteOrderEnum ByteOrder
        {
            get => _byteOrder;
            set
            {
                _byteOrder = value;
                UpdateHslByteOrder();
            }
        }
        #endregion
        #region 字符串端序配置
        private StringByteOrderEnum _stringByteOrder = StringByteOrderEnum.RegisterByteSwap;

        public StringByteOrderEnum StringByteOrder
        {
            get => _stringByteOrder;
            set => _stringByteOrder = value;
        }
        #endregion
        // 连接参数
        private string _ipAddress;
        private int _port;
        private byte _slaveId;
        private int _timeout = 1000;

        public string IpAddress
        {
            get => _ipAddress;
            set
            {
                if (!_IsConnected)
                    _ipAddress = value;
            }
        }

        public IPEndPoint IPEnd
        {
            get => _localEndPoint;
            set
            {
                if (!_IsConnected)
                    _localEndPoint = value;
            }
        }

        public int Port
        {
            get => _port;
            set
            {
                if (!_IsConnected)
                    _port = value;
            }
        }

        public byte SlaveId
        {
            get => _slaveId;
            set
            {
                if (!_IsConnected)
                    _slaveId = value;
                if (_modbusClient != null)
                    _modbusClient.Station = value;
            }
        }

        public int Timeout
        {
            get => _timeout;
            set
            {
                _timeout = value;
                _lock.EnterWriteLock();
                try
                {
                    if (_modbusClient != null)
                    {
                        _modbusClient.ConnectTimeOut = value;
                        _modbusClient.ReceiveTimeOut = value;
                    }
                }
                finally
                {
                    _lock.ExitWriteLock();
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

        public int HeartbeatInterval
        {
            get => _heartbeatInterval;
            set
            {
                _heartbeatInterval = value;
                if (_heartbeatTimer != null)
                {
                    _heartbeatTimer.Interval = value;
                }
            }
        }

        public bool HeartbeatEnabled
        {
            get => _heartbeatEnabled;
            set => _heartbeatEnabled = value;
        }

        public ushort HeartbeatAddress
        {
            get => _heartbeatAddress;
            set => _heartbeatAddress = value;
        }

        bool IPlcCommunication.IsConnected => _IsConnected;
        private bool _IsConnected = _modbusClient != null ? _modbusClient.IpAddressPing() == IPStatus.Success : false;
        #endregion

        #region 构造函数
        public CDeltaModbusTcp(string ipAddress, int port, byte slaveId,
                              string ipAddressEnd = null, int portEnd = 0,
                              ByteOrderEnum byteOrder = ByteOrderEnum.BigEndian)
        {
            _ipAddress = ipAddress;
            _port = port;
            _slaveId = slaveId;
            _byteOrder = byteOrder;
            if (IPAddress.TryParse(ipAddress, out IPAddress _IPEnd))
            {
                IPEnd = new IPEndPoint(_IPEnd, port);
            }

            if (!string.IsNullOrEmpty(ipAddressEnd) && portEnd > 0)
            {
                if (IPAddress.TryParse(ipAddressEnd, out IPAddress localIP))
                {
                    _localEndPoint = new IPEndPoint(localIP, portEnd);
                }
            }
            InitializeAutoReadHandlers();
            InitializeHeartbeat();
        }

        public CDeltaModbusTcp(string ipAddress, int port, byte slaveId, bool isBigEndian)
            : this(ipAddress, port, slaveId, null, 0, isBigEndian ? ByteOrderEnum.BigEndian : ByteOrderEnum.LittleEndian)
        {
        }

        public CDeltaModbusTcp() : this("192.168.1.100", 502, 1) { }
        #endregion

        #region 心跳管理
        private void InitializeHeartbeat()
        {
            _heartbeatTimer = new Timer(_heartbeatInterval);
            _heartbeatTimer.Elapsed += OnHeartbeatElapsed;
            _heartbeatTimer.AutoReset = true;
            _heartbeatTimer.Enabled = _heartbeatEnabled;
        }

        private void OnHeartbeatElapsed(object sender, ElapsedEventArgs e)
        {
            if (!_IsConnected || !_heartbeatEnabled)
                return;

            ThreadPool.QueueUserWorkItem(state => ExecuteHeartbeat());
        }

        private void ExecuteHeartbeat()
        {
            try
            {
                _lock.EnterReadLock();
                try
                {
                    if (!_IsConnected)
                        return;
                    var result = _modbusClient.ReadInt16(_heartbeatAddress.ToString());
                    if (!result.IsSuccess)
                        throw new Exception(result.Message);
                }
                finally
                {
                    _lock.ExitReadLock();
                }

                _currentRetryCount = 0;
                OnHeartbeatStatusChanged(new HeartbeatEventArgs(true, "心跳检测成功", 0));
            }
            catch (Exception ex)
            {
                _currentRetryCount++;
                if (_currentRetryCount >= _heartbeatRetryCount)
                {
                    OnHeartbeatStatusChanged(new HeartbeatEventArgs(false,
                        $"心跳检测失败，连接可能已断开: {ex.Message}", _currentRetryCount));
                    //AttemptReconnect( );
                }
                else
                {
                    OnHeartbeatStatusChanged(new HeartbeatEventArgs(true,
                        $"心跳检测暂时失败 ({_currentRetryCount}/{_heartbeatRetryCount}): {ex.Message}",
                        _currentRetryCount));
                }
            }
        }

        private async void AttemptReconnect()
        {
            const int maxReconnectAttempts = 3;
            const int reconnectDelay = 3000; // 3秒

            for (int attempt = 1; attempt <= maxReconnectAttempts; attempt++)
            {
                try
                {
                    _lock.EnterWriteLock();
                    try
                    {
                        CloseConnectionUnderLock();
                        await Task.Delay(reconnectDelay);

                        if (InitializeUnderLock())
                        {
                            if (TestConnection())
                            {
                                _currentRetryCount = 0;
                                OnHeartbeatStatusChanged(new HeartbeatEventArgs(true,
                                    $"连接自动恢复成功 (第{attempt}次尝试)", 0));
                                return;
                            }
                        }
                    }
                    finally
                    {
                        _lock.ExitWriteLock();
                    }
                }
                catch (Exception ex)
                {
                    OnHeartbeatStatusChanged(new HeartbeatEventArgs(false,
                        $"自动重连失败 (第{attempt}/{maxReconnectAttempts}次): {ex.Message}",
                        _currentRetryCount));
                }
            }
        }

        private bool TestConnection()
        {
            try
            {
                var result = _modbusClient.ReadInt16(_heartbeatAddress.ToString());
                return result.IsSuccess;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 连接管理
        public bool Initialize()
        {
            LoggingService.Info($"开始初始化PLC连接: {_ipAddress}:{_port}, 站号: {_slaveId}");

            _lock.EnterWriteLock();
            try
            {
                return InitializeUnderLock();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        private bool InitializeUnderLock()
        {
            try
            {
                if (_IsConnected)
                {
                    LoggingService.Info("PLC连接已存在，先关闭现有连接");
                    CloseUnderLock();
                }

                _modbusClient = new ModbusTcpNet(_ipAddress, _port, _slaveId);
                _modbusClient.ConnectTimeOut = _timeout;
                _modbusClient.ReceiveTimeOut = _timeout;
                UpdateHslByteOrder();

                if (_localEndPoint != null)
                {
                    // HSL 不直接支持绑定本地端点，可能需自定义 TcpClient
                    // 这里暂时忽略，如需支持需扩展
                }

                var result = _modbusClient.ConnectServer();
                if (!result.IsSuccess)
                {
                    LoggingService.Error($"PLC连接失败: {result.Message}");
                    _lastError = result.Message;
                    return false;
                }


                _IsConnected = true;
                _lastError = string.Empty;

                if (_heartbeatAddress > 0 && _heartbeatEnabled)
                {
                    StartHeartbeatService();
                }
                LoggingService.PLCCommunication($"PLC连接成功: {_ipAddress}:{_port}");
                return true;
            }
            catch (Exception ex)
            {
                LoggingService.Error("PLC连接初始化异常", ex);
                _lastError = ex.Message;
                return false;
            }
        }

        private void UpdateHslByteOrder()
        {
            if (_modbusClient != null)
            {
                switch (_byteOrder)
                {
                    case ByteOrderEnum.BigEndian:
                        _modbusClient.ByteTransform.DataFormat = HslCommunication.Core.DataFormat.ABCD;
                        break;
                    case ByteOrderEnum.LittleEndian:
                        _modbusClient.ByteTransform.DataFormat = HslCommunication.Core.DataFormat.DCBA;
                        break;
                    case ByteOrderEnum.BigEndianByteSwap:
                        _modbusClient.ByteTransform.DataFormat = HslCommunication.Core.DataFormat.BADC;
                        break;
                    case ByteOrderEnum.LittleEndianByteSwap:
                        _modbusClient.ByteTransform.DataFormat = HslCommunication.Core.DataFormat.CDAB;
                        break;
                }
            }
        }

        public void Close()
        {
            _lock.EnterWriteLock();
            try
            {
                CloseUnderLock();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        private void CloseUnderLock()
        {
            try
            {
                StopHeartbeatService();
                CloseConnectionUnderLock();
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                LogException(ex, "Close");
            }
        }

        private void CloseConnectionUnderLock()
        {
            _modbusClient?.ConnectClose();
            _modbusClient = null;
            // _IsConnected=false;
            _lastError = string.Empty;
        }
        #endregion

        #region 基础数据读写
        public bool ReadRegister(string address, ref int value)
        {
            if (!_IsConnected || !TryParseAddress(address, out ushort modbusAddress))
                return false;

            _lock.EnterReadLock();
            try
            {
                var result = _modbusClient.ReadInt16(modbusAddress.ToString());
                if (!result.IsSuccess)
                {
                    _lastError = result.Message;
                    return false;
                }
                value = result.Content;
            }
            catch (Exception ex)
            {
                HandleReadError(ex, $"ReadRegister({address})");
                return false;
            }
            finally
            {
                _lock.ExitReadLock();
            }

            _lastError = string.Empty;
            OnDataRead(new DataReadEventArgs(address, value, TypeCode.Int16, IPEnd));
            return true;
        }

        public bool WriteRegister(string address, int value)
        {
            if (!_IsConnected || !TryParseAddress(address, out ushort modbusAddress))
                return false;

            if (value < short.MinValue || value > short.MaxValue)
            {
                _lastError = $"Value {value} is out of short range";
                return false;
            }

            _lock.EnterWriteLock();
            try
            {
                var result = _modbusClient.Write(modbusAddress.ToString(), (short)value);
                if (!result.IsSuccess)
                {
                    _lastError = result.Message;
                    return false;
                }
                _lastError = string.Empty;
            }
            catch (Exception ex)
            {
                HandleWriteError("WriteRegister", new List<string> { address }, value, ex);
                return false;
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            OnDataWriteCompleted(new DataWriteEventArgs("WriteRegister", new List<string> { address }, value, true));
            return true;
        }

        public bool ReadInt32(string address, ref int value)
        {
            if (!_IsConnected || !TryParseAddress(address, out ushort modbusAddress))
                return false;

            _lock.EnterReadLock();
            try
            {
                var result = _modbusClient.ReadInt32(modbusAddress.ToString());
                if (!result.IsSuccess)
                {
                    _lastError = result.Message;
                    return false;
                }
                value = result.Content;
            }
            catch (Exception ex)
            {
                HandleReadError(ex, $"ReadInt32({address})");
                return false;
            }
            finally
            {
                _lock.ExitReadLock();
            }

            _lastError = string.Empty;
            OnDataRead(new DataReadEventArgs(address, value, TypeCode.Int32, IPEnd));
            return true;
        }

        public bool WriteInt32(string address, int value)
        {
            if (!_IsConnected || !TryParseAddress(address, out ushort modbusAddress))
                return false;

            _lock.EnterWriteLock();
            try
            {
                var result = _modbusClient.Write(modbusAddress.ToString(), value);
                if (!result.IsSuccess)
                {
                    _lastError = result.Message;
                    return false;
                }
                _lastError = string.Empty;
            }
            catch (Exception ex)
            {
                HandleWriteError("WriteInt32", new List<string> { address }, value, ex);
                return false;
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            OnDataWriteCompleted(new DataWriteEventArgs("WriteInt32", new List<string> { address }, value, true));
            return true;
        }

        public bool ReadFloat(string address, ref float value)
        {
            if (!_IsConnected || !TryParseAddress(address, out ushort modbusAddress))
                return false;

            _lock.EnterReadLock();
            try
            {
                var result = _modbusClient.ReadFloat(modbusAddress.ToString());
                if (!result.IsSuccess)
                {
                    _lastError = result.Message;
                    return false;
                }
                value = result.Content;
            }
            catch (Exception ex)
            {
                HandleReadError(ex, $"ReadFloat({address})");
                return false;
            }
            finally
            {
                _lock.ExitReadLock();
            }

            _lastError = string.Empty;
            OnDataRead(new DataReadEventArgs(address, value, TypeCode.Single, IPEnd));
            return true;
        }

        public bool WriteFloat(string address, float value)
        {
            if (!_IsConnected || !TryParseAddress(address, out ushort modbusAddress))
                return false;

            _lock.EnterWriteLock();
            try
            {
                var result = _modbusClient.Write(modbusAddress.ToString(), value);
                if (!result.IsSuccess)
                {
                    _lastError = result.Message;
                    return false;
                }
                _lastError = string.Empty;
            }
            catch (Exception ex)
            {
                HandleWriteError("WriteFloat", new List<string> { address }, value, ex);
                return false;
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            OnDataWriteCompleted(new DataWriteEventArgs("WriteFloat", new List<string> { address }, value, true));
            return true;
        }

        public bool ReadDouble(string address, int power, ref double value)
        {
            if (!_IsConnected || !TryParseAddress(address, out ushort modbusAddress))
                return false;

            _lock.EnterReadLock();
            try
            {
                var result = _modbusClient.ReadDouble(modbusAddress.ToString());
                if (!result.IsSuccess)
                {
                    _lastError = result.Message;
                    return false;
                }
                value = result.Content / power;
            }
            catch (Exception ex)
            {
                HandleReadError(ex, $"ReadDouble({address})");
                return false;
            }
            finally
            {
                _lock.ExitReadLock();
            }

            _lastError = string.Empty;
            OnDataRead(new DataReadEventArgs(address, value, TypeCode.Double, IPEnd));
            return true;
        }

        public bool WriteDouble(string address, int power, double value)
        {
            if (!_IsConnected || !TryParseAddress(address, out ushort modbusAddress))
                return false;

            _lock.EnterWriteLock();
            try
            {
                var result = _modbusClient.Write(modbusAddress.ToString(), value * power);
                if (!result.IsSuccess)
                {
                    _lastError = result.Message;
                    return false;
                }
                _lastError = string.Empty;
            }
            catch (Exception ex)
            {
                HandleWriteError("WriteDouble", new List<string> { address }, value, ex);
                return false;
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            OnDataWriteCompleted(new DataWriteEventArgs("WriteDouble", new List<string> { address }, value, true));
            return true;
        }

        public bool ReadRegisterBit(string address, ref bool value)
        {
            if (!_IsConnected || !TryParseExtendedAddress(address, out ushort modbusAddress, out byte bitIndex))
                return false;

            _lock.EnterReadLock();
            try
            {
                var result = _modbusClient.ReadBool($"{modbusAddress}.{bitIndex}");
                if (!result.IsSuccess)
                {
                    _lastError = result.Message;
                    return false;
                }
                value = result.Content;
            }
            catch (Exception ex)
            {
                HandleReadError(ex, $"ReadRegisterBit({address})");
                return false;
            }
            finally
            {
                _lock.ExitReadLock();
            }

            _lastError = string.Empty;
            OnDataRead(new DataReadEventArgs(address, value, TypeCode.Boolean, IPEnd));
            return true;
        }

        public bool WriteRegisterBit(string address, bool value)
        {
            if (!_IsConnected || !TryParseExtendedAddress(address, out ushort modbusAddress, out byte bitIndex))
                return false;

            _lock.EnterWriteLock();
            try
            {
                var result = _modbusClient.Write($"{modbusAddress}.{bitIndex}", value);
                if (!result.IsSuccess)
                {
                    _lastError = result.Message;
                    return false;
                }
                _lastError = string.Empty;
            }
            catch (Exception ex)
            {
                HandleWriteError("WriteRegisterBit", new List<string> { address }, value, ex);
                return false;
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            OnDataWriteCompleted(new DataWriteEventArgs("WriteRegisterBit", new List<string> { address }, value, true));
            return true;
        }
        #endregion

        #region 批量数据操作
        public bool ReadContinuousInt(string address, int length, ref int[] values)
        {
            if (!_IsConnected || !TryParseAddress(address, out ushort modbusAddress))
                return false;

            _lock.EnterReadLock();
            try
            {
                var result = _modbusClient.ReadInt16(modbusAddress.ToString(), (ushort)(short)length);
                if (!result.IsSuccess)
                {
                    _lastError = result.Message;
                    return false;
                }
                values = result.Content.Select(x => (int)x).ToArray();
            }
            catch (Exception ex)
            {
                HandleReadError(ex, $"ReadContinuousInt({address})");
                return false;
            }
            finally
            {
                _lock.ExitReadLock();
            }

            _lastError = string.Empty;
            OnDataRead(new DataReadEventArgs(address, values, TypeCode.Int16, IPEnd, true, length));
            return true;
        }

        public bool BatchWriteFloat(List<string> addresses, float[] values)
        {
            if (!_IsConnected || addresses == null || values == null || addresses.Count != values.Length)
                return false;

            var failedWrites = new List<(string Address, float Value, string Error)>();

            _lock.EnterWriteLock();
            try
            {
                for (int i = 0; i < addresses.Count; i++)
                {
                    var result = _modbusClient.Write(addresses[i], values[i]);
                    if (!result.IsSuccess)
                    {
                        failedWrites.Add((addresses[i], values[i], result.Message));
                    }
                }
            }
            catch (Exception ex)
            {
                HandleWriteError("BatchWriteFloat", addresses, values, ex);
                return false;
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            if (failedWrites.Any())
            {
                _lastError = $"批量写入失败: {string.Join("; ", failedWrites.Select(f => $"{f.Address}={f.Value}({f.Error})"))}";
                return false;
            }

            OnDataWriteCompleted(new DataWriteEventArgs("BatchWriteFloat", addresses, values, true));
            return true;
        }

        public bool BatchReadFloat(List<string> addresses, ref float[] values)
        {
            if (!_IsConnected || addresses == null || addresses.Count == 0)
                return false;

            values = new float[addresses.Count];
            _lock.EnterReadLock();
            try
            {
                for (int i = 0; i < addresses.Count; i++)
                {
                    var result = _modbusClient.ReadFloat(addresses[i]);
                    if (!result.IsSuccess)
                    {
                        _lastError = result.Message;
                        return false;
                    }
                    values[i] = result.Content;
                }
            }
            catch (Exception ex)
            {
                HandleReadError(ex, $"BatchReadFloat({string.Join(", ", addresses)})");
                return false;
            }
            finally
            {
                _lock.ExitReadLock();
            }

            _lastError = string.Empty;
            OnDataRead(new DataReadEventArgs(string.Join(", ", addresses), values, TypeCode.Single, IPEnd, true, addresses.Count));
            return true;
        }

        public bool BatchReadInt32(List<string> addresses, ref int[] values)
        {
            if (!_IsConnected || addresses == null || addresses.Count == 0)
                return false;

            values = new int[addresses.Count];
            _lock.EnterReadLock();
            try
            {
                for (int i = 0; i < addresses.Count; i++)
                {
                    var result = _modbusClient.ReadInt32(addresses[i]);
                    if (!result.IsSuccess)
                    {
                        _lastError = result.Message;
                        return false;
                    }
                    values[i] = result.Content;
                }
            }
            catch (Exception ex)
            {
                HandleReadError(ex, $"BatchReadInt32({string.Join(", ", addresses)})");
                return false;
            }
            finally
            {
                _lock.ExitReadLock();
            }

            _lastError = string.Empty;
            OnDataRead(new DataReadEventArgs(string.Join(", ", addresses), values, TypeCode.Int32, IPEnd, true, addresses.Count));
            return true;
        }

        public bool BatchWriteInt32(List<string> addresses, int[] values)
        {
            if (!_IsConnected || addresses == null || values == null || addresses.Count != values.Length)
                return false;

            _lock.EnterWriteLock();
            try
            {
                for (int i = 0; i < addresses.Count; i++)
                {
                    var result = _modbusClient.Write(addresses[i], values[i]);
                    if (!result.IsSuccess)
                    {
                        _lastError = result.Message;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                HandleWriteError("BatchWriteInt32", addresses, values, ex);
                return false;
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            OnDataWriteCompleted(new DataWriteEventArgs("BatchWriteInt32", addresses, values, true));
            return true;
        }

        public bool BatchReadInt(List<string> addresses, ref int[] values)
        {
            if (!_IsConnected || addresses == null || addresses.Count == 0)
                return false;

            values = new int[addresses.Count];
            _lock.EnterReadLock();
            try
            {
                for (int i = 0; i < addresses.Count; i++)
                {
                    var result = _modbusClient.ReadInt16(addresses[i]);
                    if (!result.IsSuccess)
                    {
                        _lastError = result.Message;
                        return false;
                    }
                    values[i] = result.Content;
                }
            }
            catch (Exception ex)
            {
                HandleReadError(ex, $"BatchReadInt({string.Join(", ", addresses)})");
                return false;
            }
            finally
            {
                _lock.ExitReadLock();
            }

            _lastError = string.Empty;
            OnDataRead(new DataReadEventArgs(string.Join(", ", addresses), values, TypeCode.Int16, IPEnd, true, addresses.Count));
            return true;
        }

        public bool BatchWriteInt(List<string> addresses, int[] values)
        {
            if (!_IsConnected || addresses == null || values == null || addresses.Count != values.Length)
                return false;

            _lock.EnterWriteLock();
            try
            {
                for (int i = 0; i < addresses.Count; i++)
                {
                    var result = _modbusClient.Write(addresses[i], (short)values[i]);
                    if (!result.IsSuccess)
                    {
                        _lastError = result.Message;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                HandleWriteError("BatchWriteInt", addresses, values, ex);
                return false;
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            OnDataWriteCompleted(new DataWriteEventArgs("BatchWriteInt", addresses, values, true));
            return true;
        }

        public bool BatchReadDouble(List<string> addresses, int power, ref double[] values)
        {
            if (!_IsConnected || addresses == null || addresses.Count == 0)
                return false;

            values = new double[addresses.Count];
            _lock.EnterReadLock();
            try
            {
                for (int i = 0; i < addresses.Count; i++)
                {
                    var result = _modbusClient.ReadDouble(addresses[i]);
                    if (!result.IsSuccess)
                    {
                        _lastError = result.Message;
                        return false;
                    }
                    values[i] = result.Content / power;
                }
            }
            catch (Exception ex)
            {
                HandleReadError(ex, $"BatchReadDouble({string.Join(", ", addresses)})");
                return false;
            }
            finally
            {
                _lock.ExitReadLock();
            }

            _lastError = string.Empty;
            OnDataRead(new DataReadEventArgs(string.Join(", ", addresses), values, TypeCode.Double, IPEnd, true, addresses.Count));
            return true;
        }

        public bool BatchWriteDouble(List<string> addresses, int power, double[] values)
        {
            if (!_IsConnected || addresses == null || values == null || addresses.Count != values.Length)
                return false;

            _lock.EnterWriteLock();
            try
            {
                for (int i = 0; i < addresses.Count; i++)
                {
                    var result = _modbusClient.Write(addresses[i], values[i] * power);
                    if (!result.IsSuccess)
                    {
                        _lastError = result.Message;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                HandleWriteError("BatchWriteDouble", addresses, values, ex);
                return false;
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            OnDataWriteCompleted(new DataWriteEventArgs("BatchWriteDouble", addresses, values, true));
            return true;
        }

        public bool BatchReadString(List<string> addresses, int maxStringLength, ref string[] texts)
        {
            if (!_IsConnected || addresses == null || addresses.Count == 0)
                return false;

            texts = new string[addresses.Count];
            _lock.EnterReadLock();
            try
            {
                for (int i = 0; i < addresses.Count; i++)
                {
                    var result = _modbusClient.ReadString(addresses[i], (ushort)maxStringLength);
                    if (!result.IsSuccess)
                    {
                        _lastError = result.Message;
                        return false;
                    }
                    string originalText = result.Content?.TrimEnd('\0', ' ');
                    texts[i] = FixStringOrder(originalText);
                }
            }
            catch (Exception ex)
            {
                HandleReadError(ex, $"BatchReadString({string.Join(", ", addresses)})");
                return false;
            }
            finally
            {
                _lock.ExitReadLock();
            }

            _lastError = string.Empty;
            OnDataRead(new DataReadEventArgs(string.Join(", ", addresses), texts, TypeCode.String, IPEnd, true, addresses.Count));
            return true;
        }

        public bool BatchWriteString(List<string> addresses, string[] texts, int maxStringLength)
        {
            if (!_IsConnected || addresses == null || texts == null || addresses.Count != texts.Length)
                return false;

            _lock.EnterWriteLock();
            try
            {
                for (int i = 0; i < addresses.Count; i++)
                {
                    // 反转字符串顺序以便正确写入
                    string correctedText = ReverseStringOrder(texts[i]);

                    var result = _modbusClient.Write(addresses[i], correctedText);
                    if (!result.IsSuccess)
                    {
                        _lastError = result.Message;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                HandleWriteError("BatchWriteString", addresses, texts, ex);
                return false;
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            OnDataWriteCompleted(new DataWriteEventArgs("BatchWriteString", addresses, texts, true));
            return true;
        }

        public bool BatchReadRegisterBits(List<string> addresses, ref bool[] values)
        {
            if (!_IsConnected || addresses == null || addresses.Count == 0)
                return false;

            values = new bool[addresses.Count];
            _lock.EnterReadLock();
            try
            {
                for (int i = 0; i < addresses.Count; i++)
                {
                    var result = _modbusClient.ReadBool(addresses[i]);
                    if (!result.IsSuccess)
                    {
                        _lastError = result.Message;
                        return false;
                    }
                    values[i] = result.Content;
                }
            }
            catch (Exception ex)
            {
                HandleReadError(ex, $"BatchReadRegisterBits({string.Join(", ", addresses)})");
                return false;
            }
            finally
            {
                _lock.ExitReadLock();
            }

            _lastError = string.Empty;
            OnDataRead(new DataReadEventArgs(string.Join(", ", addresses), values, TypeCode.Boolean, IPEnd, true, addresses.Count));
            return true;
        }

        public bool BatchWriteRegisterBits(List<string> addresses, bool[] values)
        {
            if (!_IsConnected || addresses == null || values == null || addresses.Count != values.Length)
                return false;

            _lock.EnterWriteLock();
            try
            {
                for (int i = 0; i < addresses.Count; i++)
                {
                    var result = _modbusClient.Write(addresses[i], values[i]);
                    if (!result.IsSuccess)
                    {
                        _lastError = result.Message;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                HandleWriteError("BatchWriteRegisterBits", addresses, values, ex);
                return false;
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            OnDataWriteCompleted(new DataWriteEventArgs("BatchWriteRegisterBits", addresses, values, true));
            return true;
        }
        #endregion

        #region 位操作
        public bool ReadCoil(string address, ref bool value)
        {
            if (!_IsConnected || !TryParseCoilAddress(address, out ushort coilAddress))
                return false;

            _lock.EnterReadLock();
            try
            {
                var result = _modbusClient.ReadCoil(coilAddress.ToString());
                if (!result.IsSuccess)
                {
                    _lastError = result.Message;
                    return false;
                }
                value = result.Content;
            }
            catch (Exception ex)
            {
                HandleReadError(ex, $"ReadCoil({address})");
                return false;
            }
            finally
            {
                _lock.ExitReadLock();
            }

            _lastError = string.Empty;
            OnDataRead(new DataReadEventArgs(address, value, TypeCode.Boolean, IPEnd));
            return true;
        }

        public bool WriteCoil(string address, bool value)
        {
            if (!_IsConnected || !TryParseCoilAddress(address, out ushort coilAddress))
                return false;

            _lock.EnterWriteLock();
            try
            {
                var result = _modbusClient.Write(coilAddress.ToString(), value);
                if (!result.IsSuccess)
                {
                    _lastError = result.Message;
                    return false;
                }
                _lastError = string.Empty;
            }
            catch (Exception ex)
            {
                HandleWriteError("WriteCoil", new List<string> { address }, value, ex);
                return false;
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            OnDataWriteCompleted(new DataWriteEventArgs("WriteCoil", new List<string> { address }, value, true));
            return true;
        }

        public bool ReadCoils(string address, int length, ref bool[] values)
        {
            if (!_IsConnected || !TryParseCoilAddress(address, out ushort coilAddress))
                return false;

            _lock.EnterReadLock();
            try
            {
                var result = _modbusClient.ReadCoil(coilAddress.ToString(), (ushort)(short)length);
                if (!result.IsSuccess)
                {
                    _lastError = result.Message;
                    return false;
                }
                values = result.Content;
            }
            catch (Exception ex)
            {
                HandleReadError(ex, $"ReadCoils({address})");
                return false;
            }
            finally
            {
                _lock.ExitReadLock();
            }

            _lastError = string.Empty;
            OnDataRead(new DataReadEventArgs(address, values, TypeCode.Boolean, IPEnd, true, length));
            return true;
        }

        public bool WriteCoils(string address, bool[] values)
        {
            if (!_IsConnected || !TryParseCoilAddress(address, out ushort coilAddress))
                return false;

            _lock.EnterWriteLock();
            try
            {
                var result = _modbusClient.Write(coilAddress.ToString(), values);
                if (!result.IsSuccess)
                {
                    _lastError = result.Message;
                    return false;
                }
                _lastError = string.Empty;
            }
            catch (Exception ex)
            {
                HandleWriteError("WriteCoils", new List<string> { address }, values, ex);
                return false;
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            OnDataWriteCompleted(new DataWriteEventArgs("WriteCoils", new List<string> { address }, values, true));
            return true;
        }
        #endregion

        #region 字符串操作
        public bool ReadString(string address, ref string text, int maxLength = 20)
        {
            if (!_IsConnected || !TryParseAddress(address, out ushort modbusAddress))
                return false;

            _lock.EnterReadLock();
            try
            {
                var result = _modbusClient.ReadString(modbusAddress.ToString(), (ushort)maxLength);
                if (!result.IsSuccess)
                {
                    _lastError = result.Message;
                    return false;
                }

                string originalText = result.Content?.TrimEnd('\0', ' ').Trim();
                text = FixStringOrder(originalText);
            }
            catch (Exception ex)
            {
                HandleReadError(ex, $"ReadString({address})");
                return false;
            }
            finally
            {
                _lock.ExitReadLock();
            }

            _lastError = string.Empty;
            OnDataRead(new DataReadEventArgs(address, text, TypeCode.String, IPEnd));
            return true;
        }

        public bool WriteString(string address, string text)
        {
            if (!_IsConnected || string.IsNullOrEmpty(text) || !TryParseAddress(address, out ushort modbusAddress))
                return false;

            _lock.EnterWriteLock();
            try
            {
                // 反转字符串顺序以便正确写入
                string correctedText = ReverseStringOrder(text);

                var result = _modbusClient.Write(modbusAddress.ToString(), correctedText);
                if (!result.IsSuccess)
                {
                    _lastError = result.Message;
                    return false;
                }
                _lastError = string.Empty;
            }
            catch (Exception ex)
            {
                HandleWriteError("WriteString", new List<string> { address }, text, ex);
                return false;
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            OnDataWriteCompleted(new DataWriteEventArgs("WriteString", new List<string> { address }, text, true));
            return true;
        }
        #endregion

        #region 自动读取系统
        private Dictionary<TypeCode, Func<AutoReadConfig, bool>> _autoReadHandlers;

        private void InitializeAutoReadHandlers()
        {
            _autoReadHandlers = new Dictionary<TypeCode, Func<AutoReadConfig, bool>>
            {
                [TypeCode.Int16] = config =>
                {
                    int value = 0;
                    return ReadRegister(config.Addresses[0], ref value);
                },
                [TypeCode.Int32] = config =>
                {
                    int value = 0;
                    return ReadInt32(config.Addresses[0], ref value);
                },
                [TypeCode.Single] = config =>
                {
                    float value = 0;
                    return ReadFloat(config.Addresses[0], ref value);
                },
                [TypeCode.Double] = config =>
                {
                    double value = 0;
                    return ReadDouble(config.Addresses[0], config.Power, ref value);
                },
                [TypeCode.Boolean] = config =>
                {
                    bool value = false;
                    if (config.Addresses[0].Contains('.'))
                        return ReadRegisterBit(config.Addresses[0], ref value);
                    else
                        return ReadCoil(config.Addresses[0], ref value);
                },
                [TypeCode.String] = config =>
                {
                    string text = null;
                    return ReadString(config.Addresses[0], ref text, config.MaxLength);
                }
            };
        }
        public void StartAutoRead()
        {
            _lock.EnterWriteLock();
            try
            {
                // 检查定时器是否已经存在并且已经订阅了事件
                if (_autoReadTimer != null && _autoReadTimer.Enabled)
                    return;

                // 如果定时器存在但未启用，先清理资源
                if (_autoReadTimer != null)
                {
                    _autoReadTimer.Elapsed -= OnAutoReadCheck; // 确保取消之前的订阅
                    _autoReadTimer.Dispose();
                    _autoReadTimer = null;
                }

                _autoReadTimer = new Timer(100);
                _autoReadTimer.Elapsed += OnAutoReadCheck;
                _autoReadTimer.AutoReset = true;
                _autoReadTimer.Start();
                RecalculateNextReadTime();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void StopAutoRead()
        {
            _lock.EnterWriteLock();
            try
            {
                if (_autoReadTimer != null)
                {
                    _autoReadTimer.Stop();
                    _autoReadTimer.Dispose();
                    _autoReadTimer = null;
                }

                if (_autoReadConfigs != null)
                {
                    _autoReadConfigs.Clear();
                }

                _nextReadTime = DateTime.MaxValue;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        private void OnAutoReadCheck(object sender, ElapsedEventArgs e)
        {
            if (!_IsConnected)
                return;

            var now = DateTime.Now;
            if (now < _nextReadTime)
                return;

            _nextReadTime = DateTime.MaxValue;
            bool anyRead = false;

            foreach (var config in _autoReadConfigs.Values)
            {
                if ((now - config.LastReadTime).TotalMilliseconds >= config.ReadIntervalMs)
                {
                    config.LastReadTime = now;
                    ThreadPool.QueueUserWorkItem(state => ProcessAutoReadConfig(config));
                    anyRead = true;
                }
            }

            if (anyRead)
            {
                RecalculateNextReadTime();
            }
        }

        public bool AddOrUpdateAutoRead(string key, string address, TypeCode dataType,
                                      int readIntervalMs = 1000, int power = 1, int length = 1,
                                      int maxLength = 20, bool isBulkRead = true)
        {
            try
            {
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

                _autoReadConfigs.AddOrUpdate(key, config, (k, oldValue) => config);
                RecalculateNextReadTime();
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex, $"AddOrUpdateAutoRead({key})");
                return false;
            }
        }

        public bool AddOrUpdateAutoRead(string key, List<string> addresses, TypeCode dataType,
                                       int readIntervalMs = 1000, int power = 1, int length = 1,
                                       int maxLength = 10, bool isBulkRead = true)
        {
            try
            {
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

                _autoReadConfigs.AddOrUpdate(key, config, (k, oldValue) => config);
                RecalculateNextReadTime();
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex, $"AddOrUpdateAutoRead({key})");
                return false;
            }
        }

        public bool RemoveAutoRead(string key)
        {
            bool result = _autoReadConfigs.TryRemove(key, out _);
            if (result)
            {
                RecalculateNextReadTime();
            }
            return result;
        }

        public bool RemoveAutoReadAll()
        {
            try
            {
                _autoReadConfigs.Clear();
                RecalculateNextReadTime();
                return true;
            }
            catch (Exception ex)
            {
                LogException(ex, "RemoveAutoReadAll");
                return false;
            }
        }

        public List<string> GetAutoReadKeys()
        {
            return _autoReadConfigs.Keys.ToList();
        }

        public AutoReadConfig GetAutoReadConfig(string key)
        {
            return _autoReadConfigs.TryGetValue(key, out var config) ? config : null;
        }

        public bool ExecuteAutoReadNow(string key)
        {
            if (!_autoReadConfigs.TryGetValue(key, out var config))
                return false;

            return ProcessAutoReadConfig(config);
        }

        private bool ProcessAutoReadConfig(AutoReadConfig config)
        {
            try
            {
                if (config.Addresses.Count == 0)
                    return false;

                if (config.Addresses.Count == 1)
                {
                    if (_autoReadHandlers.TryGetValue(config.DataType, out var handler))
                    {
                        return handler(config);
                    }
                }
                else
                {
                    foreach (var address in config.Addresses)
                    {
                        if (config.DataType == TypeCode.Int16)
                        {
                            int value = 0;
                            if (!ReadRegister(address, ref value))
                                return false;
                        }
                        else if (config.DataType == TypeCode.Int32)
                        {
                            int value = 0;
                            if (!ReadInt32(address, ref value))
                                return false;
                        }
                        else if (config.DataType == TypeCode.Single)
                        {
                            float value = 0;
                            if (!ReadFloat(address, ref value))
                                return false;
                        }
                        else if (config.DataType == TypeCode.Double)
                        {
                            double value = 0;
                            if (!ReadDouble(address, config.Power, ref value))
                                return false;
                        }
                        else if (config.DataType == TypeCode.Boolean)
                        {
                            bool value = false;
                            if (address.Contains('.'))
                            {
                                if (!ReadRegisterBit(address, ref value))
                                    return false;
                            }
                            else
                            {
                                if (!ReadCoil(address, ref value))
                                    return false;
                            }
                        }
                        else if (config.DataType == TypeCode.String)
                        {
                            string text = null;
                            if (!ReadString(address, ref text, config.MaxLength))
                                return false;
                        }
                    }
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                LogException(ex, $"AutoRead for {config.Key}");
                return false;
            }
        }

        private void RecalculateNextReadTime()
        {
            if (_autoReadConfigs == null || _autoReadConfigs.Count == 0)
            {
                _nextReadTime = DateTime.MaxValue;
                return;
            }

            DateTime minNextTime = DateTime.MaxValue;
            foreach (var config in _autoReadConfigs.Values)
            {
                DateTime nextRead = config.LastReadTime.AddMilliseconds(config.ReadIntervalMs);
                if (nextRead < minNextTime)
                {
                    minNextTime = nextRead;
                }
            }

            _nextReadTime = minNextTime;

            if (_autoReadTimer != null && _nextReadTime != DateTime.MaxValue)
            {
                double millisecondsUntilNextRead = (_nextReadTime - DateTime.Now).TotalMilliseconds;
                if (millisecondsUntilNextRead > 0)
                {
                    _autoReadTimer.Interval = Math.Max(100, millisecondsUntilNextRead);
                }
            }
        }
        #endregion

        #region 心跳控制方法
        public void StartHeartbeatService()
        {
            if (_heartbeatAddress == 0)
            {
                WriteLog("警告: 未设置心跳地址，使用默认地址100");
                _heartbeatAddress = 100;
            }

            HeartbeatEnabled = true;
            _heartbeatTimer?.Start();
            _currentRetryCount = 0;
        }

        public void StopHeartbeatService()
        {
            HeartbeatEnabled = false;
            _heartbeatTimer?.Stop();
        }

        public void SetHeartbeatAddress(string address)
        {
            if (!_IsConnected || !TryParseAddress(address, out ushort modbusAddress))
                return;
            HeartbeatAddress = modbusAddress;
        }

        public void SetHeartbeatInterval(int intervalMs)
        {
            HeartbeatInterval = intervalMs;
        }

        public HeartbeatStatus GetHeartbeatStatus()
        {
            return new HeartbeatStatus
            {
                IsEnabled = _heartbeatEnabled,
                Interval = _heartbeatInterval,
                LastRetryCount = _currentRetryCount,
                IsConnectionAlive = _IsConnected && _currentRetryCount == 0,
                LastCheckTime = DateTime.Now
            };
        }
        #endregion

        #region 辅助方法和资源管理
        private bool TryParseAddress(string deviceName, out ushort modbusAddress)
        {
            modbusAddress = 0;
            if (string.IsNullOrEmpty(deviceName))
                return false;

            if (ushort.TryParse(deviceName, out ushort address))
            {
                modbusAddress = address;
                return true;
            }
            return false;
        }

        private bool TryParseCoilAddress(string deviceName, out ushort coilAddress)
        {
            coilAddress = 0;
            if (string.IsNullOrEmpty(deviceName))
                return false;

            if (deviceName.StartsWith("Y") && ushort.TryParse(deviceName.Substring(1), out ushort address))
            {
                coilAddress = address;
                return true;
            }
            return false;
        }

        private bool TryParseExtendedAddress(string deviceName, out ushort modbusAddress, out byte bitIndex)
        {
            modbusAddress = 0;
            bitIndex = 0;

            if (string.IsNullOrEmpty(deviceName))
                return false;

            if (deviceName.Contains('.'))
            {
                string[] parts = deviceName.Split('.');
                if (parts.Length == 2 &&
                    ushort.TryParse(parts[0], out modbusAddress) &&
                    byte.TryParse(parts[1], out bitIndex) &&
                    bitIndex < 16)
                {
                    return true;
                }
                return false;
            }

            return TryParseAddress(deviceName, out modbusAddress);
        }

        private void LogException(Exception ex, string operation)
        {
            LoggingService.Error($"CDeltaModbusTcp 错误于 {operation}", ex);
        }

        private void HandleReadError(Exception ex, string operation)
        {
            _lastError = ex.Message;
            LoggingService.Error($"读取操作失败: {operation}", ex);
        }

        private void HandleWriteError(string operation, List<string> addresses, object values, Exception ex)
        {
            _lastError = ex.Message;
            LoggingService.Error($"写入操作失败: {operation}", ex);
            OnDataWriteCompleted(new DataWriteEventArgs(operation, addresses, values, false, ex.Message));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                StopAutoRead();
                StopHeartbeatService();
                if (_lock.TryEnterWriteLock(TimeSpan.FromSeconds(2)))
                {
                    try
                    {
                        CloseConnectionUnderLock();
                        _modbusClient?.Dispose();
                    }
                    finally
                    {
                        _lock.ExitWriteLock();
                    }
                }
                _heartbeatTimer?.Dispose();
                _autoReadTimer?.Dispose();
                _lock.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~CDeltaModbusTcp()
        {
            Dispose(false);
        }
        #endregion

        #region 字符串顺序修正方法
        /// <summary>
        /// 修正读取的字符串顺序
        /// </summary>
        private string FixStringOrder(string originalString)
        {
            if (string.IsNullOrEmpty(originalString))
                return originalString;

            switch (_stringByteOrder)
            {
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
        private string ReverseStringOrder(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            switch (_stringByteOrder)
            {
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
        private string SwapBytesInString(string str)
        {
            if (string.IsNullOrEmpty(str) || str.Length < 2)
                return str;

            char[] chars = str.ToCharArray();
            for (int i = 0; i < chars.Length; i += 2)
            {
                if (i + 1 < chars.Length)
                {
                    char temp = chars[i];
                    chars[i] = chars[i + 1];
                    chars[i + 1] = temp;
                }
            }
            return new string(chars);
        }

        /// <summary>
        /// 寄存器间交换（每4个字符交换前2个和后2个）
        /// 示例： "ABCD" -> "CDAB"
        /// </summary>
        private string SwapWordsInString(string str)
        {
            if (string.IsNullOrEmpty(str) || str.Length < 4)
                return str;

            char[] chars = str.ToCharArray();
            for (int i = 0; i < chars.Length; i += 4)
            {
                if (i + 3 < chars.Length)
                {
                    // 交换前2个字符和后2个字符
                    char temp1 = chars[i];
                    char temp2 = chars[i + 1];
                    chars[i] = chars[i + 2];
                    chars[i + 1] = chars[i + 3];
                    chars[i + 2] = temp1;
                    chars[i + 3] = temp2;
                }
            }
            return new string(chars);
        }
        #endregion

        #region 端序测试和诊断
        public bool TestByteOrder(string testAddress = "0")
        {
            LoggingService.Info($"开始端序测试，地址: {testAddress}, 当前端序: {_byteOrder}");

            if (!_IsConnected || !TryParseAddress(testAddress, out ushort modbusAddress))
            {
                LoggingService.Error("端序测试失败: PLC未连接或地址解析失败");
                return false;
            }

            var sb = new StringBuilder();
            sb.AppendLine($"端序测试开始 (地址: {testAddress}, 端序: {_byteOrder})");
            bool allTestsPassed = true;

            try
            {
                // 测试 int16
                short testValue16 = 0x1234;
                if (!WriteRegister(testAddress, testValue16))
                {
                    sb.AppendLine("16位整数写入失败");
                    allTestsPassed = false;
                }
                else
                {
                    int readValue16 = 0;
                    if (!ReadRegister(testAddress, ref readValue16))
                    {
                        sb.AppendLine("16位整数读取失败");
                        allTestsPassed = false;
                    }
                    else
                    {
                        bool success16 = (short)readValue16 == testValue16;
                        sb.AppendLine($"16位整数测试: {(success16 ? "通过" : "失败")} (写入: 0x{testValue16:X4}, 读取: 0x{readValue16:X4})");
                        allTestsPassed &= success16;
                    }
                }

                // 测试 int32
                int testValue32 = 0x12345678;
                if (!WriteInt32(testAddress, testValue32))
                {
                    sb.AppendLine("32位整数写入失败");
                    allTestsPassed = false;
                }
                else
                {
                    int readValue32 = 0;
                    if (!ReadInt32(testAddress, ref readValue32))
                    {
                        sb.AppendLine("32位整数读取失败");
                        allTestsPassed = false;
                    }
                    else
                    {
                        bool success32 = readValue32 == testValue32;
                        sb.AppendLine($"32位整数测试: {(success32 ? "通过" : "失败")} (写入: 0x{testValue32:X8}, 读取: 0x{readValue32:X8})");
                        allTestsPassed &= success32;
                    }
                }

                // 测试 float
                float testValueFloat = 123.456f;
                if (!WriteFloat(testAddress, testValueFloat))
                {
                    sb.AppendLine("32位浮点数写入失败");
                    allTestsPassed = false;
                }
                else
                {
                    float readValueFloat = 0;
                    if (!ReadFloat(testAddress, ref readValueFloat))
                    {
                        sb.AppendLine("32位浮点数读取失败");
                        allTestsPassed = false;
                    }
                    else
                    {
                        bool successFloat = Math.Abs(testValueFloat - readValueFloat) < 0.001f;
                        sb.AppendLine($"32位浮点数测试: {(successFloat ? "通过" : "失败")} (写入: {testValueFloat:F4}, 读取: {readValueFloat:F4})");
                        allTestsPassed &= successFloat;
                    }
                }

                // 测试 double
                double testValueDouble = 123456.789;
                int power = 1000;
                if (!WriteDouble(testAddress, power, testValueDouble))
                {
                    sb.AppendLine("64位双精度数写入失败");
                    allTestsPassed = false;
                }
                else
                {
                    double readValueDouble = 0;
                    if (!ReadDouble(testAddress, power, ref readValueDouble))
                    {
                        sb.AppendLine("64位双精度数读取失败");
                        allTestsPassed = false;
                    }
                    else
                    {
                        bool successDouble = Math.Abs(testValueDouble - readValueDouble) < 0.001;
                        sb.AppendLine($"64位双精度数测试: {(successDouble ? "通过" : "失败")} (写入: {testValueDouble:F4}, 读取: {readValueDouble:F4})");
                        allTestsPassed &= successDouble;
                    }
                }

                // 测试 string
                string testString = "ABCD";
                if (!WriteString(testAddress, testString))
                {
                    sb.AppendLine("字符串写入失败");
                    allTestsPassed = false;
                }
                else
                {
                    string readString = "";
                    if (!ReadString(testAddress, ref readString, testString.Length))
                    {
                        sb.AppendLine("字符串读取失败");
                        allTestsPassed = false;
                    }
                    else
                    {
                        bool successString = readString == testString;
                        sb.AppendLine($"字符串测试: {(successString ? "通过" : "失败")} (写入: \"{testString}\", 读取: \"{readString}\")");
                        allTestsPassed &= successString;
                    }
                }

                // 测试 bool
                bool testValueBool = true;
                string bitAddress = $"{testAddress}.0";
                if (!WriteRegisterBit(bitAddress, testValueBool))
                {
                    sb.AppendLine("位写入失败");
                    allTestsPassed = false;
                }
                else
                {
                    bool readValueBool = false;
                    if (!ReadRegisterBit(bitAddress, ref readValueBool))
                    {
                        sb.AppendLine("位读取失败");
                        allTestsPassed = false;
                    }
                    else
                    {
                        bool successBool = readValueBool == testValueBool;
                        sb.AppendLine($"位测试: {(successBool ? "通过" : "失败")} (写入: {testValueBool}, 读取: {readValueBool})");
                        allTestsPassed &= successBool;
                    }
                }
                LoggingService.Info("端序测试完成");
            }
            catch (Exception ex)
            {
                LoggingService.Error("端序测试异常", ex);
                allTestsPassed = false;
            }

            _lastError = sb.ToString();
            WriteLog(_lastError);
            return allTestsPassed;
        }

        public string GetByteOrderInfo()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"当前端序: {_byteOrder}");
            sb.AppendLine($"描述: {GetByteOrderDescription(_byteOrder)}");

            // 测试端序转换
            float testFloat = 123.456f;
            var result = _modbusClient.Write("0", testFloat);
            if (result.IsSuccess)
            {
                var registers = _modbusClient.ReadInt16("0", 2).Content;
                sb.AppendLine($"示例 float ({testFloat:F4}) 寄存器: [0x{registers[0]:X4}, 0x{registers[1]:X4}]");
            }

            return sb.ToString();
        }

        private string GetByteOrderDescription(ByteOrderEnum order)
        {
            switch (order)
            {
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

        public string VerifyByteOrderSettings()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"当前端序配置: {_byteOrder}");
            sb.AppendLine($"HSL DataFormat: {_modbusClient?.ByteTransform.DataFormat}");

            // 测试端序
            float testFloat = 123.456f;
            var result = _modbusClient.Write("0", testFloat);
            if (result.IsSuccess)
            {
                var registers = _modbusClient.ReadInt16("0", 2).Content;
                sb.AppendLine($"示例 float ({testFloat:F4}) 寄存器: [0x{registers[0]:X4}, 0x{registers[1]:X4}]");
            }

            return sb.ToString();
        }

        public bool AutoDetectByteOrder(string testAddress = "0")
        {
            if (!_IsConnected || !TryParseAddress(testAddress, out ushort modbusAddress))
                return false;

            var originalOrder = _byteOrder;
            var orders = Enum.GetValues(typeof(ByteOrderEnum)).Cast<ByteOrderEnum>();
            var results = new Dictionary<ByteOrderEnum, string>();

            foreach (var order in orders)
            {
                ByteOrder = order;
                _lastError = string.Empty;
                if (TestByteOrder(testAddress))
                {
                    results[order] = $"成功 (端序: {order})";
                }
                else
                {
                    results[order] = $"失败: {_lastError}";
                }
            }

            var successfulOrder = results.FirstOrDefault(r => r.Value.StartsWith("成功")).Key;
            if (successfulOrder != default)
            {
                ByteOrder = successfulOrder;
                _lastError = $"自动检测到端序: {successfulOrder}\n测试结果:\n{string.Join("\n", results.Select(r => $"{r.Key}: {r.Value}"))}";
                WriteLog(_lastError);
                return true;
            }

            ByteOrder = originalOrder;
            _lastError = $"无法自动检测端序，请手动设置\n测试结果:\n{string.Join("\n", results.Select(r => $"{r.Key}: {r.Value}"))}";
            WriteLog(_lastError);
            return false;
        }
        #endregion
    }
}
