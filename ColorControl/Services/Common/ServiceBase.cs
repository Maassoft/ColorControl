using ColorControl.Shared.EventDispatcher;
using ColorControl.Shared.Forms;
using ColorControl.Shared.Native;
using ColorControl.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NStandard;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ColorControl.Services.Common
{
    abstract class ServiceBase<T> : IServiceBase where T : PresetBase, new()
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
        protected T _lastTriggeredPreset;
        protected int _quickAccessShortcutId;

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

        public async Task<bool> ApplyPresetUi(T preset)
        {
            if (preset == null)
            {
                return false;
            }
            try
            {
                var result = await ApplyPreset(preset);
                if (!result)
                {
                    throw new Exception($"Error while applying {ServiceName}-preset. At least one setting could not be applied. Check the log for details.");
                }

                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e);

                MessageForms.ErrorOk($"Error applying {ServiceName}-preset ({e.TargetSite.Name}): {e.Message}");
                return false;
            }
        }

        public async Task<bool> ApplyPresetById(int id)
        {
            var preset = _presets.FirstOrDefault(p => p.id == id);

            if (preset == null)
            {
                return false;
            }

            return await ApplyPresetUi(preset);
        }

        protected void SetShortcuts(int quickAccessShortcutId = 0, string shortcut = null)
        {
            _quickAccessShortcutId = quickAccessShortcutId;

            var keyboardShortcutDispatcher = _appContextProvider.GetAppContext().ServiceProvider.GetService<KeyboardShortcutDispatcher>();

            if (keyboardShortcutDispatcher == null)
            {
                return;
            }

            if (quickAccessShortcutId != 0 && !shortcut.IsNullOrWhiteSpace())
            {
                keyboardShortcutDispatcher.RegisterShortcut(quickAccessShortcutId, shortcut);
            }

            foreach (var preset in _presets)
            {
                keyboardShortcutDispatcher.RegisterShortcut(preset.id, preset.shortcut);
            }

            keyboardShortcutDispatcher.RegisterAsyncEventHandler(KeyboardShortcutDispatcher.Event_HotKey, HotKeyPressed);
        }

        public virtual void InstallEventHandlers()
        {

        }

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

        protected async Task<PresetTriggerContext> CreateTriggerContext(ServiceManager serviceManager, ProcessChangedEventArgs context = null, bool? isHDRActive = null, IList<PresetTriggerType> triggerTypes = null)
        {
            triggerTypes ??= new[] { PresetTriggerType.ProcessSwitch };

            var changedProcesses = new List<Process>();
            if (context?.ForegroundProcess != null)
            {
                changedProcesses.Add(context.ForegroundProcess);
            }

            var isGsyncActive = await serviceManager.HandleExternalServiceAsync("GsyncEnabled", new[] { "" });

            var triggerContext = new PresetTriggerContext
            {
                Triggers = triggerTypes,
                IsHDRActive = isHDRActive ?? CCD.IsHDREnabled(),
                IsGsyncActive = isGsyncActive,
                ForegroundProcess = context?.ForegroundProcess,
                ForegroundProcessIsFullScreen = context?.ForegroundProcessIsFullScreen ?? false,
                IsNotificationDisabled = context?.IsNotificationDisabled ?? false,
                ChangedProcesses = changedProcesses,
                ScreenSaverTransitionState = context?.ScreenSaverTransitionState ?? ScreenSaverTransitionState.None
            };

            return triggerContext;
        }

        protected async Task ExecuteScreenSaverPresets(ServiceManager serviceManager, ProcessChangedEventArgs context, bool? isHDRActive = null)
        {
            await ExecuteEventPresets(serviceManager, new[] { PresetTriggerType.ScreensaverStart, PresetTriggerType.ScreensaverStop }, context, isHDRActive);
        }

        public async Task ExecuteEventPresets(ServiceManager serviceManager, IList<PresetTriggerType> triggerTypes, ProcessChangedEventArgs context = null, bool? isHDRActive = null)
        {
            var triggerContext = await CreateTriggerContext(serviceManager, context, isHDRActive, triggerTypes);

            var triggerPresets = _presets.Where(p => p.Triggers.Any(t => t.TriggerActive(triggerContext))).ToList();

            if (!triggerPresets.Any())
            {
                return;
            }

            Logger.Debug($"Executing event: {string.Join(',', triggerTypes)}, presets count: {triggerPresets.Count}");

            foreach (var preset in triggerPresets)
            {
                Logger.Debug($"Executing event preset: {preset.name}");

                await ApplyPreset(preset);
            }
        }

        protected virtual async Task HotKeyPressed(object sender, KeyboardShortcutEventArgs e, CancellationToken _)
        {
            if (e.HotKeyId == _quickAccessShortcutId)
            {
                ToggleQuickAccessForm();
                return;
            }

            await ApplyPresetById(e.HotKeyId);
        }
    }
}
