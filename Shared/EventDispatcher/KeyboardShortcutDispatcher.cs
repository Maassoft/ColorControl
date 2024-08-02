using ColorControl.Shared.Common;
using ColorControl.Shared.Forms;
using Linearstar.Windows.RawInput;
using Linearstar.Windows.RawInput.Native;

namespace ColorControl.Shared.EventDispatcher;

public class KeyboardShortcutEventArgs : EventArgs
{
    public int HotKeyId { get; set; }
}

public class KeyboardShortcutDispatcher : EventDispatcher<KeyboardShortcutEventArgs>
{
    [Flags]
    public enum ModKeys : int
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        Win = 8
    }

    public const string Event_HotKey = "HotKey";

    public nint MainHandle { get; set; }
    public bool UseRawInput { get; private set; }

    public static bool IsActive = true;

    private static bool WinKeyDown = false;

    private static readonly List<Keys> KeysWithoutModifiers = new List<Keys> { Keys.F13, Keys.F14, Keys.F15, Keys.F16, Keys.F17, Keys.F18, Keys.F19, Keys.F20, Keys.F21, Keys.F22, Keys.F23, Keys.F24 };
    private static readonly List<Keys> Modifiers = new List<Keys> { Keys.ControlKey, Keys.Menu, Keys.ShiftKey, Keys.LWin };

    private readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly HashSet<ModKeys> _downModifierKeys = new HashSet<ModKeys>();
    private readonly HashSet<Keys> _downKeys = new HashSet<Keys>();

    private readonly object _modifiersLock = new object();
    private readonly WindowMessageDispatcher _windowMessageDispatcher;
    private readonly Dictionary<int, ShortcutStruct> _registeredShortcuts = new Dictionary<int, ShortcutStruct>();

    public KeyboardShortcutDispatcher(WindowMessageDispatcher windowMessageDispatcher)
    {
        _windowMessageDispatcher = windowMessageDispatcher;

        _windowMessageDispatcher.RegisterEventHandler(WindowMessageDispatcher.Event_WindowMessageHotKey, HotKeyPressed);
        _windowMessageDispatcher.RegisterEventHandler(WindowMessageDispatcher.Event_WindowMessageInput, HandleInputMessage);

        MainHandle = _windowMessageDispatcher.MessageForm.Handle;
    }

    private async void HotKeyPressed(object sender, WindowMessageEventArgs e)
    {
        if (!IsActive || IsShortcutControlFocused())
        {
            return;
        }

        await DispatchEventAsync(Event_HotKey, new KeyboardShortcutEventArgs { HotKeyId = (int)e.Message.WParam });
    }

    private async void HandleInputMessage(object sender, WindowMessageEventArgs e)
    {
        if (IsShortcutControlFocused())
        {
            return;
        }

        // Create an RawInputData from the handle stored in lParam.
        var data = RawInputData.FromHandle(e.Message.LParam);

        if (data is RawInputKeyboardData keyboard)
        {
            var shortcut = GetRawKeyboardInputShortcut(keyboard);

            if (shortcut != null)
            {
                await DispatchEventAsync(Event_HotKey, new KeyboardShortcutEventArgs { HotKeyId = shortcut.Id });
            }
        }
    }

    public static bool IsShortcutControlFocused()
    {
        if (Form.ActiveForm?.GetType().Name.Equals("MessageForm") == true)
        {
            return true;
        }
        var control = Form.ActiveForm?.FindFocusedControl();
        return control?.Name.Contains("Shortcut") ?? false;
    }

    public bool ShortCutExists(string shortcut, int presetId = 0)
    {
        return _registeredShortcuts.Any(s => s.Key != presetId && s.Value.Shortcut == shortcut);
    }

    public void SetUseRawInput(bool useRawInput)
    {
        if (UseRawInput == useRawInput)
        {
            return;
        }

        UseRawInput = useRawInput;

        if (useRawInput)
        {
            UnregisterHotKeys();

            RawInputDevice.RegisterDevice(HidUsageAndPage.Keyboard, RawInputDeviceFlags.InputSink, MainHandle);
        }
        else
        {
            RawInputDevice.UnregisterDevice(HidUsageAndPage.Keyboard);

            RegisterHotKeys();
        }
    }

    private void UnregisterHotKeys()
    {
        foreach (var shortcut in _registeredShortcuts.Values.Where(s => s.HotKeyRegistered).ToList())
        {
            shortcut.UnregisterHotKey(MainHandle);
        }
    }

    private void RegisterHotKeys()
    {
        foreach (var shortcut in _registeredShortcuts.Values.Where(s => !s.HotKeyRegistered).ToList())
        {
            shortcut.RegisterHotKey(MainHandle);
        }
    }

    public ShortcutStruct GetRawKeyboardInputShortcut(RawInputKeyboardData data)
    {
        var keyboard = data.Keyboard;

        //Debug.WriteLine($"Key: {keyboard}");

        var add = !keyboard.Flags.HasFlag(RawKeyboardFlags.Up);

        var keys = (Keys)keyboard.VirutalKey;

        var isModifier = Modifiers.Contains(keys);
        var modKey = KeysToModKeys(keys);
        var changed = false;

        lock (_modifiersLock)
        {
            if (add)
            {
                if (isModifier)
                {
                    changed = _downModifierKeys.Add(modKey);
                }
                else
                {
                    changed = _downKeys.Add((Keys)keyboard.VirutalKey);
                }
            }
            else
            {
                if (isModifier)
                {
                    changed = _downModifierKeys.Remove(modKey);
                }
                else
                {
                    changed = _downKeys.Remove((Keys)keyboard.VirutalKey);
                }
            }
        }

        if (!changed || _downKeys.Count != 1 || !(_downModifierKeys.Any() || KeysWithoutModifiers.Contains(_downKeys.First())))
        {
            return null;
        }

        var sum = _downModifierKeys.Sum(k => (int)k) * 1000;
        sum += _downKeys.Sum(k => (int)k);

        var shortcut = _registeredShortcuts.Values.FirstOrDefault(s => (s.Modifiers * 1000 + s.VirtualKeyCode) == sum);

        if (shortcut == null)
        {
            return null;
        }

        return shortcut;
    }

    public bool RegisterShortcut(int id, string shortcut)
    {
        var clear = string.IsNullOrWhiteSpace(shortcut);

        var shortcutStruct = GetShortcutById(id);

        if (shortcutStruct == null)
        {
            if (clear)
            {
                return true;
            }

            var (mods, key) = ParseShortcut(shortcut);

            shortcutStruct = new ShortcutStruct(mods, key, id, shortcut);

            _registeredShortcuts[id] = shortcutStruct;
        }
        else if (clear)
        {
            shortcutStruct.UnregisterHotKey(MainHandle);

            _registeredShortcuts.Remove(id);

            return true;
        }
        else
        {
            var (mods, key) = ParseShortcut(shortcut);

            if (shortcutStruct.Modifiers == mods && shortcutStruct.VirtualKeyCode == key)
            {
                return true;
            }

            shortcutStruct.SetModifiers(mods);
            shortcutStruct.SetKey(key);
            shortcutStruct.SetShortcut(shortcut);
            shortcutStruct.UnregisterHotKey(MainHandle);
        }

        //if (UseRawInput)
        //{
        //    return true;
        //}

        return shortcutStruct.RegisterHotKey(MainHandle);
    }

    private ShortcutStruct GetShortcutById(int id)
    {
        if (_registeredShortcuts.TryGetValue(id, out var shortcut))
        {
            return shortcut;
        }

        return null;
    }

    public static (int mods, int key) ParseShortcut(string shortcut)
    {
        int mods = 0;
        var key = 0;

        shortcut = shortcut.Replace(",", "");
        shortcut = shortcut.Replace("+", "");
        shortcut = shortcut.Replace("  ", " ");
        var parts = shortcut.Split(' ');

        if (parts.Length > 0)
        {
            for (int i = 0; i + 1 < parts.Length; i++)
            {
                switch (parts[i])
                {
                    case "Alt":
                        mods += (int)ModKeys.Alt;
                        break;
                    case "Control":
                        mods += (int)ModKeys.Control;
                        break;
                    case "Shift":
                        mods += (int)ModKeys.Shift;
                        break;
                    case "Win":
                        mods += (int)ModKeys.Win;
                        break;
                }
            }

            var keyName = parts[parts.Length - 1];

            key = (int)Enum.Parse(typeof(Keys), keyName);
        }

        return (mods, key);
    }

    public static Keys ShortcutToKeys(string shortcut)
    {
        var (mods, key) = ParseShortcut(shortcut);

        var keys = Keys.None;

        if (((uint)mods & (uint)ModKeys.Alt) > 0)
        {
            keys |= Keys.Alt;
        }
        if (((uint)mods & (uint)ModKeys.Control) > 0)
        {
            keys |= Keys.Control;
        }
        if (((uint)mods & (uint)ModKeys.Shift) > 0)
        {
            keys |= Keys.Shift;
        }
        if (((uint)mods & (uint)ModKeys.Win) > 0)
        {
            keys |= Keys.LWin;
        }

        keys |= (Keys)key;

        return keys;
    }

    public static ModKeys KeysToModKeys(Keys key)
    {
        return key switch
        {
            Keys.ControlKey => ModKeys.Control,
            Keys.Menu => ModKeys.Alt,
            Keys.ShiftKey => ModKeys.Shift,
            Keys.LWin => ModKeys.Win,
            _ => ModKeys.None,
        };
    }

    public static string FormatKeyboardShortcut(bool shiftKey, bool ctrlKey, bool altKey, string key, string code)
    {
        var keys = Keys.None;

        if (shiftKey)
        {
            keys |= Keys.Shift;
        }
        if (ctrlKey)
        {
            keys |= Keys.Control;
        }
        if (altKey)
        {
            keys |= Keys.Alt;
        }

        // Meta (Windows Key) can currently not be combined with other keys...
        //if (key == "Meta")
        //{
        //    keys |= Keys.LWin;
        //    key = null;
        //}

        if (key != null && key != "Alt" && key != "Control" && key != "Meta")
        {
            if (key.Length == 1)
            {
                keys |= (Keys)key.ToUpper()[0];
            }
            else
            {
                if (!Enum.TryParse<Keys>(key, out var parsedKey))
                {
                    var names = Enum.GetNames<Keys>();
                    var name = names.FirstOrDefault(n => key.StartsWith(n));
                    if (name != null)
                    {
                        parsedKey = Enum.Parse<Keys>(name);
                    }
                }
                if (parsedKey != Keys.None)
                {
                    keys |= parsedKey;
                }
            }
        }

        var keyEvent = new KeyEventArgs(keys);

        return FormatKeyboardShortcut(keyEvent);
    }

    public static string FormatKeyboardShortcut(KeyEventArgs keyEvent)
    {
        var pressedModifiers = keyEvent.Modifiers;

        //Debug.WriteLine("KD: " + e.Modifiers + ", " + e.KeyCode);

        var shortcutString = pressedModifiers > 0 ? pressedModifiers.ToString() : "";
        if (keyEvent.KeyCode == Keys.LWin || WinKeyDown)
        {
            WinKeyDown = true;
            if (!string.IsNullOrEmpty(shortcutString))
            {
                shortcutString += ", ";
            }
            shortcutString += "Win";
        }

        var empty = string.IsNullOrEmpty(shortcutString);
        if ((!empty || KeysWithoutModifiers.Contains(keyEvent.KeyCode)) && keyEvent.KeyCode != Keys.ControlKey && keyEvent.KeyCode != Keys.ShiftKey && keyEvent.KeyCode != Keys.Menu && keyEvent.KeyCode != Keys.LWin)
        {
            if (!empty)
            {
                shortcutString += " + ";
            }
            shortcutString += keyEvent.KeyCode.ToString();
        }

        if (string.IsNullOrEmpty(shortcutString))
        {
            keyEvent.SuppressKeyPress = true;
        }

        return shortcutString;
    }

    public static bool ValidateShortcut(string shortcut)
    {
        var valid = string.IsNullOrWhiteSpace(shortcut) || shortcut.Contains("+") || ShortcutIsAllowedWithoutModifier(shortcut);

        if (!valid)
        {
            MessageForms.WarningOk("Invalid shortcut. The shortcut should have modifiers and a normal key or be F13-F24.");
        }

        return valid;
    }

    public static bool ShortcutIsAllowedWithoutModifier(string shortcut)
    {
        return Enum.TryParse<Keys>(shortcut, out var key) && KeysWithoutModifiers.Contains(key);
    }

    public static void HandleKeyboardShortcutUp(KeyEventArgs keyEvent)
    {
        if (keyEvent.KeyCode == Keys.LWin)
        {
            WinKeyDown = false;
        }
    }
}
