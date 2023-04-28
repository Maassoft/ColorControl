using ColorControl.Common;
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

        public AppContext(Config config, StartUpParams startUpParams, string dataPath)
        {
            Config = config;
            StartUpParams = startUpParams;
            DataPath = dataPath;

            CurrentContext = this;
        }

        public bool IsServiceRunning()
        {
            return Utils.IsServiceRunning();
        }
    }
}
