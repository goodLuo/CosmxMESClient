using CosmxMESClient.Communication;
using CosmxMESClient.interfaceConfig;
using CosmxMESClient.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CosmxMESClient {
    public partial class FormPLCConfig:Form {
        private BindingList<PLCConnectionConfig> _plcConfigs;
        private PLCConnectionConfig _currentConfig;
        private readonly Dictionary<string, PLCConnectionConfig> _activeConnections = new Dictionary<string, PLCConnectionConfig>();
        private readonly object _connectionsLock = new object();
        private System.Timers.Timer _connectionMonitorTimer;
        private BindingSource _bindingSource;

        private BindingSource _scanAddressesBindingSource = new BindingSource();
        private BindingSource _sendAddressesBindingSource = new BindingSource();

        public FormPLCConfig( ) {
            InitializeComponent( );
            InitializeData( );
            StartConnectionMonitor( );
            }
        private void InitializeData( ) {
            // 创建绑定源
            _bindingSource=new BindingSource( );
            _plcConfigs=new BindingList<PLCConnectionConfig>( );
            _bindingSource.DataSource=_plcConfigs;
            // 数据绑定
            dgvPLCConfigs.DataSource=_bindingSource;
            // 添加状态列（如果还没有的话）
            if (!dgvPLCConfigs.Columns.Contains("colStatus")) {
                var statusColumn = new DataGridViewTextBoxColumn
                    {
                    Name = "colStatus",
                    HeaderText = "状态",
                    DataPropertyName = "IsConnected",
                    ReadOnly = true
                    };
                dgvPLCConfigs.Columns.Add(statusColumn);
                }
            // 设置控件数据绑定
            SetupDataBindings( );

            // 初始化下拉框
            cmbPLCType.DataSource=Enum.GetValues(typeof(PLCType));
            cmbByteOrder.DataSource=Enum.GetValues(typeof(ByteOrderEnum));
            cmbStringByteOrder.DataSource=Enum.GetValues(typeof(StringByteOrderEnum));
            LoadDefaultValues( );
            LoadSavedConfigs( );
            // 初始化地址列表的绑定源
            _scanAddressesBindingSource.DataSource=typeof(BindingList<PLCScanAddress>);
            _sendAddressesBindingSource.DataSource=typeof(BindingList<PLCSendAddress>);

            // 设置ListView的数据绑定
            SetupListViewDataBindings( );

            }
        private void SetupListViewDataBindings( ) {
            // 清除现有列
            lvScanAddresses.Columns.Clear( );
            lvSendAddresses.Columns.Clear( );

            // 配置扫描地址列表的列
            lvScanAddresses.Columns.Add("地址",120);
            lvScanAddresses.Columns.Add("数据类型",100);
            lvScanAddresses.Columns.Add("描述",200);
            lvScanAddresses.Columns.Add("扫描间隔(ms)",100);
            lvScanAddresses.Columns.Add("启用状态",80);

            // 配置发送地址列表的列
            lvSendAddresses.Columns.Add("地址",120);
            lvSendAddresses.Columns.Add("数据类型",100);
            lvSendAddresses.Columns.Add("描述",200);
            lvSendAddresses.Columns.Add("发送间隔(ms)",100);
            lvSendAddresses.Columns.Add("自动发送",80);
            lvSendAddresses.Columns.Add("启用状态",80);
            }
        private void SetupDataBindings( ) {
            // 清除现有绑定
            txtName.DataBindings.Clear( );
            txtIPAddress.DataBindings.Clear( );
            numPort.DataBindings.Clear( );
            numSlaveID.DataBindings.Clear( );
            numTimeout.DataBindings.Clear( );
            cmbByteOrder.DataBindings.Clear( );
            cmbStringByteOrder.DataBindings.Clear( );
            chkHeartbeatEnabled.DataBindings.Clear( );
            numHeartbeatInterval.DataBindings.Clear( );
            txtHeartbeatAddress.DataBindings.Clear( );
            chkEnabled.DataBindings.Clear( );

            // 设置新的数据绑定
            txtName.DataBindings.Add("Text",_bindingSource,"Name",false,DataSourceUpdateMode.OnPropertyChanged);
            txtIPAddress.DataBindings.Add("Text",_bindingSource,"IPAddress",false,DataSourceUpdateMode.OnPropertyChanged);
            numPort.DataBindings.Add("Value",_bindingSource,"Port",false,DataSourceUpdateMode.OnPropertyChanged);
            numSlaveID.DataBindings.Add("Value",_bindingSource,"SlaveID",false,DataSourceUpdateMode.OnPropertyChanged);
            numTimeout.DataBindings.Add("Value",_bindingSource,"Timeout",false,DataSourceUpdateMode.OnPropertyChanged);
            cmbByteOrder.DataBindings.Add("SelectedItem",_bindingSource,"ByteOrder",false,DataSourceUpdateMode.OnPropertyChanged);
            cmbStringByteOrder.DataBindings.Add("SelectedItem",_bindingSource,"StringByteOrder",false,DataSourceUpdateMode.OnPropertyChanged);
            chkHeartbeatEnabled.DataBindings.Add("Checked",_bindingSource,"HeartbeatEnabled",false,DataSourceUpdateMode.OnPropertyChanged);
            numHeartbeatInterval.DataBindings.Add("Value",_bindingSource,"HeartbeatInterval",false,DataSourceUpdateMode.OnPropertyChanged);
            txtHeartbeatAddress.DataBindings.Add("Text",_bindingSource,"HeartbeatAddress",false,DataSourceUpdateMode.OnPropertyChanged);
            chkEnabled.DataBindings.Add("Checked",_bindingSource,"IsEnabled",false,DataSourceUpdateMode.OnPropertyChanged);
            }

        // 统一的连接管理方法
        public bool ConnectPLC( PLCConnectionConfig config ) {
            lock (_connectionsLock) {
                if (_activeConnections.ContainsKey(config.Key)) {
                    var existingConfig = _activeConnections[config.Key];

                    // 检查连接状态
                    if (existingConfig.Status==PLCConnectionConfig.ConnectionStatus.Connected) {
                        return true;
                        }
                    else if (existingConfig.Status==PLCConnectionConfig.ConnectionStatus.Connecting) {
                        LoggingService.Warn($"PLC连接正在进行中: {config.Name}");
                        return false;
                        }
                    else {
                        // 移除无效连接
                        _activeConnections.Remove(config.Key);
                        }
                    }

                if (config.Connect( )) {
                    _activeConnections[config.Key]=config;
                    UpdatePLCStatus(config,true);
                    return true;
                    }

                UpdatePLCStatus(config,false);
                return false;
                }
            }

        public void DisconnectPLC( PLCConnectionConfig config ) {
            lock (_connectionsLock) {
                if (_activeConnections.ContainsKey(config.Key)) {
                    _activeConnections[config.Key].Disconnect( );
                    _activeConnections.Remove(config.Key);
                    }
                else {
                    config.Disconnect( );
                    }
                UpdatePLCStatus(config,false);
                }
            }

        public void DisconnectAll( ) {
            lock (_connectionsLock) {
                var keys = _activeConnections.Keys.ToList();
                foreach (var key in keys) {
                    try {
                        if (_activeConnections.TryGetValue(key,out var config)) {
                            config.Disconnect( );
                            }
                        }
                    catch (Exception ex) {
                        LoggingService.Error($"断开PLC连接失败: {key}",ex);
                        }
                    }
                _activeConnections.Clear( );
                }
            }
        private void DgvPLCConfigs_DataError( object sender,DataGridViewDataErrorEventArgs e ) {
            // 忽略布尔类型转换错误
            if (e.Exception is FormatException&&e.Exception.Message.Contains("布尔值")) {
                e.ThrowException=false; // 阻止显示默认错误对话框
                return;
                }

            // 记录其他错误
            LoggingService.Error($"DataGridView数据错误: {e.Exception.Message}");
            }
        private void LoadSavedConfigs( ) {
            var configs = PLCConfigManager.LoadConfigs();
            _plcConfigs.Clear( );

            foreach (var config in configs) {
                // 为每个配置创建PLC实例
                config.CreatePLCInstance( );
                _plcConfigs.Add(config);
                }

            if (_plcConfigs.Count>0) {
                dgvPLCConfigs.ClearSelection( );
                dgvPLCConfigs.Rows[0].Selected=true;
                }
            }
        private void LoadDefaultValues( ) {
            txtName.Text="New_PLC";
            cmbPLCType.SelectedIndex=0;
            txtIPAddress.Text="192.168.1.100";
            numPort.Value=502;
            numSlaveID.Value=1;
            numTimeout.Value=1000;
            cmbByteOrder.SelectedIndex=0;
            cmbStringByteOrder.SelectedIndex=0;
            numHeartbeatInterval.Value=30000;
            txtHeartbeatAddress.Text="100";
            chkEnabled.Checked=true;
            chkHeartbeatEnabled.Checked=true;
            }

        private void LoadConfigToForm( PLCConnectionConfig config ) {
            txtName.Text=config.Name;
            cmbPLCType.SelectedItem=config.PLCType;
            txtIPAddress.Text=config.IPAddress;
            numPort.Value=config.Port;
            numSlaveID.Value=config.SlaveID;
            numTimeout.Value=config.Timeout;
            cmbByteOrder.SelectedItem=config.ByteOrder;
            cmbStringByteOrder.SelectedItem=config.StringByteOrder;
            chkHeartbeatEnabled.Checked=config.HeartbeatEnabled;
            numHeartbeatInterval.Value=config.HeartbeatInterval;
            txtHeartbeatAddress.Text=config.HeartbeatAddress;
            chkEnabled.Checked=config.IsEnabled;

           // LoadScanAddresses(config.ScanAddresses.Values.ToList( ));
           // LoadSendAddresses(config.SendAddresses.Values.ToList());
            }

        private void LoadScanAddresses( List<PLCScanAddress> addresses ) {
            lvScanAddresses.Items.Clear( );

            foreach (var address in addresses.OrderBy(a => a.Name)) {
                var item = new ListViewItem(address.Key);
                item.SubItems.Add(address.Name);
                item.SubItems.Add(address.Address);
                item.SubItems.Add(GetDataTypeDisplayName(address.DataType));
                item.SubItems.Add(address.Description);
                item.SubItems.Add(address.ReadInterval.ToString( ));
                item.SubItems.Add(address.Power.ToString( ));
                item.SubItems.Add(address.Length.ToString( ));
                item.SubItems.Add(address.IsEnabled ? "启用" : "禁用");

                // 设置行颜色
                if (!address.IsEnabled)
                    item.BackColor=Color.LightGray;
                else if (address.ReadInterval<500)
                    item.BackColor=Color.LightCyan; // 高频扫描

                item.Tag=address;
                lvScanAddresses.Items.Add(item);
                }

            UpdateScanAddressesSummary( );
            }
        private string GetDataTypeDisplayName( Type dataType ) {
            if (dataType==null)
                return "未知";

            switch (dataType.Name) {
                case "Boolean":
                    return "布尔";
                case "Int32":
                    return "整数";
                case "Single":
                    return "浮点数";
                case "Double":
                    return "双精度";
                case "String":
                    return "字符串";
                default:
                    return dataType.Name;
                }
            }
        private void UpdateScanAddressesSummary( ) {
            var enabledCount = _currentConfig.ScanAddresses.Count(a => a.IsEnabled);
            var highFreqCount = _currentConfig.ScanAddresses.Count(a => a.IsEnabled && a.ReadInterval < 500);

            grpScanAddresses.Text=$"扫描地址配置 (总数: {_currentConfig.ScanAddresses.Count}, 启用: {enabledCount}, 高频: {highFreqCount})";
            }
        private bool ValidateForm( ) {
            if (string.IsNullOrEmpty(txtName.Text)) {
                MessageBox.Show("请输入PLC名称","验证错误",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return false;
                }

            if (string.IsNullOrEmpty(txtIPAddress.Text)) {
                MessageBox.Show("请输入IP地址","验证错误",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return false;
                }

            return true;
            }

        private void SaveFormToConfig( PLCConnectionConfig config ) {
            config.Name=txtName.Text;
            config.PLCType=(PLCType) cmbPLCType.SelectedItem;
            config.IPAddress=txtIPAddress.Text;
            config.Port=(int) numPort.Value;
            config.SlaveID=(byte) numSlaveID.Value;
            config.Timeout=(int) numTimeout.Value;
            config.ByteOrder=(ByteOrderEnum) cmbByteOrder.SelectedItem;
            config.StringByteOrder=(StringByteOrderEnum) cmbStringByteOrder.SelectedItem;
            config.HeartbeatEnabled=chkHeartbeatEnabled.Checked;
            config.HeartbeatInterval=(int) numHeartbeatInterval.Value;
            config.HeartbeatAddress=txtHeartbeatAddress.Text;
            config.IsEnabled=chkEnabled.Checked;
            }

        private void RefreshDataGridView( ) {
            dgvPLCConfigs.Refresh( );
            }
        private void btnAdd_Click( object sender,EventArgs e ) {
            try {
                // 生成唯一名称
                string baseName = $"PLC_{_plcConfigs.Count + 1}";
                string uniqueName = GetUniquePLCName(baseName);

                // 使用安全的默认值创建配置
                var config = new PLCConnectionConfig
                    {
                    Name = uniqueName,
                    PLCType = PLCType.DeltaModbusTCP,
                    IPAddress = "192.168.1.100",
                    Port = 502,          // 使用有效端口号
                    SlaveID = 1,         // 使用有效站号
                    Timeout = 1000,      // 使用有效超时时间
                    ByteOrder = ByteOrderEnum.BigEndian,
                    StringByteOrder = StringByteOrderEnum.Normal,
                    HeartbeatEnabled = true,
                    HeartbeatInterval = 30000,  // 使用有效心跳间隔
                    HeartbeatAddress = "100",
                    IsEnabled = true
                    };

                _plcConfigs.Add(config);

                // 选中新添加的配置
                _bindingSource.Position=_plcConfigs.Count-1;
                _currentConfig=config;

                // 加载到表单显示
                LoadConfigToForm(_currentConfig);

                LoggingService.Info($"添加新的PLC配置: {config.Name}");
                }
            catch (Exception ex) {
                LoggingService.Error("添加PLC配置失败",ex);
                MessageBox.Show($"添加配置失败: {ex.Message}","错误",
                    MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
        private string GetUniquePLCName( string baseName ) {
            if (_plcConfigs==null||_plcConfigs.Count==0)
                return baseName;

            string name = baseName;
            int counter = 1;

            // 检查名称是否已存在，如果存在则添加序号
            while (_plcConfigs.Any(config => config.Name.Equals(name,StringComparison.OrdinalIgnoreCase))) {
                name=$"{baseName}_{counter++}";

                // 防止无限循环，设置最大尝试次数
                if (counter>1000) {
                    // 如果尝试1000次后仍有冲突，使用GUID确保唯一性
                    name=$"{baseName}_{Guid.NewGuid( ):N.Substring(0, 8)}";
                    break;
                    }
                }

            return name;
            }
        private void btnDelete_Click( object sender,EventArgs e ) {
            if (dgvPLCConfigs.SelectedRows.Count>0) {
                var selected = dgvPLCConfigs.SelectedRows[0].DataBoundItem as PLCConnectionConfig;
                if (selected!=null) {
                    _plcConfigs.Remove(selected);
                    ClearForm( );
                    }
                }
            }
        private bool ValidateNumericValues( ) {
            try {
                // 验证端口号
                if (numPort.Value<numPort.Minimum||numPort.Value>numPort.Maximum) {
                    MessageBox.Show($"端口号必须在{numPort.Minimum}-{numPort.Maximum}之间","验证错误",
                        MessageBoxButtons.OK,MessageBoxIcon.Warning);
                    numPort.Focus( );
                    return false;
                    }

                // 验证站号
                if (numSlaveID.Value<numSlaveID.Minimum||numSlaveID.Value>numSlaveID.Maximum) {
                    MessageBox.Show($"站号必须在{numSlaveID.Minimum}-{numSlaveID.Maximum}之间","验证错误",
                        MessageBoxButtons.OK,MessageBoxIcon.Warning);
                    numSlaveID.Focus( );
                    return false;
                    }

                // 验证超时时间
                if (numTimeout.Value<numTimeout.Minimum||numTimeout.Value>numTimeout.Maximum) {
                    MessageBox.Show($"超时时间必须在{numTimeout.Minimum}-{numTimeout.Maximum}之间","验证错误",
                        MessageBoxButtons.OK,MessageBoxIcon.Warning);
                    numTimeout.Focus( );
                    return false;
                    }

                // 验证心跳间隔
                if (numHeartbeatInterval.Value<numHeartbeatInterval.Minimum||
                    numHeartbeatInterval.Value>numHeartbeatInterval.Maximum) {
                    MessageBox.Show($"心跳间隔必须在{numHeartbeatInterval.Minimum}-{numHeartbeatInterval.Maximum}之间","验证错误",
                        MessageBoxButtons.OK,MessageBoxIcon.Warning);
                    numHeartbeatInterval.Focus( );
                    return false;
                    }

                return true;
                }
            catch (Exception ex) {
                LoggingService.Error("数值验证失败",ex);
                return false;
                }
            }
        private void btnSaveConfig_Click( object sender,EventArgs e ) {
            if (!ValidateNumericValues( )) {
                return;
                }
            // 数据绑定自动保存，这里只需要保存到文件
            PLCConfigManager.SaveConfigs(_plcConfigs.ToList( ));
            MessageBox.Show("配置已保存");
            }
        private void btnConfigScanAddresses_Click( object sender,EventArgs e ) {
            if (_currentConfig==null)
                return;

            using (var form = new FormPLCAddressConfig(_currentConfig,AddressDirection.ReadOnly)) {
                if (form.ShowDialog( )==DialogResult.OK) {
                    RefreshAddressLists( ); // 刷新显示
                    }
                }
            }
        private void btnConfigSendAddresses_Click( object sender,EventArgs e ) {
            if (_currentConfig==null)
                return;

            using (var form = new FormPLCAddressConfig(_currentConfig,AddressDirection.WriteOnly)) {
                if (form.ShowDialog( )==DialogResult.OK) {
                    RefreshAddressLists( ); // 刷新显示
                    }
                }
            }
        private void btnStartAutoRead_Click( object sender,EventArgs e ) {
            if (_currentConfig==null) {
                MessageBox.Show("请先选择PLC配置","提示",
                    MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
                }

            try {
                // 确保连接
                if (!ConnectPLC(_currentConfig)) {
                    MessageBox.Show("PLC连接失败，无法启动自动读取","错误",
                        MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                    }

                _currentConfig.StartAutoRead( );
                UpdateAutoReadStatus(true);
                LoggingService.Info($"启动自动读取 - {_currentConfig.Name}");
                }
            catch (Exception ex) {
                LoggingService.Error("启动自动读取失败",ex);
                MessageBox.Show($"启动自动读取失败: {ex.Message}","错误",
                    MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
        private void btnStopAutoRead_Click( object sender,EventArgs e ) {
            if (_currentConfig==null)
                return;

            try {
                _currentConfig.StopAutoRead( );
                UpdateAutoReadStatus(false);
                LoggingService.Info($"停止自动读取 - {_currentConfig.Name}");

                // 注意：这里不断开连接，因为可能还有其他操作需要使用
                }
            catch (Exception ex) {
                LoggingService.Error("停止自动读取失败",ex);
                MessageBox.Show($"停止自动读取失败: {ex.Message}","错误",
                    MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
        // 定期检查连接状态
        private void StartConnectionMonitor( ) {
            _connectionMonitorTimer=new System.Timers.Timer(30000); // 30秒检查一次
            _connectionMonitorTimer.Elapsed+=( s,e ) => CheckConnectionsHealth( );
            _connectionMonitorTimer.Start( );
            }
        private void CheckConnectionsHealth( ) {
            lock (_connectionsLock) {
                var brokenConnections = new List<string>();
                var needsReconnect = new List<PLCConnectionConfig>();

                foreach (var kvp in _activeConnections) {
                    var config = kvp.Value;

                    try {
                        // 检查连接状态
                        if (config.Status==PLCConnectionConfig.ConnectionStatus.Faulted) {
                            LoggingService.Warn($"检测到故障的PLC连接: {config.Name}");
                            needsReconnect.Add(config);
                            }
                        else if (config.Status==PLCConnectionConfig.ConnectionStatus.Disconnected) {
                            brokenConnections.Add(kvp.Key);
                            LoggingService.Warn($"检测到断开的PLC连接: {config.Name}");
                            }
                        else if (config.Status==PLCConnectionConfig.ConnectionStatus.Connected) {
                            // 定期测试连接有效性
                            int testValue = 0;
                            if (!config.PLCInstance.ReadRegister("0",ref testValue)) {
                                LoggingService.Warn($"PLC连接测试失败: {config.Name}");
                                brokenConnections.Add(kvp.Key);
                                }
                            }
                        }
                    catch (Exception ex) {
                        LoggingService.Error($"检查PLC连接状态异常: {config.Name}",ex);
                        brokenConnections.Add(kvp.Key);
                        }
                    }

                // 移除断开的连接
                foreach (var key in brokenConnections) {
                    _activeConnections.Remove(key);
                    }

                // 尝试自动重连
                foreach (var config in needsReconnect) {
                    try {
                        LoggingService.Info($"尝试自动重连PLC: {config.Name}");
                        if (config.Connect( )) {
                            _activeConnections[config.Key]=config;
                            UpdatePLCStatus(config,true);
                            LoggingService.Info($"PLC自动重连成功: {config.Name}");
                            }
                        }
                    catch (Exception ex) {
                        LoggingService.Error($"自动重连PLC失败: {config.Name}",ex);
                        }
                    }

                // 更新UI显示
                if (brokenConnections.Count>0||needsReconnect.Count>0) {
                    this.Invoke(new Action(( ) => {
                        foreach (DataGridViewRow row in dgvPLCConfigs.Rows) {
                            var config = row.DataBoundItem as PLCConnectionConfig;
                            if (config!=null) {
                                bool isConnected = _activeConnections.ContainsKey(config.Key) &&
                                            config.Status == PLCConnectionConfig.ConnectionStatus.Connected;
                                UpdatePLCStatus(config,isConnected);
                                }
                            }
                    }));
                    }
                }
            }
        private void UpdateAutoReadStatus( bool isRunning ) {
            btnStartAutoRead.Enabled=!isRunning;
            btnStopAutoRead.Enabled=isRunning;

            btnStartAutoRead.BackColor=isRunning ? Color.LightGray : Color.LightGreen;
            btnStopAutoRead.BackColor=isRunning ? Color.LightCoral : Color.LightGray;
            }
        private void dgvPLCConfigs_SelectionChanged( object sender,EventArgs e ) {
            if (dgvPLCConfigs.SelectedRows.Count>0) {
                _currentConfig=dgvPLCConfigs.SelectedRows[0].DataBoundItem as PLCConnectionConfig;
                _bindingSource.Position=_plcConfigs.IndexOf(_currentConfig);

                // 更新地址列表的显示
                RefreshAddressLists( );
                }
            }
        private void RefreshAddressLists( ) {
            if (_currentConfig==null)
                return;

            // 清空现有项
            lvScanAddresses.Items.Clear( );
            lvSendAddresses.Items.Clear( );

            // 绑定扫描地址
            _scanAddressesBindingSource.DataSource=_currentConfig.ScanAddresses;
            foreach (var address in _currentConfig.ScanAddresses) {
                var item = new ListViewItem(address.Address);
                item.SubItems.Add(GetDataTypeDisplayName(address.DataType));
                item.SubItems.Add(address.Description);
                item.SubItems.Add(address.ReadInterval.ToString( ));
                item.SubItems.Add(address.IsEnabled ? "启用" : "禁用");
                item.Tag=address; // 保存对象引用

                // 设置行颜色
                if (!address.IsEnabled)
                    item.BackColor=Color.LightGray;
                else if (address.ReadInterval<500)
                    item.BackColor=Color.LightCyan;

                lvScanAddresses.Items.Add(item);
                }

            // 绑定发送地址
            _sendAddressesBindingSource.DataSource=_currentConfig.SendAddresses;
            foreach (var address in _currentConfig.SendAddresses) {
                var item = new ListViewItem(address.Address);
                item.SubItems.Add(GetDataTypeDisplayName(address.DataType));
                item.SubItems.Add(address.Description);
                item.SubItems.Add(address.ReadInterval.ToString( ));
                item.SubItems.Add(address.AutoSend ? "是" : "否");
                item.SubItems.Add(address.IsEnabled ? "启用" : "禁用");
                item.Tag=address; // 保存对象引用

                // 设置行颜色
                if (!address.IsEnabled)
                    item.BackColor=Color.LightGray;
                else if (address.AutoSend)
                    item.BackColor=Color.LightYellow;

                lvSendAddresses.Items.Add(item);
                }

            UpdateAddressesSummary( );
            }
        private void UpdateAddressesSummary( ) {
            if (_currentConfig==null)
                return;

            var scanEnabledCount = _currentConfig.ScanAddresses.Count(a => a.IsEnabled);
            var sendEnabledCount = _currentConfig.SendAddresses.Count(a => a.IsEnabled);

            grpScanAddresses.Text=$"扫描地址配置 (总数: {_currentConfig.ScanAddresses.Count}, 启用: {scanEnabledCount})";
            grpSendAddresses.Text=$"发送地址配置 (总数: {_currentConfig.SendAddresses.Count}, 启用: {sendEnabledCount})";
            }
        private void LoadSendAddresses( List<PLCSendAddress> addresses ) {

            lvSendAddresses.Items.Clear( );

            foreach (var address in addresses.OrderBy(a => a.Name)) {
                var item = new ListViewItem(address.Key);
                item.SubItems.Add(address.Name);
                item.SubItems.Add(address.Address);
                item.SubItems.Add(GetDataTypeDisplayName(address.DataType));
                item.SubItems.Add(address.Description);
                item.SubItems.Add(address.ReadInterval.ToString( ));
                item.SubItems.Add(address.AutoSend ? "是" : "否");
                item.SubItems.Add(address.SendDelay.ToString( ));
                item.SubItems.Add(address.IsEnabled ? "启用" : "禁用");

                // 设置行颜色
                if (!address.IsEnabled)
                    item.BackColor=Color.LightGray;
                else if (address.AutoSend)
                    item.BackColor=Color.LightYellow;

                item.Tag=address;
                lvSendAddresses.Items.Add(item);
                }

            UpdateSendAddressesSummary( );
            }
        private void UpdateSendAddressesSummary( ) {
            if (_currentConfig==null||_currentConfig.SendAddresses==null) {
                grpSendAddresses.Text="发送地址配置 (总数: 0, 启用: 0, 自动发送: 0)";
                return;
                }

            var enabledCount = _currentConfig.SendAddresses.Count(a => a.IsEnabled);
            var autoSendCount = _currentConfig.SendAddresses.Count(a => a.AutoSend && a.IsEnabled);

            grpSendAddresses.Text=$"发送地址配置 (总数: {_currentConfig.SendAddresses.Count}, 启用: {enabledCount}, 自动发送: {autoSendCount})";
            }
        private void btnDeleteScanAddress_Click( object sender,EventArgs e ) {
            if (lvScanAddresses.SelectedItems.Count==0)
                return;

            var address = lvScanAddresses.SelectedItems[0].Tag as PLCScanAddress;
            if (address==null)
                return;

            var result = MessageBox.Show($"确定要删除地址 '{address.Name}' 吗？", "确认删除",
        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result==DialogResult.Yes) {
                try {
                    // 使用新的Remove方法
                    if (_currentConfig.RemoveScanAddress(address.Key)) {
                    //    LoadScanAddresses(_currentConfig.ScanAddresses());
                        LoggingService.Info($"删除扫描地址成功: {address.Name}");
                        }
                    }
                catch (Exception ex) {
                    LoggingService.Error("删除扫描地址失败",ex);
                    MessageBox.Show($"删除失败: {ex.Message}","错误",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                }
            }
        private void btnTestConnection_Click( object sender,EventArgs e ) {
            if (!ValidateNumericValues( )) {
                return;
                }
            if (_currentConfig==null)
                return;

            // 直接使用当前绑定对象的参数
            bool success = _currentConfig.Connect();

            if (success)
                MessageBox.Show("连接成功");
            else
                MessageBox.Show("连接失败");
            }
        private void btnLoadConfigs_Click( object sender,EventArgs e ) {
            var configs = PLCConfigManager.LoadConfigs();
            _plcConfigs.Clear( );
            foreach (var config in configs) {
                _plcConfigs.Add(config);
                }
            MessageBox.Show($"已加载 {configs.Count} 个PLC配置","成功",
                MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        private void btnClose_Click( object sender,EventArgs e ) {
            this.DialogResult=DialogResult.OK;
            this.Close( );
            }
        private void btnSaveAll_Click( object sender,EventArgs e ) {
            try {
                // 先保存当前编辑的配置
                if (_currentConfig!=null&&ValidateForm( )) {
                    SaveFormToConfig(_currentConfig);
                    }

                // 保存所有配置到文件
                PLCConfigManager.SaveConfigs(_plcConfigs.ToList( ));

                MessageBox.Show($"成功保存 {_plcConfigs.Count} 个PLC配置","成功",
                    MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
            catch (Exception ex) {
                MessageBox.Show($"保存配置失败: {ex.Message}","错误",
                    MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
        private void DgvPLCConfigs_CellFormatting( object sender,DataGridViewCellFormattingEventArgs e ) {
            if (dgvPLCConfigs.Columns[e.ColumnIndex].Name=="colStatus"&&e.Value is bool) {
                e.Value=(bool) e.Value ? "已连接" : "未连接";
                e.FormattingApplied=true;
                }

            if (dgvPLCConfigs.Columns[e.ColumnIndex].Name=="colEnabled"&&e.Value is bool) {
                e.Value=(bool) e.Value ? "启用" : "禁用";
                e.FormattingApplied=true;
                }
            }
        private void OnPLCLog( string message ) {
            if (this.InvokeRequired) {
                this.Invoke(new Action<string>(OnPLCLog),message);
                return;
                }

            // 可以在这里添加日志显示逻辑
            System.Diagnostics.Debug.WriteLine($"[PLC Log] {message}");
            }
        private void OnPLCHeartbeatStatusChanged( object sender,HeartbeatEventArgs e ) {
            if (this.InvokeRequired) {
                this.Invoke(new EventHandler<HeartbeatEventArgs>(OnPLCHeartbeatStatusChanged),sender,e);
                return;
                }

            // 更新心跳状态显示
            if (_currentConfig!=null&&sender==_currentConfig.PLCInstance) {
                UpdateHeartbeatStatus(e.IsAlive,e.Message,e.RetryCount);
                }
            }
        private void UpdatePLCStatus( PLCConnectionConfig config,bool isConnected ) {
            if (this.InvokeRequired) {
                this.Invoke(new Action<PLCConnectionConfig,bool>(UpdatePLCStatus),config,isConnected);
                return;
                }

            // 更新DataGridView中的状态显示
            foreach (DataGridViewRow row in dgvPLCConfigs.Rows) {
                var rowConfig = row.DataBoundItem as PLCConnectionConfig;
                if (rowConfig==config) {
                    row.Cells["colStatus"].Value=isConnected ? "已连接" : "未连接";
                    row.Cells["colStatus"].Style.ForeColor=isConnected ? Color.Green : Color.Red;
                    break;
                    }
                }
            }
        private void UpdateHeartbeatStatus( bool isAlive,string message,int retryCount ) {
            // 可以添加状态栏或标签显示心跳状态
            string status = isAlive ? $"心跳正常 (重试: {retryCount})" : $"心跳异常: {message}";
            System.Diagnostics.Debug.WriteLine($"[Heartbeat] {status}");
            }
        private void SetButtonsEnabled( bool enabled ) {
            btnTestConnection.Enabled=enabled;
            btnSaveConfig.Enabled=enabled;
            btnAdd.Enabled=enabled;
            btnDelete.Enabled=enabled;

            // 更新按钮文本显示连接状态
            if (_currentConfig!=null&&_currentConfig.IsConnected) {
                btnTestConnection.Text="断开连接";
                btnTestConnection.BackColor=Color.LightCoral;
                }
            else {
                btnTestConnection.Text="测试连接";
                btnTestConnection.BackColor=Color.LightGreen;
                }
            }
        private IPlcCommunication CreatePLCFromConfig( PLCConnectionConfig config ) {
            switch (config.PLCType) {
                case PLCType.DeltaModbusTCP:
                    return new CDeltaModbusTcp(config.IPAddress,config.Port,config.SlaveID)
                        {
                        ByteOrder=config.ByteOrder,
                        StringByteOrder=config.StringByteOrder,
                        Timeout=config.Timeout
                        };
                // 可以扩展其他PLC类型
                default:
                    throw new NotSupportedException($"不支持的PLC类型: {config.PLCType}");
                }
            }

        private void FormPLCConfig_FormClosing( object sender,FormClosingEventArgs e ) {
            _connectionMonitorTimer?.Stop( );
            _connectionMonitorTimer?.Dispose( );
            DisconnectAll( );
            }
        private void FormPLCConfig_Load( object sender,EventArgs e ) {

            }
        private void ClearForm( ) {
            //txtName.Text="";
            cmbPLCType.SelectedIndex=0;
            txtIPAddress.Text="192.168.1.100";
            numPort.Value=502;
            numSlaveID.Value=1;
            numTimeout.Value=1000;
            cmbByteOrder.SelectedIndex=0;
            cmbStringByteOrder.SelectedIndex=0;
            chkHeartbeatEnabled.Checked=true;
            numHeartbeatInterval.Value=30000;
            txtHeartbeatAddress.Text="100";
            chkEnabled.Checked=true;
            lvScanAddresses.Items.Clear( );
            }
        }

    public enum PLCType {
        DeltaModbusTCP,
        MitsubishiMCProtocol,
        SiemensS7,
        OmronFINS
        }
    }