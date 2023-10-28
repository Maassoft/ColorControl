using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using System.Security.Principal;
using System.ServiceProcess;

namespace ColorControl.Shared.Services;

public class WinApiService
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    public static string SERVICE_NAME = "Color Control Service";

    private static bool? _IsAdministrator;

    public WinApiService()
    {
    }

    public static bool IsAdministratorStatic()
    {
        if (_IsAdministrator.HasValue)
        {
            return _IsAdministrator.Value;
        }

        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        _IsAdministrator = principal.IsInRole(WindowsBuiltInRole.Administrator);

        return _IsAdministrator.Value;
    }

    public bool IsAdministrator()
    {
        return IsAdministratorStatic();
    }

    public bool IsServiceInstalled()
    {
        return GetServiceStatus() != 0;
    }

    public bool IsServiceRunning()
    {
        return GetServiceStatus() == ServiceControllerStatus.Running;
    }

    public bool IsChromeInstalled()
    {
        var key = Registry.ClassesRoot.OpenSubKey(@"ChromeHTML\shell\open\command");
        return key != null;
    }

    public bool IsChromeFixInstalled()
    {
        var key = Registry.ClassesRoot.OpenSubKey(@"ChromeHTML\shell\open\command");
        return key != null && key.GetValue(null).ToString().Contains("--disable-lcd-text");
    }

    public bool TaskExists(string taskName)
    {
        using (TaskService ts = new TaskService())
        {
            var task = ts.RootFolder.Tasks.FirstOrDefault(x => x.Name.Equals(taskName));

            return task != null;
        }
    }

    private ServiceControllerStatus GetServiceStatus()
    {
        var controller = new ServiceController(SERVICE_NAME);

        try
        {
            return controller.Status;
        }
        catch (Exception)
        {
            return 0;
        }
    }
}
