using Microsoft.Win32;
using System;

namespace ColorControl.Services.EventDispatcher
{
    public enum PowerOnOffState
    {
        None = 0,
        StartUp = 1,
        Resume = 2,
        ScreenSaver = 3,
        ShutDown = 4,
        StandBy = 5,
        MonitorOff = 6,
        MonitorOn = 7
    }

    internal enum WindowsMonitorPowerSetting
    {
        Off = 0,
        On = 1,
        Dimmed = 2
    }

    public class PowerStateChangedEventArgs : EventArgs
    {
        public PowerOnOffState State { get; private set; }

        public PowerStateChangedEventArgs(PowerOnOffState state)
        {
            State = state;
        }
    }

    public class PowerEventDispatcher : EventDispatcher<PowerStateChangedEventArgs>
    {
        public const string Event_Suspend = "Suspend";
        public const string Event_Resume = "Resume";
        public const string Event_Shutdown = "Shutdown";
        public const string Event_MonitorOff = "MonitorOff";
        public const string Event_MonitorOn = "MonitorOn";

        public PowerEventDispatcher()
        {
            SystemEvents.PowerModeChanged += OnPowerModeChanged;
        }

        public void SendEvent(string eventName)
        {
            var state = eventName switch
            {
                Event_Shutdown => PowerOnOffState.ShutDown,
                Event_MonitorOff => PowerOnOffState.MonitorOff,
                Event_MonitorOn => PowerOnOffState.MonitorOn,
                _ => PowerOnOffState.None
            };

            if (state == PowerOnOffState.None)
            {
                return;
            }

            DispatchEvent(eventName, new PowerStateChangedEventArgs(state));
        }

        private void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Suspend)
            {
                DispatchEvent(Event_Suspend, new PowerStateChangedEventArgs(PowerOnOffState.StandBy));
            }
            else if (e.Mode == PowerModes.Resume)
            {
                DispatchEvent(Event_Resume, new PowerStateChangedEventArgs(PowerOnOffState.Resume));
            }
        }
    }
}
