using System.Collections.Generic;

namespace ColorControl
{
    public class StartUpParams
    {
        public const string RunningFromScheduledTaskParam = "--scheduled";
        public const string ActivateChromeFontFixParam = "--activate-chrome-font-fix";
        public const string DeactivateChromeFontFixParam = "--deactivate-chrome-font-fix";

        public bool RunningFromScheduledTask { get; private set; }
        public bool ActivateChromeFontFix { get; private set; }
        public bool DeactivateChromeFontFix { get; private set; }

        public static StartUpParams Parse(IEnumerable<string> args)
        {
            var settings = new StartUpParams();
            foreach (var arg in args)
            {
                switch (arg.ToLowerInvariant())
                {
                    case RunningFromScheduledTaskParam:
                        {
                            settings.RunningFromScheduledTask = true;
                            break;
                        }
                    case ActivateChromeFontFixParam:
                        {
                            settings.ActivateChromeFontFix = true;
                            break;
                        }
                    case DeactivateChromeFontFixParam:
                        {
                            settings.DeactivateChromeFontFix = true;
                            break;
                        }
                }
            }
            return settings;
        }
    }
}
