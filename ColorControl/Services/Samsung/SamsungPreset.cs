using ColorControl.Services.Common;
using ColorControl.Shared.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace ColorControl.Services.Samsung
{
    class SamsungPreset : PresetBase
    {
        public string AppId { get; set; }
        public List<string> Steps { get; set; }
        public string DeviceMacAddress { get; set; }
        public static List<SamsungDevice> SamsungDevices { get; internal set; }

        public SamsungPreset() : base()
        {
            Steps = new List<string>();
        }

        public SamsungPreset(SamsungPreset preset) : this()
        {
            id = GetNewId();

            name = preset.name;
            AppId = preset.AppId;
            DeviceMacAddress = preset.DeviceMacAddress;
            Steps.AddRange(preset.Steps);
        }

        public SamsungPreset(string name, string appId, IEnumerable<string> steps = null) : this()
        {
            this.name = name;
            AppId = appId;
            if (steps != null)
            {
                Steps.AddRange(steps);
            }
        }

        public SamsungPreset Clone()
        {
            var preset = new SamsungPreset(this);

            preset.name += " (copy)";

            return preset;
        }

        public static string[] GetColumnNames()
        {
            return new[] { "Name", "Device|140", "App|200", "Steps|400", "Shortcut", "Trigger" };
        }

        public override List<string> GetDisplayValues(Config config = null)
        {
            var values = new List<string>();

            values.Add(name);

            var deviceString = "Global";
            if (!string.IsNullOrEmpty(DeviceMacAddress))
            {
                if (SamsungDevices != null)
                {
                    var device = SamsungDevices.FirstOrDefault(d => !string.IsNullOrEmpty(d.MacAddress) && d.MacAddress.Equals(DeviceMacAddress));
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

            var app = AppId;
            //if (LgApps != null)
            //{
            //    var item = LgApps.FirstOrDefault(x => x.appId.Equals(appId));
            //    if (item != null)
            //    {
            //        app = item.title + " (" + appId + ")";
            //    }
            //}
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
            return Steps.Aggregate("", (a, b) => (string.IsNullOrEmpty(a) ? "" : a + ", ") + b);
        }
    }
}

