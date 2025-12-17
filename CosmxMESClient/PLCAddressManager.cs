using CosmxMESClient.interfaceConfig;
using CosmxMESClient.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmxMESClient {
    public class PLCAddressManager {
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

        protected virtual void OnDataRead( DataReadEventArgs e ) {
            // 首先触发通用事件
            DataRead?.Invoke(this,e);

            // 然后触发特定地址的处理逻辑
            if (_addressHandlers.TryGetValue(e.Address,out var handler)) {
                try {
                    handler(e.Value);
                    }
                catch (Exception ex) {
                    LoggingService.Error($"地址处理逻辑执行失败: {e.Address}",ex);
                    }
                }
            }
        /// <summary>
        /// 注册地址处理逻辑（核心简化）
        /// </summary>
        public void RegisterAddressHandler( string addressKey,Action<object> handler ) {
            _addressHandlers[addressKey]=handler;
            }

        /// <summary>
        /// 注销地址处理逻辑
        /// </summary>
        public void UnregisterAddressHandler( string addressKey ) {
            _addressHandlers.TryRemove(addressKey,out _);
            }

        // 主程序调用接口
        public bool RegisterPLCConfig( PLCConnectionConfig config ) {
            lock (_lock) {
                if (_plcConfigs.ContainsKey(config.Key)) {
                    LoggingService.Warn($"PLC配置已存在: {config.Key}");
                    return false;
                    }

                _plcConfigs[config.Key]=config;

                // 注册到触发条件管理器
                _triggerManager.RegisterPLC(config);

                LoggingService.Info($"注册PLC配置: {config.Name} (Key: {config.Key})");
                return true;
                }
            }
        // 注册触发依赖关系
        public void RegisterTriggerDependency( string triggerKey,PLCScanAddress dependentAddress ) {
            if (!_triggerDependencies.ContainsKey(triggerKey)) {
                _triggerDependencies[triggerKey]=new List<PLCScanAddress>( );
                }

            if (!_triggerDependencies[triggerKey].Contains(dependentAddress)) {
                _triggerDependencies[triggerKey].Add(dependentAddress);
                dependentAddress.IsTriggerDependent=true;
                LoggingService.Debug($"注册触发依赖: {triggerKey} -> {dependentAddress.Key}");
                }
            }
        // 触发地址条件满足时调用此方法
        public async Task TriggerDependentReadsAsync( string triggerKey,PLCConnectionConfig config ) {
            if (_triggerDependencies.TryGetValue(triggerKey,out var dependents)) {
                foreach (var dependent in dependents.Where(d => d.IsEnabled)) {
                    await ManualReadScanAddressAsync(config,dependent);
                    }
                }
            }
        // 重建所有触发依赖关系
        public void RebuildAllTriggerDependencies( ) {
            _triggerDependencies.Clear( );

            foreach (var config in GlobalVariables.PLCConnections) {
                foreach (var scanAddress in config.ScanAddresses) {
                    // 如果该地址有触发依赖，重新注册
                    if (scanAddress.TriggerPLCScanAddress!=null&&
                        !string.IsNullOrEmpty(scanAddress.TriggerPLCScanAddress.Key)) {
                        RegisterTriggerDependency(scanAddress.TriggerPLCScanAddress.Key,scanAddress);
                        }
                    }
                }

            LoggingService.Info($"已重建触发依赖关系，共 {_triggerDependencies.Count} 个触发关系");
            }
        // 获取所有依赖关系（用于调试和显示）
        public Dictionary<string,List<string>> GetAllDependencies( ) {
            return _triggerDependencies.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Select(addr => addr.Key).ToList( )
            );
            }

        // 检查某个触发地址是否有依赖
        public bool HasDependencies( string triggerKey ) {
            return _triggerDependencies.ContainsKey(triggerKey)&&
                   _triggerDependencies[triggerKey].Count>0;
            }

        // 获取依赖某个触发地址的所有扫描地址
        public List<PLCScanAddress> GetDependentAddresses( string triggerKey ) {
            return _triggerDependencies.TryGetValue(triggerKey,out var dependents)
                ? dependents
                : new List<PLCScanAddress>( );
            }
        /***********************************************************************************************手动读取扫描地址***********************************************************************************************/
        private async Task<bool> ManualReadScanAddressAsync( PLCConnectionConfig config,PLCScanAddress address ) {
            try {
                object value = await ReadAddressByType(config, address);
                if (value!=null) {
                    // 触发数据读取事件
                    OnDataRead(new DataReadEventArgs(
                        address.Address,value,address.DataType,config.PLCInstance.IPEnd));

                    // 检查触发条件
                    address.CheckTriggerCondition(value,address.LastValue);
                    address.LastValue=value;
                    address.LastReadTime=DateTime.Now;

                    return true;
                    }
                }
            catch (Exception ex) {
                LoggingService.Error($"手动读取地址失败: {address.Key}",ex);
                }
            return false;
            }
        private async Task<object> ReadAddressByType( PLCConnectionConfig config,PLCAddressConfig address ) {
            return await Task.Run<object>(( ) =>
            {
                try {
                    switch (address.DataType) {
                        case TypeCode.Boolean:
                            bool boolValue = false;
                            if (config.PLCInstance.ReadRegisterBit(address.Address,ref boolValue))
                                return boolValue;
                            break;
                        case TypeCode.Int16:
                            int intValue = 0;
                            if (config.PLCInstance.ReadRegister(address.Address,ref intValue))
                                return intValue;
                            break;
                        case TypeCode.Int32:
                            int int32Value = 0;
                            if (config.PLCInstance.ReadInt32(address.Address,ref int32Value))
                                return int32Value;
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
                    }
                catch (Exception ex) {
                    LoggingService.Error($"读取地址失败: {address.Address}",ex);
                    }
                return null;
            });
            }
        public bool UnregisterPLCConfig( string plcKey ) {
            lock (_lock) {
                if (_plcConfigs.ContainsKey(plcKey)) {
                    _triggerManager.UnregisterPLC(plcKey);
                    _plcConfigs.Remove(plcKey);
                    LoggingService.Info($"取消注册PLC配置: {plcKey}");
                    return true;
                    }
                return false;
                }
            }

        // 增强的读取方法，支持触发条件检查
        public async Task<object> ReadAddressInternal( PLCConnectionConfig config,PLCScanAddress address,object newValue,TypeCode typeCode ) {
            if (config.PLCInstance==null) {
                throw new InvalidOperationException("PLC未连接");
                }
            return await Task.Run<object>(( ) => {
                try {
                    // 检查触发条件
                    bool shouldRead = CheckTriggerCondition(address, newValue);
                    if (!shouldRead) {
                        return null; // 条件不满足，返回null
                        }
                    // 更新地址状态
                    address.LastValue=newValue;
                    address.LastReadTime=DateTime.Now;
                    // 如果触发了条件，执行清空动作
                    if (address.IsTriggered) {
                        var cc= ExecuteClearActionAsync(config,address,typeCode);
                        }
                    return newValue;
                    }
                catch (Exception ex) {
                    LoggingService.Error($"读取地址失败: {address.Address}",ex);
                    throw;
                    }
            });
            }

        // 增强的触发条件检查
        private bool CheckTriggerCondition( PLCScanAddress address,object newValue ) {
            // 无触发条件或首次读取时直接通过
            if (address.TriggerCondition==TriggerCondition.None&&address.TriggerPLCScanAddress!=null||address.LastValue==null) {
                return true;
                }
            bool triggered = address.CheckTriggerCondition(newValue, address.LastValue);
            return triggered;
            }
        //清空动作
        private async Task<bool> ExecuteClearActionAsync( PLCConnectionConfig config,PLCScanAddress address,TypeCode action ) {
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

            if (address.Address==null||!address.IsEnabled)
                return false;

            return await Task.Run(( ) => {
                try {
                    switch (action) {
                        case TypeCode.Boolean:
                            bool boolValue = false;
                            return config.PLCInstance.WriteRegisterBit(address.Address,boolValue);
                        case TypeCode.Int16:
                            int shortValue = 0;
                            return config.PLCInstance.WriteRegister(address.Address,shortValue);
                        case TypeCode.Int32:
                            int intValue = 0;
                            return config.PLCInstance.WriteInt32(address.Address,intValue);
                        case TypeCode.Single:
                            float floatValue = 0;
                            return config.PLCInstance.WriteFloat(address.Address,floatValue);
                        case TypeCode.Double:
                            double doubleValue =0;
                            return config.PLCInstance.WriteDouble(address.Address,1,doubleValue); // 默认power=1
                        case TypeCode.String:
                            string stringValue = "";
                            return config.PLCInstance.WriteString(address.Address,stringValue);
                        default:
                            return false;
                        }
                    }
                catch (Exception ex) {
                    LoggingService.Error($"执行清空动作失败: {address.Address}",ex);
                    return false;
                    }
            });
            }
        // 触发条件管理器
        public class TriggerConditionManager {
            private Dictionary<string, List<PLCScanAddress>> _plcTriggers = new Dictionary<string, List<PLCScanAddress>>();

            public void RegisterPLC( PLCConnectionConfig config ) {
                if (!_plcTriggers.ContainsKey(config.Key)) {
                    _plcTriggers[config.Key]=new List<PLCScanAddress>( );
                    }

                // 添加有触发条件的地址
                var triggerAddresses = config.ScanAddresses.Where(a => a.TriggerCondition != TriggerCondition.None).ToList();
                _plcTriggers[config.Key].AddRange(triggerAddresses);
                }

            public void UnregisterPLC( string plcKey ) {
                _plcTriggers.Remove(plcKey);
                }

            public List<PLCScanAddress> GetTriggerAddresses( string plcKey ) {
                return _plcTriggers.TryGetValue(plcKey,out var addresses) ? addresses : new List<PLCScanAddress>( );
                }
            }
        }
    }
