using ColorControl.Common;
using ColorControl.Forms;
using ColorControl.Services.AMD;
using ColorControl.Services.LG;
using ColorControl.Services.NVIDIA;
using ColorControl.Svc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using NLog;
using NWin32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorControl
{
    static class Program
    {
        public const string TS_TASKNAME = "ColorControl";
        public static string DataDir { get; private set; }
        public static string ConfigFilename { get; private set; }
        public static string LogFilename { get; private set; }
        public static Config Config { get; private set; }
        public static AppContext AppContext { get; private set; }

        public static string MutexId { get; private set; }

        public static bool IsRestarting { get; private set; }

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static Mutex _mutex;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            DataDir = Utils.GetDataPath();

            //Utils.UpdateFiles(@"H:\temp\ColorControl\ColorControl", @"C:\Users\vinni\source\repos\ColorControl\ColorControl\bin\Debug\net6.0-windows10.0.20348.0");

            var runAsService = args.Contains("--service") || Process.GetCurrentProcess().Parent()?.ProcessName?.Equals("services", StringComparison.InvariantCultureIgnoreCase) == true;

            InitLogger(runAsService);

            Logger.Debug($"Using data path: {DataDir}");
            Logger.Debug("Parent process: " + Process.GetCurrentProcess().Parent()?.ProcessName);

            if (runAsService)
            {
                Utils.WaitForTask(RunService(args));
                return;
            }

            MutexId = $"Global\\{typeof(MainForm).GUID}";

            var currentDomain = AppDomain.CurrentDomain;
            // Handler for unhandled exceptions.
            currentDomain.UnhandledException += GlobalUnhandledExceptionHandler;
            // Handler for exceptions in threads behind forms.
            Application.ThreadException += GlobalThreadExceptionHandler;

            LoadConfig();

            if (Config.UseGdiScaling)
            {
                Application.SetHighDpiMode(HighDpiMode.DpiUnawareGdiScaled);
            }

            var startUpParams = StartUpParams.Parse(args);

            AppContext = new AppContext(Config, startUpParams);

            var existingProcess = Utils.GetProcessByName("ColorControl");

            try
            {
                if (HandleStartupParams(startUpParams, existingProcess))
                {
                    return;
                }
            }
            finally
            {
                Utils.CloseConsole();
            }

            _mutex = new Mutex(true, MutexId, out var mutexCreated);
            try
            {
                if (!mutexCreated)
                {
                    if (existingProcess != null && existingProcess.Threads.Count > 0)
                    {
                        var thread = existingProcess.Threads[0];
                        NativeMethods.EnumThreadWindows((uint)thread.Id, EnumThreadWindows, IntPtr.Zero);

                        return;
                    }

                    MessageBox.Show("Only one instance of this program can be active.", "ColorControl");
                }
                else
                {
                    _mutex.WaitOne();
                    try
                    {
                        if (Debugger.IsAttached)
                        {
                            Utils.StartService();
                        }

                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Application.Run(new MainForm(AppContext));

                        if (Debugger.IsAttached && !IsRestarting)
                        {
                            Utils.StopService();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error while initializing application: " + ex.ToLogString(Environment.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            finally
            {
                if (mutexCreated)
                {
                    _mutex?.Close();
                }
            }
        }

        public static void Restart()
        {
            IsRestarting = true;

            _mutex?.Close();
            _mutex = null;

            Thread.Sleep(1000);

            Application.Restart();

            Thread.Sleep(1000);
            Environment.Exit(0);
        }

        private static void InitLogger(bool runAsService)
        {
            var config = new NLog.Config.LoggingConfiguration();

            // Targets where to log to: File and Console
            var logFileAppendix = runAsService ? "_svc" : "";

            LogFilename = Path.Combine(DataDir, $"LogFile{logFileAppendix}.txt");

            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = LogFilename };

            //var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            // Rules for mapping loggers to targets            
            //config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, logfile);

            // Apply config           
            LogManager.Configuration = config;
        }

        private static void LoadConfig()
        {
            ConfigFilename = Path.Combine(DataDir, "Settings.json");

            try
            {
                if (File.Exists(ConfigFilename))
                {
                    var data = File.ReadAllText(ConfigFilename);
                    Config = JsonConvert.DeserializeObject<Config>(data);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"LoadConfig: {ex.Message}");
            }
            Config ??= new Config();

            Utils.UseDedicatedElevatedProcess = Config.UseDedicatedElevatedProcess;
        }

        public static bool HandleStartupParams(StartUpParams startUpParams, Process existingProcess)
        {
            if (startUpParams.ActivateChromeFontFix || startUpParams.DeactivateChromeFontFix)
            {
                Utils.InstallChromeFix(startUpParams.ActivateChromeFontFix);
                return true;
            }
            if (startUpParams.EnableAutoStart || startUpParams.DisableAutoStart)
            {
                Utils.RegisterTask(TS_TASKNAME, startUpParams.EnableAutoStart, startUpParams.AutoStartRunLevel);
                return true;
            }
            if (startUpParams.SetProcessAffinity)
            {
                Utils.SetProcessAffinity(startUpParams.ProcessId, startUpParams.AffinityMask);
                return true;
            }
            if (startUpParams.SetProcessPriority)
            {
                Utils.SetProcessPriority(startUpParams.ProcessId, startUpParams.PriorityClass);
                return true;
            }
            if (startUpParams.StartElevated)
            {
                StartElevated();
                return true;
            }
            if (startUpParams.SendWol)
            {
                return WOL.WakeFunction(startUpParams.WolMacAddress, startUpParams.WolIpAddress);
            }
            if (startUpParams.InstallService)
            {
                Utils.InstallService();
                return true;
            }
            if (startUpParams.UninstallService)
            {
                Utils.UninstallService();
                return true;
            }
            if (startUpParams.StartService)
            {
                Utils.StartService();
                return true;
            }
            if (startUpParams.StopService)
            {
                Utils.StopService();
                return true;
            }

            var useConsole = startUpParams.NoGui || existingProcess != null;

            if (!useConsole)
            {
                return false;
            }

            startUpParams.NoGui = true;

            var result = false;

            if (startUpParams.ExecuteHelp)
            {
                Utils.OpenConsole();

                Console.WriteLine("\nColorControl CLI");
                Console.WriteLine("Syntax  : ColorControl command options");
                Console.WriteLine("Commands:");
                Console.WriteLine("--nvpreset  <preset name or id>: execute NVIDIA-preset");
                Console.WriteLine("--amdpreset <preset name or id>: execute AMD-preset");
                Console.WriteLine("--lgpreset  <preset name>      : execute LG-preset");
                Console.WriteLine("--help                         : displays this help info");
                Console.WriteLine("Options:");
                Console.WriteLine("--nogui: starts command from the command line and will not open GUI (is forced when GUI is already running)");

                result = true;
            }

            if (startUpParams.ExecuteLgPreset)
            {
                Utils.OpenConsole();

                Console.WriteLine($"Executing LG-preset '{startUpParams.LgPresetName}'...");
                var task = LgService.ExecutePresetAsync(startUpParams.LgPresetName);

                var taskResult = Utils.WaitForTask(task);

                if (taskResult)
                {
                    Console.WriteLine("Done.");
                }

                result = true;
            }

            if (startUpParams.ExecuteNvidiaPreset)
            {
                Utils.OpenConsole();

                Console.WriteLine($"Executing NVIDIA-preset '{startUpParams.NvidiaPresetIdOrName}'...");
                var taskResult = NvService.ExecutePresetAsync(startUpParams.NvidiaPresetIdOrName);

                if (taskResult)
                {
                    Console.WriteLine("Done.");
                }

                result = true;
            }

            if (startUpParams.ExecuteAmdPreset)
            {
                Utils.OpenConsole();

                Console.WriteLine($"Executing AMD-preset '{startUpParams.AmdPresetIdOrName}'...");
                var taskResult = AmdService.ExecutePresetAsync(startUpParams.AmdPresetIdOrName);

                if (taskResult)
                {
                    Console.WriteLine("Done.");
                }

                result = true;
            }

            return result;
        }

        private static void StartElevated()
        {
            try
            {
                Application.Run(new ElevatedForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while initializing elevated application: " + ex.ToLogString(Environment.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static async Task RunService(string[] args)
        {
            Logger.Debug("RUNNING SERVICE");

            using IHost host = Host.CreateDefaultBuilder(args)
                .UseWindowsService(options =>
                {
                    options.ServiceName = "ColorControl Service";
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<ColorControlBackgroundService>();
                })
                .Build();

            await host.RunAsync();
        }

        public static int EnumThreadWindows(IntPtr handle, IntPtr param)
        {
            NativeMethods.SendMessageW(handle, Utils.WM_BRINGTOFRONT, UIntPtr.Zero, IntPtr.Zero);

            return 1;
        }

        private static void GlobalUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            GlobalHandleException((Exception)e.ExceptionObject, "Unhandled exception");
        }

        private static void GlobalThreadExceptionHandler(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            GlobalHandleException(e.Exception, "Exception in thread");
        }

        private static void GlobalHandleException(Exception exception, string type)
        {
            var trace = exception.ToLogString(Environment.StackTrace);
            var message = $"{type}: {trace}";

            Logger.Error(message);

            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
