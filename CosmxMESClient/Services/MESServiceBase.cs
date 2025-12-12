using CosmxMESClient.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CosmxMESClient.Services {
    public interface IMESService {
        Task<ApiResponse> PostAsync<T>( string endpoint,T requestData );
        string GetBaseUrl( );
        }

    public class MESServiceBase:IMESService {
        private static readonly ILog log = LogManager.GetLogger(typeof(MESServiceBase));

        protected readonly HttpClient _httpClient;
        protected readonly string _baseUrl;
        protected readonly int _timeoutSeconds;

        public MESServiceBase( ) {
            _baseUrl=ConfigurationManager.AppSettings["MESBaseUrl"]??"http://10.0.3.27:88/api";
            _timeoutSeconds=int.Parse(ConfigurationManager.AppSettings["TimeoutSeconds"]??"30");

            _httpClient=new HttpClient( );
            _httpClient.Timeout=TimeSpan.FromSeconds(_timeoutSeconds);
            _httpClient.DefaultRequestHeaders.Add("User-Agent","CosmxMESClient/1.0");
            }

        public async Task<ApiResponse> PostAsync<T>( string endpoint,T requestData ) {
            int retryCount = 0;
            int maxRetry = int.Parse(ConfigurationManager.AppSettings["MaxRetryCount"] ?? "3");

            LoggingService.Info($"调用MES接口: {endpoint}");

            while (retryCount<maxRetry) {
                try {
                    var json = JsonSerializer.Serialize(requestData, new JsonSerializerOptions
                        {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        });

                    LoggingService.Debug($"请求数据: {json}");

                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync($"{_baseUrl}/{endpoint}", content);

                    if (response.IsSuccessStatusCode) {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        LoggingService.Debug($"响应数据: {responseContent}");

                        var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        LoggingService.Info($"MES接口调用成功: {endpoint}");
                        return apiResponse??new ApiResponse { Code="1",Msg="响应解析失败" };
                        }
                    else {
                        LoggingService.Warn($"HTTP错误: {response.StatusCode}, 接口: {endpoint}");
                        throw new HttpRequestException($"HTTP错误: {response.StatusCode}");
                        }
                    }
                catch (Exception ex) {
                    retryCount++;
                    LoggingService.Warn($"MES接口调用失败 (第{retryCount}次重试): {endpoint} - {ex.Message}");

                    if (retryCount>=maxRetry) {
                        LoggingService.Error($"MES接口调用最终失败: {endpoint}",ex);
                        return new ApiResponse
                            {
                            Code="1",
                            Msg=$"接口调用失败({retryCount}次重试): {ex.Message}"
                            };
                        }

                    await Task.Delay(1000*retryCount);
                    }
                }

            return new ApiResponse { Code="1",Msg="未知错误" };
            }

        public string GetBaseUrl( ) => _baseUrl;

        protected void Log( string message ) {
            // 在实际项目中可以写入日志文件
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}");
            }

        protected void ShowMessage( ApiResponse response,Control parent = null,string successTitle = "成功" ) {
            if (response.IsSuccess) {
                MessageBox.Show(parent??Form.ActiveForm,response.Msg,successTitle,
                    MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
            else {
                MessageBox.Show(parent??Form.ActiveForm,response.Msg,"错误",
                    MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }

        protected void UpdateStatusLabel( Label statusLabel,ApiResponse response ) {
            if (statusLabel!=null) {
                if (response.IsSuccess) {
                    statusLabel.Text="操作成功";
                    statusLabel.ForeColor=System.Drawing.Color.Green;
                    }
                else {
                    statusLabel.Text=$"操作失败: {response.Msg}";
                    statusLabel.ForeColor=System.Drawing.Color.Red;
                    }
                }
            }
        }
    }
