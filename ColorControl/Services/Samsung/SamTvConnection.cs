using ColorControl.Shared.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Security.Cryptography.Certificates;
using Windows.Storage.Streams;

namespace ColorControl.Services.Samsung
{
    public class SendMessageException : Exception
    {
        public SendMessageException(string message, Exception e) : base(message, e)
        {

        }
    }

    public delegate void ConnectedDelegate(string token, bool disconnect);

    public class SamTvConnection : IDisposable
    {
        public static SynchronizationContext SyncContext { get; set; }

        public bool ConnectionClosed { get; private set; }
        public bool ClosedByDispose { get; private set; }

        private MessageWebSocket _connection;
        private DataWriter _messageWriter;
        private int _commandCount;
        private int _readTimeout;

        public event ConnectedDelegate Connected;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<dynamic>> _tokens = new ConcurrentDictionary<string, TaskCompletionSource<dynamic>>();
        private readonly ConcurrentDictionary<string, Func<dynamic, bool>> _callbacks = new ConcurrentDictionary<string, Func<dynamic, bool>>();

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public async Task<bool> Connect(Uri uri, int readTimeout = 5000)
        {
            _readTimeout = readTimeout;

            try
            {
                _connection?.Dispose();

                _commandCount = 0;

                // Not creating the web socket from main thread can cause a stackoverflow in combase32.dll
                if (Environment.CurrentManagedThreadId != 1)
                {
                    SyncContext.Send((_) => CreateWebSocket(), null);
                }
                else
                {
                    CreateWebSocket();
                }

                try
                {
                    await _connection.ConnectAsync(uri).AsTask().WaitAsync(TimeSpan.FromSeconds(5));

                    ConnectionClosed = false;

                    _messageWriter = new DataWriter(_connection.OutputStream);

                    return true;
                }
                catch (TimeoutException te)
                {
                    Logger.Error($"Connect to {uri}: {te.Message}");
                    return false;
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

                Logger.Error($"Connect to {uri}: {e.Message}");
                return false;
            }
        }

        private void CreateWebSocket()
        {
            _connection = new MessageWebSocket();
            _connection.Control.IgnorableServerCertificateErrors.Add(ChainValidationResult.Untrusted);
            _connection.Control.IgnorableServerCertificateErrors.Add(ChainValidationResult.InvalidName);
            _connection.Control.MessageType = SocketMessageType.Utf8;
            _connection.MessageReceived += Connection_MessageReceived;
            _connection.Closed += Connection_Closed;
        }

        public async Task SendMessageAsync(string message)
        {
            try
            {
                if (_messageWriter == null)
                {
                    throw new Exception("SendMessageAsync: no writer");
                }

                _messageWriter.WriteString(message);

                await _messageWriter.StoreAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(5));
            }
            catch (Exception e)
            {
                Logger.Error("SendMessageAsync: " + e.Message);
                ConnectionClosed = true;
                _messageWriter?.Dispose();
            }
        }

        private async Task<dynamic> SendCommandAsync(string id, string message, bool waitForAnswer = true)
        {
            try
            {
                if (waitForAnswer)
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
                else
                {
                    await SendMessageAsync(message);
                    if (ConnectionClosed)
                    {
                        throw new Exception("Connection closed");
                    }

                    return null;
                }

            }
            catch (Exception e)
            {
                throw new SendMessageException("Can't send message", e);
            }
        }

        public async Task<dynamic> SendCommandAsync(RequestMessage message, bool waitForAnswer = true)
        {
            var rawMessage = new RawRequestMessage(message, ++_commandCount);
            var serialized = JsonConvert.SerializeObject(rawMessage, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                //ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            return await SendCommandAsync(rawMessage.Id, serialized, waitForAnswer);
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
            var webSocket = Interlocked.Exchange(ref _connection, null);
            webSocket?.Dispose();
            ConnectionClosed = true;

            Logger.Debug("Connection closed");

            Connected?.Invoke(null, true);
        }

        private const string MS_CHANNEL_CONNECT_EVENT = "ms.channel.connect";
        private const string MS_CHANNEL_CLIENT_CONNECT_EVENT = "ms.channel.clientConnect";
        private const string MS_CHANNEL_CLIENT_DISCONNECT_EVENT = "ms.channel.clientDisconnect";
        private const string MS_CHANNEL_READY_EVENT = "ms.channel.ready";
        private const string MS_CHANNEL_UNAUTHORIZED = "ms.channel.unauthorized";
        private const string MS_ERROR_EVENT = "ms.error";

        private async void Connection_MessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {
            try
            {
                var task = new Task<DataReader>(args.GetDataReader);
                task.Start();
                var dataReader = await task.WaitAsync(TimeSpan.FromMilliseconds(_readTimeout));

                using (dataReader)
                {
                    dataReader.UnicodeEncoding = UnicodeEncoding.Utf8;
                    var message = dataReader.ReadString(dataReader.UnconsumedBufferLength);
                    var obj = JsonConvert.DeserializeObject<dynamic>(message);
                    var id = (string)obj.id;
                    var type = (string)obj.@event;

                    Logger.Debug($"Received message: {message}");

                    // Response:
                    // {"data":{"clients":[{"attributes":{"name":"c2Ftc3VuZ2N0bA=="},"connectTime":1685302606520,"deviceName":"c2Ftc3VuZ2N0bA==","id":"92e0cbc1-6f86-4013-9b9b-65a47afb2236","isHost":false}],"id":"92e0cbc1-6f86-4013-9b9b-65a47afb2236","token":"16224598"},"event":"ms.channel.connect"}
                    // Second response:
                    // {"data":{"clients":[{"attributes":{"name":"c2Ftc3VuZ2N0bA==","token":"16224598"},"connectTime":1685303321571,"deviceName":"c2Ftc3VuZ2N0bA==","id":"58589451-2ecf-44f0-9267-31983a5466","isHost":false}],"id":"58589451-2ecf-44f0-9267-31983a5466"},"event":"ms.channel.connect"}

                    //TaskCompletionSource<dynamic> taskCompletion;
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

                        //if (_tokens.TryRemove(id, out taskCompletion))
                        //{
                        //    var key = (string)JObject.Parse(message)["payload"]["client-key"];
                        //    taskCompletion.TrySetResult(new { clientKey = key });
                        //}

                    }
                    else
                    {

                    }
                    //else if (_tokens.TryGetValue(id, out taskCompletion))
                    //{
                    //    if (id == "register_0") return;
                    //    if (obj.type == "error")
                    //    {
                    //        taskCompletion.SetException(new Exception(obj.error?.ToString()));
                    //    }
                    //    //else if (args.Cancelled)
                    //    //{
                    //    //    taskSource.SetCanceled();
                    //    //}
                    //    taskCompletion.TrySetResult(obj.payload);

                    //    if (_callbacks.TryGetValue(id, out Func<dynamic, bool> callback))
                    //    {
                    //        try
                    //        {
                    //            callback(obj.payload);
                    //        }
                    //        catch (Exception callbackException)
                    //        {
                    //            Logger.Error($"Connection_MessageReceived: the callback threw an exception: {callbackException.ToLogString()}");
                    //        }
                    //    }
                    //}
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

        public void Close()
        {
            _connection?.Close(1000, "");
        }

        public void Dispose()
        {
            ClosedByDispose = true;
            Close();
            _connection?.Dispose();
        }
    }
}
