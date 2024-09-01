using Newtonsoft.Json;

namespace ColorControl.Shared.Contracts.LG;

public class LgPreset : PresetBase
{
	public static event GetValueByKey GetDeviceName;
	public static event GetValueByKey GetAppName;

	public string appId { get; set; }
	public List<string> steps { get; set; }
	public string DeviceMacAddress { get; set; }
	public dynamic AppParams { get; set; }

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

		Update(preset);

		name = preset.name + " (copy)";
		shortcut = null;
	}

	public LgPreset(string name, string appId, IEnumerable<string> steps = null, dynamic appParams = null) : this()
	{
		this.name = name;
		this.appId = appId;
		if (steps != null)
		{
			this.steps.AddRange(steps);
		}
		AppParams = appParams;
	}

	public LgPreset Clone()
	{
		var preset = new LgPreset(this);

		return preset;
	}

	public LgPreset CloneWithId()
	{
		var preset = new LgPreset(this);
		preset.SetId(id);
		preset.name = name;
		preset.shortcut = shortcut;

		return preset;
	}

	public void Update(LgPreset preset)
	{
		name = preset.name;
		appId = preset.appId;
		shortcut = preset.shortcut;
		steps.Clear();
		steps.AddRange(preset.steps);
		Description = preset.Description;
		DeviceMacAddress = preset.DeviceMacAddress;
		ShowInQuickAccess = preset.ShowInQuickAccess;

		var trigger = preset.Triggers.FirstOrDefault();
		if (trigger != null)
		{
			UpdateTrigger(trigger.Trigger, trigger.Conditions, trigger.IncludedProcessesAsString, trigger.ExcludedProcessesAsString);
		}
	}

	public static List<string> GetColumnNames()
	{
		return new List<string>() { "Name", "Device|140", "App|200", "Steps|400", "Shortcut", "Trigger" };
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

		var app = appId;
		if (!string.IsNullOrEmpty(appId))
		{
			app = GetAppName?.Invoke(appId) ?? appId;
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
