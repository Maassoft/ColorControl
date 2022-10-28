namespace ColorControl.Svc
{
    public enum SvcMessageType
    {
        SetLgConfig = 1,
        GetLog = 2,
        ClearLog = 3,
        ExecuteRpc = 10,
    }

    public class SvcMessage
    {
        public SvcMessageType MessageType { get; set; }

        public string Data { get; set; }
    }
}
