using ColorControl.Common;
using Microsoft.Win32.TaskScheduler;
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
        public const string ExecuteSamsungPresetParam = "--sampreset";
        public const string NoGuiParam = "--nogui";
        public const string EnableAutoStartParam = "--enable-auto-start";
        public const string DisableAutoStartParam = "--disable-auto-start";
        public const string SetProcessAffinityParam = "--set-process-affinity";
        public const string SetProcessPriorityParam = "--set-process-priority";
        public const string StartElevatedParam = "--elevated";
        public const string SendWakeOnLanParam = "--send-wol";
        public const string InstallServiceParam = "--install-service";
        public const string UninstallServiceParam = "--uninstall-service";
        public const string StartServiceParam = "--start-service";
        public const string StopServiceParam = "--stop-service";

        public bool RunningFromScheduledTask { get; private set; }
        public bool ActivateChromeFontFix { get; private set; }
        public bool DeactivateChromeFontFix { get; private set; }
        public string ChromeFontFixApplicationDataFolder { get; private set; }
        public bool ExecuteHelp { get; private set; }
        public bool ExecuteNvidiaPreset { get; private set; }
        public string NvidiaPresetIdOrName { get; private set; }
        public bool ExecuteAmdPreset { get; private set; }
        public string AmdPresetIdOrName { get; private set; }
        public bool ExecuteLgPreset { get; private set; }
        public string LgPresetName { get; private set; }
        public bool ExecuteSamsungPreset { get; private set; }
        public string SamsungPresetName { get; private set; }
        public bool NoGui { get; set; }
        public bool EnableAutoStart { get; private set; }
        public TaskRunLevel AutoStartRunLevel { get; private set; } = TaskRunLevel.LUA;
        public bool DisableAutoStart { get; private set; }
        public bool SetProcessAffinity { get; private set; }
        public int ProcessId { get; private set; }
        public uint AffinityMask { get; private set; }
        public bool SetProcessPriority { get; private set; }
        public uint PriorityClass { get; private set; }
        public bool StartElevated { get; private set; }
        public bool SendWol { get; private set; }
        public string WolMacAddress { get; private set; }
        public string WolIpAddress { get; private set; }
        public bool InstallService { get; private set; }
        public bool UninstallService { get; private set; }
        public bool StartService { get; private set; }
        public bool StopService { get; private set; }

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
                        case ActivateChromeFontFixParam:
                        case DeactivateChromeFontFixParam:
                            {
                                settings.ChromeFontFixApplicationDataFolder = arg;
                                break;
                            }
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
                        case SetProcessAffinityParam:
                            {
                                if (settings.ProcessId == 0)
                                {
                                    settings.ProcessId = int.Parse(arg);
                                    continue;
                                }

                                settings.AffinityMask = uint.Parse(arg);
                                break;
                            }
                        case SetProcessPriorityParam:
                            {
                                if (settings.ProcessId == 0)
                                {
                                    settings.ProcessId = int.Parse(arg);
                                    continue;
                                }

                                settings.PriorityClass = uint.Parse(arg);
                                break;
                            }
                        case SendWakeOnLanParam:
                            {
                                if (settings.WolMacAddress == null)
                                {
                                    settings.WolMacAddress = arg;
                                    continue;
                                }

                                settings.WolIpAddress = arg;
                                break;
                            }
                        case EnableAutoStartParam:
                            {
                                settings.AutoStartRunLevel = (TaskRunLevel)Utils.ParseInt(arg, 0);
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
                            parseNameParam = ActivateChromeFontFixParam;
                            break;
                        }
                    case DeactivateChromeFontFixParam:
                        {
                            settings.DeactivateChromeFontFix = true;
                            parseNameParam = DeactivateChromeFontFixParam;
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
                    case EnableAutoStartParam:
                        {
                            settings.EnableAutoStart = true;
                            parseNameParam = EnableAutoStartParam;
                            break;
                        }
                    case DisableAutoStartParam:
                        {
                            settings.DisableAutoStart = true;
                            break;
                        }
                    case SetProcessAffinityParam:
                        {
                            settings.SetProcessAffinity = true;
                            parseNameParam = SetProcessAffinityParam;
                            break;
                        }
                    case SetProcessPriorityParam:
                        {
                            settings.SetProcessPriority = true;
                            parseNameParam = SetProcessPriorityParam;
                            break;
                        }
                    case StartElevatedParam:
                        {
                            settings.StartElevated = true;
                            break;
                        }
                    case SendWakeOnLanParam:
                        {
                            settings.SendWol = true;
                            parseNameParam = SendWakeOnLanParam;
                            break;
                        }
                    case InstallServiceParam:
                        {
                            settings.InstallService = true;
                            break;
                        }
                    case UninstallServiceParam:
                        {
                            settings.UninstallService = true;
                            break;
                        }
                    case StartServiceParam:
                        {
                            settings.StartService = true;
                            break;
                        }
                    case StopServiceParam:
                        {
                            settings.StopService = true;
                            break;
                        }
                }
            }
            return settings;
        }
    }
}
