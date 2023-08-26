using ColorControl.Shared.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ColorControl.Services.Common
{
    abstract class ServiceBase<T> where T : PresetBase, new()
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public event EventHandler<T> AfterApplyPreset;

        public abstract string ServiceName
        {
            get;
        }
        protected abstract string PresetsBaseFilename { get; }

        protected string _dataPath;
        protected string _presetsFilename;
        protected string _presetsBackupFilename;
        protected bool _initialized = false;
        protected List<T> _presets;
        protected T _lastAppliedPreset;
        protected string _loadPresetsError;

        protected AppContextProvider _appContextProvider;

        private List<JsonConverter> _jsonConverters;

        public ServiceBase(AppContextProvider appContextProvider)
        {
            _appContextProvider = appContextProvider;
            _dataPath = appContextProvider.GetAppContext().DataPath;
            _jsonConverters = new List<JsonConverter>();

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

        public abstract Task<bool> ApplyPreset(T preset);

        public T GetLastAppliedPreset()
        {
            return _lastAppliedPreset;
        }

        public virtual T CreateNewPreset()
        {
            var preset = new T();
            preset.name = CreateNewPresetName();

            return preset;
        }

        public string CreateNewPresetName()
        {
            var name = "New preset";
            string fullname;
            var number = 1;
            do
            {
                fullname = $"{name} ({number})";
                number++;
            } while (_presets.Any(x => x.name?.Equals(fullname) == true));

            return fullname;
        }

        protected virtual void Initialize()
        {

        }

        protected virtual void Uninitialize()
        {

        }

        protected void PresetApplied()
        {
            AfterApplyPreset?.Invoke(this, _lastAppliedPreset);
        }

        protected void LoadPresets()
        {
            _presetsFilename = Path.Combine(_dataPath, PresetsBaseFilename);
            _presetsBackupFilename = Path.Combine(_dataPath, $"{PresetsBaseFilename}.backup");

            try
            {
                var presetsExists = File.Exists(_presetsFilename);
                if (presetsExists)
                {
                    try
                    {
                        var json = File.ReadAllText(_presetsFilename);

                        // Hack to convert incorrect triggers
                        json = json.Replace(@"""Triggers"":0", @"""Triggers"":[]");

                        _presets = JsonConvert.DeserializeObject<List<T>>(json, _jsonConverters.ToArray());

                        if (_presets != null)
                        {
                            if (File.Exists(_presetsBackupFilename))
                            {
                                File.Delete(_presetsBackupFilename);
                            }
                            File.Copy(_presetsFilename, _presetsBackupFilename);
                        }
                    }
                    catch (Exception ex1)
                    {
                        Logger.Error($"Error while loading presets, reverting to default presets: {ex1.Message}");
                        _loadPresetsError = ex1.Message;
                    }
                }
                if (!presetsExists || _presets == null)
                {
                    Logger.Debug("Reverting to default presets");
                    try
                    {
                        _presets = GetDefaultPresets();
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"Error loading default presets, reverting to empty list: {e.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"General error while loading presets, reverting to empty list: {ex.Message}");
                _loadPresetsError = ex.Message;
            }
            if (_presets == null)
            {
                _presets = new List<T>();
            }
        }

        protected abstract List<T> GetDefaultPresets();

        protected void AddJsonConverter(JsonConverter jsonConverter)
        {
            _jsonConverters.Add(jsonConverter);
        }

        public void ToggleQuickAccessForm()
        {
            QuickAccessForm<T>.ToggleQuickAccessForm(this);
        }

    }
}
