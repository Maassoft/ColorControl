using System;
using System.Threading.Tasks;

namespace LgTv
{
    public enum ButtonType
    {
        HOME,
        BACK,
        ENTER,
        EXIT,
        UP,
        DOWN,
        LEFT,
        RIGHT,
        RED,
        GREEN,
        YELLOW,
        BLUE,
        _1,
        _2,
        _3,
        _4,
        _5,
        _6,
        _7,
        _8,
        _9,
        _0,
        POWER,
        VOLUMEUP,
        VOLUMEDOWN,
        MUTE,
        MENU,
        CC,
        DASH,
        CHANNELUP,
        CHANNELDOWN,
        LIST,
        AD,
        SAP,
        PROGRAM,
        PLAY,
        PAUSE,
        STOP,
        REWIND,
        FASTFORWARD,
        GUIDE,
        AMAZON,
        NETFLIX,
        MAGNIFIER_ZOOM,
        LIVE_ZOOM,
        _3D_MODE,
        ASPECT_RATIO,
        RECENT,
        RECORD,
        SCREEN_REMOTE
    }

    public class LgWebOsMouseService : IDisposable
    {
        private readonly LgTvApiCore _connection;
        public LgWebOsMouseService(LgTvApiCore connection)
        {
            _connection = connection;
        }

        public async Task<bool> Connect(string uri)
        {
            var ctx = _connection.Connect(new Uri(uri), ignoreReceiver: true);
            return (await ctx);
        }
        public void SendButton(int number)
        {
            _connection.SendMessageAsync($"type:button\nname:{number}\n\n").ConfigureAwait(false);
        }
        public void SendButton(ButtonType bt)
        {
            var text = bt.ToString();
            if (text[0] == '_')
            {
                text = text.Substring(1);
            }
            _connection.SendMessageAsync($"type:button\nname:{text}\n\n").ConfigureAwait(false);
        }


        public void Move(double dx, double dy, bool drag = false)
        {
            _connection.SendMessageAsync($"type:move\ndx:{dx}\ndy:{dy}\ndown:{(drag ? 1 : 0)}\n\n").ConfigureAwait(false);
        }

        public void Scroll(double dx, double dy)
        {
            _connection.SendMessageAsync($"type:scroll\ndx:{dx}\ndy:{dy}\n\n").ConfigureAwait(false);
        }

        public void Click()
        {
            _connection.SendMessageAsync("type:click\n\n").ConfigureAwait(false);
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
