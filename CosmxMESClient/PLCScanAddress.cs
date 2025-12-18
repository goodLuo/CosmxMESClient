using CosmxMESClient.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CosmxMESClient {
    public class PLCScanAddress:PLCAddressConfig, INotifyPropertyChanged {
        private string _stringThreshold = "";
        private int _consecutiveTriggerCount = 0;
        private int _requiredConsecutiveCount = 1;
        private bool _resetOnSuccess = true;
        private DateTime _lastSuccessfulTriggerTime = DateTime.MinValue;
        private TimeSpan _triggerCooldown = TimeSpan.Zero;

        private DateTime _lastReadTime;
        private object _lastValue;
        private bool _valueChanged;
        private bool _isTriggered;
        private DateTime _lastTriggerTime;
        private List<AddressClearAction> _clearActions = new List<AddressClearAction>();

        // 触发条件配置
        private TriggerCondition _triggerCondition;
        private double _triggerThreshold;
        private double _triggerTolerance = 0.001; // 浮点数比较容差
        private bool _triggerOnRisingEdge;
        private bool _triggerOnFallingEdge;
        private int _triggerDelay;
        private int _triggerCount;
        private int _maxTriggerCount = 0; // 0表示无限制
        private PLCScanAddress _pLCScanAddress;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged( [CallerMemberName] string propertyName = null ) {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName));
            }
        /// <summary>
        /// 字符串阈值（用于字符串比较条件）
        /// </summary>
        public string StringThreshold {
            get => _stringThreshold;
            set {
                _stringThreshold=value;
                OnPropertyChanged( );
                }
            }
        /// <summary>
        /// 连续触发次数
        /// </summary>
        public int ConsecutiveTriggerCount {
            get => _consecutiveTriggerCount;
            set {
                _consecutiveTriggerCount=value;
                OnPropertyChanged( );
                }
            }
        /// <summary>
        /// 需要的连续触发次数
        /// </summary>
        public int RequiredConsecutiveCount {
            get => _requiredConsecutiveCount;
            set {
                _requiredConsecutiveCount=value;
                OnPropertyChanged( );
                }
            }
        /// <summary>
        /// 触发成功后是否重置计数
        /// </summary>
        public bool ResetOnSuccess {
            get => _resetOnSuccess;
            set {
                _resetOnSuccess=value;
                OnPropertyChanged( );
                }
            }
        /// <summary>
        /// 上次成功触发时间
        /// </summary>
        public DateTime LastSuccessfulTriggerTime {
            get => _lastSuccessfulTriggerTime;
            set {
                _lastSuccessfulTriggerTime=value;
                OnPropertyChanged( );
                }
            }
        /// <summary>
        /// 触发冷却时间
        /// </summary>
        public TimeSpan TriggerCooldown {
            get => _triggerCooldown;
            set {
                _triggerCooldown=value;
                OnPropertyChanged( );
                }
            }
        public DateTime LastReadTime {
            get => _lastReadTime;
            set {
                _lastReadTime=value;
                OnPropertyChanged( );
                }
            }
        private bool _isTriggerDependent;

        // 是否依赖触发地址（如果为true，则不自动读取）
        public bool IsTriggerDependent {
            get => _isTriggerDependent;
            set {
                _isTriggerDependent=value;
                OnPropertyChanged( );
                }
            }
        // 检查是否应该自动读取
        public bool ShouldAutoRead => !IsTriggerDependent&&IsEnabled;
        public PLCScanAddress TriggerPLCScanAddress {
            get => _pLCScanAddress;
            set {
                _pLCScanAddress=value;
                OnPropertyChanged( );
                }
            }
        public object LastValue {
            get => _lastValue;
            set {
                var oldValue = _lastValue;
                _lastValue=value;
                OnPropertyChanged( );

                // 检查值变化
                ValueChanged=!Equals(oldValue,value);
                }
            }

        public bool ValueChanged {
            get => _valueChanged;
            set {
                _valueChanged=value;
                OnPropertyChanged( );
                }
            }

        // 触发条件
        public TriggerCondition TriggerCondition {
            get => _triggerCondition;
            set {
                _triggerCondition=value;
                OnPropertyChanged( );
                }
            }

        public double TriggerThreshold {
            get => _triggerThreshold;
            set {
                _triggerThreshold=value;
                OnPropertyChanged( );
                }
            }

        public double TriggerTolerance {
            get => _triggerTolerance;
            set {
                _triggerTolerance=value;
                OnPropertyChanged( );
                }
            }

        public bool TriggerOnRisingEdge {
            get => _triggerOnRisingEdge;
            set {
                _triggerOnRisingEdge=value;
                OnPropertyChanged( );
                }
            }

        public bool TriggerOnFallingEdge {
            get => _triggerOnFallingEdge;
            set {
                _triggerOnFallingEdge=value;
                OnPropertyChanged( );
                }
            }

        public int TriggerDelay {
            get => _triggerDelay;
            set {
                _triggerDelay=value;
                OnPropertyChanged( );
                }
            }

        public int TriggerCount {
            get => _triggerCount;
            set {
                _triggerCount=value;
                OnPropertyChanged( );
                }
            }

        public int MaxTriggerCount {
            get => _maxTriggerCount;
            set {
                _maxTriggerCount=value;
                OnPropertyChanged( );
                }
            }

        public bool IsTriggered {
            get => _isTriggered;
            set {
                _isTriggered=value;
                OnPropertyChanged( );
                }
            }

        public DateTime LastTriggerTime {
            get => _lastTriggerTime;
            set {
                _lastTriggerTime=value;
                OnPropertyChanged( );
                }
            }

        // 清空动作列表
        public List<AddressClearAction> ClearActions {
            get => _clearActions;
            set {
                _clearActions=value;
                OnPropertyChanged( );
                }
            }

        //// 增强的触发条件检查方法
        //public bool CheckTriggerCondition( object newValue,object previousValue ) {
        //    if (!IsEnabled||TriggerCondition==TriggerCondition.None)
        //        return true;

        //    // 检查触发次数限制
        //    if (MaxTriggerCount>0&&TriggerCount>=MaxTriggerCount)
        //        return false;

        //    // 检查触发延迟
        //    if (TriggerDelay>0&&( DateTime.Now-LastTriggerTime ).TotalMilliseconds<TriggerDelay)
        //        return false;

        //    bool triggered = false;

        //    try {
        //        // 根据数据类型调用相应的触发检查
        //        switch (DataType) {
        //            case TypeCode.Boolean:
        //                triggered=CheckBooleanTrigger(newValue,previousValue);
        //                break;
        //            case TypeCode.Int16:
        //                triggered=CheckNumericTrigger(newValue,previousValue);
        //                break;
        //            case TypeCode.Int32:
        //                triggered=CheckNumericTrigger(newValue,previousValue);
        //                break;
        //            case TypeCode.Single:
        //                triggered=CheckNumericTrigger(newValue,previousValue);
        //                break;
        //            case TypeCode.Double:
        //                triggered=CheckNumericTrigger(newValue,previousValue);
        //                break;
        //            case TypeCode.String:
        //                triggered=CheckStringTrigger(newValue,previousValue);
        //                break;
        //            default:
        //                triggered=true; // 未知类型默认通过
        //                break;
        //            }
        //        }
        //    catch (Exception ex) {
        //        LoggingService.Error($"触发条件检查异常: {ex.Message}");
        //        triggered=false;
        //        }

        //    if (triggered) {
        //        TriggerCount++;
        //        LastTriggerTime=DateTime.Now;
        //        IsTriggered=true;

        //        // 记录触发日志
        //        LoggingService.Info($"触发条件满足 - 地址: {Key}, 新值: {newValue}, 旧值: {previousValue}, 条件: {TriggerCondition}");
        //        }

        //    return triggered;
        //    }

        /// <summary>
        /// 增强的触发条件检查方法
        /// </summary>
        public TriggerResult CheckTriggerConditionEnhanced( object newValue,object previousValue ) {
            var result = new TriggerResult { IsTriggered = false };

            if (!IsEnabled||TriggerCondition==TriggerCondition.None) {
                result.IsTriggered=true;
                result.Reason="无条件触发";
                return result;
                }

            // 检查冷却时间
            if (TriggerCooldown>TimeSpan.Zero) {
                var timeSinceLastTrigger = DateTime.Now - LastSuccessfulTriggerTime;
                if (timeSinceLastTrigger<TriggerCooldown) {
                    result.Reason=$"冷却时间内，剩余：{( TriggerCooldown-timeSinceLastTrigger ).TotalSeconds:F1}秒";
                    return result;
                    }
                }

            // 检查触发次数限制
            if (MaxTriggerCount>0&&TriggerCount>=MaxTriggerCount) {
                result.Reason=$"已达到最大触发次数限制：{MaxTriggerCount}";
                return result;
                }

            // 检查触发延迟
            if (TriggerDelay>0&&( DateTime.Now-LastTriggerTime ).TotalMilliseconds<TriggerDelay) {
                result.Reason=$"触发延迟中，剩余：{TriggerDelay-( DateTime.Now-LastTriggerTime ).TotalMilliseconds:F0}ms";
                return result;
                }

            // 检查触发条件
            bool conditionMet = CheckTriggerConditionByType(newValue, previousValue, out string conditionReason);

            if (conditionMet) {
                ConsecutiveTriggerCount++;
                result.ConsecutiveCount=ConsecutiveTriggerCount;
                result.ConditionReason=conditionReason;

                // 检查是否需要连续触发
                if (ConsecutiveTriggerCount>=RequiredConsecutiveCount) {
                    result.IsTriggered=ExecuteTriggerActions(newValue,previousValue);
                    result.Reason=result.IsTriggered ? "触发条件满足" : "触发动作执行失败";
                    }
                else {
                    result.Reason=$"连续触发计数：{ConsecutiveTriggerCount}/{RequiredConsecutiveCount}";
                    }
                }
            else {
                ConsecutiveTriggerCount=0; // 重置连续计数
                result.Reason="触发条件未满足";
                }

            return result;
            }

        /// <summary>
        /// 按类型检查触发条件
        /// </summary>
        private bool CheckTriggerConditionByType( object newValue,object previousValue,out string reason ) {
            reason="";

            try {
                switch (DataType) {
                    case TypeCode.Boolean:
                        return CheckBooleanTrigger(newValue,previousValue,out reason);
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Single:
                    case TypeCode.Double:
                        return CheckNumericTrigger(newValue,previousValue,out reason);
                    case TypeCode.String:
                        return CheckStringTrigger(newValue,previousValue,out reason);
                    default:
                        reason="未知数据类型，默认通过";
                        return true;
                    }
                }
            catch (Exception ex) {
                reason=$"触发条件检查异常：{ex.Message}";
                LoggingService.Error($"触发条件检查异常：{Key}",ex);
                return false;
                }
            }


        /// <summary>
        /// 布尔类型触发检查
        /// </summary>
        private bool CheckBooleanTrigger( object newValue,object previousValue,out string reason ) {
            bool newBool = SafeConvert.ToBoolean(newValue);
            bool prevBool = SafeConvert.ToBoolean(previousValue);

            switch (TriggerCondition) {
                case TriggerCondition.RisingEdge:
                    reason=$"上升沿触发：{prevBool} -> {newBool}";
                    return TriggerOnRisingEdge&&!prevBool&&newBool;

                case TriggerCondition.FallingEdge:
                    reason=$"下降沿触发：{prevBool} -> {newBool}";
                    return TriggerOnFallingEdge&&prevBool&&!newBool;

                case TriggerCondition.Equal:
                    bool targetBool = SafeConvert.ToBoolean(TriggerThreshold);
                    reason=$"等于检查：{newBool} == {targetBool}";
                    return newBool==targetBool;

                case TriggerCondition.NotEqual:
                    targetBool=SafeConvert.ToBoolean(TriggerThreshold);
                    reason=$"不等于检查：{newBool} != {targetBool}";
                    return newBool!=targetBool;

                default:
                    reason=$"布尔类型不支持条件：{TriggerCondition}";
                    return false;
                }
            }

        /// <summary>
        /// 数值类型触发检查
        /// </summary>
        private bool CheckNumericTrigger( object newValue,object previousValue,out string reason ) {
            double newNum = SafeConvert.ToDouble(newValue);
            double prevNum = SafeConvert.ToDouble(previousValue);
            double threshold = TriggerThreshold;

            switch (TriggerCondition) {
                case TriggerCondition.GreaterThan:
                    reason=$"{newNum} > {threshold}";
                    return newNum>threshold;

                case TriggerCondition.LessThan:
                    reason=$"{newNum} < {threshold}";
                    return newNum<threshold;

                case TriggerCondition.Equal:
                    reason=$"{newNum} == {threshold} (容差：{TriggerTolerance})";
                    return Math.Abs(newNum-threshold)<=TriggerTolerance;

                case TriggerCondition.GreaterThanOrEqual:
                    reason=$"{newNum} >= {threshold}";
                    return newNum>=threshold;

                case TriggerCondition.LessThanOrEqual:
                    reason=$"{newNum} <= {threshold}";
                    return newNum<=threshold;

                case TriggerCondition.NotEqual:
                    reason=$"{newNum} != {threshold}";
                    return Math.Abs(newNum-threshold)>TriggerTolerance;

                case TriggerCondition.RisingEdge:
                    reason=$"数值上升沿：{prevNum} -> {newNum}";
                    return TriggerOnRisingEdge&&newNum>prevNum;

                case TriggerCondition.FallingEdge:
                    reason=$"数值下降沿：{prevNum} -> {newNum}";
                    return TriggerOnFallingEdge&&newNum<prevNum;

                case TriggerCondition.ChangePercentage:
                    if (prevNum==0) {
                        reason=$"零值变化百分比：{newNum}";
                        return newNum!=0;
                        }
                    double changePercent = Math.Abs((newNum - prevNum) / prevNum * 100);
                    reason=$"变化百分比：{changePercent:F2}% >= {threshold}%";
                    return changePercent>=threshold;

                default:
                    reason=$"数值类型不支持条件：{TriggerCondition}";
                    return false;
                }
            }
        /// <summary>
        /// 字符串长度触发检查
        /// </summary>
        private bool CheckStringLengthTrigger( string newStr,string prevStr,out string reason ) {
            int newLength = newStr.Length;
            int prevLength = prevStr.Length;
            double threshold = TriggerThreshold;

            switch (TriggerCondition) {
                case TriggerCondition.GreaterThan:
                    reason=$"长度大于：{newLength} > {threshold}";
                    return newLength>threshold;
                case TriggerCondition.LessThan:
                    reason=$"长度小于：{newLength} < {threshold}";
                    return newLength<threshold;
                case TriggerCondition.GreaterThanOrEqual:
                    reason=$"长度大于等于：{newLength} >= {threshold}";
                    return newLength>=threshold;
                case TriggerCondition.LessThanOrEqual:
                    reason=$"长度小于等于：{newLength} <= {threshold}";
                    return newLength<=threshold;
                default:
                    reason="无效的长度比较条件";
                    return false;
                }
            }
        /// <summary>
        /// 执行触发动作
        /// </summary>
        private bool ExecuteTriggerActions( object newValue,object previousValue ) {
            TriggerCount++;
            LastTriggerTime=DateTime.Now;
            LastSuccessfulTriggerTime=DateTime.Now;
            IsTriggered=true;

            if (ResetOnSuccess) {
                ConsecutiveTriggerCount=0; // 重置连续计数
                }

            // 记录触发日志
            LoggingService.Info($"触发条件满足 - 地址: {Key}, 新值: {newValue}, 旧值: {previousValue}, "+
                               $"条件: {TriggerCondition}, 触发次数: {TriggerCount}");

            // 异步执行清空动作（不阻塞主线程）
            if (ClearActions.Count>0) {
                Task.Run(async ( ) =>
                {
                    try {
                        // 在实际实现中，需要通过某种方式获取PLCConnectionConfig
                        // await ExecuteClearActionsAsync(plcConfig);
                        LoggingService.Info($"触发清空动作 - 地址: {Key}, 动作数量: {ClearActions.Count}");
                        }
                    catch (Exception ex) {
                        LoggingService.Error($"执行清空动作失败: {Key}",ex);
                        }
                });
                }

            return true;
            }
        /// <summary>
        /// 字符串类型触发检查
        /// </summary>
        private bool CheckStringTrigger( object newValue,object previousValue,out string reason ) {
            string newStr = newValue?.ToString() ?? "";
            string prevStr = previousValue?.ToString() ?? "";
            string thresholdStr = StringThreshold;

            switch (TriggerCondition) {
                case TriggerCondition.Equal:
                    reason=$"字符串相等：'{newStr}' == '{prevStr}'";
                    return newStr.Equals(prevStr,StringComparison.OrdinalIgnoreCase);

                case TriggerCondition.NotEqual:
                    reason=$"字符串不相等：'{newStr}' != '{prevStr}'";
                    return !newStr.Equals(prevStr,StringComparison.OrdinalIgnoreCase);

                case TriggerCondition.Contains:
                    reason=$"包含检查：'{newStr}' 包含 '{thresholdStr}'";
                    return newStr.IndexOf(thresholdStr,StringComparison.OrdinalIgnoreCase)>=0;

                case TriggerCondition.StartsWith:
                    reason=$"开头检查：'{newStr}' 以 '{thresholdStr}' 开头";
                    return newStr.StartsWith(thresholdStr,StringComparison.OrdinalIgnoreCase);

                case TriggerCondition.EndsWith:
                    reason=$"结尾检查：'{newStr}' 以 '{thresholdStr}' 结尾";
                    return newStr.EndsWith(thresholdStr,StringComparison.OrdinalIgnoreCase);

                case TriggerCondition.GreaterThan:
                case TriggerCondition.LessThan:
                case TriggerCondition.GreaterThanOrEqual:
                case TriggerCondition.LessThanOrEqual:
                    return CheckStringLengthTrigger(newStr,prevStr,out reason);

                default:
                    reason=$"字符串类型不支持条件：{TriggerCondition}";
                    return false;
                }
            }

        /// <summary>
        /// 重置触发状态
        /// </summary>
        public void ResetTrigger( ) {
            TriggerCount=0;
            ConsecutiveTriggerCount=0;
            IsTriggered=false;
            LastTriggerTime=DateTime.MinValue;
            LastSuccessfulTriggerTime=DateTime.MinValue;
            }
        /// <summary>
        /// 获取触发条件状态信息
        /// </summary>
        public string GetTriggerStatusInfo( ) {
            return $"触发条件: {TriggerCondition}, 触发次数: {TriggerCount}/{MaxTriggerCount}, "+
                   $"连续计数: {ConsecutiveTriggerCount}/{RequiredConsecutiveCount}, "+
                   $"最后触发: {LastTriggerTime:yyyy-MM-dd HH:mm:ss}";
            }
        /// <summary>
        /// 触发结果类
        /// </summary>
        public class TriggerResult {
            public bool IsTriggered {
                get; set;
                }
            public string Reason {
                get; set;
                }
            public string ConditionReason {
                get; set;
                }
            public int ConsecutiveCount {
                get; set;
                }
            }
        /// <summary>
        /// 安全类型转换类
        /// </summary>
        public static class SafeConvert {
            public static bool ToBoolean( object value ) {
                if (value==null)
                    return false;
                if (value is bool)
                    return (bool) value;
                if (value is string str)
                    return bool.TryParse(str,out bool result)&&result;
                return Convert.ToBoolean(value);
                }

            public static double ToDouble( object value ) {
                if (value==null)
                    return 0;
                if (value is double)
                    return (double) value;
                if (value is string str)
                    return double.TryParse(str,out double result) ? result : 0;
                return Convert.ToDouble(value);
                }
            }
        // 添加清空动作
        public void AddClearAction( string address,object clearValue ) {
            ClearActions.Add(new AddressClearAction
                {
                Address=address,
                ClearValue=clearValue,
                DataType=DataType
                });
            }

        // 执行清空动作
        public async Task<bool> ExecuteClearActionsAsync( PLCConnectionConfig config ) {
            try {
                foreach (var action in ClearActions) {
                    await ExecuteClearActionAsync(config,action);
                    }
                return true;
                }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"执行清空动作失败: {ex.Message}");
                return false;
                }
            }

        private async Task<bool> ExecuteClearActionAsync( PLCConnectionConfig config,AddressClearAction action ) {
            if (config.PLCInstance==null||!config.PLCInstance.IsConnected)
                return false;

            return await Task.Run(( ) => {
                try {
                    switch (action.DataType) {
                        case TypeCode.Boolean:
                            bool boolValue = Convert.ToBoolean(action.ClearValue);
                            return config.PLCInstance.WriteCoil(action.Address,boolValue);
                        case TypeCode.Int16:
                            int intValue = Convert.ToInt32(action.ClearValue);
                            return config.PLCInstance.WriteRegister(action.Address,intValue);
                        case TypeCode.Int32:
                            int int32Value = Convert.ToInt32(action.ClearValue);
                            return config.PLCInstance.WriteInt32(action.Address,int32Value);
                        case TypeCode.Single:
                            float floatValue = Convert.ToSingle(action.ClearValue);
                            return config.PLCInstance.WriteFloat(action.Address,floatValue);
                        case TypeCode.Double:
                            double doubleValue = Convert.ToDouble(action.ClearValue);
                            return config.PLCInstance.WriteDouble(action.Address,doubleValue);
                        case TypeCode.String:
                            string stringValue = action.ClearValue?.ToString() ?? "";
                            return config.PLCInstance.WriteString(action.Address,stringValue);
                        default:
                            return false;
                        }
                    }
                catch {
                    return false;
                    }
            });
            }


        }

    // 清空动作类
    public class AddressClearAction {
        public string Address {
            get; set;
            }
        public object ClearValue {
            get; set;
            }
        public TypeCode DataType {
            get; set;
            }
        public string Description {
            get; set;
            }
        }

    // 增强的触发条件枚举
    public enum TriggerCondition {
        None,                   // 无触发条件
        GreaterThan,           // 大于
        LessThan,              // 小于
        Equal,                 // 等于
        GreaterThanOrEqual,    // 大于等于
        LessThanOrEqual,       // 小于等于
        NotEqual,              // 不等于
        RisingEdge,            // 上升沿
        FallingEdge,           // 下降沿
        ChangePercentage,      // 变化百分比
        Contains,              // 包含（字符串）
        StartsWith,            // 以...开始（字符串）
        EndsWith               // 以...结束（字符串）
        }
    }