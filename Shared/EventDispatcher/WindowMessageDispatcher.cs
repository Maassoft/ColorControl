using ColorControl.Shared.Common;
using NWin32;

namespace ColorControl.Shared.EventDispatcher;

public class WindowMessageEventArgs : EventArgs
{
    public Message Message { get; private set; }

    public WindowMessageEventArgs(Message message)
    {
        Message = message;
    }
}

public class WindowMessageDispatcher : EventDispatcher<WindowMessageEventArgs>
{
    public const string Event_WindowMessage = "WindowMessage";
    public const string Event_WindowMessageHotKey = "WindowMessageHotKey";
    public const string Event_WindowMessageInput = "WindowMessageInput";
    public const string Event_WindowMessageQueryEndSession = "WindowMessageQueryEndSession";
    public const string Event_WindowMessageEndSession = "WindowMessageEndSession";
    public const string Event_WindowMessageClose = "WindowMessageClose";
    public const string Event_WindowMessagePowerBroadcast = "WindowMessagePowerBroadcast";
    public const string Event_WindowMessageShowWindow = "WindowMessageShowWindow";
    public const string Event_WindowMessageUserBringToFront = "WindowMessageUserBringToFront";
    public const string Event_WindowMessageDisplayChange = "WindowMessageDisplayChange";

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    private readonly GlobalContext _globalContext;

    public DummyForm MessageForm { get; private set; }


    private static Dictionary<int, string> Messages = new() {
        { NativeConstants.WM_HOTKEY, Event_WindowMessageHotKey },
        { NativeConstants.WM_INPUT, Event_WindowMessageInput },
        { NativeConstants.WM_QUERYENDSESSION, Event_WindowMessageQueryEndSession },
        { NativeConstants.WM_ENDSESSION, Event_WindowMessageEndSession },
        { NativeConstants.WM_CLOSE, Event_WindowMessageClose },
        { NativeConstants.WM_POWERBROADCAST, Event_WindowMessagePowerBroadcast },
        { NativeConstants.WM_SHOWWINDOW, Event_WindowMessageShowWindow },
        { NativeConstants.WM_DISPLAYCHANGE, Event_WindowMessageDisplayChange },
        { Utils.WM_BRINGTOFRONT, Event_WindowMessageUserBringToFront }
    };

    public WindowMessageDispatcher(GlobalContext globalContext)
    {
        MessageForm = new DummyForm();
        MessageForm.OnMessage += MessageForm_OnMessage;
        _globalContext = globalContext;

        _globalContext.MainHandle = MessageForm.Handle;
    }

    private void MessageForm_OnMessage(object sender, Message e)
    {
        if (Messages.TryGetValue(e.Msg, out var eventName))
        {
            DispatchEvent(eventName, new WindowMessageEventArgs(e));
        }
    }

    public class DummyForm : Form
    {
        public event EventHandler<Message> OnMessage;

        public DummyForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            Load += DummyForm_Load;
            Shown += DummyForm_Shown;
        }

        private void DummyForm_Shown(object sender, EventArgs e)
        {
            Hide();
        }

        protected void DummyForm_Load(object sender, EventArgs e)
        {
            Size = new Size(0, 0);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeConstants.WM_QUERYENDSESSION)
            {
                Logger.Debug("WM_QUERYENDSESSION");
            }

            Logger.Debug(m);

            OnMessage?.Invoke(this, m);

            base.WndProc(ref m);
        }
    }
}
