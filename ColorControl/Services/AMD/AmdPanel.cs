using ATI.ADL;
using ColorControl.Services.Common;
using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts;
using ColorControl.Shared.Contracts.AMD;
using ColorControl.Shared.EventDispatcher;
using ColorControl.Shared.Forms;
using ColorControl.Shared.Native;
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
	partial class AmdPanel : UserControl, IModulePanel
	{
		private readonly AmdService _amdService;
		private readonly NotifyIconManager _notifyIconManager;
		private readonly GlobalContext _globalContext;
		private readonly KeyboardShortcutDispatcher _keyboardShortcutDispatcher;
		private Config _config;
		private string _lastDisplayRefreshRates = string.Empty;

		public AmdPanel(AmdService amdService, NotifyIconManager notifyIconManager, GlobalContext globalContext, KeyboardShortcutDispatcher keyboardShortcutDispatcher)
		{
			_amdService = amdService;
			_notifyIconManager = notifyIconManager;
			_globalContext = globalContext;
			_keyboardShortcutDispatcher = keyboardShortcutDispatcher;
			_config = globalContext.Config;

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
					item.BackColor = _globalContext.Config.UseDarkMode ? DarkModeUtils.ListViewDarkModeBackColor : Color.LightGray;
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

			_notifyIconManager.SetText(text);
		}

		private void AddOrUpdateItemAmd(AmdPreset preset = null)
		{
			ServiceFormUtils.AddOrUpdateListItem(lvAmdPresets, _amdService.GetPresets(), _config, preset);
		}

		public async Task AfterInitialized()
		{
			await ApplyAmdPresetOnStartup();
		}

		private async Task ApplyAmdPresetOnStartup(int attempts = 5)
		{
			var startUpParams = Shared.Common.GlobalContext.CurrentContext.StartUpParams;
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
						await _amdService.ApplyPresetUi(preset);
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
			((TextBox)sender).Text = KeyboardShortcutDispatcher.FormatKeyboardShortcut(e);
		}

		private void edtShortcut_KeyUp(object sender, KeyEventArgs e)
		{
			KeyboardShortcutDispatcher.HandleKeyboardShortcutUp(e);
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
			var refreshRate = (Rational)((ToolStripItem)sender).Tag;

			var preset = GetSelectedAmdPreset();

			preset.DisplayConfig.RefreshRate = refreshRate;

			AddOrUpdateItemAmd();
		}

		private void resolutionAmdMenuItem_Click(object sender, EventArgs e)
		{
			var mode = (VirtualResolution)((ToolStripItem)sender).Tag;

			var preset = GetSelectedAmdPreset();

			preset.DisplayConfig.Resolution.ActiveWidth = mode.ActiveWidth;
			preset.DisplayConfig.Resolution.ActiveHeight = mode.ActiveHeight;

			AddOrUpdateItemAmd();
		}

		private void virtualResolutionAmdMenuItem_Click(object sender, EventArgs e)
		{
			var mode = (VirtualResolution)((ToolStripItem)sender).Tag;

			var preset = GetSelectedAmdPreset();

			var virtualMode = new VirtualResolution(mode);
			virtualMode.ActiveWidth = preset.DisplayConfig.Resolution.ActiveWidth;
			virtualMode.ActiveHeight = preset.DisplayConfig.Resolution.ActiveHeight;

			preset.DisplayConfig.Resolution.VirtualWidth = virtualMode.VirtualWidth;
			preset.DisplayConfig.Resolution.VirtualHeight = virtualMode.VirtualHeight;

			AddOrUpdateItemAmd();
		}

		private void UpdateDisplayConfigScaling(object sender, EventArgs e)
		{
			var menuItem = (ToolStripMenuItem)sender;
			var value = (CCD.DisplayConfigScaling)menuItem.Tag;

			var preset = GetSelectedAmdPreset();

			preset.DisplayConfig.Scaling = value;

			AddOrUpdateItemAmd();
		}

		private void UpdateDisplayConfigRotation(object sender, EventArgs e)
		{
			var menuItem = (ToolStripMenuItem)sender;
			var value = (CCD.DisplayConfigRotation)menuItem.Tag;

			var preset = GetSelectedAmdPreset();
			var settings = preset.DisplayConfig;

			preset.DisplayConfig.Rotation = value;

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
			await _amdService.ApplyPresetUi(preset);
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

			ServiceFormUtils.UpdateShortcutTextBox(edtAmdShortcut, preset);
		}

		private void btnSetAmdShortcut_Click(object sender, EventArgs e)
		{
			var shortcut = edtAmdShortcut.Text.Trim();

			if (!KeyboardShortcutDispatcher.ValidateShortcut(shortcut))
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

			preset.shortcut = shortcut;
			preset.name = name;
			preset.ShowInQuickAccess = chkAmdQuickAccess.Checked;

			AddOrUpdateItemAmd();

			_keyboardShortcutDispatcher.RegisterShortcut(preset.id, preset.shortcut);
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
				WinApi.UnregisterHotKey(_globalContext.MainHandle, preset.id);
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
			mnuAmdResolution.Enabled = preset != null;
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
					miAmdActiveResolution.DropDownItems.Clear();
					miAmdVirtualResolution.DropDownItems.Clear();
				}

				if (mnuAmdRefreshRate.DropDownItems.Count == 1)
				{
					var refreshRates = _amdService.GetAvailableRefreshRates(preset);
					_lastDisplayRefreshRates = preset.displayName;

					foreach (var refreshRate in refreshRates)
					{
						var item = mnuAmdRefreshRate.DropDownItems.AddCustom(refreshRate + "Hz");
						item.Tag = refreshRate;
						item.Click += refreshRateMenuItemAmd_Click;
					}
				}

				var modes = default(List<VirtualResolution>);

				if (miAmdActiveResolution.DropDownItems.Count == 0)
				{
					modes = _amdService.GetAvailableResolutionsV2(preset);

					foreach (var mode in modes)
					{
						var item = miAmdActiveResolution.DropDownItems.AddCustom(mode.ToString());
						item.Tag = mode;
						item.Click += resolutionAmdMenuItem_Click;
					}
				}

				if (miAmdVirtualResolution.DropDownItems.Count == 0)
				{
					modes ??= _amdService.GetAvailableResolutionsV2(preset);

					foreach (var mode in modes)
					{
						var item = miAmdVirtualResolution.DropDownItems.AddCustom(mode.ToString());
						item.Tag = mode;
						item.Click += virtualResolutionAmdMenuItem_Click;
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

				miAmdRefreshRateIncluded.Checked = preset.DisplayConfig.ApplyRefreshRate;
				foreach (var item in mnuAmdRefreshRate.DropDownItems)
				{
					if (item is ToolStripMenuItem)
					{
						var menuItem = (ToolStripMenuItem)item;
						if (menuItem.Tag != null)
						{
							menuItem.Checked = ((Rational)menuItem.Tag).Equals(preset.DisplayConfig.RefreshRate);
						}
					}
				}

				miAmdResolutionIncluded.Checked = preset.DisplayConfig.ApplyResolution;
				foreach (var item in miAmdActiveResolution.DropDownItems)
				{
					if (item is ToolStripMenuItem menuItem && menuItem.Tag != null)
					{
						var mode = (VirtualResolution)menuItem.Tag;
						menuItem.Checked = mode.ActiveWidth == preset.DisplayConfig.Resolution.ActiveWidth && mode.ActiveHeight == preset.DisplayConfig.Resolution.ActiveHeight;
					}
				}

				foreach (var item in miAmdVirtualResolution.DropDownItems)
				{
					if (item is ToolStripMenuItem menuItem && menuItem.Tag != null)
					{
						var mode = (VirtualResolution)menuItem.Tag;
						menuItem.Checked = mode.VirtualWidth == preset.DisplayConfig.Resolution.VirtualWidth && mode.VirtualHeight == preset.DisplayConfig.Resolution.VirtualHeight;
					}
				}

				miAmdDitheringIncluded.Checked = preset.applyDithering;

				FormUtils.BuildDropDownMenu(mnuAmdDithering, "Mode", typeof(ADLDitherState), preset, "ditherState", amdPresetColorDataMenuItem_Click);

				miAmdHDRIncluded.Checked = preset.applyHDR;
				miAmdHDREnabled.Checked = preset.HDREnabled;
				miAmdHDRToggle.Checked = preset.toggleHDR;

				FormUtils.BuildDropDownMenu(mnuAmdResolution, "Scaling/Aspect Ratio", typeof(CCD.DisplayConfigScaling), preset.DisplayConfig, "Scaling", UpdateDisplayConfigScaling, unchanged: false,
					skipValues: [CCD.DisplayConfigScaling.Zero, CCD.DisplayConfigScaling.Custom, CCD.DisplayConfigScaling.ForceUint32]);
				FormUtils.BuildDropDownMenu(mnuAmdResolution, "Rotation", typeof(CCD.DisplayConfigRotation), preset.DisplayConfig, "Rotation", UpdateDisplayConfigRotation, unchanged: false,
					skipValues: [CCD.DisplayConfigRotation.Zero, CCD.DisplayConfigRotation.ForceUint32]);
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

			preset.DisplayConfig.ApplyRefreshRate = !preset.DisplayConfig.ApplyRefreshRate;

			AddOrUpdateItemAmd();
		}

		private void miAmdResolutionIncluded_Click(object sender, EventArgs e)
		{
			var preset = GetSelectedAmdPreset();

			preset.DisplayConfig.ApplyResolution = !preset.DisplayConfig.ApplyResolution;

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
			ServiceFormUtils.ListViewItemChecked<AmdPreset>(lvAmdPresets, e);
		}

		private void btnAmdSettings_Click(object sender, EventArgs e)
		{
			var shortcut = FormUtils.EditShortcut(_config.AmdQuickAccessShortcut, "Quick Access shortcut", "AMD controller settings");

			if (shortcut == null)
			{
				return;
			}

			_config.AmdQuickAccessShortcut = shortcut;

			_keyboardShortcutDispatcher.RegisterShortcut(AmdService.SHORTCUTID_AMDQA, _config.AmdQuickAccessShortcut);
		}

		private void lvLgPresets_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			FormUtils.ListViewSort(sender, e);
		}

		public void Init()
		{
		}
	}
}
