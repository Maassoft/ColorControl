using ColorControl.Services.AMD;
using ColorControl.Services.Common;
using ColorControl.Services.NVIDIA;
using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts;
using ColorControl.Shared.Forms;
using ColorControl.Shared.Native;
using NLog;
using NStandard;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorControl.Services.Samsung
{
    public partial class SamsungPanel : UserControl, IModulePanel
    {
        public static readonly int SHORTCUTID_SAMSUNGQA = -204;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private Config _config;
        private SamsungService _samsungService;
        private NvService _nvService;
        private AmdService _amdService;
        private IntPtr _mainHandle;

        private string _tabMessage;
        private bool _disableEvents = false;

        internal SamsungPanel(SamsungService samsungService, NvService nvService, AmdService amdService, IntPtr handle)
        {
            _samsungService = samsungService;
            _nvService = nvService;
            _amdService = amdService;
            _mainHandle = handle;

            _config = Shared.Common.AppContext.CurrentContext.Config;

            InitializeComponent();

            scSamsungController.Panel2Collapsed = true;

            FillPresets();

            FormUtils.InitSortState(lvSamsungPresets, _config.SamsungPresetsSortState);

            //_samsungService.AfterApplyPreset += SamsungServiceAfterApplyPreset;
            //_samsungService.SelectedDeviceChangedEvent += _samsungService_SelectedDeviceChangedEvent;

            edtSamsungDeviceFilter.Text = _samsungService.Config.DeviceSearchKey;

            var values = Enum.GetValues(typeof(ButtonType));
            foreach (var button in values)
            {
                var text = button.ToString();
                if (text[0] == '_')
                {
                    text = text.Substring(1);
                }

                var item = mnuLgRcButtons.DropDownItems.AddCustom(text);
                item.Click += miLgAddButton_Click;
            }
        }

        public void Init()
        {
            var _ = Handle;
            _samsungService.RefreshDevices(afterStartUp: true).ContinueWith((_) => FormUtils.BeginInvokeCheck(this, AfterSamsungServiceRefreshDevices));
            _samsungService.InstallEventHandlers();
        }

        private void _samsungService_SelectedDeviceChangedEvent(object sender, EventArgs e)
        {
            FormUtils.BeginInvokeCheck(this, () => SetSamsungDevicesSelectedIndex(sender));
        }

        private void SetSamsungDevicesSelectedIndex(object sender)
        {
            if (sender == null)
            {
                cbxSamsungDevices.SelectedIndex = -1;
                return;
            }

            cbxSamsungDevices.SelectedIndex = cbxSamsungDevices.Items.IndexOf(sender);
        }

        private void AfterSamsungServiceRefreshDevices()
        {
            FillSamsungDevices();

            var startUpParams = Shared.Common.AppContext.CurrentContext.StartUpParams;

            if (startUpParams.ExecuteSamsungPreset)
            {
                var _ = _samsungService.ApplyPreset(startUpParams.SamsungPresetName);
            }
        }

        internal void Save()
        {
            FormUtils.SaveSortState(lvSamsungPresets.ListViewItemSorter, _config.SamsungPresetsSortState);
        }

        private void edtShortcut_KeyDown(object sender, KeyEventArgs e)
        {
            ((TextBox)sender).Text = KeyboardShortcutManager.FormatKeyboardShortcut(e);
        }

        private void edtShortcut_KeyUp(object sender, KeyEventArgs e)
        {
            KeyboardShortcutManager.HandleKeyboardShortcutUp(e);
        }

        private void FillPresets()
        {
            FormUtils.InitListView(lvSamsungPresets, SamsungPreset.GetColumnNames());

            foreach (var preset in _samsungService.GetPresets())
            {
                AddOrUpdateItem(preset);
                KeyboardShortcutManager.RegisterShortcut(preset.id, preset.shortcut);
            }
        }

        private void AddOrUpdateItem(SamsungPreset preset = null, ListViewItem specItem = null)
        {
            ServiceFormUtils.AddOrUpdateListItem(lvSamsungPresets, _samsungService.GetPresets(), _config, preset, specItem);
        }

        private void btnCloneLg_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedPreset();

            var newPreset = preset.Clone();
            AddOrUpdateItem(newPreset);
        }

        private SamsungPreset GetSelectedPreset()
        {
            if (lvSamsungPresets.SelectedItems.Count > 0)
            {
                var item = lvSamsungPresets.SelectedItems[0];
                return (SamsungPreset)item.Tag;
            }
            else
            {
                return null;
            }
        }

        private void lvSamsungPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            var enabled = lvSamsungPresets.SelectedItems.Count > 0;
            btnApplyLg.Enabled = enabled;
            btnCloneLg.Enabled = enabled;
            edtNameLg.Enabled = enabled;
            chkSamsungQuickAccess.Enabled = enabled;
            edtSamsungPresetDescription.Enabled = enabled;
            cbxSamsungPresetDevice.Enabled = enabled;
            edtShortcutLg.Enabled = enabled;
            btnSetShortcutLg.Enabled = enabled;
            edtStepsLg.Enabled = enabled;
            btnSamsungAddButton.Enabled = enabled;
            cbxSamsungApps.Enabled = enabled;
            btnSamsungRefreshApps.Enabled = enabled;
            btnDeleteLg.Enabled = enabled;
            cbxSamsungPresetTrigger.Enabled = enabled;
            edtSamsungPresetTriggerConditions.Enabled = enabled;
            btnSamsungPresetEditTriggerConditions.Enabled = enabled;
            edtSamsungPresetIncludedProcesses.Enabled = enabled;
            edtSamsungPresetExcludedProcesses.Enabled = enabled;

            var preset = GetSelectedPreset();

            if (preset != null)
            {
                edtNameLg.Text = preset.name;
                chkSamsungQuickAccess.Checked = preset.ShowInQuickAccess;
                edtSamsungPresetDescription.Text = preset.Description;
                var lgApps = _samsungService?.GetApps();
                cbxSamsungApps.SelectedIndex = lgApps == null ? -1 : lgApps.FindIndex(x => x.AppId.Equals(preset.AppId));
                edtShortcutLg.Text = preset.shortcut;
                edtStepsLg.Text = preset.Steps.Aggregate("", (a, b) => (string.IsNullOrEmpty(a) ? "" : a + ", ") + b);

                var index = -1;

                for (var i = 0; i < cbxSamsungPresetDevice.Items.Count; i++)
                {
                    var pnpDev = (SamsungDevice)cbxSamsungPresetDevice.Items[i];
                    if ((string.IsNullOrEmpty(preset.DeviceMacAddress) && string.IsNullOrEmpty(pnpDev.MacAddress)) || pnpDev.MacAddress.Equals(preset.DeviceMacAddress, StringComparison.OrdinalIgnoreCase))
                    {
                        index = i;
                        break;
                    }
                }

                cbxSamsungPresetDevice.SelectedIndex = index;

                var trigger = preset.Triggers.FirstOrDefault();

                FormUtils.SetComboBoxEnumIndex(cbxSamsungPresetTrigger, (int)(trigger?.Trigger ?? PresetTriggerType.None));

                edtSamsungPresetTriggerConditions.Text = Utils.GetDescriptions<PresetConditionType>(trigger != null ? (int)trigger.Conditions : 0).Join(", ");
                edtSamsungPresetTriggerConditions.Tag = trigger != null ? (int)trigger.Conditions : 0;

                edtSamsungPresetIncludedProcesses.Text = trigger?.IncludedProcesses?.Join(", ") ?? string.Empty;
                edtSamsungPresetExcludedProcesses.Text = trigger?.ExcludedProcesses?.Join(", ") ?? string.Empty;
            }
            else
            {
                edtNameLg.Text = string.Empty;
                chkSamsungQuickAccess.Checked = false;
                edtSamsungPresetDescription.Text = string.Empty;
                cbxSamsungApps.SelectedIndex = -1;
                edtShortcutLg.Text = string.Empty;
                edtStepsLg.Text = string.Empty;
                cbxSamsungPresetDevice.SelectedIndex = -1;
                cbxSamsungPresetTrigger.SelectedIndex = -1;
                edtSamsungPresetTriggerConditions.Text = string.Empty;
                edtSamsungPresetTriggerConditions.Tag = 0;
                edtSamsungPresetIncludedProcesses.Text = string.Empty;
                edtSamsungPresetExcludedProcesses.Text = string.Empty;
            }
        }

        private async void btnApplyLg_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedPreset();

            await ApplyPreset(preset);
        }

        private void btnSetShortcutLg_Click(object sender, EventArgs e)
        {
            var shortcut = edtShortcutLg.Text.Trim();
            if (!KeyboardShortcutManager.ValidateShortcut(shortcut))
            {
                return;
            }

            var preset = GetSelectedPreset();

            var name = edtNameLg.Text.Trim();

            if (name.Length == 0 || _samsungService.GetPresets().Any(x => x.id != preset.id && x.name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                MessageForms.WarningOk("The name can not be empty and must be unique.");
                return;
            }

            var clear = !string.IsNullOrEmpty(preset.shortcut);

            preset.name = name;
            preset.ShowInQuickAccess = chkSamsungQuickAccess.Checked;
            preset.Description = edtSamsungPresetDescription.Text.Trim();

            if (cbxSamsungPresetDevice.SelectedIndex == -1)
            {
                preset.DeviceMacAddress = string.Empty;
            }
            else
            {
                var device = (SamsungDevice)cbxSamsungPresetDevice.SelectedItem;
                preset.DeviceMacAddress = device.MacAddress;
            }

            if (cbxSamsungApps.SelectedIndex == -1)
            {
                preset.AppId = string.Empty;
            }
            else
            {
                var app = (SamsungApp)cbxSamsungApps.SelectedItem;
                preset.AppId = app.AppId;
            }

            var triggerType = FormUtils.GetComboBoxEnumItem<PresetTriggerType>(cbxSamsungPresetTrigger);
            preset.UpdateTrigger(triggerType,
                                 (PresetConditionType)edtSamsungPresetTriggerConditions.Tag,
                                 edtSamsungPresetIncludedProcesses.Text,
                                 edtSamsungPresetExcludedProcesses.Text);

            preset.shortcut = shortcut;

            var text = edtStepsLg.Text;

            Utils.ParseWords(preset.Steps, text);

            AddOrUpdateItem();

            KeyboardShortcutManager.RegisterShortcut(preset.id, preset.shortcut);
        }

        public void UpdateInfo()
        {
            if (_samsungService == null)
            {
                return;
            }

            //if (scSamsungController.Panel2.Controls.Count == 0)
            //{
            //    var rcPanel = new RemoteControlPanel(_samsungService, _samsungService.GetRemoteControlButtons());
            //    rcPanel.Parent = scSamsungController.Panel2;
            //    rcPanel.Dock = DockStyle.Fill;

            //    if (DarkModeUtils.UseDarkMode)
            //    {
            //        DarkModeUtils.SetControlTheme(rcPanel);
            //    }
            //}
            //chkSamsungRemoteControlShow.Checked = _samsungService.Config.ShowRemoteControl;
            //scSamsungController.Panel2Collapsed = !_samsungService.Config.ShowRemoteControl;

            FormUtils.BuildComboBox(cbxSamsungPresetTrigger, PresetTriggerType.Resume, PresetTriggerType.Shutdown, PresetTriggerType.Standby, PresetTriggerType.Startup, PresetTriggerType.Reserved5, PresetTriggerType.ScreensaverStart, PresetTriggerType.ScreensaverStop);

            if (!string.IsNullOrEmpty(_tabMessage))
            {
                MessageForms.WarningOk(_tabMessage);
                _tabMessage = null;
            }
        }

        private void FillSamsungDevices()
        {
            var devices = _samsungService.Devices;

            cbxSamsungDevices.Items.Clear();
            cbxSamsungDevices.Items.AddRange(devices.ToArray());

            cbxSamsungPresetDevice.Items.Clear();
            cbxSamsungPresetDevice.Items.AddRange(devices.ToArray());
            var globalDevice = new SamsungDevice("Globally selected device", string.Empty, string.Empty, true, true);
            cbxSamsungPresetDevice.Items.Insert(0, globalDevice);

            var device = _samsungService.SelectedDevice;

            if (device != null)
            {
                cbxSamsungDevices.SelectedIndex = cbxSamsungDevices.Items.IndexOf(device);
            }

            if (!devices.Any())
            {
                var message = "It seems there's no Samsung TV available! Please make sure it's connected to the same network as this PC.";

                if (Visible)
                {
                    MessageForms.WarningOk(message);
                }
                else
                {
                    _tabMessage = message;
                }
            }

            //if (cbxSamsungApps.Items.Count == 0 && device != null)
            //{
            //    _samsungService.RefreshAppsAsync().ContinueWith((task) => FormUtils.BeginInvokeCheck(this, () => FillApps(false)));
            //}

            btnSamsungDeviceConvertToCustom.Enabled = devices.Any();

            cbxSamsungDevices_SelectedIndexChanged(cbxSamsungDevices, null);

            SetSamsungDevicePowerOptions();
        }

        private void SetSamsungDevicePowerOptions()
        {
            var device = _samsungService.SelectedDevice;
            clbLgPower.Enabled = device != null;

            _disableEvents = true;
            try
            {
                clbLgPower.SetItemChecked(0, device?.Options.PowerOnAfterStartup ?? false);
                clbLgPower.SetItemChecked(1, device?.Options.PowerOnAfterResume ?? false);
                clbLgPower.SetItemChecked(2, device?.Options.PowerOffOnShutdown ?? false);
                clbLgPower.SetItemChecked(3, device?.Options.PowerOffOnStandby ?? false);
                clbLgPower.SetItemChecked(4, device?.Options.PowerOffOnScreenSaver ?? false);
                clbLgPower.SetItemChecked(5, device?.Options.PowerOnAfterScreenSaver ?? false);
                clbLgPower.SetItemChecked(6, device?.Options.PowerOnAfterManualPowerOff ?? false);
                clbLgPower.SetItemChecked(7, device?.Options.TriggersEnabled ?? true);
                clbLgPower.SetItemChecked(8, device?.Options.PowerOffByWindows ?? false);
                clbLgPower.SetItemChecked(9, device?.Options.PowerOnByWindows ?? false);
                clbLgPower.SetItemChecked(10, device?.Options.UseSecureConnection ?? false);
            }
            finally
            {
                _disableEvents = false;
            }
        }

        private void FillApps(bool forced)
        {
            var apps = _samsungService?.GetApps();
            if (forced && (apps == null || !apps.Any()))
            {
                MessageForms.WarningOk("Could not refresh the apps. Check the log for details.");
                return;
            }
            InitApps();
        }

        private void InitApps()
        {
            var apps = _samsungService?.GetApps();

            cbxSamsungApps.Items.Clear();
            if (apps != null)
            {
                cbxSamsungApps.Items.AddRange(apps.ToArray());
            }

            for (var i = 0; i < lvSamsungPresets.Items.Count; i++)
            {
                var item = lvSamsungPresets.Items[i];
                AddOrUpdateItem((SamsungPreset)item.Tag, item);
            }

            var preset = GetSelectedPreset();
            if (preset != null)
            {
                cbxSamsungApps.SelectedIndex = apps == null ? -1 : apps.FindIndex(x => x.AppId.Equals(preset.AppId));
            }
        }

        private void btnDeleteLg_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedPreset();

            if (preset == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(preset.shortcut))
            {
                WinApi.UnregisterHotKey(_mainHandle, preset.id);
            }

            _samsungService.GetPresets().Remove(preset);

            var item = lvSamsungPresets.SelectedItems[0];
            lvSamsungPresets.Items.Remove(item);
        }

        private void btnAddLg_Click(object sender, EventArgs e)
        {
            AddOrUpdateItem(_samsungService.CreateNewPreset());
        }

        internal async Task ApplyPreset(SamsungPreset preset)
        {
            if (preset == null)
            {
                return;
            }

            var result = await _samsungService.ApplyPreset(preset);
            if (!result)
            {
                MessageForms.WarningOk("Could not apply the preset (entirely). Check the log for details.");
            }
        }

        private void btnSamsungRefreshApps_Click(object sender, EventArgs e)
        {
            //_samsungService.RefreshApps(true).ContinueWith((task) => FormUtils.BeginInvokeCheck(this, () => FillLgApps(true)));
        }

        private void cbxSamsungDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbxSamsungPcHdmiPort.Enabled = cbxSamsungDevices.SelectedIndex > -1;
            if (cbxSamsungDevices.SelectedIndex == -1)
            {
                _samsungService.SelectedDevice = null;
                cbxSamsungPcHdmiPort.SelectedIndex = 0;
            }
            else
            {
                _samsungService.SelectedDevice = (SamsungDevice)cbxSamsungDevices.SelectedItem;
                //cbxSamsungPcHdmiPort.SelectedIndex = _samsungService.SelectedDevice.HDMIPortNumber;
            }
            chkSamsungRemoteControlShow.Enabled = _samsungService.SelectedDevice != null;
            btnSamsungRemoveDevice.Enabled = _samsungService.SelectedDevice != null && _samsungService.SelectedDevice.IsCustom;
            btnSamsungDeviceConvertToCustom.Enabled = _samsungService.SelectedDevice != null && !_samsungService.SelectedDevice.IsCustom;

            SetSamsungDevicePowerOptions();
        }

        private void lvSamsungPresets_DoubleClick(object sender, EventArgs e)
        {
            ApplySelectedPreset();
        }

        private async void ApplySelectedPreset()
        {
            var preset = GetSelectedPreset();
            await ApplyPreset(preset);
        }

        private void btnSamsungAddButton_Click(object sender, EventArgs e)
        {
            mnuLgButtons.ShowCustom(btnSamsungAddButton);
        }

        private void miLgAddButton_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripItem;

            FormUtils.AddStepToTextBox(edtStepsLg, item.Text);
        }

        private void miLgAddAction_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripItem;
            var action = item.Tag as SamsungDevice.InvokableAction;

            var value = ShowActionForm(action);

            var text = action.Name;

            if (!string.IsNullOrEmpty(value))
            {
                text += $"({value})";
            }

            FormUtils.AddStepToTextBox(edtStepsLg, text);
        }

        private string ShowActionForm(SamsungDevice.InvokableAction action)
        {
            var text = action.Name;
            var title = action.Title ?? action.Name;

            var value = string.Empty;

            if (action.EnumType == null)
            {
                if (action.MinValue != action.MaxValue)
                {
                    List<FieldDefinition> fields = new();

                    if (action.NumberOfValues == 1)
                    {
                        fields.Add(new FieldDefinition
                        {
                            Label = "Enter desired " + title,
                            FieldType = FieldType.Numeric,
                            MinValue = action.MinValue,
                            MaxValue = action.MaxValue,
                        });
                    }
                    else
                    {
                        var array = Enumerable.Range(0, action.NumberOfValues);

                        fields.AddRange(array.Select(i => new FieldDefinition
                        {
                            Label = "Value for " + (action.ValueLabels != null ? action.ValueLabels[i] : i.ToString()),
                            FieldType = FieldType.Numeric,
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
                var values = MessageForms.ShowDialog("Choose value", new[] {
                    new FieldDefinition
                    {
                        Label = "Choose desired " + title,
                        FieldType = FieldType.DropDown,
                        Values = Utils.GetDescriptions(action.EnumType, replaceUnderscore: true),
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
            if (_disableEvents)
            {
                return;
            }

            var device = _samsungService?.SelectedDevice;
            if (device == null)
            {
                return;
            }

            if (e.Index is 0 or 1 or 4 or 7 && !(device.Options.PowerOnAfterResume || device.Options.PowerOnAfterStartup || device.Options.PowerOnAfterScreenSaver || device.Options.PowerOnByWindows))
            {
                MessageForms.InfoOk(
@"Be sure to activate the following setting on the TV, or the app will not be able to wake the TV:

Connection > Network > Expert Settings:
1) Power On with Mobile
2) IP Remote

Use 'Settings > Test power off/on' to test this functionality."
                );
            }

            FormUtils.BeginInvokeCheck(this, () =>
            {
                device.Options.PowerOnAfterStartup = clbLgPower.GetItemChecked(0);
                device.Options.PowerOnAfterResume = clbLgPower.GetItemChecked(1);
                device.Options.PowerOffOnShutdown = clbLgPower.GetItemChecked(2);
                device.Options.PowerOffOnStandby = clbLgPower.GetItemChecked(3);
                device.Options.PowerOffOnScreenSaver = clbLgPower.GetItemChecked(4);
                device.Options.PowerOnAfterScreenSaver = clbLgPower.GetItemChecked(5);
                device.Options.PowerOnAfterManualPowerOff = clbLgPower.GetItemChecked(6);
                device.Options.TriggersEnabled = clbLgPower.GetItemChecked(7);
                device.Options.PowerOffByWindows = clbLgPower.GetItemChecked(8);
                device.Options.PowerOnByWindows = clbLgPower.GetItemChecked(9);
                device.Options.UseSecureConnection = clbLgPower.GetItemChecked(10);

                if (e.Index == 10)
                {
                    _samsungService.RefreshDevices().ContinueWith((_) => FormUtils.BeginInvokeCheck(this, () => FillSamsungDevices()));
                }
            });
        }

        private void edtSamsungDeviceFilter_TextChanged(object sender, EventArgs e)
        {
            if (_samsungService != null)
            {
                _samsungService.Config.DeviceSearchKey = edtSamsungDeviceFilter.Text;
            }
        }

        private void btnSamsungDeviceFilterRefresh_Click(object sender, EventArgs e)
        {
            RefreshSamsungDevices();
        }

        private void RefreshSamsungDevices()
        {
            _samsungService.RefreshDevices(false).ContinueWith((task) => FormUtils.BeginInvokeCheck(this, () => FillSamsungDevices()));
        }

        private void edtShortcutLg_TextChanged(object sender, EventArgs e)
        {
            var text = edtShortcutLg.Text;

            var preset = GetSelectedPreset();

            ServiceFormUtils.UpdateShortcutTextBox(edtShortcutLg, preset);
        }

        private async void btnSamsungAddDevice_Click(object sender, EventArgs e)
        {
            var values = MessageForms.ShowDialog("Add tv", new[] { "Name", "Ip-address", "MAC-address" }, ValidateAddDevice);
            if (values.Any())
            {
                var device = new SamsungDevice(values[0].Value.ToString(), values[1].Value.ToString(), values[2].Value.ToString());

                var form = MessageForms.ShowProgress("Connecting to device...");

                var result = false;
                try
                {
                    result = await device.TestConnectionAsync();
                }
                finally
                {
                    form.Close();
                }

                if (!result && MessageForms.QuestionYesNo("Unable to connect to the device. Are you sure you want to add it?") != DialogResult.Yes)
                {
                    return;
                }

                _samsungService.AddCustomDevice(device);

                FillSamsungDevices();
            }
        }

        private string ValidateAddDevice(IEnumerable<FieldDefinition> values)
        {
            if (values.Any(v => string.IsNullOrEmpty(v.Value?.ToString())))
            {
                return "Please fill in all the fields";
            }

            return null;
        }

        private void btnSamsungRemoveDevice_Click(object sender, EventArgs e)
        {
            if (MessageForms.QuestionYesNo("Are you sure you want to remove this device?") != DialogResult.Yes)
            {
                return;
            }

            var device = _samsungService.SelectedDevice;

            if (device != null)
            {
                _samsungService.RemoveCustomDevice(device);

                FillSamsungDevices();
            }
        }

        private void mnuLgButtons_Opening(object sender, CancelEventArgs e)
        {
            mnuLgActions.DropDownItems.Clear();

            var preset = GetSelectedPreset();

            if (preset == null)
            {
                return;
            }

            var device = _samsungService.GetPresetDevice(preset);

            BuildActionMenu(device, mnuLgActions.DropDownItems, mnuLgActions.Name, miLgAddAction_Click, _samsungService.Config.ShowAdvancedActions);
            ServiceFormUtils.BuildServicePresetsMenu(mnuLgNvPresets, _nvService, "NVIDIA", miLgAddNvPreset_Click);
            ServiceFormUtils.BuildServicePresetsMenu(mnuLgAmdPresets, _amdService, "AMD", miLgAddAmdPreset_Click);
        }

        private void btnSamsungDeviceConvertToCustom_Click(object sender, EventArgs e)
        {
            if (MessageForms.QuestionYesNo(
@"This will convert the automatically detected device to a custom variant.
This means that the device will remain here even if it is not detected anymore.
Do you want to continue?"
               ) != DialogResult.Yes)
            {
                return;
            }

            _samsungService.SelectedDevice.ConvertToCustom();

            FillSamsungDevices();
        }

        private void chkSamsungRemoteControlShow_CheckedChanged(object sender, EventArgs e)
        {
            //scSamsungController.Panel2Collapsed = !chkSamsungRemoteControlShow.Checked;
            //_samsungService.Config.ShowRemoteControl = chkSamsungRemoteControlShow.Checked;
        }

        private void BuildActionMenu(SamsungDevice device, ToolStripItemCollection parent, string parentName, EventHandler clickEvent, bool showAdvanced = false)
        {
            if (device == null)
            {
                return;
            }

            var actions = device.GetInvokableActions(showAdvanced);

            foreach (var action in actions)
            {
                var text = action.Title ?? action.Name;

                var item = parent.AddCustom(text);
                item.Tag = action;
                item.Click += clickEvent;

                //var menu = FormUtils.BuildDropDownMenuEx(catMenuItem.DropDownItems, catMenuItem.Name, action.Title, action.EnumType, clickEvent, action, (int)action.MinValue, (int)action.MaxValue, action.NumberOfValues > 1);
            }
        }

        private void cbxSamsungApps_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ((char)Keys.Back))
            {
                cbxSamsungApps.SelectedIndex = -1;
            }
        }

        private void btnSamsungPresetEditTriggerConditions_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedPreset();

            if (preset == null)
            {
                return;
            }

            var dropDownValues = Utils.GetDescriptions<PresetConditionType>(fromValue: 1);

            var values = MessageForms.ShowDialog("Set trigger conditions", new[] {
                    new FieldDefinition
                    {
                        Label = "Set desired trigger conditions",
                        FieldType = FieldType.Flags,
                        Values = dropDownValues,
                        Value = edtSamsungPresetTriggerConditions.Tag ?? 0
                    }
                });

            if (!values.Any())
            {
                return;
            }

            var value = (PresetConditionType)values.First().Value;
            edtSamsungPresetTriggerConditions.Tag = (PresetConditionType)values.First().Value;
            edtSamsungPresetTriggerConditions.Text = Utils.GetDescriptions<PresetConditionType>((int)value).Join(", ");
        }

        private void edtSamsungGameBarShortcut_KeyDown(object sender, KeyEventArgs e)
        {
            ((TextBox)sender).Text = KeyboardShortcutManager.FormatKeyboardShortcut(e);
        }

        private void edtSamsungGameBarShortcut_KeyUp(object sender, KeyEventArgs e)
        {
            KeyboardShortcutManager.HandleKeyboardShortcutUp(e);
        }

        private void btnSamsungDeviceOptionsHelp_Click(object sender, EventArgs e)
        {
            MessageForms.InfoOk(
@"Notes:
- power on after startup requires ""Automatically start after login"" - see Options
- power on after resume from standby may need some retries for waking TV - see Options
- power off on shutdown: because this app cannot detect a restart, restarting could also trigger this. Hold down Ctrl on restart to prevent power off.
- Use Windows power settings: powering on/off of TV will follow Windows settings. If activated, it's better to disable all other power options except power off on shutdown.
");
        }

        private void cbxSamsungPcHdmiPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_samsungService?.SelectedDevice == null)
            {
                return;
            }

            //_samsungService.SelectedDevice.HDMIPortNumber = cbxSamsungPcHdmiPort.SelectedIndex;
        }

        private void lvSamsungPresets_ColumnClick(object sender, ColumnClickEventArgs e)
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

        private void btnSamsungSettings_Click(object sender, EventArgs e)
        {
            mnuLgSettings.ShowCustom(btnSamsungSettings);
        }

        private void lvSamsungPresets_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            ServiceFormUtils.ListViewItemChecked<SamsungPreset>(lvSamsungPresets, e);
        }

        private async void miTestPowerOffOn_Click(object sender, EventArgs e)
        {
            var device = _samsungService.SelectedDevice;

            if (device == null)
            {
                return;
            }

            var text =
@"The TV will now power off. Please wait for the TV to be powered off completely (relay click) and press ENTER to wake it again.
For waking up to work, you need to activate the following setting on the TV:

Connection > Network > Expert Settings:
1) Power On with Mobile
2) IP Remote

Do you want to continue?";

            if (MessageForms.QuestionYesNo(text) == DialogResult.Yes)
            {
                await device.ConnectAsync();

                await device.PowerOffAsync();

                MessageForms.InfoOk("Press ENTER to wake the TV.");

                await device.WakeAndConnect();
            }
        }

        private void miNvSettings_Click(object sender, EventArgs e)
        {
            var powerRetries = new FieldDefinition
            {
                FieldType = FieldType.Numeric,
                Label = "Maximum number of retries powering on after startup/resume.",
                SubLabel = "Retries are necessary to wait for the network link of your pc to be established.",
                MinValue = 1,
                MaxValue = 40,
                Value = _samsungService.Config.PowerOnRetries
            };

            var shutDownDelayField = new FieldDefinition
            {
                FieldType = FieldType.Numeric,
                Label = "Delay when shutting down/restarting PC (milliseconds).",
                SubLabel = "This delay may prevent the tv from powering off when restarting the pc.",
                MinValue = 0,
                MaxValue = 5000,
                Value = _samsungService.Config.ShutdownDelay
            };

            var buttonDelayField = new FieldDefinition
            {
                FieldType = FieldType.Numeric,
                Label = "Default delay between remote control button presses (milliseconds).",
                SubLabel = "Increasing this delay may prevent skipped button presses.",
                MinValue = 100,
                MaxValue = 2000,
                Value = _samsungService.Config.DefaultButtonDelay
            };

            var quickAccessShortcutField = new FieldDefinition
            {
                FieldType = FieldType.Shortcut,
                Label = "Quick Access shortcut",
                Value = _samsungService.Config.QuickAccessShortcut
            };

            var setDeviceField = new FieldDefinition
            {
                FieldType = FieldType.CheckBox,
                Label = "Automatically set selected device to last powered on",
                Value = _samsungService.Config.SetSelectedDeviceByPowerOn
            };

            var showAdvancedField = new FieldDefinition
            {
                FieldType = FieldType.CheckBox,
                Label = "Show advanced actions under the Expert-button (Service Menu)",
                SubLabel = "WARNING: entering the Service Menu is not recommended. This app and its creator are in no way accountable for any damages it may cause to your tv.",
                Value = _samsungService.Config.ShowAdvancedActions
            };

            var values = MessageForms.ShowDialog("Samsung controller settings", new[] { powerRetries, shutDownDelayField, buttonDelayField, quickAccessShortcutField, setDeviceField, showAdvancedField });

            if (values?.Any() != true)
            {
                return;
            }

            _samsungService.Config.PowerOnRetries = powerRetries.ValueAsInt;
            _samsungService.Config.ShutdownDelay = shutDownDelayField.ValueAsInt;
            _samsungService.Config.DefaultButtonDelay = buttonDelayField.ValueAsInt;

            var shortcutQA = quickAccessShortcutField.Value.ToString();

            _samsungService.Config.QuickAccessShortcut = shortcutQA;

            KeyboardShortcutManager.RegisterShortcut(SHORTCUTID_SAMSUNGQA, _samsungService.Config.QuickAccessShortcut);

            _samsungService.Config.SetSelectedDeviceByPowerOn = setDeviceField.ValueAsBool;
            _samsungService.Config.ShowAdvancedActions = showAdvancedField.ValueAsBool;
        }

        private void clbLgPower_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSamsungDeviceOptionsSettings.Enabled = clbLgPower.SelectedIndex == 4;
        }

        private void btnSamsungDeviceOptionsSettings_Click(object sender, EventArgs e)
        {
            var device = _samsungService.SelectedDevice;
            var selectedItem = clbLgPower.SelectedItem;

            var durationField = new FieldDefinition
            {
                Label = "Enter the minimal duration the screen saver must be running (in seconds)",
                FieldType = FieldType.Numeric,
                MinValue = 0,
                MaxValue = 3600,
                Value = device.Options.ScreenSaverMinimalDuration
            };
            var turnOffField = new FieldDefinition
            {
                Label = "Turn screen off instead of power off",
                SubLabel = "NOTE: turning the screen off might not work correctly on all models",
                FieldType = FieldType.CheckBox,
                Value = device.Options.TurnScreenOffOnScreenSaver
            };
            var turnOnField = new FieldDefinition
            {
                Label = "Turn screen on instead of power on",
                FieldType = FieldType.CheckBox,
                Value = device.Options.TurnScreenOnAfterScreenSaver
            };
            var manualField = new FieldDefinition
            {
                Label = "Perform action even on manually executed screen saver",
                FieldType = FieldType.CheckBox,
                Value = device.Options.HandleManualScreenSaver
            };

            if (!MessageForms.ShowDialog($"Settings - {selectedItem}", new[] { durationField, turnOffField, turnOnField, manualField }).Any())
            {
                return;
            }

            device.Options.ScreenSaverMinimalDuration = durationField.ValueAsInt;
            device.Options.TurnScreenOffOnScreenSaver = turnOffField.ValueAsBool;
            device.Options.TurnScreenOnAfterScreenSaver = turnOnField.ValueAsBool;
            device.Options.HandleManualScreenSaver = manualField.ValueAsBool;
        }

        private void btnExpert_Click(object sender, EventArgs e)
        {
            mnuExpert.ShowCustom(btnExpert);
        }

        private void mnuExpert_Opening(object sender, CancelEventArgs e)
        {
            miExpertDummy.Visible = false;

            var device = _samsungService.SelectedDevice;

            if (device == null)
            {
                return;
            }

            var actions = device.GetInvokableActions(_samsungService.Config.ShowAdvancedActions);

            foreach (var action in actions)
            {
                FormUtils.BuildMenuItem(mnuExpert.Items, action.Name, action.Name, action, mnuExpertClick);
            }
        }

        private async void mnuExpertClick(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;
            var action = (SamsungDevice.InvokableAction)item.Tag;

            var device = _samsungService.SelectedDevice;

            if (device == null)
            {
                return;
            }

            await device.ExecuteActionAsync(action, null, Shared.Common.AppContext.CurrentContext, _samsungService.Config);
        }
    }
}
