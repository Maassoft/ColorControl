using ColorControl.Shared.Contracts;
using Newtonsoft.Json;
using System.IO.Pipes;
using System.Security.Principal;

namespace ColorControl.Shared.Common
{
    public static class PipeUtils
    {
        public const string ServicePipe = "servicepipe";
        public const string ElevatedPipe = "elevatedpipe";

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static string SendMessage(string message, int timeout = 2000, string pipeName = ServicePipe)
        {
            var pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.None);
            try
            {
                pipeClient.Connect(timeout);
                try
                {
                    var ss = new StreamString(pipeClient);

                    ss.WriteString(message);

                    var result = ss.ReadString();

                    return result;
                }
                finally
                {
                    pipeClient.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "PipeUtils.SendMessage");

                return null;
            }
        }

        public static SvcResultMessage SendMessage(SvcMessage message, int timeout = 2000)
        {
            var messageJson = JsonConvert.SerializeObject(message);

            var resultJson = SendMessage(messageJson, timeout);

            if (resultJson == null)
            {
                return null;
            }

            var resultMessage = JsonConvert.DeserializeObject<SvcResultMessage>(resultJson);

            return resultMessage;
        }

        public static SvcResultMessage SendMessage(SvcMessageType messageType)
        {
            var message = new SvcMessage { MessageType = messageType };

            return SendMessage(message);
        }

        public static T SendRpcMessage<T>(SvcRpcMessage message, int timeout = 2000, string pipeName = ServicePipe)
        {
            var messageJson = JsonConvert.SerializeObject(message);

            var resultJson = SendMessage(messageJson, timeout, pipeName);

            if (resultJson == null)
            {
                return default;
            }

            var resultMessage = JsonConvert.DeserializeObject<SvcResultMessage>(resultJson);

            if (resultMessage?.Data == null)
            {
                return default;
            }

            var result = JsonConvert.DeserializeObject<T>(resultMessage.Data);

            return result;
        }
    }
}
