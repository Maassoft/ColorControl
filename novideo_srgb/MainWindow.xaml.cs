using ColorControl.Shared.XForms;
using Microsoft.Win32;
using System.ComponentModel;
using System.Windows;

namespace novideo_srgb
{
    public partial class MainWindow : BaseWindow
    {
        private readonly MainViewModel _viewModel;

        private static MainWindow _novideoWindow;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = (MainViewModel)DataContext;
            SystemEvents.DisplaySettingsChanged += _viewModel.OnDisplaySettingsChanged;
            SystemEvents.PowerModeChanged += _viewModel.OnPowerModeChanged;
        }

        public static void CreateAndShow(bool show = true)
        {
            if (System.Windows.Application.Current == null)
            {
                new System.Windows.Application();
            }

            _novideoWindow ??= new MainWindow();

            if (show)
            {
                _novideoWindow.WindowState = WindowState.Normal;
                _novideoWindow.Show();
                _novideoWindow.Topmost = true;
                _novideoWindow.Topmost = false;
            }
        }

        public static bool IsInitialized()
        {
            return _novideoWindow != null;
        }

        public static void BeforeDisplaySettingsChange()
        {
            if (!IsInitialized())
            {
                return;
            }

            _novideoWindow.UpdateMonitors();
        }

        private void UpdateMonitors()
        {
            _viewModel.UpdateMonitors(false);
        }

        public static void UpdateContextMenu(ToolStripMenuItem parent)
        {
            if (_novideoWindow == null)
            {
                return;
            }

            var menu = new ToolStripDropDownMenu();

            parent.DropDownItems.Clear();

            foreach (var monitor in _novideoWindow._viewModel.Monitors)
            {
                var item = new ToolStripMenuItem();
                parent.DropDownItems.Add(item);
                item.Text = monitor.Name;
                item.CheckState = monitor.Clamped == true ? CheckState.Checked : monitor.Clamped == false ? CheckState.Unchecked : CheckState.Indeterminate;
                item.Enabled = monitor.CanClamp;
                item.ForeColor = parent.ForeColor;
                item.Click += (sender, args) => monitor.Clamped = !monitor.Clamped;
            }

            parent.DropDownItems.Add("-").ForeColor = parent.ForeColor;

            var reapplyItem = new ToolStripMenuItem();
            parent.DropDownItems.Add(reapplyItem);
            reapplyItem.Text = "Reapply";
            reapplyItem.ForeColor = parent.ForeColor;
            reapplyItem.Click += delegate { _novideoWindow.ReapplyMonitorSettings(); };
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

        private void AboutButton_Click(object sender, RoutedEventArgs o)
        {
            var window = new AboutWindow
            {
                Owner = this
            };
            window.ShowDialog();
        }

        private void AdvancedButton_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Application.Current.Windows.Cast<System.Windows.Window>().Any(x => x is AdvancedWindow)) return;
            var monitor = ((FrameworkElement)sender).DataContext as MonitorData;
            var window = new AdvancedWindow(monitor)
            {
                Owner = this
            };

            void CloseWindow(object o, EventArgs e2) => window.Close();

            SystemEvents.DisplaySettingsChanged += CloseWindow;
            if (window.ShowDialog() == false) return;
            SystemEvents.DisplaySettingsChanged -= CloseWindow;

            if (window.ChangedCalibration)
            {
                _viewModel.SaveConfig();
                monitor?.ReapplyClamp();
            }
        }

        private void ReapplyButton_Click(object sender, RoutedEventArgs e)
        {
            ReapplyMonitorSettings();
        }

        private void ReapplyMonitorSettings()
        {
            foreach (var monitor in _viewModel.Monitors)
            {
                monitor.ReapplyClamp();
            }
        }

        public static void ApplySettings(string monitorId, bool clamp, int targetColorSpace)
        {
            CreateAndShow(false);

            _novideoWindow.ApplyClamp(monitorId, clamp, targetColorSpace);
        }

        private void ApplyClamp(string monitorId, bool clamp, int targetColorSpace)
        {
            var monitor = _viewModel.Monitors.FirstOrDefault(m => monitorId == null || m.Path.Contains(monitorId));

            if (monitor == null)
            {
                return;
            }

            monitor.Target = targetColorSpace;
            monitor.Clamped = clamp;
        }
    }
}