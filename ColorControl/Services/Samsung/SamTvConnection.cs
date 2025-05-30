using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ColorControl.Services.Samsung
{
    public class SendMessageException : Exception
    {
        public SendMessageException(string message, Exception e) : base(message, e)
        {

        }
    }

    public delegate void ConnectedDelegate(string token, bool disconnect);

    public class SamTvConnection : IAsyncDisposable
    {
        public bool? ConnectionClosed { get; private set; }
        public bool ClosedByDispose { get; private set; }

        private ClientWebSocket _clientWebSocket;
        private int _commandCount;
        private Uri _uri;

        public event ConnectedDelegate Connected;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public async Task<bool> Connect(Uri uri, int connectTimeout = 5000)
        {
            _uri = uri;

            try
            {
                _clientWebSocket?.Dispose();
                _commandCount = 0;

                CreateWebSocket();

                try
                {
                    Logger.Debug($"Connecting to {uri}");

                    var source = new CancellationTokenSource();
                    source.CancelAfter(connectTimeout);

                    await _clientWebSocket.ConnectAsync(uri, source.Token);

                    Logger.Debug($"Connected to {uri}");
                    ConnectionClosed = false;

                    await Receive(true);

                    Logger.Debug("Received");

                    return true;
                }
                catch (TimeoutException te)
                {
                    Logger.Error($"Timeout while connecting to {uri}: {te.Message}");
                    return false;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Connect to {uri}: {e.Message}");
                return false;
            }
        }

        private void CreateWebSocket()
        {
            _clientWebSocket = new ClientWebSocket();
            _clientWebSocket.Options.RemoteCertificateValidationCallback += (_, _, _, _) => true;
        }

        public async Task SendMessageAsync(string message, bool reconnect = true)
        {
            try
            {
                if (_clientWebSocket?.State != WebSocketState.Open)
                {
                    throw new InvalidOperationException("Socket not open");
                }
                var bytes = UTF8Encoding.UTF8.GetBytes(message);
                var data = bytes.AsMemory();

                await _clientWebSocket.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (WebSocketException)
            {
                if (reconnect && await Connect(_uri))
                {
                    await SendMessageAsync(message, false);
                }
            }
            catch (Exception e)
            {
                Logger.Error("SendMessageAsync: " + e.Message);
                ConnectionClosed = true;
            }
        }

        private async Task<dynamic> SendCommandAsync(string message)
        {
            try
            {
                await SendMessageAsync(message);
                if (ConnectionClosed == true)
                {
                    throw new Exception("Connection closed");
                }

                return null;
            }
            catch (Exception e)
            {
                throw new SendMessageException("Can't send message", e);
            }
        }

        public async Task<dynamic> SendCommandAsync(RequestMessage message)
        {
            var rawMessage = new RawRequestMessage(message, ++_commandCount);
            var serialized = JsonConvert.SerializeObject(rawMessage, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                //ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            return await SendCommandAsync(serialized);
        }

        private const string MS_CHANNEL_CONNECT_EVENT = "ms.channel.connect";
        private const string MS_CHANNEL_CLIENT_CONNECT_EVENT = "ms.channel.clientConnect";
        private const string MS_CHANNEL_CLIENT_DISCONNECT_EVENT = "ms.channel.clientDisconnect";
        private const string MS_CHANNEL_READY_EVENT = "ms.channel.ready";
        private const string MS_CHANNEL_UNAUTHORIZED = "ms.channel.unauthorized";
        private const string MS_ERROR_EVENT = "ms.error";

        private async Task Receive(bool singleResponse)
        {
            var dataPerPacket = 8 * 1024; //8192
            var strBuilder = new StringBuilder();
            var buffer = new ArraySegment<byte>(new byte[dataPerPacket]);

            while (_clientWebSocket?.State == WebSocketState.Open)
            {
                var receiveResult = await _clientWebSocket.ReceiveAsync(buffer, CancellationToken.None);

                strBuilder.Append(Encoding.UTF8.GetString(buffer.Array, buffer.Offset, receiveResult.Count));
                //keep doing until end of message
                while (!receiveResult.EndOfMessage)
                {
                    receiveResult = await _clientWebSocket.ReceiveAsync(buffer, CancellationToken.None);
                    strBuilder.Append(Encoding.UTF8.GetString(buffer.Array, buffer.Offset, receiveResult.Count));
                }

                var message = strBuilder.ToString();
                strBuilder.Clear();

                var obj = JsonConvert.DeserializeObject<dynamic>(message);
                var id = (string)obj.id;
                var type = (string)obj.@event;

                // Response:
                // {"data":{"clients":[{"attributes":{"name":"c2Ftc3VuZ2N0bA=="},"connectTime":1685302606520,"deviceName":"c2Ftc3VuZ2N0bA==","id":"92e0cbc1-6f86-4013-9b9b-65a47afb2236","isHost":false}],"id":"92e0cbc1-6f86-4013-9b9b-65a47afb2236","token":"16224598"},"event":"ms.channel.connect"}
                // Second response:
                // {"data":{"clients":[{"attributes":{"name":"c2Ftc3VuZ2N0bA==","token":"16224598"},"connectTime":1685303321571,"deviceName":"c2Ftc3VuZ2N0bA==","id":"58589451-2ecf-44f0-9267-31983a5466","isHost":false}],"id":"58589451-2ecf-44f0-9267-31983a5466"},"event":"ms.channel.connect"}

                if (type == MS_CHANNEL_CONNECT_EVENT)
                {
                    var token = (string)obj.data?.token;

                    if (token == null)
                    {
                        token = (string)obj.data?.clients[0]?.attributes?.token;
                    }

                    if (token != null)
                    {
                        Connected?.Invoke(token, false);
                    }
                }

                if (singleResponse)
                {
                    return;
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            ClosedByDispose = true;

            if (_clientWebSocket != null)
            {
                if (_clientWebSocket.State == WebSocketState.Open)
                {
                    try
                    {
                        await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                    }
                    catch { }
                }
                _clientWebSocket?.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}
