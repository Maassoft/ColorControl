using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts;
using ColorControl.Shared.Forms;
using ColorControl.Shared.Services;
using System.Diagnostics;
using System.Windows.Forms;

namespace ColorControl.Services.Common;

public class ElevationService
{
    private readonly WinApiService _winApiService;
    private readonly WinApiAdminService _winApiAdminService;
    private readonly GlobalContext _globalContext;

    private readonly Config _config;

    public ElevationService(WinApiService winApiService, WinApiAdminService winApiAdminService, GlobalContext globalContext)
    {
        _winApiService = winApiService;
        _winApiAdminService = winApiAdminService;
        _globalContext = globalContext;

        _config = globalContext.Config;
    }

    public void SetElevationMethod(ElevationMethod elevationMethod, bool startAfterLoginChecked = false)
    {
        _config.ElevationMethod = elevationMethod;

        var _ = _config.ElevationMethod switch
        {
            ElevationMethod.None => SetElevationMethodNone(startAfterLoginChecked),
            ElevationMethod.RunAsAdmin => SetElevationMethodRunAsAdmin(startAfterLoginChecked),
            ElevationMethod.UseService => SetElevationMethodUseService(),
            ElevationMethod.UseElevatedProcess => SetElevationMethodUseElevatedProcess(startAfterLoginChecked),
            _ => true
        };
    }

    public void RegisterScheduledTask(bool enabled = true)
    {
        _winApiAdminService.RegisterTask(Program.TS_TASKNAME, enabled, _config.ElevationMethod == ElevationMethod.RunAsAdmin ? Microsoft.Win32.TaskScheduler.TaskRunLevel.Highest : Microsoft.Win32.TaskScheduler.TaskRunLevel.LUA);
    }

    public void CheckElevationMethod()
    {
        if (_winApiService.IsAdministrator())
        {
            return;
        }

        if (_config.ElevationMethodAsked)
        {
            if (Debugger.IsAttached)
            {
                return;
            }

            if (_config.ElevationMethod == ElevationMethod.UseService && !_winApiService.IsServiceInstalled() &&
                (MessageForms.QuestionYesNo("The elevation method is set to Windows Service but it is not installed. Do you want to install it now?") == DialogResult.Yes))
            {
                _winApiAdminService.InstallService();
            }
            else if (_config.ElevationMethod == ElevationMethod.UseService && !_winApiService.IsServiceRunning() &&
                (MessageForms.QuestionYesNo("The elevation method is set to Windows Service but it is not running. Do you want to start it now?") == DialogResult.Yes))
            {
                _winApiAdminService.StartService();
            }

            return;
        }

        var result = MessageForms.QuestionYesNo(Utils.ELEVATION_MSG + @"

Do you want to install and start the Windows Service now? You can always change this on the Options-tab page.

NOTE: installing the service may cause a User Account Control popup.");
        if (result == DialogResult.Yes)
        {
            SetElevationMethod(ElevationMethod.UseService);
        }

        _config.ElevationMethodAsked = true;
    }

    private bool SetElevationMethodNone(bool startAfterLoginChecked = false)
    {
        if (startAfterLoginChecked)
        {
            RegisterScheduledTask();
        }

        _winApiAdminService.UninstallService();

        return true;
    }

    private bool SetElevationMethodRunAsAdmin(bool startAfterLoginChecked = false)
    {
        if (startAfterLoginChecked)
        {
            RegisterScheduledTask(false);
            RegisterScheduledTask();
        }

        _winApiAdminService.UninstallService();

        return true;
    }

    private bool SetElevationMethodUseService()
    {
        _winApiAdminService.InstallService();

        if (_winApiService.IsServiceInstalled())
        {
            MessageForms.InfoOk("Service installed successfully.");
            return true;
        }

        MessageForms.ErrorOk("Service could not be installed. Check the logs.");

        return false;
    }

    private bool SetElevationMethodUseElevatedProcess(bool startAfterLoginChecked = false)
    {
        if (startAfterLoginChecked)
        {
            RegisterScheduledTask();
        }

        _winApiAdminService.UninstallService();

        return true;
    }
}
