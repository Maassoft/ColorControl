using ColorControl.Shared.Contracts;
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

        public GlobalContext(Config config, StartUpParams startUpParams, string dataPath, LoggingRule loggingRule, string mutexId = null)
        {
            Config = config;
            StartUpParams = startUpParams;
            DataPath = dataPath;
            LoggingRule = loggingRule;
            MutexId = mutexId;

            CurrentContext = this;
        }

        public void SetLogLevel(LogLevel logLevel)
        {
            Config.LogLevel = logLevel.ToString();
            LoggingRule.SetLoggingLevels(logLevel, LogLevel.Fatal);
            LogManager.ReconfigExistingLoggers();
        }
    }
}
