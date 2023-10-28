using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts;
using ColorControl.Shared.Forms;
using ColorControl.Shared.Services;
using ColorControl.Shared.XForms;
using DJ;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;

namespace ColorControl.XForms
{
    public partial class LogWindow : BaseWindow
    {
        private static LogWindow _window;

        private int _logLevelIndex;
        private readonly WinApiAdminService _winApiAdminService;

        public int LogLevelIndex { get => _logLevelIndex; set => _logLevelIndex = value; }

        public LogWindow(WinApiAdminService winApiAdminService)
        {
            InitializeComponent();

            LogLevelIndex = LogLevel.FromString(GlobalContext.CurrentContext.Config.LogLevel).Ordinal;

            DataContext = this;
            _winApiAdminService = winApiAdminService;
        }

        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            var processStartInfo = new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri)
            {
                UseShellExecute = true,
            };
            System.Diagnostics.Process.Start(processStartInfo);
        }

        public static void CreateAndShow(bool show = true)
        {
            if (Application.Current == null)
            {
                new Application();
            }

            _window ??= Program.ServiceProvider.GetRequiredService<LogWindow>();

            if (show)
            {
                _window.WindowState = WindowState.Normal;
                _window.Show();
                _window.Topmost = true;
                _window.Topmost = false;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;

            Hide();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }

            base.OnStateChanged(e);
        }

        private void RawLog_Click(object sender, RoutedEventArgs e)
        {
            var context = Shared.Common.GlobalContext.CurrentContext;

            string logFile;
            if (tabControl.SelectedIndex == 0)
            {
                logFile = Program.LogFilename;
            }
            else
            {
                var winApiService = Program.ServiceProvider.GetRequiredService<WinApiService>();

                if (!winApiService.IsServiceRunning())
                {
                    MessageForms.WarningOk("Service is not running");
                    return;
                }

                var message = new SvcMessage { MessageType = SvcMessageType.GetLog };

                var result = PipeUtils.SendMessage(message);

                var logData = result?.Data ?? "Cannot get log from service";

                logFile = Path.Combine(Path.GetTempPath(), "CCServiceLogFile.txt");

                Utils.WriteText(logFile, logData);
            }

            if (!File.Exists(logFile))
            {
                MessageForms.WarningOk("Log file not found.");
                return;
            }

            _winApiAdminService.StartProcess(logFile);
        }

        private void LoadOlder_Click(object sender, RoutedEventArgs e)
        {
            var viewer = GetCurrentViewer();

            var lines = LoadLog();

            viewer.LoadLines(lines);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void LogLevel_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var context = Shared.Common.GlobalContext.CurrentContext;

            var logLevel = LogLevel.FromOrdinal(_logLevelIndex);

            context.SetLogLevel(logLevel);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var viewer = GetCurrentViewer();

            if (viewer.HasEvents())
            {
                return;
            }

            var context = Shared.Common.GlobalContext.CurrentContext;

            var lines = LoadLog();

            viewer.LoadLines(lines, context.StartTime);
        }

        private List<string> LoadLog()
        {
            string logFile;

            if (tabControl.SelectedIndex == 0)
            {
                return Utils.ReadLines(Program.LogFilename);
            }

            var winApiService = Program.ServiceProvider.GetRequiredService<WinApiService>();

            if (winApiService.IsServiceRunning())
            {
                var message = new SvcMessage { MessageType = SvcMessageType.GetLog };

                var result = PipeUtils.SendMessage(message);

                logFile = result?.Data ?? "Cannot get log from service";
            }
            else
            {
                logFile = "The service is not installed or not running";
            }

            return logFile.Split("\r\n").ToList();
        }

        private NLogViewer GetCurrentViewer()
        {
            return tabControl.SelectedIndex == 0 ? logViewerApplication : logViewerService;
        }

        private void DeleteLog_Click(object sender, RoutedEventArgs e)
        {
            if (MessageForms.QuestionYesNo("Are you sure you want to delete the log? This will clear all logging.") != System.Windows.Forms.DialogResult.Yes)
            {
                return;
            }

            if (tabControl.SelectedIndex == 0)
            {
                var filename = Program.LogFilename;
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }

                logViewerApplication.ClearCommand.Execute(null);
            }
            else
            {
                PipeUtils.SendMessage(SvcMessageType.ClearLog);
                logViewerService.ClearCommand.Execute(null);
            }
        }
    }
}