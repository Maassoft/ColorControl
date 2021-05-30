namespace ColorControl
{
    public class AppContext
    {
        public static AppContext CurrentContext { get; private set; }

        public Config Config { get; private set; }

        public StartUpParams StartUpParams { get; private set; }

        public AppContext(Config config, StartUpParams startUpParams)
        {
            Config = config;
            StartUpParams = startUpParams;

            CurrentContext = this;
        }
    }
}
