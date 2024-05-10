using ColorControl.Shared.Contracts;
using ColorControl.Shared.Native;
using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using NWin32;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.ServiceProcess;

namespace ColorControl.Shared.Services
{
    public class WinApiAdminService
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public static string SERVICE_NAME = "Color Control Service";
        private readonly WinApiService _winApiService;
        private RpcClientService _rpcService;
        private readonly WinElevatedProcessManager _elevatedProcessManager;
        private bool? _IsAdministrator;

        public WinApiAdminService(WinApiService winApiService, RpcClientService rpcService, WinElevatedProcessManager elevatedProcessManager)
        {
            _winApiService = winApiService;
            _rpcService = rpcService;
            _elevatedProcessManager = elevatedProcessManager;
            _rpcService.Name = nameof(WinApiAdminService);
        }

        public bool SuspendThread(uint threadId)
        {
            if (!_winApiService.IsAdministrator())
            {
                return _rpcService.Call<bool>(nameof(SuspendThread), threadId);
            }

            var handle = NativeMethods.OpenThread(NativeConstants.THREAD_ALL_ACCESS, false, threadId);

            try
            {
                var result = NativeMethods.SuspendThread(handle);

                return ProcessResult(result);
            }
            finally
            {
                NativeMethods.CloseHandle(handle);
            }
        }

        private int GetProcessMainThread(Process process)
        {
            var minStartTime = DateTime.MaxValue;
            var mainThread = default(ProcessThread);

            for (var i = 0; i < process.Threads.Count; i++)
            {
                var thread = process.Threads[i];
                try
                {
                    if (thread.StartTime < minStartTime)
                    {
                        mainThread = thread;
                        minStartTime = thread.StartTime;
                    }
                }
                catch { }
            }

            return mainThread.Id;
        }

        public int SuspendMainThread(int processId)
        {
            if (!_winApiService.IsAdministrator())
            {
                return _rpcService.Call<int>(nameof(SuspendMainThread), processId);
            }

            var process = Process.GetProcessById(processId);
            var threadId = GetProcessMainThread(process);

            if (SuspendThread((uint)threadId))
            {
                return threadId;
            }

            return 0;
        }

        public bool ResumeThread(uint threadId)
        {
            if (!_winApiService.IsAdministrator())
            {
                return _rpcService.Call<bool>(nameof(ResumeThread), threadId);
            }

            var handle = NativeMethods.OpenThread(NativeConstants.THREAD_ALL_ACCESS, false, threadId);

            try
            {
                var result = NativeMethods.ResumeThread(handle);

                return ProcessResult(result);
            }
            finally
            {
                NativeMethods.CloseHandle(handle);
            }
        }

        public Process StartProcess(string fileName, string arguments = null, bool hidden = false, bool wait = false, bool setWorkingDir = false, bool elevate = false, uint affinityMask = 0, uint priorityClass = 0, bool? useShellExecute = null)
        {
            var process = Process.Start(new ProcessStartInfo(fileName, arguments)
            {
                Verb = elevate ? "runas" : string.Empty, // indicates to elevate privileges
                UseShellExecute = useShellExecute.HasValue ? useShellExecute.Value : !hidden || !(fileName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".bat", StringComparison.OrdinalIgnoreCase)),
                WindowStyle = hidden ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal,
                WorkingDirectory = setWorkingDir ? Path.GetDirectoryName(fileName) : null
            });

            if (process == null)
            {
                Logger.Debug($"Process {fileName} could not be started: null returned");
                return null;
            }

            Logger.Debug($"Started process {fileName} with process-id {process.Id}: has exited {process.HasExited}");

            if (affinityMask > 0)
            {
                SetProcessAffinity(process.Id, affinityMask);
            }

            if (priorityClass > 0 && priorityClass != NativeConstants.NORMAL_PRIORITY_CLASS)
            {
                SetProcessPriority(process.Id, priorityClass);
            }

            if (wait)
            {
                process.WaitForExit();
            }

            return process;
        }

        public bool StopProcess(int processId)
        {
            if (!_winApiService.IsAdministrator())
            {
                return _rpcService.Call<bool>(nameof(StopProcess), processId);
            }

            var process = Process.GetProcessById(processId);

            if (process == null)
            {
                return false;
            }

            process.Kill();

            return true;
        }

        public bool SetProcessAffinity(int processId, uint affinityMask)
        {
            if (!_winApiService.IsAdministrator())
            {
                return _rpcService.Call<bool>(nameof(SetProcessAffinity), processId, affinityMask);
            }

            var hProcess = NativeMethods.OpenProcess(NativeConstants.PROCESS_ALL_ACCESS, false, (uint)processId);

            if (hProcess == nint.Zero)
            {
                CheckWin32Error($"Unable to set processor affinity on process {processId}");
                return false;
            }

            return NativeMethods.SetProcessAffinityMask(hProcess, affinityMask);
        }

        public bool SetProcessPriority(int processId, uint priorityClass)
        {
            if (!_winApiService.IsAdministrator())
            {
                return _rpcService.Call<bool>(nameof(SetProcessPriority), processId, priorityClass);
            }

            var hProcess = NativeMethods.OpenProcess(NativeConstants.PROCESS_ALL_ACCESS, false, (uint)processId);

            if (hProcess == nint.Zero)
            {
                CheckWin32Error($"Unable to set process priority on process {processId}");
                return false;
            }

            return NativeMethods.SetPriorityClass(hProcess, priorityClass);
        }

        public void InstallService()
        {
            if (!_winApiService.IsAdministrator())
            {
                _elevatedProcessManager.ExecuteElevated(nameof(InstallService));

                return;
            }


            if (!_winApiService.IsServiceInstalled())
            {
                StartProcess("sc.exe", @$"create ""{SERVICE_NAME}"" binpath=""{Process.GetCurrentProcess().MainModule.FileName}"" start=auto", wait: true);
                StartProcess("sc.exe", @$"description ""{SERVICE_NAME}"" ""Executes tasks for Color Control that require elevation. If this service is stopped, functions like WOL might be impacted.""", wait: true);
            }

            StartService();
        }

        public void UninstallService()
        {
            if (!_winApiService.IsAdministrator())
            {
                _elevatedProcessManager.ExecuteElevated(nameof(UninstallService));

                return;
            }

            if (!_winApiService.IsServiceInstalled())
            {
                return;
            }

            StopService();

            StartProcess("sc.exe", @$"delete ""{SERVICE_NAME}""", wait: true);
        }

        public void StartService()
        {
            if (!_winApiService.IsAdministrator())
            {
                _elevatedProcessManager.ExecuteElevated(nameof(StartService));
                return;
            }

            var controller = new ServiceController(SERVICE_NAME);

            if (controller.Status == ServiceControllerStatus.Stopped)
            {
                controller.Start();
            }
        }

        public void StopService()
        {
            if (!_winApiService.IsAdministrator())
            {
                _elevatedProcessManager.ExecuteElevated(nameof(StopService));
                return;
            }

            var controller = new ServiceController(SERVICE_NAME);

            if (controller.Status == ServiceControllerStatus.Running)
            {
                controller.Stop();
            }
        }

        public bool InstallChromeFix(bool install, string applicationDataFolder)
        {
            if (!_winApiService.IsAdministrator())
            {
                return _rpcService.Call<bool>(nameof(InstallChromeFix), new object[] { install, applicationDataFolder });
            }

            var argument = "--disable-lcd-text";

            var key = Registry.ClassesRoot.OpenSubKey(@"ChromeHTML\shell\open\command", true);
            if (key != null)
            {
                var value = key.GetValue(null).ToString();
                value = value.Replace("chrome.exe\" " + argument + " --", "chrome.exe\" --");
                if (install)
                {
                    value = value.Replace("chrome.exe\" --", "chrome.exe\" " + argument + " --");
                }
                key.SetValue(null, value);
            }

            var roamingFolder = applicationDataFolder ?? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            UpdateShortcut(Path.Combine(roamingFolder, @"Microsoft\Internet Explorer\Quick Launch\User Pinned\TaskBar\Google Chrome.lnk"), argument, !install);
            UpdateShortcut(Path.Combine(roamingFolder, @"Microsoft\Internet Explorer\Quick Launch\Google Chrome.lnk"), argument, !install);

            var allUsersStartMenuFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
            UpdateShortcut(Path.Combine(allUsersStartMenuFolder, @"Programs\Google Chrome.lnk"), argument, !install);

            return true;
        }

        public void RegisterTask(string taskName, bool enabled, TaskRunLevel runLevel = TaskRunLevel.LUA)
        {
            if (!_winApiService.IsAdministrator())
            {
                _elevatedProcessManager.ExecuteElevated(nameof(RegisterTask), new object[] { taskName, enabled, runLevel });
                return;
            }

            var file = Process.GetCurrentProcess().MainModule.FileName;
            var directory = Path.GetDirectoryName(file);

            try
            {
                using (TaskService ts = new TaskService())
                {
                    if (enabled)
                    {
                        var td = ts.NewTask();

                        td.RegistrationInfo.Description = "Start ColorControl";
                        td.Principal.RunLevel = runLevel;

                        td.Triggers.Add(new LogonTrigger { UserId = WindowsIdentity.GetCurrent().Name });

                        td.Actions.Add(new ExecAction(file, StartUpParams.RunningFromScheduledTaskParam, directory));

                        ts.RootFolder.RegisterTaskDefinition(taskName, td);
                    }
                    else
                    {
                        ts.RootFolder.DeleteTask(taskName, false);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Could not create/delete task: {e.Message}");
            }
        }

        public bool UninstallColorProfile(string filename)
        {
            if (!_winApiService.IsAdministrator())
            {
                return _rpcService.Call<bool>(nameof(UninstallColorProfile), filename);
            }

            var deleteResult = CCD.UninstallColorProfile(filename);

            return deleteResult;
        }

        private static bool UpdateShortcut(string path, string arguments, bool removeArguments = false)
        {
            if (File.Exists(path))
            {
                IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();

                IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(path);

                shortcut.Arguments = shortcut.Arguments.Replace(arguments, "");
                if (!removeArguments)
                {
                    if (!string.IsNullOrEmpty(shortcut.Arguments))
                    {
                        shortcut.Arguments = shortcut.Arguments + " ";
                    }
                    shortcut.Arguments = shortcut.Arguments + arguments;
                }

                // save it / create
                shortcut.Save();

                return true;
            }
            return false;
        }

        private static bool ProcessResult(uint result, [CallerMemberName] string callerName = "")
        {
            if (result != NativeConstants.ERROR_SUCCESS)
            {
                // Logging
                Logger.Error("Error executing {CallerName}: error code {Result}", callerName, result);
            }

            return result == NativeConstants.ERROR_SUCCESS;
        }

        private static void CheckWin32Error(string message)
        {
            var errorMessage = new Win32Exception(Marshal.GetLastWin32Error()).Message;

            throw new Exception($"{message}: {errorMessage}");
        }
    }
}
