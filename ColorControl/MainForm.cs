using LgTv;
using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using NLog;
using NStandard;
using NvAPIWrapper.Display;
using NvAPIWrapper.Native.Display;
using NWin32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Deployment.Application;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
// 1. Import the InteropServices type
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Action = System.Action;

namespace ColorControl
{
    public partial class MainForm : Form
    {
        //private Bitmap bitmap, bitmap2;

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        public extern static bool ShutdownBlockReasonCreate(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] string pwszReason);

        [DllImport("user32.dll")]
        public extern static bool ShutdownBlockReasonDestroy(IntPtr hWnd);

        private static int WM_HOTKEY = 0x0312;
        private static string TS_TASKNAME = "ColorControl";
        private static bool SystemShutdown = false;
        private static bool EndSession = false;
        private static bool UserExit = false;
        private static int SHORTCUTID_SCREENSAVER = -100;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private string _dataDir;

        private NvService _nvService;
        private List<NvPreset> _presets;
        private string _nvPresetsFilename;
        private JavaScriptSerializer _JsonSerializer;
        private JavaScriptSerializer _JsonDeserializer;
        private Keys _pressedModifiers;
        private string _lastDisplayRefreshRates = string.Empty;

        private NotifyIcon trayIcon;
        private bool _initialized = false;
        private string _configFilename;
        private Config _config;
        private bool _setVisibleCalled = false;
        private bool _winKeyDown = false;
        private RestartDetector _restartDetector;

        private LgService _lgService;
        private List<LgPreset> _lgPresets;
        private string _lgPresetsFilename;
        private List<LgApp> _lgApps;
        private RemoteControlForm _remoteControlForm;

        private MenuItem _nvTrayMenu;
        private MenuItem _lgTrayMenu;

        private StartUpParams StartUpParams { get; }

        private AmdService _amdService;

        public MainForm(StartUpParams startUpParams)
        {
            InitializeComponent();
            StartUpParams = startUpParams;

            _dataDir = Utils.GetDataPath();

            InitLogger();

            MessageForms.MainForm = this;

            _nvTrayMenu = new MenuItem("NVIDIA presets");
            _lgTrayMenu = new MenuItem("LG presets");
            trayIcon = new NotifyIcon()
            {
                Icon = Icon,
                ContextMenu = new ContextMenu(new MenuItem[] {
                    _nvTrayMenu,
                    _lgTrayMenu,
                    new MenuItem("-", OpenForm),
                    new MenuItem("Open", OpenForm),
                    new MenuItem("-"),
                    new MenuItem("Exit", Exit)
                }),
                Visible = true,
                Text = Text
            };
            trayIcon.MouseDoubleClick += trayIcon_MouseDoubleClick;
            trayIcon.ContextMenu.Popup += trayIconContextMenu_Popup;

            chkStartAfterLogin.Checked = TaskExists(true);

            chkFixChromeFonts.Enabled = Utils.IsChromeInstalled();
            if (chkFixChromeFonts.Enabled)
            {
                chkFixChromeFonts.Checked = Utils.IsChromeFixInstalled();
            }

            _JsonSerializer = new JavaScriptSerializer();
            _JsonDeserializer = new JavaScriptSerializer();

            var converter = new ColorDataConverter();
            _JsonDeserializer.RegisterConverters(new[] { converter });

            _configFilename = Path.Combine(_dataDir, "Settings.json");
            LoadConfig();

            try
            {
                _nvService = new NvService();
            }
            catch (Exception)
            {
            }

            _nvPresetsFilename = Path.Combine(_dataDir, "NvPresets.json");
            FillNvPresets();

            //try
            //{
            //    _amdService = new AmdService();
            //    lblErrorAMD.Text = "If you see this message, it means you have AMD graphics drivers installed. Unfortunately, this feature is not completed yet. If I get an AMD card I might finish it.";
            //}
            //catch (Exception)
            //{
            //    tcMain.TabPages.Remove(tabAMD);
            //}
            tcMain.TabPages.Remove(tabAMD);

            _lgPresetsFilename = Path.Combine(_dataDir, "LgPresets.json");
            var toCopy = Path.Combine(Directory.GetCurrentDirectory(), "LgPresets.json");
            if (!File.Exists(_lgPresetsFilename) && File.Exists(toCopy))
            {
                try
                {
                    File.Copy(toCopy, _lgPresetsFilename);
                }
                catch (Exception e)
                {
                    Logger.Error($"Error while copying {toCopy} to {_lgPresetsFilename}: {e.Message}");
                }
            }
            FillLgPresets();

            try
            {
                _lgService = new LgService(_dataDir, StartUpParams.RunningFromScheduledTask);

                clbLgPower.SetItemChecked(0, _lgService.Config.PowerOnAfterStartup);
                clbLgPower.SetItemChecked(1, _lgService.Config.PowerOnAfterResume);
                clbLgPower.SetItemChecked(2, _lgService.Config.PowerOffOnShutdown);
                clbLgPower.SetItemChecked(3, _lgService.Config.PowerOffOnStandby);
                edtLgPowerOnAfterResumeDelay.Value = _lgService.Config.PowerOnDelayAfterResume;
                edtLgDeviceFilter.Text = _lgService.Config.DeviceSearchKey;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            InitInfo();

            var values = Enum.GetValues(typeof(ButtonType));
            foreach (var button in values)
            {
                var text = button.ToString();
                if (text[0] == '_')
                {
                    text = text.Substring(1);
                }
                var item = mnuLgButtons.Items.Add(text);
                item.Click += miLgAddButton_Click;
            }

            if (_lgService != null)
            {
                //SystemEvents.SessionEnded += new SessionEndedEventHandler(SessionEnded);
                SystemEvents.PowerModeChanged += new PowerModeChangedEventHandler(PowerModeChanged);
            }

            _restartDetector = new RestartDetector();

            //Scale(new SizeF(1.25F, 1.25F));

            _initialized = true;

            AfterInitialized();
        }

        private void PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            var powerOn = e.Mode == PowerModes.Resume;

            Logger.Debug($"PowerModeChanged: {e.Mode}");

            if (powerOn)
            {
                _lgService.DisposeConnection();
                _lgService.WakeAfterResume();
                return;
            }

            if (_lgService.Config.PowerOffOnStandby)
            {
                NativeMethods.SetThreadExecutionState(NativeConstants.ES_CONTINUOUS | NativeConstants.ES_SYSTEM_REQUIRED | NativeConstants.ES_AWAYMODE_REQUIRED);
                try
                {
                    Logger.Debug("Powering off tv...");
                    var task = _lgService.PowerOff();
                    Utils.WaitForTask(task);
                    Logger.Debug("Done powering off tv");
                }
                finally
                {
                    NativeMethods.SetThreadExecutionState(NativeConstants.ES_CONTINUOUS);
                }
            }
        }

        private void SessionEnded(object sender, SessionEndedEventArgs e)
        {
            //if (e.Reason == SessionEndReasons.SystemShutdown)
            //{
            //    Logger.Debug($"SessionEnded: {e.Reason}");

            //    if (_lgService.Config.PowerOffOnShutdown)
            //    {
            //        _lgService.PowerOff();
            //    }
            //}
        }

        private void InitLogger()
        {
            var config = new NLog.Config.LoggingConfiguration();

            // Targets where to log to: File and Console
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = Path.Combine(_dataDir, "LogFile.txt") };
            //var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            // Rules for mapping loggers to targets            
            //config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, logfile);

            // Apply config           
            NLog.LogManager.Configuration = config;
        }

        private void InitInfo()
        {
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(Path.GetFileName(Application.ExecutablePath));

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                Text = Application.ProductName + " " + ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            else
            {
                Text = Application.ProductName + " " + Application.ProductVersion;
            }

            lblInfo.Text = Text + " - " + fileVersionInfo.LegalCopyright;

            lbPlugins.Items.Add("lgtv.net by gr4b4z");
            lbPlugins.Items.Add("Newtonsoft.Json by James Newton-King");
            lbPlugins.Items.Add("NLog by Jarek Kowalski, Kim Christensen, Julian Verdurmen");
            lbPlugins.Items.Add("NvAPIWrapper.Net by Soroush Falahati");
            lbPlugins.Items.Add("NWin32 by zmjack");
            lbPlugins.Items.Add("TaskScheduler by David Hall");
        }

        private void OpenForm(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        void Exit(object sender, EventArgs e)
        {
            UserExit = true;
            Close();
        }

        private void trayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Display.GetDisplays().First().DisplayDevice.CurrentColorData.ColorFormat = NvAPIWrapper.Native.Display.ColorDataFormat.RGB;
            // var colorData = new ColorData(NvAPIWrapper.Native.Display.ColorDataFormat.YUV444, NvAPIWrapper.Native.Display.ColorDataColorimetry.Auto, NvAPIWrapper.Native.Display.ColorDataDynamicRange.Auto, NvAPIWrapper.Native.Display.ColorDataDepth.BPC8);
            //var hdrColorData = new HDRColorData(NvAPIWrapper.Native.Display.ColorDataHDRMode.UHDA);
            //DisplayDevice.GetGDIPrimaryDisplayDevice().SetColorData(colorData);
            //Display.GetDisplays().First().DisplayDevice.SetColorData(colorData);
            //pictureBox1.Load("d:\\ss.png");

            //bitmap = new Bitmap("d:\\ss2.png");

            //Bitmap bitmap2 = Utils.SubPixelShift(bitmap);

            //pictureBox1.Image = bitmap2;
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void FillNvPresets()
        {
            var colums = NvPreset.GetColumnNames();

            foreach (var name in colums)
            {
                var columnName = name;
                var parts = name.Split('|');

                var width = 120;
                if (parts.Length > 1)
                {
                    width = Int32.Parse(parts[1]);
                    columnName = parts[0];
                }

                var header = lvNvPresets.Columns.Add(columnName);
                header.Width = width == 120 ? -2 : width;
            }

            if (File.Exists(_nvPresetsFilename))
            {
                var json = File.ReadAllText(_nvPresetsFilename);

                _presets = _JsonDeserializer.Deserialize<List<NvPreset>>(json);
            }
            else
            {
                _presets = NvPreset.GetDefaultPresets();

            }

            UpdateDisplayInfoItems();

            foreach (var preset in _presets)
            {
                AddOrUpdateItem(preset);
                RegisterShortcut(preset.id, preset.shortcut);
            }

            //UpdateTrayMenuNv();
        }

        private void UpdateDisplayInfoItems()
        {
            if (_nvService == null)
            {
                return;
            }

            Display[] displays;
            try
            {
                displays = _nvService.GetDisplays();
            }
            catch (Exception e)
            {
                Logger.Error("Error while getting displays: " + e.ToLogString());
                return;
            }

            var text = TS_TASKNAME;
            foreach (var display in displays)
            {
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

                var values = new List<string>();

                var name = display.Name;

                var screen = Screen.AllScreens.FirstOrDefault(x => x.DeviceName.Equals(name));
                if (screen != null)
                {
                    name += " (" + screen.DeviceFriendlyName() + ")";
                }

                values.Add(name);

                var colorData = display.DisplayDevice.CurrentColorData;
                var colorSettings = string.Format("{0}, {1}, {2}, {3}", colorData.ColorDepth, colorData.ColorFormat, colorData.DynamicRange, colorData.Colorimetry);

                values.Add(colorSettings);

                var refreshRate = display.DisplayDevice.CurrentTiming.Extra.RefreshRate;

                values.Add($"{refreshRate}Hz");

                var lastPreset = _nvService.GetLastAppliedPreset();
                if (lastPreset != null)
                {
                    //values.Add(_nvService.GetDithering() ? "Yes" : "No");
                    values.Add(lastPreset.GetDitheringDescription());
                }
                else
                {
                    values.Add(string.Empty);
                }

                values.Add(_nvService.IsHDREnabled() ? "Yes" : "No");

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

                text += "\n" + string.Format("{0}: {1}, {2}Hz, HDR: {3}", name, colorSettings, refreshRate, _nvService.IsHDREnabled() ? "Yes" : "No");
            }

            Utils.SetNotifyIconText(trayIcon, text);
        }

        private void RegisterShortcut(int id, string shortcut, bool clear = false)
        {
            if (clear)
            {
                UnregisterHotKey(Handle, id);
            }

            if (!string.IsNullOrEmpty(shortcut))
            {
                var (mods, key) = Utils.ParseShortcut(shortcut);
                bool registered = RegisterHotKey(Handle, id, mods, key);
            }
        }

        private void AddOrUpdateItem(NvPreset preset = null)
        {
            ListViewItem item = null;
            if (preset == null)
            {
                item = lvNvPresets.SelectedItems[0];
                preset = (NvPreset)item.Tag;
            }

            var values = preset.GetDisplayValues(_config);

            if (item == null)
            {
                item = lvNvPresets.Items.Add(values[0]);
                item.Tag = preset;
                for (var i = 1; i < values.Count; i++)
                {
                    item.SubItems.Add(values[i]);
                }
                if (!_presets.Any(x => x.id == preset.id)) {
                    _presets.Add(preset);
                }
            }
            else
            {
                item.Text = values[0];
                for (var i = 1; i < values.Count; i++)
                {
                    item.SubItems[i].Text = values[i];
                }
            }
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
            
            SaveNvPresets();
            SaveLgPresets();

            SaveConfig();

            if (SystemShutdown && _lgService.Config.PowerOffOnShutdown)
            {
                Logger.Debug($"MainForm_FormClosing: SystemShutdown");

                if (_restartDetector != null && (_restartDetector.RestartDetected || _restartDetector.IsRebootInProgress()))
                {
                    Logger.Debug("Not powering off because of a restart");
                }
                else if (NativeMethods.GetAsyncKeyState(NativeConstants.VK_CONTROL) < 0 || NativeMethods.GetAsyncKeyState(NativeConstants.VK_RCONTROL) < 0)
                {
                    Logger.Debug("Not powering off because CONTROL-key is down");
                }
                else
                {
                    Logger.Debug("Powering off tv...");
                    var task = _lgService.PowerOff();
                    Utils.WaitForTask(task);
                    Logger.Debug("Done powering off tv");
                    //ExecPowerOffPreset(true);
                }
            }
        }

        private void SaveNvPresets()
        {
            var json = _JsonSerializer.Serialize(_presets);
            File.WriteAllText(_nvPresetsFilename, json);
        }

        private void SaveLgPresets()
        {
            var json = _JsonSerializer.Serialize(_lgPresets);
            File.WriteAllText(_lgPresetsFilename, json);
        }

        private void lvNvPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            var preset = GetSelectedNvPreset();
            var enabled = preset != null;

            btnApply.Enabled = enabled;
            btnChange.Enabled = enabled;
            edtShortcut.Enabled = enabled;
            btnSetShortcut.Enabled = enabled;
            btnClone.Enabled = enabled;
            btnNvPresetDelete.Enabled = enabled;

            if (preset != null)
            {
                edtShortcut.Text = preset.shortcut;
            }
            else
            {
                edtShortcut.Text = string.Empty;
            }
        }

        protected override void WndProc(ref Message m)
        {
            // 5. Catch when a HotKey is pressed !
            if (m.Msg == WM_HOTKEY && !edtShortcut.Focused && !edtShortcutLg.Focused)
            {
                int id = m.WParam.ToInt32();
                // MessageBox.Show(string.Format("Hotkey #{0} pressed", id));

                // 6. Handle what will happen once a respective hotkey is pressed
                if (id == SHORTCUTID_SCREENSAVER)
                {
                    var screenSaver = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "scrnsave.scr");
                    Process.Start(screenSaver);
                }
                else
                {
                    var preset = _presets.FirstOrDefault(x => x.id == id);
                    if (preset != null)
                    {
                        ApplyNvPreset(preset);
                    }

                    var lgPreset = _lgPresets.FirstOrDefault(x => x.id == id);
                    if (lgPreset != null)
                    {
                        ApplyLgPreset(lgPreset);
                    }
                }

                //switch (id)
                //{
                //    case 1:
                //        MessageBox.Show("F9 Key Pressed ! Do something here ... ");
                //        break;
                //}
            }
            else if (m.Msg == NativeConstants.WM_QUERYENDSESSION)
            {
                SystemShutdown = true;
            }
            else if (m.Msg == NativeConstants.WM_ENDSESSION)
            {
                EndSession = true;
            }

            base.WndProc(ref m);
        }

        private void edtShortcut_KeyDown(object sender, KeyEventArgs e)
        {
            _pressedModifiers = e.Modifiers;

            //Debug.WriteLine("KD: " + e.Modifiers + ", " + e.KeyCode);

            var shortcutString = (_pressedModifiers > 0 ? _pressedModifiers.ToString() : "");
            if (e.KeyCode == Keys.LWin || _winKeyDown)
            {
                _winKeyDown = true;
                if (!string.IsNullOrEmpty(shortcutString))
                {
                    shortcutString += ", ";
                }
                shortcutString += "Win";
            }

            if (!string.IsNullOrEmpty(shortcutString) && e.KeyCode != Keys.ControlKey && e.KeyCode != Keys.ShiftKey && e.KeyCode != Keys.Menu && e.KeyCode != Keys.LWin)
            {
                shortcutString += " + " + e.KeyCode.ToString();
            }

            if (_pressedModifiers == 0 && !_winKeyDown)
            {
                e.SuppressKeyPress = true;
            }

            ((TextBox)sender).Text = shortcutString;
        }

        private void edtShortcut_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void btnSetShortcut_Click(object sender, EventArgs e)
        {
            var shortcut = edtShortcut.Text.Trim();

            if (!string.IsNullOrWhiteSpace(shortcut) && !shortcut.Contains("+"))
            {
                MessageForms.WarningOk("Invalid shortcut. The shortcut should have modifiers and a normal key.");
                return;
            }

            var preset = GetSelectedNvPreset();

            var clear = !string.IsNullOrEmpty(preset.shortcut);

            preset.shortcut = shortcut;

            AddOrUpdateItem();

            RegisterShortcut(preset.id, preset.shortcut, clear);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            trayIcon.Visible = false;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
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
            RegisterTask(enabled);
        }

        private void RegisterTask(bool enabled)
        {
            var file = Assembly.GetExecutingAssembly().Location;
            var directory = Path.GetDirectoryName(file);

            try
            {
                using (TaskService ts = new TaskService())
                {
                    if (enabled)
                    {
                        TaskDefinition td = ts.NewTask();
                        td.RegistrationInfo.Description = "Start ColorControl";

                        td.Triggers.Add(new LogonTrigger { UserId = WindowsIdentity.GetCurrent().Name });

                        td.Actions.Add(new ExecAction(file, StartUpParams.RunningFromScheduledTaskParam, directory));

                        ts.RootFolder.RegisterTaskDefinition(TS_TASKNAME, td);
                    }
                    else
                    {
                        ts.RootFolder.DeleteTask(TS_TASKNAME, false);
                    }
                }
            }
            catch (Exception e)
            {
                MessageForms.ErrorOk("Could not create/delete task: " + e.Message);
            }
        }

        private bool TaskExists(bool update)
        {
            var file = Assembly.GetExecutingAssembly().Location;
            using (TaskService ts = new TaskService())
            {
                var task = ts.RootFolder.Tasks.FirstOrDefault(x => x.Name.Equals(TS_TASKNAME));

                if (task != null)
                {
                    if (update && ApplicationDeployment.IsNetworkDeployed)
                    {
                        var action = task.Definition.Actions.FirstOrDefault(x => x.ActionType == TaskActionType.Execute);
                        if (action != null)
                        {
                            var execAction = action as ExecAction;
                            if (!execAction.Path.Equals(file))
                            {
                                RegisterTask(false);
                                RegisterTask(true);
                            }
                        }
                    }
                    return true;
                }
                return false;
            }
        }

        private void LoadConfig()
        {
            if (File.Exists(_configFilename))
            {
                var data = File.ReadAllText(_configFilename);
                _config = _JsonDeserializer.Deserialize<Config>(data);
            }
            else
            {
                _config = new Config();
            }

            chkStartMinimized.Checked = _config.StartMinimized;
            chkMinimizeOnClose.Checked = _config.MinimizeOnClose;
            edtDelayDisplaySettings.Value = _config.DisplaySettingsDelay;
            edtBlankScreenSaverShortcut.Text = _config.ScreenSaverShortcut;

            if (!string.IsNullOrEmpty(_config.ScreenSaverShortcut))
            {
                RegisterShortcut(SHORTCUTID_SCREENSAVER, _config.ScreenSaverShortcut);
            }

            Width = _config.FormWidth;
            Height = _config.FormHeight;
        }

        private void SaveConfig()
        {
            _config.StartMinimized = chkStartMinimized.Checked;
            if (WindowState != FormWindowState.Minimized)
            {
                _config.FormWidth = Width;
                _config.FormHeight = Height;
            }

            try
            {
                var data = _JsonSerializer.Serialize(_config);
                File.WriteAllText(_configFilename, data);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToLogString());
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
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
                lblLgError.Text = "Error while initializing the LG-controller. You either don't have a LG TV or it is disabled.";
                lblLgError.Visible = true;
            }
        }

        protected override void SetVisibleCore(bool value)
        {
            if (!_setVisibleCalled && _config.StartMinimized)
            {
                _setVisibleCalled = true;
                value = false;
            }
            if (!IsDisposed) {
                base.SetVisibleCore(value);
            }
        }

        private void edtShortcut_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.LWin)
            {
                _winKeyDown = false;
            }
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
                edtShortcut.ForeColor = _presets.Any(x => x.id != preset.id && text.Equals(x.shortcut)) ? Color.Red : SystemColors.WindowText;
            }
        }

        private NvPreset GetSelectedNvPreset()
        {
            if (lvNvPresets.SelectedItems.Count > 0)
            {
                var item = lvNvPresets.SelectedItems[0];
                return (NvPreset)item.Tag;
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
            var preset = GetSelectedNvPreset();

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

        private void mnuNvPresets_Opening(object sender, CancelEventArgs e)
        {
            var preset = GetSelectedNvPreset();

            miNvApply.Enabled = preset != null;
            miNvPresetApplyOnStartup.Enabled = preset != null;
            mnuNvDisplay.Enabled = preset != null;
            miNvPresetColorSettings.Enabled = preset != null;
            mnuNvPresetsColorSettings.Enabled = preset != null;
            mnuRefreshRate.Enabled = preset != null;
            miNvPresetDithering.Enabled = preset != null;
            miNvHDR.Enabled = preset != null;

            if (preset != null)
            {
                miNvPresetApplyOnStartup.Checked = _config.NvPresetId_ApplyOnStartup == preset.id;

                if (mnuNvDisplay.DropDownItems.Count == 1)
                {
                    var displays = _nvService.GetDisplays();
                    for (var i = 0; i < displays.Length; i++)
                    {
                        var display = displays[i];
                        var name = display.Name;
                        var screen = Screen.AllScreens.FirstOrDefault(x => x.DeviceName.Equals(name));
                        if (screen != null)
                        {
                            name += " (" + screen.DeviceFriendlyName() + ")";
                        }

                        var item = mnuNvDisplay.DropDownItems.Add(name);
                        item.Tag = display;
                        item.Click += displayMenuItem_Click;
                    }
                }

                BuildDropDownMenu(mnuNvPresetsColorSettings, "Bit depth", typeof(ColorDataDepth), preset.colorData, "ColorDepth");
                BuildDropDownMenu(mnuNvPresetsColorSettings, "Format", typeof(ColorDataFormat), preset.colorData, "ColorFormat");
                BuildDropDownMenu(mnuNvPresetsColorSettings, "Dynamic range", typeof(ColorDataDynamicRange), preset.colorData, "DynamicRange");
                BuildDropDownMenu(mnuNvPresetsColorSettings, "Color space", typeof(ColorDataColorimetry), preset.colorData, "Colorimetry");

                if (preset.displayName != _lastDisplayRefreshRates)
                {
                    while (mnuRefreshRate.DropDownItems.Count > 1)
                    {
                        mnuRefreshRate.DropDownItems.RemoveAt(mnuRefreshRate.DropDownItems.Count - 1);
                    }
                }

                if (mnuRefreshRate.DropDownItems.Count == 1)
                {
                    var refreshRates = _nvService.GetAvailableRefreshRates(preset);
                    _lastDisplayRefreshRates = preset.displayName;

                    foreach (var refreshRate in refreshRates)
                    {
                        var item = mnuRefreshRate.DropDownItems.Add(refreshRate.ToString() + "Hz");
                        item.Tag = refreshRate;
                        item.Click += refreshRateMenuItem_Click;
                    }
                }

                miNvPrimaryDisplay.Checked = preset.primaryDisplay;
                foreach (var item in mnuNvDisplay.DropDownItems)
                {
                    if (item is ToolStripMenuItem)
                    {
                        var menuItem = (ToolStripMenuItem)item;
                        if (menuItem.Tag != null)
                        {
                            menuItem.Checked = ((Display)menuItem.Tag).Name.Equals(preset.displayName);
                        }
                    }
                }

                miNvPresetColorSettings.Checked = preset.applyColorData;

                miRefreshRateIncluded.Checked = preset.applyRefreshRate;
                foreach (var item in mnuRefreshRate.DropDownItems)
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
            }
        }

        private void BuildDropDownMenu(ToolStripMenuItem mnuParent, string name, Type enumType, ColorData colorData, string propertyName)
        {
            PropertyInfo property;
            var subMenuItems = mnuParent.DropDownItems.Find("miColorSettings_" + name, false);
            ToolStripMenuItem subMenuItem = null;
            if (subMenuItems.Length == 0)
            {
                subMenuItem = (ToolStripMenuItem)mnuParent.DropDownItems.Add(name);
                subMenuItem.Name = "miColorSettings_" + name;

                property = typeof(ColorData).GetDeclaredProperty(propertyName);
                subMenuItem.Tag = property;

                foreach (var enumValue in Enum.GetValues(enumType))
                {
                    var item = subMenuItem.DropDownItems.Add(enumValue.ToString());
                    item.Tag = enumValue;
                    item.Click += nvPresetColorDataMenuItem_Click;
                }
            }
            else
            {
                subMenuItem = (ToolStripMenuItem)subMenuItems[0];
                property = (PropertyInfo)subMenuItem.Tag;
            }

            var value = property.GetValue(colorData);

            foreach (var item in subMenuItem.DropDownItems)
            {
                if (item is ToolStripMenuItem)
                {
                    var menuItem = (ToolStripMenuItem)item;
                    if (menuItem.Tag != null)
                    {
                        menuItem.Checked = menuItem.Tag.Equals(value);
                    }
                }
            }
        }

        private void nvPresetColorDataMenuItem_Click(object sender, EventArgs e)
        {
            var menuItem = (ToolStripMenuItem)sender;
            var property = (PropertyInfo)menuItem.OwnerItem.Tag;

            var value = menuItem.Tag;

            var preset = GetSelectedNvPreset();

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

            preset.colorData = colorData;

            AddOrUpdateItem();
        }

        private void refreshRateMenuItem_Click(object sender, EventArgs e)
        {
            var refreshRate = (uint)((ToolStripItem)sender).Tag;

            var preset = GetSelectedNvPreset();

            preset.refreshRate = refreshRate;

            AddOrUpdateItem();
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

        private void miHDRIncluded_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedNvPreset();

            preset.applyHDR = !preset.applyHDR;

            AddOrUpdateItem();
        }

        private void miToggleHDR_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedNvPreset();

            preset.toggleHDR = !preset.toggleHDR;

            if (preset.toggleHDR)
            {
                preset.HDREnabled = false;
            }

            AddOrUpdateItem();
        }

        private void miHDREnabled_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedNvPreset();

            preset.HDREnabled = !preset.HDREnabled;
            preset.toggleHDR = false;

            AddOrUpdateItem();
        }

        private void FillLgPresets()
        {
            var columns = LgPreset.GetColumnNames();

            foreach (var name in columns)
            {
                var columnName = name;
                var parts = name.Split('|');

                var width = 120;
                if (parts.Length > 1)
                {
                    width = Int32.Parse(parts[1]);
                    columnName = parts[0];
                }

                var header = lvLgPresets.Columns.Add(columnName);
                header.Width = width == 120 ? -2 : width;
            }

            if (File.Exists(_lgPresetsFilename))
            {
                var json = File.ReadAllText(_lgPresetsFilename);

                _lgPresets = _JsonDeserializer.Deserialize<List<LgPreset>>(json);
            }
            else
            {
                _lgPresets = new List<LgPreset>();
            }

            foreach (var preset in _lgPresets)
            {
                AddOrUpdateItemLg(preset);
                RegisterShortcut(preset.id, preset.shortcut);
            }
        }

        private void AddOrUpdateItemLg(LgPreset preset = null, ListViewItem specItem = null)
        {
            ListViewItem item = null;
            if (preset == null)
            {
                item = lvLgPresets.SelectedItems[0];
                preset = (LgPreset)item.Tag;
            }
            else
            {
                item = specItem;
            }

            if (preset.id == 0)
            {
                preset.id = preset.GetHashCode();
            }

            var values = preset.GetDisplayValues();

            if (item == null)
            {
                item = lvLgPresets.Items.Add(values[0]);
                item.Tag = preset;
                for (var i = 1; i < values.Count; i++)
                {
                    item.SubItems.Add(values[i]);
                }
                if (!_lgPresets.Any(x => x.id == preset.id))
                {
                    _lgPresets.Add(preset);
                }
            }
            else
            {
                item.Text = values[0];
                for (var i = 1; i < values.Count; i++)
                {
                    item.SubItems[i].Text = values[i];
                }
            }
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
            edtShortcutLg.Enabled = enabled;
            btnSetShortcutLg.Enabled = enabled;
            edtStepsLg.Enabled = enabled;
            btnLgAddButton.Enabled = enabled;
            cbxLgApps.Enabled = enabled;
            btnDeleteLg.Enabled = enabled;

            var preset = GetSelectedLgPreset();

            if (preset != null)
            {
                edtNameLg.Text = preset.name;
                cbxLgApps.SelectedIndex = _lgApps == null ? -1 : _lgApps.FindIndex(x => x.appId.Equals(preset.appId));
                edtShortcutLg.Text = preset.shortcut;
                edtStepsLg.Text = preset.steps.Aggregate("", (a, b) => (string.IsNullOrEmpty(a) ? "" : a + ", ") + b);
            }
            else
            {
                edtNameLg.Text = string.Empty;
                cbxLgApps.SelectedIndex = -1;
                edtShortcutLg.Text = string.Empty;
                edtStepsLg.Text = string.Empty;
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
            if (!string.IsNullOrWhiteSpace(shortcut) && !shortcut.Contains("+"))
            {
                MessageForms.WarningOk("Invalid shortcut. The shortcut should have modifiers and a normal key.");
                return;
            }

            var preset = GetSelectedLgPreset();

            var name = edtNameLg.Text.Trim();

            if (name.Length == 0 || _lgPresets.Any(x => x.id != preset.id && x.name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                MessageForms.WarningOk("The name can not be empty and must be unique.");
                return;
            }

            var clear = !string.IsNullOrEmpty(preset.shortcut);

            preset.name = name;

            if (cbxLgApps.SelectedIndex == -1)
            {
                preset.appId = string.Empty;
            }
            else
            {
                var lgApp = (LgApp)cbxLgApps.SelectedItem;
                preset.appId = lgApp.appId;
            }

            var shortcutChanged = !shortcut.Equals(preset.shortcut);
            if (shortcutChanged)
            {
                preset.shortcut = shortcut;
            }

            var text = edtStepsLg.Text;
            var clearSteps = string.IsNullOrEmpty(text);

            preset.steps.Clear();
            if (!clearSteps)
            {
                text = text.Replace(" ", string.Empty);
                var steps = text.Split(',');
                preset.steps.AddRange(steps);
            }

            AddOrUpdateItemLg();

            SaveLgPresets();

            if (shortcutChanged)
            {
                RegisterShortcut(preset.id, preset.shortcut, clear);
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
                UnregisterHotKey(Handle, preset.id);
            }

            _presets.Remove(preset);

            var item = lvNvPresets.SelectedItems[0];
            lvNvPresets.Items.Remove(item);
        }

        private void btnAddModesNv_Click(object sender, EventArgs e)
        {
            var presets = NvPreset.GetDefaultPresets();
            var added = false;

            foreach (var preset in presets)
            {
                if (!_presets.Any(x => x.colorData.Equals(preset.colorData)))
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
            if (tcMain.SelectedTab == tabLG)
            {
                if (cbxLgDevices.Items.Count == 0)
                {
                    var devices = _lgService.Devices;
                    if (devices == null || !devices.Any())
                    {
                        RefreshLgDevices();
                    }
                    else
                    {
                        FillLgDevices();
                    }
                }
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
                grpNvidiaOptions.Visible = _nvService != null;
                if (grpNvidiaOptions.Visible)
                {
                    if (cbxDitheringBitDepth.Items.Count == 0)
                    {
                        cbxDitheringBitDepth.Items.AddRange(Utils.GetDescriptions<NvDitherBits>().ToArray());
                        cbxDitheringMode.Items.AddRange(Utils.GetDescriptions<NvDitherMode>().ToArray());
                    }


                    var preset = _nvService.GetLastAppliedPreset() ?? GetSelectedNvPreset();
                    chkDitheringEnabled.Checked = preset?.ditheringEnabled ?? true;
                    cbxDitheringBitDepth.SelectedIndex = (int)(preset?.ditheringBits ?? 1);
                    cbxDitheringMode.SelectedIndex = (int)(preset?.ditheringMode ?? 4);
                    FillGradient();
                }
            }
        }

        private void FillLgDevices()
        {
            cbxLgDevices.Items.Clear();
            var devices = _lgService.Devices;
            foreach (var device in devices)
            {
                cbxLgDevices.Items.Add(device);
            }
            if (_lgService.SelectedDevice != null)
            {
                cbxLgDevices.SelectedIndex = cbxLgDevices.Items.IndexOf(_lgService.SelectedDevice);
            }

            if (!devices.Any())
            {
                MessageForms.WarningOk("It seems there's no LG TV available! Please make sure it's connected to the same network as this PC.");
            }

            if (cbxLgApps.Items.Count == 0 && _lgService.SelectedDevice != null)
            {
                _lgService.RefreshApps().ContinueWith((task) => BeginInvoke(new Action<Task<List<LgApp>>>(FillLgApps), new[] { task }));
            }
        }

        private void FillLgApps(Task<List<LgApp>> task)
        {
            _lgApps = task.Result;
            if (_lgApps == null || !_lgApps.Any())
            {
                MessageForms.WarningOk("Could not refresh the apps. Check the log for details.");
                return;
            }
            InitLgApps();
        }

        private void InitLgApps() {
            LgPreset.LgApps = _lgApps;
            cbxLgApps.Items.Clear();
            cbxLgApps.Items.AddRange(_lgApps.ToArray());

            for (var i = 0; i < lvLgPresets.Items.Count; i++)
            {
                var item = lvLgPresets.Items[i];
                AddOrUpdateItemLg((LgPreset)item.Tag, item);
            }

            var preset = GetSelectedLgPreset();
            if (preset != null)
            {
                cbxLgApps.SelectedIndex = _lgApps == null ? -1 : _lgApps.FindIndex(x => x.appId.Equals(preset.appId));
            }
        }

        private void UpdateTrayMenuNv()
        {
            var presets = _presets.Where(x => x.applyColorData || x.applyDithering || x.applyHDR || x.applyRefreshRate);

            _nvTrayMenu.MenuItems.Clear();

            foreach (var preset in presets)
            {
                var name = preset.GetTextForMenuItem();
                if (!string.IsNullOrEmpty(preset.shortcut))
                {
                    name += "\t" + preset.shortcut;
                }

                var item = new MenuItem(name);
                item.Tag = preset;
                item.Click += TrayMenuItemNv_Click;
                _nvTrayMenu.MenuItems.Add(item);
            }
        }

        private void TrayMenuItemNv_Click(object sender, EventArgs e)
        {
            var item = sender as MenuItem;
            var preset = (NvPreset)item.Tag;

            ApplyNvPreset(preset);
        }

        private void trayIconContextMenu_Popup(object sender, EventArgs e)
        {
            _nvTrayMenu.Visible = _nvService != null;

            if (_nvTrayMenu.Visible)
            {
                UpdateTrayMenuNv();
            }

            _lgTrayMenu.Visible = _lgService != null;

            if (_lgTrayMenu.Visible)
            {
                UpdateTrayMenuLg();
            }
        }

        private void UpdateTrayMenuLg()
        {
            var presets = _lgPresets.Where(x => !string.IsNullOrEmpty(x.appId) || x.steps.Any());

            _lgTrayMenu.MenuItems.Clear();

            foreach (var preset in presets)
            {
                var name = preset.name;
                if (!string.IsNullOrEmpty(preset.shortcut))
                {
                    name += "\t" + preset.shortcut;
                }

                var item = new MenuItem(name);
                item.Tag = preset;
                item.Click += TrayMenuItemLg_Click;
                _lgTrayMenu.MenuItems.Add(item);
            }
        }

        private void TrayMenuItemLg_Click(object sender, EventArgs e)
        {
            var item = sender as MenuItem;
            var preset = (LgPreset)item.Tag;

            ApplyLgPreset(preset);
        }

        private void lvNvPresets_DoubleClick(object sender, EventArgs e)
        {
            ApplySelectedNvPreset();
        }

        private void miNvApply_Click(object sender, EventArgs e)
        {
            ApplySelectedNvPreset();
        }

        private void ApplySelectedNvPreset()
        {
            var preset = GetSelectedNvPreset();
            ApplyNvPreset(preset);
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
                UnregisterHotKey(Handle, preset.id);
            }

            _lgPresets.Remove(preset);

            var item = lvLgPresets.SelectedItems[0];
            lvLgPresets.Items.Remove(item);
        }

        private void btnAddLg_Click(object sender, EventArgs e)
        {
            var preset = new LgPreset();
            var name = "New preset";
            string fullname;
            var number = 1;
            do
            {
                fullname = $"{name} ({number})";
                number++;
            } while (_lgPresets.Any(x => x.name.Equals(fullname)));

            preset.name = fullname;

            AddOrUpdateItemLg(preset);
        }

        private void edtDelayDisplaySettings_ValueChanged(object sender, EventArgs e)
        {
            _config.DisplaySettingsDelay = (int)edtDelayDisplaySettings.Value;
        }

        private void ApplyNvPreset(NvPreset preset)
        {
            if (preset == null || _nvService == null)
            {
                return;
            }
            try
            {
                var result = _nvService.ApplyPreset(preset, _config);
                if (!result)
                {
                    throw new Exception("Error while applying NVIDIA preset. At least one setting could not be applied. Check the log for details.");
                }

                UpdateDisplayInfoItems();
            }
            catch (Exception e)
            {
                MessageForms.ErrorOk($"Error applying NVIDIA-preset ({e.TargetSite.Name}): {e.Message}");
            }
        }

        private void ApplyLgPreset(LgPreset preset, bool reconnect = false, bool wait = false)
        {
            if (preset == null)
            {
                return;
            }

            var applyTask = _lgService.ApplyPreset(preset, reconnect).ContinueWith((task) => BeginInvoke(new Action<Task<bool>>(LgPresetApplied), new[] { task }));
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

        private void LoadLog()
        {
            var filename = Path.Combine(_dataDir, "LogFile.txt");

            var log = "No log file found";
            if (File.Exists(filename))
            {
                log = File.ReadAllText(filename);
            }
            edtLog.Text = log;
        }

        private void LoadInfo()
        {
            grpNVIDIAInfo.Visible = _nvService != null;
        }

        private void btnLgRefreshApps_Click(object sender, EventArgs e)
        {
            _lgService.RefreshApps(true).ContinueWith((task) => BeginInvoke(new Action<Task<List<LgApp>>>(FillLgApps), new[] { task }));
        }

        private void cbxLgDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLgDevices.SelectedIndex == -1)
            {
                _lgService.SelectedDevice = null;
            }
            else
            {
                _lgService.SelectedDevice = (PnpDev)cbxLgDevices.SelectedItem;
            }
            btnLGRemoteControl.Enabled = _lgService.SelectedDevice != null;
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

            var button = item.Text;

            var text = edtStepsLg.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                text = button;
            }
            else
            {
                var pos = edtStepsLg.SelectionStart;
                while (pos < text.Length && text.CharAt(pos) != ',')
                {
                    pos++;
                }
                if (pos == text.Length)
                {
                    text += ", " + button;
                }
                else
                {
                    text = text.Substring(0, pos + 1) + " " + button + ", " + text.Substring(pos + 1).Trim();
                }
            }
            edtStepsLg.Text = text.Trim();
        }

        private void clbLgPower_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            if (!(_lgService.Config.PowerOnAfterResume || _lgService.Config.PowerOnAfterStartup))
            {
                MessageForms.InfoOk(
@"Be sure to activate the following setting on the TV, or the app will not be able to wake the TV:

Connection > Mobile TV On > Turn on via Wi-Fi

See Options to test this functionality."
                );
            }

            BeginInvoke(new Action(() =>
            {
                _lgService.Config.PowerOnAfterStartup = clbLgPower.GetItemChecked(0);
                _lgService.Config.PowerOnAfterResume = clbLgPower.GetItemChecked(1);
                _lgService.Config.PowerOffOnShutdown = clbLgPower.GetItemChecked(2);
                _lgService.Config.PowerOffOnStandby = clbLgPower.GetItemChecked(3);
            }));
        }

        private void edtLgPowerOnAfterResumeDelay_ValueChanged(object sender, EventArgs e)
        {
            _lgService.Config.PowerOnDelayAfterResume = (int)edtLgPowerOnAfterResumeDelay.Value;
        }

        private void chkFixChromeFonts_CheckedChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
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
Do you want to continue?";

            if (MessageForms.QuestionYesNo(text) == DialogResult.Yes)
            {
                _lgService.PowerOff();

                MessageForms.InfoOk("Press ENTER to wake the TV.");

                _lgService.WakeSelectedDevice();
            }
        }

        private void btnSetShortcutScreenSaver_Click(object sender, EventArgs e)
        {
            var shortcut = edtBlankScreenSaverShortcut.Text.Trim();

            if (!string.IsNullOrWhiteSpace(shortcut) && !shortcut.Contains("+"))
            {
                MessageForms.WarningOk("Invalid shortcut. The shortcut should have modifiers and a normal key.");
                return;
            }

            var oldShortcut = _config.ScreenSaverShortcut;

            var clear = !string.IsNullOrEmpty(oldShortcut);

            _config.ScreenSaverShortcut = shortcut;

            RegisterShortcut(SHORTCUTID_SCREENSAVER, shortcut, clear);
        }

        private void chkMinimizeOnClose_CheckedChanged(object sender, EventArgs e)
        {
            _config.MinimizeOnClose = chkMinimizeOnClose.Checked;
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            UpdateDisplayInfoItems();
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            var filename = Path.Combine(_dataDir, "LogFile.txt");
            if (File.Exists(filename))
            {
                File.Delete(filename);
                edtLog.Clear();
            }
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
            _lgService.RefreshDevices(false).ContinueWith((task) => BeginInvoke(new Action(FillLgDevices)));
        }

        private void FillGradient()
        {
            if (pbGradient.Image == null)
            {
                pbGradient.Image = Utils.GenerateGradientBitmap(pbGradient.Width, pbGradient.Height);
            }
        }

        private void chkDitheringEnabled_CheckedChanged(object sender, EventArgs e)
        {
            ApplyDitheringOptions();
        }

        private void ApplyDitheringOptions()
        {
            var bitDepth = cbxDitheringBitDepth.SelectedIndex;
            var mode = cbxDitheringMode.SelectedIndex;

            _nvService.SetDithering(chkDitheringEnabled.Checked, (uint)bitDepth, (uint)mode);
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

            var preset = GetSelectedNvPreset();

            preset.ditheringBits = uint.Parse(item.Tag.ToString());

            AddOrUpdateItem();
        }

        private void spatial1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;

            var preset = GetSelectedNvPreset();

            preset.ditheringBits = uint.Parse(item.Tag.ToString());

            AddOrUpdateItem();

        }

        private void btnLGRemoteControl_Click(object sender, EventArgs e)
        {
            if (_remoteControlForm == null || _remoteControlForm.IsDisposed)
            {
                var buttons = _lgService.GetRemoteControlButtons();
                _remoteControlForm = new RemoteControlForm(_lgService, buttons);
                _remoteControlForm.Show();
            }
            else
            {
                _remoteControlForm.Show();
            }
        }

        private void AfterInitialized()
        {
            if (_config.NvPresetId_ApplyOnStartup != 0)
            {
                var preset = _presets.FirstOrDefault(p => p.id == _config.NvPresetId_ApplyOnStartup);
                if (preset == null)
                {
                    _config.NvPresetId_ApplyOnStartup = 0;
                }
                else
                {
                    ApplyNvPreset(preset);
                }
            }
        }

        private void miNvPresetApplyOnStartup_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedNvPreset();
            _config.NvPresetId_ApplyOnStartup = miNvPresetApplyOnStartup.Checked ? preset.id : 0;

            AddOrUpdateItem();
        }
    }
}