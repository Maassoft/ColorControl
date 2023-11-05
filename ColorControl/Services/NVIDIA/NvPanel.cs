using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts;
using ColorControl.Shared.Contracts.NVIDIA;
using ColorControl.Shared.Forms;
using ColorControl.Shared.Native;
using ColorControl.Shared.Services;
using NLog;
using novideo_srgb;
using nspector;
using nspector.Common;
using nspector.Common.Meta;
using NStandard;
using NvAPIWrapper.Display;
using NvAPIWrapper.GPU;
using NvAPIWrapper.Native.Display;
using NWin32.NativeTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorControl.Services.NVIDIA
{
    partial class NvPanel : UserControl, IModulePanel
    {
        public static readonly int SHORTCUTID_NVQA = -200;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private Config _config;
        private NvService _nvService;
        private IntPtr _mainHandle;
        private readonly AppContextProvider _appContextProvider;
        private string _lastDisplayRefreshRates = string.Empty;
        private NotifyIcon _trayIcon;
        private RpcClientService _rpcService;

        public NvPanel(NvService nvService, NotifyIcon trayIcon, IntPtr handle, AppContextProvider appContextProvider, RpcClientService rpcService)
        {
            _nvService = nvService;
            _trayIcon = trayIcon;
            _mainHandle = handle;
            _appContextProvider = appContextProvider;
            _config = appContextProvider.GetAppContext().Config;
            _rpcService = rpcService;

            _rpcService.Name = "NvService";

            InitializeComponent();

            FillNvPresets();

            FormUtils.InitSortState(lvNvPresets, _config.NvPresetsSortState);

            _nvService.AfterApplyPreset += NvServiceAfterApplyPreset;

            MainViewModel.ConfigPath = _appContextProvider.GetAppContext().DataPath;

            if (_nvService.Config.ApplyNovideoOnStartup)
            {
                MainWindow.CreateAndShow(false);
            }
        }

        private void lvLgPresets_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            FormUtils.ListViewSort(sender, e);
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

        private void NvServiceAfterApplyPreset(object sender, NvPreset preset)
        {
            UpdateDisplayInfoItems();
        }

        private void FillNvPresets()
        {
            FormUtils.InitListView(lvNvPresets, NvPreset.GetColumnNames());

            UpdateDisplayInfoItems();

            foreach (var preset in _nvService.GetPresets())
            {
                AddOrUpdateItem(preset);
                KeyboardShortcutManager.RegisterShortcut(preset.id, preset.shortcut);
            }
        }

        private void UpdateDisplayInfoItems()
        {
            var displays = _nvService?.GetDisplayInfos();
            if (displays == null || Program.IsRestarting)
            {
                return;
            }

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
                    //item.BackColor = Color.LightGray;
                }
                item.BackColor = _appContextProvider.GetAppContext().Config.UseDarkMode ? Color.DimGray : Color.LightGray;

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

            UpdateNvGpuInfo();

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

            btnNvSetClocks.Visible = _nvService.Config.ShowOverclocking;
        }

        private void AddOrUpdateItem(NvPreset preset = null)
        {
            ServiceFormUtils.AddOrUpdateListItem(lvNvPresets, _nvService.GetPresets(), _config, preset);
        }

        private async void btnApply_Click(object sender, EventArgs e)
        {
            await ApplySelectedNvPreset();
        }

        private void lvNvPresets_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            ServiceFormUtils.ListViewItemChecked<NvPreset>(lvNvPresets, e);
        }

        private void lvNvPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            var preset = GetSelectedNvPreset();
            var enabled = preset != null;

            FormUtils.EnableControls(this, enabled, new List<Control> { lvNvPresets, btnAddModesNv, btnChange, edtNvGpuInfo, btnNvSetClocks, btnNvSettings, edtNvOverclock, btnNvSetClocks });

            btnClone.Enabled = preset != null || GetSelectedNvPreset(true) != null;

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

        private void btnNvPresetDelete_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedNvPreset();

            if (preset == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(preset.shortcut))
            {
                WinApi.UnregisterHotKey(_mainHandle, preset.id);
            }

            _nvService.GetPresets().Remove(preset);

            var item = lvNvPresets.SelectedItems[0];
            lvNvPresets.Items.Remove(item);
        }

        private void btnClone_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedNvPreset(true, true);

            if (preset == null)
            {
                return;
            }

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

            var checkState = miNvPresetDitheringEnabled.CheckState;
            var newCheckState = checkState == CheckState.Checked ? CheckState.Unchecked : checkState == CheckState.Unchecked ? CheckState.Indeterminate : CheckState.Checked;

            if (preset.IsDisplayPreset)
            {
                miNvPresetDitheringEnabled.CheckState = newCheckState;

                _nvService.SetDithering(newCheckState == CheckState.Indeterminate ? NvDitherState.Auto : newCheckState == CheckState.Checked ? NvDitherState.Enabled : NvDitherState.Disabled);

                UpdateDisplayInfoItems();
                return;
            }

            preset.ditheringEnabled = checkState == CheckState.Indeterminate ? null : checkState == CheckState.Checked;

            AddOrUpdateItem();
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            mnuNvPresets.ShowCustom(btnChange);
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
            mnuNvOtherSettings.Enabled = preset != null;
            miNvOtherIncluded.Visible = presetItemsVisible;
            mnuNvHdmiSettings.Enabled = preset != null;
            miNvHdmiIncluded.Visible = presetItemsVisible;

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

                    var item = mnuNvDisplay.DropDownItems.AddCustom(name);
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
            miNvPresetDitheringEnabled.CheckState = preset.ditheringEnabled == null ? CheckState.Indeterminate : preset.ditheringEnabled == true ? CheckState.Checked : CheckState.Unchecked;

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
            miNvOtherIncluded.Checked = preset.applyOther;
            miNvHdmiIncluded.Checked = preset.applyHdmiSettings;
            miNvDriverSettingsIncluded.Checked = preset.applyDriverSettings;

            mnuNvPresetsColorSettings.Font = _menuItemFonts[preset.applyColorData];
            mnuRefreshRate.Font = _menuItemFonts[preset.applyRefreshRate];
            mnuNvResolution.Font = _menuItemFonts[preset.applyResolution];
            miNvPresetDithering.Font = _menuItemFonts[preset.applyDithering];
            miNvHDR.Font = _menuItemFonts[preset.applyHDR];
            mnuNvOverclocking.Font = _menuItemFonts[preset.applyOverclocking];
            mnuNvOtherSettings.Font = _menuItemFonts[preset.applyOther];
            mnuNvHdmiSettings.Font = _menuItemFonts[preset.applyHdmiSettings];

            var itemName = $"{mnuNvOtherSettings.Name}_SDRBrightness";
            var brightnessMenu = FormUtils.BuildMenuItem(mnuNvOtherSettings.DropDownItems, itemName, "SDR brightness");

            var subItemName = $"{itemName}_SubItem";
            var subItem = FormUtils.BuildMenuItem(brightnessMenu.DropDownItems, subItemName, "", onClick: nvPresetBrightnessMenuItem_Click);
            subItem.Text = (preset.SDRBrightness.HasValue ? $"{preset.SDRBrightness.Value}%" : "Unset") + " (click to change)";

            FormUtils.BuildDropDownMenu(mnuNvOtherSettings, "Scaling", typeof(Scaling), preset, "scaling", nvPresetScalingMenuItem_Click, unchanged: true, skipValues: new object[] { Scaling.Customized })
                .DropDownItems[0].Visible = !isCurrentDisplay;

            FormUtils.BuildDropDownMenu(mnuNvHdmiSettings, "Content type", typeof(InfoFrameVideoContentType), preset.HdmiInfoFrameSettings, "ContentType", nvPresetHdmiContentTypeMenuItem_Click, unchanged: true)
                .DropDownItems[0].Visible = !isCurrentDisplay;
            FormUtils.BuildDropDownMenu(mnuNvHdmiSettings, "Colorimetry", typeof(InfoFrameVideoColorimetry), preset.HdmiInfoFrameSettings, "Colorimetry", nvPresetHdmiColorimetryMenuItem_Click, unchanged: true)
                .DropDownItems[0].Visible = !isCurrentDisplay;
            FormUtils.BuildDropDownMenu(mnuNvHdmiSettings, "Extended colorimetry", typeof(InfoFrameVideoExtendedColorimetry), preset.HdmiInfoFrameSettings, "ExtendedColorimetry", nvPresetHdmiExtendedColorimetryMenuItem_Click, unchanged: true)
                .DropDownItems[0].Visible = !isCurrentDisplay;

            BuildNvOverclockingMenu(preset);

            UpdateNvPresetMenuFont();

            BuildNvidiaDriverSettingsMenu(mnuNvDriverSettings, isCurrentDisplay ? null : preset.driverSettings);

            mnuNvDriverSettings.Font = _menuItemFonts[preset.applyDriverSettings];
        }

        private void BuildNvOverclockingMenu(NvPreset preset)
        {
            miNvOverclockingIncluded.Checked = preset.applyOverclocking;
            miNvOverclockingIncluded.Visible = !preset.IsDisplayPreset;

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

            if (preset == null)
            {
                btnNvSetClocks_Click(btnNvSetClocks, new EventArgs());

                return;
            }


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
                    var item = mnuRefreshRate.DropDownItems.AddCustom(refreshRate.ToString() + "Hz");
                    item.Tag = refreshRate;
                    item.Click += refreshRateMenuItem_Click;
                }
            }

            if (mnuNvResolution.DropDownItems.Count == 1)
            {
                var modes = _nvService.GetAvailableResolutions(preset);

                foreach (var mode in modes)
                {
                    var item = mnuNvResolution.DropDownItems.AddCustom($"{mode.dmPelsWidth}x{mode.dmPelsHeight}");
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
            var handledGroupItems = new Dictionary<ToolStripMenuItem, bool>();
            var showUnchanged = settings != null;
            var defaultValue = SettingMeta.UnsetDwordValue;

            foreach (var driverSetting in driverSettings)
            {
                var groupName = $"mnuNvDriverSettingGroup_{driverSetting.GroupName}";
                var groupItem = FormUtils.BuildMenuItem(contextMenuStrip?.Items ?? parent.DropDownItems, groupName, driverSetting.GroupName);

                var itemName = $"mnuNvDriverSettingItem_{driverSetting.SettingId}";
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

                if (!handledGroupItems.ContainsKey(groupItem))
                {
                    groupItem.Tag = null;
                    handledGroupItems.Add(groupItem, !isDefault);
                }
                else if (!isDefault)
                {
                    handledGroupItems[groupItem] = true;
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
                    var index = 0;

                    foreach (var settingValue in settingMeta.DwordValues)
                    {
                        var text = settingValue.ValueName;
                        if (text.IndexOf(" (") == 10)
                        {
                            if (settingMeta.DwordValues.Take(index).Any(v => v.Value == settingValue.Value))
                            {
                                continue;
                            }

                            text = text.Substring(0, 10);
                        }

                        var subItemName = $"mnuNvDriverSettingItem_{driverSetting.SettingId}_SubItem_{settingValue.Value}";
                        var subItem = FormUtils.BuildMenuItem(item.DropDownItems, subItemName, text, settingValue, driverSettingValueNvMenuItem_Click);

                        subItem.Checked = presetSetting == settingValue.Value;
                        subItem.Font = _menuItemFonts[subItem.Checked];

                        index++;
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

            foreach (var groupItemKeyValue in handledGroupItems)
            {
                groupItemKeyValue.Key.Font = _menuItemFonts[groupItemKeyValue.Value];
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

        private void nvPresetHdmiContentTypeMenuItem_Click(object sender, EventArgs e)
        {
            UpdateHdmiSettings<InfoFrameVideoContentType>(sender);
        }

        private void nvPresetHdmiColorimetryMenuItem_Click(object sender, EventArgs e)
        {
            UpdateHdmiSettings<InfoFrameVideoColorimetry>(sender);
        }

        private void nvPresetHdmiExtendedColorimetryMenuItem_Click(object sender, EventArgs e)
        {
            UpdateHdmiSettings<InfoFrameVideoExtendedColorimetry>(sender);
        }

        private void UpdateHdmiSettings<T>(object sender) where T : struct
        {
            var menuItem = (ToolStripMenuItem)sender;
            var propName = ((PropertyInfo)menuItem.OwnerItem.Tag).Name;
            var value = (T)menuItem.Tag;

            var preset = GetSelectedNvPreset(true);

            var settings = preset.IsDisplayPreset ? new NvHdmiInfoFrameSettings() : preset.HdmiInfoFrameSettings;
            var propInfo = settings.GetType().GetDeclaredProperty(propName);
            propInfo.SetValue(settings, value);

            if (preset.IsDisplayPreset)
            {
                _nvService.SetHdmiSettings(preset.Display, settings);

                UpdateDisplayInfoItems();
                return;
            }

            AddOrUpdateItem();
        }

        private void nvPresetBrightnessMenuItem_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedNvPreset(true);

            var label = "SDR brightness in %";
            if (!preset.IsDisplayPreset)
            {
                label += " (clear to set to unchanged)";
            }

            var value = SelectValue(0, 100, preset.SDRBrightness, label);

            if (value == -1)
            {
                return;
            }

            if (preset.IsDisplayPreset)
            {
                if (!value.HasValue)
                {
                    return;
                }

                _nvService.SetSDRBrightness(preset.Display, value.Value);

                UpdateDisplayInfoItems();
                return;
            }

            preset.SDRBrightness = value;

            AddOrUpdateItem();
        }

        private void nvPresetScalingMenuItem_Click(object sender, EventArgs e)
        {
            var menuItem = (ToolStripMenuItem)sender;
            var propName = ((PropertyInfo)menuItem.OwnerItem.Tag).Name;
            var value = (Scaling?)menuItem.Tag;

            var preset = GetSelectedNvPreset(true);

            var propInfo = preset.GetType().GetDeclaredProperty(propName);
            propInfo.SetValue(preset, value);

            if (preset.IsDisplayPreset)
            {
                _nvService.SetScaling(preset.Display, value ?? Scaling.Default);

                UpdateDisplayInfoItems();
                return;
            }

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

        private void btnAddModesNv_Click(object sender, EventArgs e)
        {
            var nameField = new FieldDefinition
            {
                Label = "Preset name",
                Value = _nvService.CreateNewPresetName()
            };


            var displays = _nvService.GetSimpleDisplayInfos();
            //var primaryDisplay = _nvService.GetPrimaryDisplay();
            displays.Insert(0, new NvDisplayInfo(null, null, null, "Primary display"));

            var displayField = FieldDefinition.CreateDropDownField("Display", displays.Select(d => d.Name).ToList());

            var sectionLabel = new FieldDefinition
            {
                Label = "Check the sections below you want to configure",
                FieldType = FieldType.Label
            };

            var colorSelectField = FieldDefinition.CreateCheckField("Color settings");
            var resolutionSelectField = FieldDefinition.CreateCheckField("Resolution");
            var refreshRateSelectField = FieldDefinition.CreateCheckField("Refresh rate");

            var result = MessageForms.ShowDialog("Create new NVIDIA preset", new[] { nameField, displayField, sectionLabel, colorSelectField, resolutionSelectField, refreshRateSelectField });

            if (!result.Any())
            {
                return;
            }

            var preset = new NvPreset();
            preset.name = nameField.Value.ToString();
            preset.primaryDisplay = displayField.Value.ToString() == "Primary display";
            preset.displayName = !preset.primaryDisplay ? displays.FirstOrDefault(d => d.ToString() == displayField.Value.ToString())?.Display?.Name : null;

            AskColorData(preset, colorSelectField.ValueAsBool);
            AskResolution(preset, resolutionSelectField.ValueAsBool);
            AskRefreshRate(preset, refreshRateSelectField.ValueAsBool);

            AddOrUpdateItem(preset);

            //var presets = NvPreset.GetDefaultPresets();
            //var added = false;

            //foreach (var preset in presets)
            //{
            //    if (!_nvService.GetPresets().Any(x => x.colorData.Equals(preset.colorData)))
            //    {
            //        AddOrUpdateItem(preset);
            //        added = true;
            //    }
            //}

            //if (added)
            //{
            //    MessageForms.InfoOk("Missing presets added.");
            //}
            //else
            //{
            //    MessageForms.InfoOk("All presets for every color setting already exist.");
            //}
        }

        private void AskColorData(NvPreset preset, bool ask = true)
        {
            preset.colorData = NvPreset.DefaultColorData;

            if (!ask)
            {
                preset.applyColorData = false;
                return;
            }

            var bitDepthField = FieldDefinition.CreateEnumField("Bit depth", ColorDataDepth.Default);
            var colorFormatField = FieldDefinition.CreateEnumField("Color format", ColorDataFormat.Auto);
            var dynamicRangeField = FieldDefinition.CreateEnumField("Dynamic range", ColorDataDynamicRange.Auto);
            var colorimetryField = FieldDefinition.CreateEnumField("Color space", ColorDataColorimetry.Auto);

            var result = MessageForms.ShowDialog("Color settings", new[] { bitDepthField, colorFormatField, dynamicRangeField, colorimetryField });

            if (!result.Any())
            {
                return;
            }

            preset.colorData = new ColorData(
                colorFormatField.ValueAsEnum<ColorDataFormat>(),
                colorimetryField.ValueAsEnum<ColorDataColorimetry>(),
                dynamicRange: dynamicRangeField.ValueAsEnum<ColorDataDynamicRange>(),
                colorDepth: bitDepthField.ValueAsEnum<ColorDataDepth>());

            preset.applyColorData = true;
        }

        private void AskResolution(NvPreset preset, bool ask = true)
        {
            if (!ask)
            {
                return;
            }

            var modes = _nvService.GetAvailableResolutions(preset).Select(d => $"{d.dmPelsWidth}x{d.dmPelsHeight}").ToList();

            var modesField = FieldDefinition.CreateDropDownField("Resolution", modes);

            var result = MessageForms.ShowDialog("Resolution", new[] { modesField });

            if (!result.Any())
            {
                return;
            }

            var values = modesField.Value.ToString().Split('x').Select(s => Utils.ParseUInt(s)).ToArray();

            preset.resolutionWidth = values[0];
            preset.resolutionHeight = values[1];

            preset.applyResolution = true;
        }

        private void AskRefreshRate(NvPreset preset, bool ask = true)
        {
            if (!ask)
            {
                return;
            }

            var modes = _nvService.GetAvailableRefreshRates(preset).Select(d => d.ToString()).ToList();

            var modesField = FieldDefinition.CreateDropDownField("Refresh rate", modes);

            var result = MessageForms.ShowDialog("Refresh rate", new[] { modesField });

            if (!result.Any())
            {
                return;
            }

            var value = modesField.ValueAsUInt;

            preset.refreshRate = value;
            preset.applyRefreshRate = true;
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

        private async Task ApplySelectedNvPreset()
        {
            var preset = GetSelectedNvPreset();
            await ApplyNvPreset(preset);
        }

        public async Task<bool> ApplyNvPreset(NvPreset preset)
        {
            if (preset == null || _nvService == null)
            {
                return false;
            }
            try
            {
                var result = await _nvService.ApplyPreset(preset);
                if (!result)
                {
                    throw new Exception("Error while applying NVIDIA preset. At least one setting could not be applied. Check the log for details.");
                }

                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e);

                MessageForms.ErrorOk($"Error applying NVIDIA-preset ({e.TargetSite.Name}): {e.Message}");
                return false;
            }
        }

        private NvPreset GetSelectedNvPreset(bool currentDisplay = false, bool driverSettings = false)
        {
            if (lvNvPresets.SelectedItems.Count > 0)
            {
                var item = lvNvPresets.SelectedItems[0];
                var result = (NvPreset)item.Tag;

                if (result == null && currentDisplay)
                {
                    result = _nvService.GetPresetForDisplay(item.SubItems[1].Text, driverSettings);
                }

                return result;
            }
            else
            {
                return null;
            }
        }

        private void edtShortcut_KeyDown(object sender, KeyEventArgs e)
        {
            ((TextBox)sender).Text = KeyboardShortcutManager.FormatKeyboardShortcut(e);
        }

        private void btnSetShortcut_Click(object sender, EventArgs e)
        {
            var shortcut = edtShortcut.Text.Trim();

            if (!KeyboardShortcutManager.ValidateShortcut(shortcut))
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

            preset.shortcut = shortcut;
            preset.name = name;
            preset.ShowInQuickAccess = chkNvShowInQuickAccess.Checked;

            AddOrUpdateItem();

            KeyboardShortcutManager.RegisterShortcut(preset.id, preset.shortcut);
        }

        private void edtShortcut_KeyUp(object sender, KeyEventArgs e)
        {
            KeyboardShortcutManager.HandleKeyboardShortcutUp(e);
        }

        private void edtShortcut_TextChanged(object sender, EventArgs e)
        {
            var text = edtShortcut.Text;

            var preset = GetSelectedNvPreset();

            ServiceFormUtils.UpdateShortcutTextBox(edtShortcut, preset);
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
            var field = new FieldDefinition
            {
                MinValue = settingMeta.DwordValues.First().Value,
                MaxValue = settingMeta.DwordValues.Last().Value,
                Label = settingMeta.SettingName,
                FieldType = FieldType.Numeric,
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

        private int? SelectValue(int min, int max, int? currentValue, string label)
        {
            var field = new FieldDefinition
            {
                MinValue = min,
                MaxValue = max,
                Label = label,
                FieldType = FieldType.Numeric,
                Value = currentValue
            };

            var resultFields = MessageForms.ShowDialog(field.Label, new[] { field });

            if (!resultFields.Any())
            {
                return -1;
            }

            var strValue = (string)resultFields.First().Value;

            if (string.IsNullOrWhiteSpace(strValue))
            {
                return null;
            }

            var settingValue = Utils.ParseInt(strValue);

            return settingValue;
        }

        private void miNvProfileInspector_Click(object sender, EventArgs e)
        {
            StartNvProfileInspector();
        }

        public void StartNvProfileInspector()
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
            var quickAccessField = new FieldDefinition
            {
                Label = "Quick Access shortcut",
                FieldType = FieldType.Shortcut,
                Value = _config.NvQuickAccessShortcut
            };

            var enableOcField = new FieldDefinition
            {
                Label = "Enable overclocking",
                FieldType = FieldType.CheckBox,
                Value = _nvService.Config.ShowOverclocking
            };

            var enableNovideoField = new FieldDefinition
            {
                Label = "Apply Novideo sRGB settings on startup",
                FieldType = FieldType.CheckBox,
                Value = _nvService.Config.ApplyNovideoOnStartup
            };

            var values = MessageForms.ShowDialog("NVIDIA controller settings", new[] {
                quickAccessField,
                enableOcField,
                enableNovideoField
            });
            if (!values.Any())
            {
                return;
            }

            var shortcut = (string)quickAccessField.Value;

            _config.NvQuickAccessShortcut = shortcut;

            KeyboardShortcutManager.RegisterShortcut(SHORTCUTID_NVQA, _config.NvQuickAccessShortcut);

            _nvService.Config.ShowOverclocking = enableOcField.ValueAsBool;
            _nvService.Config.ApplyNovideoOnStartup = enableNovideoField.ValueAsBool;

            UpdateInfo();
        }

        private void btnNvSettings_Click(object sender, EventArgs e)
        {
            mnuNvSettings.ShowCustom(btnNvSettings);
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

            _nvService.ApplyOverclocking(new List<NvGpuOcSettings> { newSettings });

            UpdateNvGpuInfo();
        }

        public NvGpuOcSettings AskNvOcSettings(NvGpuInfo gpuInfo, NvGpuOcSettings currentSettings)
        {
            var overclockTypes = gpuInfo.SupportedOcTypes.Select(t => Utils.GetDescription(t));

            var values = MessageForms.ShowDialog($"Overclock settings - {gpuInfo.GPU.FullName}", new[] {
                    new FieldDefinition
                    {
                        Label = "Overclock method",
                        FieldType = FieldType.DropDown,
                        Values = overclockTypes,
                        Value = currentSettings.Type
                    } }, okButtonText: "Next >");
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
            else if (overclockType == NvGpuOcType.BoostLock)
            {
                newSettings = AskNvBoostLockSettings(gpuInfo, currentSettings);
            }
            else
            {
                newSettings = new NvGpuOcSettings
                {
                    Type = overclockType,
                    PCIIdentifier = gpuInfo.PCIIdentifier
                };
            }

            return newSettings;
        }

        public NvGpuOcSettings AskNvCurveOcSettings(NvGpuInfo gpuInfo, NvGpuOcSettings currentSettings)
        {
            var nvOverclockSettings = new NvGpuOcSettings
            {
                Type = NvGpuOcType.Curve,
                PCIIdentifier = gpuInfo.PCIIdentifier
            };

            var coreOffsetField = gpuInfo.GetCoreOffsetField(currentSettings);
            var memoryOffsetField = gpuInfo.GetMemoryOffsetField(currentSettings);

            var (voltField, freqField) = gpuInfo.GetCurveVoltFreqFields(currentSettings);

            var voltageBoostField = gpuInfo.GetVoltageBoostField(currentSettings);
            var powerField = gpuInfo.GetPowerField(currentSettings);

            var values = MessageForms.ShowDialog($"Curve overclock settings - {gpuInfo.GPU.FullName}", new[]
            {
                coreOffsetField, voltField, freqField, memoryOffsetField, voltageBoostField, powerField,
            });
            if (!values.Any())
            {
                return null;
            }

            nvOverclockSettings.GraphicsOffsetKHz = coreOffsetField.ValueAsInt * 1000;
            nvOverclockSettings.MaximumVoltageUv = voltField.ValueAsUInt * 1000;
            nvOverclockSettings.MaximumFrequencyKHz = freqField.ValueAsUInt * 1000;
            nvOverclockSettings.MemoryOffsetKHz = memoryOffsetField.ValueAsInt * 1000;
            nvOverclockSettings.VoltageBoostPercent = voltageBoostField.ValueAsUInt;
            nvOverclockSettings.PowerPCM = gpuInfo.PowerToPCM(powerField.ValueAsUInt, powerField.FieldType == FieldType.TrackBar);
            nvOverclockSettings.FrequencyPreferred = nvOverclockSettings.MaximumFrequencyKHz > 0 && nvOverclockSettings.MaximumVoltageUv == 0;

            return nvOverclockSettings;
        }

        public NvGpuOcSettings AskNvOffsetOcSettings(NvGpuInfo gpuInfo, NvGpuOcSettings currentSettings)
        {
            var nvOverclockSettings = new NvGpuOcSettings
            {
                Type = NvGpuOcType.Offset,
                PCIIdentifier = gpuInfo.PCIIdentifier
            };

            var coreOffsetField = gpuInfo.GetCoreOffsetField(currentSettings);
            var memoryOffsetField = gpuInfo.GetMemoryOffsetField(currentSettings);

            var voltageBoostField = gpuInfo.GetVoltageBoostField(currentSettings);
            var powerField = gpuInfo.GetPowerField(currentSettings);

            var values = MessageForms.ShowDialog($"Offset overclock settings - {gpuInfo.GPU.FullName}", new[] {
                    coreOffsetField,
                    memoryOffsetField,
                    voltageBoostField,
                    powerField,
            });
            if (!values.Any())
            {
                return null;
            }

            nvOverclockSettings.GraphicsOffsetKHz = coreOffsetField.ValueAsInt * 1000;
            nvOverclockSettings.MemoryOffsetKHz = memoryOffsetField.ValueAsInt * 1000;
            nvOverclockSettings.VoltageBoostPercent = voltageBoostField.ValueAsUInt;
            nvOverclockSettings.PowerPCM = gpuInfo.PowerToPCM(powerField.ValueAsUInt, powerField.FieldType == FieldType.TrackBar);
            nvOverclockSettings.FrequencyPreferred = nvOverclockSettings.MaximumFrequencyKHz > 0 && nvOverclockSettings.MaximumVoltageUv == 0;

            return nvOverclockSettings;
        }

        public NvGpuOcSettings AskNvBoostLockSettings(NvGpuInfo gpuInfo, NvGpuOcSettings currentSettings)
        {
            var nvOverclockSettings = new NvGpuOcSettings
            {
                Type = NvGpuOcType.BoostLock,
                PCIIdentifier = gpuInfo.PCIIdentifier
            };

            var curveEntries = gpuInfo.CurveV3.GPUCurveEntries;

            var currentGraphicsOffset = gpuInfo.GetOffset(NvAPIWrapper.Native.GPU.PublicClockDomain.Graphics);

            var coreOffsetField = gpuInfo.GetCoreOffsetField(currentSettings);
            var memoryOffsetField = gpuInfo.GetMemoryOffsetField(currentSettings);

            var (voltField, freqField) = gpuInfo.GetCurveVoltFreqFields(currentSettings);
            voltField.Label = "Locked voltage in mV";
            freqField.Label = "Locked boost frequency in MHz";

            var voltageBoostField = gpuInfo.GetVoltageBoostField(currentSettings);
            var powerField = gpuInfo.GetPowerField(currentSettings);

            var values = MessageForms.ShowDialog($"Boost lock overclock settings - {gpuInfo.GPU.FullName}", new[] {
                voltField,
                freqField,
                memoryOffsetField,
                voltageBoostField,
                powerField
            });
            if (!values.Any())
            {
                return null;
            }

            var newVolt = voltField.ValueAsUInt * 1000;
            var newFreq = freqField.ValueAsUInt * 1000;

            var newCurveEntryFreq = curveEntries.FirstOrDefault(e => e.DefaultFrequencyInkHz == newFreq);
            var newCurveEntryVolt = curveEntries.FirstOrDefault(e => e.DefaultVoltageInMicroV == newVolt);

            var coreOffset = (int)(newCurveEntryFreq.DefaultFrequencyInkHz - newCurveEntryVolt.DefaultFrequencyInkHz);

            nvOverclockSettings.GraphicsOffsetKHz = coreOffset;
            nvOverclockSettings.MaximumFrequencyKHz = newFreq;
            nvOverclockSettings.MaximumVoltageUv = newVolt;
            nvOverclockSettings.MemoryOffsetKHz = memoryOffsetField.ValueAsInt * 1000;
            nvOverclockSettings.VoltageBoostPercent = voltageBoostField.ValueAsUInt;
            nvOverclockSettings.PowerPCM = gpuInfo.PowerToPCM(powerField.ValueAsUInt, powerField.FieldType == FieldType.TrackBar);

            return nvOverclockSettings;
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

        private void miNvCopyId_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedNvPreset();

            if (preset != null)
            {
                Clipboard.SetText(preset.id.ToString());
            }
        }

        private void miNvPresetApplyOnStartup_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedNvPreset();
            _config.NvPresetId_ApplyOnStartup = miNvPresetApplyOnStartup.Checked ? preset.id : 0;

            AddOrUpdateItem();
        }

        private async Task ApplyNvPresetOnStartup(int attempts = 5)
        {
            var startUpParams = Shared.Common.GlobalContext.CurrentContext.StartUpParams;

            var presetIdOrName = !string.IsNullOrEmpty(startUpParams.NvidiaPresetIdOrName) ? startUpParams.NvidiaPresetIdOrName : _config.NvPresetId_ApplyOnStartup.ToString();

            if (!string.IsNullOrEmpty(presetIdOrName))
            {
                var preset = _nvService?.GetPresetByIdOrName(presetIdOrName);
                if (preset == null)
                {
                    if (string.IsNullOrEmpty(startUpParams.NvidiaPresetIdOrName))
                    {
                        _config.NvPresetId_ApplyOnStartup = 0;
                    }
                }
                else if (_nvService != null)
                {
                    if (_nvService.HasDisplaysAttached())
                    {
                        await ApplyNvPreset(preset);
                        return;
                    }
                    attempts--;
                    if (attempts > 0)
                    {
                        await Task.Delay(2000);
                        await ApplyNvPresetOnStartup(attempts);
                    }
                }
            }
        }

        private void miNvDithering6bit_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;

            var preset = GetSelectedNvPreset(true);

            var ditheringBits = uint.Parse(item.Tag.ToString());

            if (preset.IsDisplayPreset)
            {
                _nvService.SetDithering(preset.DitherState, ditheringBits, preset.ditheringMode);

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
                _nvService.SetDithering(preset.DitherState, preset.ditheringBits, ditheringMode);

                UpdateDisplayInfoItems();
                return;
            }

            preset.ditheringMode = ditheringMode;

            AddOrUpdateItem();
        }

        public void AfterInitialized()
        {
            var _ = ApplyNvPresetOnStartup();
        }

        internal void Save()
        {
            FormUtils.SaveSortState(lvNvPresets.ListViewItemSorter, _config.NvPresetsSortState);
        }

        public void UpdateInfo()
        {
            UpdateDisplayInfoItems();
        }

        private void miNvNovideo_Click(object sender, EventArgs e)
        {
            MainWindow.CreateAndShow();
        }

        private void miNvOtherIncluded_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedNvPreset();

            preset.applyOther = !preset.applyOther;

            AddOrUpdateItem();
        }

        private void miNvHdmiIncluded_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedNvPreset();

            preset.applyHdmiSettings = !preset.applyHdmiSettings;

            AddOrUpdateItem();
        }

        private void miNvTestDithering_Click(object sender, EventArgs e)
        {
            var panel = new NvDitherPanel(_nvService);

            MessageForms.ShowControl(panel, "Test dithering");
        }

        private void miNVIDIAInfo_Click(object sender, EventArgs e)
        {
            var panel = new NvInfoPanel(_nvService);

            MessageForms.ShowControl(panel, "NVIDIA Info", height: 800);
        }
    }
}
