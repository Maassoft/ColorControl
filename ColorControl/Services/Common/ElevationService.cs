using ColorControl.Shared.Contracts;
using ColorControl.Shared.Forms;
using ColorControl.Shared.Services;

namespace ColorControl.Services.Common;

public class ElevationService
{
    private readonly WinApiService _winApiService;
    private readonly WinApiAdminService _winApiAdminService;
    private readonly AppContextProvider _appContextProvider;

    private readonly Config _config;

    public ElevationService(WinApiService winApiService, WinApiAdminService winApiAdminService, AppContextProvider appContextProvider)
    {
        _winApiService = winApiService;
        _winApiAdminService = winApiAdminService;
        _appContextProvider = appContextProvider;

        _config = appContextProvider.GetAppContext().Config;
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
