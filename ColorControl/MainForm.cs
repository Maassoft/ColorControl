using ATI.ADL;
using ColorControl.Common;
using ColorControl.Forms;
using ColorControl.Native;
using ColorControl.Services.AMD;
using ColorControl.Services.Common;
using ColorControl.Services.GameLauncher;
using ColorControl.Services.LG;
using ColorControl.Services.NVIDIA;
using ColorControl.Svc;
using LgTv;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using nspector;
using nspector.Common;
using nspector.Common.Meta;
using NStandard;
using NvAPIWrapper.Display;
using NvAPIWrapper.GPU;
using NvAPIWrapper.Native.Display;
using NWin32;
using NWin32.NativeTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
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
        private static int SHORTCUTID_GAMEBAR = -101;
        private static int SHORTCUTID_NVQA = -200;
        private static int SHORTCUTID_AMDQA = -201;
        private static int SHORTCUTID_LGQA = -202;
        private static int SHORTCUTID_GAMEQA = -203;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private string _dataDir;

        private NvService _nvService;
        private string _lastDisplayRefreshRates = string.Empty;

        private NotifyIcon _trayIcon;
        private bool _initialized = false;
        private bool _disableEvents = false;
        private Config _config;
        private bool _setVisibleCalled = false;

        private LgService _lgService;

        private ToolStripMenuItem _nvTrayMenu;
        private ToolStripMenuItem _amdTrayMenu;
        private ToolStripMenuItem _lgTrayMenu;
        private ToolStripMenuItem _gameTrayMenu;

        private StartUpParams StartUpParams { get; }
        public string _lgTabMessage { get; private set; }

        private AmdService _amdService;
        private bool _skipResize;
        private FileVersionInfo _currentVersionInfo;
        private bool _checkedForUpdates = false;
        private string _updateHtmlUrl;
        private bool _updatingDitherSettings;
        private string _downloadUrl;

        private LgGameBar _gameBarForm;

        private GameService _gameService;

        //private KeyboardHookManager _keyboardHookManager;
        private IntPtr _screenStateNotify;

        private Dictionary<int, Action> _shortcuts = new();

        public MainForm(AppContext appContext)
        {
            InitializeComponent();
            StartUpParams = appContext.StartUpParams;

            _dataDir = Program.DataDir;
            _config = Program.Config;

            LoadConfig();

            MessageForms.MainForm = this;

            _nvTrayMenu = new ToolStripMenuItem("NVIDIA presets");
            _amdTrayMenu = new ToolStripMenuItem("AMD presets");
            _lgTrayMenu = new ToolStripMenuItem("LG presets");
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
                    _amdTrayMenu,
                    _lgTrayMenu,
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

            //Utils.StartService();

            InitLgService();
            InitNvService();
            InitAmdService();
            InitGameService();

            InitInfo();
            UpdateServiceInfo();

            UserSessionInfo.Install();
            _screenStateNotify = WinApi.RegisterPowerSettingNotification(Handle, ref Utils.GUID_CONSOLE_DISPLAY_STATE, 0);

            //Scale(new SizeF(1.25F, 1.25F));

            _initialized = true;

            //Task.Run(() => Utils.StartPipeAsync());

            AfterInitialized();
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

        private void InitNvService()
        {
            try
            {
                //throw new Exception("bla");
                Logger.Debug("Initializing NVIDIA...");
                var appContextProvider = Program.ServiceProvider.GetRequiredService<AppContextProvider>();
                _nvService = new NvService(appContextProvider);
                FillNvPresets();
                Logger.Debug("Initializing NVIDIA...Done.");

                InitSortState(lvNvPresets, _config.NvPresetsSortState);

                InitShortcut(SHORTCUTID_NVQA, _config.NvQuickAccessShortcut, _nvService.ToggleQuickAccessForm);

                _nvService.AfterApplyPreset += NvServiceAfterApplyPreset;
            }
            catch (Exception ex)
            {
                //Logger.Error("Error initializing NvService: " + e.ToLogString());
                Logger.Debug($"No NVIDIA device detected: {ex.ToLogString()}");
                tcMain.TabPages.Remove(tabNVIDIA);
            }
        }

        private void NvServiceAfterApplyPreset(object sender, NvPreset preset)
        {
            UpdateDisplayInfoItems();
        }

        private void InitAmdService()
        {
            try
            {
                var appContextProvider = Program.ServiceProvider.GetRequiredService<AppContextProvider>();
                _amdService = new AmdService(appContextProvider);
                FillAmdPresets();

                InitSortState(lvAmdPresets, _config.AmdPresetsSortState);

                InitShortcut(SHORTCUTID_AMDQA, _config.AmdQuickAccessShortcut, _amdService.ToggleQuickAccessForm);

                _amdService.AfterApplyPreset += AmdServiceAfterApplyPreset;
            }
            catch (Exception)
            {
                //Logger.Error("Error initializing AmdService: " + e.ToLogString());
                Logger.Debug("No AMD device detected");
                tcMain.TabPages.Remove(tabAMD);
            }
        }

        private void InitGameService()
        {
            try
            {
                var appContextProvider = Program.ServiceProvider.GetRequiredService<AppContextProvider>();
                _gameService = new GameService(appContextProvider, HandleExternalServiceForLgDevice);
                FillGamePresets();

                InitSortState(lvGamePresets, _config.GamePresetsSortState);

                InitShortcut(SHORTCUTID_GAMEQA, _config.GameQuickAccessShortcut, _gameService.ToggleQuickAccessForm);
            }
            catch (Exception e)
            {
                Logger.Error("Error initializing GameService: " + e.ToLogString());
                tcMain.TabPages.Remove(tabGameLauncher);
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

        private void AmdServiceAfterApplyPreset(object sender, AmdPreset preset)
        {
            UpdateDisplayInfoItemsAmd();
        }

        private void InitLgService()
        {
            try
            {
                var appContextProvider = Program.ServiceProvider.GetRequiredService<AppContextProvider>();

                _lgService = new LgService(appContextProvider, StartUpParams.RunningFromScheduledTask);
                _lgService.RefreshDevices(afterStartUp: true).ContinueWith((_) => BeginInvoke(() => AfterLgServiceRefreshDevices()));
                _lgService.SelectedDeviceChangedEvent += _lgService_SelectedDeviceChangedEvent;

                FillLgPresets();

                edtLgMaxPowerOnRetries.Value = _lgService.Config.PowerOnRetries;
                edtLgOptionShutdownDelay.Value = _lgService.Config.ShutdownDelay;
                edtLgDeviceFilter.Text = _lgService.Config.DeviceSearchKey;
                chkLgShowAdvancedActions.Checked = _lgService.Config.ShowAdvancedActions;
                chkLgSetSelectedDeviceByPowerOn.Checked = _lgService.Config.SetSelectedDeviceByPowerOn;

                InitSortState(lvLgPresets, _config.LgPresetsSortState);

                var values = Enum.GetValues(typeof(ButtonType));
                foreach (var button in values)
                {
                    var text = button.ToString();
                    if (text[0] == '_')
                    {
                        text = text.Substring(1);
                    }

                    var item = mnuLgRcButtons.DropDownItems.Add(text);
                    item.Click += miLgAddButton_Click;
                }

                _lgService.InstallEventHandlers();

                LgDevice.ExternalServiceHandler = HandleExternalServiceForLgDevice;

                InitShortcut(SHORTCUTID_GAMEBAR, _lgService.Config.GameBarShortcut, ToggleGameBar);
                InitShortcut(SHORTCUTID_LGQA, _config.LgQuickAccessShortcut, _lgService.ToggleQuickAccessForm);

                if (!string.IsNullOrEmpty(_lgService.Config.GameBarShortcut))
                {
                    // New shortcut manager, not working yet
                    //if (_keyboardHookManager == null)
                    //{
                    //    _keyboardHookManager = new KeyboardHookManager();
                    //    _keyboardHookManager.Start();
                    //}
                    //_keyboardHookManager.RegisterHotkey(NonInvasiveKeyboardHookLibrary.ModifierKeys.Control | NonInvasiveKeyboardHookLibrary.ModifierKeys.Alt, (int)Keys.F9, new Action(TestKey));

                    edtLgGameBarShortcut.Text = _lgService.Config.GameBarShortcut;
                }
            }
            catch (Exception e)
            {
                Logger.Error("Error initializing LgService: " + e.ToLogString());
            }
        }

        private void TestKey()
        {
            BeginInvoke(async () =>
            {
                var preset = _lgService.GetPresets().FirstOrDefault(p => p.name == "Backlight 20");

                await _lgService.ApplyPreset(preset);
            });
        }

        private void _lgService_SelectedDeviceChangedEvent(object sender, EventArgs e)
        {
            BeginInvoke(() => SetLgDevicesSelectedIndex(sender));
        }

        private void SetLgDevicesSelectedIndex(object sender)
        {
            if (sender == null)
            {
                cbxLgDevices.SelectedIndex = -1;
                return;
            }

            cbxLgDevices.SelectedIndex = cbxLgDevices.Items.IndexOf(sender);
        }

        private void InitSortState(ListView listView, ListViewSortState sortState)
        {
            var sorter = new ListViewColumnSorter();
            sorter.SortColumn = sortState.SortIndex;
            sorter.Order = sortState.SortOrder;
            listView.ListViewItemSorter = sorter;
        }

        private void Invoke()
        {
            BeginInvoke(() => ToggleGameBar());
        }

        private void AfterLgServiceRefreshDevices()
        {
            FillLgDevices();

            if (StartUpParams.ExecuteLgPreset)
            {
                var _ = _lgService.ApplyPreset(StartUpParams.LgPresetName);
            }
        }

        private bool HandleExternalServiceForLgDevice(string serviceName, string[] parameters)
        {
            if (string.IsNullOrEmpty(serviceName) || parameters.Length == 0)
            {
                return false;
            }

            if (_nvService != null && serviceName.Equals("NvPreset", StringComparison.OrdinalIgnoreCase))
            {
                return _nvService.ApplyPreset(parameters[0]);
            }
            if (_nvService != null && serviceName.Equals("GsyncEnabled", StringComparison.OrdinalIgnoreCase))
            {
                return _nvService.IsGsyncEnabled();
            }
            if (_amdService != null && serviceName.Equals("AmdPreset", StringComparison.OrdinalIgnoreCase))
            {
                return _amdService.ApplyPreset(parameters[0]);
            }
            if (_lgService != null && serviceName.Equals("LgPreset", StringComparison.OrdinalIgnoreCase))
            {
                Utils.WaitForTask(_lgService.ApplyPreset(parameters[0]));

                return true;
            }

            if (serviceName.Equals("StartProgram", StringComparison.OrdinalIgnoreCase))
            {
                Utils.StartProcess(parameters[0], parameters.Length > 1 ? string.Join(" ", parameters.Skip(1)) : null, setWorkingDir: true);

                return true;
            }

            return false;
        }

        private void InitInfo()
        {
            _currentVersionInfo = FileVersionInfo.GetVersionInfo(Path.GetFileName(Application.ExecutablePath));

            //if (ApplicationDeployment.IsNetworkDeployed)
            //{
            //    Text = Application.ProductName + " " + ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            //}
            //else
            //{
            Text = Application.ProductName + " " + Application.ProductVersion;
            //}

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

        private void FillNvPresets()
        {
            FormUtils.InitListView(lvNvPresets, NvPreset.GetColumnNames());

            UpdateDisplayInfoItems();

            foreach (var preset in _nvService.GetPresets())
            {
                AddOrUpdateItem(preset);
                Utils.RegisterShortcut(Handle, preset.id, preset.shortcut);
            }
        }

        private void FillAmdPresets()
        {
            FormUtils.InitListView(lvAmdPresets, AmdPreset.GetColumnNames());

            UpdateDisplayInfoItemsAmd();

            foreach (var preset in _amdService.GetPresets())
            {
                AddOrUpdateItemAmd(preset);
                Utils.RegisterShortcut(Handle, preset.id, preset.shortcut);
            }
        }

        private void FillGamePresets()
        {
            FormUtils.InitListView(lvGamePresets, GamePreset.GetColumnNames());

            foreach (var preset in _gameService.GetPresets())
            {
                AddOrUpdateItemGame(preset);
                Utils.RegisterShortcut(Handle, preset.id, preset.shortcut);
            }
        }

        private void UpdateDisplayInfoItems()
        {
            var displays = _nvService?.GetDisplayInfos();
            if (displays == null || Program.IsRestarting)
            {
                return;
            }

            var gpus = new List<PhysicalGPU>();

            var text = Program.TS_TASKNAME;
            foreach (var displayInfo in displays)
            {
                var display = displayInfo.Display;

                var id = Math.Abs((int)display.Handle.MemoryAddress.ToInt64());

                ListViewItem item = null;
                for (var i = 0; i < lvNvPresets.Items.Count; i++)
                {
                    item = lvNvPresets.Items[i];

                    if (item.Tag == null && item.ImageIndex == id)
                    {
                        break;
                    }
                    item = null;
                }

                if (item == null)
                {
                    item = lvNvPresets.Items.Add(display.Name);
                    item.ImageIndex = id;
                    item.Font = new Font(item.Font, item.Font.Style | FontStyle.Bold);
                    item.BackColor = Color.LightGray;
                }

                var values = displayInfo.Values;

                item.Text = values[0];
                for (var i = 1; i < values.Count; i++)
                {
                    if (item.SubItems.Count - 1 >= i)
                    {
                        item.SubItems[i].Text = values[i];
                    }
                    else
                    {
                        item.SubItems.Add(values[i]);
                    }
                }

                text += "\n" + displayInfo.InfoLine;

                if (!gpus.Contains(displayInfo.Display.DisplayDevice.PhysicalGPU))
                {
                    gpus.Add(displayInfo.Display.DisplayDevice.PhysicalGPU);
                }
            }

            UpdateNvGpuInfo(gpus);

            FormUtils.SetNotifyIconText(_trayIcon, text);
        }

        private void UpdateNvGpuInfo(List<PhysicalGPU> gpus = null)
        {
            gpus ??= _nvService.GetGPUs();

            cbxNvGPU.Items.Clear();
            cbxNvGPU.Items.AddRange(gpus.ToArray());
            if (cbxNvGPU.Items.Count > 0 && cbxNvGPU.SelectedIndex == -1)
            {
                cbxNvGPU.SelectedIndex = 0;
            }
        }

        private void UpdateDisplayInfoItemsAmd()
        {
            var displays = _amdService?.GetDisplayInfos();
            if (displays == null)
            {
                return;
            }

            var text = Program.TS_TASKNAME;
            foreach (var displayInfo in displays)
            {
                var display = displayInfo.Display;

                var id = display.DisplayID.DisplayPhysicalIndex;

                ListViewItem item = null;
                for (var i = 0; i < lvAmdPresets.Items.Count; i++)
                {
                    item = lvAmdPresets.Items[i];

                    if (item.Tag == null && item.ImageIndex == id)
                    {
                        break;
                    }
                    item = null;
                }

                if (item == null)
                {
                    item = lvAmdPresets.Items.Add(display.DisplayName);
                    item.ImageIndex = id;
                    item.Font = new Font(item.Font, item.Font.Style | FontStyle.Bold);
                    item.BackColor = Color.LightGray;
                }

                var values = displayInfo.Values;

                item.Text = values[0];
                for (var i = 1; i < values.Count; i++)
                {
                    if (item.SubItems.Count - 1 >= i)
                    {
                        item.SubItems[i].Text = values[i];
                    }
                    else
                    {
                        item.SubItems.Add(values[i]);
                    }
                }

                text += "\n" + displayInfo.InfoLine;
            }

            FormUtils.SetNotifyIconText(_trayIcon, text);
        }

        private void AddOrUpdateItem(NvPreset preset = null)
        {
            FormUtils.AddOrUpdateListItem(lvNvPresets, _nvService.GetPresets(), _config, preset);
        }

        private void AddOrUpdateItemAmd(AmdPreset preset = null)
        {
            FormUtils.AddOrUpdateListItem(lvAmdPresets, _amdService.GetPresets(), _config, preset);
        }

        private void AddOrUpdateItemGame(GamePreset preset = null)
        {
            FormUtils.AddOrUpdateListItem(lvGamePresets, _gameService.GetPresets(), _config, preset);
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            ApplySelectedNvPreset();
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

            if (SystemShutdown && _lgService != null)
            {
                Logger.Debug($"MainForm_FormClosing: SystemShutdown");

                var devices = _lgService.Devices.Where(d => d.PowerOffOnShutdown);

                _lgService.PowerOffDevices(devices);
            }
        }

        private void GlobalSave()
        {
            _nvService?.GlobalSave();
            _amdService?.GlobalSave();
            _lgService?.GlobalSave();
            _gameService?.GlobalSave();

            SaveSortState(lvNvPresets.ListViewItemSorter, _config.NvPresetsSortState);
            SaveSortState(lvAmdPresets.ListViewItemSorter, _config.AmdPresetsSortState);
            SaveSortState(lvLgPresets.ListViewItemSorter, _config.LgPresetsSortState);
            SaveSortState(lvGamePresets.ListViewItemSorter, _config.GamePresetsSortState);

            SaveConfig();
        }

        private void SaveSortState(IComparer comparer, ListViewSortState sortState)
        {
            if (!(comparer is ListViewColumnSorter sorter))
            {
                return;
            }

            sortState.SortOrder = sorter.Order;
            sortState.SortIndex = sorter.SortColumn;
        }

        private void lvNvPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            var preset = GetSelectedNvPreset();
            var enabled = preset != null;

            FormUtils.EnableControls(tabNVIDIA, enabled, new List<Control> { lvNvPresets, btnAddModesNv, btnChange, edtNvGpuInfo, btnNvSetClocks, btnNvSettings, edtNvOverclock });

            if (enabled)
            {
                edtNvPresetName.Text = preset.name;
                edtShortcut.Text = preset.shortcut;
                chkNvShowInQuickAccess.Checked = preset.ShowInQuickAccess;
            }
            else
            {
                edtNvPresetName.Text = string.Empty;
                edtShortcut.Text = string.Empty;
                chkNvShowInQuickAccess.Checked = false;
            }
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
                    var preset = _nvService?.GetPresets().FirstOrDefault(x => x.id == id);
                    if (preset != null)
                    {
                        ApplyNvPreset(preset);
                    }

                    var amdPreset = _amdService?.GetPresets().FirstOrDefault(x => x.id == id);
                    if (amdPreset != null)
                    {
                        ApplyAmdPreset(amdPreset);
                    }

                    var lgPreset = _lgService?.GetPresets().FirstOrDefault(x => x.id == id);
                    if (lgPreset != null)
                    {
                        ApplyLgPreset(lgPreset);
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

                if (m.WParam.ToInt32() == NativeConstants.SC_MONITORPOWER)
                {

                }
            }
            else if (m.Msg == NativeConstants.WM_POWERBROADCAST)
            {
                if (m.WParam.ToInt32() == WinApi.PBT_POWERSETTINGCHANGE)
                {
                    var ps = Marshal.PtrToStructure<WinApi.POWERBROADCAST_SETTING>(m.LParam);

                    Logger.Debug($"PBT_POWERSETTINGCHANGE: {ps.Data}");

                    _lgService?.PowerSettingChanged((LgService.WindowsPowerSetting)ps.Data);
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

        private void btnSetShortcut_Click(object sender, EventArgs e)
        {
            var shortcut = edtShortcut.Text.Trim();

            if (!Utils.ValidateShortcut(shortcut))
            {
                return;
            }

            var preset = GetSelectedNvPreset();

            var name = edtNvPresetName.Text.Trim();

            if (!string.IsNullOrEmpty(name) && _nvService.GetPresets().Any(x => x.id != preset.id && x.name != null && x.name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                MessageForms.WarningOk("The name must be unique.");
                return;
            }

            if (string.IsNullOrEmpty(name) && chkNvShowInQuickAccess.Checked)
            {
                MessageForms.WarningOk("A name must be entered before enabling Quick Access.");
                return;
            }

            var clear = !string.IsNullOrEmpty(preset.shortcut);

            preset.shortcut = shortcut;
            preset.name = name;
            preset.ShowInQuickAccess = chkNvShowInQuickAccess.Checked;

            AddOrUpdateItem();

            Utils.RegisterShortcut(Handle, preset.id, preset.shortcut, clear);
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

        private void ShowControls(Control parent, bool show = true, Control exclude = null)
        {
            for (var i = 0; i < parent.Controls.Count; i++)
            {
                var control = parent.Controls[i];
                if (control != exclude)
                {
                    control.Visible = show;
                }
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (_nvService == null)
            {
                ShowControls(tabNVIDIA, false, lblError);

                lblError.Text = "Error while initializing the NVIDIA-wrapper. You either don't have a NVIDIA GPU or it is disabled. NVIDIA controls will not be available.";
                lblError.Visible = true;
            }

            if (_amdService == null)
            {
                ShowControls(tabAMD, false, lblErrorAMD);

                lblErrorAMD.Text = "Error while initializing the ADL-wrapper. You either don't have a AMD GPU or it is disabled. AMD controls will not be available.";
                lblErrorAMD.Visible = true;
            }

            if (_lgService == null)
            {
                ShowControls(tabLG, false, lblLgError);
                lblLgError.Text = "Error while initializing the LG-controller. You either don't have a LG TV, it is disabled or a configuration file is corrupt.";
                lblLgError.Visible = true;
            }

            InitSelectedTab();
            CheckForUpdates();

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
            if (!_setVisibleCalled && _config.StartMinimized)
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

        private void edtShortcut_TextChanged(object sender, EventArgs e)
        {
            var text = edtShortcut.Text;

            var preset = GetSelectedNvPreset();

            if (preset == null || string.IsNullOrEmpty(text))
            {
                edtShortcut.ForeColor = SystemColors.WindowText;
            }
            else
            {
                edtShortcut.ForeColor = ShortCutExists(text, preset.id) ? Color.Red : SystemColors.WindowText;
            }
        }

        private NvPreset GetSelectedNvPreset(bool currentDisplay = false)
        {
            if (lvNvPresets.SelectedItems.Count > 0)
            {
                var item = lvNvPresets.SelectedItems[0];
                var result = (NvPreset)item.Tag;

                if (result == null && currentDisplay)
                {
                    result = _nvService.GetPresetForDisplay(item.SubItems[1].Text);
                }

                return result;
            }
            else
            {
                return null;
            }
        }

        private AmdPreset GetSelectedAmdPreset()
        {
            if (lvAmdPresets.SelectedItems.Count > 0)
            {
                var item = lvAmdPresets.SelectedItems[0];
                return (AmdPreset)item.Tag;
            }
            else
            {
                return null;
            }
        }

        private GamePreset GetSelectedGamePreset()
        {
            if (lvGamePresets.SelectedItems.Count > 0)
            {
                var item = lvGamePresets.SelectedItems[0];
                return (GamePreset)item.Tag;
            }
            else
            {
                return null;
            }
        }

        private void btnClone_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedNvPreset();

            var newPreset = preset.Clone();
            AddOrUpdateItem(newPreset);
        }

        private void miNvPresetColorSettings_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedNvPreset();

            preset.applyColorData = !preset.applyColorData;

            AddOrUpdateItem();
        }

        private void miNvPresetApplyDithering_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedNvPreset();

            preset.applyDithering = !preset.applyDithering;

            AddOrUpdateItem();
        }

        private void miNvPresetDitheringEnabled_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedNvPreset(true);

            if (preset.IsDisplayPreset)
            {
                miNvPresetDitheringEnabled.Checked = !miNvPresetDitheringEnabled.Checked;

                _nvService.SetDithering(miNvPresetDitheringEnabled.Checked ? NvDitherState.Enabled : NvDitherState.Disabled);

                UpdateDisplayInfoItems();
                return;
            }

            preset.ditheringEnabled = !preset.ditheringEnabled;

            AddOrUpdateItem();
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            mnuNvPresets.Show(btnChange, btnChange.PointToClient(Cursor.Position));
        }

        private void includedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedNvPreset();

            preset.applyRefreshRate = !preset.applyRefreshRate;

            AddOrUpdateItem();
        }

        private Font _normalFont;
        private Font _boldFont;
        private Dictionary<bool, Font> _menuItemFonts;

        private void mnuNvPresets_Opening(object sender, CancelEventArgs e)
        {
            var preset = GetSelectedNvPreset(true);
            var isCurrentDisplay = preset?.IsDisplayPreset == true;

            var presetItemsVisible = preset != null && !isCurrentDisplay;

            miNvApply.Visible = presetItemsVisible;
            miNvPresetApplyOnStartup.Visible = presetItemsVisible;
            tssNvPresetMenu.Visible = presetItemsVisible;
            mnuNvDisplay.Visible = presetItemsVisible;
            miNvPresetColorSettings.Enabled = presetItemsVisible;
            mnuNvPresetsColorSettings.Enabled = preset != null;
            mnuRefreshRate.Enabled = preset != null;
            mnuNvResolution.Enabled = preset != null;
            miNvPresetDithering.Enabled = preset != null;
            miNvHDR.Enabled = preset != null;
            miNvCopyId.Visible = presetItemsVisible;
            miNvDriverSettingsIncluded.Visible = presetItemsVisible;
            mnuNvDriverSettings.Enabled = preset != null;
            mnuNvOverclocking.Visible = preset != null && _nvService.Config.ShowOverclocking;

            if (preset == null)
            {
                return;
            }

            _normalFont ??= new Font(mnuNvDriverSettings.Font, FontStyle.Regular);
            _boldFont ??= new Font(_normalFont, FontStyle.Bold);
            _menuItemFonts ??= new Dictionary<bool, Font> { { false, _normalFont }, { true, _boldFont } };

            miNvPresetApplyOnStartup.Checked = preset.id > 0 && _config.NvPresetId_ApplyOnStartup == preset.id;

            miNvPresetApplyDithering.Visible = !isCurrentDisplay;
            miNvPresetColorSettings.Visible = !isCurrentDisplay;
            miNvResolutionIncluded.Visible = !isCurrentDisplay;
            miRefreshRateIncluded.Visible = !isCurrentDisplay;
            miHDRIncluded.Visible = !isCurrentDisplay;

            if (mnuNvDisplay.DropDownItems.Count == 1)
            {
                var displays = _nvService.GetDisplays();
                for (var i = 0; i < displays.Length; i++)
                {
                    var display = displays[i];
                    var name = FormUtils.ExtendedDisplayName(display.Name);

                    var item = mnuNvDisplay.DropDownItems.Add(name);
                    item.Tag = display;
                    item.Click += displayMenuItem_Click;
                }
            }

            FormUtils.BuildDropDownMenu(mnuNvPresetsColorSettings, "Bit depth", typeof(ColorDataDepth), preset.colorData, "ColorDepth", nvPresetColorDataMenuItem_Click);
            FormUtils.BuildDropDownMenu(mnuNvPresetsColorSettings, "Format", typeof(ColorDataFormat), preset.colorData, "ColorFormat", nvPresetColorDataMenuItem_Click);
            FormUtils.BuildDropDownMenu(mnuNvPresetsColorSettings, "Dynamic range", typeof(ColorDataDynamicRange), preset.colorData, "DynamicRange", nvPresetColorDataMenuItem_Click);
            FormUtils.BuildDropDownMenu(mnuNvPresetsColorSettings, "Color space", typeof(ColorDataColorimetry), preset.colorData, "Colorimetry", nvPresetColorDataMenuItem_Click);

            BuildNvRefreshRateResolutionMenu(preset);

            miNvPrimaryDisplay.Checked = preset.primaryDisplay;
            foreach (var item in mnuNvDisplay.DropDownItems)
            {
                if (item is ToolStripMenuItem menuItem && menuItem.Tag != null)
                {
                    menuItem.Checked = ((Display)menuItem.Tag).Name.Equals(preset.displayName);
                }
            }

            miNvPresetColorSettings.Checked = preset.applyColorData;
            miNvPresetApplyDithering.Checked = preset.applyDithering;
            miNvPresetDitheringEnabled.Checked = preset.ditheringEnabled;

            foreach (var item in mnuNvDitheringBitDepth.DropDownItems.OfType<ToolStripMenuItem>())
            {
                if (item.Tag != null)
                {
                    item.Checked = uint.Parse(item.Tag.ToString()) == preset.ditheringBits;
                }
            }
            foreach (var item in mnuNvDitheringMode.DropDownItems.OfType<ToolStripMenuItem>())
            {
                if (item.Tag != null)
                {
                    item.Checked = uint.Parse(item.Tag.ToString()) == preset.ditheringMode;
                }
            }

            miHDRIncluded.Checked = preset.applyHDR;
            miHDREnabled.Checked = preset.HDREnabled;
            miToggleHDR.Checked = preset.toggleHDR;

            miNvDriverSettingsIncluded.Checked = preset.applyDriverSettings;

            mnuNvPresetsColorSettings.Font = _menuItemFonts[preset.applyColorData];
            mnuRefreshRate.Font = _menuItemFonts[preset.applyRefreshRate];
            mnuNvResolution.Font = _menuItemFonts[preset.applyResolution];
            miNvPresetDithering.Font = _menuItemFonts[preset.applyDithering];
            miNvHDR.Font = _menuItemFonts[preset.applyHDR];
            mnuNvOverclocking.Font = _menuItemFonts[preset.applyOverclocking];

            BuildNvOverclockingMenu(preset);

            UpdateNvPresetMenuFont();

            BuildNvidiaDriverSettingsMenu(mnuNvDriverSettings, isCurrentDisplay ? null : preset.driverSettings);

            mnuNvDriverSettings.Font = _menuItemFonts[preset.applyDriverSettings];
        }

        private void BuildNvOverclockingMenu(NvPreset preset)
        {
            miNvOverclockingIncluded.Checked = preset.applyOverclocking;

            var gpus = _nvService.GetGPUs();

            foreach (var gpu in gpus)
            {
                var gpuPciId = gpu.BusInformation.PCIIdentifiers.ToString();

                var name = $"{mnuNvOverclocking.Name}_{gpu.GPUId}";

                var gpuItem = FormUtils.BuildMenuItem(mnuNvOverclocking.DropDownItems, name, gpu.FullName);

                var subName = $"{name}_change";

                var ocSettings = preset.ocSettings.FirstOrDefault(s => s.PCIIdentifier == gpuPciId) ?? new NvGpuOcSettings { PCIIdentifier = gpuPciId };

                var subText = $"{ocSettings} (click to change)";

                var subItem = FormUtils.BuildMenuItem(gpuItem.DropDownItems, subName, subText, ocSettings, nvChangeOverclockingMenuItem_Click);
            }
        }

        private void nvChangeOverclockingMenuItem_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedNvPreset();

            var menuItem = (ToolStripItem)sender;

            var currentSettings = (NvGpuOcSettings)menuItem.Tag;

            var gpuInfo = NvGpuInfo.GetGpuInfo(currentSettings.PCIIdentifier);

            var newSettings = AskNvOcSettings(gpuInfo, currentSettings);

            if (newSettings == null)
            {
                return;
            }

            preset.UpdateOverclockingSetting(newSettings);

            AddOrUpdateItem();
        }

        private void BuildNvRefreshRateResolutionMenu(NvPreset preset)
        {
            var displayName = preset.primaryDisplay ? null : preset.displayName;

            if (displayName != _lastDisplayRefreshRates)
            {
                while (mnuRefreshRate.DropDownItems.Count > 1)
                {
                    mnuRefreshRate.DropDownItems.RemoveAt(mnuRefreshRate.DropDownItems.Count - 1);
                }
                while (mnuNvResolution.DropDownItems.Count > 1)
                {
                    mnuNvResolution.DropDownItems.RemoveAt(mnuNvResolution.DropDownItems.Count - 1);
                }
            }

            if (mnuRefreshRate.DropDownItems.Count == 1)
            {
                var refreshRates = _nvService.GetAvailableRefreshRates(preset);
                _lastDisplayRefreshRates = displayName;

                foreach (var refreshRate in refreshRates)
                {
                    var item = mnuRefreshRate.DropDownItems.Add(refreshRate.ToString() + "Hz");
                    item.Tag = refreshRate;
                    item.Click += refreshRateMenuItem_Click;
                }
            }

            if (mnuNvResolution.DropDownItems.Count == 1)
            {
                var modes = _nvService.GetAvailableResolutions(preset);

                foreach (var mode in modes)
                {
                    var item = mnuNvResolution.DropDownItems.Add($"{mode.dmPelsWidth}x{mode.dmPelsHeight}");
                    item.Tag = mode;
                    item.Click += resolutionNvMenuItem_Click;
                }
            }

            miRefreshRateIncluded.Checked = preset.applyRefreshRate;
            foreach (var item in mnuRefreshRate.DropDownItems)
            {
                if (item is ToolStripMenuItem menuItem && menuItem.Tag != null)
                {
                    menuItem.Checked = (uint)menuItem.Tag == preset.refreshRate;
                }
            }

            miNvResolutionIncluded.Checked = preset.applyResolution;
            foreach (var item in mnuNvResolution.DropDownItems)
            {
                if (item is ToolStripMenuItem menuItem && menuItem.Tag != null)
                {
                    var mode = (DEVMODEA)menuItem.Tag;
                    menuItem.Checked = mode.dmPelsWidth == preset.resolutionWidth && mode.dmPelsHeight == preset.resolutionHeight;
                }
            }
        }

        private void UpdateNvPresetMenuFont(bool updateTopItems = false)
        {
            foreach (var menu in mnuNvPresets.Items.OfType<ToolStripMenuItem>().Where(i => i != mnuNvDriverSettings))
            {
                if (updateTopItems)
                {
                    menu.Font = _normalFont;
                }

                SetMenuFont(menu, _normalFont);
            }
        }

        private void SetMenuFont(ToolStripMenuItem menu, Font font)
        {
            foreach (var item in menu.DropDownItems.OfType<ToolStripMenuItem>())
            {
                item.Font = item.Checked ? _boldFont : font;

                SetMenuFont(item, font);
            }
        }

        private void BuildNvidiaDriverSettingsMenu(ToolStripDropDownItem parent, Dictionary<uint, uint> settings = null, ContextMenuStrip contextMenuStrip = null)
        {
            var driverSettings = _nvService.GetVisibleSettings().OrderBy(s => s.GroupName);
            var handledGroupItems = new List<ToolStripMenuItem>();
            var showUnchanged = settings != null;
            var defaultValue = SettingMeta.UnsetDwordValue;

            foreach (var driverSetting in driverSettings)
            {
                var groupName = $"mnuNvDriverSettingGroup_{driverSetting.GroupName}";
                var groupItem = FormUtils.BuildMenuItem(contextMenuStrip?.Items ?? parent.DropDownItems, groupName, driverSetting.GroupName);

                var itemName = $"mnuNvDriverSettingItem_{driverSetting.SettingText}";
                var item = FormUtils.BuildMenuItem(groupItem.DropDownItems, itemName, driverSetting.SettingText, driverSetting);
                var settingMeta = _nvService.GetSettingMeta(driverSetting.SettingId);

                var presetSetting = defaultValue;
                var isDefault = false;
                if (settings != null)
                {
                    presetSetting = settings.GetValueOrDefault(driverSetting.SettingId, defaultValue);
                    isDefault = presetSetting == defaultValue;
                }
                else
                {
                    (presetSetting, isDefault) = settingMeta.ToIntValue(driverSetting.ValueText);
                }

                item.Font = _menuItemFonts[!isDefault];

                if (!handledGroupItems.Contains(groupItem))
                {
                    groupItem.Tag = null;
                    handledGroupItems.Add(groupItem);
                }
                if (!isDefault)
                {
                    groupItem.Tag = true;
                }

                var subItemUnchangedName = $"mnuNvDriverSettingItem_{driverSetting.SettingId}_Unchanged";
                var subItemUnchanged = FormUtils.BuildMenuItem(item.DropDownItems, subItemUnchangedName, "Unchanged", onClick: driverSettingValueNvMenuItem_Click);
                subItemUnchanged.Visible = showUnchanged;

                if (showUnchanged)
                {
                    subItemUnchanged.Checked = presetSetting == defaultValue;
                    subItemUnchanged.Font = _menuItemFonts[subItemUnchanged.Checked];
                }

                if (driverSetting.SettingId == NvService.DRS_FRAME_RATE_LIMITER_V3)
                {
                    var value = settingMeta.DwordValues.FirstOrDefault(v => v.ValueName == driverSetting.ValueText);

                    var subItemName = $"mnuNvDriverSettingItem_{driverSetting.SettingId}_SubItem";
                    var subItem = FormUtils.BuildMenuItem(item.DropDownItems, subItemName, "", onClick: driverSettingFrameRateNvMenuItem_Click);

                    var metaValue = settingMeta.DwordValues.FirstOrDefault(v => v.Value == presetSetting);
                    subItem.Text = $"{metaValue?.ValueName ?? "??FPS"} (click to change)";

                    subItem.Tag = value?.Value ?? 0;
                    subItem.Checked = presetSetting != defaultValue;

                    continue;
                }

                if (settingMeta.SettingType == nspector.Native.NVAPI2.NVDRS_SETTING_TYPE.NVDRS_DWORD_TYPE)
                {
                    foreach (var settingValue in settingMeta.DwordValues)
                    {
                        var text = settingValue.ValueName;
                        if (text.IndexOf(" (") == 10)
                        {
                            text = text.Substring(0, 10);
                        }

                        var subItemName = $"mnuNvDriverSettingItem_{driverSetting.SettingId}_SubItem_{settingValue.Value}";
                        var subItem = FormUtils.BuildMenuItem(item.DropDownItems, subItemName, text, settingValue, driverSettingValueNvMenuItem_Click);

                        subItem.Checked = presetSetting == settingValue.Value;
                        subItem.Font = _menuItemFonts[subItem.Checked];
                    }
                }

                if (settingMeta.SettingType == nspector.Native.NVAPI2.NVDRS_SETTING_TYPE.NVDRS_BINARY_TYPE)
                {
                    foreach (var settingValue in settingMeta.BinaryValues)
                    {
                        var text = settingValue.ValueName.Substring(0, 18);
                        var subItemName = $"mnuNvDriverSettingItem_{driverSetting.SettingId}_SubItem_{text}";

                        var subItem = FormUtils.BuildMenuItem(item.DropDownItems, subItemName, text, settingValue, driverSettingValueNvMenuItem_Click);

                        var intValue = BitConverter.ToUInt32(settingValue.Value);

                        subItem.Checked = presetSetting == intValue;
                        subItem.Font = _menuItemFonts[subItem.Checked];
                    }
                }

                if (item.DropDownItems.Count == 0 || item.DropDownItems.Count == 1 && item.DropDownItems[0].Text == "Unchanged")
                {
                    groupItem.DropDownItems.Remove(item);
                }
            }

            foreach (var groupItem in handledGroupItems)
            {
                groupItem.Font = _menuItemFonts[groupItem.Tag != null];
            }
        }

        private void nvPresetColorDataMenuItem_Click(object sender, EventArgs e)
        {
            var menuItem = (ToolStripMenuItem)sender;
            var property = (PropertyInfo)menuItem.OwnerItem.Tag;

            var value = menuItem.Tag;

            var preset = GetSelectedNvPreset(true);

            var dictionary = new Dictionary<string, object>
            {
                { "ColorFormat", preset.colorData.ColorFormat },
                { "ColorDepth", preset.colorData.ColorDepth },
                { "Colorimetry", preset.colorData.Colorimetry },
                { "DynamicRange", preset.colorData.DynamicRange },
                { "SelectionPolicy", preset.colorData.SelectionPolicy }
            };

            dictionary[property.Name] = value;

            var colorData = NvPreset.GenerateColorData(dictionary);

            if (preset.IsDisplayPreset)
            {
                _nvService.SetColorData(preset.Display, colorData);

                UpdateDisplayInfoItems();
                return;
            }

            preset.colorData = colorData;

            AddOrUpdateItem();
        }

        private void refreshRateMenuItem_Click(object sender, EventArgs e)
        {
            var refreshRate = (uint)((ToolStripItem)sender).Tag;

            var preset = GetSelectedNvPreset(true);

            if (preset.IsDisplayPreset)
            {
                _nvService.SetMode(refreshRate: refreshRate);

                UpdateDisplayInfoItems();
                return;
            }

            preset.refreshRate = refreshRate;

            AddOrUpdateItem();
        }

        private void resolutionNvMenuItem_Click(object sender, EventArgs e)
        {
            var mode = (DEVMODEA)((ToolStripItem)sender).Tag;

            var preset = GetSelectedNvPreset(true);

            if (preset.IsDisplayPreset)
            {
                _nvService.SetMode(mode.dmPelsWidth, mode.dmPelsHeight);

                UpdateDisplayInfoItems();
                return;
            }

            preset.resolutionWidth = mode.dmPelsWidth;
            preset.resolutionHeight = mode.dmPelsHeight;

            AddOrUpdateItem();
        }

        private void refreshRateMenuItemAmd_Click(object sender, EventArgs e)
        {
            var refreshRate = (uint)((ToolStripItem)sender).Tag;

            var preset = GetSelectedAmdPreset();

            preset.refreshRate = refreshRate;

            AddOrUpdateItemAmd();
        }

        private void displayMenuItem_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedNvPreset();

            var menuItem = (ToolStripItem)sender;
            if (menuItem.Tag != null)
            {
                var display = (Display)menuItem.Tag;

                preset.displayName = display.Name;
                preset.primaryDisplay = false;
            }
            else
            {
                preset.primaryDisplay = true;
                preset.displayName = null;
            }

            AddOrUpdateItem();
        }

        private void displayMenuItemAmd_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedAmdPreset();

            var menuItem = (ToolStripItem)sender;
            if (menuItem.Tag != null)
            {
                var display = (ADLDisplayInfo)menuItem.Tag;

                preset.displayName = display.DisplayName;
                preset.primaryDisplay = false;
            }
            else
            {
                preset.primaryDisplay = true;
                preset.displayName = null;
            }

            AddOrUpdateItemAmd();
        }

        private void miHDRIncluded_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedNvPreset();

            preset.applyHDR = !preset.applyHDR;

            AddOrUpdateItem();
        }

        private void miToggleHDR_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedNvPreset(true);

            if (preset.IsDisplayPreset)
            {
                _nvService.SetHDRState(preset.Display, !_nvService.IsHDREnabled());

                UpdateDisplayInfoItems();
                return;
            }

            preset.toggleHDR = !preset.toggleHDR;

            if (preset.toggleHDR)
            {
                preset.HDREnabled = false;
            }

            AddOrUpdateItem();
        }

        private void miHDREnabled_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedNvPreset(true);

            if (preset.IsDisplayPreset)
            {
                miHDREnabled.Checked = !miHDREnabled.Checked;
                _nvService.SetHDRState(preset.Display, miHDREnabled.Checked);

                UpdateDisplayInfoItems();
                return;
            }

            preset.HDREnabled = !preset.HDREnabled;
            preset.toggleHDR = false;

            AddOrUpdateItem();
        }

        private void FillLgPresets()
        {
            FormUtils.InitListView(lvLgPresets, LgPreset.GetColumnNames());

            foreach (var preset in _lgService.GetPresets())
            {
                AddOrUpdateItemLg(preset);
                Utils.RegisterShortcut(Handle, preset.id, preset.shortcut);
            }
        }

        private void AddOrUpdateItemLg(LgPreset preset = null, ListViewItem specItem = null)
        {
            FormUtils.AddOrUpdateListItem(lvLgPresets, _lgService.GetPresets(), _config, preset, specItem);
        }

        private void btnCloneLg_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedLgPreset();

            var newPreset = preset.Clone();
            AddOrUpdateItemLg(newPreset);
        }

        private LgPreset GetSelectedLgPreset()
        {
            if (lvLgPresets.SelectedItems.Count > 0)
            {
                var item = lvLgPresets.SelectedItems[0];
                return (LgPreset)item.Tag;
            }
            else
            {
                return null;
            }
        }

        private void lvLgPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            var enabled = lvLgPresets.SelectedItems.Count > 0;
            btnApplyLg.Enabled = enabled;
            btnCloneLg.Enabled = enabled;
            edtNameLg.Enabled = enabled;
            chkLgQuickAccess.Enabled = enabled;
            edtLgPresetDescription.Enabled = enabled;
            cbxLgPresetDevice.Enabled = enabled;
            edtShortcutLg.Enabled = enabled;
            btnSetShortcutLg.Enabled = enabled;
            edtStepsLg.Enabled = enabled;
            btnLgAddButton.Enabled = enabled;
            cbxLgApps.Enabled = enabled;
            btnLgRefreshApps.Enabled = enabled;
            btnDeleteLg.Enabled = enabled;
            cbxLgPresetTrigger.Enabled = enabled;
            edtLgPresetTriggerConditions.Enabled = enabled;
            btnLgPresetEditTriggerConditions.Enabled = enabled;
            edtLgPresetIncludedProcesses.Enabled = enabled;
            edtLgPresetExcludedProcesses.Enabled = enabled;

            var preset = GetSelectedLgPreset();

            if (preset != null)
            {
                edtNameLg.Text = preset.name;
                chkLgQuickAccess.Checked = preset.ShowInQuickAccess;
                edtLgPresetDescription.Text = preset.Description;
                var lgApps = _lgService?.GetApps();
                cbxLgApps.SelectedIndex = lgApps == null ? -1 : lgApps.FindIndex(x => x.appId.Equals(preset.appId));
                edtShortcutLg.Text = preset.shortcut;
                edtStepsLg.Text = preset.steps.Aggregate("", (a, b) => (string.IsNullOrEmpty(a) ? "" : a + ", ") + b);

                var index = -1;

                for (var i = 0; i < cbxLgPresetDevice.Items.Count; i++)
                {
                    var pnpDev = (LgDevice)cbxLgPresetDevice.Items[i];
                    if ((string.IsNullOrEmpty(preset.DeviceMacAddress) && string.IsNullOrEmpty(pnpDev.MacAddress)) || pnpDev.MacAddress.Equals(preset.DeviceMacAddress, StringComparison.OrdinalIgnoreCase))
                    {
                        index = i;
                        break;
                    }
                }

                cbxLgPresetDevice.SelectedIndex = index;

                var trigger = preset.Triggers.FirstOrDefault();

                FormUtils.SetComboBoxEnumIndex(cbxLgPresetTrigger, (int)(trigger?.Trigger ?? PresetTriggerType.None));

                edtLgPresetTriggerConditions.Text = Utils.GetDescriptions<PresetConditionType>(trigger != null ? (int)trigger.Conditions : 0).Join(", ");
                edtLgPresetTriggerConditions.Tag = trigger != null ? (int)trigger.Conditions : 0;

                edtLgPresetIncludedProcesses.Text = trigger?.IncludedProcesses?.Join(", ") ?? string.Empty;
                edtLgPresetExcludedProcesses.Text = trigger?.ExcludedProcesses?.Join(", ") ?? string.Empty;
            }
            else
            {
                edtNameLg.Text = string.Empty;
                chkLgQuickAccess.Checked = false;
                edtLgPresetDescription.Text = string.Empty;
                cbxLgApps.SelectedIndex = -1;
                edtShortcutLg.Text = string.Empty;
                edtStepsLg.Text = string.Empty;
                cbxLgPresetDevice.SelectedIndex = -1;
                cbxLgPresetTrigger.SelectedIndex = -1;
                edtLgPresetTriggerConditions.Text = string.Empty;
                edtLgPresetTriggerConditions.Tag = 0;
                edtLgPresetIncludedProcesses.Text = string.Empty;
                edtLgPresetExcludedProcesses.Text = string.Empty;
            }
        }

        private void btnApplyLg_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedLgPreset();

            ApplyLgPreset(preset);
        }

        private void btnSetShortcutLg_Click(object sender, EventArgs e)
        {
            var shortcut = edtShortcutLg.Text.Trim();
            if (!Utils.ValidateShortcut(shortcut))
            {
                return;
            }

            var preset = GetSelectedLgPreset();

            var name = edtNameLg.Text.Trim();

            if (name.Length == 0 || _lgService.GetPresets().Any(x => x.id != preset.id && x.name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                MessageForms.WarningOk("The name can not be empty and must be unique.");
                return;
            }

            var clear = !string.IsNullOrEmpty(preset.shortcut);

            preset.name = name;
            preset.ShowInQuickAccess = chkLgQuickAccess.Checked;
            preset.Description = edtLgPresetDescription.Text.Trim();

            if (cbxLgPresetDevice.SelectedIndex == -1)
            {
                preset.DeviceMacAddress = string.Empty;
            }
            else
            {
                var device = (LgDevice)cbxLgPresetDevice.SelectedItem;
                preset.DeviceMacAddress = device.MacAddress;
            }

            if (cbxLgApps.SelectedIndex == -1)
            {
                preset.appId = string.Empty;
            }
            else
            {
                var lgApp = (LgApp)cbxLgApps.SelectedItem;
                preset.appId = lgApp.appId;
            }

            var triggerType = FormUtils.GetComboBoxEnumItem<PresetTriggerType>(cbxLgPresetTrigger);
            preset.UpdateTrigger(triggerType,
                                 (PresetConditionType)edtLgPresetTriggerConditions.Tag,
                                 edtLgPresetIncludedProcesses.Text,
                                 edtLgPresetExcludedProcesses.Text);

            if (triggerType != PresetTriggerType.None)
            {
                _lgService.InstallEventHandlers();
            }

            var shortcutChanged = !shortcut.Equals(preset.shortcut);
            if (shortcutChanged)
            {
                preset.shortcut = shortcut;
            }

            var text = edtStepsLg.Text;

            Utils.ParseWords(preset.steps, text);

            AddOrUpdateItemLg();

            if (shortcutChanged)
            {
                Utils.RegisterShortcut(Handle, preset.id, preset.shortcut, clear);
            }
        }

        private void btnNvPresetDelete_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedNvPreset();

            if (preset == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(preset.shortcut))
            {
                WinApi.UnregisterHotKey(Handle, preset.id);
            }

            _nvService.GetPresets().Remove(preset);

            var item = lvNvPresets.SelectedItems[0];
            lvNvPresets.Items.Remove(item);
        }

        private void btnAddModesNv_Click(object sender, EventArgs e)
        {
            var presets = NvPreset.GetDefaultPresets();
            var added = false;

            foreach (var preset in presets)
            {
                if (!_nvService.GetPresets().Any(x => x.colorData.Equals(preset.colorData)))
                {
                    AddOrUpdateItem(preset);
                    added = true;
                }
            }

            if (added)
            {
                MessageForms.InfoOk("Missing presets added.");
            }
            else
            {
                MessageForms.InfoOk("All presets for every color setting already exist.");
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitSelectedTab();
        }

        private void InitSelectedTab()
        {
            if (tcMain.SelectedTab == tabLG)
            {
                InitLgTab();
            }
            else if (tcMain.SelectedTab == tabLog)
            {
                LoadLog();
            }
            else if (tcMain.SelectedTab == tabInfo)
            {
                LoadInfo();
            }
            else if (tcMain.SelectedTab == tabOptions)
            {
                InitOptionsTab();
            }
            else if (tcMain.SelectedTab == tabNVIDIA)
            {
                UpdateDisplayInfoItems();
            }
            else if (tcMain.SelectedTab == tabGameLauncher)
            {
                UpdateGameLauncherTab();
            }
        }

        private void UpdateGameLauncherTab()
        {
            if (cbxGameStepType.Items.Count == 0)
            {
                cbxGameStepType.Items.AddRange(Utils.GetDescriptions<GameStepType>().ToArray());
                cbxGameStepType.SelectedIndex = 0;
            }
        }

        private void InitLgTab()
        {
            if (_lgService == null)
            {
                return;
            }

            if (scLgController.Panel2.Controls.Count == 0)
            {
                var rcPanel = new RemoteControlPanel(_lgService, _lgService.GetRemoteControlButtons());
                rcPanel.Parent = scLgController.Panel2;
                rcPanel.Dock = DockStyle.Fill;
            }
            chkLgRemoteControlShow.Checked = _lgService.Config.ShowRemoteControl;
            scLgController.Panel2Collapsed = !_lgService.Config.ShowRemoteControl;

            FormUtils.BuildComboBox(cbxLgPresetTrigger, PresetTriggerType.Resume, PresetTriggerType.Shutdown, PresetTriggerType.Standby, PresetTriggerType.Startup, PresetTriggerType.Reserved5, PresetTriggerType.ScreensaverStart, PresetTriggerType.ScreensaverStop);

            if (!string.IsNullOrEmpty(_lgTabMessage))
            {
                MessageForms.WarningOk(_lgTabMessage);
                _lgTabMessage = null;
            }
        }

        private void InitOptionsTab()
        {
            grpNvidiaOptions.Visible = _nvService != null;
            if (grpNvidiaOptions.Visible)
            {
                if (cbxDitheringBitDepth.Items.Count == 0)
                {
                    cbxDitheringBitDepth.Items.AddRange(Utils.GetDescriptions<NvDitherBits>().ToArray());
                    cbxDitheringMode.Items.AddRange(Utils.GetDescriptions<NvDitherMode>().ToArray());
                    FillGradient();
                }

                if (cbxDitheringDisplay.Items.Count == 0)
                {
                    var displays = _nvService.GetDisplayInfos();
                    var primaryDisplay = _nvService.GetPrimaryDisplay();
                    var primaryDisplayInfo = displays.FirstOrDefault(d => d.Display == primaryDisplay);
                    var index = primaryDisplayInfo != null ? displays.IndexOf(primaryDisplayInfo) : 0;

                    cbxDitheringDisplay.Items.AddRange(displays.ToArray());

                    cbxDitheringDisplay.SelectedIndex = index;
                }

                UpdateDitherSettings();
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

        private void UpdateDitherSettings()
        {
            var display = ((NvDisplayInfo)cbxDitheringDisplay.SelectedItem)?.Display;
            var ditherInfo = _nvService.GetDithering(display);

            if (ditherInfo.state == -1)
            {
                MessageForms.ErrorOk("Error retrieving dithering settings. See log for details.");
                return;
            }

            var state = (NvDitherState)ditherInfo.state;

            _updatingDitherSettings = true;
            try
            {
                chkDitheringEnabled.CheckState = state switch { NvDitherState.Enabled => CheckState.Checked, NvDitherState.Disabled => CheckState.Unchecked, _ => CheckState.Indeterminate };
                cbxDitheringBitDepth.SelectedIndex = ditherInfo.bits;
                cbxDitheringMode.SelectedIndex = ditherInfo.mode;

                cbxDitheringBitDepth.Enabled = state == NvDitherState.Enabled;
                cbxDitheringMode.Enabled = state == NvDitherState.Enabled;
            }
            finally
            {
                _updatingDitherSettings = false;
            }
        }

        private void FillLgDevices()
        {
            var devices = _lgService.Devices;

            cbxLgDevices.Items.Clear();
            cbxLgDevices.Items.AddRange(devices.ToArray());

            cbxLgPresetDevice.Items.Clear();
            cbxLgPresetDevice.Items.AddRange(devices.ToArray());
            var globalDevice = new LgDevice("Globally selected device", string.Empty, string.Empty, true, true);
            cbxLgPresetDevice.Items.Insert(0, globalDevice);

            var device = _lgService.SelectedDevice;

            if (device != null)
            {
                cbxLgDevices.SelectedIndex = cbxLgDevices.Items.IndexOf(device);
            }

            if (!devices.Any())
            {
                var message = "It seems there's no LG TV available! Please make sure it's connected to the same network as this PC.";

                if (tcMain.SelectedTab == tabLG)
                {
                    MessageForms.WarningOk(message);
                }
                else
                {
                    _lgTabMessage = message;
                }
            }

            if (cbxLgApps.Items.Count == 0 && device != null)
            {
                _lgService.RefreshApps().ContinueWith((task) => BeginInvoke(() => FillLgApps(false)));
            }

            btnLgDeviceConvertToCustom.Enabled = devices.Any();
            btnLgExpert.Enabled = devices.Any();
            btnLgGameBar.Enabled = devices.Any();

            cbxLgDevices_SelectedIndexChanged(cbxLgDevices, null);

            SetLgDevicePowerOptions();
        }

        private void SetLgDevicePowerOptions()
        {
            var device = _lgService.SelectedDevice;
            clbLgPower.Enabled = device != null;

            _disableEvents = true;
            try
            {
                clbLgPower.SetItemChecked(0, device?.PowerOnAfterStartup ?? false);
                clbLgPower.SetItemChecked(1, device?.PowerOnAfterResume ?? false);
                clbLgPower.SetItemChecked(2, device?.PowerOffOnShutdown ?? false);
                clbLgPower.SetItemChecked(3, device?.PowerOffOnStandby ?? false);
                clbLgPower.SetItemChecked(4, device?.PowerOffOnScreenSaver ?? false);
                clbLgPower.SetItemChecked(5, device?.PowerOnAfterScreenSaver ?? false);
                clbLgPower.SetItemChecked(6, device?.PowerOnAfterManualPowerOff ?? false);
                clbLgPower.SetItemChecked(7, device?.TriggersEnabled ?? true);
                clbLgPower.SetItemChecked(8, device?.PowerOffByWindows ?? false);
                clbLgPower.SetItemChecked(9, device?.PowerOnByWindows ?? false);
                clbLgPower.SetItemChecked(10, device?.UseSecureConnection ?? false);
            }
            finally
            {
                _disableEvents = false;
            }
        }

        private void FillLgApps(bool forced)
        {
            var lgApps = _lgService?.GetApps();
            if (forced && (lgApps == null || !lgApps.Any()))
            {
                MessageForms.WarningOk("Could not refresh the apps. Check the log for details.");
                return;
            }
            InitLgApps();
        }

        private void InitLgApps()
        {
            var lgApps = _lgService?.GetApps();

            cbxLgApps.Items.Clear();
            if (lgApps != null)
            {
                cbxLgApps.Items.AddRange(lgApps.ToArray());
            }

            for (var i = 0; i < lvLgPresets.Items.Count; i++)
            {
                var item = lvLgPresets.Items[i];
                AddOrUpdateItemLg((LgPreset)item.Tag, item);
            }

            var preset = GetSelectedLgPreset();
            if (preset != null)
            {
                cbxLgApps.SelectedIndex = lgApps == null ? -1 : lgApps.FindIndex(x => x.appId.Equals(preset.appId));
            }
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
                menu.DropDownItems.Add(item);
            }
        }

        private void TrayMenuItemNv_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            var preset = (NvPreset)item.Tag;

            ApplyNvPreset(preset);
        }

        private void TrayMenuItemAmd_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            var preset = (AmdPreset)item.Tag;

            ApplyAmdPreset(preset);
        }

        private void TrayMenuItemGame_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            var preset = (GamePreset)item.Tag;

            ApplyGamePreset(preset);
        }

        private void trayIconContextMenu_Popup(object sender, EventArgs e)
        {
            _nvTrayMenu.Visible = _nvService != null;
            if (_nvTrayMenu.Visible)
            {
                var presets = _nvService.GetPresets().Where(x => x.applyColorData || x.applyDithering || x.applyHDR || x.applyRefreshRate || x.applyResolution || x.applyDriverSettings);

                UpdateTrayMenu(_nvTrayMenu, presets, TrayMenuItemNv_Click);
            }

            _amdTrayMenu.Visible = _amdService != null;
            if (_amdTrayMenu.Visible)
            {
                var presets = _amdService.GetPresets().Where(x => x.applyColorData || x.applyDithering || x.applyHDR || x.applyRefreshRate);

                UpdateTrayMenu(_amdTrayMenu, presets, TrayMenuItemAmd_Click);
            }

            _lgTrayMenu.Visible = _lgService != null;
            if (_lgTrayMenu.Visible)
            {
                var presets = _lgService.GetPresets().Where(x => !string.IsNullOrEmpty(x.appId) || x.steps.Any());

                _lgTrayMenu.DropDownItems.Clear();

                UpdateTrayMenu(_lgTrayMenu, presets, TrayMenuItemLg_Click);
            }

            _gameTrayMenu.Visible = _gameService != null;
            if (_gameTrayMenu.Visible)
            {
                var presets = _gameService.GetPresets();

                _gameTrayMenu.DropDownItems.Clear();

                UpdateTrayMenu(_gameTrayMenu, presets, TrayMenuItemGame_Click);
            }
        }

        private void TrayMenuItemLg_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            var preset = (LgPreset)item.Tag;

            ApplyLgPreset(preset);
        }

        private void ApplySelectedNvPreset()
        {
            var preset = GetSelectedNvPreset();
            ApplyNvPreset(preset);
        }

        private void ApplySelectedAmdPreset()
        {
            var preset = GetSelectedAmdPreset();
            ApplyAmdPreset(preset);
        }

        private void ApplySelectedGamePreset()
        {
            var preset = GetSelectedGamePreset();
            ApplyGamePreset(preset);
        }

        private void btnDeleteLg_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedLgPreset();

            if (preset == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(preset.shortcut))
            {
                WinApi.UnregisterHotKey(Handle, preset.id);
            }

            _lgService.GetPresets().Remove(preset);

            var item = lvLgPresets.SelectedItems[0];
            lvLgPresets.Items.Remove(item);
        }

        private void btnAddLg_Click(object sender, EventArgs e)
        {
            AddOrUpdateItemLg(_lgService.CreateNewPreset());
        }

        private void edtDelayDisplaySettings_ValueChanged(object sender, EventArgs e)
        {
        }

        private bool ApplyNvPreset(NvPreset preset)
        {
            if (preset == null || _nvService == null)
            {
                return false;
            }
            try
            {
                var result = _nvService.ApplyPreset(preset);
                if (!result)
                {
                    throw new Exception("Error while applying NVIDIA preset. At least one setting could not be applied. Check the log for details.");
                }

                return true;
            }
            catch (Exception e)
            {
                MessageForms.ErrorOk($"Error applying NVIDIA-preset ({e.TargetSite.Name}): {e.Message}");
                return false;
            }
        }

        private bool ApplyAmdPreset(AmdPreset preset)
        {
            if (preset == null || _amdService == null)
            {
                return false;
            }
            try
            {
                var result = _amdService.ApplyPreset(preset);
                if (!result)
                {
                    throw new Exception("Error while applying AMD preset. At least one setting could not be applied. Check the log for details.");
                }

                return true;
            }
            catch (Exception e)
            {
                MessageForms.ErrorOk($"Error applying AMD-preset ({e.TargetSite.Name}): {e.Message}");
                return false;
            }
        }

        private void ApplyLgPreset(LgPreset preset, bool reconnect = false, bool wait = false)
        {
            if (preset == null)
            {
                return;
            }

            var applyTask = _lgService.ApplyPreset(preset, reconnect).ContinueWith((task) => BeginInvoke(() => LgPresetApplied(task)));
            if (wait)
            {
                Utils.WaitForTask(applyTask);
            }
        }

        private void LgPresetApplied(Task<bool> task)
        {
            var result = task.Result;
            if (!result)
            {
                MessageForms.WarningOk("Could not apply the preset (entirely). Check the log for details.");
            }
        }

        private bool ApplyGamePreset(GamePreset preset)
        {
            if (preset == null || _gameService == null)
            {
                return false;
            }
            try
            {
                var result = _gameService.ApplyPreset(preset, Program.AppContext);
                if (!result)
                {
                    throw new Exception("Error while applying Game-preset. At least one setting could not be applied. Check the log for details.");
                }

                return true;
            }
            catch (Exception e)
            {
                MessageForms.ErrorOk($"Error applying Game-preset ({e.TargetSite.Name}): {e.Message}");
                return false;
            }
        }

        private void LoadLog()
        {
            var logType = cbxLogType.SelectedIndex;

            if (logType == -1)
            {
                cbxLogType.SelectedIndex = 0;
                return;
            }

            string text;

            if (logType == 0)
            {
                var filename = Path.Combine(_dataDir, "LogFile.txt");

                text = Utils.ReadText(filename);

                text ??= "No log file found";
            }
            else
            {
                if (Utils.IsServiceRunning())
                {
                    var message = new SvcMessage { MessageType = SvcMessageType.GetLog };

                    var result = PipeUtils.SendMessage(message);

                    text = result?.Data ?? "Cannot get log from service";
                }
                else
                {
                    text = "The service is not installed or not running";
                }
            }

            edtLog.Text = text;

            edtLog.SelectionStart = text.Length;
            edtLog.ScrollToCaret();
        }

        private void LoadInfo()
        {
            grpNVIDIAInfo.Visible = _nvService != null;
        }

        private void btnLgRefreshApps_Click(object sender, EventArgs e)
        {
            _lgService.RefreshApps(true).ContinueWith((task) => BeginInvoke(() => FillLgApps(true)));
        }

        private void cbxLgDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbxLgPcHdmiPort.Enabled = cbxLgDevices.SelectedIndex > -1;
            if (cbxLgDevices.SelectedIndex == -1)
            {
                _lgService.SelectedDevice = null;
                cbxLgPcHdmiPort.SelectedIndex = 0;
            }
            else
            {
                _lgService.SelectedDevice = (LgDevice)cbxLgDevices.SelectedItem;
                cbxLgPcHdmiPort.SelectedIndex = _lgService.SelectedDevice.HDMIPortNumber;
            }
            chkLgRemoteControlShow.Enabled = _lgService.SelectedDevice != null;
            btnLgRemoveDevice.Enabled = _lgService.SelectedDevice != null && _lgService.SelectedDevice.IsCustom;
            btnLgDeviceConvertToCustom.Enabled = _lgService.SelectedDevice != null && !_lgService.SelectedDevice.IsCustom;

            SetLgDevicePowerOptions();
        }

        private void lvLgPresets_DoubleClick(object sender, EventArgs e)
        {
            ApplySelectedLgPreset();
        }

        private void ApplySelectedLgPreset()
        {
            var preset = GetSelectedLgPreset();
            ApplyLgPreset(preset);
        }

        private void btnRefreshNVIDIAInfo_Click(object sender, EventArgs e)
        {
            if (_nvService != null)
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

        private void btnLgAddButton_Click(object sender, EventArgs e)
        {
            mnuLgButtons.Show(btnLgAddButton, btnLgAddButton.PointToClient(Cursor.Position));
        }

        private void miLgAddButton_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripItem;

            FormUtils.AddStepToTextBox(edtStepsLg, item.Text);
        }

        private void miLgAddAction_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripItem;
            var action = item.Tag as LgDevice.InvokableAction;

            var value = ShowLgActionForm(action);

            if (!string.IsNullOrEmpty(value))
            {
                var text = $"{action.Name}({value})";

                FormUtils.AddStepToTextBox(edtStepsLg, text);
            }
        }

        private string ShowLgActionForm(LgDevice.InvokableAction action)
        {
            var text = action.Name;
            var title = action.Title ?? action.Name;

            var value = string.Empty;

            if (action.EnumType == null)
            {
                if (action.MinValue != action.MaxValue)
                {
                    List<MessageForms.FieldDefinition> fields = new();

                    if (action.NumberOfValues == 1)
                    {
                        fields.Add(new MessageForms.FieldDefinition
                        {
                            Label = "Enter desired " + title,
                            FieldType = MessageForms.FieldType.Numeric,
                            MinValue = action.MinValue,
                            MaxValue = action.MaxValue,
                        });
                    }
                    else
                    {
                        var array = Enumerable.Range(0, action.NumberOfValues);

                        fields.AddRange(array.Select(i => new MessageForms.FieldDefinition
                        {
                            Label = "Value for " + i,
                            FieldType = MessageForms.FieldType.Numeric,
                            MinValue = action.MinValue,
                            MaxValue = action.MaxValue,
                        }));
                    }

                    var values = MessageForms.ShowDialog($"Enter value for {title}", fields);

                    if (!values.Any())
                    {
                        return null;
                    }

                    if (values.Count > 1)
                    {
                        value = string.Join("; ", values.Select(v => v.Value));
                    }
                    else
                    {
                        value = values.First().Value.ToString();
                    }
                }
            }
            else
            {
                var dropDownValues = new List<string>();
                foreach (var enumValue in Enum.GetValues(action.EnumType))
                {
                    var description = Utils.GetDescription(action.EnumType, enumValue as IConvertible) ?? enumValue.ToString().Replace("_", "");
                    dropDownValues.Add(description);
                }

                var values = MessageForms.ShowDialog("Choose value", new[] {
                    new MessageForms.FieldDefinition
                    {
                        Label = "Choose desired " + title,
                        FieldType = MessageForms.FieldType.DropDown,
                        Values = dropDownValues
                    }
                });

                if (!values.Any())
                {
                    return null;
                }

                value = values.First().Value.ToString();
                var enumName = Utils.GetEnumNameByDescription(action.EnumType, value);
                if (enumName != null)
                {
                    value = enumName;
                }
            }

            return value;
        }

        private void miLgAddNvPreset_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripItem;
            var preset = item.Tag as NvPreset;

            var text = $"NvPreset({preset.name})";

            FormUtils.AddStepToTextBox(edtStepsLg, text);
        }

        private void miLgAddAmdPreset_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripItem;
            var preset = item.Tag as AmdPreset;

            var text = $"AmdPreset({preset.name})";

            FormUtils.AddStepToTextBox(edtStepsLg, text);
        }

        private void clbLgPower_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!_initialized || _disableEvents)
            {
                return;
            }

            var device = _lgService?.SelectedDevice;
            if (device == null)
            {
                return;
            }

            if (e.Index is 0 or 1 or 4 or 7 && !(device.PowerOnAfterResume || device.PowerOnAfterStartup || device.PowerOnAfterScreenSaver || device.PowerOnByWindows))
            {
                MessageForms.InfoOk(
@"Be sure to activate the following setting on the TV, or the app will not be able to wake the TV:

Connection > Mobile TV On > Turn on via Wi-Fi (Networked Standby Mode)

See Options to test this functionality.
You can also activate this option by using the Expert-button and selecting Wake-On-LAN > Enabled."
                );
            }

            BeginInvoke(() =>
            {
                device.PowerOnAfterStartup = clbLgPower.GetItemChecked(0);
                device.PowerOnAfterResume = clbLgPower.GetItemChecked(1);
                device.PowerOffOnShutdown = clbLgPower.GetItemChecked(2);
                device.PowerOffOnStandby = clbLgPower.GetItemChecked(3);
                device.PowerOffOnScreenSaver = clbLgPower.GetItemChecked(4);
                device.PowerOnAfterScreenSaver = clbLgPower.GetItemChecked(5);
                device.PowerOnAfterManualPowerOff = clbLgPower.GetItemChecked(6);
                device.TriggersEnabled = clbLgPower.GetItemChecked(7);
                device.PowerOffByWindows = clbLgPower.GetItemChecked(8);
                device.PowerOnByWindows = clbLgPower.GetItemChecked(9);
                device.UseSecureConnection = clbLgPower.GetItemChecked(10);

                _lgService.InstallEventHandlers();

                if (e.Index == 10)
                {
                    _lgService.RefreshDevices().ContinueWith((_) => BeginInvoke(() => FillLgDevices()));
                }
            });
        }

        private void edtLgPowerOnAfterResumeDelay_ValueChanged(object sender, EventArgs e)
        {
            _lgService.Config.PowerOnRetries = (int)edtLgMaxPowerOnRetries.Value;
        }

        private void chkFixChromeFonts_CheckedChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                _config.FixChromeFonts = chkFixChromeFonts.Checked;
                if (chkFixChromeFonts.Checked)
                {
                    Utils.ExecuteElevated(StartUpParams.ActivateChromeFontFixParam);
                }
                else
                {
                    Utils.ExecuteElevated(StartUpParams.DeactivateChromeFontFixParam);
                }
            }
        }

        private void btnLGTestPower_Click(object sender, EventArgs e)
        {
            var text =
@"The TV will now power off. Please wait for the TV to be powered off completely (relay click) and press ENTER to wake it again.
For waking up to work, you need to activate the following setting on the TV:

Connection > Mobile TV On > Turn on via Wi-Fi

It will also work over a wired connection.
You can also activate this option by using the Expert-button and selecting Wake-On-LAN > Enabled.

Do you want to continue?";

            if (MessageForms.QuestionYesNo(text) == DialogResult.Yes)
            {
                Utils.WaitForTask(_lgService.PowerOff());

                MessageForms.InfoOk("Press ENTER to wake the TV.");

                _lgService.WakeSelectedDevice();
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
            if (tcMain.SelectedTab == tabNVIDIA)
            {
                UpdateDisplayInfoItems();
            }
            else if (tcMain.SelectedTab == tabAMD)
            {
                UpdateDisplayInfoItemsAmd();
            }
            else if (tcMain.SelectedTab == tabLog)
            {
                LoadLog();
            }
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            var logType = cbxLogType.SelectedIndex;

            if (logType == -1)
            {
                return;
            }

            if (logType == 0)
            {
                var filename = Program.LogFilename;
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }
            }
            else
            {
                PipeUtils.SendMessage(SvcMessageType.ClearLog);
            }
            edtLog.Clear();
        }

        private void edtLgDeviceFilter_TextChanged(object sender, EventArgs e)
        {
            if (_lgService != null)
            {
                _lgService.Config.DeviceSearchKey = edtLgDeviceFilter.Text;
            }
        }

        private void btnLgDeviceFilterRefresh_Click(object sender, EventArgs e)
        {
            RefreshLgDevices();
        }

        private void RefreshLgDevices()
        {
            _lgService.RefreshDevices(false).ContinueWith((task) => BeginInvoke(() => FillLgDevices()));
        }

        private void FillGradient()
        {
            if (pbGradient.Image == null)
            {
                pbGradient.Image = Utils.GenerateGradientBitmap(pbGradient.Width, pbGradient.Height);
            }
        }

        private void ApplyDitheringOptions()
        {
            if (_updatingDitherSettings)
            {
                return;
            }

            var state = chkDitheringEnabled.CheckState switch { CheckState.Checked => NvDitherState.Enabled, CheckState.Unchecked => NvDitherState.Disabled, _ => NvDitherState.Auto };
            var bitDepth = cbxDitheringBitDepth.SelectedIndex;
            var mode = cbxDitheringMode.SelectedIndex;
            var display = ((NvDisplayInfo)cbxDitheringDisplay.SelectedItem)?.Display;

            if (_nvService.SetDithering(state, (uint)bitDepth, (uint)(mode > -1 ? mode : (int)NvDitherMode.Temporal), currentDisplay: display) && state == NvDitherState.Auto)
            {
                UpdateDitherSettings();
            }
        }

        private void cbxDitheringBitDepth_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyDitheringOptions();
        }

        private void cbxDitheringMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyDitheringOptions();
        }

        private void miNvDithering6bit_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;

            var preset = GetSelectedNvPreset(true);

            var ditheringBits = uint.Parse(item.Tag.ToString());

            if (preset.IsDisplayPreset)
            {
                _nvService.SetDithering(preset.ditheringEnabled ? NvDitherState.Enabled : NvDitherState.Disabled, ditheringBits, preset.ditheringMode);

                UpdateDisplayInfoItems();
                return;
            }

            preset.ditheringBits = ditheringBits;

            AddOrUpdateItem();
        }

        private void spatial1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;

            var preset = GetSelectedNvPreset(true);

            var ditheringMode = uint.Parse(item.Tag.ToString());

            if (preset.IsDisplayPreset)
            {
                _nvService.SetDithering(preset.ditheringEnabled ? NvDitherState.Enabled : NvDitherState.Disabled, preset.ditheringBits, ditheringMode);

                UpdateDisplayInfoItems();
                return;
            }

            preset.ditheringMode = ditheringMode;

            AddOrUpdateItem();
        }

        private void AfterInitialized()
        {
            ApplyNvPresetOnStartup();
            ApplyAmdPresetOnStartup();
            if (_trayIcon.Visible)
            {
                CheckForUpdates();
            }
        }

        private void CheckForUpdates()
        {
            if (!_config.CheckForUpdates || _checkedForUpdates || Debugger.IsAttached)
            {
                return;
            }

            _checkedForUpdates = true;

            var _ = Utils.GetRestJsonAsync("https://api.github.com/repos/maassoft/colorcontrol/releases/latest", InitHandleCheckForUpdates);
        }

        private void InitHandleCheckForUpdates(dynamic latest)
        {
            BeginInvoke(() => HandleCheckForUpdates(latest));
        }

        private void HandleCheckForUpdates(dynamic latest)
        {
            if (latest?.tag_name == null)
            {
                return;
            }

            var currentVersion = Application.ProductVersion;

            var newVersion = latest.tag_name.Value.Substring(1);
            if (newVersion.CompareTo(currentVersion) > 0)
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

            var result = PipeUtils.SendMessage(message, 10000);

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

        private void ApplyNvPresetOnStartup(int attempts = 5)
        {

            var presetIdOrName = !string.IsNullOrEmpty(StartUpParams.NvidiaPresetIdOrName) ? StartUpParams.NvidiaPresetIdOrName : _config.NvPresetId_ApplyOnStartup.ToString();

            if (!string.IsNullOrEmpty(presetIdOrName))
            {
                var preset = _nvService?.GetPresetByIdOrName(presetIdOrName);
                if (preset == null)
                {
                    if (string.IsNullOrEmpty(StartUpParams.NvidiaPresetIdOrName))
                    {
                        _config.NvPresetId_ApplyOnStartup = 0;
                    }
                }
                else if (_nvService != null)
                {
                    if (_nvService.HasDisplaysAttached())
                    {
                        ApplyNvPreset(preset);
                    }
                    else
                    {
                        attempts--;
                        if (attempts > 0)
                        {
                            Task.Run(async () =>
                            {
                                await Task.Delay(2000);
                                BeginInvoke(() => ApplyNvPresetOnStartup(attempts));
                            });
                        }
                    }
                }
            }
        }

        private void ApplyAmdPresetOnStartup(int attempts = 5)
        {
            var presetIdOrName = !string.IsNullOrEmpty(StartUpParams.AmdPresetIdOrName) ? StartUpParams.AmdPresetIdOrName : _config.AmdPresetId_ApplyOnStartup.ToString();

            if (!string.IsNullOrEmpty(presetIdOrName))
            {
                var preset = _amdService?.GetPresetByIdOrName(presetIdOrName);
                if (preset == null)
                {
                    if (string.IsNullOrEmpty(StartUpParams.AmdPresetIdOrName))
                    {
                        _config.AmdPresetId_ApplyOnStartup = 0;
                    }
                }
                else if (_amdService != null)
                {
                    if (_amdService.HasDisplaysAttached())
                    {
                        ApplyAmdPreset(preset);
                    }
                    else
                    {
                        attempts--;
                        if (attempts > 0)
                        {
                            Task.Run(async () =>
                            {
                                await Task.Delay(2000);
                                BeginInvoke(() => ApplyAmdPresetOnStartup(attempts));
                            });
                        }
                    }
                }
            }
        }

        private void miNvPresetApplyOnStartup_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedNvPreset();
            _config.NvPresetId_ApplyOnStartup = miNvPresetApplyOnStartup.Checked ? preset.id : 0;

            AddOrUpdateItem();
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            GlobalSave();
        }

        private void btnApplyAmd_Click(object sender, EventArgs e)
        {
            ApplySelectedAmdPreset();
        }

        private void btnChangeAmd_Click(object sender, EventArgs e)
        {
            mnuAmdPresets.Show(btnChangeAmd, btnChangeAmd.PointToClient(Cursor.Position));
        }

        private void edtAmdShortcut_TextChanged(object sender, EventArgs e)
        {
            var text = edtAmdShortcut.Text;

            var preset = GetSelectedAmdPreset();

            if (preset == null || string.IsNullOrEmpty(text))
            {
                edtAmdShortcut.ForeColor = SystemColors.WindowText;
            }
            else
            {
                edtAmdShortcut.ForeColor = ShortCutExists(text, preset.id) ? Color.Red : SystemColors.WindowText;
            }
        }

        private bool ShortCutExists(string shortcut, int presetId = 0)
        {
            return
                (_nvService?.GetPresets().Any(x => x.id != presetId && shortcut.Equals(x.shortcut)) ?? false) ||
                (_amdService?.GetPresets().Any(x => x.id != presetId && shortcut.Equals(x.shortcut)) ?? false) ||
                (_lgService?.GetPresets().Any(x => x.id != presetId && shortcut.Equals(x.shortcut)) ?? false);
        }

        private void edtShortcutLg_TextChanged(object sender, EventArgs e)
        {
            var text = edtShortcutLg.Text;

            var preset = GetSelectedLgPreset();

            if (preset == null || string.IsNullOrEmpty(text))
            {
                edtShortcutLg.ForeColor = SystemColors.WindowText;
            }
            else
            {
                edtShortcutLg.ForeColor = ShortCutExists(text, preset.id) ? Color.Red : SystemColors.WindowText;
            }
        }

        private void btnSetAmdShortcut_Click(object sender, EventArgs e)
        {
            var shortcut = edtAmdShortcut.Text.Trim();

            if (!Utils.ValidateShortcut(shortcut))
            {
                return;
            }

            var preset = GetSelectedAmdPreset();

            var name = edtAmdPresetName.Text.Trim();

            if (!string.IsNullOrEmpty(name) && _amdService.GetPresets().Any(x => x.id != preset.id && x.name != null && x.name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                MessageForms.WarningOk("The name must be unique.");
                return;
            }

            if (string.IsNullOrEmpty(name) && chkAmdQuickAccess.Checked)
            {
                MessageForms.WarningOk("A name must be entered before enabling Quick Access.");
                return;
            }

            var clear = !string.IsNullOrEmpty(preset.shortcut);

            preset.shortcut = shortcut;
            preset.name = name;
            preset.ShowInQuickAccess = chkAmdQuickAccess.Checked;

            AddOrUpdateItemAmd();

            Utils.RegisterShortcut(Handle, preset.id, preset.shortcut, clear);
        }

        private void btnCloneAmd_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedAmdPreset();

            var newPreset = preset.Clone();
            AddOrUpdateItemAmd(newPreset);
        }

        private void btnDeleteAmd_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedAmdPreset();

            if (preset == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(preset.shortcut))
            {
                WinApi.UnregisterHotKey(Handle, preset.id);
            }

            _amdService.GetPresets().Remove(preset);

            var item = lvAmdPresets.SelectedItems[0];
            lvAmdPresets.Items.Remove(item);
        }

        private void mnuAmdPresets_Opening(object sender, CancelEventArgs e)
        {
            var preset = GetSelectedAmdPreset();

            miAmdApply.Enabled = preset != null;
            miAmdPresetApplyOnStartup.Enabled = preset != null;
            mnuAmdDisplay.Enabled = preset != null;
            mnuAmdColorSettings.Enabled = preset != null;
            mnuAmdRefreshRate.Enabled = preset != null;
            mnuAmdDithering.Enabled = preset != null;
            mnuAmdHDR.Enabled = preset != null;

            if (preset != null)
            {
                miAmdPresetApplyOnStartup.Checked = _config.AmdPresetId_ApplyOnStartup == preset.id;

                if (mnuAmdDisplay.DropDownItems.Count == 1)
                {
                    var displays = _amdService.GetDisplays();
                    for (var i = 0; i < displays.Count; i++)
                    {
                        var display = displays[i];
                        var name = _amdService.GetFullDisplayName(display);

                        var item = mnuAmdDisplay.DropDownItems.Add(name);
                        item.Tag = display;
                        item.Click += displayMenuItemAmd_Click;
                    }
                }

                FormUtils.BuildDropDownMenu(mnuAmdColorSettings, "Color depth", typeof(ADLColorDepth), preset, "colorDepth", amdPresetColorDataMenuItem_Click);
                FormUtils.BuildDropDownMenu(mnuAmdColorSettings, "Pixel format", typeof(ADLPixelFormat), preset, "pixelFormat", amdPresetColorDataMenuItem_Click);

                if (preset.displayName != _lastDisplayRefreshRates)
                {
                    while (mnuAmdRefreshRate.DropDownItems.Count > 1)
                    {
                        mnuAmdRefreshRate.DropDownItems.RemoveAt(mnuAmdRefreshRate.DropDownItems.Count - 1);
                    }
                }

                if (mnuAmdRefreshRate.DropDownItems.Count == 1)
                {
                    var refreshRates = _amdService.GetAvailableRefreshRates(preset);
                    _lastDisplayRefreshRates = preset.displayName;

                    foreach (var refreshRate in refreshRates)
                    {
                        var item = mnuAmdRefreshRate.DropDownItems.Add(refreshRate.ToString() + "Hz");
                        item.Tag = refreshRate;
                        item.Click += refreshRateMenuItemAmd_Click;
                    }
                }

                miAmdPrimaryDisplay.Checked = preset.primaryDisplay;
                foreach (var item in mnuAmdDisplay.DropDownItems)
                {
                    if (item is ToolStripMenuItem)
                    {
                        var menuItem = (ToolStripMenuItem)item;
                        if (menuItem.Tag != null)
                        {
                            menuItem.Checked = ((ADLDisplayInfo)menuItem.Tag).DisplayName.Equals(preset.displayName);
                        }
                    }
                }

                miAmdColorSettingsIncluded.Checked = preset.applyColorData;

                miAmdRefreshRateIncluded.Checked = preset.applyRefreshRate;
                foreach (var item in mnuAmdRefreshRate.DropDownItems)
                {
                    if (item is ToolStripMenuItem)
                    {
                        var menuItem = (ToolStripMenuItem)item;
                        if (menuItem.Tag != null)
                        {
                            menuItem.Checked = (uint)menuItem.Tag == preset.refreshRate;
                        }
                    }
                }

                miAmdDitheringIncluded.Checked = preset.applyDithering;

                FormUtils.BuildDropDownMenu(mnuAmdDithering, "Mode", typeof(ADLDitherState), preset, "ditherState", amdPresetColorDataMenuItem_Click);

                miAmdHDRIncluded.Checked = preset.applyHDR;
                miAmdHDREnabled.Checked = preset.HDREnabled;
                miAmdHDRToggle.Checked = preset.toggleHDR;
            }
        }

        private void miAmdPresetApplyOnStartup_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedAmdPreset();
            _config.AmdPresetId_ApplyOnStartup = miAmdPresetApplyOnStartup.Checked ? preset.id : 0;

            AddOrUpdateItemAmd();
        }

        private void lvAmdPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            var preset = GetSelectedAmdPreset();
            var enabled = preset != null;

            FormUtils.EnableControls(tabAMD, enabled, new List<Control> { lvAmdPresets, btnAddAmd });

            if (enabled)
            {
                edtAmdPresetName.Text = preset.name;
                edtAmdShortcut.Text = preset.shortcut;
                chkAmdQuickAccess.Checked = preset.ShowInQuickAccess;
            }
            else
            {
                edtAmdPresetName.Text = string.Empty;
                edtAmdShortcut.Text = string.Empty;
                chkAmdQuickAccess.Checked = false;
            }
        }

        private void amdPresetColorDataMenuItem_Click(object sender, EventArgs e)
        {
            var menuItem = (ToolStripMenuItem)sender;
            var property = (PropertyInfo)menuItem.OwnerItem.Tag;

            var value = menuItem.Tag;

            var preset = GetSelectedAmdPreset();

            property.SetValue(preset, value);

            AddOrUpdateItemAmd();
        }

        private void miAmdColorSettingsIncluded_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedAmdPreset();

            preset.applyColorData = !preset.applyColorData;

            AddOrUpdateItemAmd();
        }

        private void miAmdRefreshRateIncluded_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedAmdPreset();

            preset.applyRefreshRate = !preset.applyRefreshRate;

            AddOrUpdateItemAmd();
        }

        private void miAmdDitheringIncluded_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedAmdPreset();

            preset.applyDithering = !preset.applyDithering;

            AddOrUpdateItemAmd();
        }

        private void miAmdHDRIncluded_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedAmdPreset();

            preset.applyHDR = !preset.applyHDR;

            AddOrUpdateItemAmd();
        }

        private void miAmdHDRToggle_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedAmdPreset();

            preset.toggleHDR = !preset.toggleHDR;

            if (preset.toggleHDR)
            {
                preset.HDREnabled = false;
            }

            AddOrUpdateItemAmd();
        }

        private void miAmdHDREnabled_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedAmdPreset();

            preset.HDREnabled = !preset.HDREnabled;
            preset.toggleHDR = false;

            AddOrUpdateItemAmd();
        }

        private void btnAddAmd_Click(object sender, EventArgs e)
        {
            var presets = AmdPreset.GetDefaultPresets();
            var added = false;

            foreach (var preset in presets)
            {
                AddOrUpdateItemAmd(preset);
            }

            if (added)
            {
                MessageForms.InfoOk("Missing presets added.");
            }
            else
            {
                MessageForms.InfoOk("All presets for every color setting already exist.");
            }
        }

        private void chkMinimizeToSystemTray_CheckedChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                _config.MinimizeToTray = chkMinimizeToSystemTray.Checked;
                _trayIcon.Visible = _config.MinimizeToTray;
            }
        }

        private void miNvCopyId_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedNvPreset();

            if (preset != null)
            {
                Clipboard.SetText(preset.id.ToString());
            }
        }

        private void miAmdCopyId_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedAmdPreset();

            if (preset != null)
            {
                Clipboard.SetText(preset.id.ToString());
            }
        }

        private void btnLgAddDevice_Click(object sender, EventArgs e)
        {
            var values = MessageForms.ShowDialog("Add tv", new[] { "Name", "Ip-address", "MAC-address" }, ValidateAddDevice);
            if (values.Any())
            {
                var device = new LgDevice(values[0].Value.ToString(), values[1].Value.ToString(), values[2].Value.ToString());

                var form = MessageForms.ShowProgress("Connecting to device...");

                var result = false;
                Enabled = false;
                try
                {
                    var task = device.TestConnection();

                    result = Utils.WaitForTask(task);
                }
                finally
                {
                    form.Close();
                    Enabled = true;
                }

                if (!result && MessageForms.QuestionYesNo("Unable to connect to the device. Are you sure you want to add it?") != DialogResult.Yes)
                {
                    return;
                }

                _lgService.AddCustomDevice(device);

                FillLgDevices();
            }
        }

        private string ValidateAddDevice(IEnumerable<MessageForms.FieldDefinition> values)
        {
            if (values.Any(v => string.IsNullOrEmpty(v.Value?.ToString())))
            {
                return "Please fill in all the fields";
            }

            return null;
        }

        private void btnLgRemoveDevice_Click(object sender, EventArgs e)
        {
            if (MessageForms.QuestionYesNo("Are you sure you want to remove this device?") != DialogResult.Yes)
            {
                return;
            }

            var device = _lgService.SelectedDevice;

            if (device != null)
            {
                _lgService.RemoveCustomDevice(device);

                FillLgDevices();
            }
        }

        private void mnuLgButtons_Opening(object sender, CancelEventArgs e)
        {
            mnuLgActions.DropDownItems.Clear();

            var preset = GetSelectedLgPreset();

            if (preset == null)
            {
                return;
            }

            var device = _lgService.GetPresetDevice(preset);

            BuildLgActionMenu(device, mnuLgActions.DropDownItems, mnuLgActions.Name, miLgAddAction_Click);
            BuildServicePresetsMenu(mnuLgNvPresets, _nvService, "NVIDIA", miLgAddNvPreset_Click);
            BuildServicePresetsMenu(mnuLgAmdPresets, _amdService, "AMD", miLgAddAmdPreset_Click);
        }

        private void btnLgDeviceConvertToCustom_Click(object sender, EventArgs e)
        {
            if (MessageForms.QuestionYesNo(
@"This will convert the automatically detected device to a custom variant.
This means that the device will remain here even if it is not detected anymore.
Do you want to continue?"
               ) != DialogResult.Yes)
            {
                return;
            }

            _lgService.SelectedDevice.ConvertToCustom();

            FillLgDevices();
        }

        private static void BuildServicePresetsMenu<T>(ToolStripMenuItem menu, ServiceBase<T> service, string name, EventHandler eventHandler) where T : PresetBase, new()
        {
            menu.DropDownItems.Clear();

            foreach (var nvPreset in service?.GetPresets() ?? new List<T>())
            {
                var text = nvPreset.name;

                if (!string.IsNullOrEmpty(text))
                {
                    var item = menu.DropDownItems.Add(text);
                    item.Tag = nvPreset;
                    item.Click += eventHandler;
                }
            }

            menu.Visible = service != null;

            menu.Text = menu.DropDownItems.Count > 0 ? $"{name} presets" : $"{name} presets (no named presets found)";
        }

        private void btnLgRemoteControlShow_Click(object sender, EventArgs e)
        {
        }

        private void chkLgRemoteControlShow_CheckedChanged(object sender, EventArgs e)
        {
            scLgController.Panel2Collapsed = !chkLgRemoteControlShow.Checked;
            _lgService.Config.ShowRemoteControl = chkLgRemoteControlShow.Checked;
        }

        private void mnuLgExpert_Opening(object sender, CancelEventArgs e)
        {
            var device = _lgService.SelectedDevice;

            var eligibleModels = new[] { "B9", "C9", "E9", "W9", "C2", "G2" };
            var eligibleModelsEnable = new[] { "B9", "C9", "E9", "W9" };

            var visible = eligibleModels.Any(m => device?.ModelName?.Contains(m) == true);
            var enableVisible = eligibleModelsEnable.Any(m => device?.ModelName?.Contains(m) == true);
            mnuLgOLEDMotionPro.Visible = visible;
            miLgEnableMotionPro.Visible = enableVisible;
            miLgExpertSeparator1.Visible = visible;

            BuildLgActionMenu(device, mnuLgExpert.Items, mnuLgExpert.Name, btnLgExpertColorGamut_Click, _lgService.Config.ShowAdvancedActions, true);
        }

        private void BuildLgActionMenu(LgDevice device, ToolStripItemCollection parent, string parentName, EventHandler clickEvent, bool showAdvanced = false, bool showGameBar = false)
        {
            if (device == null)
            {
                return;
            }

            var actions = device.GetInvokableActions(showAdvanced);
            var gameBarActions = device.GetInvokableActionsForGameBar();
            var activatedGameBarActions = device.GetActionsForGameBar();

            const string gameBarName = "miGameBar";

            var expertActions = actions.Where(a => a.EnumType != null || a.MaxValue > a.MinValue).ToList();

            var categories = expertActions.Select(a => a.Category ?? "misc").Where(c => !string.IsNullOrEmpty(c)).Distinct();

            foreach (var category in categories)
            {
                var catMenuItem = FormUtils.BuildDropDownMenuEx(parent, parentName, Utils.FirstCharUpperCase(category), null, null, category);

                foreach (var action in expertActions.Where(a => (a.Category ?? "misc") == category))
                {
                    if (!showGameBar)
                    {
                        var text = action.Title ?? action.Name;

                        var item = catMenuItem.DropDownItems.Add(text);
                        item.Tag = action;
                        item.Click += clickEvent;
                        continue;
                    }

                    var menu = FormUtils.BuildDropDownMenuEx(catMenuItem.DropDownItems, catMenuItem.Name, action.Title, action.EnumType, clickEvent, action, (int)action.MinValue, (int)action.MaxValue, action.NumberOfValues > 1);

                    if (!gameBarActions.Contains(action))
                    {
                        continue;
                    }

                    var itemName = $"{menu.Name}_{gameBarName}";
                    var gameBarItem = (ToolStripMenuItem)menu.DropDownItems.Find(itemName, false).FirstOrDefault();

                    if (gameBarItem == null)
                    {
                        var separator = new ToolStripSeparator();
                        menu.DropDownItems.Add(separator);

                        gameBarItem = (ToolStripMenuItem)menu.DropDownItems.Add("Show in Game Bar", null, miLgGameBarToggle_Click);
                        gameBarItem.CheckOnClick = true;
                        gameBarItem.Checked = activatedGameBarActions.Contains(action);
                        gameBarItem.Name = itemName;
                        gameBarItem.Tag = action.Name;
                    }
                }
            }

            if (!showAdvanced)
            {
                return;
            }

            var presetActions = actions.Where(a => a.Preset != null);

            if (presetActions.Any() && parent.Find("miLgExpertActionsSeparator", false).Length == 0)
            {
                parent.Add(new ToolStripSeparator
                {
                    Name = "miLgExpertActionsSeparator"
                });
            }

            foreach (var presetAction in presetActions)
            {
                var menu = FormUtils.BuildDropDownMenuEx(parent, parentName, presetAction.Name, null, btnLgExpertPresetAction_Click, presetAction.Preset);
            }
        }

        private void miLgGameBarToggle_Click(object sender, EventArgs e)
        {
            var device = _lgService.SelectedDevice;

            if (device == null)
            {
                return;
            }

            var item = sender as ToolStripMenuItem;
            if (item.Checked)
            {
                device.AddGameBarAction((string)item.Tag);
            }
            else
            {
                device.RemoveGameBarAction((string)item.Tag);
            }
        }

        private void btnLgExpert_Click(object sender, EventArgs e)
        {
            mnuLgExpert.Show(btnLgExpert, btnLgExpert.PointToClient(Cursor.Position));
        }

        private void btnLgExpertColorGamut_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripItem;
            var action = item.Tag as LgDevice.InvokableAction;
            var value = item.AccessibleName ?? item.Text;

            if (action.NumberOfValues > 1)
            {
                ApplyLgExpertValueRange(action);
                return;
            }

            try
            {
                _lgService.SelectedDevice?.ExecuteAction(action, new[] { value });
            }
            catch (InvalidOperationException ex)
            {
                Logger.Error(ex);

                MessageForms.ErrorOk("Error exectuting action: " + ex.Message);
            }
        }

        private void ApplyLgExpertValueRange(LgDevice.InvokableAction action)
        {
            var value = ShowLgActionForm(action);

            if (value == null)
            {
                return;
            }

            var values = value.Split(";");

            _lgService.SelectedDevice?.ExecuteAction(action, values);
        }

        private void btnLgExpertPresetAction_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripItem;
            var preset = (LgPreset)item.Tag;

            _lgService.ApplyPreset(preset).ConfigureAwait(false);
        }

        private void miLgEnableMotionPro_Click(object sender, EventArgs e)
        {
            if (MessageForms.QuestionYesNo("Are you sure you want to enable OLED Motion Pro? This app and its creator are in no way accountable for any damages it may cause to your tv.") == DialogResult.Yes)
            {
                _lgService.SelectedDevice?.SetOLEDMotionPro("OLED Motion Pro");

                MessageForms.InfoOk("Setting applied.");
            }
        }

        private void miLgDisableMotionPro_Click(object sender, EventArgs e)
        {
            _lgService.SelectedDevice?.SetOLEDMotionPro("OLED Motion");

            MessageForms.InfoOk("Setting applied.");
        }

        private void cbxLgApps_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ((char)Keys.Back))
            {
                cbxLgApps.SelectedIndex = -1;
            }
        }

        private void btnLgPresetEditTriggerConditions_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedLgPreset();

            if (preset == null)
            {
                return;
            }

            var dropDownValues = Utils.GetDescriptions<PresetConditionType>(fromValue: 1);

            var values = MessageForms.ShowDialog("Set trigger conditions", new[] {
                    new MessageForms.FieldDefinition
                    {
                        Label = "Set desired trigger conditions",
                        FieldType = MessageForms.FieldType.Flags,
                        Values = dropDownValues,
                        Value = edtLgPresetTriggerConditions.Tag ?? 0
                    }
                });

            if (!values.Any())
            {
                return;
            }

            var value = (PresetConditionType)values.First().Value;
            edtLgPresetTriggerConditions.Tag = (PresetConditionType)values.First().Value;
            edtLgPresetTriggerConditions.Text = Utils.GetDescriptions<PresetConditionType>((int)value).Join(", ");
        }

        private void chkCheckForUpdates_CheckedChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                _config.CheckForUpdates = chkCheckForUpdates.Checked;
                chkAutoInstallUpdates.Enabled = _config.CheckForUpdates && _config.ElevationMethod == ElevationMethod.UseService;
            }
        }

        private void chkLgShowAdvancedActions_CheckedChanged(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            if (chkLgShowAdvancedActions.Checked)
            {
                if (MessageForms.QuestionYesNo(
@"Are you sure you want to enable the advanced actions under the Expert-button?
These actions include:
- InStart service menu
- EzAdjust service menu
- Software Update-app with firmware downgrade functionality enabled

These features may cause irreversible damage to your tv and will void your warranty.
This app and its creator are in no way accountable for any damages it may cause to your tv."
                ) != DialogResult.Yes)
                {
                    chkLgShowAdvancedActions.Checked = false;
                    return;
                }
                MessageForms.InfoOk(
@"Advanced actions activated.
The InStart and Software Update items are now visible under the Expert-button."
                );
            }

            _lgService.Config.ShowAdvancedActions = chkLgShowAdvancedActions.Checked;
        }

        private void ToggleGameBar()
        {
            if (_gameBarForm == null || !_gameBarForm.Visible)
            {
                if (_gameBarForm == null || _gameBarForm.IsDisposed)
                {
                    if (_lgService?.SelectedDevice == null)
                    {
                        return;
                    }

                    _gameBarForm = new LgGameBar(_lgService);
                }

                _gameBarForm.Show();
                _gameBarForm.Activate();
            }
            else
            {
                _gameBarForm?.Hide();
            }
        }

        private void btnLgGameBar_Click(object sender, EventArgs e)
        {
            ToggleGameBar();
        }

        private void edtLgGameBarShortcut_KeyDown(object sender, KeyEventArgs e)
        {
            ((TextBox)sender).Text = Utils.FormatKeyboardShortcut(e);
        }

        private void edtLgGameBarShortcut_KeyUp(object sender, KeyEventArgs e)
        {
            Utils.HandleKeyboardShortcutUp(e);
        }

        private void edtLgGameBarShortcut_TextChanged(object sender, EventArgs e)
        {
            var text = edtLgGameBarShortcut.Text;

            var blnOk = string.IsNullOrEmpty(text) || !ShortCutExists(text);
            edtLgGameBarShortcut.ForeColor = blnOk ? Color.Red : SystemColors.WindowText;

            if (blnOk)
            {
                _lgService.Config.GameBarShortcut = text;

                if (string.IsNullOrEmpty(text))
                {
                    WinApi.UnregisterHotKey(Handle, SHORTCUTID_GAMEBAR);
                }
                else
                {
                    Utils.RegisterShortcut(Handle, SHORTCUTID_GAMEBAR, text);
                }
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

        private void chkDitheringEnabled_CheckStateChanged(object sender, EventArgs e)
        {
            cbxDitheringBitDepth.Enabled = chkDitheringEnabled.CheckState == CheckState.Checked;
            cbxDitheringMode.Enabled = chkDitheringEnabled.CheckState == CheckState.Checked;

            ApplyDitheringOptions();
        }

        private void btnLgDeviceOptionsHelp_Click(object sender, EventArgs e)
        {
            MessageForms.InfoOk(
@"Notes:
- power on after startup requires ""Automatically start after login"" - see Options
- power on after resume from standby may need some retries for waking TV - see Options
- power off on shutdown: because this app cannot detect a restart, restarting could also trigger this. Hold down Ctrl on restart to prevent power off.
- Use Windows power settings: powering on/off of TV will follow Windows settings. If activated, it's better to disable all other power options except power off on shutdown.
");
        }

        private void edtLgOptionShutdownDelay_ValueChanged(object sender, EventArgs e)
        {
            _lgService.Config.ShutdownDelay = (int)edtLgOptionShutdownDelay.Value;
        }

        private void cbxLgPcHdmiPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_lgService?.SelectedDevice == null)
            {
                return;
            }

            _lgService.SelectedDevice.HDMIPortNumber = cbxLgPcHdmiPort.SelectedIndex;
        }

        private void lvLgPresets_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            var listView = (ListView)sender;
            var sorter = (ListViewColumnSorter)listView.ListViewItemSorter;

            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == sorter.SortColumn)
            {
                //if (sorter.Order == SortOrder.None)
                //{
                //    sorter.Order = SortOrder.Ascending;
                //}
                // Reverse the current sort direction for this column.
                if (sorter.Order == SortOrder.Ascending)
                {
                    sorter.Order = SortOrder.Descending;
                }
                else
                {
                    sorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                sorter.SortColumn = e.Column;
                sorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            listView.Sort();
        }

        private void miNvResolutionIncluded_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedNvPreset();

            preset.applyResolution = !preset.applyResolution;

            AddOrUpdateItem();
        }

        private void miNvDriverSettingsIncluded_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedNvPreset();

            preset.applyDriverSettings = !preset.applyDriverSettings;

            AddOrUpdateItem();
        }

        private void driverSettingValueNvMenuItem_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedNvPreset(true);

            var menuItem = (ToolStripItem)sender;
            var parentMenuItem = menuItem.OwnerItem as ToolStripMenuItem;

            var settingItem = (SettingItem)parentMenuItem.Tag;

            if (menuItem.Tag == null)
            {
                if (!preset.IsDisplayPreset)
                {
                    preset.ResetDriverSetting(settingItem.SettingId);
                    AddOrUpdateItem();
                }
                return;
            }

            var intValue = menuItem.Tag is SettingValue<uint> intSettingValue ? intSettingValue.Value :
                menuItem.Tag is SettingValue<byte[]> byteSettingValue ? BitConverter.ToUInt32(byteSettingValue.Value) : 0;

            if (preset.IsDisplayPreset)
            {
                _nvService.SetDriverSetting(settingItem.SettingId, intValue);

                UpdateDisplayInfoItems();

                return;
            }

            preset.UpdateDriverSetting(settingItem, intValue);

            AddOrUpdateItem();
        }

        private void driverSettingFrameRateNvMenuItem_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedNvPreset(true);

            var menuItem = (ToolStripItem)sender;
            var parentMenuItem = menuItem.OwnerItem as ToolStripMenuItem;

            var settingItem = (SettingItem)parentMenuItem.Tag;

            var settingMeta = _nvService.GetSettingMeta(settingItem.SettingId);

            uint presetValue;
            if (preset.IsDisplayPreset)
            {
                (presetValue, _) = settingMeta.ToIntValue(settingItem.ValueText);
            }
            else
            {
                presetValue = preset.driverSettings.GetValueOrDefault(settingItem.SettingId, settingMeta.DefaultDwordValue);
            }

            var settingValue = SelectNvDriverValue(settingMeta, presetValue);

            if (settingValue == SettingMeta.UnsetDwordValue)
            {
                return;
            }


            if (preset.IsDisplayPreset)
            {
                _nvService.SetDriverSetting(settingItem.SettingId, settingValue);

                UpdateDisplayInfoItems();

                return;
            }

            preset.UpdateDriverSetting(settingItem, settingValue);

            AddOrUpdateItem();
        }

        private uint SelectNvDriverValue(SettingMeta settingMeta, uint currentValue)
        {
            var field = new MessageForms.FieldDefinition
            {
                MinValue = settingMeta.DwordValues.First().Value,
                MaxValue = settingMeta.DwordValues.Last().Value,
                Label = settingMeta.SettingName,
                FieldType = MessageForms.FieldType.Numeric,
                Value = currentValue
            };

            var resultFields = MessageForms.ShowDialog(field.Label, new[] { field });

            if (!resultFields.Any())
            {
                return SettingMeta.UnsetDwordValue;
            }

            var settingValue = (uint)Utils.ParseInt((string)resultFields.First().Value);

            return settingValue;
        }

        private void mnuLgProgram_Click(object sender, EventArgs e)
        {
            var file = Utils.SelectFile();

            if (file != null)
            {
                FormUtils.AddStepToTextBox(edtStepsLg, $"StartProgram({file.FullName})");
            }
        }

        private void lvGamePresets_SelectedIndexChanged(object sender, EventArgs _)
        {
            var preset = GetSelectedGamePreset();
            var enabled = preset != null;

            FormUtils.EnableControls(tabGameLauncher, enabled, new List<Control> { lvGamePresets, btnGameAdd });

            if (preset != null)
            {
                edtGameName.Text = preset.name;
                edtGamePath.Text = preset.Path;
                edtGameParameters.Text = preset.Parameters;
                chkGameRunAsAdmin.Checked = preset.RunAsAdministrator;
                chkGameQuickAccess.Checked = preset.ShowInQuickAccess;
                ShowGameSteps();
            }
            else
            {
                edtGameName.Text = string.Empty;
                edtGamePath.Text = string.Empty;
                edtGameParameters.Text = string.Empty;
                chkGameRunAsAdmin.Checked = false;
                chkGameQuickAccess.Checked = false;
                edtGamePrelaunchSteps.Text = string.Empty;
            }
        }

        private void lvGamePresets_DoubleClick(object sender, EventArgs e)
        {
            ApplySelectedGamePreset();
        }

        private void btnGameLaunch_Click(object sender, EventArgs e)
        {
            ApplySelectedGamePreset();
        }

        private void btnGameClone_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedGamePreset();

            var newPreset = preset.Clone();

            AddOrUpdateItemGame(newPreset);
        }

        private void btnGameAdd_Click(object sender, EventArgs e)
        {
            var file = Utils.SelectFile();

            AddGamePreset(file);
        }

        private void AddGamePreset(FileInfo file)
        {
            var preset = _gameService.CreateNewPreset();

            if (file != null)
            {
                var versionInfo = FileVersionInfo.GetVersionInfo(file.FullName);

                preset.Path = file.FullName;

                var fileName = Path.GetFileNameWithoutExtension(preset.Path);

                preset.name = !string.IsNullOrEmpty(versionInfo.FileDescription) ? versionInfo.FileDescription :
                    !string.IsNullOrEmpty(versionInfo.ProductName) ? versionInfo.ProductName : fileName;
            }

            AddOrUpdateItemGame(preset);
        }

        private void btnGameSave_Click(object sender, EventArgs e)
        {
            SaveGamePreset();
        }

        private void SaveGamePreset()
        {
            var shortcut = string.Empty;
            if (!Utils.ValidateShortcut(shortcut))
            {
                return;
            }

            var preset = GetSelectedGamePreset();

            var name = edtGameName.Text.Trim();

            if (name.Length == 0 || _gameService.GetPresets().Any(x => x.id != preset.id && x.name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                MessageForms.WarningOk("The name can not be empty and must be unique.");
                return;
            }

            preset.name = name;
            preset.Path = edtGamePath.Text;
            preset.Parameters = edtGameParameters.Text;
            preset.RunAsAdministrator = chkGameRunAsAdmin.Checked;
            preset.ShowInQuickAccess = chkGameQuickAccess.Checked;

            var clear = !string.IsNullOrEmpty(preset.shortcut);

            var shortcutChanged = !shortcut.Equals(preset.shortcut);
            if (shortcutChanged)
            {
                preset.shortcut = shortcut;
            }

            var text = edtGamePrelaunchSteps.Text;

            var stepsList = (GameStepType)cbxGameStepType.SelectedIndex switch
            {
                GameStepType.PreLaunch => preset.PreLaunchSteps,
                GameStepType.PostLaunch => preset.PostLaunchSteps,
                GameStepType.Finalize => preset.FinalizeSteps,
                _ => throw new InvalidOperationException("Invalid game step type")
            };

            Utils.ParseWords(stepsList, text);

            AddOrUpdateItemGame();

            if (shortcutChanged)
            {
                Utils.RegisterShortcut(Handle, preset.id, preset.shortcut, clear);
            }
        }

        private void btnGameBrowse_Click(object sender, EventArgs e)
        {
            var file = Utils.SelectFile();

            if (file == null)
            {
                return;
            }

            var preset = GetSelectedGamePreset();

            preset.Path = file.FullName;

            edtGamePath.Text = preset.Path;

            if (string.IsNullOrEmpty(preset.name) || preset.name.StartsWith("New preset", StringComparison.OrdinalIgnoreCase))
            {
                var fileName = Path.GetFileNameWithoutExtension(preset.Path);

                edtGameName.Text = fileName;
                preset.name = fileName;
            }

            AddOrUpdateItemGame();
        }

        private void btnGameDelete_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedGamePreset();

            if (preset == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(preset.shortcut))
            {
                WinApi.UnregisterHotKey(Handle, preset.id);
            }

            _gameService.GetPresets().Remove(preset);

            var item = lvGamePresets.SelectedItems[0];
            lvGamePresets.Items.Remove(item);
        }

        private void mnuGameAddStep_Opening(object sender, CancelEventArgs e)
        {
            BuildServicePresetsMenu(mnuGameNvidiaPresets, _nvService, "NVIDIA", miGameAddPreset_Click);
            BuildServicePresetsMenu(mnuGameAmdPresets, _amdService, "AMD", miGameAddPreset_Click);
            BuildServicePresetsMenu(mnuGameLgPresets, _lgService, "LG", miGameAddPreset_Click);
        }

        private void miGameAddPreset_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripItem;
            var preset = item.Tag as PresetBase;

            var text = $"{preset.GetType().Name}({preset.name})";

            FormUtils.AddStepToTextBox(edtGamePrelaunchSteps, text);
        }

        private void btnGameAddStep_Click(object sender, EventArgs e)
        {
            mnuGameAddStep.Show(btnGameAddStep, btnGameAddStep.PointToClient(Cursor.Position));
        }

        private void mnuGameStartProgram_Click(object sender, EventArgs e)
        {
            var file = Utils.SelectFile();

            if (file != null)
            {
                FormUtils.AddStepToTextBox(edtGamePrelaunchSteps, $"StartProgram({file.FullName})");
            }
        }

        private void lvGamePresets_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var file = new FileInfo(files.First());

                AddGamePreset(file);
            }
        }

        private void lvGamePresets_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void btnGameActions_Click(object sender, EventArgs e)
        {
            mnuGameActions.Show(btnGameSettings, btnGameSettings.PointToClient(Cursor.Position));
        }

        private void mnuGameNvInspector_Click(object sender, EventArgs e)
        {
            StartNvProfileInspector();
        }

        private string EditShortcut(string shortcut, string label = "Shortcut", string title = null)
        {
            var field = new MessageForms.FieldDefinition
            {
                Label = label,
                FieldType = MessageForms.FieldType.Shortcut,
                Value = shortcut
            };
            var values = MessageForms.ShowDialog(title ?? "Set shortcut", new[] { field });
            if (!values.Any())
            {
                return null;
            }

            return values.First().Value.ToString();
        }


        private void miGameSetQuickAccessShortcut_Click(object sender, EventArgs e)
        {
            var shortcut = EditShortcut(_config.GameQuickAccessShortcut);

            if (shortcut == null)
            {
                return;
            }

            var clear = !string.IsNullOrEmpty(_config.GameQuickAccessShortcut);

            _config.GameQuickAccessShortcut = shortcut;

            Utils.RegisterShortcut(Handle, SHORTCUTID_GAMEQA, _config.GameQuickAccessShortcut, clear);
        }

        private void btnNvSetQuickAccessShortcut_Click(object sender, EventArgs e)
        {
        }

        private void btnLgSettings_Click(object sender, EventArgs e)
        {
            var shortcut = EditShortcut(_config.LgQuickAccessShortcut, "Quick Access shortcut", "LG controller settings");

            if (shortcut == null)
            {
                return;
            }

            var clear = !string.IsNullOrEmpty(_config.LgQuickAccessShortcut);

            _config.LgQuickAccessShortcut = shortcut;

            Utils.RegisterShortcut(Handle, SHORTCUTID_LGQA, _config.LgQuickAccessShortcut, clear);
        }

        private void mnuGameActions_Opening(object sender, CancelEventArgs e)
        {
            mnuGameNvInspector.Visible = _nvService != null;
        }

        private void miNvProfileInspector_Click(object sender, EventArgs e)
        {
            StartNvProfileInspector();
        }

        private void StartNvProfileInspector()
        {
            var form = new frmDrvSettings();
            form.Show();
        }

        private void miNvSettings_Click(object sender, EventArgs e)
        {
            EditNvSettings();
        }

        private void EditNvSettings()
        {
            var values = MessageForms.ShowDialog("NVIDIA controller settings", new[] {
                new MessageForms.FieldDefinition
                {
                    Label = "Quick Access shortcut",
                    FieldType = MessageForms.FieldType.Shortcut,
                    Value = _config.NvQuickAccessShortcut
                },
                new MessageForms.FieldDefinition
                {
                    Label = "Enable overclocking",
                    FieldType = MessageForms.FieldType.CheckBox,
                    Value = _nvService.Config.ShowOverclocking
                }
            });
            if (!values.Any())
            {
                return;
            }

            var shortcut = values[0].Value.ToString();
            var enableOc = (bool)values[1].Value;

            var clear = !string.IsNullOrEmpty(_config.NvQuickAccessShortcut);

            _config.NvQuickAccessShortcut = shortcut;

            Utils.RegisterShortcut(Handle, SHORTCUTID_NVQA, _config.NvQuickAccessShortcut, clear);

            _nvService.Config.ShowOverclocking = enableOc;
        }


        private void btnNvSettings_Click(object sender, EventArgs e)
        {
            mnuNvSettings.Show(btnNvSettings, btnNvSettings.PointToClient(Cursor.Position));
        }

        private void lvGamePresets_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            ListViewItemChecked<GamePreset>(lvGamePresets, e);
        }

        private void ListViewItemChecked<T>(ListView listView, ItemCheckedEventArgs e) where T : PresetBase
        {
            var checkedPreset = (T)e.Item.Tag;

            if (checkedPreset == null || checkedPreset.ShowInQuickAccess == e.Item.Checked)
            {
                return;
            }

            var point = listView.PointToClient(Cursor.Position);

            if (point.X >= 20)
            {
                e.Item.Checked = !e.Item.Checked;
                return;
            }

            checkedPreset.ShowInQuickAccess = e.Item.Checked;

            var preset = listView.GetSelectedItemTag<T>();

            if (preset == checkedPreset)
            {
                listView.FireEvent("SelectedIndexChanged", listView, e);
            }
        }

        private void lvNvPresets_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            ListViewItemChecked<NvPreset>(lvNvPresets, e);
        }

        private void lvLgPresets_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            ListViewItemChecked<LgPreset>(lvLgPresets, e);
        }

        private void lvNvPresets_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var listView = sender as ListView;

            if (!listView.Focused)
            {
                return;
            }


            var point = listView.PointToClient(Cursor.Position);

            if (point.X >= 20)
            {
                return;
            }

            var listItem = listView.Items[e.Index];

            if (listItem.Tag == null)
            {
                MessageForms.WarningOk("Quick Access cannot be enabled for this item.");
                e.NewValue = e.CurrentValue;
            }

            if (e.NewValue == CheckState.Checked && string.IsNullOrEmpty(listItem.Text))
            {
                MessageForms.WarningOk("A name must be entered before enabling Quick Access.");
                e.NewValue = e.CurrentValue;
            }
        }

        private void lvAmdPresets_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            ListViewItemChecked<AmdPreset>(lvAmdPresets, e);
        }

        private void btnAmdSettings_Click(object sender, EventArgs e)
        {
            var shortcut = EditShortcut(_config.AmdQuickAccessShortcut, "Quick Access shortcut", "AMD controller settings");

            if (shortcut == null)
            {
                return;
            }

            var clear = !string.IsNullOrEmpty(_config.AmdQuickAccessShortcut);

            _config.AmdQuickAccessShortcut = shortcut;

            Utils.RegisterShortcut(Handle, SHORTCUTID_AMDQA, _config.AmdQuickAccessShortcut, clear);
        }

        private void btnGameProcessAffinity_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedGamePreset();

            if (preset == null)
            {
                return;
            }

            var numberOfProcessors = Environment.ProcessorCount;

            var options = Enumerable.Range(0, numberOfProcessors).Select(i => $"CPU #{i}");

            var values = MessageForms.ShowDialog("Set processor affinity", new[] {
                    new MessageForms.FieldDefinition
                    {
                        Label = "Set desired processors which are allowed to run program",
                        FieldType = MessageForms.FieldType.Flags,
                        Values = options,
                        Value = preset.ProcessAffinityMask
                    }
                });

            if (!values.Any())
            {
                return;
            }

            var value = values.First().Value;

            preset.ProcessAffinityMask = (uint)(int)value;
        }

        private void btnGameOptions_Click(object sender, EventArgs e)
        {
            mnuGameOptions.Show(btnGameOptions, btnGameOptions.PointToClient(Cursor.Position));
        }

        private void miGameProcessPriority_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedGamePreset();

            if (preset == null)
            {
                return;
            }

            var dropDownValues = Utils.GetDescriptions<GamePriorityClass>();

            var values = MessageForms.ShowDialog("Set process priority", new[] {
                    new MessageForms.FieldDefinition
                    {
                        Label = "Set desired process priority",
                        FieldType = MessageForms.FieldType.DropDown,
                        Values = dropDownValues,
                        Value = preset.ProcessPriorityClass > 0 ? ((GamePriorityClass)preset.ProcessPriorityClass).GetDescription() : GamePriorityClass.Normal.GetDescription()
                    }
                });

            if (!values.Any())
            {
                return;
            }

            var value = values.First().Value.ToString();
            var enumName = Utils.GetEnumNameByDescription(typeof(GamePriorityClass), value);
            if (enumName != null)
            {
                value = enumName;
            }

            var enumValue = Enum.Parse(typeof(GamePriorityClass), value);

            preset.ProcessPriorityClass = (uint)(int)enumValue;
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

        private void btnStartStopService_Click(object sender, EventArgs e)
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

                var wait = 1000;

                while (wait > 0)
                {
                    Utils.WaitForTask(Task.Delay(100));

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

        private void cbxLogType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadLog();
        }

        private void cbxGameStepType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowGameSteps();
        }

        private void ShowGameSteps()
        {
            var preset = GetSelectedGamePreset();

            if (preset == null)
            {
                return;
            }

            switch ((GameStepType)cbxGameStepType.SelectedIndex)
            {
                case GameStepType.PreLaunch:
                    edtGamePrelaunchSteps.Text = string.Join(", ", preset.PreLaunchSteps);
                    break;
                case GameStepType.PostLaunch:
                    edtGamePrelaunchSteps.Text = string.Join(", ", preset.PostLaunchSteps);
                    break;
                case GameStepType.Finalize:
                    edtGamePrelaunchSteps.Text = string.Join(", ", preset.FinalizeSteps);
                    break;
            }
        }

        private void edtGamePrelaunchSteps_Leave(object sender, EventArgs e)
        {
            SaveGamePreset();
        }

        private void MainForm_Click(object sender, EventArgs e)
        {
            //PipeUtils.SendMessage(SvcMessageType.RestartAfterUpdate);
            //Program.Restart();
            //Environment.Exit(0);
            //InstallUpdate("");
        }

        private void chkAutoInstallUpdates_CheckedChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                _config.AutoInstallUpdates = chkAutoInstallUpdates.Checked;
            }
        }

        private void chkLgSetSelectedDeviceByWol_CheckedChanged(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            _lgService.Config.SetSelectedDeviceByPowerOn = chkLgSetSelectedDeviceByPowerOn.Checked;
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

        private void cbxDitheringDisplay_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            UpdateDitherSettings();
        }

        private ListViewItem _lastItem;
        private int _lastSubItemIndex;

        private void lvNvPresets_MouseMove(object sender, MouseEventArgs e)
        {
            var item = lvNvPresets.GetItemAt(e.X, e.Y);
            var subItem = item?.GetSubItemAt(e.X, e.Y);

            if (item == null || subItem == null || subItem?.Text?.Length < 20)
            {
                lvNvPresetsToolTip.Active = false;
                _lastItem = null;

                return;
            }

            var index = item.SubItems.IndexOf(subItem);

            lvNvPresets.ShowItemToolTips = false;

            if (item == _lastItem && index == _lastSubItemIndex)
            {
                return;
            }

            _lastItem = item;
            _lastSubItemIndex = index;

            var text = subItem.Text;

            if (index == 7)
            {
                text = text.Replace(", ", "\r\n");
            }

            var point = lvNvPresets.PointToClient(Cursor.Position);
            point.X += 10;
            point.Y += 0;

            lvNvPresetsToolTip.Active = false;
            if (index >= 0)
            {
                lvNvPresetsToolTip.Active = true;
                lvNvPresetsToolTip.Show("", lvNvPresets, point);
                lvNvPresetsToolTip.Show(text, lvNvPresets, point);
            }
        }

        private void lvNvPresets_MouseLeave(object sender, EventArgs e)
        {
            lvNvPresetsToolTip.Active = false;
        }

        private void btnNvSetClocks_Click(object sender, EventArgs e)
        {
            var gpu = cbxNvGPU.SelectedItem as PhysicalGPU;

            if (gpu == null)
            {
                return;
            }

            var gpuInfo = NvGpuInfo.GetGpuInfo(gpu);
            var currentSettings = gpuInfo.GetOverclockSettings();

            var newSettings = AskNvOcSettings(gpuInfo, currentSettings);

            if (newSettings == null)
            {
                return;
            }

            _nvService.ApplyOverclocking(newSettings);

            UpdateNvGpuInfo();
        }

        public NvGpuOcSettings AskNvOcSettings(NvGpuInfo gpuInfo, NvGpuOcSettings currentSettings)
        {
            var overclockTypes = Utils.GetDescriptions<NvGpuOcType>();

            var values = MessageForms.ShowDialog("Choose desired overclock settings", new[] {
                    new MessageForms.FieldDefinition
                    {
                        Label = "Overclock method",
                        FieldType = MessageForms.FieldType.DropDown,
                        Values = overclockTypes,
                        Value = currentSettings.Type
                    } });
            if (!values.Any())
            {
                return null;
            }

            var overclockType = Enum.Parse<NvGpuOcType>(values[0].Value.ToString());
            var newSettings = default(NvGpuOcSettings);

            if (overclockType == NvGpuOcType.Curve)
            {
                newSettings = AskNvCurveOcSettings(gpuInfo, currentSettings);
            }
            else if (overclockType == NvGpuOcType.Offset)
            {
                newSettings = AskNvOffsetOcSettings(gpuInfo, currentSettings);
            }
            else
            {
                newSettings = new NvGpuOcSettings
                {
                    Type = overclockType,
                    PCIIdentifier = gpuInfo.GPU.BusInformation.PCIIdentifiers.ToString()
                };
            }

            return newSettings;
        }

        public NvGpuOcSettings AskNvCurveOcSettings(NvGpuInfo gpuInfo, NvGpuOcSettings currentSettings)
        {
            var nvOverclockSettings = new NvGpuOcSettings
            {
                Type = NvGpuOcType.Curve,
                PCIIdentifier = gpuInfo.GPU.BusInformation.PCIIdentifiers.ToString()
            };

            var curveEntries = gpuInfo.CurveV3.GPUCurveEntries;

            var graphicsDelta = gpuInfo.GetDeltaClockMinMax(NvAPIWrapper.Native.GPU.PublicClockDomain.Graphics);

            var minCurveFreq = curveEntries.Max(e => e.DefaultFrequencyInkHz) + graphicsDelta.Item1;
            var freqValues = curveEntries
                .Where(e => e.DefaultFrequencyInkHz == 0 || e.DefaultFrequencyInkHz >= minCurveFreq)
                .Select(e => (e.DefaultFrequencyInkHz / 1000).ToString()).Distinct()
                .OrderBy(x => x)
                .ToList();
            var voltValues = curveEntries
                .Where(e => e.DefaultVoltageInMicroV == 0 || e.DefaultFrequencyInkHz >= minCurveFreq)
                .OrderBy(e => e.DefaultVoltageInMicroV)
                .Select(e => (e.DefaultVoltageInMicroV / 1000).ToString())
                .Distinct()
                .ToList();

            var currentGraphicsVoltage = currentSettings.MaximumVoltageUv > 0 ? currentSettings.MaximumVoltageUv : 1050000;

            var curveEntry = curveEntries.FirstOrDefault(e => e.VoltageInMicroV == currentGraphicsVoltage);
            var currentGpuFreq = curveEntry.FrequencyInkHz;

            var memoryDelta = gpuInfo.GetDeltaClockMinMax(NvAPIWrapper.Native.GPU.PublicClockDomain.Memory);

            var powerLimits = gpuInfo.GetPowerLimits();

            var defaultPowerLimitMw = gpuInfo.DefaultPowerInMilliWatts;

            var powerField = defaultPowerLimitMw > 0 ?
                new MessageForms.FieldDefinition
                {
                    Label = $"Power limit in milli watts. Min: {gpuInfo.MinPowerInMilliWatts}, max: {gpuInfo.MaxPowerInMilliWatts}",
                    FieldType = MessageForms.FieldType.TrackBar,
                    Value = (int)((decimal)defaultPowerLimitMw / 100000 * currentSettings.PowerPCM),
                    MinValue = gpuInfo.MinPowerInMilliWatts,
                    MaxValue = gpuInfo.MaxPowerInMilliWatts
                }
                : new MessageForms.FieldDefinition
                {
                    Label = $"Power limit in PCM (per cent mille). Min: {powerLimits.Item1}, max: {powerLimits.Item2}",
                    FieldType = MessageForms.FieldType.TrackBar,
                    Value = currentSettings.PowerPCM,
                    MinValue = powerLimits.Item1,
                    MaxValue = powerLimits.Item2
                };

            var values = MessageForms.ShowDialog("Set desired clock speeds and voltage", new[] {
                    new MessageForms.FieldDefinition
                    {
                        Label = $"Core offset in MHz ({graphicsDelta.Item1.ToUnitString()} to {graphicsDelta.Item2.ToUnitString()})",
                        FieldType = MessageForms.FieldType.Numeric,
                        MinValue = graphicsDelta.Item1 / 1000,
                        MaxValue = graphicsDelta.Item2 / 1000,
                        Value = currentSettings.GraphicsOffsetKHz / 1000
                    },
                    new MessageForms.FieldDefinition
                    {
                        Label = "Optional undervolt: maximum voltage in milli volt",
                        FieldType = MessageForms.FieldType.DropDown,
                        Value = currentSettings.MaximumVoltageUv / 1000,
                        Values = voltValues,
                    },
                    new MessageForms.FieldDefinition
                    {
                        Label = "Optional undervolt: maximum core frequency in MHz",
                        FieldType = MessageForms.FieldType.DropDown,
                        Value = currentSettings.MaximumFrequencyKHz / 1000,
                        Values = freqValues,
                    },
                    new MessageForms.FieldDefinition
                    {
                        Label = $"Memory offset in MHz ({memoryDelta.Item1.ToUnitString()} to {memoryDelta.Item2.ToUnitString()})",
                        FieldType = MessageForms.FieldType.Numeric,
                        MinValue = memoryDelta.Item1 / 1000,
                        MaxValue = memoryDelta.Item2 / 1000,
                        Value = currentSettings.MemoryOffsetKHz / 1000
                    },
                    new MessageForms.FieldDefinition
                    {
                        Label = "Voltage boost in %",
                        FieldType = MessageForms.FieldType.Numeric,
                        Value = currentSettings.VoltageBoostPercent,
                        MinValue = 0,
                        MaxValue = 100
                    },
                    powerField,
            });
            if (!values.Any())
            {
                return null;
            }

            nvOverclockSettings.GraphicsOffsetKHz = values[0].ValueAsInt * 1000;
            nvOverclockSettings.MaximumVoltageUv = values[1].ValueAsUInt * 1000;
            nvOverclockSettings.MaximumFrequencyKHz = values[2].ValueAsUInt * 1000;
            nvOverclockSettings.MemoryOffsetKHz = values[3].ValueAsInt * 1000;
            nvOverclockSettings.VoltageBoostPercent = values[4].ValueAsUInt;

            var power = values[5].ValueAsUInt;

            nvOverclockSettings.PowerPCM = defaultPowerLimitMw > 0 ? (uint)((decimal)power / defaultPowerLimitMw * 100000) : power;

            return nvOverclockSettings;
        }

        public NvGpuOcSettings AskNvOffsetOcSettings(NvGpuInfo gpuInfo, NvGpuOcSettings currentSettings)
        {
            var nvOverclockSettings = new NvGpuOcSettings
            {
                Type = NvGpuOcType.Offset,
                PCIIdentifier = gpuInfo.GPU.BusInformation.PCIIdentifiers.ToString()
            };

            var curveEntries = gpuInfo.CurveV3.GPUCurveEntries;

            var freqValues = curveEntries.Select(e => (e.DefaultFrequencyInkHz / 1000).ToString()).Distinct().ToList();
            var voltValues = curveEntries.Select(e => (e.DefaultVoltageInMicroV / 1000).ToString()).Distinct().ToList();

            var currentGraphicsVoltage = currentSettings.MaximumVoltageUv > 0 ? currentSettings.MaximumVoltageUv : 1050000;

            var curveEntry = curveEntries.FirstOrDefault(e => e.VoltageInMicroV == currentGraphicsVoltage);
            var currentGpuFreq = curveEntry.FrequencyInkHz;

            var graphicsDelta = gpuInfo.GetDeltaClockMinMax(NvAPIWrapper.Native.GPU.PublicClockDomain.Graphics);
            var memoryDelta = gpuInfo.GetDeltaClockMinMax(NvAPIWrapper.Native.GPU.PublicClockDomain.Memory);

            var powerLimits = gpuInfo.GetPowerLimits();

            var values = MessageForms.ShowDialog("Set desired clock speeds and voltage", new[] {
                    new MessageForms.FieldDefinition
                    {
                        Label = $"Core offset in MHz ({graphicsDelta.Item1.ToUnitString()} to {graphicsDelta.Item2.ToUnitString()})",
                        FieldType = MessageForms.FieldType.Numeric,
                        MinValue = graphicsDelta.Item1 / 1000,
                        MaxValue = graphicsDelta.Item2 / 1000,
                        Value = currentSettings.GraphicsOffsetKHz / 1000
                    },
                    new MessageForms.FieldDefinition
                    {
                        Label = $"Memory offset in MHz ({memoryDelta.Item1.ToUnitString()} to {memoryDelta.Item2.ToUnitString()})",
                        FieldType = MessageForms.FieldType.Numeric,
                        MinValue = memoryDelta.Item1 / 1000,
                        MaxValue = memoryDelta.Item2 / 1000,
                        Value = currentSettings.MemoryOffsetKHz / 1000
                    },
                    new MessageForms.FieldDefinition
                    {
                        Label = "Voltage boost in %",
                        FieldType = MessageForms.FieldType.Numeric,
                        Value = currentSettings.VoltageBoostPercent,
                        MinValue = 0,
                        MaxValue = 100
                    },
                    new MessageForms.FieldDefinition
                    {
                        Label = $"Power limit in PCM (per cent mille). Min: {powerLimits.Item1}, max: {powerLimits.Item2}",
                        FieldType = MessageForms.FieldType.Numeric,
                        Value = currentSettings.PowerPCM,
                        MinValue = powerLimits.Item1,
                        MaxValue = powerLimits.Item2
                    },
            });
            if (!values.Any())
            {
                return null;
            }

            nvOverclockSettings.GraphicsOffsetKHz = values[0].ValueAsInt * 1000;
            nvOverclockSettings.MemoryOffsetKHz = values[1].ValueAsInt * 1000;
            nvOverclockSettings.VoltageBoostPercent = values[2].ValueAsUInt;
            nvOverclockSettings.PowerPCM = values[3].ValueAsUInt;

            return nvOverclockSettings;
        }

        public void SetClocks()
        {
            var gpu = cbxNvGPU.SelectedItem as PhysicalGPU;

            var gpuInfo = NvGpuInfo.GetGpuInfo(gpu);

            var curveEntries = gpuInfo.CurveV3.GPUCurveEntries;

            var freqValues = curveEntries.Select(e => e.DefaultFrequencyInkHz.ToString()).Distinct().ToList();
            var voltValues = curveEntries.Select(e => e.DefaultVoltageInMicroV.ToString()).Distinct().ToList();

            var lockedGraphics = gpuInfo.GetLockedBoost(NvAPIWrapper.Native.GPU.PublicClockDomain.Graphics);

            var currentGraphicsVoltage = lockedGraphics.Item1 > 0 ? lockedGraphics.Item1 : 1050000;

            var curveEntry = curveEntries.FirstOrDefault(e => e.VoltageInMicroV == currentGraphicsVoltage);
            var currentGpuFreq = curveEntry.FrequencyInkHz;

            var graphicsDelta = gpuInfo.GetDeltaClockLimits(NvAPIWrapper.Native.GPU.PublicClockDomain.Graphics);
            var memoryDelta = gpuInfo.GetDeltaClockLimits(NvAPIWrapper.Native.GPU.PublicClockDomain.Memory);

            var graphicsMinValue = graphicsDelta?.DeltaRange.Minimum ?? -502000;
            var graphicsMaxValue = graphicsDelta?.DeltaRange.Maximum ?? 1000000;
            var memoryMinValue = memoryDelta?.DeltaRange.Minimum ?? -502000;
            var memoryMaxValue = memoryDelta?.DeltaRange.Maximum ?? 3000000;

            var currentGraphicsOffset = gpuInfo.GetOffset(NvAPIWrapper.Native.GPU.PublicClockDomain.Graphics);
            var currentMemoryOffset = gpuInfo.GetOffset(NvAPIWrapper.Native.GPU.PublicClockDomain.Memory);

            var values = MessageForms.ShowDialog("Set desired clock speeds and voltage", new[] {
                    new MessageForms.FieldDefinition
                    {
                        Label = $"Core offset in MHz ({graphicsMinValue}KHz to {graphicsMaxValue}KHz)",
                        FieldType = MessageForms.FieldType.Numeric,
                        MinValue = graphicsDelta?.DeltaRange.Minimum ?? -502000,
                        MaxValue = graphicsDelta?.DeltaRange.Maximum ?? 1000000,
                        Value = currentGraphicsOffset
                    },
                    new MessageForms.FieldDefinition
                    {
                        Label = "Voltage in micro volt",
                        FieldType = MessageForms.FieldType.DropDown,
                        Values = voltValues,
                        //FieldType = MessageForms.FieldType.Numeric,
                        //MinValue = 650000,
                        //MaxValue = 1100000,
                        //Value = currentGraphicsVoltage
                    },
                    new MessageForms.FieldDefinition
                    {
                        Label = "Core frequency in KHz",
                        FieldType = MessageForms.FieldType.DropDown,
                        Values = freqValues,
                        //FieldType = MessageForms.FieldType.Numeric,
                        //MinValue = 2000000,
                        //MaxValue = 3200000,
                        //Value = currentGpuFreq
                    },
                    new MessageForms.FieldDefinition
                    {
                        Label = $"Memory offset in KHz ({memoryMinValue}KHz to {memoryMaxValue}KHz)",
                        FieldType = MessageForms.FieldType.Numeric,
                        MinValue = graphicsDelta?.DeltaRange.Minimum ?? -502000,
                        MaxValue = 2500000,
                        Value = currentMemoryOffset
                    },
            });
            if (!values.Any())
            {
                return;
            }

            var voltage = (uint)values[0].ValueAsInt;
            //var coreOffset = values[1].ValueAsInt;
            var coreFreq = values[1].ValueAsInt;
            var memoryOffset = values[2].ValueAsInt;

            var normalized = (coreFreq / 15000) * 15000;
            var newCurveEntryFreq = curveEntries.FirstOrDefault(e => e.DefaultFrequencyInkHz == normalized);
            var newCurveEntryVolt = curveEntries.FirstOrDefault(e => e.DefaultVoltageInMicroV == voltage);

            var coreOffset = (int)(newCurveEntryFreq.DefaultFrequencyInkHz - newCurveEntryVolt.DefaultFrequencyInkHz);

            gpuInfo.SetGpuClocks(voltage, coreOffset, memoryOffset);
        }

        private void miNvOverclockingIncluded_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedNvPreset();

            preset.applyOverclocking = !preset.applyOverclocking;

            AddOrUpdateItem();
        }

        private void cbxNvGPU_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshNvGpuInfo();
        }

        private void RefreshNvGpuInfo()
        {
            var gpu = (PhysicalGPU)cbxNvGPU.SelectedItem;

            if (gpu == null)
            {
                return;
            }

            var gpuInfo = NvGpuInfo.GetGpuInfo(gpu);
            var ocSettings = gpuInfo.GetOverclockSettings();

            edtNvGpuInfo.Text = gpuInfo.ToString();
            edtNvOverclock.Text = ocSettings.ToString();
        }
    }
}