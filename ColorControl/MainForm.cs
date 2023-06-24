using ColorControl.Common;
using ColorControl.Forms;
using ColorControl.Native;
using ColorControl.Services.AMD;
using ColorControl.Services.Common;
using ColorControl.Services.EventDispatcher;
using ColorControl.Services.GameLauncher;
using ColorControl.Services.LG;
using ColorControl.Services.NVIDIA;
using ColorControl.Services.Samsung;
using ColorControl.Svc;
using ColorControl.XForms;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using novideo_srgb;
using NvAPIWrapper.Display;
using NWin32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorControl
{
    public partial class MainForm : Form
    {
        private static bool SystemShutdown = false;
        private static bool EndSession = false;
        private static bool UserExit = false;
        private static int SHORTCUTID_SCREENSAVER = -100;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly PowerEventDispatcher _powerEventDispatcher;
        private readonly ServiceManager _serviceManager;
        private string _dataDir;

        private NvPanel _nvPanel;
        private NvDitherPanel _nvDitherPanel;

        private NotifyIcon _trayIcon;
        private bool _initialized = false;
        private Config _config;
        private bool _setVisibleCalled = false;

        private LgPanel _lgPanel;
        private SamsungPanel _samsungPanel;

        private ToolStripMenuItem _nvTrayMenu;
        private ToolStripMenuItem _novideoTrayMenu;
        private ToolStripMenuItem _amdTrayMenu;
        private ToolStripMenuItem _lgTrayMenu;
        private ToolStripMenuItem _samsungTrayMenu;
        private ToolStripMenuItem _gameTrayMenu;

        private StartUpParams StartUpParams { get; }
        public string _lgTabMessage { get; private set; }

        private AmdPanel _amdPanel;
        private bool _skipResize;
        private FileVersionInfo _currentVersionInfo;
        private bool _checkedForUpdates = false;
        private string _updateHtmlUrl;
        private string _downloadUrl;

        private GamePanel _gamePanel;

        //private KeyboardHookManager _keyboardHookManager;
        private IntPtr _screenStateNotify;

        private Dictionary<int, Action> _shortcuts = new();
        private Dictionary<string, Func<UserControl>> _modules = new();

        public MainForm(AppContextProvider appContextProvider, PowerEventDispatcher powerEventDispatcher, ServiceManager serviceManager)
        {
            InitializeComponent();

            StartUpParams = appContextProvider.GetAppContext().StartUpParams;
            appContextProvider.GetAppContext().SynchronizationContext = AsyncOperationManager.SynchronizationContext;

            _powerEventDispatcher = powerEventDispatcher;
            _serviceManager = serviceManager;
            _dataDir = Program.DataDir;
            _config = Program.Config;

            LoadConfig();

            LoadModules();

            MessageForms.MainForm = this;

            _nvTrayMenu = new ToolStripMenuItem("NVIDIA presets");
            _novideoTrayMenu = new ToolStripMenuItem("Novideo sRGB");
            _amdTrayMenu = new ToolStripMenuItem("AMD presets");
            _lgTrayMenu = new ToolStripMenuItem("LG presets");
            _samsungTrayMenu = new ToolStripMenuItem("Samsung presets");
            _gameTrayMenu = new ToolStripMenuItem("Game Launcher");
            _trayIcon = new NotifyIcon()
            {
                Icon = Icon,
                ContextMenuStrip = new ContextMenuStrip(),
                Visible = _config.MinimizeToTray,
                Text = Text
            };

            _trayIcon.ContextMenuStrip.Items.AddRange(new ToolStripItem[] {
                    _nvTrayMenu,
                    _novideoTrayMenu,
                    _amdTrayMenu,
                    _lgTrayMenu,
                    _samsungTrayMenu,
                    _gameTrayMenu,
                    new ToolStripSeparator(),
                    new ToolStripMenuItem("Open", null, OpenForm),
                    new ToolStripSeparator(),
                    new ToolStripMenuItem("Restart", null, Restart),
                    new ToolStripMenuItem("Exit", null, Exit)
                });

            _trayIcon.MouseDoubleClick += trayIcon_MouseDoubleClick;
            _trayIcon.ContextMenuStrip.Opened += trayIconContextMenu_Popup;
            _trayIcon.BalloonTipClicked += trayIconBalloonTip_Clicked;

            chkStartAfterLogin.Checked = Utils.TaskExists(Program.TS_TASKNAME);

            chkFixChromeFonts.Enabled = Utils.IsChromeInstalled();
            if (chkFixChromeFonts.Enabled)
            {
                var fixInstalled = Utils.IsChromeFixInstalled();
                if (_config.FixChromeFonts && !fixInstalled)
                {
                    Utils.ExecuteElevated(StartUpParams.ActivateChromeFontFixParam);
                }
                chkFixChromeFonts.Checked = Utils.IsChromeFixInstalled();
            }

            _shortcuts.Add(SHORTCUTID_SCREENSAVER, StartScreenSaver);

            InitModules();
            InitInfo();
            UpdateServiceInfo();

            _screenStateNotify = WinApi.RegisterPowerSettingNotification(Handle, ref Utils.GUID_CONSOLE_DISPLAY_STATE, 0);

            //Scale(new SizeF(1.25F, 1.25F));

            _initialized = true;

            AfterInitialized();

            if (_config.UseDarkMode)
            {
                SetTheme(_config.UseDarkMode);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        private void LoadModules()
        {
            _modules.Add("NVIDIA controller", InitNvService);
            _modules.Add("AMD controller", InitAmdService);
            _modules.Add("LG controller", InitLgService);
            _modules.Add("Game launcher", InitGameService);
            _modules.Add("Samsung controller", InitSamsungService);

            foreach (var keyValue in _modules)
            {
                var existingModule = _config.Modules.FirstOrDefault(m => m.DisplayName == keyValue.Key);

                if (existingModule == null)
                {
                    existingModule = new Module { DisplayName = keyValue.Key, IsActive = true, InitAction = keyValue.Value };
                    _config.Modules.Add(existingModule);
                }
                else
                {
                    existingModule.InitAction = keyValue.Value;
                }
            }
        }

        private void InitModules()
        {
            var tabIndex = 0;

            var _ = tcMain.Handle;

            foreach (var module in _config.Modules.Where(m => m.IsActive))
            {
                var control = module.InitAction();

                if (control == null)
                {
                    continue;
                }

                var tabPage = new TabPage(module.DisplayName);
                tcMain.TabPages.Insert(tabIndex, tabPage);
                //tcMain.TabPages.Add(tabPage);

                tabPage.Controls.Add(control);

                control.Size = tabPage.ClientSize;
                control.BackColor = SystemColors.Window;

                tabIndex++;
            }

            tcMain.SelectedIndex = 0;
        }

        private void UpdateServiceInfo()
        {
            var text = "Use Windows Service";
            btnStartStopService.Enabled = true;

            if (Utils.IsServiceRunning())
            {
                text += " (running)";
                btnStartStopService.Text = "Stop";
            }
            else if (Utils.IsServiceInstalled())
            {
                text += " (installed, not running)";
                btnStartStopService.Text = "Start";
            }
            else
            {
                text += " (not installed)";
                btnStartStopService.Text = "Start";
                btnStartStopService.Enabled = false;
            }

            rbElevationService.Text = text;
        }

        private UserControl InitNvService()
        {
            try
            {
                //throw new Exception("bla");
                Logger.Debug("Initializing NVIDIA...");
                var appContextProvider = Program.ServiceProvider.GetRequiredService<AppContextProvider>();
                _serviceManager.NvService = new NvService(appContextProvider);
                Logger.Debug("Initializing NVIDIA...Done.");

                _nvPanel = new NvPanel(_serviceManager.NvService, _trayIcon, Handle, appContextProvider);
                _nvPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

                InitShortcut(NvPanel.SHORTCUTID_NVQA, _config.NvQuickAccessShortcut, _serviceManager.NvService.ToggleQuickAccessForm);

                return _nvPanel;
            }
            catch (Exception ex)
            {
                //Logger.Error("Error initializing NvService: " + e.ToLogString());
                Logger.Debug($"No NVIDIA device detected: {ex.ToLogString()}");

                return null;
            }
        }

        private UserControl InitAmdService()
        {
            try
            {
                var appContextProvider = Program.ServiceProvider.GetRequiredService<AppContextProvider>();
                _serviceManager.AmdService = new AmdService(appContextProvider);

                _amdPanel = new AmdPanel(_serviceManager.AmdService, _trayIcon, Handle);
                _amdPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

                InitShortcut(AmdPanel.SHORTCUTID_AMDQA, _config.AmdQuickAccessShortcut, _serviceManager.AmdService.ToggleQuickAccessForm);

                return _amdPanel;
            }
            catch (Exception)
            {
                //Logger.Error("Error initializing AmdService: " + e.ToLogString());
                Logger.Debug("No AMD device detected");

                return null;
            }
        }

        private UserControl InitGameService()
        {
            try
            {
                _serviceManager.GameService = Program.ServiceProvider.GetRequiredService<GameService>();

                _gamePanel = new GamePanel(_serviceManager.GameService, _serviceManager.NvService, _serviceManager.AmdService, _serviceManager.LgService, _trayIcon, Handle);
                _gamePanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

                InitShortcut(GamePanel.SHORTCUTID_GAMEQA, _config.GameQuickAccessShortcut, _serviceManager.GameService.ToggleQuickAccessForm);

                return _gamePanel;
            }
            catch (Exception e)
            {
                Logger.Error("Error initializing GameService: " + e.ToLogString());

                return null;
            }
        }

        private void InitShortcut(int shortcutId, string shortcut, Action action)
        {
            _shortcuts.Add(shortcutId, action);

            if (!string.IsNullOrEmpty(shortcut))
            {
                Utils.RegisterShortcut(Handle, shortcutId, shortcut);
            }
        }

        private UserControl InitLgService()
        {
            try
            {
                _serviceManager.LgService = Program.ServiceProvider.GetRequiredService<LgService>();

                _lgPanel = new LgPanel(_serviceManager.LgService, _serviceManager.NvService, _serviceManager.AmdService, _trayIcon, Handle);
                _lgPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

                _lgPanel.Init();

                InitShortcut(LgPanel.SHORTCUTID_GAMEBAR, _serviceManager.LgService.Config.GameBarShortcut, _lgPanel.ToggleGameBar);
                InitShortcut(LgPanel.SHORTCUTID_LGQA, _serviceManager.LgService.Config.QuickAccessShortcut, _serviceManager.LgService.ToggleQuickAccessForm);

                return _lgPanel;

                // New shortcut manager, not working yet
                //if (_keyboardHookManager == null)
                //{
                //    _keyboardHookManager = new KeyboardHookManager();
                //    _keyboardHookManager.Start();
                //}
                //_keyboardHookManager.RegisterHotkey(NonInvasiveKeyboardHookLibrary.ModifierKeys.Control | NonInvasiveKeyboardHookLibrary.ModifierKeys.Alt, (int)Keys.F9, new Action(TestKey));
            }
            catch (Exception e)
            {
                Logger.Error("Error initializing LgService: " + e.ToLogString());

                return null;
            }
        }

        private UserControl InitSamsungService()
        {
            try
            {
                _serviceManager.SamsungService = Program.ServiceProvider.GetRequiredService<SamsungService>();

                //_serviceManager.SamsungService.Init();

                _samsungPanel = new SamsungPanel(_serviceManager.SamsungService, _serviceManager.NvService, _serviceManager.AmdService, Handle);
                _samsungPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

                _samsungPanel.Init();

                InitShortcut(SamsungPanel.SHORTCUTID_SAMSUNGQA, _serviceManager.SamsungService.Config.QuickAccessShortcut, _serviceManager.SamsungService.ToggleQuickAccessForm);

                return _samsungPanel;
            }
            catch (Exception e)
            {
                Logger.Error("Error initializing SamsungService: " + e.ToLogString());

                return null;
            }
        }

        private void TestKey()
        {
            BeginInvoke(async () =>
            {
                var preset = _serviceManager.LgService.GetPresets().FirstOrDefault(p => p.name == "Backlight 20");

                await _serviceManager.LgService.ApplyPreset(preset);
            });
        }

        private void InitInfo()
        {
            _currentVersionInfo = FileVersionInfo.GetVersionInfo(Application.ExecutablePath);

            Text = Application.ProductName + " " + Application.ProductVersion;

            if (Utils.IsAdministrator())
            {
                Text += " (administrator)";
            }

            lblInfo.Text = Text + " - " + _currentVersionInfo.LegalCopyright;

            lbPlugins.Items.Add("lgtv.net by gr4b4z");
            lbPlugins.Items.Add("Newtonsoft.Json by James Newton-King");
            lbPlugins.Items.Add("NLog by Jarek Kowalski, Kim Christensen, Julian Verdurmen");
            lbPlugins.Items.Add("NvAPIWrapper.Net by Soroush Falahati");
            lbPlugins.Items.Add("NWin32 by zmjack");
            lbPlugins.Items.Add("TaskScheduler by David Hall");
            lbPlugins.Items.Add("NVIDIA Profile Inspector by Orbmu2k");
            lbPlugins.Items.Add("NvidiaML wrapper by LibreHardwareMonitor");
            lbPlugins.Items.Add("Novideo sRGB by ledoge");
        }

        private void OpenForm(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            Activate();
        }

        void Exit(object sender, EventArgs e)
        {
            UserExit = true;
            Close();
        }

        void Restart(object sender, EventArgs e)
        {
            Program.Restart();
        }

        private void trayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void trayIconBalloonTip_Clicked(object sender, EventArgs e)
        {
            if (_updateHtmlUrl != null)
            {
                Utils.StartProcess(_updateHtmlUrl);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!(SystemShutdown || EndSession || UserExit) && _config.MinimizeOnClose)
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;
                UserExit = false;
                return;
            }

            GlobalSave();

            if (SystemShutdown)
            {
                Logger.Debug($"MainForm_FormClosing: SystemShutdown");

                _powerEventDispatcher.SendEvent(PowerEventDispatcher.Event_Shutdown);
            }
        }

        private void GlobalSave()
        {
            _serviceManager.Save();

            _nvPanel?.Save();
            _amdPanel?.Save();
            _lgPanel?.Save();
            _gamePanel?.Save();

            SaveConfig();
        }

        private Control FindFocusedControl()
        {
            ContainerControl container = this;
            Control control = null;
            while (container != null)
            {
                control = container.ActiveControl;
                container = control as ContainerControl;
            }

            return control;
        }

        private bool IsShortcutControlFocused()
        {
            if (Form.ActiveForm?.GetType().Name.Equals("MessageForm") == true)
            {
                return true;
            }
            var control = FindFocusedControl();
            return control?.Name.Contains("Shortcut") ?? false;
        }

        protected override void WndProc(ref Message m)
        {
            // 5. Catch when a HotKey is pressed !
            if (m.Msg == NativeConstants.WM_HOTKEY && !IsShortcutControlFocused())
            {
                var id = m.WParam.ToInt32();

                if (_shortcuts.TryGetValue(id, out var action))
                {
                    action();
                }
                // 6. Handle what will happen once a respective hotkey is pressed
                else
                {
                    var preset = _serviceManager.NvService?.GetPresets().FirstOrDefault(x => x.id == id);
                    if (preset != null)
                    {
                        _nvPanel?.ApplyNvPreset(preset);
                    }

                    var amdPreset = _serviceManager.AmdService?.GetPresets().FirstOrDefault(x => x.id == id);
                    if (amdPreset != null)
                    {
                        _amdPanel?.ApplyAmdPreset(amdPreset);
                    }

                    var lgPreset = _serviceManager.LgService?.GetPresets().FirstOrDefault(x => x.id == id);
                    if (lgPreset != null)
                    {
                        _lgPanel?.ApplyLgPreset(lgPreset);
                    }

                    var samsungPreset = _serviceManager.SamsungService?.GetPresets().FirstOrDefault(x => x.id == id);
                    if (samsungPreset != null)
                    {
                        _samsungPanel?.ApplyPreset(samsungPreset);
                    }
                }
            }
            else if (m.Msg == NativeConstants.WM_QUERYENDSESSION)
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
                OpenForm(this, EventArgs.Empty);
            }
            else if (m.Msg == NativeConstants.WM_SYSCOMMAND)
            {
                //Logger.Debug($"WM_SYSCOMMAND: {m.WParam.ToInt32()}");

                //if (m.WParam.ToInt32() == NativeConstants.SC_MONITORPOWER)
                //{

                //}
            }
            else if (m.Msg == NativeConstants.WM_POWERBROADCAST)
            {
                //Logger.Debug($"WM_POWERBROADCAST: {m.WParam.ToInt32()}");

                if (m.WParam.ToInt32() == WinApi.PBT_POWERSETTINGCHANGE)
                {
                    var ps = Marshal.PtrToStructure<WinApi.POWERBROADCAST_SETTING>(m.LParam);

                    var power = (WindowsMonitorPowerSetting)ps.Data;

                    Logger.Debug($"PBT_POWERSETTINGCHANGE: {power}");

                    _powerEventDispatcher.SendEvent(power == WindowsMonitorPowerSetting.Off ? PowerEventDispatcher.Event_MonitorOff : PowerEventDispatcher.Event_MonitorOn);
                }
            }

            base.WndProc(ref m);
        }

        private void StartScreenSaver()
        {
            var screenSaver = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "scrnsave.scr");
            Process.Start("explorer.exe", screenSaver);
        }

        private void edtShortcut_KeyDown(object sender, KeyEventArgs e)
        {
            ((TextBox)sender).Text = Utils.FormatKeyboardShortcut(e);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            _trayIcon.Visible = false;

            if (_screenStateNotify != IntPtr.Zero)
            {
                WinApi.UnregisterPowerSettingNotification(_screenStateNotify);
                _screenStateNotify = IntPtr.Zero;
            }
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

        private void chkStartAfterLogin_CheckedChanged(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }
            var enabled = chkStartAfterLogin.Checked;
            RegisterScheduledTask(enabled);

            rbElevationAdmin.Enabled = enabled;
            if (!enabled && rbElevationAdmin.Checked)
            {
                rbElevationNone.Checked = true;
            }
        }

        private void LoadConfig()
        {
            chkStartMinimized.Checked = _config.StartMinimized;
            chkMinimizeOnClose.Checked = _config.MinimizeOnClose;
            chkMinimizeToSystemTray.Checked = _config.MinimizeToTray;
            chkCheckForUpdates.Checked = _config.CheckForUpdates;
            chkAutoInstallUpdates.Checked = _config.AutoInstallUpdates;
            chkAutoInstallUpdates.Enabled = _config.CheckForUpdates && _config.ElevationMethod == ElevationMethod.UseService;
            edtBlankScreenSaverShortcut.Text = _config.ScreenSaverShortcut;
            chkGdiScaling.Checked = _config.UseGdiScaling;
            chkOptionsUseDarkMode.Checked = _config.UseDarkMode;

            if (!string.IsNullOrEmpty(_config.ScreenSaverShortcut))
            {
                Utils.RegisterShortcut(Handle, SHORTCUTID_SCREENSAVER, _config.ScreenSaverShortcut);
            }

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
            _config.StartMinimized = chkStartMinimized.Checked;
            if (WindowState != FormWindowState.Minimized)
            {
                _config.FormWidth = Width;
                _config.FormHeight = Height;
            }

            Utils.WriteObject(Program.ConfigFilename, _config);
        }

        private async void MainForm_Shown(object sender, EventArgs e)
        {
            InitSelectedTab();
            await CheckForUpdates();

            CheckElevationMethod();
        }

        private void CheckElevationMethod()
        {
            if (Utils.IsAdministrator())
            {
                return;
            }

            if (_config.ElevationMethodAsked)
            {
                if (Debugger.IsAttached)
                {
                    return;
                }

                if (_config.ElevationMethod == ElevationMethod.UseService && !Utils.IsServiceInstalled() &&
                    (MessageForms.QuestionYesNo("The elevation method is set to Windows Service but it is not installed. Do you want to install it now?") == DialogResult.Yes))
                {
                    Utils.InstallService();
                }
                else if (_config.ElevationMethod == ElevationMethod.UseService && !Utils.IsServiceRunning() &&
                    (MessageForms.QuestionYesNo("The elevation method is set to Windows Service but it is not running. Do you want to start it now?") == DialogResult.Yes))
                {
                    Utils.StartService();
                }

                return;
            }

            var result = MessageForms.QuestionYesNo(Utils.ELEVATION_MSG + @"

Do you want to install and start the Windows Service now? You can always change this on the Options-tab page.

NOTE: installing the service may cause a User Account Control popup.");
            if (result == DialogResult.Yes)
            {
                SetElevationMethod(ElevationMethod.UseService);
            }

            _config.ElevationMethodAsked = true;
        }

        protected override void SetVisibleCore(bool value)
        {
            if (!_setVisibleCalled && _config.StartMinimized && !Debugger.IsAttached)
            {
                _setVisibleCalled = true;
                if (_config.MinimizeToTray)
                {
                    value = false;
                }
                else
                {
                    WindowState = FormWindowState.Minimized;
                }
            }
            if (!IsDisposed)
            {
                base.SetVisibleCore(value);
            }
        }

        private void edtShortcut_KeyUp(object sender, KeyEventArgs e)
        {
            Utils.HandleKeyboardShortcutUp(e);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitSelectedTab();
        }

        private void InitSelectedTab()
        {
            if (tcMain.SelectedTab == tabInfo)
            {
                LoadInfo();
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
            if (chkModules.Items.Count == 0)
            {
                foreach (var module in _config.Modules)
                {
                    chkModules.Items.Add(module.DisplayName, module.IsActive);
                }
            }

            if (_serviceManager.NvService != null)
            {
                if (_nvDitherPanel == null)
                {
                    _nvDitherPanel = new NvDitherPanel(_serviceManager.NvService);

                    tabOptions.Controls.Add(_nvDitherPanel);

                    _nvDitherPanel.Top = grpGeneralOptions.Top;
                    _nvDitherPanel.Left = grpGeneralOptions.Left + grpGeneralOptions.Width + 4;
                    _nvDitherPanel.Width = tabOptions.Width - _nvDitherPanel.Left - 4;

                    _nvDitherPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                }
                else
                {
                    _nvDitherPanel.UpdateInfo();
                }
            }

            _initialized = false;
            try
            {
                var _ = _config.ElevationMethod switch
                {
                    ElevationMethod.None => rbElevationNone.Checked = true,
                    ElevationMethod.RunAsAdmin => rbElevationAdmin.Checked = true,
                    ElevationMethod.UseService => rbElevationService.Checked = true,
                    ElevationMethod.UseElevatedProcess => rbElevationProcess.Checked = true,
                    _ => false
                };
            }
            finally
            {
                _initialized = true;
            }

            UpdateServiceInfo();
        }

        private void UpdateTrayMenu(ToolStripMenuItem menu, IEnumerable<PresetBase> presets, EventHandler eventHandler)
        {
            menu.DropDownItems.Clear();

            foreach (var preset in presets)
            {
                var name = preset.GetTextForMenuItem();
                var keys = Keys.None;

                if (!string.IsNullOrEmpty(preset.shortcut))
                {
                    name += "        " + preset.shortcut;
                    //keys = Utils.ShortcutToKeys(preset.shortcut);
                }

                var item = new ToolStripMenuItem(name, null, null, keys);
                item.Tag = preset;
                item.Click += eventHandler;
                item.ForeColor = FormUtils.MenuItemForeColor;
                menu.DropDownItems.Add(item);
            }
        }

        private async void TrayMenuItemNv_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            var preset = (NvPreset)item.Tag;

            await _nvPanel?.ApplyNvPreset(preset);
        }

        private async void TrayMenuItemAmd_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            var preset = (AmdPreset)item.Tag;

            await _amdPanel?.ApplyAmdPreset(preset);
        }

        private async void TrayMenuItemGame_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            var preset = (GamePreset)item.Tag;

            await _gamePanel?.ApplyGamePreset(preset);
        }

        private void trayIconContextMenu_Popup(object sender, EventArgs e)
        {
            _nvTrayMenu.Visible = _serviceManager.NvService != null;
            if (_nvTrayMenu.Visible)
            {
                var presets = _serviceManager.NvService.GetPresets().Where(x => x.applyColorData || x.applyDithering || x.applyHDR || x.applyRefreshRate || x.applyResolution || x.applyDriverSettings);

                UpdateTrayMenu(_nvTrayMenu, presets, TrayMenuItemNv_Click);
            }

            _novideoTrayMenu.Visible = _serviceManager.NvService != null && MainWindow.IsInitialized();
            if (_novideoTrayMenu.Visible)
            {
                MainWindow.UpdateContextMenu(_novideoTrayMenu);
            }

            _amdTrayMenu.Visible = _serviceManager.AmdService != null;
            if (_amdTrayMenu.Visible)
            {
                var presets = _serviceManager.AmdService.GetPresets().Where(x => x.applyColorData || x.applyDithering || x.applyHDR || x.applyRefreshRate);

                UpdateTrayMenu(_amdTrayMenu, presets, TrayMenuItemAmd_Click);
            }

            _lgTrayMenu.Visible = _serviceManager.LgService != null;
            if (_lgTrayMenu.Visible)
            {
                var presets = _serviceManager.LgService.GetPresets().Where(x => !string.IsNullOrEmpty(x.appId) || x.steps.Any());

                _lgTrayMenu.DropDownItems.Clear();

                UpdateTrayMenu(_lgTrayMenu, presets, TrayMenuItemLg_Click);
            }

            _samsungTrayMenu.Visible = _serviceManager.SamsungService != null;
            if (_samsungTrayMenu.Visible)
            {
                var presets = _serviceManager.SamsungService.GetPresets().Where(x => x.Steps.Any());

                _samsungTrayMenu.DropDownItems.Clear();

                UpdateTrayMenu(_samsungTrayMenu, presets, TrayMenuItemSamsung_Click);
            }

            _gameTrayMenu.Visible = _serviceManager.GameService != null;
            if (_gameTrayMenu.Visible)
            {
                var presets = _serviceManager.GameService.GetPresets();

                _gameTrayMenu.DropDownItems.Clear();

                UpdateTrayMenu(_gameTrayMenu, presets, TrayMenuItemGame_Click);
            }
        }

        private async void TrayMenuItemLg_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            var preset = (LgPreset)item.Tag;

            await _lgPanel?.ApplyLgPreset(preset);
        }

        private async void TrayMenuItemSamsung_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            var preset = (SamsungPreset)item.Tag;

            await _samsungPanel?.ApplyPreset(preset);
        }

        private void LoadInfo()
        {
            grpNVIDIAInfo.Visible = _serviceManager.NvService != null;
        }

        private void btnRefreshNVIDIAInfo_Click(object sender, EventArgs e)
        {
            if (_serviceManager.NvService != null)
            {
                tvNVIDIAInfo.Nodes.Clear();
                var displays = Display.GetDisplays();
                for (var i = 0; i < displays.Length; i++)
                {
                    var display = displays[i];
                    var node = TreeNodeBuilder.CreateTree(display, $"Display[{i}]");
                    tvNVIDIAInfo.Nodes.Add(node);
                }
            }
        }

        private void btnSetShortcutScreenSaver_Click(object sender, EventArgs e)
        {
            var shortcut = edtBlankScreenSaverShortcut.Text.Trim();

            if (!Utils.ValidateShortcut(shortcut))
            {
                return;
            }

            var oldShortcut = _config.ScreenSaverShortcut;

            var clear = !string.IsNullOrEmpty(oldShortcut);

            _config.ScreenSaverShortcut = shortcut;

            Utils.RegisterShortcut(Handle, SHORTCUTID_SCREENSAVER, shortcut, clear);
        }

        private void chkMinimizeOnClose_CheckedChanged(object sender, EventArgs e)
        {
            _config.MinimizeOnClose = chkMinimizeOnClose.Checked;
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            if (tcMain.SelectedTab?.Controls.Count > 0 && tcMain.SelectedTab.Controls[0] is IModulePanel panel)
            {
                panel.UpdateInfo();
            }
        }

        private void AfterInitialized()
        {
            _nvPanel?.AfterInitialized();
            _amdPanel?.AfterInitialized();
            if (_trayIcon.Visible)
            {
                var _ = CheckForUpdates();
            }
        }

        private async Task CheckForUpdates()
        {
            if (!_config.CheckForUpdates || _checkedForUpdates || Debugger.IsAttached)
            {
                return;
            }

            _checkedForUpdates = true;

            await Utils.GetRestJsonAsync("https://api.github.com/repos/maassoft/colorcontrol/releases/latest", InitHandleCheckForUpdates);
        }

        private void InitHandleCheckForUpdates(dynamic latest)
        {
            BeginInvoke(() => HandleCheckForUpdates(latest));
        }

        private void HandleCheckForUpdates(dynamit)
        {
            if (latest?.tag_name == null)
            {
                return;
            }

            var currentVersion = Application.ProductVersion;
            var cvParts = currentVersion.Split(".");

            var newVersion = (string)latest.tag_name.Value.Substring(1);
            var nvParts = newVersion.Split(".");

            bool CompareVersions()
            {
                var result = true;

                for (var i = 0; i < nvParts.Length; i++)
                {
                    var part = nvParts[i];
                    var cvPart = cvParts[i];

                    if (Utils.ParseInt(part) >= Utils.ParseInt(cvPart))
                    {
                        continue;
                    }

                    result = false;
                    break;
                }

                return result;
            }

            if (nvParts.Length != cvParts.Length || CompareVersions())
            {
                _updateHtmlUrl = latest.html_url.Value;

                if (latest.assets != null && Utils.IsServiceRunning())
                {
                    var asset = latest.assets[0];
                    _downloadUrl = asset.browser_download_url.Value;

                    if (_config.AutoInstallUpdates)
                    {
                        InstallUpdate(_downloadUrl);

                        return;
                    }

                    btnUpdate.Visible = true;

                    if (_trayIcon.Visible)
                    {
                        _trayIcon.ShowBalloonTip(5000, "Update available", $"Version {newVersion} is available. Click on the Update-button to update", ToolTipIcon.Info);
                    }
                    else
                    {
                        MessageForms.InfoOk($"New version {newVersion} is available. Click on the Update-button to update", "Update available", $"https://github.com/Maassoft/ColorControl/releases/tag/v{newVersion}");
                    }

                    return;
                }

                if (_trayIcon.Visible)
                {
                    _trayIcon.ShowBalloonTip(5000, "Update available", $"Version {newVersion} is available. Click to open the GitHub page", ToolTipIcon.Info);
                }
                else
                {
                    MessageForms.InfoOk($"New version {newVersion} is available. Click on the Help-button to open the GitHub page.", "Update available", $"https://github.com/Maassoft/ColorControl/releases/tag/v{newVersion}");
                }
            }
        }

        private void InstallUpdate(string downloadUrl)
        {
            var message = new SvcInstallUpdateMessage
            {
                DownloadUrl = downloadUrl,
                ClientPath = new FileInfo(Application.ExecutablePath).Directory.FullName
            };

            var result = PipeUtils.SendMessage(message, 30000);

            if (result != null && !result.Result)
            {
                MessageForms.ErrorOk($"Error while updating: {result.ErrorMessage}");

                return;
            }

            PipeUtils.SendMessage(SvcMessageType.RestartAfterUpdate);

            if (MessageForms.QuestionYesNo("Update installed successfully. Do you want to restart the application?") == DialogResult.Yes)
            {
                Program.Restart();
            }
            else
            {
                _downloadUrl = null;
                btnUpdate.Visible = true;
            }
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            GlobalSave();
        }

        private bool ShortCutExists(string shortcut, int presetId = 0)
        {
            return
                (_serviceManager.NvService?.GetPresets().Any(x => x.id != presetId && shortcut.Equals(x.shortcut)) ?? false) ||
                (_serviceManager.AmdService?.GetPresets().Any(x => x.id != presetId && shortcut.Equals(x.shortcut)) ?? false) ||
                (_serviceManager.LgService?.GetPresets().Any(x => x.id != presetId && shortcut.Equals(x.shortcut)) ?? false);
        }

        private void chkMinimizeToSystemTray_CheckedChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                _config.MinimizeToTray = chkMinimizeToSystemTray.Checked;
                _trayIcon.Visible = _config.MinimizeToTray;
            }
        }

        private void chkCheckForUpdates_CheckedChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                _config.CheckForUpdates = chkCheckForUpdates.Checked;
                chkAutoInstallUpdates.Enabled = _config.CheckForUpdates && _config.ElevationMethod == ElevationMethod.UseService;
            }
        }

        private void chkGdiScaling_CheckedChanged(object sender, EventArgs e)
        {
            _config.UseGdiScaling = chkGdiScaling.Checked;
        }

        private void MainForm_ResizeBegin(object sender, EventArgs e)
        {
            SuspendLayout();
        }

        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            ResumeLayout(true);
        }

        private void rbElevationNone_CheckedChanged(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }


            if (sender is not RadioButton button || !button.Checked)
            {
                return;
            }

            SetElevationMethod((ElevationMethod)Utils.ParseInt((string)button.Tag));
        }

        private void SetElevationMethod(ElevationMethod elevationMethod)
        {
            _config.ElevationMethod = elevationMethod;

            Utils.UseDedicatedElevatedProcess = _config.ElevationMethod == ElevationMethod.UseElevatedProcess;

            var _ = _config.ElevationMethod switch
            {
                ElevationMethod.None => SetElevationMethodNone(),
                ElevationMethod.RunAsAdmin => SetElevationMethodRunAsAdmin(),
                ElevationMethod.UseService => SetElevationMethodUseService(),
                ElevationMethod.UseElevatedProcess => SetElevationMethodUseElevatedProcess(),
                _ => true
            };

            UpdateServiceInfo();
        }

        private bool SetElevationMethodNone()
        {
            if (chkStartAfterLogin.Enabled)
            {
                RegisterScheduledTask();
            }

            Utils.UninstallService();

            return true;
        }

        private bool SetElevationMethodRunAsAdmin()
        {
            if (chkStartAfterLogin.Enabled)
            {
                RegisterScheduledTask(false);
                RegisterScheduledTask();
            }

            Utils.UninstallService();

            return true;
        }

        private bool SetElevationMethodUseService()
        {
            Utils.InstallService();

            if (Utils.IsServiceInstalled())
            {
                MessageForms.InfoOk("Service installed successfully.");
                return true;
            }

            MessageForms.ErrorOk("Service could not be installed. Check the logs.");

            return false;
        }

        private bool SetElevationMethodUseElevatedProcess()
        {
            if (chkStartAfterLogin.Enabled)
            {
                RegisterScheduledTask();
            }

            Utils.UninstallService();

            return true;
        }

        private void RegisterScheduledTask(bool enabled = true)
        {
            Utils.RegisterTask(Program.TS_TASKNAME, enabled, _config.ElevationMethod == ElevationMethod.RunAsAdmin ? Microsoft.Win32.TaskScheduler.TaskRunLevel.Highest : Microsoft.Win32.TaskScheduler.TaskRunLevel.LUA);
        }

        private void btnElevationInfo_Click(object sender, EventArgs e)
        {
            MessageForms.InfoOk(Utils.ELEVATION_MSG + @$"

NOTE: if ColorControl itself already runs as administrator this is indicated in the title: ColorControl {Application.ProductVersion} (administrator).

Currently ColorControl is {(Utils.IsAdministrator() ? "" : "not ")}running as administrator.
");
        }

        private async void btnStartStopService_Click(object sender, EventArgs e)
        {
            btnStartStopService.Enabled = false;
            try
            {
                var start = btnStartStopService.Text == "Start";

                if (start)
                {
                    Utils.StartService();
                }
                else
                {
                    Utils.StopService();
                }

                var wait = 1500;

                while (wait > 0)
                {
                    await Task.Delay(100);

                    if (start == Utils.IsServiceRunning())
                    {
                        break;
                    }

                    wait -= 100;
                }
            }
            finally
            {
                btnStartStopService.Enabled = true;
            }

            UpdateServiceInfo();
        }

        private async void MainForm_Click(object sender, EventArgs e)
        {
            //_powerEventDispatcher.SendEvent(PowerEventDispatcher.Event_Shutdown);
            //PipeUtils.SendMessage(SvcMessageType.RestartAfterUpdate);
            //Program.Restart();
            //Environment.Exit(0);
            //InstallUpdate("");
            //await Test();

            //Utils.SetBrightness(Handle);
        }

        private async Task Test()
        {
        }

        private void SetTheme(bool toDark)
        {
            this.UpdateTheme(toDark);

            WinApi.RefreshImmersiveColorPolicyState();

            DarkModeUtils.SetContextMenuForeColor(_trayIcon.ContextMenuStrip, FormUtils.CurrentForeColor);

            InitSelectedTab();
        }

        private void chkAutoInstallUpdates_CheckedChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                _config.AutoInstallUpdates = chkAutoInstallUpdates.Checked;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (_downloadUrl != null)
            {
                InstallUpdate(_downloadUrl);

                return;
            }

            Program.Restart();
        }

        internal void CloseForRestart()
        {
            Hide();
            _trayIcon.Visible = false;
        }

        private void chkFixChromeFonts_CheckedChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                _config.FixChromeFonts = chkFixChromeFonts.Checked;

                var param = chkFixChromeFonts.Checked ? StartUpParams.ActivateChromeFontFixParam : StartUpParams.DeactivateChromeFontFixParam;

                var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                Utils.ExecuteElevated($"{param} {folder}");
            }
        }

        private void chkModules_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            for (var i = 0; i < chkModules.Items.Count; i++)
            {
                var item = chkModules.Items[i].ToString();

                var module = _config.Modules.First(m => m.DisplayName == item.ToString());

                module.IsActive = chkModules.GetItemChecked(i);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((keyData == (Keys.Control | Keys.PageUp) || keyData == (Keys.Control | Keys.PageDown)) && IsShortcutControlFocused())
            {
                var control = FindFocusedControl() as TextBox;

                if (control != null)
                {
                    var keyEvent = new KeyEventArgs(keyData);

                    control.Text = Utils.FormatKeyboardShortcut(keyEvent);
                }

                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void chkOptionsUseDarkMode_CheckedChanged(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            _config.UseDarkMode = chkOptionsUseDarkMode.Checked;

            SetTheme(_config.UseDarkMode);
        }

        private void btnOptionsAdvanced_Click(object sender, EventArgs e)
        {
            var processPollingIntervalField = new MessageForms.FieldDefinition
            {
                FieldType = MessageForms.FieldType.Numeric,
                Label = "Polling interval of process monitor (milliseconds).",
                SubLabel = "Decreasing this delay may execute triggered presets sooner but can cause a higher CPU load",
                MinValue = 50,
                MaxValue = 5000,
                Value = _config.ProcessMonitorPollingInterval
            };

            var values = MessageForms.ShowDialog("Advanced settings", new[] { processPollingIntervalField });

            if (values?.Any() != true)
            {
                return;
            }

            _config.ProcessMonitorPollingInterval = processPollingIntervalField.ValueAsInt;
        }

        private void btnOptionsLog_Click(object sender, EventArgs e)
        {
            LogWindow.CreateAndShow();
        }
    }
}