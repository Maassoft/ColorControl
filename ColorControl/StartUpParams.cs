using System.Collections.Generic;

namespace ColorControl
{
    public class StartUpParams
    {
        public const string RunningFromScheduledTaskParam = "--scheduled";
        public const string ActivateChromeFontFixParam = "--activate-chrome-font-fix";
        public const string DeactivateChromeFontFixParam = "--deactivate-chrome-font-fix";
        public const string ExecuteHelpParam = "--help";
        public const string ExecuteNvidiaPresetParam = "--nvpreset";
        public const string ExecuteAmdPresetParam = "--amdpreset";
        public const string ExecuteLgPresetParam = "--lgpreset";
        public const string NoGuiParam = "--nogui";
        public const string EnableAutoStartParam = "--enable-auto-start";
        public const string DisableAutoStartParam = "--disable-auto-start";

        public bool RunningFromScheduledTask { get; private set; }
        public bool ActivateChromeFontFix { get; private set; }
        public bool DeactivateChromeFontFix { get; private set; }
        public bool ExecuteHelp { get; private set; }
        public bool ExecuteNvidiaPreset { get; private set; }
        public string NvidiaPresetIdOrName { get; private set; }
        public bool ExecuteAmdPreset { get; private set; }
        public string AmdPresetIdOrName { get; private set; }
        public bool ExecuteLgPreset { get; private set; }
        public string LgPresetName { get; private set; }
        public bool NoGui { get; set; }
        public bool EnableAutoStart { get; private set; }
        public bool DisableAutoStart { get; private set; }

        public static StartUpParams Parse(IEnumerable<string> args)
        {
            var settings = new StartUpParams();
            var parseNameParam = string.Empty;
            foreach (var arg in args)
            {
                if (!string.IsNullOrEmpty(parseNameParam))
                {
                    switch (parseNameParam)
                    {
                        case ExecuteNvidiaPresetParam:
                            {
                                settings.NvidiaPresetIdOrName = arg;
                                break;
                            }
                        case ExecuteAmdPresetParam:
                            {
                                settings.AmdPresetIdOrName = arg;
                                break;
                            }
                        case ExecuteLgPresetParam:
                            {
                                settings.LgPresetName = arg;
                                break;
                            }
                    }

                    parseNameParam = string.Empty;
                    continue;
                }

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
                    case ExecuteHelpParam:
                        {
                            settings.ExecuteHelp = true;
                            settings.NoGui = true;
                            break;
                        }
                    case ExecuteNvidiaPresetParam:
                        {
                            settings.ExecuteNvidiaPreset = true;
                            parseNameParam = ExecuteNvidiaPresetParam;
                            break;
                        }
                    case ExecuteAmdPresetParam:
                        {
                            settings.ExecuteAmdPreset = true;
                            parseNameParam = ExecuteAmdPresetParam;
                            break;
                        }
                    case ExecuteLgPresetParam:
                        {
                            settings.ExecuteLgPreset = true;
                            parseNameParam = ExecuteLgPresetParam;
                            break;
                        }
                    case NoGuiParam:
                        {
                            settings.NoGui = true;
                            break;
                        }
                    case EnableAutoStartParam:
                        {
                            settings.EnableAutoStart = true;
                            break;
                        }
                    case DisableAutoStartParam:
                        {
                            settings.DisableAutoStart = true;
                            break;
                        }
                }
            }
            return settings;
        }
    }
}
