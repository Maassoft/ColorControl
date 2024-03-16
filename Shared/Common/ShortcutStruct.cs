using ColorControl.Shared.Native;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ColorControl.Shared.Common;

public class ShortcutStruct
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public int VirtualKeyCode { get; private set; }
    public int Modifiers { get; private set; }
    public int Id { get; }
    public string Shortcut { get; private set; }
    public bool HotKeyRegistered { get; set; }

    public ShortcutStruct(int modifiers, int virtualKeyCode, int id, string shortcut)
    {
        VirtualKeyCode = virtualKeyCode;
        Modifiers = modifiers;
        Id = id;
        Shortcut = shortcut;
    }

    internal void SetModifiers(int mods)
    {
        Modifiers = mods;
    }

    internal void SetKey(int key)
    {
        VirtualKeyCode = key;
    }

    internal void SetShortcut(string shortcut)
    {
        Shortcut = shortcut;
    }

    internal bool RegisterHotKey(nint handle)
    {
        var result = WinApi.RegisterHotKey(handle, Id, Modifiers, VirtualKeyCode);
        if (!result)
        {
            var errorMessage = new Win32Exception(Marshal.GetLastWin32Error()).Message;
            Logger.Error($"Could not register shortcut {Shortcut}: {errorMessage}");
        }
        else
        {
            HotKeyRegistered = true;
        }

        return result;
    }

    internal void UnregisterHotKey(nint handle)
    {
        if (!HotKeyRegistered)
        {
            return;
        }

        WinApi.UnregisterHotKey(handle, Id);

        HotKeyRegistered = false;
    }
}
