using LgTv;
using System.Collections.Generic;
using System.Linq;

namespace ColorControl
{
    class LgPreset : PresetBase
    {
        public string name { get; set; }
        public string appId { get; set; }
        public List<string> steps { get; set; }

        public static List<LgApp> LgApps { get; set; }

        public LgPreset()
        {
            steps = new List<string>();
        }

        public LgPreset(LgPreset preset) : this()
        {
            id = GetNewId();
            name = preset.name + " (copy)";
            appId = preset.appId;

            foreach (var step in preset.steps)
            {
                steps.Add(step);
            }
        }

        public LgPreset Clone()
        {
            var preset = new LgPreset(this);

            return preset;
        }

        public static List<string> GetColumnNames()
        {
            return new List<string>() { "Name", "App|200", "Steps|400", "Shortcut" };
        }

        public List<string> GetDisplayValues()
        {
            var values = new List<string>();

            values.Add(name);
            var app = appId;
            if (LgApps != null)
            {
                var item = LgApps.FirstOrDefault(x => x.appId.Equals(appId));
                if (item != null)
                {
                    app = item.title + " (" + appId + ")";
                }
            }
            values.Add(app);
            values.Add(GetStepsDisplay());
            values.Add(shortcut);

            return values;
        }

        public string GetStepsDisplay()
        {
            return steps.Aggregate("", (a, b) => (string.IsNullOrEmpty(a) ? "" : a + ", ") + b);
        }
    }
}
