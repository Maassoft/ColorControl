using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using ColorControl.Services.AMD;
using ColorControl.Services.LG;
using ColorControl.Services.NVIDIA;
using ColorControl.Services.Samsung;
using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts;
using ColorControl.Svc;
using Microsoft.Extensions.DependencyInjection;

namespace ColorControl
{
    public static class CommandLineHandler
    {
        public static async Task<bool> HandleStartupParams(StartUpParams startUpParams, Process existingProcess)
        {
            if (startUpParams.StartElevated)
            {
                await StartElevated();
                return true;
            }

            var useConsole = startUpParams.NoGui || existingProcess?.SessionId > 0;

            if (!useConsole)
            {
                return false;
            }

            var result = false;

            if (startUpParams.ExecuteHelp)
            {
                PrepareConsole(startUpParams);

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
                PrepareConsole(startUpParams);

                Console.WriteLine($"Executing LG-preset '{startUpParams.LgPresetName}'...");
                await LgService.ExecutePresetAsync(startUpParams.LgPresetName);

                Console.WriteLine("Done.");

                result = true;
            }

            if (startUpParams.ExecuteSamsungPreset)
            {
                PrepareConsole(startUpParams);

                Console.WriteLine($"Executing Samsung-preset '{startUpParams.SamsungPresetName}'...");
                await SamsungService.ExecutePresetAsync(startUpParams.SamsungPresetName);

                Console.WriteLine("Done.");

                result = true;
            }

            if (startUpParams.ExecuteNvidiaPreset)
            {
                PrepareConsole(startUpParams);

                Console.WriteLine($"Executing NVIDIA-preset '{startUpParams.NvidiaPresetIdOrName}'...");
                await NvService.ExecutePresetAsync(startUpParams.NvidiaPresetIdOrName);

                Console.WriteLine("Done.");

                result = true;
            }

            if (startUpParams.ExecuteAmdPreset)
            {
                PrepareConsole(startUpParams);

                Console.WriteLine($"Executing AMD-preset '{startUpParams.AmdPresetIdOrName}'...");
                await AmdService.ExecutePresetAsync(startUpParams.AmdPresetIdOrName);

                Console.WriteLine("Done.");

                result = true;
            }

            return result;
        }

        private static void PrepareConsole(StartUpParams startUpParams)
        {
            Utils.OpenConsole();
            startUpParams.NoGui = true;
        }

        private static async Task StartElevated()
        {
            try
            {
                var backgroundService = Program.ServiceProvider.GetRequiredService<ColorControlBackgroundService>();

                backgroundService.PipeName = PipeUtils.ElevatedPipe;

                await backgroundService.StartAndStopWithMutex();

                Application.Run();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while initializing elevated application: " + ex.ToLogString(Environment.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
