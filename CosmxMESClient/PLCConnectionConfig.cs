using CosmxMESClient.Communication;
using CosmxMESClient.interfaceConfig;
using CosmxMESClient.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace CosmxMESClient {
    public class PLCConnectionConfig:INotifyPropertyChanged {
        private IPlcCommunication _plcInstance;
        private bool _isConnected = false;
        private readonly object _connectionLock = new object();
        private DateTime _lastConnectTime;
        private int _connectRetryCount = 0;
        private const int MAX_RETRY_COUNT = 3;
        public event PropertyChangedEventHandler PropertyChanged;
        public BindingList<PLCScanAddress> ScanAddresses { get; set; } = new BindingList<PLCScanAddress>( );
        // 地址配置集合
        // 使用可绑定的集合替代Dictionary
        public BindingList<PLCSendAddress> SendAddresses { get; set; } = new BindingList<PLCSendAddress>( );

        // 对外暴露BindingList
        private Dictionary<string, PLCScanAddress> _scanAddressesDict = new Dictionary<string, PLCScanAddress>();
        private Dictionary<string, PLCSendAddress> _sendAddressesDict = new Dictionary<string, PLCSendAddress>();
        private readonly object _addressLock = new object();
        // 添加连接状态变化事件
        public event EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChanged;
        
        public class ConnectionStatusChangedEventArgs:EventArgs {
            public ConnectionStatus PreviousStatus {
                get; set;
                }
            public ConnectionStatus CurrentStatus {
                get; set;
                }
            public string Message {
                get; set;
                }
            }
        private string _ipAddress;
        private int _port;
        private byte _slaveID;
        private int _timeout;
        private ByteOrderEnum _byteOrder;
        private StringByteOrderEnum _stringByteOrder;
        private bool _heartbeatEnabled;
        private int _heartbeatInterval;
        private string _heartbeatAddress;

        // 添加连接状态枚举
        public enum ConnectionStatus {
            Disconnected,
            Connecting,
            Connected,
            Faulted
            }

        private ConnectionStatus _connectionStatus = ConnectionStatus.Disconnected;
        private string _name;
        [JsonIgnore]
        public string Key => Name;// PLC连接唯一键值
        public string Name {
            get => _name;
            set {
                if (string.IsNullOrWhiteSpace(value)) {
                    throw new ArgumentException("PLC名称不能为空");
                    }
                _name=value.Trim( );
                }
            }
        public PLCType PLCType {
            get; set;
            }
        public string IPAddress {
            get => _ipAddress;
            set {
                if (_ipAddress!=value) {
                    _ipAddress=value;
                    OnPropertyChanged( );
                    // 在线更新PLC实例
                    UpdatePLCInstanceProperty(nameof(IPAddress),value);
                    }
                }
            }
        // 添加重连相关属性
        public int ReconnectAttempts {
            get; set;
            }
        public DateTime LastReconnectAttempt {
            get; set;
            }
        public DateTime LastConnectedTime {
            get; set;
            }
        public int Port {
            get => _port;
            set {
                if (_port!=value) {
                    _port=value;
                    OnPropertyChanged( );
                    UpdatePLCInstanceProperty(nameof(Port),value);
                    }
                }
            }

        public byte SlaveID {
            get => _slaveID;
            set {
                if (_slaveID!=value) {
                    _slaveID=value;
                    OnPropertyChanged( );
                    UpdatePLCInstanceProperty(nameof(SlaveID),value);
                    }
                }
            }

        public int Timeout {
            get => _timeout;
            set {
                if (_timeout!=value) {
                    _timeout=value;
                    OnPropertyChanged( );
                    UpdatePLCInstanceProperty(nameof(Timeout),value);
                    }
                }
            }

        public ByteOrderEnum ByteOrder {
            get => _byteOrder;
            set {
                if (_byteOrder!=value) {
                    _byteOrder=value;
                    OnPropertyChanged( );
                    UpdatePLCInstanceProperty(nameof(ByteOrder),value);
                    }
                }
            }

        public StringByteOrderEnum StringByteOrder {
            get => _stringByteOrder;
            set {
                if (_stringByteOrder!=value) {
                    _stringByteOrder=value;
                    OnPropertyChanged( );
                    UpdatePLCInstanceProperty(nameof(StringByteOrder),value);
                    }
                }
            }


        public bool HeartbeatEnabled {
            get => _heartbeatEnabled;
            set {
                if (_heartbeatEnabled!=value) {
                    _heartbeatEnabled=value;
                    OnPropertyChanged( );
                    UpdatePLCInstanceProperty(nameof(HeartbeatEnabled),value);
                    }
                }
            }

        public int HeartbeatInterval {
            get => _heartbeatInterval;
            set {
                if (_heartbeatInterval!=value) {
                    _heartbeatInterval=value;
                    OnPropertyChanged( );
                    UpdatePLCInstanceProperty(nameof(HeartbeatInterval),value);
                    }
                }
            }


        public string HeartbeatAddress {
            get => _heartbeatAddress;
            set {
                if (_heartbeatAddress!=value) {
                    _heartbeatAddress=value;
                    OnPropertyChanged( );
                    UpdatePLCInstanceProperty(nameof(HeartbeatAddress),value);
                    }
                }
            }

        public bool IsEnabled {
            get; set;
            }



        private void UpdatePLCInstanceProperty( string propertyName,object value ) {
            if (_plcInstance==null||!IsConnected)
                return;

            try {
                switch (propertyName) {
                    case nameof(Timeout):
                        if (_plcInstance is CDeltaModbusTcp delta)
                            delta.Timeout=(int) value;
                        break;
                    case nameof(StringByteOrder):
                        if (_plcInstance is CDeltaModbusTcp delta2)
                            delta2.StringByteOrder=(StringByteOrderEnum) value;
                        break;
                    case nameof(HeartbeatEnabled):
                        if (_plcInstance is CDeltaModbusTcp delta3) {
                            if ((bool) value)
                                delta3.StartHeartbeatService( );
                            else
                                delta3.StopHeartbeatService( );
                            }
                        break;
                    case nameof(HeartbeatInterval):
                        if (_plcInstance is CDeltaModbusTcp delta4)
                            delta4.SetHeartbeatInterval((int) value);
                        break;
                    case nameof(HeartbeatAddress):
                        if (_plcInstance is CDeltaModbusTcp delta5)
                            delta5.SetHeartbeatAddress((string) value);
                        break;
                    }
                }
            catch (Exception ex) {
                LoggingService.Warn($"在线更新PLC参数失败 {propertyName}: {ex.Message}");
                }
            }

        protected virtual void OnPropertyChanged( [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null ) {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName));
            }

        // 地址管理方法
        public bool AddScanAddress( PLCScanAddress address ) {
            lock (_addressLock) {
                if (string.IsNullOrEmpty(address.Key)) {
                    address.Key=GenerateAddressKey(address.Name,AddressDirection.ReadOnly);
                    }

                if (_scanAddressesDict.ContainsKey(address.Key)) {
                    throw new ArgumentException($"发送地址键值已存在: {address.Key}");
                    }

                _scanAddressesDict[address.Key]=address;
                ScanAddresses.Add(address); // 添加到可绑定集合
                return true;
                }
            }
        public bool AddSendAddress( PLCSendAddress address ) {
            if (string.IsNullOrEmpty(address.Key)) {
                address.Key=GenerateAddressKey(address.Name,AddressDirection.WriteOnly);
                }

            if (_sendAddressesDict.ContainsKey(address.Key)) {
                throw new ArgumentException($"发送地址键值已存在: {address.Key}");
                }

            _sendAddressesDict[address.Key]=address;
            SendAddresses.Add(address); // 添加到可绑定集合
            return true;
            }
        public PLCConnectionConfig( ) {
            // 设置安全的默认值，避免数值范围错误
            Name="New_PLC";
            IPAddress="192.168.1.100";
            Port=502;           // 有效端口号
            SlaveID=1;          // 有效站号
            Timeout=1000;       // 有效超时时间
            HeartbeatInterval=30000;  // 有效心跳间隔
            HeartbeatAddress="100";
            IsEnabled=true;
            HeartbeatEnabled=true;
            }

        public PLCConnectionConfig( string name ) {
            Name=name;
            }
        // 重写Equals和GetHashCode，方便比较
        public override bool Equals( object obj ) {
            return obj is PLCConnectionConfig other&&
                   Key.Equals(other.Key,StringComparison.OrdinalIgnoreCase);
            }

        public override int GetHashCode( ) {
            return Key.GetHashCode( );
            }

        public override string ToString( ) {
            return $"{Name} ({IPAddress}:{Port})";
            }



        public bool RemoveScanAddress( string key ) {
            lock (_addressLock) {
                if (_scanAddressesDict.Remove(key)) {
                    var addressToRemove = ScanAddresses.FirstOrDefault(a => a.Key == key);
                    if (addressToRemove!=null) {
                        ScanAddresses.Remove(addressToRemove);
                        }
                    return true;
                    }
                return false;
                }
            }

        public bool RemoveSendAddress( string key ) {
            if (_sendAddressesDict.Remove(key)) {
                var addressToRemove = SendAddresses.FirstOrDefault(a => a.Key == key);
                if (addressToRemove!=null) {
                    SendAddresses.Remove(addressToRemove);
                    }
                return true;
                }
            return false;
            }

        private string GenerateAddressKey( string name,AddressDirection direction ) {
            string prefix;
            switch (direction) {
                case AddressDirection.ReadOnly:
                    prefix="R_";
                    break;
                case AddressDirection.WriteOnly:
                    prefix="W_";
                    break;
                case AddressDirection.ReadWrite:
                    prefix="RW_";
                    break;
                default:
                    prefix="A_";
                    break;
                }

            return $"{prefix}{name}_{DateTime.Now:HHmmssfff}";
            }
        // 添加PLC实例引用
        [JsonIgnore]
        public IPlcCommunication PLCInstance {
            get {
                lock (_connectionLock) {
                    if (_plcInstance==null) {
                        _plcInstance=CreatePLCInstanceInternal( );
                        }
                    return _plcInstance;
                    }
                }
            }

        [JsonIgnore]
        public bool IsConnected {
            get {
                lock (_connectionLock) {
                    return _isConnected&&_plcInstance!=null&&_plcInstance.IsConnected;
                    }
                }
            }

        // 添加用于显示的字符串属性
        //  [Browsable(false)] // 不在属性网格中显示
        //  public string ConnectionStatus => IsConnected ? "已连接" : "未连接";
        [JsonIgnore]
        public ConnectionStatus Status {
            get {
                lock (_connectionLock) {
                    return _connectionStatus;
                    }
                }
            }

        [Browsable(false)]
        public string EnabledStatus => IsEnabled ? "启用" : "禁用";

        // 添加最后错误信息
        [JsonIgnore]
        public string LastError {
            get; private set;
            }

        /// <summary>
        /// 创建PLC实例
        /// </summary>
        public IPlcCommunication CreatePLCInstance( ) {
            try {
                IPlcCommunication plc = null;

                switch (PLCType) {
                    case PLCType.DeltaModbusTCP:
                        plc=new CDeltaModbusTcp(IPAddress,Port,SlaveID)
                            {
                            ByteOrder=ByteOrder,
                            StringByteOrder=StringByteOrder,
                            Timeout=Timeout
                            };
                        break;
                    // 可以扩展其他PLC类型
                    default:
                        throw new NotSupportedException($"不支持的PLC类型: {PLCType}");
                    }

                // 设置心跳配置
                if (HeartbeatEnabled&&!string.IsNullOrEmpty(HeartbeatAddress)) {
                    plc.SetHeartbeatAddress(HeartbeatAddress);
                    plc.SetHeartbeatInterval(HeartbeatInterval);
                    }

                _plcInstance=plc;
                return plc;
                }
            catch (Exception ex) {
                LastError=$"创建PLC实例失败: {ex.Message}";
                return null;
                }
            }
        // 内部创建实例方法
        private IPlcCommunication CreatePLCInstanceInternal( ) {
            try {
                IPlcCommunication plc = null;

                switch (PLCType) {
                    case PLCType.DeltaModbusTCP:
                        plc=new CDeltaModbusTcp(IPAddress,Port,SlaveID)
                            {
                            ByteOrder=ByteOrder,
                            StringByteOrder=StringByteOrder,
                            Timeout=Timeout
                            };
                        break;
                    default:
                        throw new NotSupportedException($"不支持的PLC类型: {PLCType}");
                    }

                // 设置心跳配置
                if (HeartbeatEnabled&&!string.IsNullOrEmpty(HeartbeatAddress)) {
                    plc.SetHeartbeatAddress(HeartbeatAddress);
                    plc.SetHeartbeatInterval(HeartbeatInterval);
                    }

                return plc;
                }
            catch (Exception ex) {
                LastError=$"创建PLC实例失败: {ex.Message}";
                throw;
                }
            }
        public bool Connect( ) {
            lock (_connectionLock) {
                var previousStatus = _connectionStatus;

                try {
                    if (IsConnected) {
                        OnConnectionStatusChanged(previousStatus,ConnectionStatus.Connected,"已经连接");
                        return true;
                        }

                    // 设置连接中状态
                    _connectionStatus=ConnectionStatus.Connecting;
                    OnConnectionStatusChanged(previousStatus,_connectionStatus,"正在连接...");

                    // 创建PLC实例
                    _plcInstance=CreatePLCInstanceInternal( );

                    bool result = _plcInstance.Initialize();

                    if (result) {
                        _isConnected=true;
                        _connectionStatus=ConnectionStatus.Connected;

                        if (HeartbeatEnabled) {
                            _plcInstance.SetHeartbeatAddress(HeartbeatAddress);
                            _plcInstance.SetHeartbeatInterval(HeartbeatInterval);
                            _plcInstance.StartHeartbeatService( );
                            }

                        // 订阅事件
                        _plcInstance.HeartbeatStatusChanged+=OnHeartbeatStatusChanged;

                        OnConnectionStatusChanged(previousStatus,_connectionStatus,"连接成功");
                        LoggingService.Info($"PLC连接成功: {Name}");
                        }
                    else {
                        _connectionStatus=ConnectionStatus.Faulted;
                        OnConnectionStatusChanged(previousStatus,_connectionStatus,"连接失败");
                        }

                    return result;
                    }
                catch (Exception ex) {
                    _connectionStatus=ConnectionStatus.Faulted;
                    OnConnectionStatusChanged(previousStatus,_connectionStatus,$"连接异常: {ex.Message}");
                    LoggingService.Error($"PLC连接异常: {Name}",ex);
                    return false;
                    }
                }
            }
        protected virtual void OnConnectionStatusChanged( ConnectionStatus previous,ConnectionStatus current,string message ) {
            ConnectionStatusChanged?.Invoke(this,new ConnectionStatusChangedEventArgs
                {
                PreviousStatus=previous,
                CurrentStatus=current,
                Message=message
                });
            }
        public void Disconnect( ) {
            lock (_connectionLock) {
                try {
                    if (_plcInstance!=null) {
                        _plcInstance.HeartbeatStatusChanged-=OnHeartbeatStatusChanged;
                        _plcInstance.StopHeartbeatService( );
                        _plcInstance.Close( );
                        }

                    _isConnected=false;
                    _connectionStatus=ConnectionStatus.Disconnected;
                    LoggingService.Info($"PLC连接已关闭: {Name}");
                    }
                catch (Exception ex) {
                    LoggingService.Error($"关闭PLC连接异常: {Name}",ex);
                    _connectionStatus=ConnectionStatus.Faulted;
                    }
                }
            }
        // 心跳状态变化处理
        private void OnHeartbeatStatusChanged( object sender,HeartbeatEventArgs e ) {
            lock (_connectionLock) {
                if (!e.IsAlive) {
                    _connectionStatus=ConnectionStatus.Faulted;
                    LoggingService.Warn($"PLC心跳检测失败: {Name}, 重试次数: {e.RetryCount}");

                    // 如果重试次数过多，自动尝试重连
                    if (e.RetryCount>=3) {
                        LoggingService.Info($"尝试自动重连PLC: {Name}");
                        ThreadPool.QueueUserWorkItem(_ => {
                            if (Connect( )) {
                                LoggingService.Info($"PLC自动重连成功: {Name}");
                                }
                        });
                        }
                    }
                else {
                    _connectionStatus=ConnectionStatus.Connected;
                    }
                }
            }
        // 添加资源清理
        public void Dispose( ) {
            Disconnect( );
            _plcInstance?.Dispose( );
            _plcInstance=null;
            }
        /// <summary>
        /// 测试连接
        /// </summary>
        public bool TestConnection( ) {
            lock (_connectionLock) {
                try {
                    if (_connectionStatus!=ConnectionStatus.Connected) {
                        if (!Connect( )) {
                            return false;
                            }
                        }

                    // 使用现有的PLC实例进行测试
                    int testValue = 0;
                    var result = PLCInstance.ReadRegister("0", ref testValue);

                    if (!result) {
                        // 测试失败，断开连接
                        Disconnect( );
                        }

                    return result;
                    }
                catch (Exception ex) {
                    LoggingService.Error($"PLC连接测试失败: {Name}",ex);
                    Disconnect( );
                    return false;
                    }
                }
            }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void CloseConnection( ) {
            try {
                if (PLCInstance!=null) {
                    PLCInstance.StopHeartbeatService( );
                    PLCInstance.Close( );
                    }
                }
            catch (Exception ex) {
                LastError=$"关闭连接异常: {ex.Message}";
                }
            }

        /// <summary>
        /// 开始自动读取
        /// </summary>
        public void StartAutoRead( ) {
            try {
                if (PLCInstance!=null&&PLCInstance.IsConnected) {
                    // 添加扫描地址到自动读取
                    foreach (var address in ScanAddresses.Where(a => a.IsEnabled)) {
                        PLCInstance.AddOrUpdateAutoRead(
                            $"{Name}_{address.Key}",
                            address.Address,
                            address.DataType,
                            address.ReadInterval,
                            address.Power,
                            address.Length,
                            20,
                            true
                        );

                        LoggingService.Info($"已添加自动读取地址: {address.Name} ({address.Address})");
                        }

                    PLCInstance.StartAutoRead( );
                    LoggingService.Info($"已启动自动读取: {Name}");
                    }
                else {
                    LoggingService.Warn($"无法启动自动读取: PLC未连接 - {Name}");
                    }
                }
            catch (Exception ex) {
                LastError=$"启动自动读取失败: {ex.Message}";
                LoggingService.Error($"启动自动读取失败: {Name}",ex);
                throw;
                }
            }

        /// <summary>
        /// 停止自动读取
        /// </summary>
        public void StopAutoRead( ) {
            try {
                PLCInstance?.StopAutoRead( );
                }
            catch (Exception ex) {
                LastError=$"停止自动读取失败: {ex.Message}";
                }
            }
        }
    }
