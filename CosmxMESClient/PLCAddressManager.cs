using CosmxMESClient.interfaceConfig;
using CosmxMESClient.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmxMESClient
{
    public class PLCAddressManager
    {
        private static readonly PLCAddressManager _instance = new PLCAddressManager();
        public static PLCAddressManager Instance => _instance;

        private Dictionary<string, PLCConnectionConfig> _plcConfigs = new Dictionary<string, PLCConnectionConfig>();
        private readonly object _lock = new object();

        private Dictionary<string, List<PLCScanAddress>> _triggerDependencies =
        new Dictionary<string, List<PLCScanAddress>>();

        // 触发条件管理器
        private TriggerConditionManager _triggerManager = new TriggerConditionManager();

        public event EventHandler<DataReadEventArgs> DataRead;
        public event EventHandler<DataReadEventArgs> DataAutoTriggerRead;

        // 使用字典来存储特定地址的处理逻辑
        private readonly ConcurrentDictionary<string, Action<object>> _addressHandlers =
        new ConcurrentDictionary<string, Action<object>>();

        private object _triggerLock = new object();

        protected virtual void OnDataRead(DataReadEventArgs e)
        {
            // 然后触发特定地址的处理逻辑
            if (_addressHandlers.TryGetValue(e.Address, out var handler))
            {
                try
                {
                    handler(e.Value);
                }
                catch (Exception ex)
                {
                    LoggingService.Error($"地址处理逻辑执行失败: {e.Address}", ex);
                }
            }

            // 首先触发通用事件
            DataRead?.Invoke(this, e);
        }
        /// <summary>
        /// 注册地址处理逻辑（核心简化）
        /// </summary>
        public void RegisterAddressHandler(string addressKey, Action<object> handler)
        {
            _addressHandlers[addressKey] = handler;
        }

        /// <summary>
        /// 注销地址处理逻辑
        /// </summary>
        public void UnregisterAddressHandler(string addressKey)
        {
            _addressHandlers.TryRemove(addressKey, out _);
        }

        // 主程序调用接口
        public bool RegisterPLCConfig(PLCConnectionConfig config)
        {
            lock (_lock)
            {
                if (_plcConfigs.ContainsKey(config.Key))
                {
                    LoggingService.Warn($"PLC配置已存在: {config.Key}");
                    return false;
                }

                _plcConfigs[config.Key] = config;

                // 注册到触发条件管理器
                _triggerManager.RegisterPLC(config);

                LoggingService.Info($"注册PLC配置: {config.Name} (Key: {config.Key})");
                return true;
            }
        }
        // 注册触发依赖关系
        public void RegisterTriggerDependency(string triggerKey, PLCScanAddress dependentAddress)
        {
            if (!_triggerDependencies.ContainsKey(triggerKey))
            {
                _triggerDependencies[triggerKey] = new List<PLCScanAddress>();
            }

            if (!_triggerDependencies[triggerKey].Contains(dependentAddress))
            {
                _triggerDependencies[triggerKey].Add(dependentAddress);
                dependentAddress.IsTriggerDependent = true;
                LoggingService.Debug($"注册触发依赖: {triggerKey} -> {dependentAddress.Key}");
            }
        }
        // 触发地址条件满足时调用此方法
        public async Task TriggerDependentReadsAsync(string triggerKey, PLCConnectionConfig config)
        {
            if (_triggerDependencies.TryGetValue(triggerKey, out var dependents))
            {
                //config.PLCInstance.GroupContinuousAddresses(dependents.Where(d => d.IsEnabled))
                foreach (var dependent in dependents.Where(d => d.IsEnabled))
                {
                    await ManualReadScanAddressAsync(config, dependent);
                }
            }
        }
        // 重建所有触发依赖关系
        public void RebuildAllTriggerDependencies()
        {
            _triggerDependencies.Clear();

            foreach (var config in GlobalVariables.PLCConnections)
            {
                foreach (var scanAddress in config.ScanAddresses)
                {
                    // 如果该地址有触发依赖，重新注册
                    if (scanAddress.TriggerPLCScanAddress != null &&
                        !string.IsNullOrEmpty(scanAddress.TriggerPLCScanAddress.Key))
                    {
                        RegisterTriggerDependency(scanAddress.TriggerPLCScanAddress.Key, scanAddress);
                    }
                }
            }

            LoggingService.Info($"已重建触发依赖关系，共 {_triggerDependencies.Count} 个触发关系");
        }
        // 获取所有依赖关系（用于调试和显示）
        public Dictionary<string, List<string>> GetAllDependencies()
        {
            return _triggerDependencies.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Select(addr => addr.Key).ToList()
            );
        }

        // 检查某个触发地址是否有依赖
        public bool HasDependencies(string triggerKey)
        {
            return _triggerDependencies.ContainsKey(triggerKey) &&
                   _triggerDependencies[triggerKey].Count > 0;
        }

        // 获取依赖某个触发地址的所有扫描地址
        public List<PLCScanAddress> GetDependentAddresses(string triggerKey)
        {
            return _triggerDependencies.TryGetValue(triggerKey, out var dependents)
                ? dependents
                : new List<PLCScanAddress>();
        }
        /***********************************************************************************************手动读取扫描地址***********************************************************************************************/
        private async Task<bool> ManualReadScanAddressAsync(PLCConnectionConfig config, PLCScanAddress address)
        {
            try
            {
                object value = await ReadAddressByType(config, address);
                if (value != null)
                {
                    // 触发数据读取事件
                    OnDataRead(new DataReadEventArgs(
                        address.Address, value, address.DataType, config.PLCInstance.IPEnd));

                    // 检查触发条件
                    address.CheckTriggerConditionEnhanced(value, address.LastValue);
                    address.LastValue = value;
                    address.LastReadTime = DateTime.Now;

                    return true;
                }
            }
            catch (Exception ex)
            {
                LoggingService.Error($"手动读取地址失败: {address.Key}", ex);
            }
            return false;
        }
        /**************************************增加动态分组******************************************/
        private async Task<object> ReadAddressByType(PLCConnectionConfig config, PLCAddressConfig address)
        {
            

            return await Task.Run<object>(() =>
            {
                try
                {
                    switch (address.DataType)
                    {
                        case TypeCode.Boolean:
                            bool boolValue = false;
                            if (config.PLCInstance.ReadRegisterBit(address.Address, ref boolValue))
                                return boolValue;
                            break;
                        case TypeCode.Int16:
                            int intValue = 0;
                            if (config.PLCInstance.ReadRegister(address.Address, ref intValue, address.Power))
                                return intValue;
                            break;
                        case TypeCode.Int32:
                            int int32Value = 0;
                            if (config.PLCInstance.ReadInt32(address.Address, ref int32Value, address.Power))
                                return int32Value;
                            break;
                        case TypeCode.Single:
                            float floatValue = 0;
                            if (config.PLCInstance.ReadFloat(address.Address, ref floatValue, address.Power))
                                return floatValue;
                            break;
                        case TypeCode.Double:
                            double doubleValue = 0;
                            if (config.PLCInstance.ReadDouble(address.Address, ref doubleValue, address.Power))
                                return doubleValue;
                            break;
                        case TypeCode.String:
                            string stringValue = "";
                            if (config.PLCInstance.ReadString(address.Address, ref stringValue, address.Length))
                                return stringValue;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    LoggingService.Error($"读取地址失败: {address.Address}", ex);
                }
                return null;
            });
        }
        public bool UnregisterPLCConfig(string plcKey)
        {
            lock (_lock)
            {
                if (_plcConfigs.ContainsKey(plcKey))
                {
                    _triggerManager.UnregisterPLC(plcKey);
                    _plcConfigs.Remove(plcKey);
                    LoggingService.Info($"取消注册PLC配置: {plcKey}");
                    return true;
                }
                return false;
            }
        }

        // 增强的读取方法，支持触发条件检查
        public async Task<object> ReadAddressInternal(PLCConnectionConfig config, PLCScanAddress address, object newValue, TypeCode typeCode)
        {
            if (config.PLCInstance == null)
            {
                throw new InvalidOperationException("PLC未连接");
            }
            return await Task.Run<object>(() =>
            {
                try
                {
                    // 检查触发条件
                    bool shouldRead = CheckTriggerCondition(address, newValue);
                    if (!shouldRead)
                    {
                        return null; // 条件不满足，返回null
                    }
                    // 更新地址状态
                    address.LastValue = newValue;
                    address.LastReadTime = DateTime.Now;
                    // 如果触发了条件，执行清空动作
                    if (address.IsTriggered)
                    {
                        var cc = ExecuteClearActionAsync(config, address, typeCode);
                    }
                    return newValue;
                }
                catch (Exception ex)
                {
                    LoggingService.Error($"读取地址失败: {address.Address}", ex);
                    throw;
                }
            });
        }
        /// <summary>
        /// 增强的触发依赖检查
        /// </summary>
        public async Task<DependencyTriggerResult> TriggerDependentReadsEnhancedAsync(
            string triggerKey,
            PLCConnectionConfig config,
            object triggerValue)
        {
            var result = new DependencyTriggerResult { TriggerKey = triggerKey };

            if (!_triggerDependencies.TryGetValue(triggerKey, out var dependents))
            {
                result.Reason = "无依赖地址";
                return result;
            }

            var tasks = new List<Task<DependencyAddressResult>>();
            var enabledDependents = dependents.Where(d => d.IsEnabled).ToList();

            foreach (var dependent in enabledDependents)
            {
                tasks.Add(ProcessDependentAddressAsync(config, dependent, triggerValue));
            }

            if (tasks.Count > 0)
            {
                var results = await Task.WhenAll(tasks);
                result.TriggeredAddresses = results.Where(r => r.IsTriggered).ToList();
                result.SuccessCount = result.TriggeredAddresses.Count;
                result.TotalCount = enabledDependents.Count;
                result.Reason = $"成功触发 {result.SuccessCount}/{result.TotalCount} 个依赖地址";
            }

            return result;
        }
        /// <summary>
        /// 处理依赖地址
        /// </summary>
        private async Task<DependencyAddressResult> ProcessDependentAddressAsync(
            PLCConnectionConfig config,
            PLCScanAddress dependent,
            object triggerValue)
        {
            var result = new DependencyAddressResult
            {
                AddressKey = dependent.Key,
                AddressName = dependent.Description
            };

            try
            {
                // 检查是否应该触发依赖地址
                if (ShouldTriggerDependent(dependent, triggerValue))
                {
                    result.ShouldTrigger = true;

                    // 执行手动读取
                    var readResult = await ManualReadScanAddressAsync(config, dependent);
                    result.IsTriggered = readResult;
                    result.ReadValue = dependent.LastValue;
                    result.Reason = readResult ? "依赖触发成功" : "依赖触发读取失败";

                    if (readResult)
                    {
                        LoggingService.Debug($"依赖触发成功: {dependent.Key} -> 值: {dependent.LastValue}");
                    }
                }
                else
                {
                    result.Reason = "依赖触发条件不满足";
                }
            }
            catch (Exception ex)
            {
                result.IsTriggered = false;
                result.Reason = $"依赖触发异常: {ex.Message}";
                LoggingService.Error($"处理依赖地址异常: {dependent.Key}", ex);
            }

            return result;
        }
        /// <summary>
        /// 检查是否应该触发依赖地址
        /// </summary>
        private bool ShouldTriggerDependent(PLCScanAddress dependent, object triggerValue)
        {
            // 如果依赖地址没有设置触发条件，直接触发
            if (dependent.TriggerCondition == TriggerCondition.None)
                return true;


            lock (_triggerLock)
            {
                // 使用触发值来检查依赖地址的触发条件
                var triggerResult = dependent.CheckTriggerConditionEnhanced(triggerValue, dependent.LastValue);
                return triggerResult.IsTriggered;
            }


                
            
        }
        /// <summary>
        /// 验证触发依赖关系
        /// </summary>
        public DependencyValidationResult ValidateTriggerDependency(string triggerKey, string dependentKey)
        {
            var result = new DependencyValidationResult
            {
                TriggerKey = triggerKey,
                DependentKey = dependentKey
            };

            // 检查自身依赖
            if (triggerKey == dependentKey)
            {
                result.IsValid = false;
                result.Errors.Add("不能依赖自身");
                return result;
            }

            // 检查循环依赖
            if (HasCircularDependency(triggerKey, dependentKey))
            {
                result.IsValid = false;
                result.Errors.Add("检测到循环依赖");
                return result;
            }

            // 检查依赖深度（防止过深的依赖链）
            var depth = CalculateDependencyDepth(triggerKey, dependentKey);
            if (depth > 10) // 最大深度限制
            {
                result.IsValid = false;
                result.Errors.Add($"依赖链过深: {depth}层（最大10层）");
            }

            result.IsValid = true;
            return result;
        }
        /// <summary>
        /// 检查循环依赖
        /// </summary>
        private bool HasCircularDependency(string currentKey, string targetKey)
        {
            if (!_triggerDependencies.ContainsKey(targetKey))
                return false;

            var visited = new HashSet<string> { currentKey };
            var queue = new Queue<string>();
            queue.Enqueue(targetKey);

            while (queue.Count > 0)
            {
                var key = queue.Dequeue();
                if (visited.Contains(key))
                    return true;

                visited.Add(key);

                if (_triggerDependencies.TryGetValue(key, out var dependents))
                {
                    foreach (var dependent in dependents)
                    {
                        queue.Enqueue(dependent.Key);
                    }
                }
            }

            return false;
        }
        /// <summary>
        /// 计算依赖深度
        /// </summary>
        private int CalculateDependencyDepth(string startKey, string targetKey, int currentDepth = 0)
        {
            if (currentDepth > 20)
                return currentDepth; // 防止无限递归

            if (!_triggerDependencies.ContainsKey(startKey))
                return currentDepth;

            var dependents = _triggerDependencies[startKey];
            foreach (var dependent in dependents)
            {
                if (dependent.Key == targetKey)
                    return currentDepth + 1;

                var depth = CalculateDependencyDepth(dependent.Key, targetKey, currentDepth + 1);
                if (depth > 0)
                    return depth;
            }

            return 0;
        }
        /// <summary>
        /// 获取所有触发依赖关系树
        /// </summary>
        public DependencyTree GetDependencyTree()
        {
            var tree = new DependencyTree();

            foreach (var kvp in _triggerDependencies)
            {
                var node = new DependencyTreeNode
                {
                    Key = kvp.Key,
                    Dependencies = kvp.Value.Select(addr => addr.Key).ToList()
                };

                tree.Nodes.Add(node);
            }

            return tree;
        }
        /// <summary>
        /// 清理无效的依赖关系
        /// </summary>
        public void CleanupInvalidDependencies()
        {
            var invalidDependencies = new List<string>();

            foreach (var kvp in _triggerDependencies)
            {
                // 移除不存在的PLC配置的依赖
                var validDependents = kvp.Value.Where(dependent =>
            _plcConfigs.Values.Any(config =>
                config.ScanAddresses.Any(addr => addr.Key == dependent.Key))).ToList();

                if (validDependents.Count != kvp.Value.Count)
                {
                    _triggerDependencies[kvp.Key] = validDependents;
                    LoggingService.Info($"清理无效依赖: {kvp.Key} -> 移除{kvp.Value.Count - validDependents.Count}个无效依赖");
                }

                // 如果触发键不存在于任何PLC配置中，标记为无效
                if (!_plcConfigs.Values.Any(config =>
                    config.ScanAddresses.Any(addr => addr.Key == kvp.Key)))
                {
                    invalidDependencies.Add(kvp.Key);
                }
            }

            // 移除无效的依赖关系
            foreach (var invalidKey in invalidDependencies)
            {
                _triggerDependencies.Remove(invalidKey);
                LoggingService.Info($"移除无效触发键的依赖关系: {invalidKey}");
            }
        }
        // 增强的触发条件检查
        private bool CheckTriggerCondition(PLCScanAddress address, object newValue)
        {
            // 无触发条件或首次读取时直接通过
            if (address.TriggerCondition == TriggerCondition.None && address.TriggerPLCScanAddress != null || address.LastValue == null)
            {
                return true;
            }
            bool triggered = address.CheckTriggerConditionEnhanced(newValue, address.LastValue).IsTriggered;
            return triggered;
        }
        //清空动作
        private async Task<bool> ExecuteClearActionAsync(PLCConnectionConfig config, PLCScanAddress address, TypeCode action)
        {
            //// 检查PLC实例是否存在，如果不存在则创建
            //if (config.PLCInstance==null) {
            //    LoggingService.Warn($"PLC实例为空，尝试创建实例: {config.Name}");
            //    config.CreatePLCInstance( );
            //    }

            //// 检查连接状态，如果未连接则尝试连接
            //if (!config.IsConnected) {
            //    LoggingService.Warn($"PLC未连接，尝试连接: {config.Name}");
            //    bool connected = await Task.Run(() => config.Connect());
            //    if (!connected) {
            //        LoggingService.Error($"无法执行清空动作: PLC连接失败 - {config.Name}");
            //        return false;
            //        }
            //    }

            if (address.Address == null || !address.IsEnabled)
                return false;

            return await Task.Run(() =>
            {
                try
                {
                    switch (action)
                    {
                        case TypeCode.Boolean:
                            bool boolValue = false;
                            return config.PLCInstance.WriteRegisterBit(address.Address, boolValue);
                        case TypeCode.Int16:
                            int shortValue = 0;
                            return config.PLCInstance.WriteRegister(address.Address, shortValue);
                        case TypeCode.Int32:
                            int intValue = 0;
                            return config.PLCInstance.WriteInt32(address.Address, intValue);
                        case TypeCode.Single:
                            float floatValue = 0;
                            return config.PLCInstance.WriteFloat(address.Address, floatValue);
                        case TypeCode.Double:
                            double doubleValue = 0;
                            return config.PLCInstance.WriteDouble(address.Address,  doubleValue,1); // 默认power=1
                        case TypeCode.String:
                            string stringValue = "";
                            return config.PLCInstance.WriteString(address.Address, stringValue);
                        default:
                            return false;
                    }
                }
                catch (Exception ex)
                {
                    LoggingService.Error($"执行清空动作失败: {address.Address}", ex);
                    return false;
                }
            });
        }
        // 触发条件管理器
        public class TriggerConditionManager
        {
            private Dictionary<string, List<PLCScanAddress>> _plcTriggers = new Dictionary<string, List<PLCScanAddress>>();

            public void RegisterPLC(PLCConnectionConfig config)
            {
                if (!_plcTriggers.ContainsKey(config.Key))
                {
                    _plcTriggers[config.Key] = new List<PLCScanAddress>();
                }

                // 添加有触发条件的地址
                var triggerAddresses = config.ScanAddresses.Where(a => a.TriggerCondition != TriggerCondition.None).ToList();
                _plcTriggers[config.Key].AddRange(triggerAddresses);
            }

            public void UnregisterPLC(string plcKey)
            {
                _plcTriggers.Remove(plcKey);
            }

            public List<PLCScanAddress> GetTriggerAddresses(string plcKey)
            {
                return _plcTriggers.TryGetValue(plcKey, out var addresses) ? addresses : new List<PLCScanAddress>();
            }
        }
        /// <summary>
        /// 依赖关系结果类
        /// </summary>
        public class DependencyTriggerResult
        {
            public string TriggerKey
            {
                get; set;
            }
            public int SuccessCount
            {
                get; set;
            }
            public int TotalCount
            {
                get; set;
            }
            public string Reason
            {
                get; set;
            }
            public List<DependencyAddressResult> TriggeredAddresses { get; set; } = new List<DependencyAddressResult>();
        }

        public class DependencyAddressResult
        {
            public string AddressKey
            {
                get; set;
            }
            public string AddressName
            {
                get; set;
            }
            public bool ShouldTrigger
            {
                get; set;
            }
            public bool IsTriggered
            {
                get; set;
            }
            public object ReadValue
            {
                get; set;
            }
            public string Reason
            {
                get; set;
            }
        }

        public class DependencyValidationResult
        {
            public string TriggerKey
            {
                get; set;
            }
            public string DependentKey
            {
                get; set;
            }
            public bool IsValid
            {
                get; set;
            }
            public List<string> Errors { get; set; } = new List<string>();
        }

        public class DependencyTree
        {
            public List<DependencyTreeNode> Nodes { get; set; } = new List<DependencyTreeNode>();
        }

        public class DependencyTreeNode
        {
            public string Key
            {
                get; set;
            }
            public List<string> Dependencies { get; set; } = new List<string>();
        }
    }
}
