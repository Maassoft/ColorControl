using System.Web.Script.Serialization;

namespace ColorControl
{
    class ServiceBase
    {
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        protected string _dataPath;
        protected string _presetsFilename;
        protected JavaScriptSerializer _JsonSerializer;
        protected JavaScriptSerializer _JsonDeserializer;
        protected bool _initialized = false;

        public ServiceBase(string dataPath)
        {
            _dataPath = dataPath;

            _JsonSerializer = new JavaScriptSerializer();
            _JsonDeserializer = new JavaScriptSerializer();

            Initialize();
        }

        ~ServiceBase()
        {
            Uninitialize();
        }

        protected virtual void Initialize()
        {

        }

        protected virtual void Uninitialize()
        {

        }
    }
}
