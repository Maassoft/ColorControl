using ColorControl.Shared.Common;
using ColorControl.Shared.Contracts;
using NStandard;
using NWin32;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ColorControl.Shared.Forms
{
    public static class FormUtils
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

        public static Color DarkModeForeColor { get; set; } = Color.FromArgb(255, 255, 255);
        public static Color DarkModeBackColor { get; set; } = Color.FromArgb(28, 28, 28);
        public static Color MenuItemForeColor { get; set; } = Color.Black;
        public static Color CurrentForeColor { get; set; } = SystemColors.ControlText;
        public static Color CurrentBackColor { get; set; } = Color.White;

        [DllImport("shell32.dll")]
        public static extern int SHQueryUserNotificationState(out UserNotificationState pquns);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static UserNotificationState LastNotificationState;

        public static void SetNotifyIconText(NotifyIcon ni, string text)
        {
            ni.Text = text.Substring(0, Math.Min(text.Length, 127));
        }

        public static ToolStripItem AddCustom(this ToolStripItemCollection collection, string text)
        {
            var item = collection.Add(text);
            item.ForeColor = MenuItemForeColor;

            return item;
        }

        public static void ShowCustom(this ContextMenuStrip contextMenu, Control control)
        {
            DarkModeUtils.SetContextMenuForeColor(contextMenu, MenuItemForeColor);

            contextMenu.Show(control, control.PointToClient(Cursor.Position));
        }

        public static Control FindFocusedControl(this Form form)
        {
            ContainerControl container = form ?? Form.ActiveForm;
            Control control = null;
            while (container != null)
            {
                control = container.ActiveControl;
                container = control as ContainerControl;
            }

            return control;
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
            subMenuItem = (ToolStripMenuItem)items.AddCustom(name);
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
                    var item = subMenuItem.DropDownItems.AddCustom(itemText);
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

                    var item = subMenuItem.DropDownItems.AddCustom(value.ToString());
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

        public static ToolStripMenuItem BuildDropDownMenu(ToolStripDropDownItem mnuParent, string name, Type enumType, object colorData, string propertyName, EventHandler clickEvent, Font font = null, bool unchanged = false, object[] skipValues = null)
        {
            PropertyInfo property = null;
            var subMenuItems = mnuParent.DropDownItems.Find("miColorSettings_" + name, false);
            ToolStripMenuItem subMenuItem;
            if (subMenuItems.Length == 0)
            {
                subMenuItem = (ToolStripMenuItem)mnuParent.DropDownItems.AddCustom(name);
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

                if (unchanged)
                {
                    var item = subMenuItem.DropDownItems.AddCustom("Unchanged");
                    item.Click += clickEvent;

                    if (font != null)
                    {
                        item.Font = font;
                    }
                }

                foreach (var enumValue in Enum.GetValues(enumType))
                {
                    if (skipValues?.Contains(enumValue) == true)
                    {
                        continue;
                    }

                    var text = (enumValue as IConvertible)?.GetDescription() ?? enumValue.ToString();

                    var item = subMenuItem.DropDownItems.AddCustom(text);
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
                subMenuItem.ForeColor = MenuItemForeColor;
                property = (PropertyInfo)subMenuItem.Tag;
            }

            if (colorData == null)
            {
                return subMenuItem;
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
                    else
                    {
                        menuItem.Checked = value == null;
                    }
                }
            }
            return subMenuItem;
        }

        public static ToolStripMenuItem BuildMenuItem(ToolStripItemCollection itemCollection, string name, string text, object tag = null, EventHandler onClick = null)
        {
            var items = itemCollection.Find(name, false);
            var item = (ToolStripMenuItem)items.FirstOrDefault();

            if (item == null)
            {
                item = new ToolStripMenuItem(text) { Name = name, Tag = tag };
                item.ForeColor = MenuItemForeColor;
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
                item.ForeColor = MenuItemForeColor;
            }
            //else if (onClick != null)
            //{
            //    RemoveEvents(item, "Click");
            //}

            return item;
        }

        public static void BuildMenuItemAndSub(ToolStripMenuItem parent, string text, string subText, EventHandler onClick)
        {
            var itemName = $"{parent.Name}_{text}";
            var menuItem = BuildMenuItem(parent.DropDownItems, itemName, text);

            var subItemName = $"{itemName}_SubItem";
            var subItem = BuildMenuItem(menuItem.DropDownItems, subItemName, "", onClick: onClick);
            subItem.Text = subText;
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
                if (control is not Label && (!excludedControls?.Contains(control) ?? true))
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

        public static IntPtr GetMonitorForDisplayName(string displayName)
        {
            var name = displayName;
            var screen = Screen.AllScreens.FirstOrDefault(x => x.DeviceName.Equals(name));
            if (screen != null)
            {
                return screen.GetHashCode();
            }

            return IntPtr.Zero;
        }

        public static string EditShortcut(string shortcut, string label = "Shortcut", string title = null)
        {
            var field = new FieldDefinition
            {
                Label = label,
                FieldType = FieldType.Shortcut,
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

        public static IAsyncResult BeginInvokeCheck(Control control, Delegate action)
        {
            return BeginInvokeCheck(control, action, null);
        }

        public static IAsyncResult BeginInvokeCheck(Control control, Delegate action, params object[] args)
        {
            if (control?.IsHandleCreated != true)
            {
                Logger.Debug($"Skipping BeginInvoke due to missing handle of control {control.Name}");

                return null;
            }

            return control.BeginInvoke(action, args);
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
