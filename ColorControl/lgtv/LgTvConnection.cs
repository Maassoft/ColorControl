using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.Web;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;

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
                await _connection.ConnectAsync(uri);

                IsConnected?.Invoke(true);

                _messageWriter = new DataWriter(_connection.OutputStream);
                return true;
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

        public async void SendMessageAsync(string message)
        {
            try
            {
                _messageWriter.WriteString(message);
                await _messageWriter.StoreAsync();
            }
            catch (Exception e)
            {
                Logger.Error("SendMessageAsync: " + e.Message);
                ConnectionClosed = true;
                _messageWriter?.Dispose();
            }
        }

        public Task<dynamic> SendCommandAsync(string message)
        {
            var obj = JsonConvert.DeserializeObject<dynamic>(message);
            return SendCommandAsync((string)obj.id, message);
        }

        private Task<dynamic> SendCommandAsync(string id, string message)
        {
            try
            {
                var taskSource = new TaskCompletionSource<dynamic>();
                _tokens.TryAdd(id, taskSource);
                SendMessageAsync(message);
                if (ConnectionClosed)
                {
                    throw new Exception("Connection closed");
                }
                return taskSource.Task;
            }
            catch (Exception e)
            {
                throw new SendMessageException("Can't send message", e);
            }
        }

        public Task<dynamic> SendCommandAsync(RequestMessage message)
        {
            var rawMessage = new RawRequestMessage(message, ++_commandCount);
            var serialized = JsonConvert.SerializeObject(rawMessage, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            return SendCommandAsync(rawMessage.Id, serialized);
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
                    else
                    if (_tokens.TryGetValue(id, out taskCompletion))
                    {
                        if (id == "register_0") return;
                        if (obj.type == "error")
                        {
                            taskCompletion.SetException(new Exception(obj.error));
                        }
                        //else if (args.Cancelled)
                        //{
                        //    taskSource.SetCanceled();
                        //}
                        taskCompletion.TrySetResult(obj.payload);
                    }
                }
            }
            catch (Exception ex)
            {
                var status = WebSocketError.GetStatus(ex.GetBaseException().HResult);
                Logger.Error($"Connection_MessageReceived: status: {status}, exception: {ex.Message}");
                ConnectionClosed = true;
                _messageWriter?.Dispose();
            }
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
