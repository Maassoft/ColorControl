using ColorControl.Common;
using ColorControl.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ColorControl.Services.GameLauncher
{
    class GameService : ServiceBase<GamePreset>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public override string ServiceName => "Game";

        private Func<string, string[], bool> _externalServiceHandler;

        public GameService(string dataPath, Func<string, string[], bool> externalServiceHandler = null) : base(dataPath, "GamePresets.json")
        {
            LoadPresets();
            _externalServiceHandler = externalServiceHandler;
        }

        public static bool ExecutePresetAsync(string idOrName)
        {
            try
            {
                var service = new GameService(Program.DataDir);

                var result = service.ApplyPreset(idOrName);

                if (!result)
                {
                    Console.WriteLine("Preset not found or error while executing.");
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error executing preset: " + ex.ToLogString());
                return false;
            }
        }

        protected override List<GamePreset> GetDefaultPresets()
        {
            return new List<GamePreset>();
        }

        private void SavePresets()
        {
            try
            {
                var json = JsonConvert.SerializeObject(_presets);
                File.WriteAllText(_presetsFilename, json);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToLogString());
            }
        }

        public void GlobalSave()
        {
            SavePresets();
        }

        public bool ApplyPreset(string idOrName)
        {
            var preset = GetPresetByIdOrName(idOrName);
            if (preset != null)
            {
                return ApplyPreset(preset, Program.AppContext);
            }
            else
            {
                return false;
            }
        }

        public override bool ApplyPreset(GamePreset preset)
        {
            return ApplyPreset(preset, Program.AppContext);
        }

        public bool ApplyPreset(GamePreset preset, AppContext appContext)
        {
            var result = true;

            foreach (var step in preset.PreLaunchSteps)
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

                if (_externalServiceHandler != null && parameters != null)
                {
                    handled = _externalServiceHandler(key, parameters);
                }

                if (!handled)
                {
                }

                if (delay > 0)
                {
                    Utils.WaitForTask(Task.Delay(delay));
                }

            }

            if (!string.IsNullOrEmpty(preset.Path))
            {
                Utils.StartProcess(preset.Path, preset.Parameters, setWorkingDir: true, elevate: preset.RunAsAdministrator, affinityMask: preset.ProcessAffinityMask, priorityClass: preset.ProcessPriorityClass);
            }

            _lastAppliedPreset = preset;

            PresetApplied();

            return result;
        }
    }
}
