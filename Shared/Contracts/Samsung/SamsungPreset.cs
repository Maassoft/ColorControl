namespace ColorControl.Shared.Contracts.Samsung;

public class SamsungPreset : PresetBase
{
    public static event GetValueByKey GetDeviceName;

    public string AppId { get; set; }
    public string DeviceMacAddress { get; set; }

    public SamsungPreset() : base()
    {
    }

    public SamsungPreset(SamsungPreset preset) : this()
    {
        id = GetNewId();

        Update(preset);
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

    public void Update(SamsungPreset preset)
    {
        name = preset.name;
        Description = preset.Description;
        AppId = preset.AppId;
        DeviceMacAddress = preset.DeviceMacAddress;
        ShowInQuickAccess = preset.ShowInQuickAccess;
        var trigger = preset.Triggers.FirstOrDefault();
        if (trigger != null)
        {
            UpdateTrigger(trigger.Trigger, trigger.Conditions, trigger.IncludedProcessesAsString, trigger.ExcludedProcessesAsString, trigger.ConnectedDisplaysRegex);
        }
        Steps.Clear();
        Steps.AddRange(preset.Steps);
    }

    public SamsungPreset Clone()
    {
        var preset = new SamsungPreset(this);

        preset.name += " (copy)";

        return preset;
    }

    public SamsungPreset CloneWithId()
    {
        var preset = new SamsungPreset(this);
        preset.SetId(id);
        preset.name = name;
        preset.shortcut = shortcut;

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
            deviceString = GetDeviceName?.Invoke(DeviceMacAddress) ?? DeviceMacAddress;
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

