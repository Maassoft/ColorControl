using ColorControl.Properties;
using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts;
using ColorControl.Shared.Contracts.AMD;
using ColorControl.Shared.Contracts.Game;
using ColorControl.Shared.Contracts.LG;
using ColorControl.Shared.Contracts.NVIDIA;
using ColorControl.Shared.Contracts.Samsung;
using ColorControl.Shared.Forms;
using ColorControl.Shared.Services;
using novideo_srgb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ColorControl.Services.Common;

public class NotifyIconManager
{
	private readonly GlobalContext _globalContext;
	private readonly ServiceManager _serviceManager;
	private readonly WinApiAdminService _winApiAdminService;
	private readonly WinApiService _winApiService;
	private readonly Config _config;

	public NotifyIcon NotifyIcon { get; private set; }

	private ToolStripMenuItem _nvTrayMenu;
	private ToolStripMenuItem _novideoTrayMenu;
	private ToolStripMenuItem _amdTrayMenu;
	private ToolStripMenuItem _lgTrayMenu;
	private ToolStripMenuItem _samsungTrayMenu;
	private ToolStripMenuItem _gameTrayMenu;

	private ToolStripMenuItem _stopServiceAndExitMenuItem;
	private ToolStripMenuItem _openWinFormsMenuItem;
	private ToolStripMenuItem _openNewUiMenuItem;
	private ToolStripMenuItem _openNewUiEmbeddedMenuItem;

	public NotifyIconManager(GlobalContext globalContext, ServiceManager serviceManager, WinApiAdminService winApiAdminService, WinApiService winApiService)
	{
		_globalContext = globalContext;
		_serviceManager = serviceManager;
		_winApiAdminService = winApiAdminService;
		_winApiService = winApiService;
		_config = globalContext.Config;
	}

	public void Build()
	{
		if (_nvTrayMenu != null)
		{
			return;
		}

		_nvTrayMenu = new ToolStripMenuItem("NVIDIA presets");
		_novideoTrayMenu = new ToolStripMenuItem("Novideo sRGB");
		_amdTrayMenu = new ToolStripMenuItem("AMD presets");
		_lgTrayMenu = new ToolStripMenuItem("LG presets");
		_samsungTrayMenu = new ToolStripMenuItem("Samsung presets");
		_gameTrayMenu = new ToolStripMenuItem("Game Launcher");
		NotifyIcon = new NotifyIcon()
		{
			Icon = Resources.AppIcon,
			ContextMenuStrip = new ContextMenuStrip(),
			Visible = _config.MinimizeToTray
		};

		SetText();

		NotifyIcon.ContextMenuStrip.Items.AddRange(new ToolStripItem[] {
					_nvTrayMenu,
					_novideoTrayMenu,
					_amdTrayMenu,
					_lgTrayMenu,
					_samsungTrayMenu,
					_gameTrayMenu,
					new ToolStripSeparator(),
					new ToolStripMenuItem("Open", null, OpenForm),
					_openWinFormsMenuItem = new ToolStripMenuItem("Open WinForms UI", null, OpenWinFormsUi),
					_openNewUiMenuItem = new ToolStripMenuItem("Open web UI (browser)", null, OpenNewUi),
					_openNewUiEmbeddedMenuItem = new ToolStripMenuItem("Open web UI (embedded)", null, OpenNewUiEmbedded),
					new ToolStripSeparator(),
					new ToolStripMenuItem("Restart", null, Restart),
					_stopServiceAndExitMenuItem = new ToolStripMenuItem("Stop Service And Exit", null, StopServiceAndExit),
					new ToolStripMenuItem("Exit", null, Exit)
				});

		NotifyIcon.MouseDoubleClick += trayIcon_MouseDoubleClick;
		NotifyIcon.ContextMenuStrip.Opened += trayIconContextMenu_Popup;
	}

	public void HideIcon()
	{
		if (NotifyIcon != null)
		{
			NotifyIcon.Visible = false;
		}
	}

	private void StopServiceAndExit(object sender, EventArgs e)
	{
		_winApiAdminService.StopService();

		Program.Exit();
	}

	private void Exit(object sender, EventArgs e)
	{
		Program.Exit();
	}

	private void Restart(object sender, EventArgs e)
	{
		Program.Restart();
	}

	private void OpenForm(object sender, EventArgs e)
	{
		Program.OpenDefaultUi();
	}

	private void OpenWinFormsUi(object sender, EventArgs e)
	{
		Program.OpenMainForm();
	}

	private async void OpenNewUi(object sender, EventArgs e)
	{
		await Program.OpenNewUi();
	}

	private async void OpenNewUiEmbedded(object sender, EventArgs e)
	{
		await Program.OpenNewUiEmbedded();
	}

	private void trayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
	{
		Program.OpenDefaultUi();
	}

	private void trayIconContextMenu_Popup(object sender, EventArgs e)
	{
		_nvTrayMenu.Visible = _serviceManager.NvService != null;
		if (_nvTrayMenu.Visible)
		{
			var presets = _serviceManager.NvService.GetPresets().Where(x => x.applyColorData || x.applyDithering || x.applyHDR || x.DisplayConfig.ApplyRefreshRate || x.DisplayConfig.ApplyResolution || x.applyDriverSettings || x.applyOther || x.applyOverclocking || x.ApplyColorEnhancements || x.applyHdmiSettings);

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
			var presets = _serviceManager.AmdService.GetPresets().Where(x => x.applyColorData || x.applyDithering || x.applyHDR || x.DisplayConfig.ApplyRefreshRate || x.DisplayConfig.ApplyResolution);

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

		_stopServiceAndExitMenuItem.Visible = _winApiService.IsServiceRunning();
	}

	private void UpdateTrayMenu(ToolStripMenuItem menu, IEnumerable<PresetBase> presets, EventHandler eventHandler)
	{
		menu.DropDownItems.Clear();

		foreach (var preset in presets.OrderBy(p => p.name))
		{
			var name = preset.GetTextForMenuItem();
			var keys = Keys.None;

			if (!string.IsNullOrEmpty(preset.shortcut))
			{
				name += "        " + preset.shortcut;
				//keys = KeyboardShortcutDispatcher.ShortcutToKeys(preset.shortcut);
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

		await _serviceManager.NvService?.ApplyPresetUi(preset);
	}

	private async void TrayMenuItemAmd_Click(object sender, EventArgs e)
	{
		var item = sender as ToolStripMenuItem;
		var preset = (AmdPreset)item.Tag;

		await _serviceManager.AmdService?.ApplyPresetUi(preset);
	}

	private async void TrayMenuItemGame_Click(object sender, EventArgs e)
	{
		var item = sender as ToolStripMenuItem;
		var preset = (GamePreset)item.Tag;

		await _serviceManager.GameService?.ApplyPresetUi(preset);
	}

	private async void TrayMenuItemLg_Click(object sender, EventArgs e)
	{
		var item = sender as ToolStripMenuItem;
		var preset = (LgPreset)item.Tag;

		await _serviceManager.LgService?.ApplyPresetUi(preset);
	}

	private async void TrayMenuItemSamsung_Click(object sender, EventArgs e)
	{
		var item = sender as ToolStripMenuItem;
		var preset = (SamsungPreset)item.Tag;

		await _serviceManager.SamsungService?.ApplyPresetUi(preset);
	}

	internal void SetText(string text = null)
	{
		var fullText = $"{_globalContext.ApplicationTitleAdmin}{(text == null ? "" : $"\n{text}")}";

		FormUtils.SetNotifyIconText(NotifyIcon, fullText.Trim());
	}
}
