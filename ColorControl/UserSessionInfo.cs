using Microsoft.Win32;

namespace ColorControl
{
    static class UserSessionInfo
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static bool UserLocalSession { get; private set; } = true;
        public static SessionSwitchReason LastSessionSwitchReason { get; private set; } = 0;

        public delegate void UserSessionSwitchHandler(bool toLocal);
        public static event UserSessionSwitchHandler UserSessionSwitch;

        public static void Install()
        {
            SystemEvents.SessionSwitch += SessionSwitchHandler;
        }

        public static void SessionSwitchHandler(object sender, SessionSwitchEventArgs evt)
        {
            if (evt.Reason == SessionSwitchReason.ConsoleDisconnect)
            {
                Logger.Debug("Detected a disconnect from the console");
                UserLocalSession = false;
                UserSessionSwitch(false);
            }
            else if (evt.Reason == SessionSwitchReason.ConsoleConnect)
            {
                Logger.Debug("Detected a connect to the console");
                if (!UserLocalSession)
                {
                    Logger.Debug("Session state switched to local");
                    UserLocalSession = true;
                    UserSessionSwitch(true);
                }
            }
            LastSessionSwitchReason = evt.Reason;
        }
    }
}
