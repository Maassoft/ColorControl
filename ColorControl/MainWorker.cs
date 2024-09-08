﻿using ColorControl.Services.Common;
using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts;
using ColorControl.Shared.EventDispatcher;
using ColorControl.Shared.Forms;
using ColorControl.Shared.Native;
using ColorControl.Shared.Services;
using NLog;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ColorControl;

internal class MainWorker
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private readonly BackgroundWorker _backgroundWorker;
    private readonly GlobalContext _globalContext;
    private readonly ServiceManager _serviceManager;
    private readonly Config _config;
    private readonly NotifyIconManager _notifyIconManager;
    private readonly WindowMessageDispatcher _windowMessageDispatcher;
    private readonly KeyboardShortcutDispatcher _keyboardShortcutDispatcher;
    private readonly PowerEventDispatcher _powerEventDispatcher;
    private readonly WinApiService _winApiService;
    private readonly WinApiAdminService _winApiAdminService;
    private readonly UpdateManager _updateManager;
    private nint _screenStateNotify;

    public static int SHORTCUTID_SCREENSAVER = -100;

    public MainWorker(
        GlobalContext globalContext,
        ServiceManager serviceManager,
        NotifyIconManager notifyIconManager,
        WindowMessageDispatcher windowMessageDispatcher,
        KeyboardShortcutDispatcher keyboardShortcutDispatcher,
        PowerEventDispatcher powerEventDispatcher,
        WinApiService winApiService,
        WinApiAdminService winApiAdminService,
        UpdateManager updateManager)
    {
        _backgroundWorker = new BackgroundWorker();
        _globalContext = globalContext;
        _serviceManager = serviceManager;
        _notifyIconManager = notifyIconManager;
        _windowMessageDispatcher = windowMessageDispatcher;
        _keyboardShortcutDispatcher = keyboardShortcutDispatcher;
        _powerEventDispatcher = powerEventDispatcher;
        _winApiService = winApiService;
        _winApiAdminService = winApiAdminService;
        _updateManager = updateManager;
        _config = _globalContext.Config;
    }

    public async Task DoWork()
    {
        if (_backgroundWorker.CancellationPending)
        {
            return;
        }

        await Init();

        System.Windows.Forms.Application.Run(_windowMessageDispatcher.MessageForm);

        _serviceManager.Save();
        _notifyIconManager.HideIcon();

        if (_screenStateNotify != 0)
        {
            WinApi.UnregisterPowerSettingNotification(_screenStateNotify);
            _screenStateNotify = 0;
        }
    }

    private async Task Init()
    {
        _globalContext.SynchronizationContext = AsyncOperationManager.SynchronizationContext;

        _windowMessageDispatcher.RegisterEventHandler(WindowMessageDispatcher.Event_WindowMessageQueryEndSession, HandleQueryEndSessionEvent);
        _windowMessageDispatcher.RegisterEventHandler(WindowMessageDispatcher.Event_WindowMessagePowerBroadcast, HandlePowerBroadcastEvent);
        _windowMessageDispatcher.RegisterEventHandler(WindowMessageDispatcher.Event_WindowMessageShowWindow, HandleShowWindowEvent);
        _windowMessageDispatcher.RegisterEventHandler(WindowMessageDispatcher.Event_WindowMessageUserBringToFront, HandleUserBringToFrontEvent);
        _windowMessageDispatcher.RegisterEventHandler(WindowMessageDispatcher.Event_WindowMessageDisplayChange, HandleDisplayChangeEvent);

        _screenStateNotify = WinApi.RegisterPowerSettingNotification(_windowMessageDispatcher.MessageForm.Handle, ref Utils.GUID_CONSOLE_DISPLAY_STATE, 0);

        _keyboardShortcutDispatcher.SetUseRawInput(_config.UseRawInput);
        MessageForms.KeyboardShortcutDispatcher = _keyboardShortcutDispatcher;

        _notifyIconManager.Build();

        _serviceManager.LoadModules();

        // Auto-apply fix if desired but not currently installed
        if (_config.FixChromeFonts && _winApiService.IsChromeInstalled() && !_winApiService.IsChromeFixInstalled())
        {
            _winApiAdminService.InstallChromeFix(true, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
        }

        InitTheme();

        _keyboardShortcutDispatcher.RegisterShortcut(SHORTCUTID_SCREENSAVER, _config.ScreenSaverShortcut);
        _keyboardShortcutDispatcher.RegisterEventHandler(KeyboardShortcutDispatcher.Event_HotKey, HandleHotKeyEvent);

        if (_config.UiType != UiType.WinForms)
        {
            await Program.StartUiServer();
        }
    }

    private bool _startupSent = false;

    private void HandleShowWindowEvent(object sender, WindowMessageEventArgs e)
    {
        if (_startupSent)
        {
            return;
        }

        _startupSent = true;

        _ = _powerEventDispatcher.SendEventAsync(PowerEventDispatcher.Event_Startup);

        if (_config.CheckForUpdates)
        {
            _ = _updateManager.CheckForUpdatesAndInstall();
        }

        if (!_config.StartMinimized /*|| Debugger.IsAttached*/)
        {
            Program.OpenDefaultUi();
        }
    }

    private void HandleUserBringToFrontEvent(object sender, WindowMessageEventArgs e)
    {
        Program.OpenDefaultUi();
    }

    private void HandlePowerBroadcastEvent(object sender, WindowMessageEventArgs e)
    {
        if (e.Message.WParam.ToInt32() == WinApi.PBT_POWERSETTINGCHANGE)
        {
            var ps = Marshal.PtrToStructure<WinApi.POWERBROADCAST_SETTING>(e.Message.LParam);

            var power = (WindowsMonitorPowerSetting)ps.Data;

            Logger.Debug($"PBT_POWERSETTINGCHANGE: {power}");

            _powerEventDispatcher.SendEvent(power == WindowsMonitorPowerSetting.Off ? PowerEventDispatcher.Event_MonitorOff : PowerEventDispatcher.Event_MonitorOn);
        }
    }

    private void HandleQueryEndSessionEvent(object sender, WindowMessageEventArgs e)
    {
        Logger.Debug($"MainWorker: QueryEndSession");
        Logger.Debug($"MainWorker: SystemShutdown");

        _serviceManager.Save();

        _powerEventDispatcher.SendEvent(PowerEventDispatcher.Event_Shutdown);
    }

    private void HandleHotKeyEvent(object sender, KeyboardShortcutEventArgs args)
    {
        if (args.HotKeyId == SHORTCUTID_SCREENSAVER)
        {
            StartScreenSaver();
        }
    }
    
    private void HandleDisplayChangeEvent(object sender, WindowMessageEventArgs e)
    {
    }

    private void StartScreenSaver()
    {
        var screenSaver = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "scrnsave.scr");
        Process.Start("explorer.exe", screenSaver);
    }

    private void InitTheme()
    {
        DarkModeUtils.SetDarkMode(_config.UseDarkMode);

        WinApi.RefreshImmersiveColorPolicyState();

        DarkModeUtils.InitWpfTheme();

        DarkModeUtils.SetContextMenuForeColor(_notifyIconManager.NotifyIcon.ContextMenuStrip, FormUtils.CurrentForeColor);
    }
}
