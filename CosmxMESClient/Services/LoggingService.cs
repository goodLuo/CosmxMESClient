using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CosmxMESClient.Services {
    public class LoggingService {
        private static readonly ILog log = LogManager.GetLogger(typeof(LoggingService));
        private static TextBox _logTextBox;
        private static RichTextBox _logRichTextBox;
        private static bool _initialized = false;

        static LoggingService( ) {
            Initialize( );
            }

        public static void Initialize( ) {
            if (_initialized)
                return;

            try {
                var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
                XmlConfigurator.Configure(logRepository,new FileInfo("App.config"));

                _initialized=true;
                Info("日志系统初始化成功");
                }
            catch (Exception ex) {
                MessageBox.Show($"日志初始化失败: {ex.Message}","错误",
                    MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }

        public static void SetLogTextBox( TextBox textBox ) {
            _logTextBox=textBox;
            }

        public static void SetLogRichTextBox( RichTextBox richTextBox ) {
            _logRichTextBox=richTextBox;
            }

        public static void Debug( string message ) {
            log.Debug(message);
            AppendToUI($"[DEBUG] {DateTime.Now:HH:mm:ss} - {message}",System.Drawing.Color.Gray);
            }

        public static void Info( string message ) {
            log.Info(message);
            AppendToUI($"[INFO] {DateTime.Now:HH:mm:ss} - {message}",System.Drawing.Color.Black);
            }

        public static void Warn( string message ) {
            log.Warn(message);
            AppendToUI($"[WARN] {DateTime.Now:HH:mm:ss} - {message}",System.Drawing.Color.Orange);
            }

        public static void Error( string message ) {
            log.Error(message);
            AppendToUI($"[ERROR] {DateTime.Now:HH:mm:ss} - {message}",System.Drawing.Color.Red);
            }

        public static void Error( string message,Exception ex ) {
            log.Error($"{message} - {ex.Message}",ex);
            AppendToUI($"[ERROR] {DateTime.Now:HH:mm:ss} - {message}: {ex.Message}",System.Drawing.Color.Red);
            }

        public static void PLCCommunication( string message,bool isSuccess = true ) {
            var logMessage = $"[PLC] {message}";
            if (isSuccess) {
                log.Info(logMessage);
                AppendToUI($"[PLC] {DateTime.Now:HH:mm:ss} - {message}",System.Drawing.Color.Green);
                }
            else {
                log.Error(logMessage);
                AppendToUI($"[PLC] {DateTime.Now:HH:mm:ss} - {message}",System.Drawing.Color.DarkRed);
                }
            }

        private static void AppendToUI( string message,System.Drawing.Color color ) {
            if (_logRichTextBox!=null&&!_logRichTextBox.IsDisposed) {
                if (_logRichTextBox.InvokeRequired) {
                    _logRichTextBox.Invoke(new Action<string,System.Drawing.Color>(AppendToUI),message,color);
                    return;
                    }

                try {
                    _logRichTextBox.SelectionStart=_logRichTextBox.TextLength;
                    _logRichTextBox.SelectionLength=0;
                    _logRichTextBox.SelectionColor=color;
                    _logRichTextBox.AppendText(message+Environment.NewLine);
                    _logRichTextBox.SelectionColor=_logRichTextBox.ForeColor;
                    _logRichTextBox.ScrollToCaret( );
                    }
                catch (Exception ex) {
                    System.Diagnostics.Debug.WriteLine($"日志输出到UI失败: {ex.Message}");
                    }
                }
            else if (_logTextBox!=null&&!_logTextBox.IsDisposed) {
                if (_logTextBox.InvokeRequired) {
                    _logTextBox.Invoke(new Action<string,System.Drawing.Color>(( msg,col ) =>
                        AppendToUITextBox(msg)),message);
                    return;
                    }
                AppendToUITextBox(message);
                }
            }

        private static void AppendToUITextBox( string message ) {
            try {
                _logTextBox.AppendText(message+Environment.NewLine);
                _logTextBox.SelectionStart=_logTextBox.TextLength;
                _logTextBox.ScrollToCaret( );
                }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"日志输出到TextBox失败: {ex.Message}");
                }
            }

        public static void ClearUILog( ) {
            if (_logRichTextBox!=null&&!_logRichTextBox.IsDisposed) {
                if (_logRichTextBox.InvokeRequired) {
                    _logRichTextBox.Invoke(new Action(ClearUILog));
                    return;
                    }
                _logRichTextBox.Clear( );
                }
            else if (_logTextBox!=null&&!_logTextBox.IsDisposed) {
                if (_logTextBox.InvokeRequired) {
                    _logTextBox.Invoke(new Action(ClearUILog));
                    return;
                    }
                _logTextBox.Clear( );
                }
            }
        }
    }