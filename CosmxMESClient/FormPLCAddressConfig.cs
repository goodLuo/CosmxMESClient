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
            }
        private void UpdateTriggerControlsVisibility( ) {
            int selectedIndex = cmbTriggerCondition.SelectedIndex;

            // 根据选择的触发条件显示/隐藏相关控件
            bool showThreshold = selectedIndex >= 1 && selectedIndex <= 6; // 阈值相关条件
            bool showEdgeTrigger = selectedIndex == 7 || selectedIndex == 8; // 边沿触发
            bool showPercentage = selectedIndex == 9; // 百分比触发
            bool showStringConditions = selectedIndex >= 10 && selectedIndex <= 12; // 字符串条件

            lblTriggerThreshold.Visible=showThreshold||showPercentage;
            numTriggerThreshold.Visible=lblTriggerThreshold.Visible;

            chkTriggerRisingEdge.Visible=showEdgeTrigger;
            chkTriggerFallingEdge.Visible=showEdgeTrigger;

            lblTriggerDelay.Visible=selectedIndex!=0;
            numTriggerDelay.Visible=selectedIndex!=0;

            // 更新标签文本
            if (showPercentage) {
                lblTriggerThreshold.Text="变化百分比(%):";
                numTriggerThreshold.DecimalPlaces=2;
                numTriggerThreshold.Increment=0.1m;
                }
            else if (showThreshold) {
                lblTriggerThreshold.Text="触发阈值:";
                numTriggerThreshold.DecimalPlaces=3;
                numTriggerThreshold.Increment=0.001m;
                }
            }
        private void FormPLCAddressConfig_Load( object sender,EventArgs e ) {
            try {
                LoadFormData( );
                }
            catch (Exception ex) {
                LoggingService.Error("窗体加载失败",ex);
                MessageBox.Show($"窗体初始化失败: {ex.Message}","错误",
                    MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }

        private void LoadAddresses( ) {
            lvAddresses.Items.Clear( );

            // 获取实际的数据集合
            IEnumerable<PLCAddressConfig> addresses;
            if (_currentDirection==AddressDirection.ReadOnly) {
                addresses=_currentConfig.ScanAddresses.Cast<PLCAddressConfig>( );
                }
            else {
                addresses=_currentConfig.SendAddresses.Cast<PLCAddressConfig>( );
                }

            foreach (var address in addresses.OrderBy(a => a.Name)) {
                var item = new ListViewItem(address.Key);
                item.SubItems.Add(address.Name);
                item.SubItems.Add(address.Address);
                item.SubItems.Add(PLCAddressConfig.GetDataTypeDisplayName(address.DataType));
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
                    return "无触发条件";
                case TriggerCondition.GreaterThan:
                    return "大于阈值";
                case TriggerCondition.LessThan:
                    return "小于阈值";
                case TriggerCondition.Equal:
                    return "等于阈值";
                case TriggerCondition.GreaterThanOrEqual:
                    return "大于等于阈值";
                case TriggerCondition.LessThanOrEqual:
                    return "小于等于阈值";
                case TriggerCondition.NotEqual:
                    return "不等于阈值";
                case TriggerCondition.RisingEdge:
                    return "上升沿触发";
                case TriggerCondition.FallingEdge:
                    return "下降沿触发";
                case TriggerCondition.ChangePercentage:
                    return "变化百分比";
                case TriggerCondition.Contains:
                    return "包含字符串";
                case TriggerCondition.StartsWith:
                    return "以...开始";
                case TriggerCondition.EndsWith:
                    return "以...结束";
                default:
                    return "未知条件";
                }
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
            //foreach (ComboBoxItem item in cmbDataType.Items) {
            //    if (Enum.TryParse(item.Text,out TypeCode result)) {
            //        cmbDataType.SelectedItem=result;
            //        break;
            //        }
            //    }

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
                if (Enum.TryParse(selectedItem.Text,out TypeCode result)) {
                    cmbDataType.SelectedItem=result;
                    }
                else {
                    cmbDataType.SelectedItem=TypeCode.Int16;
                    }
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
                    DataType=TypeCode.Int16,
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
                    DataType=TypeCode.Int16,
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
        private object GenerateTestValue( TypeCode dataType,int iteration ) {
            switch (dataType) {
                case TypeCode.Boolean:
                    return iteration%2==0;
                case TypeCode.Int16:
                    return iteration*5;
                case TypeCode.Int32:
                    return iteration*10;
                case TypeCode.Single:
                    return iteration*1f;
                case TypeCode.Double:
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
                string testValue = PLCAddressConfig.GetTestValueForDataType(_currentAddress.DataType);

                var result = MessageBox.Show($"测试写入地址：{_currentAddress.Address}\n\n" +
            $"写入值：{testValue}\n" +
            $"数据类型：{PLCAddressConfig.GetDataTypeDisplayName(_currentAddress.DataType)}\n" +
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
                            $"数据类型：{PLCAddressConfig.GetDataTypeDisplayName(_currentAddress.DataType)}\n"+
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
        private async Task<PLCTestResult> ExecutePLCWriteTest( PLCConnectionConfig config,PLCAddressConfig address,string testValue ) {
            var stopwatch = Stopwatch.StartNew();

            try {
                // 检查PLC连接状态，但不自动重连
                if (config.PLCInstance==null||!config.PLCInstance.IsConnected) {
                    return new PLCTestResult
                        {
                        Success=false,
                        ErrorMessage="PLC未连接，请先确保PLC已连接"
                        };
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
                switch (address.DataType) {
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
                    switch (address.DataType) {
                        case TypeCode.Boolean:
                            bool boolValue = bool.Parse(value);
                            return config.PLCInstance.WriteRegisterBit(address.Address,boolValue);

                        case TypeCode.Int16:
                            int intValue = int.Parse(value);
                            return config.PLCInstance.WriteRegister(address.Address,intValue);

                        case TypeCode.Int32:
                            int int32Value = int.Parse(value);
                            return config.PLCInstance.WriteInt32(address.Address,int32Value);

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
                    throw new Exception($"值格式错误：{value} 无法转换为 {address.DataType}");
                    }
            });
            }
        #endregion

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
