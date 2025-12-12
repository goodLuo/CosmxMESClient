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
        private Type _dataType;
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

        public string Name {
            get => _name;
            set {
                _name=value;
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

        public Type DataType {
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
    // 新增触发条件枚举
    public enum TriggerCondition {
        None,               // 无触发条件
        ThresholdAbove,     // 阈值以上触发
        ThresholdBelow,     // 阈值以下触发
        ChangePercentage,   // 变化百分比触发
        RisingEdge,         // 上升沿触发
        FallingEdge         // 下降沿触发
        }
    public class PLCScanAddress:PLCAddressConfig {
        private DateTime _lastReadTime;
        private object _lastValue;
        private bool _valueChanged;

        // 触发条件配置
        private TriggerCondition _triggerCondition;
        private double _triggerThreshold;
        private bool _triggerOnRisingEdge;
        private bool _triggerOnFallingEdge;
        private int _triggerDelay;
        private bool _isTriggered;

        public DateTime LastReadTime {
            get => _lastReadTime;
            set {
                _lastReadTime=value;
                OnPropertyChanged( );
                }
            }

        public object LastValue {
            get => _lastValue;
            set {
                _lastValue=value;
                OnPropertyChanged( );
                }
            }

        public bool ValueChanged {
            get => _valueChanged;
            set {
                _valueChanged=value;
                OnPropertyChanged( );
                }
            }

        // 触发条件枚举
        public TriggerCondition TriggerCondition {
            get => _triggerCondition;
            set {
                _triggerCondition=value;
                OnPropertyChanged( );
                }
            }

        // 触发阈值
        public double TriggerThreshold {
            get => _triggerThreshold;
            set {
                _triggerThreshold=value;
                OnPropertyChanged( );
                }
            }

        // 上升沿触发
        public bool TriggerOnRisingEdge {
            get => _triggerOnRisingEdge;
            set {
                _triggerOnRisingEdge=value;
                OnPropertyChanged( );
                }
            }

        // 下降沿触发
        public bool TriggerOnFallingEdge {
            get => _triggerOnFallingEdge;
            set {
                _triggerOnFallingEdge=value;
                OnPropertyChanged( );
                }
            }

        // 触发延迟(ms)
        public int TriggerDelay {
            get => _triggerDelay;
            set {
                _triggerDelay=value;
                OnPropertyChanged( );
                }
            }

        // 当前是否已触发
        public bool IsTriggered {
            get => _isTriggered;
            set {
                _isTriggered=value;
                OnPropertyChanged( );
                }
            }

        // 触发条件验证方法
        public bool CheckTriggerCondition( object newValue,object previousValue ) {
            if (!IsEnabled||TriggerCondition==TriggerCondition.None)
                return true;

            try {
                double newVal = Convert.ToDouble(newValue);
                double prevVal = Convert.ToDouble(previousValue);

                switch (TriggerCondition) {
                    case TriggerCondition.ThresholdAbove:
                        return newVal>TriggerThreshold;

                    case TriggerCondition.ThresholdBelow:
                        return newVal<TriggerThreshold;

                    case TriggerCondition.ChangePercentage:
                        if (prevVal==0)
                            return true;
                        double changePercent = Math.Abs((newVal - prevVal) / prevVal * 100);
                        return changePercent>=TriggerThreshold;

                    case TriggerCondition.RisingEdge:
                        return TriggerOnRisingEdge&&newVal>prevVal;

                    case TriggerCondition.FallingEdge:
                        return TriggerOnFallingEdge&&newVal<prevVal;

                    default:
                        return true;
                    }
                }
            catch {
                return true;
                }
            }
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
