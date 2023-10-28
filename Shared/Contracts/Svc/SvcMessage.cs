using ColorControl.Shared.Contracts.NVIDIA;

namespace ColorControl.Shared.Contracts;

public enum SvcMessageType
{
    SetLgConfig = 1,
    GetLog = 2,
    ClearLog = 3,
    ExecuteRpc = 10,
    ExecuteRpcGeneric = 11,
    ExecuteRpcGenericTyped = 12,
    ExecuteUpdate = 20,
    RestartAfterUpdate = 21,
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

public class SvcRpcMessage : SvcMessage
{
    public SvcRpcMessage()
    {
        MessageType = SvcMessageType.ExecuteRpcGeneric;
    }

    private string _typeName;
    public string TypeName
    {
        get
        {
            if (_typeName == null)
            {
                _typeName = GetType().Name;
            }

            return _typeName;

        }
        set { _typeName = value; }
    }
    public string ServiceName { get; set; }
    public string MethodName { get; set; }
    public object[] Arguments { get; set; }
}

public class SvcRpcMessageTyped : SvcRpcMessage
{
    public SvcRpcMessageTyped() : base()
    {
        MessageType = SvcMessageType.ExecuteRpcGenericTyped;
    }
}

public class SvcRpcSetNvDriverSettingsMessage : SvcRpcMessageTyped
{
    public string ProfileName { get; set; }

    public List<KeyValuePair<uint, string>> DriverSettings { get; set; }
}

public class SvcRpcSetNvOverclockingMessage : SvcRpcMessageTyped
{
    public List<NvGpuOcSettings> OverclockingSettings { get; set; }
}

