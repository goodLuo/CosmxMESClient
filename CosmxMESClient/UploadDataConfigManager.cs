using CosmxMESClient.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;

namespace CosmxMESClient
{
    /// <summary>
    /// MES上传参数配置管理器
    /// </summary>
    public static class UploadDataConfigManager
    {
        private static readonly string ConfigDir = Path.Combine(Application.StartupPath, "FormUploadData");
        private static readonly string ConfigFilePath = Path.Combine(ConfigDir, "UploadDataConfig.json");

        private static UploadDataConfig _cachedConfig;


        /// <summary>
        /// 获取当前配置
        /// </summary>
        public static UploadDataConfig GetConfig() => LoadConfig();

        /// <summary>
        /// 加载配置（带缓存，重复调用不会反复读文件）
        /// </summary>
        public static UploadDataConfig LoadConfig()
        {
            try
            {
                if (_cachedConfig != null)
                    return _cachedConfig;

                if (!Directory.Exists(ConfigDir))
                    Directory.CreateDirectory(ConfigDir);

                if (File.Exists(ConfigFilePath))
                {
                    string json = File.ReadAllText(ConfigFilePath, Encoding.UTF8);
                    _cachedConfig = JsonConvert.DeserializeObject<UploadDataConfig>(json) ?? new UploadDataConfig();
                }
                else
                {
                    _cachedConfig = new UploadDataConfig();
                }

                // 配置加载完毕后初始化PLC扫描地址
                InitBindPLCScanAddress();


                return _cachedConfig;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载MES上传配置失败：{ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _cachedConfig = new UploadDataConfig();
                return _cachedConfig;
            }
        }

        private static void InitBindPLCScanAddress()
        {
            try
            {
                foreach (var item in _cachedConfig.UploadItems)
                {
                    item.Value.BindPLCScanAddress = (GlobalVariables.AllAvailableTriggerAddresses.FirstOrDefault(p =>
                    p.Key == item.Value.PLCScanAddressKey.ToString().Split('|')[0]).Value) == null ?
                    null :
                    (GlobalVariables.AllAvailableTriggerAddresses.FirstOrDefault
                    (p => p.Key == item.Value.PLCScanAddressKey.ToString().Split('|')[0]).Value);
                }
            }
            catch (Exception ex)
            {
                LoggingService.Error($"数据上传配初始化PLC扫描地址异常", ex);
            }
        }





        /// <summary>
        /// 保存配置（会覆盖文件，并更新缓存）
        /// </summary>
        public static void SaveConfig(UploadDataConfig config)
        {
            try
            {
                if (!Directory.Exists(ConfigDir))
                    Directory.CreateDirectory(ConfigDir);

                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(ConfigFilePath, json, Encoding.UTF8);

                _cachedConfig = config; // 更新缓存
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存MES上传配置失败：{ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        

        /// <summary>
        /// 强制重新从文件加载
        /// </summary>
        public static void Reload()
        {
            _cachedConfig = null;
            LoadConfig();
        }
    }
}