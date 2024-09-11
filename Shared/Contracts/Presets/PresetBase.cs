﻿using ColorControl.Shared.Common;
using ColorControl.Shared.EventDispatcher;
using ColorControl.Shared.Native;
using NStandard;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Navigation;

namespace ColorControl.Shared.Contracts;

public enum PresetTriggerType
{
	[Description("None")]
	None = 0,
	[Description("Startup")]
	Startup = 1,
	[Description("Resume")]
	Resume = 2,
	[Description("Shutdown")]
	Shutdown = 3,
	[Description("Standby")]
	Standby = 4,
	[Description("Reserved5")]
	Reserved5 = 5,
	[Description("Process switch")]
	ProcessSwitch = 6,
	[Description("Screensaver start")]
	ScreensaverStart = 7,
	[Description("Screensaver stop")]
	ScreensaverStop = 8,
	[Description("Display Change")]
	DisplayChange = 9,
}

[Flags]
public enum PresetConditionType
{
	[Description("None")]
	None = 0,
	[Description("SDR")]
	SDR = 1,
	[Description("HDR")]
	HDR = 2,
	[Description("Full Screen")]
	FullScreen = 4,
	[Description("Notifications Disabled")]
	NotificationsDisabled = 8,
	[Description("G-SYNC Disabled")]
	GsyncDisabled = 16,
	[Description("G-SYNC Enabled")]
	GsyncEnabled = 32
}

public enum PresetOrder
{
	None = 0,
	[Description("Name")]
	ByName = 1,
	[Description("Last used")]
	ByLastUsed = 2
}

public class PresetTrigger
{
	public PresetTriggerType Trigger { get; set; }
	public List<string> IncludedProcesses { get; set; }
	public List<string> ExcludedProcesses { get; set; }
	public string ConnectedDisplaysRegex { get; set; }
	public PresetConditionType Conditions { get; set; }

	public string IncludedProcessesAsString
	{
		get => IncludedProcesses?.Join(", ");
		set
		{
			IncludedProcesses ??= new List<string>();
			Utils.ParseWords(IncludedProcesses, value);
		}
	}

	public string ExcludedProcessesAsString
	{
		get => ExcludedProcesses?.Join(", ");
		set
		{
			ExcludedProcesses ??= new List<string>();
			Utils.ParseWords(ExcludedProcesses, value);
		}
	}

	public PresetTrigger()
	{
		IncludedProcesses = new List<string>();
		ExcludedProcesses = new List<string>();
	}

	public bool TriggerActive(PresetTriggerContext context)
	{
		var active = Conditions == PresetConditionType.None ||
			 (!Conditions.HasFlag(PresetConditionType.SDR) || !context.IsHDRActive)
			 && (!Conditions.HasFlag(PresetConditionType.HDR) || context.IsHDRActive)
			 && (!Conditions.HasFlag(PresetConditionType.GsyncDisabled) || !context.IsGsyncActive)
			 && (!Conditions.HasFlag(PresetConditionType.GsyncEnabled) || context.IsGsyncActive);

        active = active && context.Triggers.Contains(Trigger);

        if (!ConnectedDisplaysRegex.IsNullOrWhiteSpace())
		{
			active = active && CCD.EnumerateDisplayDeviceNames().Any(s =>
            {
				var extendedDisplayName = CCD.GetDisplayInfo(s)?.ExtendedName;
                return extendedDisplayName is not null && Regex.IsMatch(extendedDisplayName, ConnectedDisplaysRegex);
            });
		}

		if (Trigger == PresetTriggerType.ProcessSwitch)
		{
			var allProcesses = IncludedProcesses.Contains("*");
			active = active && (allProcesses || (context.ChangedProcesses?.Any() ?? false));

			if (active)
			{
				var included = allProcesses || context.ChangedProcesses.Any(cp => IncludedProcesses.Any(ip => cp.ProcessName.NormEquals(ip)));
				var excluded = context.ChangedProcesses.Any(cp => ExcludedProcesses.Any(ep => cp.ProcessName.NormEquals(ep)));

				var screenSizeCheck = (!Conditions.HasFlag(PresetConditionType.FullScreen) && !context.ForegroundProcessIsFullScreen) ||
					(context.ForegroundProcess != null && context.ForegroundProcessIsFullScreen &&
						(allProcesses || IncludedProcesses.Any(ip => context.ForegroundProcess.ProcessName.NormEquals(ip))));

				var notificationsDisabledCheck = !Conditions.HasFlag(PresetConditionType.NotificationsDisabled) || context.IsNotificationDisabled;

				active = included && !excluded && screenSizeCheck && notificationsDisabledCheck;
			}
		}
		else if (Trigger == PresetTriggerType.ScreensaverStart || Trigger == PresetTriggerType.ScreensaverStop)
		{
			active &= (Trigger == PresetTriggerType.ScreensaverStart && context.ScreenSaverTransitionState == ScreenSaverTransitionState.Started) ||
					 (Trigger == PresetTriggerType.ScreensaverStop && context.ScreenSaverTransitionState == ScreenSaverTransitionState.Stopped);
		}

		return active;
	}

	public override string ToString()
	{
		if (Trigger == PresetTriggerType.None)
		{
			return string.Empty;
		}

		var text = $"On {Trigger.GetDescription()}";

		if (Trigger == PresetTriggerType.ProcessSwitch)
		{
			text += $"of {DisplayProcesses(IncludedProcesses)}{(ExcludedProcesses.Any() ? ", excluding " + DisplayProcesses(ExcludedProcesses) : string.Empty)}";
		}
		
		text += $"{(Conditions > 0 ? ", only in " + string.Join(", ", Utils.GetDescriptions<PresetConditionType>((int)Conditions)) : string.Empty)}";

		text += ConnectedDisplaysRegex.IsNullOrWhiteSpace() ? "" : $", connected display name matching {ConnectedDisplaysRegex}";

		return text;
	}

	public static string DisplayProcesses(IEnumerable<string> processes)
	{
		return processes.Contains("*") ? "all processes" : string.Join(", ", processes);
	}
}

public class PresetTriggerContext
{
	public IEnumerable<PresetTriggerType> Triggers { get; set; } = new[] { PresetTriggerType.ProcessSwitch };
	public bool IsHDRActive { get; set; }
	public bool IsGsyncActive { get; set; }
	public Process ForegroundProcess { get; set; }
	public bool ForegroundProcessIsFullScreen { get; set; }
	public List<Process> ChangedProcesses { get; set; }
	public bool IsNotificationDisabled { get; set; }
	public ScreenSaverTransitionState ScreenSaverTransitionState { get; set; }
}

public abstract class PresetBase
{
	public string name { get; set; }
	public string shortcut { get; set; }
	public List<PresetTrigger> Triggers { get; set; }
	public string Description { get; set; }
	public bool ShowInQuickAccess { get; set; }
	public DateTime? LastUsed { get; set; }
	public string IdOrName => name.IsNullOrEmpty() ? _id.ToString() : name;

	private int _id;

	protected PresetBase()
	{
		Triggers = new List<PresetTrigger>();
	}

	public int id
	{
		get
		{
			return _id;
		}
		set
		{
			if (value == 0 /*|| ids.Contains(value)*/)
			{
				value = GetNewId();
			}
			else
			{
				ids.Add(value);
			}
			_id = value;
		}
	}

	public void SetId(int id)
	{
		_id = id;
	}

	public static List<int> ids = new List<int>();

	public static int GetNewId()
	{
		int id;
		do
		{
			id = new Random().Next();
		}
		while (ids.Contains(id));

		ids.Add(id);

		return id;
	}

	public void SetCloneValues()
	{
		_id = 0;
		if (!string.IsNullOrEmpty(name))
		{
			name += " (copy)";
		}
	}

	public abstract List<string> GetDisplayValues(Config config = null);

	public virtual string GetTextForMenuItem()
	{
		return name;
	}

	public void UpdateTrigger(PresetTriggerType triggerType, PresetConditionType conditions, string includedProcesses = null, string excludedProcesses = null, string connectedDisplaysRegex = null)
	{
		if (!Triggers.Any())
		{
			Triggers.Add(new PresetTrigger());
		}

		var trigger = Triggers.First();

		trigger.Trigger = triggerType;
		trigger.Conditions = conditions;

		if (trigger.IncludedProcesses == null)
		{
			trigger.IncludedProcesses = new List<string>();
		}
		if (trigger.ExcludedProcesses == null)
		{
			trigger.ExcludedProcesses = new List<string>();
		}
		Utils.ParseWords(trigger.IncludedProcesses, includedProcesses);
		Utils.ParseWords(trigger.ExcludedProcesses, excludedProcesses);

		trigger.ConnectedDisplaysRegex = connectedDisplaysRegex;
	}

	public PresetTrigger AddTrigger()
	{
		if (!Triggers.Any())
		{
			Triggers.Add(new PresetTrigger());
		}

		return Triggers.First();
	}
}