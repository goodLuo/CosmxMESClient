using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmxMESClient
{
    public class GlobalVariables
    {
        /// <summary>
        /// PLC配置
        /// </summary>
        public static List<PLCConnectionConfig> PLCConnections = new List<PLCConnectionConfig>();
        /// <summary>
        /// 扫描地址配置
        /// </summary>
        public static Dictionary<string, PLCScanAddress> AvailableTriggerAddresses = new Dictionary<string, PLCScanAddress>();

        public static Dictionary<string, PLCScanAddress> AllAvailableTriggerAddresses = new Dictionary<string, PLCScanAddress>();

    }
}
