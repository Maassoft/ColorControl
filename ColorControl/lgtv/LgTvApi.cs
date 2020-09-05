using ColorControl;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;

namespace LgTv
{
    //https://mym.hackpad.com/ep/pad/static/rLlshKkzdNj

    public enum ControlButtons
    {
        Back,Down,Left,Right,
        OK,
        Exit
    }
    public class LgTvApi:IDisposable
    {
        public bool ConnectionClosed { get => _connection == null || _connection.ConnectionClosed; }

        private readonly LgTvApiCore _connection;
        private LgWebOsMouseService _mouseConnection;

        private readonly ClientKeyStore _keyStore;

        private List<Channel> _channelList;
        private List<App> _appList;
        private string _ip;

        private const string BEFORE_PAIR_HAND_SHAKE = "{\"type\":\"register\",\"id\":\"register_0\",\"payload\":{\"forcePairing\":false,\"pairingType\":\"PROMPT\",\"manifest\":{\"manifestVersion\":1,\"appVersion\":\"1.1\",\"signed\":{\"created\":\"20140509\",\"appId\":\"com.lge.test\",\"vendorId\":\"com.lge\",\"localizedAppNames\":{\"\":\"LG Remote App\",\"ko-KR\":\"리모컨 앱\",\"zxx-XX\":\"ЛГ Rэмotэ AПП\"},\"localizedVendorNames\":{\"\":\"LG Electronics\"},\"permissions\":[\"TEST_SECURE\",\"CONTROL_INPUT_TEXT\",\"CONTROL_MOUSE_AND_KEYBOARD\",\"READ_INSTALLED_APPS\",\"READ_LGE_SDX\",\"READ_NOTIFICATIONS\",\"SEARCH\",\"WRITE_SETTINGS\",\"WRITE_NOTIFICATION_ALERT\",\"CONTROL_POWER\",\"READ_CURRENT_CHANNEL\",\"READ_RUNNING_APPS\",\"READ_UPDATE_INFO\",\"UPDATE_FROM_REMOTE_APP\",\"READ_LGE_TV_INPUT_EVENTS\",\"READ_TV_CURRENT_TIME\"],\"serial\":\"2f930e2d2cfe083771f68e4fe7bb07\"},\"permissions\":[\"LAUNCH\",\"LAUNCH_WEBAPP\",\"APP_TO_APP\",\"CLOSE\",\"TEST_OPEN\",\"TEST_PROTECTED\",\"CONTROL_AUDIO\",\"CONTROL_DISPLAY\",\"CONTROL_INPUT_JOYSTICK\",\"CONTROL_INPUT_MEDIA_RECORDING\",\"CONTROL_INPUT_MEDIA_PLAYBACK\",\"CONTROL_INPUT_TV\",\"CONTROL_POWER\",\"READ_APP_STATUS\",\"READ_CURRENT_CHANNEL\",\"READ_INPUT_DEVICE_LIST\",\"READ_NETWORK_STATE\",\"READ_RUNNING_APPS\",\"READ_TV_CHANNEL_LIST\",\"WRITE_NOTIFICATION_TOAST\",\"READ_POWER_STATE\",\"READ_COUNTRY_INFO\"],\"signatures\":[{\"signatureVersion\":1,\"signature\":\"eyJhbGdvcml0aG0iOiJSU0EtU0hBMjU2Iiwia2V5SWQiOiJ0ZXN0LXNpZ25pbmctY2VydCIsInNpZ25hdHVyZVZlcnNpb24iOjF9.hrVRgjCwXVvE2OOSpDZ58hR+59aFNwYDyjQgKk3auukd7pcegmE2CzPCa0bJ0ZsRAcKkCTJrWo5iDzNhMBWRyaMOv5zWSrthlf7G128qvIlpMT0YNY+n/FaOHE73uLrS/g7swl3/qH/BGFG2Hu4RlL48eb3lLKqTt2xKHdCs6Cd4RMfJPYnzgvI4BNrFUKsjkcu+WD4OO2A27Pq1n50cMchmcaXadJhGrOqH5YmHdOCj5NSHzJYrsW0HPlpuAx/ECMeIZYDh6RMqaFM2DXzdKX9NmmyqzJ3o/0lkk/N97gfVRLW5hA29yeAwaCViZNCP8iC9aO0q9fQojoa7NQnAtw==\"}]}}}";

        private const string AFTER_PAIR_HAND_SHAKE = "{\"type\":\"register\",\"id\":\"register_0\",\"payload\":{\"forcePairing\":false,\"pairingType\":\"PROMPT\",\"client-key\":\"CLIENTKEYGOESHERE\",\"manifest\":{\"manifestVersion\":1,\"appVersion\":\"1.1\",\"signed\":{\"created\":\"20140509\",\"appId\":\"com.lge.test\",\"vendorId\":\"com.lge\",\"localizedAppNames\":{\"\":\"LG Remote App\",\"ko-KR\":\"리모컨 앱\",\"zxx-XX\":\"ЛГ Rэмotэ AПП\"},\"localizedVendorNames\":{\"\":\"LG Electronics\"},\"permissions\":[\"TEST_SECURE\",\"CONTROL_INPUT_TEXT\",\"CONTROL_MOUSE_AND_KEYBOARD\",\"READ_INSTALLED_APPS\",\"READ_LGE_SDX\",\"READ_NOTIFICATIONS\",\"SEARCH\",\"WRITE_SETTINGS\",\"WRITE_NOTIFICATION_ALERT\",\"CONTROL_POWER\",\"READ_CURRENT_CHANNEL\",\"READ_RUNNING_APPS\",\"READ_UPDATE_INFO\",\"UPDATE_FROM_REMOTE_APP\",\"READ_LGE_TV_INPUT_EVENTS\",\"READ_TV_CURRENT_TIME\"],\"serial\":\"2f930e2d2cfe083771f68e4fe7bb07\"},\"permissions\":[\"LAUNCH\",\"LAUNCH_WEBAPP\",\"APP_TO_APP\",\"CLOSE\",\"TEST_OPEN\",\"TEST_PROTECTED\",\"CONTROL_AUDIO\",\"CONTROL_DISPLAY\",\"CONTROL_INPUT_JOYSTICK\",\"CONTROL_INPUT_MEDIA_RECORDING\",\"CONTROL_INPUT_MEDIA_PLAYBACK\",\"CONTROL_INPUT_TV\",\"CONTROL_POWER\",\"READ_APP_STATUS\",\"READ_CURRENT_CHANNEL\",\"READ_INPUT_DEVICE_LIST\",\"READ_NETWORK_STATE\",\"READ_RUNNING_APPS\",\"READ_TV_CHANNEL_LIST\",\"WRITE_NOTIFICATION_TOAST\",\"READ_POWER_STATE\",\"READ_COUNTRY_INFO\"],\"signatures\":[{\"signatureVersion\":1,\"signature\":\"eyJhbGdvcml0aG0iOiJSU0EtU0hBMjU2Iiwia2V5SWQiOiJ0ZXN0LXNpZ25pbmctY2VydCIsInNpZ25hdHVyZVZlcnNpb24iOjF9.hrVRgjCwXVvE2OOSpDZ58hR+59aFNwYDyjQgKk3auukd7pcegmE2CzPCa0bJ0ZsRAcKkCTJrWo5iDzNhMBWRyaMOv5zWSrthlf7G128qvIlpMT0YNY+n/FaOHE73uLrS/g7swl3/qH/BGFG2Hu4RlL48eb3lLKqTt2xKHdCs6Cd4RMfJPYnzgvI4BNrFUKsjkcu+WD4OO2A27Pq1n50cMchmcaXadJhGrOqH5YmHdOCj5NSHzJYrsW0HPlpuAx/ECMeIZYDh6RMqaFM2DXzdKX9NmmyqzJ3o/0lkk/N97gfVRLW5hA29yeAwaCViZNCP8iC9aO0q9fQojoa7NQnAtw==\"}]}}}";

        public Task ControlButton(object oK)
        {
            //   throw new NotImplementedException();
            return null;
        }

        private string _currentPariKey;
        private string webSocketUri;

        public static async Task<LgTvApi> CreateLgTvApi(string ip, int retries = 1)
        {
            var instance = new LgTvApi(ip, new LgTvApiCore(), new ClientKeyStore(ip));
            while (retries > 0)
            {
                var connected = await instance.Connect();
                if (connected)
                {
                    await instance.MakeHandShake();
                    return instance;
                }
                retries--;
                await Task.Delay(500);
            }
            instance.Dispose();
            return null;
        }

        private LgTvApi(string ip, LgTvApiCore connection, ClientKeyStore keyStore)
        {
            webSocketUri = "ws://" + ip + ":3000";
            _ip = ip;
            _connection = connection;
            _keyStore = keyStore;
        }

        public string GetIpAddress()
        {
            return _ip;
        }

        public async Task MakeHandShake()
        {
            _currentPariKey = _keyStore.GetClientKey();
            if (_currentPariKey != null)
            {
                var key = AFTER_PAIR_HAND_SHAKE.Replace("CLIENTKEYGOESHERE", _currentPariKey);
                var conn = await _connection.SendCommandAsync(key);
                _keyStore.SaveClientKey((string)conn.clientKey);
                return;
            }

            dynamic result = await _connection.SendCommandAsync(BEFORE_PAIR_HAND_SHAKE);
            _keyStore.SaveClientKey(result.clientKey);
        }
    

        public async Task<bool> Connect()
        {
            var ctx =  _connection.Connect(new Uri(webSocketUri));
            return await ctx;
        }

        public async Task<LgWebOsMouseService> GetMouse()
        {
            if (_mouseConnection != null) return _mouseConnection;
            var requestMessage = new RequestMessage("ssap://com.webos.service.networkinput/getPointerInputSocket", new { });
            var s = await _connection.SendCommandAsync(requestMessage);
            var socketPath = (string)s.socketPath;
           
            _mouseConnection = new LgWebOsMouseService(new LgTvApiCore());
            await _mouseConnection.Connect(socketPath);
            return _mouseConnection;
        }

        public async Task<IEnumerable<Channel>> ChannelList()
        {
            _channelList = new List<Channel>();
            var requestMessage = new RequestMessage("channels", "ssap://tv/getChannelList");
            var command = _connection.SendCommandAsync(requestMessage);
            var res = await command;
            foreach (var c in res.channelList)
            {
                _channelList.Add(new Channel
                {
                    Id = c.channelId,
                    Name = c.channelName,
                    Number = int.Parse((string)c.channelNumber)
                });
            }
            return _channelList.OrderBy(e => e.Number);
        }

        public async Task<Channel> GetChannel()
        {
            var requestMessage = new RequestMessage("channels", "ssap://tv/getCurrentChannel");
            var command = _connection.SendCommandAsync(requestMessage);

            var res = await command;
            return new Channel() { Id = res.channelId, Name = res.channelName, Number = int.Parse(res.channelNumber) };
        }
        public async Task ShowToast()
        {
            var requestMessage = new RequestMessage("ssap://system.notifications/createToast",new {message = "Co tam u Ciebie?"});
            await _connection.SendCommandAsync(requestMessage);
        }
       
        //public async Task<string> LaunchApp(string appId,Uri uri)
        //{
        //    var url = uri.ToString();
        //    var requestMessage = new RequestMessage("ssap://system.launcher/launch", new { id = appId, @params={contentTarget } });
        //    var response = await _connection.SendCommandAsync(requestMessage);
        //    return (string)response.sessionId;
        //}

        public async Task VolumeDown()
        {
            var requestMessage = new RequestMessage("volumedown", "ssap://audio/volumeDown");
            await _connection.SendCommandAsync(requestMessage);
        }
        public async Task  VolumeUp()
        {
            var requestMessage = new RequestMessage("volumeup", "ssap://audio/volumeUp");
            await _connection.SendCommandAsync(requestMessage);
        }
        public async Task ChannelInfo()
        {
            var requestMessage = new RequestMessage("programinfo", "ssap://tv/getChannelProgramInfo");
            await _connection.SendCommandAsync(requestMessage);
        }

        public async Task  SetChannel(string channelId)
        {
            var requestMessage = new RequestMessage("ssap://tv/openChannel", new { channelId });
            await _connection.SendCommandAsync(requestMessage);
        }
        public async  Task  SetMute(bool value)
        {
            var requestMessage = new RequestMessage("ssap://audio/setMute", new { mute = value });
            await _connection.SendCommandAsync(requestMessage);
        }
        public async Task ToogleMute()
        {
            await SetMute(!await IsMuted());
        }
        public async Task<bool> IsMuted()
        {
            var requestMessage = new RequestMessage("status", "ssap://audio/getStatus");
            var command = _connection.SendCommandAsync(requestMessage);
            return (bool)(await command).mute;
        }

        public async Task  SetVolume(int value)
        {
            if (value >= 0 && value <= 100)
            {
                var requestMessage = new RequestMessage("ssap://audio/setVolume", new { volume = value });
                await _connection.SendCommandAsync(requestMessage);
            }
        }
        public async Task  TurnOff()
        {
            await _connection.SendCommandAsync(new RequestMessage("","ssap://system/turnOff"));
        }
        public async Task  Play()
        {
            await _connection.SendCommandAsync(new RequestMessage("play", "ssap://media.controls/play"));
        }
        public async Task Pause()
        {
            await _connection.SendCommandAsync(new RequestMessage("pause", "ssap://media.controls/pause"));
        }

        public async Task ChannelUp()
        {
            await _connection.SendCommandAsync(new RequestMessage("channelUp", "ssap://tv/channelUp"));
        }
        public async Task ChannelDown()
        {
            await _connection.SendCommandAsync(new RequestMessage("channelDown", "ssap://tv/channelDown"));
        }

        public async Task  Stop()
        {
            await _connection.SendCommandAsync(new RequestMessage("stop", "ssap://media.controls/stop"));
        }

        public async Task TurnOn3D()
        {
            await _connection.SendCommandAsync(new RequestMessage("3d", "ssap://com.webos.service.tv.display/set3DOn"));
        }
        public async Task TurnOff3D()
        {
            await _connection.SendCommandAsync(new RequestMessage("3d", "ssap://com.webos.service.tv.display/set3DOff"));
        }
        public async Task<bool> IsTurnedOn3D()
        {
            //Response: { returnValue: true,  status3D: { status: true, pattern: ’2Dto3D’ } }
            var requestMessage = new RequestMessage("status3D", "ssap://com.webos.service.tv.display/get3DStatus");
            var o = await _connection.SendCommandAsync(requestMessage);
           return (bool)o.status3D.status;
        }

        public async Task<int> GetVolume()
        {
            // {
            //     "type": "response",
            //     "id": "status_1",
            //     "payload": {
            //         "muted": false,
            //         "scenario": "mastervolume_tv_speaker",
            //         "active": false,
            //         "action": "requested",
            //         "volume": 7,
            //         "returnValue": true,
            //         "subscribed": true
            //     }
            // }
            var requestMessage = new RequestMessage("status", "ssap://audio/getVolume");
            var o = await _connection.SendCommandAsync(requestMessage);
            return( (bool)o.muted) ? -1 :(int) o.volume;
        }
        public async Task<IEnumerable<ExternalInput>> GetInputList()
        {
            var requestMessage = new RequestMessage("input","ssap://tv/getExternalInputList");
           var results =  await _connection.SendCommandAsync(requestMessage);
            var l = new List<ExternalInput>();
            foreach (var result in results)
            {
                l.Add(new ExternalInput(result.id, result.label)
                {
                    Icon = result.icon
                });
            }
            return l;
        }

        public async Task  SetInput(string id)
        {
            var requestMessage = new RequestMessage("ssap://tv/switchInput", new { inputId = id });
            await _connection.SendCommandAsync(requestMessage);
        }

        public async Task<IEnumerable<App>> GetLaunchPoints()
        {
            _appList = new List<App>();
            var requestMessage = new RequestMessage("launcher", "ssap://com.webos.applicationManager/listLaunchPoints");
            var command = _connection.SendCommandAsync(requestMessage);
            var res = await command;
            foreach (var c in res.launchpoints)
            {
                _appList.Add(new App
                {
                    Id = c.id,
                    LaunchPointId = c.launchPointId,
                    Title = c.title,
                    Icon = c.icon
                });
            }
            return _appList.OrderBy(e => e.Title);
        }

        public async Task<IEnumerable<LgApp>> GetApps(bool force = false)
        {
            var appList = new List<LgApp>();

            dynamic res;

            var filename = Path.Combine(Utils.GetDataPath(), _ip + "_listApps.json");
            if (!force && File.Exists(filename))
            {
                var json = File.ReadAllText(filename);
                res = JToken.Parse(json);
            }
            else
            {
                var requestMessage = new RequestMessage("ssap://com.webos.applicationManager/listApps", new { });
                var command = _connection.SendCommandAsync(requestMessage);
                res = await command;

                var json = Convert.ToString(res);
                File.WriteAllText(filename, json);
            }

            foreach (var c in res.apps)
            {
                appList.Add(new LgApp((string)c.id, (string)c.title));
            }

            return appList.OrderBy(e => e.title);
        }

        public async Task<string> LaunchApp(string appId)
        {
            var requestMessage = new RequestMessage("ssap://system.launcher/launch", new { id = appId });
            var response = await _connection.SendCommandAsync(requestMessage);
            return (string)response.sessionId;
        }
        public async Task<string> CloseApp(string appId)
        {
            var requestMessage = new RequestMessage("ssap://system.launcher/close", new { id = appId });
            var response = await _connection.SendCommandAsync(requestMessage);
            return (string)response.sessionId;
        }
        public async Task<string> OpenWebBrowser(Uri uri)
        {
            var url = uri.ToString();
            var requestMessage = new RequestMessage("ssap://system.launcher/open", new { target = url });
            var response = await _connection.SendCommandAsync(requestMessage);
            return (string)response.sessionId;
        }

        public async Task<string> LaunchYouTube(string videoId)
        {
            return await LaunchYouTube(new Uri($"http://www.youtube.com/tv?v={videoId}"));
        }
        public async Task<string> LaunchYouTube(Uri uri)
        {
            var url = uri.ToString();
            var requestMessage = new RequestMessage("ssap://system.launcher/launch", new { id = "youtube.leanback.v4", @params = new { contentTarget = url } });
            var response = await _connection.SendCommandAsync(requestMessage);
            return (string)response.sessionId;
        }


        public void Close()
        {
            _connection.Close();
        }
        public void Dispose()
        {
           _connection?.Dispose();
        }
    }
}