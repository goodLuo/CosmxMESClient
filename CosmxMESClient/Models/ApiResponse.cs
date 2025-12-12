using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CosmxMESClient.Models {
    public class ApiResponse {
        [JsonPropertyName("code")]
        public string Code { get; set; } = "1";

        [JsonPropertyName("msg")]
        public string Msg { get; set; } = "";

        [JsonPropertyName("data")]
        public object Data {
            get; set;
            }

        [JsonIgnore]
        public bool IsSuccess => Code=="0";
        }
    public class ProductCodeResponse {
        public string[] ProjCode {
            get; set;
            }
        }

    public class CardCheckResponse {
        public string Code {
            get; set;
            }
        public string Message {
            get; set;
            }
        public string EmployeeNo {
            get; set;
            }
        public string CheckTime {
            get; set;
            }
        public string Department {
            get; set;
            }
        public string PermissionLevel {
            get; set;
            }
        }
    }
