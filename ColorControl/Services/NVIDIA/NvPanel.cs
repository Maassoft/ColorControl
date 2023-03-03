﻿using ColorControl.Common;
using ColorControl.Forms;
using ColorControl.Native;
using NLog;
using nspector;
using nspector.Common;
using nspector.Common.Meta;
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
    partial class NvPanel : UserControl
    {
        private static int SHORTCUTID_NVQA = -200;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private Config _config;
        private NvService _nvService;
        private IntPtr _mainHandle;
        private string _lastDisplayRefreshRates = string.Empty;
        private NotifyIcon _trayIcon;

        public NvPanel(NvService nvService, NotifyIcon trayIcon, IntPtr handle)
        {
            _nvService = nvService;
            _trayIcon = trayIcon;
            _mainHandle = handle;

            _config = AppContext.CurrentContext.Config;

            InitializeComponent();

            FillNvPresets();

            FormUtils.InitSortState(lvNvPresets, _config.NvPresetsSortState);

            _nvService.AfterApplyPreset += NvServiceAfterApplyPreset;
        }

        private void RemoteControlPanel_Load(object sender, EventArgs e)
        {
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
                Utils.RegisterShortcut(_mainHandle, preset.id, preset.shortcut);
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

        private void AddOrUpdateItem(NvPreset preset = null)
        {
            FormUtils.AddOrUpdateListItem(lvNvPresets, _nvService.GetPresets(), _config, preset);
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            ApplySelectedNvPreset();
        }

        private void lvNvPresets_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            FormUtils.ListViewItemChecked<NvPreset>(lvNvPresets, e);
        }

        private void lvNvPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            var preset = GetSelectedNvPreset();
            var enabled = preset != null;

            FormUtils.EnableControls(this, enabled, new List<Control> { lvNvPresets, btnAddModesNv, btnChange, edtNvGpuInfo, btnNvSetClocks, btnNvSettings, edtNvOverclock });

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

        private void ApplySelectedNvPreset()
        {
            var preset = GetSelectedNvPreset();
            ApplyNvPreset(preset);
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

            Utils.RegisterShortcut(_mainHandle, preset.id, preset.shortcut, clear);
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
                //edtShortcut.ForeColor = ShortCutExists(text, preset.id) ? Color.Red : SystemColors.WindowText;
            }
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

            Utils.RegisterShortcut(_mainHandle, SHORTCUTID_NVQA, _config.NvQuickAccessShortcut, clear);

            _nvService.Config.ShowOverclocking = enableOc;
        }

        private void btnNvSettings_Click(object sender, EventArgs e)
        {
            mnuNvSettings.Show(btnNvSettings, btnNvSettings.PointToClient(Cursor.Position));
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

        private void ApplyNvPresetOnStartup(int attempts = 5)
        {
            var startUpParams = AppContext.CurrentContext.StartUpParams;

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

        public void AfterInitialized()
        {
            ApplyNvPresetOnStartup();
        }

        internal void Save()
        {
            FormUtils.SaveSortState(lvNvPresets.ListViewItemSorter, _config.NvPresetsSortState);
        }

        internal void UpdateInfo()
        {
            UpdateDisplayInfoItems();
        }
    }
}
