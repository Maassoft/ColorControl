using ColorControl.Common;
using NLog.Config;
using System;
using System.ComponentModel;
using System.Threading;

namespace ColorControl
{
    public class AppContext
    {
        public static AppContext CurrentContext { get; private set; }

        public Config Config { get; private set; }

        public StartUpParams StartUpParams { get; private set; }
        public string DataPath { get; private set; }
        public SynchronizationContext SynchronizationContext { get; set; } = AsyncOperationManager.SynchronizationContext;
        public LoggingRule LoggingRule { get; private set; }
        public DateTime StartTime { get; private set; } = DateTime.Now;

        public AppContext(Config config, StartUpParams startUpParams, string dataPath, LoggingRule loggingRule)
        {
            Config = config;
            StartUpParams = startUpParams;
            DataPath = dataPath;
            LoggingRule = loggingRule;

            CurrentContext = this;
        }

        public bool IsServiceRunning()
        {
            return Utils.IsServiceRunning();
        }

        public void SetLogLevel(NLog.LogLevel logLevel)
        {
            Config.LogLevel = logLevel.ToString();
            LoggingRule.SetLoggingLevels(logLevel, NLog.LogLevel.Fatal);
        }
    }
}
