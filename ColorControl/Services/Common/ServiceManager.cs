namespace ColorControl.Services.Common
{
    class ServiceManager
    {
        public static ServiceManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ServiceManager();
                }

                return _instance;
            }
        }
        private static ServiceManager _instance { get; set; }

        //private List<ServiceBase<PresetBase>> _services = new();

        public void Initialize()
        {

        }

    }
}
