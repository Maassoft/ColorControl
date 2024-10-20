using Microsoft.Win32;

namespace ColorControl.Shared.EventDispatcher;

public class SessionSwitchDispatcher : EventDispatcher<SessionSwitchEventArgs>
{
    public const string Event_SessionSwitch = "SessionSwitch";

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public bool UserLocalSession { get; private set; } = true;
    public SessionSwitchReason LastSessionSwitchReason { get; private set; } = 0;

    public SessionSwitchDispatcher()
    {
        SystemEvents.SessionSwitch += SessionSwitchHandler;
    }

    public void SessionSwitchHandler(object sender, SessionSwitchEventArgs evt)
    {
        Logger.Debug($"Session switch: {evt.Reason}");

        if (evt.Reason == SessionSwitchReason.ConsoleDisconnect)
        {
            UserLocalSession = false;

            DispatchEvent(Event_SessionSwitch, evt);
        }
        else if (evt.Reason == SessionSwitchReason.ConsoleConnect)
        {
            if (!UserLocalSession)
            {
                Logger.Debug("Session state switched to local");
                UserLocalSession = true;
                DispatchEvent(Event_SessionSwitch, evt);
            }
        }
        else if (evt.Reason is SessionSwitchReason.SessionUnlock or SessionSwitchReason.SessionLock)
        {
            Task.Run(async () => await DispatchEventAsync(Event_SessionSwitch, evt));
        }

        LastSessionSwitchReason = evt.Reason;
    }
}
