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

        // 主程序调用接口
        public bool RegisterPLCConfig( PLCConnectionConfig config ) {
            lock (_lock) {
                if (_plcConfigs.ContainsKey(config.Key)) {
                    LoggingService.Warn($"PLC配置已存在: {config.Key}");
                    return false;
                    }

                _plcConfigs[config.Key]=config;
                LoggingService.Info($"注册PLC配置: {config.Name} (Key: {config.Key})");
                return true;
                }
            }

        // 添加方法：根据地址键值读取数据
        public async Task<object> ReadAddressAsync( string plcKey,string addressKey ) {
            if (!_plcConfigs.TryGetValue(plcKey,out var config)) {
                throw new ArgumentException($"未找到PLC配置: {plcKey}");
                }

            // 查找地址配置
            var address = config.ScanAddresses.FirstOrDefault(a => a.Key == addressKey);
            if (address==null) {
                throw new ArgumentException($"未找到地址配置: {addressKey}");
                }

            return await ReadAddressInternal(config,address);
            }

        // 添加方法：写入数据到指定地址
        public async Task<bool> WriteAddressAsync( string plcKey,string addressKey,object value ) {
            if (!_plcConfigs.TryGetValue(plcKey,out var config)) {
                throw new ArgumentException($"未找到PLC配置: {plcKey}");
                }

            // 查找地址配置
            var address = config.SendAddresses.FirstOrDefault(a => a.Key == addressKey);
            if (address==null) {
                throw new ArgumentException($"未找到地址配置: {addressKey}");
                }

            return await WriteAddressInternal(config,address,value);
            }

        //// 获取所有地址键值
        //public List<string> GetAllAddressKeys( string plcKey ) {
        //    if (!_plcConfigs.TryGetValue(plcKey,out var config)) {
        //        return new List<string>( );
        //        }

        //    var keys = new List<string>();
        //    keys.AddRange(config.ScanAddresses.Keys);
        //    keys.AddRange(config.SendAddresses.Keys);

        //    return keys;
        //    }

        private async Task<object> ReadAddressInternal( PLCConnectionConfig config,PLCScanAddress address ) {
            if (config.PLCInstance==null||!config.PLCInstance.IsConnected)
                throw new InvalidOperationException("PLC未连接");

            return await Task.Run<object>(( ) =>
            {
                object newValue = ReadValueByDataType(config, address);

                // 检查触发条件
                if (!CheckTriggerCondition(address,newValue)) {
                    return null; // 条件不满足，返回null
                    }

                address.LastValue=newValue;
                address.IsTriggered=true;

                return newValue;
            });
            }
        private bool CheckTriggerCondition( PLCScanAddress address,object newValue ) {
            // 无触发条件或首次读取时直接通过
            if (address.TriggerCondition==TriggerCondition.None||address.LastValue==null)
                return true;

            return address.CheckTriggerCondition(newValue,address.LastValue);
            }

        private object ReadValueByDataType( PLCConnectionConfig config,PLCScanAddress address ) {
            switch (address.DataType.Name) {
                case nameof(Boolean):
                    bool boolValue = false;
                    if (config.PLCInstance.ReadCoil(address.Address,ref boolValue))
                        return boolValue;
                    break;

                case nameof(Int32):
                    int intValue = 0;
                    if (config.PLCInstance.ReadRegister(address.Address,ref intValue))
                        return intValue;
                    break;

                case nameof(Single):
                    float floatValue = 0;
                    if (config.PLCInstance.ReadFloat(address.Address,ref floatValue))
                        return floatValue;
                    break;

                case nameof(Double):
                    double doubleValue = 0;
                    if (config.PLCInstance.ReadDouble(address.Address,address.Power,ref doubleValue))
                        return doubleValue;
                    break;

                case nameof(String):
                    string stringValue = "";
                    if (config.PLCInstance.ReadString(address.Address,ref stringValue,address.Length))
                        return stringValue;
                    break;
                }
            throw new Exception($"读取地址失败: {address.Address}");
            }
        private async Task<bool> WriteAddressInternal( PLCConnectionConfig config,PLCSendAddress address,object value ) {
            if (config.PLCInstance==null||!config.PLCInstance.IsConnected) {
                throw new InvalidOperationException("PLC未连接");
                }

            // 数据验证
            if (!ValidateValue(address,value)) {
                throw new ArgumentException($"值验证失败: {value}");
                }
            return await Task.Run<bool>(( ) => {
                // 根据数据类型调用相应的写入方法
                switch (address.DataType.Name) {
                    case nameof(Boolean):
                        if (value is bool boolValue)
                            return config.PLCInstance.WriteCoil(address.Address,boolValue);
                        break;

                    case nameof(Int32):
                        if (value is int intValue)
                            return config.PLCInstance.WriteRegister(address.Address,intValue);
                        break;

                    case nameof(Single):
                        if (value is float floatValue)
                            return config.PLCInstance.WriteFloat(address.Address,floatValue);
                        break;

                    case nameof(Double):
                        if (value is double doubleValue)
                            return config.PLCInstance.WriteDouble(address.Address,address.Power,doubleValue);
                        break;

                    case nameof(String):
                        if (value is string stringValue)
                            return config.PLCInstance.WriteString(address.Address,stringValue);
                        break;
                    }

                throw new Exception($"写入地址失败: {address.Address}");
            });
            }

        private bool ValidateValue( PLCSendAddress address,object value ) {
            // 这里可以添加更复杂的验证逻辑
            if (value==null)
                return false;

            // 基本类型验证
            try {
                Convert.ChangeType(value,address.DataType);
                return true;
                }
            catch {
                return false;
                }
            }
        }
    }
