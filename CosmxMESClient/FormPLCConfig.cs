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

namespace CosmxMESClient
{
    public partial class FormPLCConfig : Form
    {
        private BindingList<PLCConnectionConfig> _plcConfigs;
        private PLCConnectionConfig _currentConfig;
        private readonly Dictionary<string, PLCConnectionConfig> _activeConnections = new Dictionary<string, PLCConnectionConfig>();
        private readonly object _connectionsLock = new object();
        private System.Timers.Timer _connectionMonitorTimer;
        private BindingSource _bindingSource;
        private BindingSource _scanAddressesBindingSource = new BindingSource();
        private BindingSource _sendAddressesBindingSource = new BindingSource();
        public FormPLCConfig()
        {
            InitializeComponent();
            InitializeData();
            StartConnectionMonitor();
        }
        private void InitializeData()
        {
            // 创建绑定源
            _bindingSource = new BindingSource();
            _plcConfigs = new BindingList<PLCConnectionConfig>();
            _bindingSource.DataSource = _plcConfigs;
            // 数据绑定
            dgvPLCConfigs.DataSource = _bindingSource;
            // 添加状态列（如果还没有的话）
            if (!dgvPLCConfigs.Columns.Contains("colStatus"))
            {
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
            SetupDataBindings();
            // 初始化地址列表的绑定源
            //_scanAddressesBindingSource.DataSource = typeof(BindingList<PLCScanAddress>);
            //_sendAddressesBindingSource.DataSource = typeof(BindingList<PLCSendAddress>);


            // 初始化下拉框
            cmbPLCType.DataSource = Enum.GetValues(typeof(PLCType));
            cmbByteOrder.DataSource = Enum.GetValues(typeof(ByteOrderEnum));
            cmbStringByteOrder.DataSource = Enum.GetValues(typeof(StringByteOrderEnum));
            LoadDefaultValues();
            LoadSavedConfigs();
            // 初始化地址列表的绑定源
            //_scanAddressesBindingSource.DataSource = typeof(BindingList<PLCScanAddress>);
            //_sendAddressesBindingSource.DataSource = typeof(BindingList<PLCSendAddress>);
            _scanAddressesBindingSource.DataSource = new BindingList<PLCScanAddress>();
            _sendAddressesBindingSource.DataSource = new BindingList<PLCSendAddress>();


            // 设置数据绑定
            SetupDataGridViewDataBindings();

        }
        private void SetupDataGridViewDataBindings()
        {
            // 扫描地址DataGridView的数据绑定
            dgvScanAddresses.DataSource = _scanAddressesBindingSource;

            // 发送地址DataGridView的数据绑定
            dgvSendAddresses.DataSource = _sendAddressesBindingSource;

            // 设置单元格格式化
            dgvScanAddresses.CellFormatting += DgvAddresses_CellFormatting;
            dgvSendAddresses.CellFormatting += DgvAddresses_CellFormatting;

            // 添加行预处理事件
            dgvScanAddresses.RowPrePaint += DgvScanAddresses_RowPrePaint;
            dgvSendAddresses.RowPrePaint += DgvSendAddresses_RowPrePaint;
        }
        private void DgvAddresses_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var dataGridView = sender as DataGridView;
            if (dataGridView == null)
                return;

            var row = dataGridView.Rows[e.RowIndex];
            var addressConfig = row.DataBoundItem as PLCAddressConfig;
            if (addressConfig == null)
                return;

            // 处理数据类型列的显示
            if (dataGridView.Columns[e.ColumnIndex].Name == "colScanDataType" ||
                dataGridView.Columns[e.ColumnIndex].Name == "colSendDataType")
            {
                e.Value = PLCAddressConfig.GetDataTypeDisplayName(addressConfig.DataType);
                e.FormattingApplied = true;
            }

            // 处理启用状态列的显示
            if (dataGridView.Columns[e.ColumnIndex].Name == "colScanEnabled" ||
                dataGridView.Columns[e.ColumnIndex].Name == "colSendEnabled")
            {
                e.Value = addressConfig.IsEnabled ? "启用" : "禁用";
                e.FormattingApplied = true;
            }
        }

        private void DgvScanAddresses_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var row = dgvScanAddresses.Rows[e.RowIndex];
            var address = row.DataBoundItem as PLCScanAddress;
            if (address == null)
                return;

            // 设置行颜色
            if (!address.IsEnabled)
            {
                row.DefaultCellStyle.BackColor = Color.LightGray;
            }
            else if (address.ReadInterval < 500)
            {
                row.DefaultCellStyle.BackColor = Color.LightCyan;
            }
            else
            {
                row.DefaultCellStyle.BackColor = Color.White;
            }
        }

        private void DgvSendAddresses_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var row = dgvSendAddresses.Rows[e.RowIndex];
            var address = row.DataBoundItem as PLCSendAddress;
            if (address == null)
                return;

            // 设置行颜色
            if (!address.IsEnabled)
            {
                row.DefaultCellStyle.BackColor = Color.LightGray;
            }
            else if (address.AutoSend)
            {
                row.DefaultCellStyle.BackColor = Color.LightYellow;
            }
            else
            {
                row.DefaultCellStyle.BackColor = Color.White;
            }
        }
        private void SetupDataBindings()
        {
            // 清除现有绑定
            txtName.DataBindings.Clear();
            txtIPAddress.DataBindings.Clear();
            numPort.DataBindings.Clear();
            numSlaveID.DataBindings.Clear();
            numTimeout.DataBindings.Clear();
            cmbByteOrder.DataBindings.Clear();
            cmbStringByteOrder.DataBindings.Clear();
            chkHeartbeatEnabled.DataBindings.Clear();
            numHeartbeatInterval.DataBindings.Clear();
            txtHeartbeatAddress.DataBindings.Clear();
            chkEnabled.DataBindings.Clear();

            // 设置新的数据绑定
            txtName.DataBindings.Add("Text", _bindingSource, "Name", false, DataSourceUpdateMode.OnPropertyChanged);
            txtIPAddress.DataBindings.Add("Text", _bindingSource, "IPAddress", false, DataSourceUpdateMode.OnPropertyChanged);
            numPort.DataBindings.Add("Value", _bindingSource, "Port", false, DataSourceUpdateMode.OnPropertyChanged);
            numSlaveID.DataBindings.Add("Value", _bindingSource, "SlaveID", false, DataSourceUpdateMode.OnPropertyChanged);
            numTimeout.DataBindings.Add("Value", _bindingSource, "Timeout", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbByteOrder.DataBindings.Add("SelectedItem", _bindingSource, "ByteOrder", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbStringByteOrder.DataBindings.Add("SelectedItem", _bindingSource, "StringByteOrder", false, DataSourceUpdateMode.OnPropertyChanged);
            chkHeartbeatEnabled.DataBindings.Add("Checked", _bindingSource, "HeartbeatEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
            numHeartbeatInterval.DataBindings.Add("Value", _bindingSource, "HeartbeatInterval", false, DataSourceUpdateMode.OnPropertyChanged);
            txtHeartbeatAddress.DataBindings.Add("Text", _bindingSource, "HeartbeatAddress", false, DataSourceUpdateMode.OnPropertyChanged);
            chkEnabled.DataBindings.Add("Checked", _bindingSource, "IsEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
        }
        // 统一的连接管理方法
        public bool ConnectPLC(PLCConnectionConfig config)
        {
            lock (_connectionsLock)
            {
                if (_activeConnections.ContainsKey(config.Key))
                {
                    var existingConfig = _activeConnections[config.Key];

                    // 检查连接状态
                    if (existingConfig.Status == PLCConnectionConfig.ConnectionStatus.Connected)
                    {
                        return true;
                    }
                    else if (existingConfig.Status == PLCConnectionConfig.ConnectionStatus.Connecting)
                    {
                        LoggingService.Warn($"PLC连接正在进行中: {config.Name}");
                        return false;
                    }
                    else
                    {
                        // 移除无效连接
                        _activeConnections.Remove(config.Key);
                    }
                }

                if (config.Connect())
                {
                    _activeConnections[config.Key] = config;
                    UpdatePLCStatus(config, true);
                    return true;
                }

                UpdatePLCStatus(config, false);
                return false;
            }
        }
        public void DisconnectAll()
        {
            lock (_connectionsLock)
            {
                var keys = _activeConnections.Keys.ToList();
                foreach (var key in keys)
                {
                    try
                    {
                        if (_activeConnections.TryGetValue(key, out var config))
                        {
                            config.Disconnect();
                        }
                    }
                    catch (Exception ex)
                    {
                        LoggingService.Error($"断开PLC连接失败: {key}", ex);
                    }
                }
                _activeConnections.Clear();
            }
        }
        private void DgvPLCConfigs_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // 忽略布尔类型转换错误
            if (e.Exception is FormatException && e.Exception.Message.Contains("布尔值"))
            {
                e.ThrowException = false; // 阻止显示默认错误对话框
                return;
            }

            // 记录其他错误
            LoggingService.Error($"DataGridView数据错误: {e.Exception.Message}");
        }
        private void LoadSavedConfigs()
        {
            //var configs = PLCConfigManager.LoadConfigs();
            _plcConfigs.Clear();

            foreach (var config in GlobalVariables.PLCConnections)
            {
                // 为每个配置创建PLC实例
                config.CreatePLCInstance();
                _plcConfigs.Add(config);
            }

            if (_plcConfigs.Count > 0)
            {
                dgvPLCConfigs.ClearSelection();
                dgvPLCConfigs.Rows[0].Selected = true;
            }

            // 重要：加载配置后重建触发依赖关系
            RebuildTriggerDependencies();

            LoggingService.Info($"已加载 {GlobalVariables.PLCConnections.Count} 个PLC配置，并重建触发依赖关系");
        }
        private void RebuildTriggerDependencies()
        {
            try
            {
                // 获取PLCAddressManager实例并重建依赖
                var addressManager = PLCAddressManager.Instance;
                addressManager.RebuildAllTriggerDependencies();

                // 可选：显示依赖关系信息
                var dependencies = addressManager.GetAllDependencies();
                if (dependencies.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("触发依赖关系:");
                    foreach (var kvp in dependencies)
                    {
                        sb.AppendLine($"  {kvp.Key} -> {string.Join(", ", kvp.Value)}");
                    }
                    LoggingService.Info(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                LoggingService.Error("重建触发依赖关系失败", ex);
            }
        }
        private void LoadDefaultValues()
        {
            txtName.Text = "New_PLC";
            cmbPLCType.SelectedIndex = 0;
            txtIPAddress.Text = "192.168.1.100";
            numPort.Value = 502;
            numSlaveID.Value = 1;
            numTimeout.Value = 1000;
            cmbByteOrder.SelectedIndex = 0;
            cmbStringByteOrder.SelectedIndex = 0;
            numHeartbeatInterval.Value = 30000;
            txtHeartbeatAddress.Text = "100";
            chkEnabled.Checked = true;
            chkHeartbeatEnabled.Checked = true;
        }
        private void LoadConfigToForm(PLCConnectionConfig config)
        {
            txtName.Text = config.Name;
            cmbPLCType.SelectedItem = config.PLCType;
            txtIPAddress.Text = config.IPAddress;
            numPort.Value = config.Port;
            numSlaveID.Value = config.SlaveID;
            numTimeout.Value = config.Timeout;
            cmbByteOrder.SelectedItem = config.ByteOrder;
            cmbStringByteOrder.SelectedItem = config.StringByteOrder;
            chkHeartbeatEnabled.Checked = config.HeartbeatEnabled;
            numHeartbeatInterval.Value = config.HeartbeatInterval;
            txtHeartbeatAddress.Text = config.HeartbeatAddress;
            chkEnabled.Checked = config.IsEnabled;

            // LoadScanAddresses(config.ScanAddresses.Values.ToList( ));
            // LoadSendAddresses(config.SendAddresses.Values.ToList());
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
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
                _bindingSource.Position = _plcConfigs.Count - 1;
                _currentConfig = config;

                // 加载到表单显示
                LoadConfigToForm(_currentConfig);

                LoggingService.Info($"添加新的PLC配置: {config.Name}");
            }
            catch (Exception ex)
            {
                LoggingService.Error("添加PLC配置失败", ex);
                MessageBox.Show($"添加配置失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string GetUniquePLCName(string baseName)
        {
            if (_plcConfigs == null || _plcConfigs.Count == 0)
                return baseName;

            string name = baseName;
            int counter = 1;

            // 检查名称是否已存在，如果存在则添加序号
            while (_plcConfigs.Any(config => config.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                name = $"{baseName}_{counter++}";

                // 防止无限循环，设置最大尝试次数
                if (counter > 1000)
                {
                    // 如果尝试1000次后仍有冲突，使用GUID确保唯一性
                    name = $"{baseName}_{Guid.NewGuid():N.Substring(0, 8)}";
                    break;
                }
            }

            return name;
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvPLCConfigs.SelectedRows.Count > 0)
            {
                var selected = dgvPLCConfigs.SelectedRows[0].DataBoundItem as PLCConnectionConfig;
                if (selected != null)
                {
                    _plcConfigs.Remove(selected);
                    ClearForm();
                }
            }
        }
        private bool ValidateNumericValues()
        {
            try
            {
                // 验证端口号
                if (numPort.Value < numPort.Minimum || numPort.Value > numPort.Maximum)
                {
                    MessageBox.Show($"端口号必须在{numPort.Minimum}-{numPort.Maximum}之间", "验证错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    numPort.Focus();
                    return false;
                }

                // 验证站号
                if (numSlaveID.Value < numSlaveID.Minimum || numSlaveID.Value > numSlaveID.Maximum)
                {
                    MessageBox.Show($"站号必须在{numSlaveID.Minimum}-{numSlaveID.Maximum}之间", "验证错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    numSlaveID.Focus();
                    return false;
                }

                // 验证超时时间
                if (numTimeout.Value < numTimeout.Minimum || numTimeout.Value > numTimeout.Maximum)
                {
                    MessageBox.Show($"超时时间必须在{numTimeout.Minimum}-{numTimeout.Maximum}之间", "验证错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    numTimeout.Focus();
                    return false;
                }

                // 验证心跳间隔
                if (numHeartbeatInterval.Value < numHeartbeatInterval.Minimum ||
                    numHeartbeatInterval.Value > numHeartbeatInterval.Maximum)
                {
                    MessageBox.Show($"心跳间隔必须在{numHeartbeatInterval.Minimum}-{numHeartbeatInterval.Maximum}之间", "验证错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    numHeartbeatInterval.Focus();
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                LoggingService.Error("数值验证失败", ex);
                return false;
            }
        }
        private void btnSaveConfig_Click(object sender, EventArgs e)
        {
            if (!ValidateNumericValues())
            {
                return;
            }
            // 数据绑定自动保存，这里只需要保存到文件
            PLCConfigManager.SaveConfigs(_plcConfigs.ToList());
            MessageBox.Show("配置已保存");
        }
        private void btnConfigScanAddresses_Click(object sender, EventArgs e)
        {
            if (_currentConfig == null)
                return;

            using (var form = new FormPLCAddressConfig(_currentConfig, AddressDirection.ReadOnly))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    RefreshAddressLists(); // 刷新显示
                }
            }
        }
        private void btnConfigSendAddresses_Click(object sender, EventArgs e)
        {
            if (_currentConfig == null)
                return;

            using (var form = new FormPLCAddressConfig(_currentConfig, AddressDirection.WriteOnly))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    RefreshAddressLists(); // 刷新显示
                }
            }
        }
        private void btnStartAutoRead_Click(object sender, EventArgs e)
        {
            if (_currentConfig == null)
            {
                MessageBox.Show("请先选择PLC配置", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 确保连接
                if (!ConnectPLC(_currentConfig))
                {
                    MessageBox.Show("PLC连接失败，无法启动自动读取", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _currentConfig.StartAutoRead();
                UpdateAutoReadStatus(true);
                LoggingService.Info($"启动自动读取 - {_currentConfig.Name}");
            }
            catch (Exception ex)
            {
                LoggingService.Error("启动自动读取失败", ex);
                MessageBox.Show($"启动自动读取失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnStopAutoRead_Click(object sender, EventArgs e)
        {
            if (_currentConfig == null)
                return;

            try
            {
                _currentConfig.StopAutoRead();
                UpdateAutoReadStatus(false);
                LoggingService.Info($"停止自动读取 - {_currentConfig.Name}");

                // 注意：这里不断开连接，因为可能还有其他操作需要使用
            }
            catch (Exception ex)
            {
                LoggingService.Error("停止自动读取失败", ex);
                MessageBox.Show($"停止自动读取失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // 定期检查连接状态
        private void StartConnectionMonitor()
        {
            _connectionMonitorTimer = new System.Timers.Timer(30000); // 30秒检查一次
            _connectionMonitorTimer.Elapsed += (s, e) => CheckConnectionsHealth();
            _connectionMonitorTimer.Start();
        }
        private void CheckConnectionsHealth()
        {
            lock (_connectionsLock)
            {
                var brokenConnections = new List<string>();
                var needsReconnect = new List<PLCConnectionConfig>();

                foreach (var kvp in _activeConnections)
                {
                    var config = kvp.Value;

                    try
                    {
                        // 检查连接状态
                        if (config.Status == PLCConnectionConfig.ConnectionStatus.Faulted)
                        {
                            LoggingService.Warn($"检测到故障的PLC连接: {config.Name}");
                            needsReconnect.Add(config);
                        }
                        else if (config.Status == PLCConnectionConfig.ConnectionStatus.Disconnected)
                        {
                            brokenConnections.Add(kvp.Key);
                            LoggingService.Warn($"检测到断开的PLC连接: {config.Name}");
                        }
                        else if (config.Status == PLCConnectionConfig.ConnectionStatus.Connected)
                        {
                            // 定期测试连接有效性
                            int testValue = 0;
                            if (!config.PLCInstance.ReadRegister("0", ref testValue))
                            {
                                LoggingService.Warn($"PLC连接测试失败: {config.Name}");
                                brokenConnections.Add(kvp.Key);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LoggingService.Error($"检查PLC连接状态异常: {config.Name}", ex);
                        brokenConnections.Add(kvp.Key);
                    }
                }

                // 移除断开的连接
                foreach (var key in brokenConnections)
                {
                    _activeConnections.Remove(key);
                }

                // 尝试自动重连
                foreach (var config in needsReconnect)
                {
                    try
                    {
                        LoggingService.Info($"尝试自动重连PLC: {config.Name}");
                        if (config.Connect())
                        {
                            _activeConnections[config.Key] = config;
                            UpdatePLCStatus(config, true);
                            LoggingService.Info($"PLC自动重连成功: {config.Name}");
                        }
                    }
                    catch (Exception ex)
                    {
                        LoggingService.Error($"自动重连PLC失败: {config.Name}", ex);
                    }
                }

                // 更新UI显示
                if (brokenConnections.Count > 0 || needsReconnect.Count > 0)
                {
                    this.Invoke(new Action(() =>
                    {
                        foreach (DataGridViewRow row in dgvPLCConfigs.Rows)
                        {
                            var config = row.DataBoundItem as PLCConnectionConfig;
                            if (config != null)
                            {
                                bool isConnected = _activeConnections.ContainsKey(config.Key) &&
                                            config.Status == PLCConnectionConfig.ConnectionStatus.Connected;
                                UpdatePLCStatus(config, isConnected);
                            }
                        }
                    }));
                }
            }
        }
        private void UpdateAutoReadStatus(bool isRunning)
        {
            btnStartAutoRead.Enabled = !isRunning;
            btnStopAutoRead.Enabled = isRunning;

            btnStartAutoRead.BackColor = isRunning ? Color.LightGray : Color.LightGreen;
            btnStopAutoRead.BackColor = isRunning ? Color.LightCoral : Color.LightGray;
        }
        private void dgvPLCConfigs_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPLCConfigs.SelectedRows.Count > 0)
            {
                _currentConfig = dgvPLCConfigs.SelectedRows[0].DataBoundItem as PLCConnectionConfig;
                _bindingSource.Position = _plcConfigs.IndexOf(_currentConfig);

                // 更新地址列表的显示
                RefreshAddressLists();
            }
        }
        private void RefreshAddressLists()
        {
            if (_currentConfig == null)
                return;

            // 创建用于显示的包装类列表
            var scanDisplayList = _currentConfig.ScanAddresses.Select(addr => new ScanAddressDisplayWrapper(addr, _currentConfig)).ToList();
            var sendDisplayList = _currentConfig.SendAddresses.Select(addr => new SendAddressDisplayWrapper(addr, _currentConfig)).ToList();

            // 绑定到数据源
            _scanAddressesBindingSource.DataSource = new BindingList<ScanAddressDisplayWrapper>(scanDisplayList);
            _sendAddressesBindingSource.DataSource = new BindingList<SendAddressDisplayWrapper>(sendDisplayList);
            UpdateAddressesSummary();
        }
        // 地址显示包装类
        // 扫描地址显示包装类（只包含扫描地址的属性）
        private class ScanAddressDisplayWrapper
        {
            private readonly PLCScanAddress _address;
            private readonly PLCConnectionConfig _config;

            public ScanAddressDisplayWrapper(PLCScanAddress address, PLCConnectionConfig config)
            {
                _address = address;
                _config = config;
            }
            // 扫描地址属性
            [DisplayName("键值")]
            public string Name => _address.Key;
            [DisplayName("地址")]
            public string Address => _address.Address;
            [DisplayName("数据类型")]
            public string DataTypeDisplay => PLCAddressConfig.GetDataTypeDisplayName(_address.DataType);
            [DisplayName("描述")]
            public string Description => _address.Description;
            [DisplayName("扫描间隔")]
            public int ReadInterval => _address.ReadInterval;
            [DisplayName("启用状态")]
            public bool IsEnabled => _address.IsEnabled;
            [DisplayName("IP地址")]
            public string IPAddress => _config.IPAddress;
            [DisplayName("端口号")]
            public int Port => _config.Port;
        }

        // 发送地址显示包装类（包含发送地址特有属性）
        private class SendAddressDisplayWrapper
        {
            private readonly PLCSendAddress _address;
            private readonly PLCConnectionConfig _config;

            public SendAddressDisplayWrapper(PLCSendAddress address, PLCConnectionConfig config)
            {
                _address = address;
                _config = config;
            }

            // 公共属性
            [DisplayName("键值")]
            public string Name => _address.Key;
            [DisplayName("地址")]
            public string Address => _address.Address;
            [DisplayName("数据类型")]
            public string DataTypeDisplay => PLCAddressConfig.GetDataTypeDisplayName(_address.DataType);
            [DisplayName("描述")]
            public string Description => _address.Description;
            [DisplayName("发送间隔")]
            public int ReadInterval => _address.ReadInterval;
            [DisplayName("启用状态")]
            public bool IsEnabled => _address.IsEnabled;
            [DisplayName("IP地址")]
            public string IPAddress => _config.IPAddress;
            [DisplayName("端口号")]
            public int Port => _config.Port;
        }
        private void UpdateAddressesSummary()
        {
            if (_currentConfig == null)
                return;

            var scanEnabledCount = _currentConfig.ScanAddresses.Count(a => a.IsEnabled);
            var sendEnabledCount = _currentConfig.SendAddresses.Count(a => a.IsEnabled);

            grpScanAddresses.Text = $"扫描地址配置 (总数: {_currentConfig.ScanAddresses.Count}, 启用: {scanEnabledCount})";
            grpSendAddresses.Text = $"发送地址配置 (总数: {_currentConfig.SendAddresses.Count}, 启用: {sendEnabledCount})";
        }
        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            if (!ValidateNumericValues())
            {
                return;
            }
            if (_currentConfig == null)
                return;

            // 直接使用当前绑定对象的参数
            bool success = _currentConfig.Connect();

            if (success)
                MessageBox.Show("连接成功");
            else
                MessageBox.Show("连接失败");
        }
        private void btnLoadConfigs_Click(object sender, EventArgs e)
        {
            try
            {
                // 原有的加载逻辑
                //var configs = PLCConfigManager.LoadConfigs();
                _plcConfigs.Clear();
                foreach (var config in GlobalVariables.PLCConnections)
                {
                    _plcConfigs.Add(config);
                }

                // 重建触发依赖关系
                RebuildTriggerDependencies();

                MessageBox.Show($"已加载 {GlobalVariables.PLCConnections.Count} 个PLC配置，并重建触发依赖关系", "成功",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                LoggingService.Error("加载配置失败", ex);
                MessageBox.Show($"加载配置失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void btnSaveAll_Click(object sender, EventArgs e)
        {
            try
            {
                // 先保存当前编辑的配置
                //if (_currentConfig!=null&&ValidateForm( )) {
                //    SaveFormToConfig(_currentConfig);
                //    }

                // 保存所有配置到文件
                PLCConfigManager.SaveConfigs(_plcConfigs.ToList());

                MessageBox.Show($"成功保存 {_plcConfigs.Count} 个PLC配置", "成功",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存配置失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void DgvPLCConfigs_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvPLCConfigs.Columns[e.ColumnIndex].Name == "colStatus" && e.Value is bool)
            {
                e.Value = (bool)e.Value ? "已连接" : "未连接";
                e.FormattingApplied = true;
            }

            if (dgvPLCConfigs.Columns[e.ColumnIndex].Name == "colEnabled" && e.Value is bool)
            {
                e.Value = (bool)e.Value ? "启用" : "禁用";
                e.FormattingApplied = true;
            }
        }
        private void OnPLCLog(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(OnPLCLog), message);
                return;
            }

            // 可以在这里添加日志显示逻辑
            System.Diagnostics.Debug.WriteLine($"[PLC Log] {message}");
        }
        private void OnPLCHeartbeatStatusChanged(object sender, HeartbeatEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler<HeartbeatEventArgs>(OnPLCHeartbeatStatusChanged), sender, e);
                return;
            }

            // 更新心跳状态显示
            if (_currentConfig != null && sender == _currentConfig.PLCInstance)
            {
                UpdateHeartbeatStatus(e.IsAlive, e.Message, e.RetryCount);
            }
        }
        private void UpdatePLCStatus(PLCConnectionConfig config, bool isConnected)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<PLCConnectionConfig, bool>(UpdatePLCStatus), config, isConnected);
                return;
            }

            // 更新DataGridView中的状态显示
            foreach (DataGridViewRow row in dgvPLCConfigs.Rows)
            {
                var rowConfig = row.DataBoundItem as PLCConnectionConfig;
                if (rowConfig == config)
                {
                    row.Cells["colStatus"].Value = isConnected ? "已连接" : "未连接";
                    row.Cells["colStatus"].Style.ForeColor = isConnected ? Color.Green : Color.Red;
                    break;
                }
            }
        }
        private void UpdateHeartbeatStatus(bool isAlive, string message, int retryCount)
        {
            // 可以添加状态栏或标签显示心跳状态
            string status = isAlive ? $"心跳正常 (重试: {retryCount})" : $"心跳异常: {message}";
            System.Diagnostics.Debug.WriteLine($"[Heartbeat] {status}");
        }
        private void FormPLCConfig_FormClosing(object sender, FormClosingEventArgs e)
        {
            _connectionMonitorTimer?.Stop();
            _connectionMonitorTimer?.Dispose();
            DisconnectAll();
        }
        private void ClearForm()
        {
            //txtName.Text="";
            cmbPLCType.SelectedIndex = 0;
            txtIPAddress.Text = "192.168.1.100";
            numPort.Value = 502;
            numSlaveID.Value = 1;
            numTimeout.Value = 1000;
            cmbByteOrder.SelectedIndex = 0;
            cmbStringByteOrder.SelectedIndex = 0;
            chkHeartbeatEnabled.Checked = true;
            numHeartbeatInterval.Value = 30000;
            txtHeartbeatAddress.Text = "100";
            chkEnabled.Checked = true;
        }
    }
    public enum PLCType
    {
        DeltaModbusTCP,
        MitsubishiMCProtocol,
        SiemensS7,
        OmronFINS
    }
}