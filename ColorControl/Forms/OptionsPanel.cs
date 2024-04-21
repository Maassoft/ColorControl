using ColorControl.Services.Common;
using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts;
using ColorControl.Shared.EventDispatcher;
using ColorControl.Shared.Forms;
using ColorControl.Shared.Services;
using ColorControl.XForms;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorControl.Forms;

public partial class OptionsPanel : UserControl
{
    private readonly WinApiService _winApiService;
    private readonly WinApiAdminService _winApiAdminService;
    private readonly GlobalContext _globalContext;
    private readonly ElevationService _elevationService;
    private readonly KeyboardShortcutDispatcher _keyboardShortcutDispatcher;
    private bool _initialized;
    private Config _config;

    public OptionsPanel(WinApiService winApiService, WinApiAdminService winApiAdminService, GlobalContext globalContext, ElevationService elevationService, KeyboardShortcutDispatcher keyboardShortcutDispatcher)
    {
        _winApiService = winApiService;
        _winApiAdminService = winApiAdminService;
        _globalContext = globalContext;
        _elevationService = elevationService;
        _keyboardShortcutDispatcher = keyboardShortcutDispatcher;
        _config = globalContext.Config;

        InitializeComponent();

        if (DarkModeUtils.UseDarkMode)
        {
            for (var i = 0; i < Controls.Count; i++)
            {
                var control = Controls[i];

                DarkModeUtils.SetControlTheme(control);
            }
        }

        Init();

        UpdateServiceInfo();
    }

    private void Init()
    {
        if (chkModules.Items.Count == 0)
        {
            foreach (var module in _config.Modules)
            {
                chkModules.Items.Add(module.DisplayName, module.IsActive);
            }
        }

        chkStartMinimized.Checked = _config.StartMinimized;
        chkMinimizeOnClose.Checked = _config.MinimizeOnClose;
        chkMinimizeToSystemTray.Checked = _config.MinimizeToTray;
        chkCheckForUpdates.Checked = _config.CheckForUpdates;
        chkAutoInstallUpdates.Checked = _config.AutoInstallUpdates;
        chkAutoInstallUpdates.Enabled = _config.CheckForUpdates && _config.ElevationMethod == ElevationMethod.UseService;
        edtBlankScreenSaverShortcut.Text = _config.ScreenSaverShortcut;
        chkGdiScaling.Checked = _config.UseGdiScaling;
        chkOptionsUseDarkMode.Checked = _config.UseDarkMode;

        chkStartAfterLogin.Checked = _winApiService.TaskExists(Program.TS_TASKNAME);

        chkFixChromeFonts.Enabled = _winApiService.IsChromeInstalled();
        if (chkFixChromeFonts.Enabled)
        {
            chkFixChromeFonts.Checked = _winApiService.IsChromeFixInstalled();
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
    }

    private void UpdateServiceInfo()
    {
        var text = "Use Windows Service";
        btnStartStopService.Enabled = true;

        if (_winApiService.IsServiceRunning())
        {
            text += " (running)";
            btnStartStopService.Text = "Stop";
        }
        else if (_winApiService.IsServiceInstalled())
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

    private void edtShortcut_KeyDown(object sender, KeyEventArgs e)
    {
        ((TextBox)sender).Text = _keyboardShortcutDispatcher.FormatKeyboardShortcut(e);
    }

    private void edtShortcut_KeyUp(object sender, KeyEventArgs e)
    {
        _keyboardShortcutDispatcher.HandleKeyboardShortcutUp(e);
    }

    private void btnSetShortcutScreenSaver_Click(object sender, EventArgs e)
    {
        var shortcut = edtBlankScreenSaverShortcut.Text.Trim();

        if (!KeyboardShortcutDispatcher.ValidateShortcut(shortcut))
        {
            return;
        }

        _config.ScreenSaverShortcut = shortcut;

        _keyboardShortcutDispatcher.RegisterShortcut(MainWorker.SHORTCUTID_SCREENSAVER, shortcut);
    }

    private void chkFixChromeFonts_CheckedChanged(object sender, EventArgs e)
    {
        if (_initialized)
        {
            _config.FixChromeFonts = chkFixChromeFonts.Checked;

            var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            _winApiAdminService.InstallChromeFix(chkFixChromeFonts.Checked, folder);
        }
    }

    private void chkAutoInstallUpdates_CheckedChanged(object sender, EventArgs e)
    {
        if (_initialized)
        {
            _config.AutoInstallUpdates = chkAutoInstallUpdates.Checked;
        }
    }

    private void chkMinimizeToSystemTray_CheckedChanged(object sender, EventArgs e)
    {
        if (_initialized)
        {
            _config.StartMinimized = chkStartMinimized.Checked;
            Program.GetNotifyIcon().Visible = _config.MinimizeToTray;
        }
    }

    private void chkCheckForUpdates_CheckedChanged(object sender, EventArgs e)
    {
        if (_initialized)
        {
            _config.CheckForUpdates = chkCheckForUpdates.Checked;
            chkAutoInstallUpdates.Enabled = _config.CheckForUpdates && _config.ElevationMethod == ElevationMethod.UseService;
        }
    }

    private void chkGdiScaling_CheckedChanged(object sender, EventArgs e)
    {
        _config.UseGdiScaling = chkGdiScaling.Checked;
    }

    private void chkStartAfterLogin_CheckedChanged(object sender, EventArgs e)
    {
        if (!_initialized)
        {
            return;
        }
        var enabled = chkStartAfterLogin.Checked;
        _elevationService.RegisterScheduledTask(enabled);

        rbElevationAdmin.Enabled = enabled;
        if (!enabled && rbElevationAdmin.Checked)
        {
            rbElevationNone.Checked = true;
        }
    }

    private void btnElevationInfo_Click(object sender, EventArgs e)
    {
        MessageForms.InfoOk(Utils.ELEVATION_MSG + @$"

NOTE: if ColorControl itself already runs as administrator this is indicated in the title: ColorControl {Application.ProductVersion} (administrator).

Currently ColorControl is {(_winApiService.IsAdministrator() ? "" : "not ")}running as administrator.
");
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

        _elevationService.SetElevationMethod((ElevationMethod)Utils.ParseInt((string)button.Tag), chkStartAfterLogin.Checked);

        UpdateServiceInfo();
        chkAutoInstallUpdates.Enabled = _config.CheckForUpdates && _config.ElevationMethod == ElevationMethod.UseService;
    }

    private async void btnStartStopService_Click(object sender, EventArgs e)
    {
        btnStartStopService.Enabled = false;
        try
        {
            var start = btnStartStopService.Text == "Start";

            if (start)
            {
                _winApiAdminService.StartService();
            }
            else
            {
                _winApiAdminService.StopService();
            }

            var wait = 1500;

            while (wait > 0)
            {
                await Task.Delay(100);

                if (start == _winApiService.IsServiceRunning())
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

    private void chkMinimizeOnClose_CheckedChanged(object sender, EventArgs e)
    {
        _config.MinimizeOnClose = chkMinimizeOnClose.Checked;
    }

    private void chkOptionsUseDarkMode_CheckedChanged(object sender, EventArgs e)
    {
        if (!_initialized)
        {
            return;
        }

        _config.UseDarkMode = chkOptionsUseDarkMode.Checked;

        Program.SetTheme(_config.UseDarkMode);
    }

    private void btnOptionsAdvanced_Click(object sender, EventArgs e)
    {
        var processPollingIntervalField = new FieldDefinition
        {
            FieldType = FieldType.Numeric,
            Label = "Polling interval of process monitor (milliseconds).",
            SubLabel = "Decreasing this delay may execute triggered presets sooner but can cause a higher CPU load",
            MinValue = 50,
            MaxValue = 5000,
            Value = _config.ProcessMonitorPollingInterval
        };
        var useRawInputField = new FieldDefinition
        {
            FieldType = FieldType.CheckBox,
            Label = "Use Raw Input for shortcuts (hot keys)",
            SubLabel = "This enables shortcuts to work during applications/games that block certain keys (like WinKey or Control). NOTE: if the application in the foreground runs with higher privileges than ColorControl, Raw Input does not work and normal hot keys are used",
            Value = _config.UseRawInput
        };
        var setMinTmlAndMaxTmlField = new FieldDefinition
        {
            FieldType = FieldType.CheckBox,
            Label = "Set MinTML and MaxTML when applying color profiles",
            SubLabel = "When this is enabled MinTML and MaxTML will be automatically be set to respectively the minimum luminance and the maximum luminance of the color profile",
            Value = _config.SetMinTmlAndMaxTml
        };
        var disableErrorPopupField = new FieldDefinition
        {
            FieldType = FieldType.CheckBox,
            Label = "Disable error popup when applying presets",
            SubLabel = "Check this to disable the error popup when there was an error applying a preset",
            Value = _config.DisableErrorPopupWhenApplyingPreset
        };

        var values = MessageForms.ShowDialog("Advanced settings", new[] { processPollingIntervalField, useRawInputField, setMinTmlAndMaxTmlField, disableErrorPopupField });

        if (values?.Any() != true)
        {
            return;
        }

        _config.ProcessMonitorPollingInterval = processPollingIntervalField.ValueAsInt;
        _config.UseRawInput = useRawInputField.ValueAsBool;
        _config.SetMinTmlAndMaxTml = setMinTmlAndMaxTmlField.ValueAsBool;
        _config.DisableErrorPopupWhenApplyingPreset = disableErrorPopupField.ValueAsBool;

        _keyboardShortcutDispatcher.SetUseRawInput(_config.UseRawInput);
    }

    private void btnOptionsLog_Click(object sender, EventArgs e)
    {
        LogWindow.CreateAndShow();
    }

    private void chkModules_ItemCheck(object sender, ItemCheckEventArgs e)
    {
        if (chkModules.Items.Count != _config.Modules.Count)
        {
            return;
        }
        var index = e.Index;

        if (index < 0)
        {
            return;
        }

        var module = _config.Modules[index];
        module.IsActive = e.NewValue == CheckState.Checked;
    }

    private void miCreateHDRColorProfile_Click(object sender, EventArgs e)
    {
        ColorProfileWindow.CreateAndShow();
    }

    private void btnOptionsColorProfiles_Click(object sender, EventArgs e)
    {
        mnuColorProfiles.ShowCustom(btnOptionsColorProfiles);
    }

    private void miCreateSDRColorProfile_Click(object sender, EventArgs e)
    {
        ColorProfileWindow.CreateAndShow(isHDR: false);
    }

    private void chkStartMinimized_CheckedChanged(object sender, EventArgs e)
    {
        if (_initialized)
        {
            _config.StartMinimized = chkStartMinimized.Checked;
        }
    }
}
