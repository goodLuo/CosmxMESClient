using CosmxMESClient.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmxMESClient {
    public class SimplifiedPLCReconnectManager:IDisposable {
        private readonly ConcurrentDictionary<string, PLCConnectionConfig> _plcConfigs;
        private readonly System.Timers.Timer _reconnectTimer;
        private readonly int _reconnectInterval = 10000; // 10秒
        private readonly int _maxRetryCount = 5;
        private bool _disposed = false;

        public SimplifiedPLCReconnectManager( ) {
            _plcConfigs=new ConcurrentDictionary<string,PLCConnectionConfig>( );
            _reconnectTimer=new System.Timers.Timer(_reconnectInterval);
            _reconnectTimer.Elapsed+=async ( s,e ) => await CheckAndReconnectAllAsync( );
            _reconnectTimer.Start( );
            }

        public void RegisterPLC( PLCConnectionConfig config ) {
            if (_plcConfigs.TryAdd(config.Key,config)) {
                // 立即尝试连接
                _=TryConnectPLCAsync(config);
                }
            }

        public void UnregisterPLC( string configKey ) {
            _plcConfigs.TryRemove(configKey,out _);
            }

        private async Task CheckAndReconnectAllAsync( ) {
            var tasks = _plcConfigs.Values
            .Where(config => !config.IsConnected && ShouldRetry(config))
            .Select(config => TryConnectPLCAsync(config))
            .ToArray();

            if (tasks.Length>0) {
                await Task.WhenAll(tasks);
                }
            }

        private async Task<bool> TryConnectPLCAsync( PLCConnectionConfig config ) {
            try {
                config.ReconnectAttempts++;
                config.LastReconnectAttempt=DateTime.Now;

                LoggingService.Info($"尝试连接PLC: {config.Name} (第{config.ReconnectAttempts}次)");

                bool success = await Task.Run(() => config.Connect());

                if (success) {
                    config.ReconnectAttempts=0;
                    config.LastConnectedTime=DateTime.Now;
                    LoggingService.Info($"PLC连接成功: {config.Name}");

                    // 连接成功后启动自动读取
                    config.StartAutoRead( );
                    return true;
                    }

                LoggingService.Warn($"PLC连接失败: {config.Name}");
                return false;
                }
            catch (Exception ex) {
                LoggingService.Error($"PLC连接异常: {config.Name}",ex);
                return false;
                }
            }

        private bool ShouldRetry( PLCConnectionConfig config ) {
            if (config.ReconnectAttempts>=_maxRetryCount)
                return false;

            // 指数退避算法
            var minWaitTime = TimeSpan.FromSeconds(Math.Pow(2, config.ReconnectAttempts) * 5);
            var timeSinceLastAttempt = DateTime.Now - config.LastReconnectAttempt;

            return timeSinceLastAttempt>minWaitTime;
            }

        public void Dispose( ) {
            if (!_disposed) {
                _reconnectTimer?.Stop( );
                _reconnectTimer?.Dispose( );
                _disposed=true;
                }
            }
        }
    }
