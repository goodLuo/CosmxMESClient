using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CosmxMESClient {
    public class PLCAddressConfig:INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _key;
        private string _name;
        private string _address;
        private TypeCode _dataType;
        private string _description;
        private int _readInterval = 1000;
        private bool _isEnabled = true;
        private int _power = 1;
        private int _length = 1;
        public string Key {
            get => _key;
            set {
                _key=value;
                OnPropertyChanged( );
                }
            }

        public string Address {
            get => _address;
            set {
                _address=value;
                OnPropertyChanged( );
                }
            }

        public TypeCode DataType {
            get => _dataType;
            set {
                _dataType=value;
                OnPropertyChanged( );
                }
            }

        public string Description {
            get => _description;
            set {
                _description=value;
                OnPropertyChanged( );
                }
            }

        public int ReadInterval {
            get => _readInterval;
            set {
                _readInterval=value;
                OnPropertyChanged( );
                }
            }

        public bool IsEnabled {
            get => _isEnabled;
            set {
                _isEnabled=value;
                OnPropertyChanged( );
                }
            }

        public int Power {
            get => _power;
            set {
                _power=value;
                OnPropertyChanged( );
                }
            }

        public int Length {
            get => _length;
            set {
                _length=value;
                OnPropertyChanged( );
                }
            }
        protected virtual void OnPropertyChanged( [CallerMemberName] string propertyName = null ) {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName));
            }
        public static string GetDataTypeDisplayName( TypeCode dataType ) {
            switch (dataType) {
                case TypeCode.Boolean:
                    return "布尔";
                case TypeCode.Int16:
                    return "整数(Int16)";
                case TypeCode.Int32:
                    return "整数(Int32)";
                case TypeCode.Single:
                    return "浮点数";
                case TypeCode.Double:
                    return "双精度";
                case TypeCode.String:
                    return "字符串";
                default:
                    return "未知";
                }
            }
        public static string GetTestValueForDataType( TypeCode dataType ) {
            switch (dataType) {
                case TypeCode.Boolean:
                    return "True";
                case TypeCode.Int16:
                    return "100";
                case TypeCode.Int32:
                    return "101";
                case TypeCode.Single:
                    return "123.45";
                case TypeCode.Double:
                    return "678.90";
                case TypeCode.String:
                    return "Test";
                default:
                    return "Default";
                }
            }
        public AddressDirection Direction {
            get; set;
            } // 地址方向
        public string DefaultValue {
            get; set;
            } // 默认值
        public string ValidationRule {
            get; set;
            } // 验证规则
        }

    public enum AddressDirection {
        ReadOnly,    // 只读（扫描地址）
        WriteOnly,   // 只写（发送地址）  
        ReadWrite    // 读写
        }
    public class PLCSendAddress:PLCAddressConfig {
        private string _triggerCondition;
        private bool _autoSend;
        private int _sendDelay;
        private DateTime _lastSendTime;

        public string TriggerCondition {
            get => _triggerCondition;
            set {
                _triggerCondition=value;
                OnPropertyChanged( );
                }
            }

        public bool AutoSend {
            get => _autoSend;
            set {
                _autoSend=value;
                OnPropertyChanged( );
                }
            }

        public int SendDelay {
            get => _sendDelay;
            set {
                _sendDelay=value;
                OnPropertyChanged( );
                }
            }

        public DateTime LastSendTime {
            get => _lastSendTime;
            set {
                _lastSendTime=value;
                OnPropertyChanged( );
                }
            }
        }
    public class PLCTestResult {
        public bool Success {
            get; set;
            }
        public string Value {
            get; set;
            }
        public string ErrorMessage {
            get; set;
            }
        public long ElapsedMilliseconds {
            get; set;
            }
        }
    }
