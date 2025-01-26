using ColorControl.Shared.Contracts.LG;
using System;
using System.Threading.Tasks;

namespace LgTv
{
    public class LgWebOsMouseService : IAsyncDisposable
    {
        private readonly LgTvApiCoreCws _connection;

        public LgWebOsMouseService(LgTvApiCoreCws connection)
        {
            _connection = connection;
        }

        public async Task<bool> Connect(string uri)
        {
            var ctx = _connection.Connect(new Uri(uri), ignoreReceiver: true);
            return (await ctx);
        }

        public async Task SendButton(ButtonType bt)
        {
            var text = bt.ToString();
            if (text[0] == '_')
            {
                text = text.Substring(1);
            }
            await _connection.SendMessageAsync($"type:button\nname:{text}\n\n");
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

        public async ValueTask DisposeAsync()
        {
            if (_connection != null)
            {
                await _connection.DisposeAsync();
            }

            GC.SuppressFinalize(this);
        }
    }
}
