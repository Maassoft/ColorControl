using ColorControl.Common;
using ColorControl.Services.Common;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ColorControl.Services.GameLauncher
{
    class GameService : ServiceBase<GamePreset>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public override string ServiceName => "Game";

        protected override string PresetsBaseFilename => "GamePresets.json";

        private ServiceManager _serviceManager;

        public GameService(AppContextProvider appContextProvider, ServiceManager serviceManager) : base(appContextProvider)
        {
            LoadPresets();
            _serviceManager = serviceManager;
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
        }

        public async Task<bool> ApplyPreset(string idOrName)
        {
            var preset = GetPresetByIdOrName(idOrName);
            if (preset != null)
            {
                return await ApplyPreset(preset, Program.AppContext);
            }
            else
            {
                return false;
            }
        }

        public override async Task<bool> ApplyPreset(GamePreset preset)
        {
            return await ApplyPreset(preset, Program.AppContext);
        }

        public async Task<bool> ApplyPreset(GamePreset preset, AppContext appContext)
        {
            var result = true;

            await ExecuteSteps(preset.PreLaunchSteps);

            Process process = null;

            if (!string.IsNullOrEmpty(preset.Path))
            {
                process = Utils.StartProcess(preset.Path, preset.Parameters, setWorkingDir: true, elevate: preset.RunAsAdministrator, affinityMask: preset.ProcessAffinityMask, priorityClass: preset.ProcessPriorityClass);
            }

            await ExecuteSteps(preset.PostLaunchSteps);

            if (process != null && preset.FinalizeSteps?.Any() == true)
            {
                await ExecuteFinalizationStepsAsync(process, preset.FinalizeSteps);
            }

            _lastAppliedPreset = preset;

            PresetApplied();

            return result;
        }

        private async Task ExecuteFinalizationStepsAsync(Process process, List<string> finalizeSteps)
        {
            await process.WaitForExitAsync();

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
    }
}
