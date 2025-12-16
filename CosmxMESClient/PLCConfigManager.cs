using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CosmxMESClient {
    public static class PLCConfigManager {
        private static readonly string ConfigFilePath =
            Path.Combine(Application.StartupPath, "PLCConfigs.json");

        public static void SaveConfigs( List<PLCConnectionConfig> configs ) {
            try {
                var options = new JsonSerializerOptions
                    {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    };

                var json = JsonSerializer.Serialize(configs, options);
                File.WriteAllText(ConfigFilePath,json);
                }
            catch (Exception ex) {
                MessageBox.Show($"保存配置失败: {ex.Message}","错误",
                    MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
        public static List<PLCConnectionConfig> LoadConfigs( ) {
            try {
                if (File.Exists(ConfigFilePath)) {
                    var json = File.ReadAllText(ConfigFilePath);
                    var options = new JsonSerializerOptions
                        {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        };

                    return JsonSerializer.Deserialize<List<PLCConnectionConfig>>(json,options)
                        ??new List<PLCConnectionConfig>( );
                    }
                }
            catch (Exception ex) {
                MessageBox.Show($"加载配置失败: {ex.Message}","错误",
                    MessageBoxButtons.OK,MessageBoxIcon.Error);
                }

            return new List<PLCConnectionConfig>( );
            }
        }
    }
