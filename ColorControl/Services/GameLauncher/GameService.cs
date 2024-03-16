using ColorControl.Services.Common;
using ColorControl.Shared.Common;
using ColorControl.Shared.EventDispatcher;
using ColorControl.Shared.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ColorControl.Services.GameLauncher
{
    class GameService : ServiceBase<GamePreset>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public override string ServiceName => "Game";

        protected override string PresetsBaseFilename => "GamePresets.json";

        private ProcessEventDispatcher _processEventDispatcher;
        private readonly WinApiAdminService _winApiAdminService;
        private readonly WinApiService _winApiService;
        private ServiceManager _serviceManager;
        private int _lastStartedProcessId;
        private System.DateTime _lastStartedPresetDateTime = System.DateTime.MinValue;
        private string _lastStartedProcessPath;
        private string _configFilename;

        public GameServiceConfig Config { get; private set; }

        public GameService(AppContextProvider appContextProvider, ServiceManager serviceManager, ProcessEventDispatcher processEventDispatcher, WinApiAdminService winApiAdminService, WinApiService winApiService) : base(appContextProvider)
        {
            LoadConfig();
            LoadPresets();
            _serviceManager = serviceManager;
            _processEventDispatcher = processEventDispatcher;
            _winApiAdminService = winApiAdminService;
            _winApiService = winApiService;
            _processEventDispatcher.RegisterAsyncEventHandler(ProcessEventDispatcher.Event_ProcessChanged, ProcessChanged);

            SetShortcuts(-203, _appContextProvider.GetAppContext().Config.GameQuickAccessShortcut);
        }

        private async Task ProcessChanged(object sender, ProcessChangedEventArgs e, CancellationToken token)
        {
            if (e.StartedProcesses?.Any() != true || !Config.ApplyExternallyLaunched)
            {
                return;
            }

            if (_lastStartedPresetDateTime > System.DateTime.Now.AddSeconds(-5))
            {
                return;
            }
            _lastStartedProcessId = 0;

            var startedProcesses = e.StartedProcesses;

            foreach (var startedProcess in startedProcesses)
            {
                try
                {
                    if (await HandleStartedProcess(startedProcess))
                    {
                        return;
                    }
                }
                catch (System.Exception) { }
            }
        }

        private async Task<bool> HandleStartedProcess(Process process)
        {
            GamePreset preset;

            if (_winApiService.IsAdministrator())
            {
                var mainModule = process.MainModule;

                if (mainModule == null)
                {
                    return false;
                }

                var fileName = mainModule.FileName;

                preset = _presets.FirstOrDefault(p => fileName.Contains(Path.GetDirectoryName(p.Path), System.StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                var name = process.ProcessName.Replace("-Win64-Shipping", "");
                preset = _presets.FirstOrDefault(p => Path.GetFileName(p.Path).Contains(name, System.StringComparison.OrdinalIgnoreCase));
            }

            if (preset == null || !preset.AutoSettings.AllowAutoApply)
            {
                return false;
            }

            if (_lastStartedProcessId != process.Id)
            {
                Logger.Debug($"Auto-starting preset {preset.name} for process {process.ProcessName}");

                var mainThreadId = 0;
                var suspended = false;
                if (preset.AutoSettings.ProcessAutoAction == ProcessAutoAction.Suspend)
                {
                    mainThreadId = _winApiAdminService.SuspendMainThread(process.Id);
                    suspended = true;
                }
                else if (preset.AutoSettings.ProcessAutoAction == ProcessAutoAction.Restart)
                {
                    _winApiAdminService.StopProcess(process.Id);
                    process = null;
                }

                var _ = ExecutePreset(preset, process, suspended ? mainThreadId : 0);

                return true;
            }

            return false;
        }

        protected override List<GamePreset> GetDefaultPresets()
        {
            return new List<GamePreset>();
        }

        private void SavePresets()
        {
            Utils.WriteObject(_presetsFilename, _presets);
        }

        public void GlobalSave()
        {
            SavePresets();
            SaveConfig();
        }

        public async Task<bool> ApplyPreset(string idOrName)
        {
            var preset = GetPresetByIdOrName(idOrName);
            if (preset != null)
            {
                return await ApplyPreset(preset);
            }
            else
            {
                return false;
            }
        }

        public override async Task<bool> ApplyPreset(GamePreset preset)
        {
            var result = await ExecutePreset(preset);

            _lastAppliedPreset = preset;

            PresetApplied();

            return result;
        }

        private async Task<bool> ExecutePreset(GamePreset preset, Process process = null, int resumeThreadId = default)
        {
            var result = true;

            await ExecuteSteps(preset.PreLaunchSteps);

            if (process == null && !string.IsNullOrEmpty(preset.Path))
            {
                process = _winApiAdminService.StartProcess(preset.Path, preset.Parameters, setWorkingDir: true, elevate: preset.RunAsAdministrator, affinityMask: preset.ProcessAffinityMask, priorityClass: preset.ProcessPriorityClass);

                _lastStartedPresetDateTime = System.DateTime.Now;
                _lastStartedProcessId = process.Id;
                _lastStartedProcessPath = Path.GetDirectoryName(preset.Path);
            }
            else if (resumeThreadId != default)
            {
                _winApiAdminService.ResumeThread((uint)resumeThreadId);
            }

            await ExecuteSteps(preset.PostLaunchSteps);

            if (process != null && preset.FinalizeSteps?.Any() == true)
            {
                await ExecuteFinalizationStepsAsync(process, preset.FinalizeSteps);
            }

            return result;
        }

        private async Task ExecuteFinalizationStepsAsync(Process process, List<string> finalizeSteps)
        {
            await Task.Delay(1000);

            try
            {
                if (process.HasExited)
                {
                    return;
                }

                await process.WaitForExitAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return;
            }

            await ExecuteSteps(finalizeSteps);
        }

        private async Task ExecuteSteps(List<string> steps)
        {
            if (steps?.Any() != true)
            {
                return;
            }

            foreach (var step in steps)
            {
                var keySpec = step.Split(':');

                var delay = 0;
                var key = step;
                if (keySpec.Length == 2)
                {
                    delay = Utils.ParseInt(keySpec[1]);
                    if (delay > 0)
                    {
                        key = keySpec[0];
                    }
                }

                var index = key.IndexOf("(");
                string[] parameters = null;
                if (index > -1)
                {
                    var keyValue = key.Split('(');
                    key = keyValue[0];
                    parameters = keyValue[1].Substring(0, keyValue[1].Length - 1).Trim().Split(';');
                }

                var handled = false;

                if (parameters != null)
                {
                    handled = await _serviceManager.HandleExternalServiceAsync(key, parameters);
                }

                if (!handled)
                {
                }

                if (delay > 0)
                {
                    await Task.Delay(delay);
                }

            }
        }

        private void LoadConfig()
        {
            _configFilename = Path.Combine(_dataPath, "GameConfig.json");
            try
            {
                if (File.Exists(_configFilename))
                {
                    Config = JsonConvert.DeserializeObject<GameServiceConfig>(File.ReadAllText(_configFilename));
                }
            }
            catch (System.Exception ex)
            {
                Logger.Error($"LoadConfig: {ex.Message}");
            }
            Config ??= new GameServiceConfig();
        }

        private void SaveConfig()
        {
            Utils.WriteObject(_configFilename, Config);
        }
    }
}
