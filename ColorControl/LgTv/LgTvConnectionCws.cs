using ColorControl.Shared.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;
using System.Net.Security;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LgTv
{
    public class SendMessageExceptionCws : Exception
    {
        public SendMessageExceptionCws(string message, Exception e) : base(message, e)
        {

        }
    }

    public delegate void IsConnectedDelegateCws(bool status);

    public class LgTvApiCoreCws : IAsyncDisposable
    {
        public bool ConnectionClosed { get; private set; }

        private ClientWebSocket _clientWebSocket;
        private int _commandCount;
        private Uri _uri;

        public event IsConnectedDelegateCws IsConnected;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<dynamic>> _tokens = new ConcurrentDictionary<string, TaskCompletionSource<dynamic>>();
        private readonly ConcurrentDictionary<string, Func<dynamic, bool>> _callbacks = new ConcurrentDictionary<string, Func<dynamic, bool>>();

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public async Task<bool> Connect(Uri uri, bool ignoreReceiver = false)
        {
            _uri = uri;

            try
            {
                ConnectionClosed = false;
                _commandCount = 0;
                CreateWebSocket();

                try
                {
                    await _clientWebSocket.ConnectAsync(uri, CancellationToken.None).WaitAsync(TimeSpan.FromMilliseconds(5000));
                    ConnectionClosed = false;

                    var _ = Receive(false);

                    IsConnected?.Invoke(true);
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
            _clientWebSocket.Options.RemoteCertificateValidationCallback += CertificationValidationCallback;
        }

        private bool CertificationValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors) => true;

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

        public async Task<dynamic> SendCommandAsync(string message)
        {
            var obj = JsonConvert.DeserializeObject<dynamic>(message);
            return await SendCommandAsync((string)obj.id, message);
        }

        private async Task<dynamic> SendCommandAsync(string id, string message)
        {
            try
            {
                var taskSource = new TaskCompletionSource<dynamic>();
                _tokens.TryAdd(id, taskSource);
                await SendMessageAsync(message);
                if (ConnectionClosed)
                {
                    throw new Exception("Connection closed");
                }

                return await taskSource.Task;
            }
            catch (Exception e)
            {
                throw new SendMessageExceptionCws("Can't send message", e);
            }
        }

        public async Task<dynamic> SendCommandAsync(RequestMessage message)
        {
            var rawMessage = new RawRequestMessage(message, ++_commandCount);
            var serialized = JsonConvert.SerializeObject(rawMessage, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            return await SendCommandAsync(rawMessage.Id, serialized);
        }

        public async Task<dynamic> SubscribeAsync(RequestMessage message, Func<dynamic, bool> callback)
        {
            var rawMessage = new RawRequestMessage(message, ++_commandCount);
            var serialized = JsonConvert.SerializeObject(rawMessage, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            _callbacks.TryAdd(rawMessage.Id, callback);

            return await SendCommandAsync(rawMessage.Id, serialized);
        }

        private async Task Receive(bool singleResponse)
        {
            var dataPerPacket = 8 * 1024; //8192
            var strBuilder = new StringBuilder();
            var buffer = new ArraySegment<byte>(new byte[dataPerPacket]);

            try
            {
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

                    HandleMessage(message);

                    if (singleResponse)
                    {
                        return;
                    }
                }
            }
            catch (Exception)
            {
                ConnectionClosed = true;
                Logger.Debug("Connection closed");
                IsConnected?.Invoke(false);
            }
        }

        public void HandleMessage(string message)
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<dynamic>(message);
                var id = (string)obj.id;
                var type = (string)obj.type;

                TaskCompletionSource<dynamic> taskCompletion;
                if (type == "registered")
                {
                    if (_tokens.TryRemove(id, out taskCompletion))
                    {
                        var key = (string)JObject.Parse(message)["payload"]["client-key"];
                        taskCompletion.TrySetResult(new { clientKey = key });
                    }

                }
                else if (_tokens.TryGetValue(id, out taskCompletion))
                {
                    if (id == "register_0")
                    {
                        //taskCompletion.TrySetResult(obj.payload);
                        return;
                    }
                    if (obj.type == "error")
                    {
                        taskCompletion.SetException(new Exception(obj.error?.ToString()));
                    }
                    //else if (args.Cancelled)
                    //{
                    //    taskSource.SetCanceled();
                    //}
                    taskCompletion.TrySetResult(obj.payload);

                    if (_callbacks.TryGetValue(id, out Func<dynamic, bool> callback))
                    {
                        try
                        {
                            callback(obj.payload);
                        }
                        catch (Exception callbackException)
                        {
                            Logger.Error($"Connection_MessageReceived: the callback threw an exception: {callbackException.ToLogString()}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"HandleMessage: status: {_clientWebSocket.State}, exception: {ex.ToLogString()}");

                SetExceptionOnAllTokens(ex);

                ConnectionClosed = true;
            }
        }

        public async ValueTask DisposeAsync()
        {
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

        private void SetExceptionOnAllTokens(Exception ex)
        {
            //Logger.Debug($"Tokens: {_tokens.Count}");

            foreach (var taskCompletion in _tokens.Values)
            {
                try
                {
                    if (taskCompletion.Task.Status != TaskStatus.RanToCompletion)
                    {
                        Logger.Debug("taskCompletion.Task.Status: " + taskCompletion.Task.Status);
                        taskCompletion.SetException(new Exception(ex.Message));
                    }
                }
                catch (Exception ex2)
                {
                    Logger.Error("SetExceptionOnAllTokens: " + ex2.Message);
                }
            }

            _tokens.Clear();
        }
    }
}
