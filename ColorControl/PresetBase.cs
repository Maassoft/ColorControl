using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ColorControl
{
    [Flags]
    enum PresetTrigger
    {
        [Description("Startup")]
        Startup = 1,
        [Description("Resume")]
        Resume = 2,
        [Description("Shutdown")]
        Shutdown = 4,
        [Description("Standby")]
        Standby = 8,
        [Description("Screensaver")]
        Screensaver = 16,
        [Description("Process start")]
        ProcessStart = 32,
        [Description("Process exit")]
        ProcessExit = 64
    }

    internal abstract class PresetBase
    {
        public string name { get; set; }
        public string shortcut { get; set; }
        public PresetTrigger Triggers { get; set; }

        private int _id;

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
    }
}