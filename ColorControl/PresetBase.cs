using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ColorControl
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
            var active = Conditions == PresetConditionType.None || (Conditions.HasFlag(PresetConditionType.SDR) && !context.IsHDRActive) || (Conditions.HasFlag(PresetConditionType.HDR) && context.IsHDRActive);
            var allProcesses = IncludedProcesses.Contains("*");

            if (Trigger == PresetTriggerType.ProcessSwitch)
            {
                active = active && (allProcesses || (context.ChangedProcesses?.Any() ?? false));

                if (active)
                {
                    var included = allProcesses || context.ChangedProcesses.Any(cp => IncludedProcesses.Any(ip => cp.ProcessName.Equals(ip, StringComparison.OrdinalIgnoreCase)));
                    var excluded = context.ChangedProcesses.Any(cp => ExcludedProcesses.Any(ep => cp.ProcessName.Equals(ep, StringComparison.OrdinalIgnoreCase)));

                    var screenSizeCheck = (!Conditions.HasFlag(PresetConditionType.FullScreen) && !context.ForegroundProcessIsFullScreen) ||
                        (context.ForegroundProcess != null && context.ForegroundProcessIsFullScreen && 
                            (allProcesses || IncludedProcesses.Any(ip => context.ForegroundProcess.ProcessName.Equals(ip, StringComparison.OrdinalIgnoreCase))));

                    var notificationsDisabledCheck = !Conditions.HasFlag(PresetConditionType.NotificationsDisabled) || context.IsNotificationDisabled;

                    active = included && !excluded && screenSizeCheck && notificationsDisabledCheck;
                }
            }

            return active;
        }

        public override string ToString()
        {
            if (Trigger == PresetTriggerType.None)
            {
                return string.Empty;
            }

            return $"On {Trigger.GetDescription()} of {DisplayProcesses(IncludedProcesses)}{(ExcludedProcesses.Any() ? ", excluding " + DisplayProcesses(ExcludedProcesses) : string.Empty)}{(Conditions > 0 ? ", only in " + string.Join(", ", Utils.GetDescriptions<PresetConditionType>((int)Conditions)) : string.Empty)}";
        }

        public static string DisplayProcesses(IEnumerable<string> processes)
        {
            return processes.Contains("*") ? "all processes" : string.Join(", ", processes);
        }
    }

    class PresetTriggerContext
    {
        public bool IsHDRActive { get; set; }
        public Process ForegroundProcess { get; set; }
        public bool ForegroundProcessIsFullScreen { get; set; }
        public List<Process> ChangedProcesses { get; set; }
        public bool IsNotificationDisabled { get; set; }
    }

    internal abstract class PresetBase
    {
        public string name { get; set; }
        public string shortcut { get; set; }
        public List<PresetTrigger> Triggers { get; set; }
        public string Description { get; set; }

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

        public void UpdateTrigger(PresetTriggerType triggerType, PresetConditionType conditions, string includedProcesses, string excludedProcesses)
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