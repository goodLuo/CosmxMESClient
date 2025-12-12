using CosmxMESClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CosmxMESClient {
    internal static class Program {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main( ) {
            // 初始化日志系统
            LoggingService.Initialize( );

            Application.EnableVisualStyles( );
            Application.SetCompatibleTextRenderingDefault(false);
            try {
                LoggingService.Info("应用程序启动");
                Application.Run(new Form1( ));
                }
            catch (Exception ex) {
                LoggingService.Error("应用程序运行异常",ex);
                MessageBox.Show($"应用程序启动失败: {ex.Message}","错误",
                    MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            finally {
                LoggingService.Info("应用程序退出");
                }
            }
        }
    }
