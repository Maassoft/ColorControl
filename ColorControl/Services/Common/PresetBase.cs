using ColorControl.Services.EventDispatcher;
using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts;
using NStandard;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace ColorControl.Services.Common
{
    enum PresetTriggerType
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
    }

    [Flags]
    enum PresetConditionType
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

    class PresetTrigger
    {
        public PresetTriggerType Trigger { get; set; }
        public List<string> IncludedProcesses { get; set; }
        public List<string> ExcludedProcesses { get; set; }
        public PresetConditionType Conditions { get; set; }

        public PresetTrigger()
        {
            IncludedProcesses = new List<string>();
            ExcludedProcesses = new List<string>();
        }

        public bool TriggerActive(PresetTriggerContext context)
        {
            var active = Conditions == PresetConditionType.None ||
                 (Conditions.HasFlag(PresetConditionType.SDR) ? !context.IsHDRActive : true) && (Conditions.HasFlag(PresetConditionType.HDR) ? context.IsHDRActive : true) &&
                 (Conditions.HasFlag(PresetConditionType.GsyncDisabled) ? !context.IsGsyncActive : true) && (Conditions.HasFlag(PresetConditionType.GsyncEnabled) ? context.IsGsyncActive : true);

            if (Trigger == PresetTriggerType.ProcessSwitch && context.Triggers.Contains(Trigger))
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
            else if ((Trigger == PresetTriggerType.ScreensaverStart || Trigger == PresetTriggerType.ScreensaverStop) && context.Triggers.Contains(Trigger))
            {
                active = active && (Trigger == PresetTriggerType.ScreensaverStart && context.ScreenSaverTransitionState == ScreenSaverTransitionState.Started ||
                         Trigger == PresetTriggerType.ScreensaverStop && context.ScreenSaverTransitionState == ScreenSaverTransitionState.Stopped);
            }
            else
            {
                active = context.Triggers.Contains(Trigger);
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
            return text += $"{(Conditions > 0 ? ", only in " + string.Join(", ", Utils.GetDescriptions<PresetConditionType>((int)Conditions)) : string.Empty)}";
        }

        public static string DisplayProcesses(IEnumerable<string> processes)
        {
            return processes.Contains("*") ? "all processes" : string.Join(", ", processes);
        }
    }

    class PresetTriggerContext
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

    internal abstract class PresetBase
    {
        public string name { get; set; }
        public string shortcut { get; set; }
        public List<PresetTrigger> Triggers { get; set; }
        public string Description { get; set; }
        public bool ShowInQuickAccess { get; set; }
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
                if (value == 0 || ids.Contains(value))
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

        public abstract List<string> GetDisplayValues(Config config = null);

        public virtual string GetTextForMenuItem()
        {
            return name;
        }

        public void UpdateTrigger(PresetTriggerType triggerType, PresetConditionType conditions, string includedProcesses = null, string excludedProcesses = null)
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
}