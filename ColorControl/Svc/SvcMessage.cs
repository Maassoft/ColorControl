using System.Collections.Generic;

namespace ColorControl.Svc
{
    public enum SvcMessageType
    {
        SetLgConfig = 1,
        GetLog = 2,
        ClearLog = 3,
        ExecuteRpc = 10,
        ExecuteUpdate = 20,
        RestartAfterUpdate = 21,
        ApplyNvidiaDriverSettings = 101,
    }

    public class SvcMessage
    {
        public SvcMessageType MessageType { get; set; }

        public string Data { get; set; }
    }

    public class SvcInstallUpdateMessage : SvcMessage
    {
        public SvcInstallUpdateMessage()
        {
            MessageType = SvcMessageType.ExecuteUpdate;
        }

        public string DownloadUrl { get; set; }
        public string ClientPath { get; set; }
    }

    public class SvcNvDriverSettingsMessage : SvcMessage
    {
        public SvcNvDriverSettingsMessage()
        {
            MessageType = SvcMessageType.ApplyNvidiaDriverSettings;
        }

        public List<KeyValuePair<uint, string>> DriverSettings { get; set; }
    }
}
