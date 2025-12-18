using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static CosmxMESClient.Communication.CDeltaModbusTcp;

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
        #region 基础数据读写（添加power参数）
        bool ReadRegister( string address,ref int value,int power = 1 );
        bool WriteRegister( string address,int value,int power = 1 );

        bool ReadInt32( string address,ref int value,int power = 1 );
        bool WriteInt32( string address,int value,int power = 1 );

        bool ReadFloat( string address,ref float value,int power = 1 );
        bool WriteFloat( string address,float value,int power = 1 );

        bool ReadDouble( string address,ref double value,int power = 1 );
        bool WriteDouble( string address,double value,int power = 1 );

        // bool类型不需要power参数
        bool ReadRegisterBit( string address,ref bool value );
        bool WriteRegisterBit( string address,bool value );
        #endregion

        #region 批量数据操作（添加power参数）
        bool BatchWriteFloat( List<string> addresses,float[] values,int power = 1 );
        bool BatchReadFloat( List<string> addresses,ref float[] values,int power = 1 );

        bool BatchReadInt32( List<string> addresses,ref int[] values,int power = 1 );
        bool BatchWriteInt32( List<string> addresses,int[] values,int power = 1 );

        bool BatchReadInt( List<string> addresses,ref int[] values,int power = 1 );
        bool BatchWriteInt( List<string> addresses,int[] values,int power = 1 );

        bool BatchReadDouble( List<string> addresses,ref double[] values,int power = 1 );
        bool BatchWriteDouble( List<string> addresses,double[] values,int power = 1 );

        // 字符串和bool类型不需要power参数
        bool BatchReadString( List<string> addresses,int maxStringLength,ref string[] texts );
        bool BatchWriteString( List<string> addresses,string[] texts,int maxStringLength );

        bool BatchReadRegisterBits( List<string> addresses,ref bool[] values );
        bool BatchWriteRegisterBits( List<string> addresses,bool[] values );
        #endregion

        #region 自动读取操作
        bool AddOrUpdateAutoRead( string key,string address,TypeCode dataType,int readIntervalMs = 1000,
                                int power = 1,int length = 1,int maxLength = 20,bool isBulkRead = true );

        bool AddOrUpdateAutoRead( string key,List<string> addresses,TypeCode dataType,int readIntervalMs = 1000,
                                int power = 1,int length = 1,int maxLength = 10,bool isBulkRead = true );

        bool RemoveAutoRead( string key );
        bool RemoveAutoReadAll( );
        List<string> GetAutoReadKeys( );
        AutoReadConfig GetAutoReadConfig( string key );
        void StartAutoRead( );
        void StopAutoRead( );
        bool ExecuteAutoReadNow( string key );
        #endregion
        #region 位操作（不需要power参数）
        bool ReadCoil( string address,ref bool value );
        bool WriteCoil( string address,bool value );
        bool ReadCoils( string address,int length,ref bool[] values );
        bool WriteCoils( string address,bool[] values );
        #endregion
        #region 字符串操作（不需要power参数）
        bool ReadString( string address,ref string text,int maxLength = 20 );
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

        #region 动态分组
        List<AddressGroup> GroupContinuousAddresses(List<string> addresses, TypeCode dataType);
        #endregion
    }
}
