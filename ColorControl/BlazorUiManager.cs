using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts;
using ColorControl.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ColorControl;

public static class BlazorUiManager
{
    private const string ImageName = "ColorControl.UI.exe";
    private const string ProcessName = "ColorControl.UI";
    private const bool DebugUi = true;

    private static bool FirsTime = true;
    private static Mutex UiMutex;
    private static string MutexId;

    public static bool IsRunning(Config config)
    {
        return Process.GetProcessesByName(ProcessName).Length > 0 &&
            (config.UiPort == 0 || config.CurrentUiPort == config.UiPort) &&
            config.CurrentUiAllowRemoteConnections == config.UiAllowRemoteConnections;
    }

    public static async Task<bool> Start(Config config)
    {
        if ((!FirsTime || Debugger.IsAttached && DebugUi) && IsRunning(config))
        {
            return false;
        }

        FirsTime = false;

        await Stop();

        var uiExe = Path.Combine(Environment.CurrentDirectory, ImageName);
        if (!File.Exists(uiExe))
        {
            uiExe = Path.Combine(Environment.CurrentDirectory.Replace("ColorControl\\bin", "ColorControl.UI\\bin"), ImageName);

            if (!File.Exists(uiExe))
            {
                throw new InvalidOperationException("UI executable not found");
            }
        }

        MutexId = $"Global\\{ProcessName}_{Guid.NewGuid()}";
        Utils.ExecuteOnMainThread(() =>
        {
            UiMutex = new Mutex(false, MutexId);
            UiMutex.WaitOne();
        });

        var windowStyle = ProcessWindowStyle.Hidden;

        if (Debugger.IsAttached)
        {
            windowStyle = ProcessWindowStyle.Normal;
        }

        var processStartInfo = new ProcessStartInfo(uiExe, [MutexId, config.UseDarkMode.ToString(), config.UiPort.ToString(), config.UiAllowRemoteConnections.ToString()])
        {
            WindowStyle = windowStyle
        };

        Process.Start(processStartInfo);

        return true;
    }

    public static async Task Stop()
    {
        if (UiMutex != null)
        {
            Utils.ExecuteOnMainThread(() =>
            {
                UiMutex.ReleaseMutex();
                UiMutex.Close();
                UiMutex = null;
            });
            return;
        }

        var processes = Process.GetProcessesByName(ProcessName);

        if (processes.Length <= 0)
        {
            return;
        }

        var winApi = Program.ServiceProvider.GetRequiredService<WinApiAdminService>();
        winApi.StopProcessByName(ProcessName);
    }

    public static string GetCurrentUrl(Config config)
    {
        var port = GetCurrentPort(config);

        return $"http://localhost:{port}";
    }

    public static int GetCurrentPort(Config config = null)
    {
        return config?.CurrentUiPort ?? -1;
    }
}