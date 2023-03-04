using ColorControl.Common;
using ColorControl.Forms;
using ColorControl.Native;
using ColorControl.Services.AMD;
using ColorControl.Services.Common;
using ColorControl.Services.NVIDIA;
using LgTv;
using NLog;
using NStandard;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorControl.Services.LG
{
    public partial class LgPanel : UserControl
    {
        private static int SHORTCUTID_LGQA = -202;
        private static int SHORTCUTID_GAMEBAR = -101;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private Config _config;
        private LgService _lgService;
        private NvService _nvService;
        private AmdService _amdService;
        private IntPtr _mainHandle;
        private NotifyIcon _trayIcon;

        private string _lgTabMessage;
        private bool _initialized = false;
        private bool _disableEvents = false;
        private LgGameBar _gameBarForm;

        internal LgPanel(LgService lgService, NvService nvService, AmdService amdService, NotifyIcon trayIcon, IntPtr handle)
        {
            _lgService = lgService;
            _nvService = nvService;
            _amdService = amdService;
            _trayIcon = trayIcon;
            _mainHandle = handle;

            _config = AppContext.CurrentContext.Config;

            InitializeComponent();

            FillLgPresets();

            FormUtils.InitSortState(lvLgPresets, _config.LgPresetsSortState);

            //_lgService.AfterApplyPreset += LgServiceAfterApplyPreset;
            _lgService.SelectedDeviceChangedEvent += _lgService_SelectedDeviceChangedEvent;

            edtLgDeviceFilter.Text = _lgService.Config.DeviceSearchKey;

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

            Load += LgPanel_Load;
        }

        private void LgPanel_Load(object sender, EventArgs e)
        {
            _lgService.RefreshDevices(afterStartUp: true).ContinueWith((_) => BeginInvoke(() => AfterLgServiceRefreshDevices()));

            _lgService.InstallEventHandlers();
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

        private void AfterLgServiceRefreshDevices()
        {
            FillLgDevices();

            var startUpParams = AppContext.CurrentContext.StartUpParams;

            if (startUpParams.ExecuteLgPreset)
            {
                var _ = _lgService.ApplyPreset(startUpParams.LgPresetName);
            }
        }

        internal void Save()
        {
            FormUtils.SaveSortState(lvLgPresets.ListViewItemSorter, _config.LgPresetsSortState);
        }

        private void edtShortcut_KeyDown(object sender, KeyEventArgs e)
        {
            ((TextBox)sender).Text = Utils.FormatKeyboardShortcut(e);
        }

        private void edtShortcut_KeyUp(object sender, KeyEventArgs e)
        {
            Utils.HandleKeyboardShortcutUp(e);
        }

        private void FillLgPresets()
        {
            FormUtils.InitListView(lvLgPresets, LgPreset.GetColumnNames());

            foreach (var preset in _lgService.GetPresets())
            {
                AddOrUpdateItemLg(preset);
                Utils.RegisterShortcut(_mainHandle, preset.id, preset.shortcut);
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
                Utils.RegisterShortcut(_mainHandle, preset.id, preset.shortcut, clear);
            }
        }

        public void UpdateInfo()
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

                if (Visible /*tcMain.SelectedTab == tabLG*/)
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

        private void btnDeleteLg_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedLgPreset();

            if (preset == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(preset.shortcut))
            {
                WinApi.UnregisterHotKey(_mainHandle, preset.id);
            }

            _lgService.GetPresets().Remove(preset);

            var item = lvLgPresets.SelectedItems[0];
            lvLgPresets.Items.Remove(item);
        }

        private void btnAddLg_Click(object sender, EventArgs e)
        {
            AddOrUpdateItemLg(_lgService.CreateNewPreset());
        }

        internal void ApplyLgPreset(LgPreset preset, bool reconnect = false, bool wait = false)
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
                //edtShortcutLg.ForeColor = ShortCutExists(text, preset.id) ? Color.Red : SystemColors.WindowText;
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
            FormUtils.BuildServicePresetsMenu(mnuLgNvPresets, _nvService, "NVIDIA", miLgAddNvPreset_Click);
            FormUtils.BuildServicePresetsMenu(mnuLgAmdPresets, _amdService, "AMD", miLgAddAmdPreset_Click);
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

        public void ToggleGameBar()
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
            FormUtils.ListViewSort(sender, e);
        }

        private void mnuLgProgram_Click(object sender, EventArgs e)
        {
            var file = Utils.SelectFile();

            if (file != null)
            {
                FormUtils.AddStepToTextBox(edtStepsLg, $"StartProgram({file.FullName})");
            }
        }

        private void btnLgSettings_Click(object sender, EventArgs e)
        {
            var shortcut = FormUtils.EditShortcut(_config.LgQuickAccessShortcut, "Quick Access shortcut", "LG controller settings");

            if (shortcut == null)
            {
                return;
            }

            var clear = !string.IsNullOrEmpty(_config.LgQuickAccessShortcut);

            _config.LgQuickAccessShortcut = shortcut;

            Utils.RegisterShortcut(_mainHandle, SHORTCUTID_LGQA, _config.LgQuickAccessShortcut, clear);
        }

        private void lvLgPresets_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            FormUtils.ListViewItemChecked<LgPreset>(lvLgPresets, e);
        }

        private void lvNvPresets_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var listView = sender as System.Windows.Forms.ListView;

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
    }
}
