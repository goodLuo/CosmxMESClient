using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CosmxMESClient.interfaceConfig {
    public class HeartbeatStatus {
        public bool IsEnabled {
            get; set;
            }
        public int Interval {
            get; set;
            }
        public int LastRetryCount {
            get; set;
            }
        public bool IsConnectionAlive {
            get; set;
            }
        public DateTime LastCheckTime { get; set; } = DateTime.Now;
        }
    // 自动读取配置类
    public class AutoReadConfig {
        public string Key {
            get; set;
            } // 配置的唯一标识
        public List<string> Addresses { get; set; } = new List<string>( ); // 支持单个或多个地址
        public TypeCode DataType {
            get; set;
            }
        /// <summary>
        /// // 用于double类型
        /// </summary>
        public int Power { get; set; } = 1;
        /// <summary>
        /// 用于数组类型
        /// </summary>
        public int Length { get; set; } = 1;
        /// <summary>
        /// 用于字符串类型
        /// </summary>
        public int MaxLength { get; set; } = 20;
        public DateTime LastReadTime {
            get; set;
            }
        public int ReadIntervalMs { get; set; } = 1000; // 默认1秒读取一次
        public bool IsBulkRead { get; set; } = true; // 是否使用批量读取
        }
    public enum ByteOrderEnum {
        BigEndian,           // 大端序 (ABCD)
        LittleEndian,        // 小端序 (DCBA)
        BigEndianByteSwap,   // 大端序字节交换 (BADC)
        LittleEndianByteSwap // 小端序字节交换 (CDAB)
        }
    /// 字符串端序配置
    public enum StringByteOrderEnum {
        Normal,           // 正常顺序 ABCD -> ABCD
        RegisterByteSwap, // 寄存器内字节交换 ABCD -> BADC
        WordSwap,         // 寄存器间交换 ABCD -> CDAB
        FullSwap          // 完全交换 ABCD -> DCBA
        }
    // 事件参数类，用于传递读取的数据
    public class DataReadEventArgs:EventArgs {
        public string Address {
            get; set;
            }
        public object Value {
            get; set;
            }
        public TypeCode ValueType {
            get; set;
            }
        public bool IsArray {
            get; set;
            }
        public int ArrayLength {
            get; set;
            }
        public IPEndPoint LocalEndPoint {
            get; set;
            }


        public DataReadEventArgs( string address,object value,TypeCode? valueType,IPEndPoint localEndPoint,bool isArray = false,int arrayLength = 0 ) {
            Address=address;
            Value=value;
            ValueType=(TypeCode) valueType ;
            IsArray=isArray;
            ArrayLength=arrayLength;
            LocalEndPoint=localEndPoint;
            }
        }
    // 写入操作完成事件参数
    public class DataWriteEventArgs:EventArgs {
        public string OperationType {
            get; set;
            }
        public List<string> Addresses {
            get; set;
            }
        public object Values {
            get; set;
            }
        public bool Success {
            get; set;
            }
        public string ErrorMessage {
            get; set;
            }
        public DateTime Timestamp {
            get; set;
            }

        public DataWriteEventArgs( string operationType,List<string> addresses,object values,bool success,string errorMessage = null ) {
            OperationType=operationType;
            Addresses=addresses;
            Values=values;
            Success=success;
            ErrorMessage=errorMessage;
            Timestamp=DateTime.Now;
            }
        }
    public class HeartbeatEventArgs:EventArgs {
        public bool IsAlive {
            get; set;
            }
        public DateTime Timestamp {
            get; set;
            }
        public string Message {
            get; set;
            }
        public int RetryCount {
            get; set;
            }

        public HeartbeatEventArgs( bool isAlive,string message,int retryCount ) {
            IsAlive=isAlive;
            Timestamp=DateTime.Now;
            Message=message;
            RetryCount=retryCount;
            }
        }
    /// <summary>
    /// PLC通信通用接口
    /// 定义不同品牌PLC（三菱、台达等）应实现的通信方法
    /// </summary>
    public interface IPlcCommunication:IDisposable {

        #region 事件
        event EventHandler<DataReadEventArgs> DataRead;
        event EventHandler<DataWriteEventArgs> DataWriteCompleted;
        // 心跳状态事件
        event EventHandler<HeartbeatEventArgs> HeartbeatStatusChanged;
        event Action<string> WriteLog;
        #endregion
        #region 连接管理
        /// <summary>
        /// 初始化PLC连接
        /// </summary>
        /// <returns>成功返回true，失败返回false</returns>
        bool Initialize( );

        /// <summary>
        /// 关闭连接
        /// </summary>
        void Close( );

        /// <summary>
        /// 获取连接状态
        /// </summary>
        bool IsConnected {
            get;
            }
        #region 心跳控制方法
        void StartHeartbeatService( );
        void StopHeartbeatService( );
        void SetHeartbeatAddress( string Address );
        void SetHeartbeatInterval( int intervalMs );
        HeartbeatStatus GetHeartbeatStatus( );
        #endregion
        #endregion
        StringByteOrderEnum StringByteOrder {
            get; set;
            }
        #region 基础数据读写
        /// <summary>
        /// 读取单个寄存器值（16位整型）
        /// </summary>
        /// <param name="address">寄存器地址（如"D100"）</param>
        /// <param name="value">读取到的值</param>
        /// <returns>成功返回true，失败返回false</returns>
        bool ReadRegister( string address,ref int value );
        bool WriteRegister( string address,int value );
        bool ReadInt32( string address,ref int value );
        bool WriteInt32( string address,int value );
        /// <summary>
        /// float类型读取
        /// </summary>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool ReadFloat( string address,ref float value );
        /// <summary>
        /// float类型写入
        /// </summary>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool WriteFloat( string address,float value );

        /// <summary>
        /// 读取32位数据（通常占用2个寄存器）
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="power">缩放系数（如10000表示精度为0.0001）</param>
        /// <param name="value">读取到的值</param>
        /// <returns>成功返回true，失败返回false</returns>
        bool ReadDouble( string address,int power,ref double value );

        /// <summary>
        /// 写入32位数据（通常占用2个寄存器）
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="power">缩放系数</param>
        /// <param name="value">要写入的值</param>
        /// <returns>成功返回true，失败返回false</returns>
        bool WriteDouble( string address,int power,double value );
        /// <summary>
        /// 位读
        /// </summary>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool ReadRegisterBit( string address,ref bool value );
        /// <summary>
        /// 位写
        /// </summary>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool WriteRegisterBit( string address,bool value );
        #endregion

        #region 批量数据操作
        /// <summary>
        /// 读取连续地址INT
        /// </summary>
        /// <param name="address"></param>
        /// <param name="length"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        bool ReadContinuousInt( string address,int length,ref int[] values );
        bool BatchWriteFloat( List<string> addresses,float[] values );
        bool BatchReadFloat( List<string> addresses,ref float[] values );
        bool BatchReadInt32( List<string> addresses,ref int[] values );
        bool BatchWriteInt32( List<string> addresses,int[] values );
        /// <summary>
        /// 批量读取多个int值
        /// </summary>
        bool BatchReadInt( List<string> addresses,ref int[] values );

        /// <summary>
        /// 批量写入多个int值
        /// </summary>
        bool BatchWriteInt( List<string> addresses,int[] values );

        /// <summary>
        /// 批量读取多个double值
        /// </summary>
        bool BatchReadDouble( List<string> addresses,int power,ref double[] values );

        /// <summary>
        /// 批量写入多个double值
        /// </summary>
        bool BatchWriteDouble( List<string> addresses,int power,double[] values );

        /// <summary>
        /// 批量读取多个字符串值
        /// </summary>
        bool BatchReadString( List<string> addresses,int maxStringLength,ref string[] texts );

        /// <summary>
        /// 批量写入多个字符串值
        /// </summary>
        bool BatchWriteString( List<string> addresses,string[] texts,int maxStringLength );
        /// <summary>
        /// 批量读位
        /// </summary>
        /// <param name="addresses"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        bool BatchReadRegisterBits( List<string> addresses,ref bool[] values );
        /// <summary>
        /// 批量写位
        /// </summary>
        /// <param name="addresses"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        bool BatchWriteRegisterBits( List<string> addresses,bool[] values );
        #endregion

        #region 自动读取操作
        /// <summary>
        /// 添加或更新自动读取配置（单个地址）
        /// 如果指定的键已存在，则更新对应的配置；如果不存在，则创建新的自动读取配置
        /// </summary>
        /// <param name="key">自动读取配置的唯一标识符，用于后续管理和引用</param>
        /// <param name="address">要读取的PLC设备地址，例如："D100"、"M200"等</param>
        /// <param name="dataType">要读取的数据类型，例如：typeof(int)、typeof(float)等</param>
        /// <param name="readIntervalMs">读取间隔时间（毫秒），默认值为1000毫秒（1秒）</param>
        /// <param name="power">数据转换的幂次方系数，用于数据缩放，默认值为1（不缩放）</param>
        /// <param name="length">要读取的数据长度（对于数组类型），默认值为1（单个值）</param>
        /// <param name="maxLength">最大数据长度限制，默认值为10</param>
        /// <param name="isBulkRead">是否启用批量读取模式，启用后可优化多个地址的读取性能，默认值为true</param>
        /// <returns>如果成功添加或更新配置返回true，否则返回false</returns>
        bool AddOrUpdateAutoRead( string key,string address,TypeCode dataType,int readIntervalMs = 1000,
                                 int power = 1,int length = 1,int maxLength = 20,bool isBulkRead = true );
        /// <summary>
        /// 添加或更新自动读取配置（地址列表）
        /// 如果指定的键已存在，则更新对应的配置；如果不存在，则创建新的自动读取配置
        /// 此重载方法允许一次性配置多个地址的自动读取
        /// </summary>
        /// <param name="key">自动读取配置的唯一标识符，用于后续管理和引用</param>
        /// <param name="addresses">要读取的PLC设备地址列表，例如：["D100", "D101", "M200"]</param>
        /// <param name="dataType">要读取的数据类型，例如：typeof(int)、typeof(float)等</param>
        /// <param name="readIntervalMs">读取间隔时间（毫秒），默认值为1000毫秒（1秒）</param>
        /// <param name="power">数据转换的幂次方系数，用于数据缩放，默认值为1（不缩放）</param>
        /// <param name="length">要读取的数据长度（对于数组类型），默认值为1（单个值）</param>
        /// <param name="maxLength">批最大数据长度限制，默认值为10</param>
        /// <param name="isBulkRead">是否启用批量读取模式，启用后可优化多个地址的读取性能，默认值为true</param>
        /// <returns>如果成功添加或更新配置返回true，否则返回false</returns>
        bool AddOrUpdateAutoRead( string key,List<string> addresses,TypeCode dataType,int readIntervalMs = 1000,
                                 int power = 1,int length = 1,int maxLength = 10,bool isBulkRead = true );
        /// <summary>
        /// 移除自动读取配置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool RemoveAutoRead( string key );

        bool RemoveAutoReadAll( );

        /// <summary>
        /// 获取所有自动读取配置的键
        /// </summary>
        /// <returns></returns>
        List<string> GetAutoReadKeys( );

        /// <summary>
        /// 获取特定配置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        AutoReadConfig GetAutoReadConfig( string key );
        /// <summary>
        /// 启动自动读取
        /// </summary>
        void StartAutoRead( );

        /// <summary>
        /// 停止自动读取
        /// </summary>
        void StopAutoRead( );

        /// <summary>
        /// 立即执行一次自动读取（手动触发）
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool ExecuteAutoReadNow( string key );
        #endregion

        #region 位操作
        /// <summary>
        /// 读取单个位（线圈）状态
        /// </summary>
        /// <param name="address">位地址（如"Y0"）</param>
        /// <param name="value">读取到的状态</param>
        /// <returns>成功返回true，失败返回false</returns>
        bool ReadCoil( string address,ref bool value );

        /// <summary>
        /// 写入单个位（线圈）状态
        /// </summary>
        /// <param name="address">位地址</param>
        /// <param name="value">要写入的状态</param>
        /// <returns>成功返回true，失败返回false</returns>
        bool WriteCoil( string address,bool value );

        /// <summary>
        /// 读取多个位（线圈）状态
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">要读取的数量</param>
        /// <param name="values">读取到的状态数组</param>
        /// <returns>成功返回true，失败返回false</returns>
        bool ReadCoils( string address,int length,ref bool[] values );

        /// <summary>
        /// 写入多个位（线圈）状态
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="values">要写入的状态数组</param>
        /// <returns>成功返回true，失败返回false</returns>
        bool WriteCoils( string address,bool[] values );
        #endregion

        #region 字符串操作
        /// <summary>
        /// 读取字符串（从多个寄存器）
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="text">读取到的字符串</param>
        /// <param name="maxLength">最大长度（字符数）</param>
        /// <returns>成功返回true，失败返回false</returns>
        bool ReadString( string address,ref string text,int maxLength = 20 );

        /// <summary>
        /// 写入字符串（到多个寄存器）
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="text">要写入的字符串</param>
        /// <returns>成功返回true，失败返回false</returns>
        bool WriteString( string address,string text );
        #endregion

        #region 配置与状态
        /// <summary>
        /// 获取或设置PLC IP地址
        /// </summary>
        string IpAddress {
            get; set;
            }

        /// <summary>
        /// 获取或设置通信端口
        /// </summary>
        int Port {
            get; set;
            }
        IPEndPoint IPEnd {
            get; set;
            }
        /// <summary>
        /// 获取或设置从站地址（Slave ID）
        /// </summary>
        byte SlaveId {
            get; set;
            }

        /// <summary>
        /// 获取或设置通信超时时间（毫秒）
        /// </summary>
        int Timeout {
            get; set;
            }

        /// <summary>
        /// 获取最后一次错误信息
        /// </summary>
        string LastError {
            get;
            }
        #endregion
        }
    }
