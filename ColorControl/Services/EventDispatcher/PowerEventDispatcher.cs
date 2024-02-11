using Microsoft.Win32;
using NWin32;
using System;
using System.Threading.Tasks;

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
        public const string Event_Startup = "Startup";
        public const string Event_Shutdown = "Shutdown";
        public const string Event_MonitorOff = "MonitorOff";
        public const string Event_MonitorOn = "MonitorOn";

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public PowerEventDispatcher()
        {
            SystemEvents.PowerModeChanged += OnPowerModeChanged;
        }

        public void SendEvent(string eventName)
        {
            var state = eventName switch
            {
                Event_Shutdown => PowerOnOffState.ShutDown,
                Event_Startup => PowerOnOffState.StartUp,
                Event_MonitorOff => PowerOnOffState.MonitorOff,
                Event_MonitorOn => PowerOnOffState.MonitorOn,
                _ => PowerOnOffState.None
            };

            if (state == PowerOnOffState.None)
            {
                return;
            }

            if (state == PowerOnOffState.ShutDown)
            {
                DispatchEventWithExecutionState(Event_Shutdown, PowerOnOffState.ShutDown);
                return;
            }

            DispatchEvent(eventName, new PowerStateChangedEventArgs(state));
        }

        public async Task SendEventAsync(string eventName)
        {
            var state = eventName switch
            {
                Event_Startup => PowerOnOffState.StartUp,
                _ => PowerOnOffState.None
            };

            await DispatchEventAsync(eventName, new PowerStateChangedEventArgs(state));
        }

        private async void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Suspend)
            {
                DispatchEventWithExecutionState(Event_Suspend, PowerOnOffState.StandBy);
            }
            else if (e.Mode == PowerModes.Resume)
            {
                await DispatchEventAsync(Event_Resume, new PowerStateChangedEventArgs(PowerOnOffState.Resume));
                DispatchEvent(Event_Resume, new PowerStateChangedEventArgs(PowerOnOffState.Resume));
            }
        }

        private void DispatchEventWithExecutionState(string eventName, PowerOnOffState powerState)
        {
            var error = NativeMethods.SetThreadExecutionState(NativeConstants.ES_CONTINUOUS | NativeConstants.ES_SYSTEM_REQUIRED | NativeConstants.ES_AWAYMODE_REQUIRED);
            try
            {
                Logger.Debug($"SetThreadExecutionState: {error}, Thread#: {Environment.CurrentManagedThreadId}");
                DispatchEvent(eventName, new PowerStateChangedEventArgs(powerState));
            }
            finally
            {
                var error2 = NativeMethods.SetThreadExecutionState(NativeConstants.ES_CONTINUOUS);
                Logger.Debug($"SetThreadExecutionState (reset): {error2}, Thread#: {Environment.CurrentManagedThreadId}");
            }
        }
    }
}
