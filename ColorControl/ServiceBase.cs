using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace ColorControl
{
    abstract class ServiceBase<T> where T : PresetBase
    {
        protected string _dataPath;
        protected string _presetsFilename;
        protected JavaScriptSerializer _JsonSerializer;
        protected JavaScriptSerializer _JsonDeserializer;
        protected bool _initialized = false;
        protected List<T> _presets;

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

        public List<T> GetPresets()
        {
            return _presets;
        }

        public T GetPresetByIdOrName(string idOrName)
        {
            var preset = _presets.FirstOrDefault(p => p.name != null && p.name.Equals(idOrName, StringComparison.OrdinalIgnoreCase));
            if (preset == null && int.TryParse(idOrName, out var id))
            {
                preset = _presets.FirstOrDefault(p => p.id == id);
            }

            return preset;
        }

        protected virtual void Initialize()
        {

        }

        protected virtual void Uninitialize()
        {

        }
    }
}
