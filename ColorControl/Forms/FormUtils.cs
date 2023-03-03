using ColorControl.Common;
using ColorControl.Services.Common;
using NStandard;
using NWin32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ColorControl.Forms
{
    static class FormUtils
    {
        public enum UserNotificationState : int
        {
            QUNS_NOT_PRESENT = 1,
            QUNS_BUSY,
            QUNS_RUNNING_D3D_FULL_SCREEN,
            QUNS_PRESENTATION_MODE,
            QUNS_ACCEPTS_NOTIFICATIONS,
            QUNS_QUIET_TIME,
            QUNS_APP
        }

        public static UserNotificationState[] NotificationsDisabledStates = new[] {
            UserNotificationState.QUNS_BUSY,
            UserNotificationState.QUNS_RUNNING_D3D_FULL_SCREEN,
            UserNotificationState.QUNS_PRESENTATION_MODE,
        };

        [DllImport("shell32.dll")]
        public static extern int SHQueryUserNotificationState(out UserNotificationState pquns);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static UserNotificationState LastNotificationState;

        public static void SetNotifyIconText(NotifyIcon ni, string text)
        {
            text = text.Substring(0, Math.Min(text.Length, 127));
            Type t = typeof(NotifyIcon);
            BindingFlags hidden = BindingFlags.NonPublic | BindingFlags.Instance;
            t.GetField("text", hidden).SetValue(ni, text);
            if ((bool)t.GetField("added", hidden).GetValue(ni))
                t.GetMethod("UpdateIcon", hidden).Invoke(ni, new object[] { true });
        }

        public static ToolStripMenuItem BuildDropDownMenuEx(ToolStripItemCollection items, string parentName, string name, Type enumType, EventHandler clickEvent, object tag = null, int min = 0, int max = 0, bool noSubItems = false)
        {
            var subMenuName = $"{parentName}_{name}";
            var subMenuItems = items.Find(subMenuName, false);

            if (subMenuItems.Length > 0)
            {
                return subMenuItems[0] as ToolStripMenuItem;
            }

            ToolStripMenuItem subMenuItem;
            subMenuItem = (ToolStripMenuItem)items.Add(name);
            subMenuItem.Name = subMenuName;

            if (noSubItems)
            {
                subMenuItem.Tag = tag;
                subMenuItem.Click += clickEvent;

                return subMenuItem;
            }

            if (enumType != null)
            {
                foreach (var enumValue in Enum.GetValues(enumType))
                {
                    var itemText = (enumValue as IConvertible).GetDescription() ?? enumValue.ToString();
                    var item = subMenuItem.DropDownItems.Add(itemText);
                    item.Tag = tag;
                    var strValue = enumValue.ToString();
                    if (strValue.EndsWith("_"))
                    {
                        strValue = strValue.Substring(0, strValue.Length - 1);
                    }
                    item.AccessibleName = strValue;
                    item.Click += clickEvent;
                }
            }
            else if (max > min)
            {
                List<int> range;

                if (max - min <= 20)
                {
                    range = Enumerable.Range(min, (max - min) + 1).ToList();
                }
                else
                {
                    range = new List<int>();
                    for (var i = min < 0 ? -10 : 0; i <= 10; i++)
                    {
                        range.Add(i * (max / 10));
                    }
                }

                foreach (var value in range)
                {
                    var subSubItemName = $"{subMenuName}_{value}";

                    var item = subMenuItem.DropDownItems.Add(value.ToString());
                    item.Name = subSubItemName;
                    item.Tag = tag;
                    item.Click += clickEvent;
                }
            }
            else
            {
                subMenuItem.Tag = tag;
                subMenuItem.Click += clickEvent;
            }

            return subMenuItem;
        }

        public static void BuildDropDownMenu(ToolStripDropDownItem mnuParent, string name, Type enumType, object colorData, string propertyName, EventHandler clickEvent, Font font = null)
        {
            PropertyInfo property = null;
            var subMenuItems = mnuParent.DropDownItems.Find("miColorSettings_" + name, false);
            ToolStripMenuItem subMenuItem;
            if (subMenuItems.Length == 0)
            {
                subMenuItem = (ToolStripMenuItem)mnuParent.DropDownItems.Add(name);
                subMenuItem.Name = "miColorSettings_" + name;
                if (font != null)
                {
                    subMenuItem.Font = font;
                }

                if (colorData != null)
                {
                    property = colorData.GetType().GetDeclaredProperty(propertyName);
                    subMenuItem.Tag = property;
                }

                foreach (var enumValue in Enum.GetValues(enumType))
                {
                    var item = subMenuItem.DropDownItems.Add(enumValue.ToString());
                    item.Tag = enumValue;
                    item.Click += clickEvent;

                    if (font != null)
                    {
                        item.Font = font;
                    }
                }
            }
            else
            {
                subMenuItem = (ToolStripMenuItem)subMenuItems[0];
                property = (PropertyInfo)subMenuItem.Tag;
            }

            if (colorData == null)
            {
                return;
            }
            var value = property.GetValue(colorData);

            foreach (var item in subMenuItem.DropDownItems)
            {
                if (item is ToolStripMenuItem)
                {
                    var menuItem = (ToolStripMenuItem)item;
                    if (menuItem.Tag != null)
                    {
                        menuItem.Checked = menuItem.Tag.Equals(value);
                    }
                }
            }
        }

        public static ToolStripMenuItem BuildMenuItem(ToolStripItemCollection itemCollection, string name, string text, object tag = null, EventHandler onClick = null)
        {
            var items = itemCollection.Find(name, false);
            var item = (ToolStripMenuItem)items.FirstOrDefault();

            if (item == null)
            {
                item = new ToolStripMenuItem(text) { Name = name, Tag = tag };
                itemCollection.Add(item);

                if (onClick != null)
                {
                    item.Click += onClick;
                }
            }
            else
            {
                item.Text = text;
                item.Tag = tag;
            }
            //else if (onClick != null)
            //{
            //    RemoveEvents(item, "Click");
            //}

            return item;
        }

        public static void BuildComboBox<T>(ComboBox comboBox, params T[] skip) where T : IConvertible
        {
            if (comboBox.Items.Count == 0)
            {
                foreach (var enumValue in Enum.GetValues(typeof(T)))
                {
                    if (skip.Contains((T)enumValue))
                    {
                        continue;
                    }
                    var item = new EnumComboBoxItem
                    {
                        Name = ((T)enumValue).GetDescription(),
                        Id = (int)enumValue
                    };

                    comboBox.Items.Add(item);
                }
            }
        }

        public static int SetComboBoxEnumIndex(ComboBox comboBox, int value)
        {
            if (comboBox.Items.Count == 0)
            {
                return -1;
            }

            var index = 0;

            for (var i = 0; i < comboBox.Items.Count; i++)
            {
                var item = comboBox.Items[i] as EnumComboBoxItem;
                if (item.Id == value)
                {
                    index = i;
                    break;
                }
            }

            comboBox.SelectedIndex = index;

            return index;
        }

        public static T GetComboBoxEnumItem<T>(ComboBox comboBox) where T : IConvertible
        {
            if (comboBox.SelectedIndex == -1)
            {
                return (T)Enum.Parse(typeof(T), "None");
            }

            var item = comboBox.SelectedItem as EnumComboBoxItem;

            return (T)Enum.Parse(typeof(T), item.Id.ToString());
        }

        public static void InitListView(ListView listView, IEnumerable<string> columns)
        {
            foreach (var name in columns)
            {
                var columnName = name;
                var parts = name.Split('|');

                var width = 120;
                if (parts.Length > 1)
                {
                    width = int.Parse(parts[1]);
                    columnName = parts[0];
                }

                var header = listView.Columns.Add(columnName);
                header.Width = width == 120 ? -2 : width;
            }
        }

        public static void AddOrUpdateListItem<T>(ListView listView, List<T> presets, Config config, T preset = null, ListViewItem specItem = null) where T : PresetBase
        {
            ListViewItem item = null;
            if (preset == null)
            {
                item = listView.SelectedItems[0];
                preset = (T)item.Tag;
            }
            else
            {
                item = specItem;
            }

            if (preset.id == 0)
            {
                preset.id = preset.GetHashCode();
            }

            var values = preset.GetDisplayValues(config);

            if (item == null)
            {
                item = listView.Items.Add(values[0]);
                item.Tag = preset;
                item.Checked = preset.ShowInQuickAccess;
                for (var i = 1; i < values.Count; i++)
                {
                    item.SubItems.Add(values[i]);
                }
                if (!presets.Any(x => x.id == preset.id))
                {
                    presets.Add(preset);
                    listView.SelectedIndices.Clear();
                    listView.SelectedIndices.Add(item.Index);
                }
            }
            else
            {
                item.Text = values[0];
                item.Checked = preset.ShowInQuickAccess;
                for (var i = 1; i < values.Count; i++)
                {
                    if (item.SubItems.Count == i)
                    {
                        item.SubItems.Add(values[i]);
                    }
                    else
                    {
                        item.SubItems[i].Text = values[i];
                    }
                }
            }

            if (listView.ListViewItemSorter is ListViewColumnSorter sorter && sorter?.Order != SortOrder.None)
            {
                listView.Sort();
            }
        }

        public static bool IsForegroundFullScreenAndDisabledNotifications(Screen screen = null)
        {
            return IsNotificationDisabled() && IsForegroundFullScreen(screen);
        }

        public static bool IsNotificationDisabled()
        {
            var result = SHQueryUserNotificationState(out var state);

            if (result == NativeConstants.ERROR_SUCCESS && state != LastNotificationState)
            {
                Logger.Debug($"Detected NotificationState change from {LastNotificationState} to {state}");
                LastNotificationState = state;
            }

            return NotificationsDisabledStates.Contains(state);
            //return state != UserNotificationState.QUNS_ACCEPTS_NOTIFICATIONS && state != UserNotificationState.QUNS_QUIET_TIME;
        }

        public static bool IsForegroundFullScreen(Screen screen = null)
        {
            if (screen == null)
            {
                screen = Screen.PrimaryScreen;
            }
            var hWnd = NativeMethods.GetForegroundWindow();

            NativeMethods.GetWindowRect(hWnd, out var rect);

            /* in case you want the process name:
            uint procId = 0;
            GetWindowThreadProcessId(hWnd, out procId);
            var proc = System.Diagnostics.Process.GetProcessById((int)procId);
            Console.WriteLine(proc.ProcessName);
            */

            return screen.Bounds.Width <= (rect.right - rect.left) && screen.Bounds.Height <= (rect.bottom - rect.top);
        }

        public static (uint, bool) GetForegroundProcessIdAndIfFullScreen(Screen screen = null)
        {
            if (screen == null)
            {
                screen = Screen.PrimaryScreen;
            }
            var hWnd = NativeMethods.GetForegroundWindow();

            if (hWnd == IntPtr.Zero)
            {
                return (0, false);
            }

            NativeMethods.GetWindowRect(hWnd, out var rect);

            GetWindowThreadProcessId(hWnd, out var processId);

            var isFullScreen = screen.Bounds.Width <= (rect.right - rect.left) && screen.Bounds.Height <= (rect.bottom - rect.top);

            return (processId, isFullScreen);
        }

        public static Process GetForegroundFullScreenProcess(Screen screen = null)
        {
            var (processId, isFullScreen) = GetForegroundProcessIdAndIfFullScreen(screen);

            if (!isFullScreen)
            {
                return null;
            }

            return Process.GetProcessById((int)processId);
        }

        public static void EnableControls(Control parent, bool enable = true, IEnumerable<Control> excludedControls = null)
        {
            for (var i = 0; i < parent.Controls.Count; i++)
            {
                var control = parent.Controls[i];
                if (!excludedControls?.Contains(control) ?? true)
                {
                    control.Enabled = enable;
                }
            }
        }

        public static void AddStepToTextBox(TextBox textBox, string step)
        {
            var text = textBox.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                text = step;
            }
            else
            {
                var pos = textBox.SelectionStart;
                while (pos < text.Length && text.CharAt(pos) != ',')
                {
                    pos++;
                }
                if (pos == text.Length)
                {
                    text += ", " + step;
                }
                else
                {
                    text = text.Substring(0, pos + 1) + " " + step + ", " + text.Substring(pos + 1).Trim();
                }
            }
            textBox.Text = text.Trim();
        }

        public static T GetSelectedItemTag<T>(this ListView listView)
        {
            if (listView.SelectedItems.Count > 0)
            {
                var item = listView.SelectedItems[0];
                return (T)item.Tag;
            }
            else
            {
                return default(T);
            }
        }

        public static void FireEvent(this Control ctrl, string eventName, params object[] eventParams)
        {
            //EventSelectedIndexChanged
            var propertyInfo = ctrl.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            var eventHandlerList = propertyInfo.GetValue(ctrl) as EventHandlerList;

            var headField = typeof(EventHandlerList).GetField("_head", BindingFlags.NonPublic | BindingFlags.Instance);

            var assembly = typeof(EventHandlerList).Assembly;
            var listEntryType = assembly.GetTypes().FirstOrDefault(t => t.Name == "ListEntry");

            var nextField = listEntryType.GetField("_next", BindingFlags.NonPublic | BindingFlags.Instance);
            var handlerField = listEntryType.GetField("_handler", BindingFlags.NonPublic | BindingFlags.Instance);

            var entry = headField.GetValue(eventHandlerList);
            Delegate foundHandler = null;

            while (entry != null)
            {
                var handler = handlerField.GetValue(entry) as Delegate;

                if (handler.Method.Name.Contains(eventName))
                {
                    foundHandler = handler;
                    break;
                }

                entry = nextField.GetValue(entry);
            }

            if (foundHandler == null)
            {
                return;
            }

            var invocationList = foundHandler.GetInvocationList();
            foreach (var item in invocationList)
            {
                item.Method.Invoke(item.Target, eventParams);
            }
        }

        public static void RemoveEvents(object ctrl, string eventName)
        {
            var propertyInfo = ctrl.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            var eventHandlerList = propertyInfo.GetValue(ctrl) as EventHandlerList;

            eventHandlerList.Dispose();
        }

        public static void ListViewItemChecked<T>(ListView listView, ItemCheckedEventArgs e) where T : PresetBase
        {
            var checkedPreset = (T)e.Item.Tag;

            if (checkedPreset == null || checkedPreset.ShowInQuickAccess == e.Item.Checked)
            {
                return;
            }

            var point = listView.PointToClient(Cursor.Position);

            if (point.X >= 20)
            {
                e.Item.Checked = !e.Item.Checked;
                return;
            }

            checkedPreset.ShowInQuickAccess = e.Item.Checked;

            var preset = listView.GetSelectedItemTag<T>();

            if (preset == checkedPreset)
            {
                listView.FireEvent("SelectedIndexChanged", listView, e);
            }
        }

        public static string ExtendedDisplayName(string displayName)
        {
            var name = displayName;
            var screen = Screen.AllScreens.FirstOrDefault(x => x.DeviceName.Equals(name));
            if (screen != null)
            {
                name += " (" + screen.DeviceFriendlyName() + ")";
            }

            return name;
        }

        public static string EditShortcut(string shortcut, string label = "Shortcut", string title = null)
        {
            var field = new MessageForms.FieldDefinition
            {
                Label = label,
                FieldType = MessageForms.FieldType.Shortcut,
                Value = shortcut
            };
            var values = MessageForms.ShowDialog(title ?? "Set shortcut", new[] { field });
            if (!values.Any())
            {
                return null;
            }

            return values.First().Value.ToString();
        }

        public static void ListViewSort(object sender, ColumnClickEventArgs e)
        {
            var listView = (ListView)sender;
            var sorter = (ListViewColumnSorter)listView.ListViewItemSorter;

            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == sorter.SortColumn)
            {
                //if (sorter.Order == SortOrder.None)
                //{
                //    sorter.Order = SortOrder.Ascending;
                //}
                // Reverse the current sort direction for this column.
                if (sorter.Order == SortOrder.Ascending)
                {
                    sorter.Order = SortOrder.Descending;
                }
                else
                {
                    sorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                sorter.SortColumn = e.Column;
                sorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            listView.Sort();
        }

        public static void BuildServicePresetsMenu<T>(ToolStripMenuItem menu, ServiceBase<T> service, string name, EventHandler eventHandler) where T : PresetBase, new()
        {
            menu.DropDownItems.Clear();

            foreach (var nvPreset in service?.GetPresets() ?? new List<T>())
            {
                var text = nvPreset.name;

                if (!string.IsNullOrEmpty(text))
                {
                    var item = menu.DropDownItems.Add(text);
                    item.Tag = nvPreset;
                    item.Click += eventHandler;
                }
            }

            menu.Visible = service != null;

            menu.Text = menu.DropDownItems.Count > 0 ? $"{name} presets" : $"{name} presets (no named presets found)";
        }

        public static void ShowControls(Control parent, bool show = true, Control exclude = null)
        {
            for (var i = 0; i < parent.Controls.Count; i++)
            {
                var control = parent.Controls[i];
                if (control != exclude)
                {
                    control.Visible = show;
                }
            }
        }

        public static void InitSortState(ListView listView, ListViewSortState sortState)
        {
            var sorter = new ListViewColumnSorter();
            sorter.SortColumn = sortState.SortIndex;
            sorter.Order = sortState.SortOrder;
            listView.ListViewItemSorter = sorter;
        }

        public static void SaveSortState(IComparer comparer, ListViewSortState sortState)
        {
            if (!(comparer is ListViewColumnSorter sorter))
            {
                return;
            }

            sortState.SortOrder = sorter.Order;
            sortState.SortIndex = sorter.SortColumn;
        }
    }

    public class EnumComboBoxItem
    {
        public string Name;
        public int Id;

        public override string ToString()
        {
            return Name;
        }
    }
}
