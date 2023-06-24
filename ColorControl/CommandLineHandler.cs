using ColorControl.Common;
using ColorControl.Forms;
using ColorControl.Services.AMD;
using ColorControl.Services.LG;
using ColorControl.Services.NVIDIA;
using ColorControl.Services.Samsung;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorControl
{
    public static class CommandLineHandler
    {
        public static async Task<bool> HandleStartupParams(StartUpParams startUpParams, Process existingProcess)
        {
            if (startUpParams.ActivateChromeFontFix || startUpParams.DeactivateChromeFontFix)
            {
                Utils.InstallChromeFix(startUpParams.ActivateChromeFontFix, startUpParams.ChromeFontFixApplicationDataFolder);
                return true;
            }
            if (startUpParams.EnableAutoStart || startUpParams.DisableAutoStart)
            {
                Utils.RegisterTask(Program.TS_TASKNAME, startUpParams.EnableAutoStart, startUpParams.AutoStartRunLevel);
                return true;
            }
            if (startUpParams.SetProcessAffinity)
            {
                Utils.SetProcessAffinity(startUpParams.ProcessId, startUpParams.AffinityMask);
                return true;
            }
            if (startUpParams.SetProcessPriority)
            {
                Utils.SetProcessPriority(startUpParams.ProcessId, startUpParams.PriorityClass);
                return true;
            }
            if (startUpParams.StartElevated)
            {
                StartElevated();
                return true;
            }
            if (startUpParams.SendWol)
            {
                return WOL.WakeFunction(startUpParams.WolMacAddress, startUpParams.WolIpAddress);
            }
            if (startUpParams.InstallService)
            {
                Utils.InstallService();
                return true;
            }
            if (startUpParams.UninstallService)
            {
                Utils.UninstallService();
                return true;
            }
            if (startUpParams.StartService)
            {
                Utils.StartService();
                return true;
            }
            if (startUpParams.StopService)
            {
                Utils.StopService();
                return true;
            }

            var useConsole = startUpParams.NoGui || existingProcess != null;

            if (!useConsole)
            {
                return false;
            }

            startUpParams.NoGui = true;

            var result = false;

            if (startUpParams.ExecuteHelp)
            {
                Utils.OpenConsole();

                Console.WriteLine();
                Console.WriteLine($@"ColorControl CLI {Application.ProductVersion}
Syntax  : ColorControl command options
Commands:
--nvpreset  <preset name or id>: execute NVIDIA-preset
--amdpreset <preset name or id>: execute AMD-preset
--lgpreset  <preset name>      : execute LG-preset
--sampreset <preset name>      : execute Samsung-preset
--help                         : displays this help info
Options:
--nogui     : starts command from the command line and will not open GUI (is forced when GUI is already running)
--no-refresh: when using LG or Samsung-preset: skip refreshing devices (speeds up executing preset)");

                result = true;
            }

            if (startUpParams.ExecuteLgPreset)
            {
                Utils.OpenConsole();

                Console.WriteLine($"Executing LG-preset '{startUpParams.LgPresetName}'...");
                await LgService.ExecutePresetAsync(startUpParams.LgPresetName);

                Console.WriteLine("Done.");

                result = true;
            }

            if (startUpParams.ExecuteSamsungPreset)
            {
                Utils.OpenConsole();

                Console.WriteLine($"Executing Samsung-preset '{startUpParams.SamsungPresetName}'...");
                await SamsungService.ExecutePresetAsync(startUpParams.SamsungPresetName);

                Console.WriteLine("Done.");

                result = true;
            }

            if (startUpParams.ExecuteNvidiaPreset)
            {
                Utils.OpenConsole();

                Console.WriteLine($"Executing NVIDIA-preset '{startUpParams.NvidiaPresetIdOrName}'...");
                await NvService.ExecutePresetAsync(startUpParams.NvidiaPresetIdOrName);

                Console.WriteLine("Done.");

                result = true;
            }

            if (startUpParams.ExecuteAmdPreset)
            {
                Utils.OpenConsole();

                Console.WriteLine($"Executing AMD-preset '{startUpParams.AmdPresetIdOrName}'...");
                await AmdService.ExecutePresetAsync(startUpParams.AmdPresetIdOrName);

                Console.WriteLine("Done.");

                result = true;
            }

            return result;
        }

        private static void StartElevated()
        {
            try
            {
                Application.Run(new ElevatedForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while initializing elevated application: " + ex.ToLogString(Environment.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
