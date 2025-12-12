using CosmxMESClient.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CosmxMESClient {
    public partial class FormPLCAddressConfig:Form {
        private PLCConnectionConfig _currentConfig;
        private AddressDirection _currentDirection;
        private PLCAddressConfig _currentAddress;
        private bool _isEditing = false;
        private BindingSource _addressBindingSource = new BindingSource();
        public FormPLCAddressConfig( PLCConnectionConfig config,AddressDirection direction ) {
            InitializeComponent( );
            _currentConfig=config;
            _currentDirection=direction;
            _currentAddress=CreateNewAddress( );

            // 初始化界面
            //  InitializeDataTypeComboBox( );
            InitializeInterface( );
            LoadAddresses( );
            LoadFormData( );
            UpdateFormState( );
            }
        private void InitializeInterface( ) {
            switch (_currentDirection) {
                case AddressDirection.ReadOnly:
                    Text=$"扫描地址配置 - {_currentConfig?.Name}";
                    grpTriggerConditions.Visible=true; // 显示触发条件配置
                    grpTriggerConditions.Enabled=true;
                    break;
                case AddressDirection.WriteOnly:
                    Text=$"发送地址配置 - {_currentConfig?.Name}";
                    grpTriggerConditions.Visible=false; // 隐藏触发条件配置
                    grpTriggerConditions.Enabled=false;
                    // 调整布局，让基本配置区域占用更多空间
                    grpAddressConfig.Height+=grpTriggerConditions.Height;

                    break;
                default:
                    Text=$"地址配置 - {_currentConfig?.Name}";
                    break;
                }

            // 根据方向显示/隐藏相关控件
            if (_currentDirection==AddressDirection.ReadOnly) {
                lblReadInterval.Text="扫描间隔(ms):";
                btnTestAddress.Text="测试读取";
                // 如果是扫描地址，隐藏发送相关控件
                chkAutoSend.Visible=false;
                numSendDelay.Visible=false;
                lblSendDelay.Visible=false;
                }
            else {
                lblReadInterval.Text="发送间隔(ms):";
                btnTestAddress.Text="测试写入";
                chkAutoSend.Visible=true;
                lblSendDelay.Visible=true;
                numSendDelay.Visible=true;
                }

            // 初始化数据类型下拉框
              InitializeDataTypeComboBox( );
            }
        private void InitializeDataTypeComboBox( ) {
            cmbDataType.Items.Clear( );
            var dataTypes = new[]
        {
            new ComboBoxItem { Text = "布尔(Bool)", Value = typeof(bool) },
            new ComboBoxItem { Text = "整数(Int32)", Value = typeof(int) },
            new ComboBoxItem { Text = "浮点数(Float)", Value = typeof(float) },
            new ComboBoxItem { Text = "双精度(Double)", Value = typeof(double) },
            new ComboBoxItem { Text = "字符串(String)", Value = typeof(string) }
        };

            cmbDataType.Items.AddRange(dataTypes);
            cmbDataType.DisplayMember="Text";
            cmbDataType.ValueMember="Value";

            if (cmbDataType.Items.Count>0)
                cmbDataType.SelectedIndex=0;
            }
        private void FormPLCAddressConfig_Load( object sender,EventArgs e ) {
            LoadFormData( );
            }

        private void LoadAddresses( ) {
            lvAddresses.Items.Clear( );

            // 获取实际的数据集合
            IEnumerable<PLCAddressConfig> addresses;
            if (_currentDirection==AddressDirection.ReadOnly) {
                addresses=_currentConfig.ScanAddresses.Cast<PLCAddressConfig>( );

                //// 添加触发状态列
                //if (!lvAddresses.Columns.ContainsKey("触发状态")) {
                //    lvAddresses.Columns.Add("触发状态",80);
                //    }
                }
            else {
                addresses=_currentConfig.SendAddresses.Cast<PLCAddressConfig>( );
                }

            foreach (var address in addresses.OrderBy(a => a.Name)) {
                var item = new ListViewItem(address.Key);
                item.SubItems.Add(address.Name);
                item.SubItems.Add(address.Address);
                item.SubItems.Add(GetDataTypeDisplayName(address.DataType));
                item.SubItems.Add(address.Description);
                item.SubItems.Add(address.ReadInterval.ToString( ));
                item.SubItems.Add(address.IsEnabled ? "启用" : "禁用");

                // 根据地址类型显示特定列
                if (address is PLCScanAddress scanAddress) {
                    // 扫描地址：显示触发条件相关信息
                    string triggerCondition = GetTriggerConditionDisplay(scanAddress.TriggerCondition);
                    item.SubItems.Add(triggerCondition);

                    string thresholdText = scanAddress.TriggerCondition != TriggerCondition.None
                ? scanAddress.TriggerThreshold.ToString("F2")
                : "N/A";
                    item.SubItems.Add(thresholdText);

                    string triggerStatus = scanAddress.TriggerCondition != TriggerCondition.None
                ? (scanAddress.IsTriggered ? "已触发" : "待触发")
                : "无条件";
                    item.SubItems.Add(triggerStatus);

                    // 设置行颜色
                    if (scanAddress.IsTriggered)
                        item.BackColor=Color.LightGreen;
                    else if (scanAddress.TriggerCondition!=TriggerCondition.None)
                        item.BackColor=Color.LightYellow;
                    }
                else {
                    // 发送地址：显示发送相关信息
                    if (address is PLCSendAddress sendAddress) {
                        item.SubItems.Add("N/A"); // 触发条件
                        item.SubItems.Add("N/A"); // 触发阈值
                        item.SubItems.Add(sendAddress.AutoSend ? "自动发送" : "手动发送"); // 发送状态
                        }
                    else {
                        item.SubItems.Add("N/A");
                        item.SubItems.Add("N/A");
                        item.SubItems.Add("N/A");
                        }
                    }
                item.Tag=address;
                lvAddresses.Items.Add(item);
                }
            }
        private string GetTriggerConditionDisplay( TriggerCondition condition ) {
            switch (condition) {
                case TriggerCondition.None:
                    return "无";
                case TriggerCondition.ThresholdAbove:
                    return "阈值以上";
                case TriggerCondition.ThresholdBelow:
                    return "阈值以下";
                case TriggerCondition.ChangePercentage:
                    return "变化百分比";
                case TriggerCondition.RisingEdge:
                    return "上升沿";
                case TriggerCondition.FallingEdge:
                    return "下降沿";
                default:
                    return "未知";
                }
            }
        private string GetDataTypeDisplayName( Type dataType ) {
            if (dataType==null)
                return "未知";

            if (dataType==typeof(bool))
                return "布尔";
            if (dataType==typeof(int))
                return "整数";
            if (dataType==typeof(float))
                return "浮点数";
            if (dataType==typeof(double))
                return "双精度";
            if (dataType==typeof(string))
                return "字符串";

            return dataType.Name;
            }
        private void LoadFormData( ) {
            if (_currentAddress==null) {
                _currentAddress=CreateNewAddress( );
                }

            // 手动加载数据到控件（不使用数据绑定）
            txtName.Text=_currentAddress.Name??"";
            txtAddress.Text=_currentAddress.Address??"";
            txtDescription.Text=_currentAddress.Description??"";
            numReadInterval.Value=_currentAddress.ReadInterval>0 ? _currentAddress.ReadInterval : 1000;
            numPower.Value=_currentAddress.Power>0 ? _currentAddress.Power : 1;
            numLength.Value=_currentAddress.Length>0 ? _currentAddress.Length : 1;
            chkEnabled.Checked=_currentAddress.IsEnabled;

            // 设置数据类型
            if (_currentAddress.DataType!=null) {
                foreach (ComboBoxItem item in cmbDataType.Items) {
                    if (item.Value==_currentAddress.DataType) {
                        cmbDataType.SelectedItem=item;
                        break;
                        }
                    }
                }
            else {
                // 设置默认数据类型
                cmbDataType.SelectedIndex=0;
                }

            // 如果是发送地址，设置特有属性
            if (_currentAddress is PLCSendAddress sendAddress) {
                chkAutoSend.Checked=sendAddress.AutoSend;
                numSendDelay.Value=sendAddress.SendDelay;
                }
            else if (_currentAddress is PLCScanAddress scanAddress) {


                // 扫描地址：加载触发条件
                cmbTriggerCondition.SelectedIndex=(int) scanAddress.TriggerCondition;
                numTriggerThreshold.Value=(decimal) scanAddress.TriggerThreshold;
                numTriggerDelay.Value=scanAddress.TriggerDelay;
                chkTriggerRisingEdge.Checked=scanAddress.TriggerOnRisingEdge;
                chkTriggerFallingEdge.Checked=scanAddress.TriggerOnFallingEdge;

                }
            }
        private void SaveFormData( ) {
            if (_currentAddress==null)
                return;

            // 手动从控件保存数据到对象
            _currentAddress.Name=txtName.Text.Trim( );
            _currentAddress.Address=txtAddress.Text.Trim( );
            _currentAddress.Description=txtDescription.Text.Trim( );
            _currentAddress.ReadInterval=(int) numReadInterval.Value;
            _currentAddress.Power=(int) numPower.Value;
            _currentAddress.Length=(int) numLength.Value;
            _currentAddress.IsEnabled=chkEnabled.Checked;

            // 保存数据类型
            if (cmbDataType.SelectedItem is ComboBoxItem selectedItem) {
                _currentAddress.DataType=selectedItem.Value;
                }
            else {
                _currentAddress.DataType=typeof(int); // 默认类型
                }

            // 如果是发送地址，保存特有属性
            if (_currentAddress is PLCSendAddress sendAddress) {
                sendAddress.AutoSend=chkAutoSend.Checked;
                sendAddress.SendDelay=(int) numSendDelay.Value;
                }
            if (_currentAddress is PLCScanAddress scanAddress) {
                scanAddress.TriggerCondition=(TriggerCondition) cmbTriggerCondition.SelectedIndex;
                scanAddress.TriggerThreshold=(double) numTriggerThreshold.Value;
                scanAddress.TriggerDelay=(int) numTriggerDelay.Value;
                scanAddress.TriggerOnRisingEdge=chkTriggerRisingEdge.Checked;
                scanAddress.TriggerOnFallingEdge=chkTriggerFallingEdge.Checked;
                }
            }
        private PLCAddressConfig CreateNewAddress( ) {
            PLCAddressConfig newAddress = _currentDirection == AddressDirection.ReadOnly
        ? (PLCAddressConfig)new PLCScanAddress( )
        : (PLCAddressConfig)new PLCSendAddress( );

            if (newAddress is PLCScanAddress) {
                // 创建扫描地址
                newAddress=new PLCScanAddress
                    {
                    Name=$"扫描地址_{DateTime.Now:HHmmss}",
                    Address="D100", // 默认扫描地址
                    DataType=typeof(int),
                    Description="",
                    ReadInterval=1000,
                    Power=1,
                    Length=1,
                    IsEnabled=true,
                    TriggerCondition=TriggerCondition.None, // 默认无触发条件
                    TriggerThreshold=0,
                    TriggerDelay=0,
                    TriggerOnRisingEdge=false,
                    TriggerOnFallingEdge=false
                    };
                }
            else {
                // 创建发送地址
                newAddress=new PLCSendAddress
                    {
                    Name=$"发送地址_{DateTime.Now:HHmmss}",
                    Address="D200", // 默认发送地址
                    DataType=typeof(int),
                    Description="",
                    ReadInterval=1000,
                    Power=1,
                    Length=1,
                    IsEnabled=true,
                    AutoSend=false, // 默认不自动发送
                    SendDelay=0
                    };
                }
                return newAddress;
            }
        private void btnAdd_Click( object sender,EventArgs e ) {
            if (!ValidateForm( ))
                return;

            try {
                // 保存当前表单数据到_currentAddress
                SaveFormData( );

                // 添加到实际的数据集合中
                if (_currentDirection==AddressDirection.ReadOnly) {
                    var scanAddress = _currentAddress as PLCScanAddress;
                    if (scanAddress!=null) {
                        _currentConfig.AddScanAddress(scanAddress);
                        }
                    }
                else {
                    var sendAddress = _currentAddress as PLCSendAddress;
                    if (sendAddress!=null) {
                        _currentConfig.AddSendAddress(sendAddress);
                        }
                    }

                LoggingService.Info($"添加{GetDirectionDisplayName( )}地址: {_currentAddress.Name}");

                // 刷新表格显示
                LoadAddresses( );

                // 重置表单
                ResetForm( );

                MessageBox.Show("地址添加成功","成功",MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
            catch (Exception ex) {
                LoggingService.Error($"添加地址失败",ex);
                MessageBox.Show($"添加地址失败: {ex.Message}","错误",
                    MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }

        // 触发条件选择变化事件
        private void cmbTriggerCondition_SelectedIndexChanged( object sender,EventArgs e ) {
            UpdateTriggerControlsVisibility( );
            }

        private void UpdateTriggerControlsVisibility( ) {
            var condition = (TriggerCondition)cmbTriggerCondition.SelectedIndex;

            // 根据选择的触发条件显示/隐藏相关控件
            lblTriggerThreshold.Visible=condition==TriggerCondition.ThresholdAbove||
                                        condition==TriggerCondition.ThresholdBelow||
                                        condition==TriggerCondition.ChangePercentage;
            numTriggerThreshold.Visible=lblTriggerThreshold.Visible;

            chkTriggerRisingEdge.Visible=condition==TriggerCondition.RisingEdge;
            chkTriggerFallingEdge.Visible=condition==TriggerCondition.FallingEdge;

            lblTriggerDelay.Visible=condition!=TriggerCondition.None;
            numTriggerDelay.Visible=condition!=TriggerCondition.None;
            }

        private void btnEdit_Click( object sender,EventArgs e ) {
            if (lvAddresses.SelectedItems.Count==0) {
                MessageBox.Show("请选择要编辑的地址","提示",
                    MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
                }

            if (!ValidateForm( ))
                return;

            try {
                // 保存当前编辑的数据
                SaveFormData( );

                // 由于_currentAddress已经是实际对象的引用，数据已经自动更新
                // 只需要刷新显示即可

                LoggingService.Info($"修改{GetDirectionDisplayName( )}地址: {_currentAddress.Name}");

                // 刷新表格显示
                LoadAddresses( );

                // 重置表单
                ResetForm( );

                MessageBox.Show("地址修改成功","成功",MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
            catch (Exception ex) {
                LoggingService.Error($"编辑地址失败",ex);
                MessageBox.Show($"编辑地址失败: {ex.Message}","错误",
                    MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
        private void btnDelete_Click( object sender,EventArgs e ) {
            if (lvAddresses.SelectedItems.Count==0)
                return;

            var address = lvAddresses.SelectedItems[0].Tag as PLCAddressConfig;
            if (address==null)
                return;

            var result = MessageBox.Show($"确定要删除地址 '{address.Name}' 吗？", "确认删除",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result==DialogResult.Yes) {
                try {
                    bool success = false;
                    if (_currentDirection==AddressDirection.ReadOnly) {
                        success=_currentConfig.RemoveScanAddress(address.Key);
                        }
                    else {
                        success=_currentConfig.RemoveSendAddress(address.Key);
                        }

                    if (success) {
                        LoggingService.Info($"删除{GetDirectionDisplayName( )}地址: {address.Name}");
                        LoadAddresses( );
                        ResetForm( );
                        }
                    }
                catch (Exception ex) {
                    LoggingService.Error($"删除地址失败",ex);
                    MessageBox.Show($"删除地址失败: {ex.Message}","错误",
                        MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                }
            }
        private void lvAddresses_SelectedIndexChanged( object sender,EventArgs e ) {
            if (lvAddresses.SelectedItems.Count>0) {
                // 选中模式
                _currentAddress=lvAddresses.SelectedItems[0].Tag as PLCAddressConfig;
                _isEditing=true;
                LoadFormData( );
                }
            else {
                // 取消选中模式 - 恢复到添加模式
                _isEditing=false;
                _currentAddress=CreateNewAddress( ); // 创建新的空地址对象
                LoadFormData( );
                }
            UpdateFormState( );
            }

        private void btnTestAddress_Click( object sender,EventArgs e ) {
            if (!ValidateForm( ))
                return;

            try {
                SaveFormData( );
                TestAddressFunctionality( );
                }
            catch (Exception ex) {
                LoggingService.Error("地址测试失败",ex);
                MessageBox.Show($"测试失败: {ex.Message}","错误",
                    MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }

        private async void TestAddressFunctionality( ) {
            if (_currentDirection==AddressDirection.ReadOnly) {
                await TestTriggerConditions( );
                }
            else {
                TestWriteAddress( );
                }
            }

        private async Task TestTriggerConditions( ) {
            if (_currentAddress==null||!( _currentAddress is PLCScanAddress scanAddress ))
                return;

            try {
                // 测试扫描地址的触发条件
                for (int i = 0; i<5; i++) {
                    object testValue = GenerateTestValue(scanAddress.DataType, i);
                    bool triggered = scanAddress.CheckTriggerCondition(testValue, scanAddress.LastValue);

                    LoggingService.Info($"触发条件测试 [{i}]: 值={testValue}, 触发={triggered}");

                    if (triggered) {
                        MessageBox.Show($"触发条件满足！\n当前值: {testValue}\n触发条件: {scanAddress.TriggerCondition}",
                            "触发测试",MessageBoxButtons.OK,MessageBoxIcon.Information);
                        break;
                        }

                    await Task.Delay(500);
                    }
                }
            catch (Exception ex) {
                LoggingService.Error("触发条件测试失败",ex);
                }
            }
        private object GenerateTestValue( Type dataType,int iteration ) {
            switch (dataType.Name) {
                case nameof(Boolean):
                    return iteration%2==0;
                case nameof(Int32):
                    return iteration*10;
                case nameof(Single):
                    return iteration*1.5f;
                default:
                    return iteration;
                }
            }
        private async void TestWriteAddress( ) {
            if (_currentAddress==null||_currentConfig==null) {
                MessageBox.Show("请先配置地址和PLC连接","提示",
                    MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
                }

            try {
                // 测试发送地址的写入功能
                string testValue = GetTestValueForDataType();

                var result = MessageBox.Show($"测试写入地址：{_currentAddress.Address}\n\n" +
            $"写入值：{testValue}\n" +
            $"数据类型：{GetDataTypeDisplayName(_currentAddress.DataType)}\n" +
            $"PLC名称：{_currentConfig.Name}\n\n是否继续？",
            "测试写入", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                if (result==DialogResult.OK) {
                    // 执行PLC写入测试
                    var writeResult = await ExecutePLCWriteTest(_currentConfig, _currentAddress, testValue);

                    if (writeResult.Success) {
                        LoggingService.Info($"测试写入地址成功：{_currentAddress.Address} 值：{testValue}");
                        MessageBox.Show($"写入测试完成！\n\n"+
                            $"地址：{_currentAddress.Address}\n"+
                            $"写入值：{testValue}\n"+
                            $"数据类型：{GetDataTypeDisplayName(_currentAddress.DataType)}\n"+
                            $"写入时间：{writeResult.ElapsedMilliseconds}ms",
                            "测试结果",MessageBoxButtons.OK,MessageBoxIcon.Information);
                        }
                    else {
                        throw new Exception(writeResult.ErrorMessage);
                        }
                    }
                }
            catch (Exception ex) {
                LoggingService.Error($"测试写入地址失败：{_currentAddress.Address}",ex);
                MessageBox.Show($"写入测试失败：{ex.Message}","错误",
                    MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
        #region 实际的PLC测试逻辑
        private async Task<PLCTestResult> ExecutePLCReadTest( PLCConnectionConfig config,PLCAddressConfig address ) {
            var stopwatch = Stopwatch.StartNew();

            try {
                // 使用统一的连接管理
                if (!await EnsurePLCConnected(config)) {
                    return new PLCTestResult
                        {
                        Success=false,
                        ErrorMessage="PLC连接失败，请检查网络和配置",
                        ElapsedMilliseconds=stopwatch.ElapsedMilliseconds
                        };
                    }

                // 根据数据类型执行读取操作
                object value = await ReadAddressByType(config, address);

                stopwatch.Stop( );

                return new PLCTestResult
                    {
                    Success=true,
                    Value=value?.ToString( )??"null",
                    ElapsedMilliseconds=stopwatch.ElapsedMilliseconds
                    };
                }
            catch (Exception ex) {
                stopwatch.Stop( );
                return new PLCTestResult
                    {
                    Success=false,
                    ErrorMessage=ex.Message,
                    ElapsedMilliseconds=stopwatch.ElapsedMilliseconds
                    };
                }
            }
        private async Task<bool> EnsurePLCConnected( PLCConnectionConfig config ) {
            return await Task.Run(( ) => {
                try {
                    if (config.IsConnected) {
                        return true; // 已经连接，直接返回
                        }

                    return config.Connect( ); // 尝试连接
                    }
                catch (Exception ex) {
                    LoggingService.Error($"确保PLC连接失败: {config.Name}",ex);
                    return false;
                    }
            });
            }
        private async Task<PLCTestResult> ExecutePLCWriteTest( PLCConnectionConfig config,PLCAddressConfig address,string testValue ) {
            var stopwatch = Stopwatch.StartNew();

            try {
                // 检查PLC连接
                if (config.PLCInstance==null||!config.PLCInstance.IsConnected) {
                    // 尝试重新连接
                    if (!await TryConnectPLC(config)) {
                        return new PLCTestResult { Success=false,ErrorMessage="PLC连接失败" };
                        }
                    }

                // 根据数据类型执行写入操作
                bool writeSuccess = await WriteAddressByType(config, address, testValue);

                stopwatch.Stop( );

                if (writeSuccess) {
                    // 可选：写入后立即读取验证
                    var verifyValue = await ReadAddressByType(config, address);

                    return new PLCTestResult
                        {
                        Success=true,
                        Value=$"写入成功，验证值：{verifyValue}",
                        ElapsedMilliseconds=stopwatch.ElapsedMilliseconds
                        };
                    }
                else {
                    return new PLCTestResult
                        {
                        Success=false,
                        ErrorMessage="写入操作失败",
                        ElapsedMilliseconds=stopwatch.ElapsedMilliseconds
                        };
                    }
                }
            catch (Exception ex) {
                stopwatch.Stop( );
                return new PLCTestResult
                    {
                    Success=false,
                    ErrorMessage=ex.Message,
                    ElapsedMilliseconds=stopwatch.ElapsedMilliseconds
                    };
                }
            }

        private async Task<object> ReadAddressByType( PLCConnectionConfig config,PLCAddressConfig address ) {
            return await Task.Run<object>(( ) => {
                switch (Type.GetTypeCode(address.DataType)) {
                    case TypeCode.Boolean:
                        bool boolValue = false;
                        if (config.PLCInstance.ReadRegisterBit(address.Address,ref boolValue))
                            return boolValue;
                        break;

                    case TypeCode.Int32:
                        int intValue = 0;
                        if (config.PLCInstance.ReadRegister(address.Address,ref intValue))
                            return intValue;
                        break;

                    case TypeCode.Single:
                        float floatValue = 0;
                        if (config.PLCInstance.ReadFloat(address.Address,ref floatValue))
                            return floatValue;
                        break;

                    case TypeCode.Double:
                        double doubleValue = 0;
                        if (config.PLCInstance.ReadDouble(address.Address,address.Power,ref doubleValue))
                            return doubleValue;
                        break;

                    case TypeCode.String:
                        string stringValue = "";
                        if (config.PLCInstance.ReadString(address.Address,ref stringValue,address.Length))
                            return stringValue;
                        break;
                    }

                throw new Exception($"读取地址失败：{address.Address}");
            });
            }

        private async Task<bool> WriteAddressByType( PLCConnectionConfig config,PLCAddressConfig address,string value ) {
            return await Task.Run(( ) => {
                try {
                    switch (Type.GetTypeCode(address.DataType)) {
                        case TypeCode.Boolean:
                            bool boolValue = bool.Parse(value);
                            return config.PLCInstance.WriteRegisterBit(address.Address,boolValue);

                        case TypeCode.Int32:
                            int intValue = int.Parse(value);
                            return config.PLCInstance.WriteRegister(address.Address,intValue);

                        case TypeCode.Single:
                            float floatValue = float.Parse(value);
                            return config.PLCInstance.WriteFloat(address.Address,floatValue);

                        case TypeCode.Double:
                            double doubleValue = double.Parse(value);
                            return config.PLCInstance.WriteDouble(address.Address,address.Power,doubleValue);

                        case TypeCode.String:
                            return config.PLCInstance.WriteString(address.Address,value);
                        }

                    return false;
                    }
                catch (FormatException) {
                    throw new Exception($"值格式错误：{value} 无法转换为 {address.DataType.Name}");
                    }
            });
            }

        private async Task<bool> TryConnectPLC( PLCConnectionConfig config ) {
            return await Task.Run(( ) => {
                try {
                    if (config.PLCInstance==null) {
                        config.CreatePLCInstance( );
                        }

                    return config.PLCInstance.Initialize( );
                    }
                catch (Exception ex) {
                    LoggingService.Error($"PLC连接失败：{config.Name}",ex);
                    return false;
                    }
            });
            }
        #endregion
        private string GetTestValueForDataType( ) {
            switch (_currentAddress.DataType.Name) {
                case "Boolean":
                    return "True";
                case "Int32":
                    return "100";
                case "Single":
                    return "123.45";
                case "Double":
                    return "678.90";
                case "String":
                    return "Test";
                default:
                    return "Default";
                }
            }

        private void btnSaveAll_Click( object sender,EventArgs e ) {
            // 数据绑定自动保存，只需要关闭窗体
            this.DialogResult=DialogResult.OK;
            this.Close( );
            }

        private void btnClose_Click( object sender,EventArgs e ) {
            this.DialogResult=DialogResult.OK;
            this.Close( );
            }

        private bool ValidateForm( ) {
            if (string.IsNullOrWhiteSpace(txtName.Text)) {
                MessageBox.Show("请输入地址名称","验证错误",
                    MessageBoxButtons.OK,MessageBoxIcon.Warning);
                txtName.Focus( );
                return false;
                }

            if (string.IsNullOrWhiteSpace(txtAddress.Text)) {
                MessageBox.Show("请输入PLC地址","验证错误",
                    MessageBoxButtons.OK,MessageBoxIcon.Warning);
                txtAddress.Focus( );
                return false;
                }

            if (cmbDataType.SelectedItem==null) {
                MessageBox.Show("请选择数据类型","验证错误",
                    MessageBoxButtons.OK,MessageBoxIcon.Warning);
                cmbDataType.Focus( );
                return false;
                }

            // 扫描地址：验证触发条件配置
            if (_currentDirection==AddressDirection.ReadOnly) {
                if (cmbTriggerCondition.SelectedIndex<0) {
                    MessageBox.Show("请选择触发条件","验证错误",
                        MessageBoxButtons.OK,MessageBoxIcon.Warning);
                    cmbTriggerCondition.Focus( );
                    return false;
                    }

                var triggerCondition = (TriggerCondition)cmbTriggerCondition.SelectedIndex;
                if (triggerCondition!=TriggerCondition.None) {
                    if (numTriggerThreshold.Value<=0) {
                        MessageBox.Show("触发阈值必须大于0","验证错误",
                            MessageBoxButtons.OK,MessageBoxIcon.Warning);
                        numTriggerThreshold.Focus( );
                        return false;
                        }
                    }
                }

            return true;
            }

        private void ResetForm( ) {
            _currentAddress=CreateNewAddress( );
            _isEditing=false;
            LoadFormData( );
            UpdateFormState( );
            lvAddresses.SelectedItems.Clear( );
            }

        private void UpdateFormState( ) {
            btnAdd.Enabled=!_isEditing;
            btnEdit.Enabled=_isEditing;
            btnDelete.Enabled=_isEditing;
            btnTestAddress.Enabled=_currentAddress!=null;

            if (_isEditing) {
                grpAddressConfig.Text="编辑地址";
                btnAdd.BackColor=Color.LightGray;
                btnEdit.BackColor=Color.LightBlue;
                }
            else {
                grpAddressConfig.Text="新增地址";
                btnAdd.BackColor=Color.LightGreen;
                btnEdit.BackColor=Color.LightGray;
                }
            }

        private string GetDirectionDisplayName( ) {
            return _currentDirection==AddressDirection.ReadOnly ? "扫描" : "发送";
            }

        // 辅助类用于ComboBox数据绑定
        private class ComboBoxItem {
            public string Text {
                get; set;
                }
            public Type Value {
                get; set;
                }
            }
        }
    }
