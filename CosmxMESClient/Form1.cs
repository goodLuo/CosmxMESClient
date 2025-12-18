using CosmxMESClient.interfaceConfig;
using CosmxMESClient.Models;
using CosmxMESClient.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CosmxMESClient
{
    public partial class Form1 : Form
    {
        private PublicLibraryService _publicService;
        private BakeService _bakeService;
        private IJInfoService _ijService;

        // PLC配置管理器
        private PLCAddressManager _plcAddressManager;
        private FormPLCConfig _plcConfigForm;
        private FormUploadData _uploadDataForm;


        private SimplifiedPLCReconnectManager _reconnectManager;

        // PLC状态显示相关
        private Dictionary<string, ListViewItem> _plcStatusItems;
        private object _statusLock = new object();
        private System.Timers.Timer _statusRefreshTimer;
        private readonly Dictionary<string, bool> _eventSubscriptions = new Dictionary<string, bool>();
        public Form1()
        {
            InitializeComponent();
            // 初始化日志系统
            InitializeLogging();
            InitializeServices();

            _plcStatusItems = new Dictionary<string, ListViewItem>();

            // 初始化UI
            InitializeUI();
            InitializeStatusRefreshTimer(); // 初始化状态刷新定时器

            // 设置默认值
            txtMachineNo.Text = "10MEJQ10-1204";
            cmbProcName.SelectedIndex = 2; // 注液工序
            cmbIJType.SelectedIndex = 0;   // 一次注液

            UpdateStatus("系统启动中...", true);
            AppendLog("MES客户端启动成功");

            InitializeMenu();

            // 重要修改：延迟初始化PLC管理器
            _ = InitializePLCManagerAsync();
            // 加载上传数据配置
            _ = UploadDataConfigManager.GetConfig();
        }
        private void InitializePLCManager()
        {
            _plcAddressManager.DataRead += (sender, e) =>
            {
                // 统一处理所有数据读取，不再需要复杂的分发逻辑
                HandlePLCDataRead(e);
            };
        }
        #region 初始化方法
        private void InitializeStatusRefreshTimer()
        {
            _statusRefreshTimer = new System.Timers.Timer(1000); // 1秒刷新一次
            _statusRefreshTimer.Elapsed += (s, e) => RefreshStatusDisplay();
            _statusRefreshTimer.Start();
        }
        private void InitializeStatusDisplay()
        {
            try
            {
                if (lstPLCStatus != null)
                {
                    lstPLCStatus.BeginUpdate();
                    lstPLCStatus.Items.Clear();
                    lstPLCStatus.EndUpdate();
                }
                AppendLog("PLC状态显示系统已初始化");
            }
            catch (Exception ex)
            {
                LoggingService.Error("状态显示初始化失败", ex);
            }
        }
        private void InitializeUI()
        {
            // 确保界面立即响应
            UpdateStatus("界面初始化完成", true);
        }
        private void InitializeLogging()
        {
            try
            {
                // 设置日志输出到UI
                LoggingService.SetLogRichTextBox(txtLog);
                LoggingService.Initialize();

                LoggingService.Info("日志系统初始化完成");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"日志初始化失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void InitializeServices()
        {
            _publicService = new PublicLibraryService();
            _bakeService = new BakeService();
            _ijService = new IJInfoService();
        }
        #endregion
        #region PLC管理核心方法
        // 初始化PLC管理器
        private async Task InitializePLCManagerAsync()
        {
            try
            {
                AppendLog("开始初始化PLC管理器...");
                _plcAddressManager = PLCAddressManager.Instance;
                InitializeStatusDisplay();

                // 只加载配置，不立即连接
                await LoadPLCConfigsAsync();

                InitializePLCManager();

                // 加载配置后重建触发依赖关系
                RebuildTriggerDependencies();

                // 延迟3秒后统一连接（避免重复）
                await Task.Delay(3000);

                // 使用新的单次连接方法
                await ConnectAllPLCsOnceAsync();

                UpdateStatus("系统就绪", true);
                AppendLog("PLC管理器初始化完成");
            }
            catch (Exception ex)
            {
                LoggingService.Error("PLC管理器初始化失败", ex);
                UpdateStatus($"初始化失败: {ex.Message}", false);
            }
        }

        private void RebuildTriggerDependencies()
        {
            try
            {
                _plcAddressManager.RebuildAllTriggerDependencies();

                // 显示依赖关系信息
                var dependencies = _plcAddressManager.GetAllDependencies();
                if (dependencies.Count > 0)
                {
                    AppendLog($"发现 {dependencies.Count} 个触发依赖关系");
                    foreach (var kvp in dependencies)
                    {
                        AppendLog($"  触发地址: {kvp.Key} -> 依赖地址: {string.Join(", ", kvp.Value)}");
                    }
                }
                else
                {
                    AppendLog("未发现触发依赖关系");
                }
            }
            catch (Exception ex)
            {
                LoggingService.Error("重建触发依赖关系失败", ex);
            }
        }
        private async Task ConnectAllPLCsOnceAsync()
        {
            var configs = GlobalVariables.PLCConnections.Where(c => c.IsEnabled).ToList();

            AppendLog($"开始连接 {configs.Count} 个启用的PLC...");

            foreach (var config in configs)
            {
                // 检查是否已连接
                if (config.IsConnected)
                {
                    AppendLog($"PLC {config.Name} 已连接，跳过");
                    continue;
                }

                try
                {
                    // 使用统一的连接方法
                    bool connected = await SafeConnectPLCAsync(config);
                    if (connected)
                    {
                        AppendLog($"PLC {config.Name} 连接成功");
                        // 安全启动自动读取
                        await SafeStartAutoRead(config);
                    }
                    else
                    {
                        AppendLog($"PLC {config.Name} 连接失败");
                    }
                }
                catch (Exception ex)
                {
                    LoggingService.Error($"连接PLC失败: {config.Name}", ex);
                }

                await Task.Delay(500); // 连接间隔
            }
        }
        // 安全的PLC连接方法
        private async Task<bool> SafeConnectPLCAsync(PLCConnectionConfig config)
        {
            try
            {
                UpdateStatusItem(config.Key, "连接中...", Color.Orange, 0);

                // 检查是否已连接
                if (config.IsConnected)
                {
                    AppendLog($"PLC {config.Name} 已经连接");
                    return true;
                }

                // 异步连接
                bool success = await Task.Run(() =>
                {
                    try
                    {
                        return config.Connect();
                    }
                    catch (Exception ex)
                    {
                        LoggingService.Error($"PLC连接异常: {config.Name}", ex);
                        return false;
                    }
                });

                if (success)
                {
                    UpdateStatusItem(config.Key, "已连接", Color.Green, 0);
                    // 订阅事件
                    SubscribeToPLCDataEvents(config);
                    return true;
                }
                else
                {
                    UpdateStatusItem(config.Key, "连接失败", Color.Red, 0);
                    return false;
                }
            }
            catch (Exception ex)
            {
                LoggingService.Error($"安全连接PLC失败: {config.Name}", ex);
                UpdateStatusItem(config.Key, "连接异常", Color.Red, 0);
                return false;
            }
        }
        private async Task LoadPLCConfigsAsync()
        {
            try
            {
                AppendLog("开始加载PLC配置...");
                GlobalVariables.PLCConnections = await Task.Run(() => PLCConfigManager.LoadConfigs());
                if (GlobalVariables.PLCConnections.Count == 0)
                {
                    AppendLog("未找到PLC配置，请通过菜单进行配置");
                    return;
                }

                int enabledCount = 0;
                foreach (var config in GlobalVariables.PLCConnections)
                {
                    if (config.IsEnabled)
                    {
                        enabledCount++;
                        // 只注册配置，不连接
                        RegisterPLCConfigOnly(config);
                    }
                }
                RefreshAddress();

                AppendLog($"已加载 {enabledCount} 个启用的PLC配置");
            }
            catch (Exception ex)
            {
                LoggingService.Error("加载PLC配置失败", ex);
                AppendLog($"加载PLC配置失败: {ex.Message}");
            }
        }
        private void RefreshAddress()
        {
            GlobalVariables.AvailableTriggerAddresses.Clear();
            GlobalVariables.AllAvailableTriggerAddresses.Clear();

            // 获取所有可用的触发条件地址（除当前地址外的其他扫描地址）
            GlobalVariables.AvailableTriggerAddresses = GlobalVariables.PLCConnections.SelectMany(p => p.ScanAddresses)
          .Where(addr => addr.IsEnabled && addr.TriggerCondition != TriggerCondition.None).ToDictionary(key => key.Key);

            GlobalVariables.AllAvailableTriggerAddresses = GlobalVariables.PLCConnections.SelectMany(p => p.ScanAddresses)
          .Where(addr => addr.IsEnabled).ToDictionary(key => key.Key);

        }
        // 只注册配置，不连接
        private void RegisterPLCConfigOnly(PLCConnectionConfig config)
        {
            try
            {
                if (_plcAddressManager.RegisterPLCConfig(config))
                {
                    CreateStatusItem(config);
                    AppendLog($"已注册PLC配置: {config.Name}");
                }
            }
            catch (Exception ex)
            {
                LoggingService.Error($"注册PLC配置失败: {config.Name}", ex);
            }
        }
        private async Task SafeStartAutoRead(PLCConnectionConfig config)
        {
            try
            {
                // 检查PLC连接状态
                if (config.PLCInstance == null || !config.PLCInstance.IsConnected)
                {
                    throw new InvalidOperationException("PLC未连接");
                }

                // 先停止可能的现有自动读取
                config.StopAutoRead();
                await Task.Delay(500);

                // 启动自动读取
                config.StartAutoRead();

                // 验证自动读取是否真正启动
                await Task.Delay(1000);
                if (!IsAutoReadRunning(config))
                {
                    throw new Exception("自动读取未正常启动");
                }
            }
            catch (Exception ex)
            {
                LoggingService.Error($"安全启动自动读取失败: {config.Name}", ex);
                throw;
            }
        }
        #endregion
        #region 状态显示更新
        private void UpdateStatusItem(string configKey, string status, Color color, int retryCount)
        {
            if (lstPLCStatus == null || lstPLCStatus.IsDisposed)
                return;

            if (lstPLCStatus.InvokeRequired)
            {
                lstPLCStatus.Invoke(new Action<string, string, Color, int>(UpdateStatusItem),
                    configKey, status, color, retryCount);
                return;
            }

            try
            {
                if (_plcStatusItems.TryGetValue(configKey, out var item))
                {
                    item.SubItems[1].Text = status;
                    item.SubItems[1].ForeColor = color;
                    item.SubItems[2].Text = DateTime.Now.ToString("HH:mm:ss");
                    item.SubItems[3].Text = retryCount.ToString();

                    // 设置行颜色
                    item.BackColor = color == Color.Green ? Color.LightGreen :
                                   color == Color.Red ? Color.LightPink :
                                   color == Color.Orange ? Color.LightYellow : Color.White;
                }
            }
            catch (Exception ex)
            {
                LoggingService.Error("更新状态项失败", ex);
            }
        }
        private void RefreshStatusDisplay()
        {
            if (lstPLCStatus == null || lstPLCStatus.IsDisposed)
                return;

            if (lstPLCStatus.InvokeRequired)
            {
                lstPLCStatus.Invoke(new Action(RefreshStatusDisplay));
                return;
            }

            try
            {
                // 更新在线时间显示
                //var configs = PLCConfigManager.LoadConfigs();
                foreach (var config in GlobalVariables.PLCConnections.Where(c => c.IsConnected))
                {
                    if (_plcStatusItems.TryGetValue(config.Key, out var item))
                    {
                        var onlineTime = DateTime.Now - config.LastConnectedTime;
                        item.SubItems[1].Text = $"运行中({onlineTime:mm\\:ss})";
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingService.Error("刷新状态显示失败", ex);
            }
        }
        #endregion
        #region 加载PLC配置并启动自动读取
        private void LoadPLCConfigs()
        {
            try
            {
                //var configs = PLCConfigManager.LoadConfigs();

                AppendLog($"找到 {GlobalVariables.PLCConnections.Count} 个PLC配置");

                int enabledCount = 0;
                foreach (var config in GlobalVariables.PLCConnections)
                {
                    if (config.IsEnabled)
                    {
                        RegisterPLCConfig(config);
                        enabledCount++;
                    }
                    else
                    {
                        AppendLog($"PLC配置已禁用: {config.Name}");
                    }
                }

                AppendLog($"已启用 {enabledCount} 个PLC配置，共 {GlobalVariables.PLCConnections.Count} 个配置");

                if (enabledCount == 0)
                {
                    AppendLog("警告：没有启用的PLC配置，请检查配置");
                }
            }
            catch (Exception ex)
            {
                LoggingService.Error("加载PLC配置失败", ex);
                AppendLog($"加载PLC配置失败: {ex.Message}");
            }
        }
        private void RegisterPLCConfig(PLCConnectionConfig config)
        {
            try
            {
                // 注册到地址管理器
                if (_plcAddressManager.RegisterPLCConfig(config))
                {
                    // 创建状态显示项
                    CreateStatusItem(config);

                    AppendLog($"已注册PLC配置: {config.Name} (Key: {config.Key})");

                }
                else
                {
                    AppendLog($"注册到地址管理器失败: {config.Name}");
                }
            }
            catch (Exception ex)
            {
                LoggingService.Error($"注册PLC配置失败: {config.Name}", ex);
                AppendLog($"注册PLC配置失败: {config.Name} - {ex.Message}");
            }
        }
        private async Task<bool> TryConnectPLCOnceAsync(PLCConnectionConfig config)
        {
            try
            {
                UpdateStatusItem(config.Key, "连接中...", Color.Orange, 0);

                bool success = await Task.Run(() => config.Connect());

                if (success)
                {
                    config.StartAutoRead();
                    UpdateStatusItem(config.Key, "已连接", Color.Green, 0);
                    AppendLog($"PLC连接成功: {config.Name}");
                    return true;
                }
                else
                {
                    UpdateStatusItem(config.Key, "连接失败", Color.Red, 0);
                    AppendLog($"PLC连接失败: {config.Name}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                LoggingService.Error($"PLC连接异常: {config.Name}", ex);
                UpdateStatusItem(config.Key, "连接异常", Color.Red, 0);
                return false;
            }
        }
        private void CreateStatusItem(PLCConnectionConfig config)
        {
            if (lstPLCStatus == null || lstPLCStatus.IsDisposed)
                return;

            if (lstPLCStatus.InvokeRequired)
            {
                lstPLCStatus.Invoke(new Action<PLCConnectionConfig>(CreateStatusItem), config);
                return;
            }

            try
            {
                var item = new ListViewItem(config.Name);
                item.SubItems.Add("未连接");
                item.SubItems.Add(DateTime.Now.ToString("HH:mm:ss"));
                item.SubItems.Add("0");
                item.Tag = config.Key;

                lstPLCStatus.Items.Add(item);
                _plcStatusItems[config.Key] = item;

                AppendLog($"创建状态显示项: {config.Name}");
            }
            catch (Exception ex)
            {
                LoggingService.Error("创建状态项失败", ex);
            }
        }
        #endregion
        #region PLC事件管理

        // 订阅PLC数据事件
        private void SubscribeToPLCDataEvents(PLCConnectionConfig config)
        {
            if (config.PLCInstance == null)
                return;

            string key = config.Key;

            // 避免重复订阅
            if (_eventSubscriptions.ContainsKey(key) && _eventSubscriptions[key])
            {
                UnsubscribeFromPLCDataEvents(config);
            }

            try
            {
                // 订阅数据读取事件
                config.PLCInstance.DataRead -= OnPLCDataRead; // 先取消
                config.PLCInstance.DataRead += OnPLCDataRead;

                // 订阅心跳事件
                config.PLCInstance.HeartbeatStatusChanged -= (sender, e) =>
                    OnPLCHeartbeatStatusChanged(sender, e, config);

                config.PLCInstance.HeartbeatStatusChanged += (senderL, eL) =>
                    OnPLCHeartbeatStatusChanged(senderL, eL, config);

                // 订阅写入完成事件
                config.PLCInstance.DataWriteCompleted -= OnPLCDataWriteCompleted;
                config.PLCInstance.DataWriteCompleted += OnPLCDataWriteCompleted;

                _eventSubscriptions[key] = true;
                AppendLog($"PLC事件订阅成功: {config.Name}");
            }
            catch (Exception ex)
            {
                LoggingService.Error($"订阅PLC事件失败: {config.Name}", ex);
                _eventSubscriptions[key] = false;
            }
        }
        private void UnsubscribeFromPLCDataEvents(PLCConnectionConfig config)
        {
            if (config.PLCInstance == null)
                return;
            try
            {
                config.PLCInstance.DataRead -= OnPLCDataRead;
                config.PLCInstance.HeartbeatStatusChanged -= (sender, e) =>
                    OnPLCHeartbeatStatusChanged(sender, e, config);
                config.PLCInstance.DataWriteCompleted -= OnPLCDataWriteCompleted;

                string key = config.Key;
                if (_eventSubscriptions.ContainsKey(key))
                {
                    _eventSubscriptions[key] = false;
                }
            }
            catch (Exception ex)
            {
                LoggingService.Error($"取消订阅PLC事件失败: {config.Name}", ex);
            }
        }
        private bool IsAutoReadRunning(PLCConnectionConfig config)
        {
            try
            {
                if (config.PLCInstance == null)
                    return false;

                // 检查自动读取键值是否存在（通过反射或接口方法）
                var keys = config.PLCInstance.GetAutoReadKeys();
                return keys != null && keys.Count > 0;
            }
            catch
            {
                return false;
            }
        }
        // PLC数据读取事件处理（自动读取的核心）
        private void OnPLCDataRead(object sender, DataReadEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler<DataReadEventArgs>(OnPLCDataRead), sender, e);
                return;
            }

            try
            {
                // 查找对应的PLC配置
                var config = GlobalVariables.PLCConnections
            .FirstOrDefault(p => p.PLCInstance.IPEnd.Equals(e.LocalEndPoint));

                if (config != null)
                {
                    // 查找对应的地址配置
                    var address = config.ScanAddresses
                .FirstOrDefault(a => a.Address == e.Address);

                    if (address != null)
                    {
                        // 如果这个地址是触发地址，检查是否满足条件
                        if (address.TriggerCondition != TriggerCondition.None)
                        {
                            bool triggered = address.CheckTriggerConditionEnhanced(e.Value, address.LastValue).IsTriggered;
                            if (triggered)
                            {
                                // 触发依赖此地址的所有扫描地址
                                _ = PLCAddressManager.Instance.TriggerDependentReadsAsync(address.Key, config);
                                // 处理业务逻辑
                                _ = _plcAddressManager.ReadAddressInternal(config, address, e.Value, e.ValueType);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                LoggingService.Error("处理PLC数据读取事件失败", ex);
            }
        }
        // PLC心跳状态事件处理
        private void OnPLCHeartbeatStatusChanged(object sender, HeartbeatEventArgs e, PLCConnectionConfig config)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<object, HeartbeatEventArgs, PLCConnectionConfig>(OnPLCHeartbeatStatusChanged),
                    sender, e, config);
                return;
            }

            string statusMessage = e.IsAlive ?
            $"PLC心跳正常" :
            $"PLC心跳异常: {e.Message}";

            if (!e.IsAlive)
            {
                LoggingService.Warn($"{config.Name} - {statusMessage}");
                UpdateStatusItem(config.Key, "心跳异常", Color.Orange, e.RetryCount);
            }
            else
            {
                UpdateStatusItem(config.Key, "运行中", Color.Green, 0);
            }
        }
        // PLC数据写入完成事件处理
        private void OnPLCDataWriteCompleted(object sender, DataWriteEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler<DataWriteEventArgs>(OnPLCDataWriteCompleted), sender, e);
                return;
            }

            string result = e.Success ? "成功" : "失败";
            string message = $"PLC数据写入{result} - 地址: {string.Join(",", e.Addresses)}";

            if (!e.Success && !string.IsNullOrEmpty(e.ErrorMessage))
            {
                message += $", 错误: {e.ErrorMessage}";
            }

            AppendLog(message);
        }
        // 更新设备状态显示
        private void UpdateEquipmentStatusDisplay(string statusText)
        {
            // 可以在界面上显示设备状态
            // 例如：更新状态栏、指示灯等
        }

        #endregion
        #region 公共接口方法

        private async void btnGetProductCode_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMachineNo.Text))
            {
                LoggingService.Warn("获取产品型号失败: 设备编号为空");
                MessageBox.Show("请输入设备编号", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetButtonEnabled(btnGetProductCode, false);
            LoggingService.Info($"开始获取产品型号，设备: {txtMachineNo.Text}");

            try
            {
                var response = await _publicService.GetProjCodeAsync(txtMachineNo.Text);

                if (response.IsSuccess)
                {
                    if (response.Data != null)
                    {
                        var productCodes = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(
                        response.Data.ToString() ?? "[]");

                        lstProductCodes.Items.Clear();
                        foreach (var code in productCodes)
                        {
                            lstProductCodes.Items.Add(code);
                        }

                        LoggingService.Info($"成功获取 {productCodes.Length} 个产品型号");
                        UpdateStatus($"获取成功: {productCodes.Length} 个型号", true);
                    }
                    else
                    {
                        LoggingService.Warn("获取产品型号成功，但返回数据为空");
                        UpdateStatus("获取成功，但无数据", true);
                    }
                }
                else
                {
                    LoggingService.Error($"获取产品型号失败: {response.Msg}");
                    UpdateStatus($"获取失败: {response.Msg}", false);
                }
            }
            catch (Exception ex)
            {
                LoggingService.Error("获取产品型号发生错误", ex);
                UpdateStatus($"错误: {ex.Message}", false);
            }
            finally
            {
                SetButtonEnabled(btnGetProductCode, true);
            }
        }

        private async void btnCardCheck_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCardNo.Text) || string.IsNullOrEmpty(txtMachineNo.Text))
            {
                MessageBox.Show("请输入卡号和设备编号", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbProcName.SelectedItem == null)
            {
                MessageBox.Show("请选择工序", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetButtonEnabled(btnCardCheck, false);
            AppendLog($"开始刷卡验证，卡号: {txtCardNo.Text}, 工序: {cmbProcName.SelectedItem}");

            try
            {
                var response = await _publicService.CheckCardAsync(
                    txtCardNo.Text,
                    txtMachineNo.Text,
                    cmbProcName.SelectedItem.ToString());

                if (response.IsSuccess)
                {
                    // 解析刷卡返回数据
                    var cardResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<CardCheckResponse>(
                        response.Data?.ToString() ?? "{}");

                    AppendLog($"刷卡验证成功 - 工号: {cardResponse?.EmployeeNo}, 权限: {cardResponse?.PermissionLevel}");
                    UpdateStatus("刷卡验证成功", true);

                    // 显示详细信息
                    ShowCardCheckResult(cardResponse);
                }
                else
                {
                    AppendLog($"刷卡验证失败: {response.Msg}");
                    UpdateStatus($"验证失败: {response.Msg}", false);
                    MessageBox.Show($"刷卡验证失败: {response.Msg}", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                AppendLog($"刷卡验证错误: {ex.Message}");
                UpdateStatus($"错误: {ex.Message}", false);
            }
            finally
            {
                SetButtonEnabled(btnCardCheck, true);
            }
        }

        private void ShowCardCheckResult(CardCheckResponse result)
        {
            if (result == null)
                return;

            string message = $"刷卡验证成功!\n\n" +
                           $"员工工号: {result.EmployeeNo}\n" +
                           $"部门: {result.Department}\n" +
                           $"权限等级: {result.PermissionLevel}\n" +
                           $"验证时间: {result.CheckTime}\n\n" +
                           $"允许开机生产";

            MessageBox.Show(message, "刷卡成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion
        #region 烘烤工序方法

        private async void btnCheckCell_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBakeCellName.Text) || string.IsNullOrEmpty(txtBakeProjCode.Text))
            {
                MessageBox.Show("请输入电芯条码和产品型号", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetButtonEnabled(btnCheckCell, false);
            AppendLog($"开始电芯校验，条码: {txtBakeCellName.Text}, 型号: {txtBakeProjCode.Text}");

            try
            {
                var response = await _bakeService.CheckCellAsync(
                    txtBakeCellName.Text,
                    txtBakeProjCode.Text,
                    txtMachineNo.Text);

                if (response.IsSuccess)
                {
                    AppendLog("电芯校验通过");
                    UpdateStatus("电芯校验成功", true);
                    MessageBox.Show("电芯校验通过，可以继续生产", "成功",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    AppendLog($"电芯校验失败: {response.Msg}");
                    UpdateStatus($"校验失败: {response.Msg}", false);
                    MessageBox.Show($"电芯校验失败: {response.Msg}", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                AppendLog($"电芯校验错误: {ex.Message}");
                UpdateStatus($"错误: {ex.Message}", false);
            }
            finally
            {
                SetButtonEnabled(btnCheckCell, true);
            }
        }

        private async void btnUploadBakeData_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBakeCellName.Text) || string.IsNullOrEmpty(txtBakeProjCode.Text))
            {
                MessageBox.Show("请输入电芯条码和产品型号", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetButtonEnabled(btnUploadBakeData, false);
            AppendLog($"开始上传烘烤数据，条码: {txtBakeCellName.Text}");

            try
            {
                // 创建烘烤数据请求
                var request = new UploadBakeDataRequest
                {
                    Cell_Name = txtBakeCellName.Text,
                    Proj_Code = txtBakeProjCode.Text,
                    Quality = "0", // 合格
                    NGReason = "",
                    StartDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                    Baking_Start_Time = DateTime.Now.AddHours(-2).ToString("yyyy/MM/dd HH:mm:ss"),
                    Baking_End_Time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                    Cavity = "1",
                    Lay = "2",
                    Position = "10",
                    Temperature = "85",
                    Machine_No = txtMachineNo.Text,
                    Operator = "AutoSystem",
                    Water_Content = "150.5",
                    IsTestSample = "0"
                };

                var response = await _bakeService.UploadBakeDataAsync(request);

                if (response.IsSuccess)
                {
                    AppendLog("烘烤数据上传成功");
                    UpdateStatus("数据上传成功", true);
                }
                else
                {
                    AppendLog($"烘烤数据上传失败: {response.Msg}");
                    UpdateStatus($"上传失败: {response.Msg}", false);
                }
            }
            catch (Exception ex)
            {
                AppendLog($"烘烤数据上传错误: {ex.Message}");
                UpdateStatus($"错误: {ex.Message}", false);
            }
            finally
            {
                SetButtonEnabled(btnUploadBakeData, true);
            }
        }

        private async void btnGetWaterInfo_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBakeCellName.Text))
            {
                MessageBox.Show("请输入电芯条码", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetButtonEnabled(btnGetWaterInfo, false);
            AppendLog($"获取水分数据，条码: {txtBakeCellName.Text}");

            try
            {
                var response = await _bakeService.GetWaterInfoAsync(txtBakeCellName.Text);

                if (response.IsSuccess)
                {
                    AppendLog("水分数据获取成功");
                    UpdateStatus("水分数据获取成功", true);

                    // 显示水分数据
                    var waterData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(response.Data?.ToString() ?? "{}");
                    string waterInfo = $"水分含量: {waterData?.Water_Content ?? "N/A"} ppm\n" +
                                     $"检测时间: {waterData?.Water_Date ?? "N/A"}\n" +
                                     $"状态: {waterData?.IsOk ?? "N/A"}";

                    MessageBox.Show(waterInfo, "水分数据", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    AppendLog($"水分数据获取失败: {response.Msg}");
                    UpdateStatus($"获取失败: {response.Msg}", false);
                }
            }
            catch (Exception ex)
            {
                AppendLog($"水分数据获取错误: {ex.Message}");
                UpdateStatus($"错误: {ex.Message}", false);
            }
            finally
            {
                SetButtonEnabled(btnGetWaterInfo, true);
            }
        }

        #endregion

        #region 注液工序方法

        private async void btnElectCheck_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtElectrolyte.Text) || string.IsNullOrEmpty(txtIJProjCode.Text))
            {
                MessageBox.Show("请输入电解液编号和产品型号", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetButtonEnabled(btnElectCheck, false);
            AppendLog($"开始电解液验证，电解液: {txtElectrolyte.Text}, 型号: {txtIJProjCode.Text}");

            try
            {
                var response = await _ijService.ElectCheckAsync(
                    txtElectrolyte.Text,
                    txtIJProjCode.Text,
                    txtMachineNo.Text,
                    cmbIJType.SelectedItem?.ToString() ?? "一次注液");

                if (response.IsSuccess)
                {
                    AppendLog("电解液验证通过");
                    UpdateStatus("电解液验证成功", true);

                    // 解析返回的电解液信息
                    var electInfo = response.Msg.Split(',');
                    if (electInfo.Length >= 4)
                    {
                        string message = $"电解液验证成功!\n\n" +
                                       $"物料编号: {electInfo[1]}\n" +
                                       $"物料名称: {electInfo[2]}\n" +
                                       $"物料批号: {electInfo[3]}\n" +
                                       $"物料数量: {electInfo[4]}g";

                        MessageBox.Show(message, "电解液验证", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("电解液验证通过", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    AppendLog($"电解液验证失败: {response.Msg}");
                    UpdateStatus($"验证失败: {response.Msg}", false);
                    MessageBox.Show($"电解液验证失败: {response.Msg}", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                AppendLog($"电解液验证错误: {ex.Message}");
                UpdateStatus($"错误: {ex.Message}", false);
            }
            finally
            {
                SetButtonEnabled(btnElectCheck, true);
            }
        }

        private async void btnGetIJPara_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtIJProjCode.Text))
            {
                MessageBox.Show("请输入产品型号", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetButtonEnabled(btnGetIJPara, false);
            AppendLog($"获取注液参数，型号: {txtIJProjCode.Text}");

            try
            {
                var response = await _ijService.GetIJParaAsync(
                    txtIJProjCode.Text,
                    cmbIJType.SelectedItem?.ToString() ?? "一次注液");

                if (response.IsSuccess && response.Data != null)
                {
                    AppendLog("注液参数获取成功");
                    UpdateStatus("参数获取成功", true);

                    // 显示参数信息
                    var parameters = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic[]>(response.Data.ToString() ?? "[]");
                    string paraInfo = $"注液参数规格 ({parameters.Length} 个参数):\n\n";

                    foreach (var para in parameters)
                    {
                        paraInfo += $"{para.PARA_CODE}: {para.LSL} ~ {para.USL}\n";
                    }

                    MessageBox.Show(paraInfo, "注液参数", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    AppendLog($"注液参数获取失败: {response.Msg}");
                    UpdateStatus($"获取失败: {response.Msg}", false);
                }
            }
            catch (Exception ex)
            {
                AppendLog($"注液参数获取错误: {ex.Message}");
                UpdateStatus($"错误: {ex.Message}", false);
            }
            finally
            {
                SetButtonEnabled(btnGetIJPara, true);
            }
        }

        private async void btnCheckCondition_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtIJCellName.Text) || string.IsNullOrEmpty(txtIJProjCode.Text))
            {
                MessageBox.Show("请输入电芯条码和产品型号", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Button btn = sender as Button;
            SetButtonEnabled(btn, false);
            AppendLog($"注液前检查，条码: {txtIJCellName.Text}, 型号: {txtIJProjCode.Text}");

            try
            {
                var response = await _ijService.CheckConditionAsync(
                    txtIJCellName.Text,
                    txtIJProjCode.Text,
                    txtMachineNo.Text,
                    cmbIJType.SelectedItem?.ToString() ?? "一次注液");

                if (response.IsSuccess)
                {
                    AppendLog("注液前检查通过");
                    UpdateStatus("检查通过", true);
                    MessageBox.Show("注液前检查通过，可以继续生产", "成功",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    AppendLog($"注液前检查失败: {response.Msg}");
                    UpdateStatus($"检查失败: {response.Msg}", false);
                    MessageBox.Show($"注液前检查失败: {response.Msg}", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                AppendLog($"注液前检查错误: {ex.Message}");
                UpdateStatus($"错误: {ex.Message}", false);
            }
            finally
            {
                SetButtonEnabled(btn, true);
            }
        }

        #endregion

        #region 通用方法

        private void lstProductCodes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstProductCodes.SelectedItem != null)
            {
                string selectedCode = lstProductCodes.SelectedItem.ToString();
                txtBakeProjCode.Text = selectedCode;
                txtIJProjCode.Text = selectedCode;
                AppendLog($"已选择产品型号: {selectedCode}");
            }
        }

        private async void btnTestConnection_Click(object sender, EventArgs e)
        {
            AppendLog("开始测试MES服务连接...");

            try
            {
                // 使用获取产品型号接口测试连接
                var response = await _publicService.GetProjCodeAsync(txtMachineNo.Text);

                if (response.IsSuccess)
                {
                    AppendLog("MES服务连接测试成功");
                    UpdateStatus("连接正常", true);
                    MessageBox.Show("MES服务连接正常", "连接测试",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    AppendLog($"MES服务连接测试失败: {response.Msg}");
                    UpdateStatus("连接失败", false);
                }
            }
            catch (Exception ex)
            {
                AppendLog($"连接测试错误: {ex.Message}");
                UpdateStatus("连接错误", false);
                MessageBox.Show($"连接测试失败: {ex.Message}", "连接错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
        #region 工具方法
        private void AppendLog(string message)
        {
            LoggingService.Info(message);
        }
        private void UpdateStatus(string message, bool isSuccess)
        {
            if (lblStatus.InvokeRequired)
            {
                lblStatus.Invoke(new Action<string, bool>(UpdateStatus), message, isSuccess);
                return;
            }

            lblStatus.Text = message;
            lblStatus.ForeColor = isSuccess ? System.Drawing.Color.Green : System.Drawing.Color.Red;
        }
        private void SetButtonEnabled(Button button, bool enabled)
        {
            if (button.InvokeRequired)
            {
                button.Invoke(new Action<Button, bool>(SetButtonEnabled), button, enabled);
                return;
            }

            button.Enabled = enabled;
            button.BackColor = enabled ?
                System.Drawing.SystemColors.Control :
                System.Drawing.Color.LightGray;
        }
        private void ShowErrorDialog(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(ShowErrorDialog), message);
                return;
            }

            MessageBox.Show(message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion
        #region 菜单和工具栏

        private void InitializeMenu()
        {
            // 创建菜单栏
            MenuStrip mainMenu = new MenuStrip();

            // 文件菜单
            ToolStripMenuItem fileMenu = new ToolStripMenuItem("文件(&F)");

            // PLC配置菜单项
            ToolStripMenuItem plcConfigItem = new ToolStripMenuItem("PLC配置(&P)");
            plcConfigItem.Click += (s, e) => ShowPLCConfigDialog();
            fileMenu.DropDownItems.Add(plcConfigItem);

            // 数据上传配置菜单
            ToolStripMenuItem uploadDataItem = new ToolStripMenuItem("数据上传配置(&U)");
            uploadDataItem.Click += (s, e) => ShowUploadDataConfigDialog();
            fileMenu.DropDownItems.Add(uploadDataItem);


            ToolStripMenuItem exitItem = new ToolStripMenuItem("退出(&X)");
            exitItem.Click += (s, e) => Application.Exit();
            fileMenu.DropDownItems.Add(exitItem);

            // 工具菜单
            ToolStripMenuItem toolsMenu = new ToolStripMenuItem("工具(&T)");
            ToolStripMenuItem clearLogItem = new ToolStripMenuItem("清空日志(&C)");
            clearLogItem.Click += (s, e) => txtResult.Clear();
            toolsMenu.DropDownItems.Add(clearLogItem);

            // 帮助菜单
            ToolStripMenuItem helpMenu = new ToolStripMenuItem("帮助(&H)");
            ToolStripMenuItem aboutItem = new ToolStripMenuItem("关于(&A)");
            aboutItem.Click += (s, e) => ShowAboutDialog();
            helpMenu.DropDownItems.Add(aboutItem);

            mainMenu.Items.AddRange(new ToolStripItem[] { fileMenu, toolsMenu, helpMenu });
            this.MainMenuStrip = mainMenu;
            this.Controls.Add(mainMenu);
        }
        private void ShowPLCConfigDialog()
        {
            try
            {
                if (_plcConfigForm == null || _plcConfigForm.IsDisposed)
                {
                    _plcConfigForm = new FormPLCConfig();

                    // 订阅PLC配置变化事件
                    _plcConfigForm.FormClosed += OnPLCConfigFormClosed;
                }

                if (_plcConfigForm.Visible)
                {
                    _plcConfigForm.BringToFront();
                }
                else
                {
                    _plcConfigForm.Show(this);
                }
            }
            catch (Exception ex)
            {
                LoggingService.Error("打开PLC配置对话框失败", ex);
                MessageBox.Show($"打开PLC配置失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowUploadDataConfigDialog()
        {
            try
            {
                if (_uploadDataForm == null || _uploadDataForm.IsDisposed)
                {
                    _uploadDataForm = new FormUploadData();

                    // 订阅PLC配置变化事件
                    //_uploadDataForm.FormClosed += OnPLCConfigFormClosed;
                }

                if (_uploadDataForm.Visible)
                {
                    _uploadDataForm.BringToFront();
                }
                else
                {
                    _uploadDataForm.Show(this);
                }
            }
            catch (Exception ex)
            {
                LoggingService.Error("打开PLC参数上传对话框失败", ex);
                MessageBox.Show($"打开PLC参数上传失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }





        // PLC配置对话框关闭事件处理
        private void OnPLCConfigFormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                AppendLog("PLC配置已更新，重新加载配置");

                // 重新加载PLC配置
                ReloadPLCConfigs();
                // 重建触发依赖关系
                //RebuildTriggerDependencies( );
            }
            catch (Exception ex)
            {
                LoggingService.Error("重新加载PLC配置失败", ex);
            }
        }
        // 重新加载PLC配置
        private void ReloadPLCConfigs()
        {
            try
            {
                // 停止现有的自动读取
                StopAllPLCAutoRead();

                // 重新加载配置
                LoadPLCConfigs();

                AppendLog("PLC配置重新加载完成");
            }
            catch (Exception ex)
            {
                LoggingService.Error("重新加载PLC配置失败", ex);
            }
        }
        // 停止所有PLC自动读取
        private void StopAllPLCAutoRead()
        {
            try
            {
                //var configs = PLCConfigManager.LoadConfigs();
                foreach (var config in GlobalVariables.PLCConnections)
                {
                    if (config.IsEnabled)
                    {
                        config.StopAutoRead();
                        AppendLog($"已停止自动读取: {config.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingService.Error("停止PLC自动读取失败", ex);
            }
        }
        private void ShowAboutDialog()
        {
            string aboutInfo = "冠宇MES系统客户端\n\n" +
                              "版本: 3.1\n" +
                              "开发: 珠海冠宇电池有限公司\n" +
                              "描述: MES系统接口调用客户端\n" +
                              "支持工序: 烘烤、注液、化成、分选等";

            MessageBox.Show(aboutInfo, "关于", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion
        private void ClearLog(object sender, EventArgs e)
        {
            LoggingService.ClearUILog();
        }

        private void SaveLog(object sender, EventArgs e)
        {
            try
            {
                using (var saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "日志文件 (*.log)|*.log|文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*";
                    saveDialog.FileName = $"MESClient_Log_{DateTime.Now:yyyyMMdd_HHmmss}.log";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(saveDialog.FileName, txtLog.Text);
                        LoggingService.Info($"日志已保存到: {saveDialog.FileName}");
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingService.Error("保存日志失败", ex);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // 停止所有PLC自动读取
                //var configs = PLCConfigManager.LoadConfigs();
                foreach (var config in GlobalVariables.PLCConnections.Where(c => c.IsConnected))
                {
                    config.StopAutoRead();
                    config.Disconnect();
                }

                AppendLog("应用程序关闭完成");
            }
            catch (Exception ex)
            {
                LoggingService.Error("窗体关闭时清理资源失败", ex);
            }
        }
        private async void btnStartAllConnections_Click(object sender, EventArgs e)
        {
            try
            {
                AppendLog("手动启动所有PLC连接...");

                var configs = GlobalVariables.PLCConnections
                .Where(c => c.IsEnabled && !c.IsConnected)
                .ToList();

                if (configs.Count == 0)
                {
                    AppendLog("所有PLC已连接或没有启用的配置");
                    return;
                }

                foreach (var config in configs)
                {
                    await TryConnectPLCOnceAsync(config);
                }

                AppendLog("手动启动连接完成");
            }
            catch (Exception ex)
            {
                LoggingService.Error("手动启动连接失败", ex);
                ShowErrorDialog($"启动连接失败: {ex.Message}");
            }
        }

        private void btnStopAllConnections_Click(object sender, EventArgs e)
        {
            try
            {
                AppendLog("手动停止所有PLC连接...");

                var configs = GlobalVariables.PLCConnections
                .Where(c => c.IsEnabled && c.IsConnected)
                .ToList();

                foreach (var config in configs)
                {
                    config.StopAutoRead();
                    config.Disconnect();
                    UpdateStatusItem(config.Key, "已停止", Color.Gray, 0);
                }

                AppendLog("已停止所有PLC连接");
            }
            catch (Exception ex)
            {
                LoggingService.Error("手动停止连接失败", ex);
                ShowErrorDialog($"停止连接失败: {ex.Message}");
            }
        }
        #region  配置
        /// <summary>
        /// 简化的数据处理方法
        /// </summary>
        private void HandlePLCDataRead(DataReadEventArgs e)
        {
            // 根据地址直接处理，不需要复杂的事件注册
            //switch (e.Address)
            //{
            //    case "D100":
            //        ProcessTemperature(e.Value);
            //        break;
            //    case "D101":
                    ProcessPressure(e);
            ProcessPressure1(e);
            //        break;
            //    default:
            //        // 默认处理或忽略
            //        break;
            //}
        }
        // 具体的处理逻辑保持不变
        private void ProcessTemperature(object value)
        {
            if (value is int temp)
            {
                // 温度处理逻辑
                AppendLog($"温度: {temp}°C");
            }
        }

        private void ProcessPressure(DataReadEventArgs value)
        {
            //if (value is float pressure)
            //{
            //    // 压力处理逻辑
            //    AppendLog($"压力: {pressure}MPa");
            //}
           var item= UploadDataConfigManager.GetConfig().UploadItems.FirstOrDefault(p => p.Value.BindPLCScanAddress.Address == value.Address).Value;
            if (item != null)
            {
                item.ParameterValue = value.Value.ToString();

                AppendLog("参数名：" + item.ParameterName);
                AppendLog("参数值：" + item.ParameterValue);

            }
            //foreach (var item in UploadDataConfigManager.GetConfig().UploadItems)
            //{

            //}
        }
        private void ProcessPressure1(DataReadEventArgs value)
        {
            //if (value is float pressure)
            //{
            //    // 压力处理逻辑
            //    
            //}
            if (value.Address == "1")
            {
                AppendLog($"数据上传");
            }
            //foreach (var item in UploadDataConfigManager.GetConfig().UploadItems)
            //{

            //}



        }
        #endregion
    }
}
