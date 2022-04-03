using Newtonsoft.Json;
using NLog;
using NWin32;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace ColorControl
{
    static class Program
    {
        public const string TS_TASKNAME = "ColorControl";
        public static string DataDir { get; private set; }
        public static string ConfigFilename { get; private set; }
        public static Config Config { get; private set; }
        public static AppContext AppContext { get; private set; }

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var currentDomain = AppDomain.CurrentDomain;
            // Handler for unhandled exceptions.
            currentDomain.UnhandledException += GlobalUnhandledExceptionHandler;
            // Handler for exceptions in threads behind forms.
            Application.ThreadException += GlobalThreadExceptionHandler;

            DataDir = Utils.GetDataPath();
            InitLogger();

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

            string mutexId = $"Global\\{typeof(MainForm).GUID}";
            var mutex = new Mutex(true, mutexId, out var mutexCreated);
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
                    try
                    {
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Application.Run(new MainForm(AppContext));
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
                    mutex.Dispose();
                }
            }
        }

        private static void InitLogger()
        {
            var config = new NLog.Config.LoggingConfiguration();

            // Targets where to log to: File and Console
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = Path.Combine(DataDir, "LogFile.txt") };
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
        }

        private static bool HandleStartupParams(StartUpParams startUpParams, Process existingProcess)
        {
            if (startUpParams.ActivateChromeFontFix || startUpParams.DeactivateChromeFontFix)
            {
                Utils.InstallChromeFix(startUpParams.ActivateChromeFontFix);
                return true;
            }
            if (startUpParams.EnableAutoStart || startUpParams.DisableAutoStart)
            {
                Utils.RegisterTask(TS_TASKNAME, startUpParams.EnableAutoStart);
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
