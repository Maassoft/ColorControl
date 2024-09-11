using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NStandard.Measures.Length;

namespace ColorControl.Shared.EventDispatcher;

public class DisplayChangedEventArgs : EventArgs
{
    public uint Width { get; set; }
    public uint Height { get; set; }
    public uint BitsPerPixel{ get; set; }
}

public class DisplayChangeEventDispatcher : EventDispatcher<DisplayChangedEventArgs>
{
    public const string Event_DisplayChanged = "DisplayChanged";
    public DisplayChangeEventDispatcher(WindowMessageDispatcher windowMessageDispatcher)
    {
        windowMessageDispatcher.RegisterEventHandler(WindowMessageDispatcher.Event_WindowMessageDisplayChange, OnWindowMessageDisplayChange);
    }

    private async void OnWindowMessageDisplayChange(object sender, WindowMessageEventArgs e)
    {
        uint width = (uint)(e.Message.LParam & 0xffff);
        uint height = (uint)(e.Message.LParam >> 16);
        uint bpp = (uint)e.Message.WParam;
        await DispatchEventAsync(Event_DisplayChanged, new DisplayChangedEventArgs { Width = width, Height = height, BitsPerPixel = bpp });
    }
}

