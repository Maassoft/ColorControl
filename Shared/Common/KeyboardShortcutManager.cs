using ColorControl.Shared.Forms;
using Linearstar.Windows.RawInput;
using Linearstar.Windows.RawInput.Native;
using NWin32;

namespace ColorControl.Shared.Common
{
    public class KeyboardShortcutManager
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

        public static nint MainHandle { get; set; }
        public static bool UseRawInput { get; private set; }

        private static bool WinKeyDown = false;

        private static List<Keys> KeysWithoutModifiers = new List<Keys> { Keys.F13, Keys.F14, Keys.F15, Keys.F16, Keys.F17, Keys.F18, Keys.F19, Keys.F20, Keys.F21, Keys.F22, Keys.F23, Keys.F24 };
        private static List<Keys> Modifiers = new List<Keys> { Keys.ControlKey, Keys.Menu, Keys.ShiftKey, Keys.LWin };

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static readonly HashSet<ModKeys> _downModifierKeys = new HashSet<ModKeys>();
        private static readonly HashSet<Keys> _downKeys = new HashSet<Keys>();

        private static readonly object _modifiersLock = new object();

        private static Dictionary<int, ShortcutStruct> _registeredShortcuts = new Dictionary<int, ShortcutStruct>();

        public static void SetUseRawInput(bool useRawInput)
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

        private static void UnregisterHotKeys()
        {
            foreach (var shortcut in _registeredShortcuts.Values.Where(s => s.HotKeyRegistered).ToList())
            {
                shortcut.UnregisterHotKey(MainHandle);
            }
        }

        private static void RegisterHotKeys()
        {
            foreach (var shortcut in _registeredShortcuts.Values.Where(s => !s.HotKeyRegistered).ToList())
            {
                shortcut.RegisterHotKey(MainHandle);
            }
        }

        public static void HandleRawKeyboardInput(RawInputKeyboardData data)
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
                return;
            }

            var sum = _downModifierKeys.Sum(k => (int)k) * 1000;
            sum += _downKeys.Sum(k => (int)k);

            var shortcut = _registeredShortcuts.Values.FirstOrDefault(s => (s.Modifiers * 1000 + s.VirtualKeyCode) == sum);

            if (shortcut == null)
            {
                return;
            }

            NativeMethods.SendMessageW(MainHandle, NativeConstants.WM_HOTKEY, (uint)shortcut.Id, 0);
        }

        public static bool RegisterShortcut(int id, string shortcut)
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
                shortcutStruct.UnregisterHotKey(id);

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

        private static ShortcutStruct GetShortcutById(int id)
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
                keys |= Keys.Control;
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
}
