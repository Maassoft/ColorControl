using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts;
using NWin32;
using System.Diagnostics;

namespace ColorControl.Shared.Services;

public class WinElevatedProcessManager
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    private bool UseDedicatedElevatedProcess = true;

    public WinElevatedProcessManager()
    {
    }

    public void ExecuteElevated(string method, params object[] arguments)
    {
        ExecuteElevated<object>(method, arguments);
    }

    public T ExecuteElevated<T>(string method, params object[] arguments)
    {
        CheckElevatedProcess();

        var rpcMessage = new SvcRpcMessage
        {
            ServiceName = nameof(WinApiAdminService),
            MethodName = method,
            Arguments = arguments
        };

        return PipeUtils.SendRpcMessage<T>(rpcMessage, pipeName: PipeUtils.ElevatedPipe);
    }

    public T ExecuteElevated<T>(SvcRpcMessage message)
    {
        CheckElevatedProcess();

        return PipeUtils.SendRpcMessage<T>(message, pipeName: PipeUtils.ElevatedPipe);
    }

    private int ExecuteDirect(string args, bool wait = true)
    {
        var fileName = Process.GetCurrentProcess().MainModule.FileName;

        var info = new ProcessStartInfo(fileName, args)
        {
            Verb = "runas", // indicates to elevate privileges
            UseShellExecute = true,
        };

        var process = new Process
        {
            EnableRaisingEvents = true, // enable WaitForExit()
            StartInfo = info
        };
        try
        {
            process.Start();

            if (wait)
            {
                process.WaitForExit(); // sleep calling process thread until evoked process exit
            }

            return process.Id;
        }
        catch (Exception e)
        {
            Logger.Error("ExecuteElevated: " + e.Message);
        }
        return 0;
    }


    private static nint ElevatedProcessWindowHandle = nint.Zero;

    public void CheckElevatedProcess()
    {
        ElevatedProcessWindowHandle = NativeMethods.FindWindowW(null, "ElevatedForm");

        if (ElevatedProcessWindowHandle != nint.Zero)
        {
            return;
        }

        ExecuteDirect(StartUpParams.StartElevatedParam, false);

        var delay = 1000;

        while (delay >= 0)
        {
            Thread.Sleep(100);

            ElevatedProcessWindowHandle = NativeMethods.FindWindowW(null, "ElevatedForm");

            if (ElevatedProcessWindowHandle != nint.Zero)
            {
                Thread.Sleep(500);
                break;
            }

            delay -= 100;
        }
    }

}
