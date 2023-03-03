using ColorControl.Services.NVIDIA;
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
        RestoreNvidiaDriverSetting = 102,
        ApplyNvidiaOverclocking = 110,
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
        public SvcNvDriverSettingsMessage(SvcMessageType messageType = SvcMessageType.ApplyNvidiaDriverSettings)
        {
            MessageType = messageType;
        }

        public string ProfileName { get; set; }

        public List<KeyValuePair<uint, string>> DriverSettings { get; set; }
    }

    public class SvcNvOverclockingMessage : SvcMessage
    {
        public SvcNvOverclockingMessage(SvcMessageType messageType = SvcMessageType.ApplyNvidiaOverclocking)
        {
            MessageType = messageType;
        }

        public List<NvGpuOcSettings> OverclockingSettings { get; set; }
    }
}
