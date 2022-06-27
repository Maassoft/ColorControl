using ColorControl.Services.Common;
using System.Collections.Generic;

namespace ColorControl.Services.GameLauncher
{
    class GamePreset : PresetBase
    {
        public string Path { get; set; }
        public string Parameters { get; set; }
        public bool RunAsAdministrator { get; set; }
        public List<string> PreLaunchSteps { get; set; }
        public List<string> PostLaunchSteps { get; set; }

        public GamePreset() : base()
        {
            PreLaunchSteps = new List<string>();
            PostLaunchSteps = new List<string>();
        }

        public GamePreset(GamePreset preset) : this()
        {
            id = GetNewId();

            name = preset.name;
            Path = preset.Path;
            Parameters = preset.Parameters;
            RunAsAdministrator = preset.RunAsAdministrator;
            PreLaunchSteps.AddRange(preset.PreLaunchSteps);
            PostLaunchSteps.AddRange(preset.PostLaunchSteps);
        }

        public GamePreset Clone()
        {
            var preset = new GamePreset(this);

            preset.name += " (copy)";

            return preset;
        }

        public static string[] GetColumnNames()
        {
            return new[] { "Name|160", "File/URI|400", "Parameters|200", "Pre-launch steps|300" };
        }

        public override List<string> GetDisplayValues(Config config = null)
        {
            var values = new List<string>();

            values.Add(name);
            values.Add(Path);
            values.Add(Parameters);

            values.Add(string.Join(", ", PreLaunchSteps));
            values.Add(string.Join(", ", PostLaunchSteps));

            return values;
        }
    }
}

