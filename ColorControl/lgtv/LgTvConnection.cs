using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using ColorControl;

namespace LgTv
{
    public class SendMessageException : Exception
    {
        public SendMessageException(string message, Exception e) : base(message, e)
        {

        }
    }
   
    public class LgTvApiCore : IDisposable
    {
        public bool ConnectionClosed { get; private set; }

        private MessageWebSocket _connection;
        private DataWriter _messageWriter;
        private int _commandCount;

        public event IsConnectedDelegate IsConnected;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<dynamic>> _tokens = new ConcurrentDictionary<string, TaskCompletionSource<dynamic>>();
        private readonly ConcurrentDictionary<string, Func<dynamic, bool>> _callbacks = new ConcurrentDictionary<string, Func<dynamic, bool>>();

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public async Task<bool> Connect(Uri uri, bool ignoreReceiver = false)
        {
            try
            {
                ConnectionClosed = false;
                _commandCount = 0;
                _connection = new MessageWebSocket();
                _connection.Control.MessageType = SocketMessageType.Utf8;
              //  if(ignoreReceiver==false)
                    _connection.MessageReceived += Connection_MessageReceived;

                _connection.Closed += Connection_Closed;
                using (var cancellationTokenSource = new CancellationTokenSource(5000))
                {
                    var connectTask = _connection.ConnectAsync(uri).AsTask(cancellationTokenSource.Token);
                    var result = await connectTask.ContinueWith((antecedent) =>
                    {
                        if (antecedent.Status == TaskStatus.RanToCompletion)
                        {
                            // connectTask ran to completion, so we know that the MessageWebSocket is connected.
                            // Add additional code here to use the MessageWebSocket.
                            IsConnected?.Invoke(true);

                            _messageWriter = new DataWriter(_connection.OutputStream);
                            return true;
                        }
                        else
                        {
                            // connectTask timed out, or faulted.
                            return false;
                        }
                    });

                    return result;
                }
            }
            catch (Exception e)
            {
                switch (SocketError.GetStatus(e.HResult))
                {
                    case SocketErrorStatus.HostNotFound:
                        // Handle HostNotFound Error
                        break;
                    default:
                        // Handle Unknown Error
                        break;
                }
                return false;
            }
        }

        public async Task SendMessageAsync(string message)
        {
            try
            {
                _messageWriter.WriteString(message);

                using (var cancellationTokenSource = new CancellationTokenSource(5000))
                {
                    await _messageWriter.StoreAsync().AsTask(cancellationTokenSource.Token).ContinueWith((antecedent) =>
                    {
                        if (antecedent.Status != TaskStatus.RanToCompletion)
                        {
                            throw new Exception("SendMessageAsync cancelled due to timeout");
                        }
                    });
                }
            }
            catch (Exception e)
            {
                Logger.Error("SendMessageAsync: " + e.Message);
                ConnectionClosed = true;
                _messageWriter?.Dispose();
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
                var result = await taskSource.Task.ConfigureAwait(false);

                return result;
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

        private void Connection_Closed(IWebSocket sender, WebSocketClosedEventArgs args)
        {
            MessageWebSocket webSocket = Interlocked.Exchange(ref _connection, null);
            webSocket?.Dispose();
            ConnectionClosed = true;
        }

        private void Connection_MessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {
            try
            {
                using (var dr = args.GetDataReader())
                {
                    dr.UnicodeEncoding = UnicodeEncoding.Utf8;
                    var message = dr.ReadString(dr.UnconsumedBufferLength);
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
                        if (id == "register_0") return;
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
                            callback(obj.payload);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var status = WebSocketError.GetStatus(ex.GetBaseException().HResult);
                Logger.Error($"Connection_MessageReceived: status: {status}, exception: {ex.ToLogString()}");

                SetExceptionOnAllTokens(ex);

                ConnectionClosed = true;
                _messageWriter?.Dispose();
            }
        }

        private void SetExceptionOnAllTokens(Exception ex)
        {
            //Logger.Debug($"Tokens: {_tokens.Count}");

            foreach (var taskCompletion in _tokens.Values)
            {
                try
                {
                    if (taskCompletion.Task?.Status == TaskStatus.Running)
                    {
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

        public void Close()
        {
            _connection?.Close(1, "");
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
