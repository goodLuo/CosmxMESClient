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
                    Converters = { new TypeConverter() }
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
                        Converters = { new TypeConverter() }
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

        // Type转换器用于JSON序列化
        private class TypeConverter:JsonConverter<Type> {
            public override Type Read( ref Utf8JsonReader reader,Type typeToConvert,JsonSerializerOptions options ) {
                var typeName = reader.GetString();
                return Type.GetType(typeName)??typeof(int);
                }

            public override void Write( Utf8JsonWriter writer,Type value,JsonSerializerOptions options ) {
                writer.WriteStringValue(value.AssemblyQualifiedName);
                }
            }
        }
    }
