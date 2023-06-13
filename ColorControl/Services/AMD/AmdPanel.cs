using ATI.ADL;
using ColorControl.Common;
using ColorControl.Forms;
using ColorControl.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorControl.Services.AMD
{
    public partial class AmdPanel : UserControl, IModulePanel
    {
        public static readonly int SHORTCUTID_AMDQA = -201;

        private AmdService _amdService;
        private NotifyIcon _trayIcon;
        private IntPtr _mainHandle;
        private Config _config;
        private string _lastDisplayRefreshRates = string.Empty;

        internal AmdPanel(AmdService amdService, NotifyIcon trayIcon, IntPtr handle)
        {
            _amdService = amdService;
            _trayIcon = trayIcon;
            _mainHandle = handle;

            _config = AppContext.CurrentContext.Config;

            InitializeComponent();

            FillAmdPresets();

            FormUtils.InitSortState(lvAmdPresets, _config.AmdPresetsSortState);

            _amdService.AfterApplyPreset += AmdServiceAfterApplyPreset;
        }

        private void AmdServiceAfterApplyPreset(object sender, AmdPreset preset)
        {
            UpdateDisplayInfoItemsAmd();
        }

        private void FillAmdPresets()
        {
            FormUtils.InitListView(lvAmdPresets, AmdPreset.GetColumnNames());

            UpdateDisplayInfoItemsAmd();

            foreach (var preset in _amdService.GetPresets())
            {
                AddOrUpdateItemAmd(preset);
                Utils.RegisterShortcut(_mainHandle, preset.id, preset.shortcut);
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

        private void AddOrUpdateItemAmd(AmdPreset preset = null)
        {
            FormUtils.AddOrUpdateListItem(lvAmdPresets, _amdService.GetPresets(), _config, preset);
        }

        public void AfterInitialized()
        {
            var _ = ApplyAmdPresetOnStartup();
        }

        private async Task ApplyAmdPresetOnStartup(int attempts = 5)
        {
            var startUpParams = AppContext.CurrentContext.StartUpParams;
            var presetIdOrName = !string.IsNullOrEmpty(startUpParams.AmdPresetIdOrName) ? startUpParams.AmdPresetIdOrName : _config.AmdPresetId_ApplyOnStartup.ToString();

            if (!string.IsNullOrEmpty(presetIdOrName))
            {
                var preset = _amdService?.GetPresetByIdOrName(presetIdOrName);
                if (preset == null)
                {
                    if (string.IsNullOrEmpty(startUpParams.AmdPresetIdOrName))
                    {
                        _config.AmdPresetId_ApplyOnStartup = 0;
                    }
                }
                else if (_amdService != null)
                {
                    if (_amdService.HasDisplaysAttached())
                    {
                        await ApplyAmdPreset(preset);
                    }
                    else
                    {
                        attempts--;
                        if (attempts > 0)
                        {
                            await Task.Delay(2000);
                            await ApplyAmdPresetOnStartup(attempts);
                        }
                    }
                }
            }
        }

        internal void Save()
        {
            FormUtils.SaveSortState(lvAmdPresets.ListViewItemSorter, _config.AmdPresetsSortState);
        }

        public void UpdateInfo()
        {
            UpdateDisplayInfoItemsAmd();
        }

        private void edtShortcut_KeyDown(object sender, KeyEventArgs e)
        {
            ((TextBox)sender).Text = Utils.FormatKeyboardShortcut(e);
        }

        private void edtShortcut_KeyUp(object sender, KeyEventArgs e)
        {
            Utils.HandleKeyboardShortcutUp(e);
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

        private void refreshRateMenuItemAmd_Click(object sender, EventArgs e)
        {
            var refreshRate = (uint)((ToolStripItem)sender).Tag;

            var preset = GetSelectedAmdPreset();

            preset.refreshRate = refreshRate;

            AddOrUpdateItemAmd();
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

        private async Task ApplySelectedAmdPreset()
        {
            var preset = GetSelectedAmdPreset();
            await ApplyAmdPreset(preset);
        }

        internal async Task<bool> ApplyAmdPreset(AmdPreset preset)
        {
            if (preset == null || _amdService == null)
            {
                return false;
            }
            try
            {
                var result = await _amdService.ApplyPreset(preset);
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

        private async void btnApplyAmd_Click(object sender, EventArgs e)
        {
            await ApplySelectedAmdPreset();
        }

        private void btnChangeAmd_Click(object sender, EventArgs e)
        {
            mnuAmdPresets.ShowCustom(btnChangeAmd);
        }

        private void edtAmdShortcut_TextChanged(object sender, EventArgs e)
        {
            var text = edtAmdShortcut.Text;

            var preset = GetSelectedAmdPreset();

            FormUtils.UpdateShortcutTextBox(edtAmdShortcut, preset);
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

            Utils.RegisterShortcut(_mainHandle, preset.id, preset.shortcut, clear);
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
                WinApi.UnregisterHotKey(_mainHandle, preset.id);
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

                        var item = mnuAmdDisplay.DropDownItems.AddCustom(name);
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
                        var item = mnuAmdRefreshRate.DropDownItems.AddCustom(refreshRate.ToString() + "Hz");
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

            FormUtils.EnableControls(this, enabled, new List<Control> { lvAmdPresets, btnAddAmd });

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
            //var added = false;

            foreach (var preset in presets)
            {
                AddOrUpdateItemAmd(preset);
            }

            //if (added)
            //{
            //    MessageForms.InfoOk("Missing presets added.");
            //}
            //else
            //{
            //    MessageForms.InfoOk("All presets for every color setting already exist.");
            //}
        }

        private void miAmdCopyId_Click(object sender, EventArgs e)
        {
            var preset = GetSelectedAmdPreset();

            if (preset != null)
            {
                Clipboard.SetText(preset.id.ToString());
            }
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
            FormUtils.ListViewItemChecked<AmdPreset>(lvAmdPresets, e);
        }

        private void btnAmdSettings_Click(object sender, EventArgs e)
        {
            var shortcut = FormUtils.EditShortcut(_config.AmdQuickAccessShortcut, "Quick Access shortcut", "AMD controller settings");

            if (shortcut == null)
            {
                return;
            }

            var clear = !string.IsNullOrEmpty(_config.AmdQuickAccessShortcut);

            _config.AmdQuickAccessShortcut = shortcut;

            Utils.RegisterShortcut(_mainHandle, SHORTCUTID_AMDQA, _config.AmdQuickAccessShortcut, clear);
        }

        private void lvLgPresets_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            FormUtils.ListViewSort(sender, e);
        }
    }
}
