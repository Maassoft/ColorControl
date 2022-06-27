using Microsoft.Win32;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;

namespace ColorControl
{
    public class RestartDetector : IDisposable
    {
        public bool RestartDetected { get; private set; }
        public bool PowerOffDetected { get; private set; }

        private EventLogWatcher watcher = null;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly IEnumerable<string> RestartNames = new[] { "restart", "herstart", "reinicio" };
        private static readonly IEnumerable<string> PowerOffNames = new[] { "power off", "uitschakelen", "apagar" };

        public RestartDetector()
        {
            try
            {
                EventLogQuery subscriptionQuery = new EventLogQuery("System", PathType.LogName, "*[System[Provider[@Name='USER32'] and (EventID=1074)]]");

                watcher = new EventLogWatcher(subscriptionQuery);

                // Make the watcher listen to the EventRecordWritten
                // events.  When this event happens, the callback method
                // (EventLogEventRead) is called.
                watcher.EventRecordWritten +=
                    new EventHandler<EventRecordWrittenEventArgs>(
                        EventLogEventRead);

                // Activate the subscription
                watcher.Enabled = true;
            }
            catch (EventLogReadingException)
            {
            }
        }

        public bool IsRebootInProgress()
        {
            var registry = Registry.LocalMachine;
            var subKey = registry.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Component Based Servicing");
            return subKey.GetValueNames().Any(x => x.Equals("RebootInProgress"));
        }

        public void EventLogEventRead(object obj, EventRecordWrittenEventArgs arg)
        {
            RestartDetected = false;
            PowerOffDetected = false;
            try
            {
                Logger.Debug("Logging event: " + arg);
                // Make sure there was no error reading the event.
                if (arg.EventRecord != null)
                {
                    foreach (EventProperty x in ((EventLogRecord)arg.EventRecord).Properties) 
                    {
                        var strValue = x.Value.ToString();
                        Logger.Debug("Event value: " + strValue);
                        if (RestartNames.Any(n => n.Equals(strValue, StringComparison.OrdinalIgnoreCase)))
                        {
                            RestartDetected = true; 
                            break; 
                        }
                        if (PowerOffNames.Any(n => n.Equals(strValue, StringComparison.OrdinalIgnoreCase)))
                        {
                            PowerOffDetected = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public void Dispose()
        {
            // Stop listening to events
            if (watcher != null)
            {
                watcher.Enabled = false;
                watcher.Dispose();
            }
        }
    }
}
