using ColorControl.Forms;
using ColorControl.Properties;
using ColorControl.Services.AMD;
using ColorControl.Services.Common;
using ColorControl.Services.GameLauncher;
using ColorControl.Services.LG;
using ColorControl.Services.NVIDIA;
using ColorControl.Services.Samsung;
using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts;
using ColorControl.Shared.EventDispatcher;
using ColorControl.Shared.Forms;
using ColorControl.Shared.Native;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NWin32;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorControl
{
    public partial class MainForm : Form
    {
        private static bool SystemShutdown = false;
        private static bool EndSession = false;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ServiceManager _serviceManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly ElevationService _elevationService;

        private NvPanel _nvPanel;

        public NotifyIconManager _notifyIconManager;
        private readonly KeyboardShortcutDispatcher _keyboardShortcutDispatcher;
        private readonly UpdateManager _updateManager;
        private Config _config;

        private LgPanel _lgPanel;
        private SamsungPanel _samsungPanel;

        private AmdPanel _amdPanel;
        private bool _skipResize;

        private GamePanel _gamePanel;

        public MainForm(GlobalContext globalContext, ServiceManager serviceManager, IServiceProvider serviceProvider, ElevationService elevationService,
            NotifyIconManager notifyIconManager, KeyboardShortcutDispatcher keyboardShortcutDispatcher, UpdateManager updateManager)
        {
            InitializeComponent();

            Icon = Resources.AppIcon;

            _serviceManager = serviceManager;
            _serviceProvider = serviceProvider;
            _elevationService = elevationService;
            _notifyIconManager = notifyIconManager;
            _keyboardShortcutDispatcher = keyboardShortcutDispatcher;
            _updateManager = updateManager;
            _config = Program.Config;

            Text = globalContext.ApplicationTitleAdmin;

            LoadConfig();

            MessageForms.MainForm = this;

            _notifyIconManager.Build();

            InitModules();

            Task.Run(AfterInitialized);

            if (_config.UseDarkMode)
            {
                SetTheme(_config.UseDarkMode);
            }
        }

        private void InitModules()
        {
            var _ = tcMain.Handle;

            _nvPanel = InitPanel<NvPanel>("NVIDIA controller");
            _amdPanel = InitPanel<AmdPanel>("AMD controller");
            _lgPanel = InitPanel<LgPanel>("LG controller");
            _samsungPanel = InitPanel<SamsungPanel>("Samsung controller");
            _gamePanel = InitPanel<GamePanel>("Game launcher");

            tcMain.SelectedIndex = 0;
        }

        private T InitPanel<T>(string displayName) where T : UserControl, IModulePanel
        {
            var module = _config.Modules.FirstOrDefault(m => m.IsActive && m.DisplayName == displayName);
            if (module == null)
            {
                return default;
            }

            try
            {
                var panel = _serviceProvider.GetRequiredService<T>();
                panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

                panel.Init();

                var tabPage = new TabPage(module.DisplayName);
                tcMain.TabPages.Insert(tcMain.TabPages.Count - 2, tabPage);

                tabPage.Controls.Add(panel);

                panel.Size = tabPage.ClientSize;
                panel.BackColor = SystemColors.Window;

                //Logger.Debug($"Panel {panel.GetType().Name}, size: {panel.Size.Width}x{panel.Size.Height}");

                return panel;
            }
            catch (Exception ex)
            {
                Logger.Debug($"Error creating panel {typeof(T).Name}: {ex.ToLogString()}");

                return null;
            }
        }

        public void OpenForm()
        {
            Show();
            WindowState = FormWindowState.Normal;
            Activate();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Program.UserExit)
            {
                return;
            }

            if (!(SystemShutdown || EndSession) && _config.MinimizeOnClose)
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;
                Program.UserExit = false;
                return;
            }

            GlobalSave();

            if (SystemShutdown)
            {
                Logger.Debug($"MainForm_FormClosing: SystemShutdown");
            }

            Program.Exit();
        }

        private void GlobalSave()
        {
            _serviceManager.Save();

            _nvPanel?.Save();
            _amdPanel?.Save();
            _lgPanel?.Save();
            _gamePanel?.Save();
            _samsungPanel?.Save();

            SaveConfig();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeConstants.WM_QUERYENDSESSION)
            {
                SystemShutdown = true;
            }
            else if (m.Msg == NativeConstants.WM_ENDSESSION)
            {
                EndSession = true;
            }
            else if (m.Msg == Utils.WM_BRINGTOFRONT)
            {
                Logger.Debug("WM_BRINGTOFRONT message received, opening form");
                OpenForm();
            }

            base.WndProc(ref m);
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (_skipResize)
            {
                return;
            }

            if (WindowState == FormWindowState.Minimized)
            {
                if (_config.MinimizeToTray)
                {
                    Hide();
                }
            }
            else if (WindowState == FormWindowState.Normal && _config != null)
            {
                _config.FormWidth = Width;
                _config.FormHeight = Height;
            }
        }

        private void LoadConfig()
        {
            _skipResize = true;
            try
            {
                Width = _config.FormWidth;
                Height = _config.FormHeight;
            }
            finally
            {
                _skipResize = false;
            }
        }

        private void SaveConfig()
        {
            if (WindowState != FormWindowState.Minimized)
            {
                _config.FormWidth = Width;
                _config.FormHeight = Height;
            }

            Utils.WriteObject(Program.ConfigFilename, _config);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            InitSelectedTab();

            _elevationService.CheckElevationMethod();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitSelectedTab();
        }

        private void InitSelectedTab()
        {
            if (tcMain.SelectedTab == tabInfo)
            {
                InitInfoTab();
            }
            else if (tcMain.SelectedTab == tabOptions)
            {
                InitOptionsTab();
            }
            else if (tcMain.SelectedTab?.Controls.Count > 0 && tcMain.SelectedTab.Controls[0] is IModulePanel panel)
            {
                panel.UpdateInfo();
            }
        }

        private void InitOptionsTab()
        {
            if (tabOptions.Controls.Count > 0)
            {
                return;
            }

            var control = _serviceProvider.GetRequiredService<OptionsPanel>();
            control.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            tabOptions.Controls.Add(control);

            control.Size = tabInfo.ClientSize;
        }

        private void InitInfoTab()
        {
            if (tabInfo.Controls.Count > 0)
            {
                return;
            }
            var control = _serviceProvider.GetRequiredService<InfoPanel>();
            control.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            tabInfo.Controls.Add(control);

            control.Size = tabInfo.ClientSize;
            //control.BackColor = SystemColors.Window;
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            if (tcMain.SelectedTab?.Controls.Count > 0 && tcMain.SelectedTab.Controls[0] is IModulePanel panel)
            {
                panel.UpdateInfo();
            }
        }

        private async Task AfterInitialized()
        {
            if (_nvPanel != null)
            {
                await _nvPanel.AfterInitialized();
            }
            if (_amdPanel != null)
            {
                await _amdPanel.AfterInitialized();
            }
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            GlobalSave();
        }

        private void MainForm_ResizeBegin(object sender, EventArgs e)
        {
            SuspendLayout();
        }

        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            ResumeLayout(true);
        }

        private async void MainForm_Click(object sender, EventArgs e)
        {
            _serviceManager.NvService?.TestResolution();
        }

        public void SetTheme(bool toDark)
        {
            this.UpdateTheme(toDark);

            WinApi.RefreshImmersiveColorPolicyState();

            DarkModeUtils.InitWpfTheme();

            DarkModeUtils.SetContextMenuForeColor(_notifyIconManager.NotifyIcon.ContextMenuStrip, FormUtils.CurrentForeColor);

            InitSelectedTab();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            _updateManager.InstallUpdate();

            Program.Restart();
        }

        internal void CloseForRestart()
        {
            Hide();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((keyData == (Keys.Control | Keys.PageUp) || keyData == (Keys.Control | Keys.PageDown)) && KeyboardShortcutDispatcher.IsShortcutControlFocused())
            {
                var control = this.FindFocusedControl() as TextBox;

                if (control != null)
                {
                    var keyEvent = new KeyEventArgs(keyData);

                    control.Text = _keyboardShortcutDispatcher.FormatKeyboardShortcut(keyEvent);
                }

                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}