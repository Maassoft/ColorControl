using ColorControl.Shared.Contracts;
using ColorControl.Shared.Services;
using NLog;
using NLog.Config;
using System.ComponentModel;

namespace ColorControl.Shared.Common

{
    public class GlobalContext
    {
        public static GlobalContext CurrentContext { get; private set; }

        public Config Config { get; private set; }

        public StartUpParams StartUpParams { get; private set; }
        public string DataPath { get; private set; }
        public SynchronizationContext SynchronizationContext { get; set; } = AsyncOperationManager.SynchronizationContext;
        public LoggingRule LoggingRule { get; private set; }
        public DateTime StartTime { get; private set; } = DateTime.Now;
        public string MutexId { get; private set; }
        public nint MainHandle { get; set; }
        public IServiceProvider ServiceProvider { get; set; }

        public string ApplicationTitle { get; private set; }
        public string ApplicationTitleAdmin { get; private set; }
        public bool UpdateAvailable { get; set; }

        public GlobalContext(Config config, StartUpParams startUpParams, string dataPath, LoggingRule loggingRule, IServiceProvider serviceProvider, string mutexId = null)
        {
            Config = config;
            StartUpParams = startUpParams;
            DataPath = dataPath;
            LoggingRule = loggingRule;
            ServiceProvider = serviceProvider;
            MutexId = mutexId;

            CurrentContext = this;

            ApplicationTitle = Application.ProductName + " " + Application.ProductVersion;

            if (WinApiService.IsAdministratorStatic())
            {
                ApplicationTitleAdmin = ApplicationTitle + " (administrator)";
            }
            else
            {
                ApplicationTitleAdmin = ApplicationTitle;
            }
        }

        public void SetLogLevel(LogLevel logLevel)
        {
            Config.LogLevel = logLevel.ToString();
            LoggingRule.SetLoggingLevels(logLevel, LogLevel.Fatal);
            LogManager.ReconfigExistingLoggers();
        }
    }
}
