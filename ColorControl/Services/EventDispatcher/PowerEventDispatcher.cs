using Microsoft.Win32;

namespace ColorControl.Services.EventDispatcher
{
    public class PowerEventDispatcher : EventDispatcher<PowerModeChangedEventArgs>
    {
        public const string Event_Suspend = "Suspend";
        public const string Event_Resume = "Resume";

        public PowerEventDispatcher()
        {
            SystemEvents.PowerModeChanged += OnPowerModeChanged;
        }

        private void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Suspend)
            {
                DispatchEvent(Event_Suspend, e);
            }
            else if (e.Mode == PowerModes.Resume)
            {
                DispatchEvent(Event_Resume, e);
            }
        }
    }
}
