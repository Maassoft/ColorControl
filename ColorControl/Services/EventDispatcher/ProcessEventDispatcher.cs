using ColorControl.Common;
using ColorControl.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading.Tasks;

namespace ColorControl.Services.EventDispatcher
{
    public class ProcessChangedEventArgs : EventArgs
    {
        public IList<Process> StartedProcesses { get; set; }
        public IList<Process> StoppedProcesses { get; set; }
        public IList<Process> RunningProcesses { get; set; }
        public bool IsNotificationDisabled { get; set; }
        public Process ForegroundProcess { get; set; }
        public bool ForegroundProcessIsFullScreen { get; set; }
        public string LastFullScreenProcessName = string.Empty;
        public bool StoppedFullScreen { get; set; }
        public bool IsScreenSaverActive { get; set; }
    }


    public class ProcessEventDispatcher : EventDispatcher<ProcessChangedEventArgs>
    {
        public const string Event_ProcessChanged = "ProcessChanged";
        public bool IsRunning { get; set; }

        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private ProcessChangedEventArgs MonitorContext { get; set; }

        public ProcessEventDispatcher()
        {
            IsRunning = true;

            var _ = CheckProcesses();
        }

        private async Task CheckProcesses()
        {
            await Task.Delay(2000);

            //var startWatch = new ManagementEventWatcher("SELECT * FROM Win32_ProcessStartTrace");
            //startWatch.EventArrived += startWatch_EventArrived;
            //startWatch.Start();

            MonitorContext ??= new ProcessChangedEventArgs();
            //Process[] lastProcesses = null;

            while (IsRunning)
            {
                await Task.Delay(1000);

                if (!HasHandlers(Event_ProcessChanged))
                {
                    continue;
                }

                try
                {
                    FillContext(MonitorContext);

                    // Ignore if own process has the foreground
                    if (MonitorContext.ForegroundProcess?.Id == Environment.ProcessId)
                    {
                        continue;
                    }

                    await DispatchEventAsync(Event_ProcessChanged, MonitorContext);
                }
                catch (Exception ex)
                {
                    Logger.Error("CheckProcesses: " + ex.ToLogString());
                }
            }
        }

        private void FillContext(ProcessChangedEventArgs context)
        {
            var processes = Process.GetProcesses();

            context.RunningProcesses = processes;

            context.IsNotificationDisabled = FormUtils.IsNotificationDisabled();

            var (processId, isFullScreen) = FormUtils.GetForegroundProcessIdAndIfFullScreen();

            if (processId > 0)
            {
                var process = context.RunningProcesses.FirstOrDefault(p => p.Id == processId);

                context.IsScreenSaverActive = process?.ProcessName?.Contains(".scr") == true;
                context.ForegroundProcess = process;
                context.ForegroundProcessIsFullScreen = isFullScreen;

                if (isFullScreen)
                {
                    context.LastFullScreenProcessName = context.ForegroundProcess.ProcessName;
                    //Logger.Debug($"Foreground fullscreen app detected: {context.ForegroundProcess.ProcessName}");
                }
            }
            else
            {
                context.StoppedFullScreen = context.ForegroundProcessIsFullScreen;
                context.ForegroundProcess = null;
                context.ForegroundProcessIsFullScreen = false;
                context.LastFullScreenProcessName = string.Empty;
            }
        }

        private void startWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            //Logger.Debug("Process started: " + e.NewEvent.Properties["ProcessName"].Value);
        }
    }
}
