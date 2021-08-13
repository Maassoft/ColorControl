using LgTv;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace ColorControl
{
    class LgPreset : PresetBase
    {
        public static List<LgApp> LgApps { get; set; }
        public static List<LgDevice> LgDevices { get; set; }


        public string appId { get; set; }
        public List<string> steps { get; set; }
        public string DeviceMacAddress { get; set; }

        [JsonIgnore]
        public int X { get; set; }
        [JsonIgnore]
        public int Y { get; set; }

        public LgPreset() : base()
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
            return new List<string>() { "Name", "Device|120", "App|200", "Steps|400", "Shortcut", "Trigger" };
        }

        public override List<string> GetDisplayValues(Config config = null)
        {
            var values = new List<string>();

            values.Add(name);

            var deviceString = "Global";
            if (!string.IsNullOrEmpty(DeviceMacAddress))
            {
                if (LgDevices != null)
                {
                    var device = LgDevices.FirstOrDefault(d => !string.IsNullOrEmpty(d.MacAddress) && d.MacAddress.Equals(DeviceMacAddress));
                    if (device != null)
                    {
                        deviceString = device.Name;
                    }
                    else
                    {
                        deviceString = "Unknown: device not found";
                    }
                }
                else
                {
                    deviceString = DeviceMacAddress;
                }
            }
            values.Add(deviceString);

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

            if (Triggers.Any())
            {
                values.Add(Triggers.First().ToString());
            }

            return values;
        }

        public string GetStepsDisplay()
        {
            return steps.Aggregate("", (a, b) => (string.IsNullOrEmpty(a) ? "" : a + ", ") + b);
        }
    }
}
