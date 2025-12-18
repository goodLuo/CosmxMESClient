using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmxMESClient
{
    [Serializable]
    public class UploadDataConfig
    {
        public Dictionary<string, UploadItem> UploadItems { get; set; } = new Dictionary<string, UploadItem>();
        public bool EnableMESUploadData { get; set; } // 是否启用MES

    }

    /// <summary>
    /// 上传项类
    /// </summary>
    [Serializable]
    public class UploadItem
    {
        public string ParameterName { get; set; } // 参数名（唯一ID）
        public bool IsUpload { get; set; } // 是否上传（bool）
        public string Description { get; set; } // 参数描述（字符串）
        public ParameterType Type { get; set; } // 参数类型（枚举）
        public bool IsNullable { get; set; } // 是否可空（bool）
        public int? Length { get; set; } // 长度（当参数类型为String必填，整数，可空）
        public string ParameterValue { get; set; } // 参数值
        public string PLCScanAddressKey { get; set; } // PLC扫描地址Key
        private PLCScanAddress? _bindPLCScanAddress;
        //public PLCScanAddress? BindPLCScanAddress
        //{
        //    get => (GlobalVariables.AllAvailableTriggerAddresses.FirstOrDefault(p => p.Key==PLCScanAddressKey).Value)==null?null: (GlobalVariables.AllAvailableTriggerAddresses.FirstOrDefault(p => p.Key == PLCScanAddressKey.Split('|')[0]).Value).TriggerPLCScanAddress;
        //set
        //}
        public PLCScanAddress? BindPLCScanAddress
        {
            get => _bindPLCScanAddress;
            set
            {
                _bindPLCScanAddress = value; 
            }
        }
    }

    // 枚举：参数类型
    public enum ParameterType
    {
        字符串String,
        整数Int,
        布尔Bool,
        单精度浮点Float,
        双精度浮点Double,
        时间TimeStamp
    }
}
