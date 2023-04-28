using ColorControl.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Security.Cryptography.Certificates;
using Windows.Storage.Streams;

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
        public static SynchronizationContext SyncContext { get; set; }

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
                //Logger.Debug($"Connecting from thread: {Environment.CurrentManagedThreadId}");

                _connection?.Dispose();

                ConnectionClosed = false;
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

                    _messageWriter = new DataWriter(_connection.OutputStream);

                    IsConnected?.Invoke(true);
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

                return await taskSource.Task;
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
                var task = new Task<DataReader>(new Func<DataReader>(args.GetDataReader));
                task.Start();
                var result = task.Wait(5000);

                var dr = result ? task.Result : null;
                if (!result)
                {
                    throw new Exception("Timeout while reading response, possible disconnect");
                }

                using (dr)
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
            _connection?.Close(1, "");
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
