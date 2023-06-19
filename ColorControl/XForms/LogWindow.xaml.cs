using ColorControl.Common;
using NLog;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Navigation;

namespace ColorControl.XForms
{
    public partial class LogWindow : Window
    {
        private static LogWindow _window;

        private int _logLevelIndex;

        public int LogLevelIndex { get => _logLevelIndex; set => _logLevelIndex = value; }

        public LogWindow()
        {
            InitializeComponent();

            LogLevelIndex = LogLevel.FromString(AppContext.CurrentContext.Config.LogLevel).Ordinal;

            DataContext = this;
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

            _window ??= new LogWindow();

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
            var context = AppContext.CurrentContext;
            var logFile = Path.Combine(context.DataPath, "LogFile.txt");

            Utils.StartProcess(logFile);
        }

        private void LoadOlder_Click(object sender, RoutedEventArgs e)
        {
            var context = AppContext.CurrentContext;
            var logFile = Path.Combine(context.DataPath, "LogFile.txt");

            var lines = Utils.ReadLines(logFile);

            logViewer.LoadLines(lines);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void LogLevel_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var context = AppContext.CurrentContext;

            var logLevel = LogLevel.FromOrdinal(_logLevelIndex);

            context.SetLogLevel(logLevel);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (logViewer.HasEvents())
            {
                return;
            }

            var context = AppContext.CurrentContext;
            var logFile = Path.Combine(context.DataPath, "LogFile.txt");

            var lines = Utils.ReadLines(logFile);

            logViewer.LoadLines(lines, context.StartTime);
        }
    }
}