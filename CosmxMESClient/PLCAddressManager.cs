using CosmxMESClient.Services;
using System;
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

        // 触发条件管理器
        private TriggerConditionManager _triggerManager = new TriggerConditionManager();

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
            if (address.TriggerCondition==TriggerCondition.None||address.LastValue==null) {
                return true;
                }

            bool triggered = address.CheckTriggerCondition(newValue, address.LastValue);
            return triggered;
            }
        //清空动作
        private async Task<bool> ExecuteClearActionAsync( PLCConnectionConfig config,PLCScanAddress address,TypeCode action ) {
            // 检查PLC实例是否存在，如果不存在则创建
            if (config.PLCInstance==null) {
                LoggingService.Warn($"PLC实例为空，尝试创建实例: {config.Name}");
                config.CreatePLCInstance( );
                }

            // 检查连接状态，如果未连接则尝试连接
            if (!config.IsConnected) {
                LoggingService.Warn($"PLC未连接，尝试连接: {config.Name}");
                bool connected = await Task.Run(() => config.Connect());
                if (!connected) {
                    LoggingService.Error($"无法执行清空动作: PLC连接失败 - {config.Name}");
                    return false;
                    }
                }

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
