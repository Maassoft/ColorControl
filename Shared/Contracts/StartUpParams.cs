namespace ColorControl.Shared.Contracts
{
    public class StartUpParams
    {
        public const string RunningFromScheduledTaskParam = "--scheduled";
        public const string ExecuteHelpParam = "--help";
        public const string ExecuteNvidiaPresetParam = "--nvpreset";
        public const string ExecuteAmdPresetParam = "--amdpreset";
        public const string ExecuteLgPresetParam = "--lgpreset";
        public const string ExecuteSamsungPresetParam = "--sampreset";
        public const string NoDeviceRefreshParam = "--no-refresh";
        public const string NoGuiParam = "--nogui";
        public const string StartElevatedParam = "--elevated";

        public bool RunningFromScheduledTask { get; private set; }
        public bool ExecuteHelp { get; private set; }
        public bool ExecuteNvidiaPreset { get; private set; }
        public string NvidiaPresetIdOrName { get; private set; }
        public bool ExecuteAmdPreset { get; private set; }
        public string AmdPresetIdOrName { get; private set; }
        public bool ExecuteLgPreset { get; private set; }
        public string LgPresetName { get; private set; }
        public bool ExecuteSamsungPreset { get; private set; }
        public string SamsungPresetName { get; private set; }
        public bool NoDeviceRefresh { get; private set; }
        public bool NoGui { get; set; }
        public bool StartElevated { get; private set; }

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
                        case ExecuteSamsungPresetParam:
                            {
                                settings.SamsungPresetName = arg;
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
                    case ExecuteSamsungPresetParam:
                        {
                            settings.ExecuteSamsungPreset = true;
                            parseNameParam = ExecuteSamsungPresetParam;
                            break;
                        }
                    case NoGuiParam:
                        {
                            settings.NoGui = true;
                            break;
                        }
                    case StartElevatedParam:
                        {
                            settings.StartElevated = true;
                            break;
                        }
                    case NoDeviceRefreshParam:
                        {
                            settings.NoDeviceRefresh = true;
                            break;
                        }
                }
            }
            return settings;
        }
    }
}
