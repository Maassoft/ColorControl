using ColorControl.Services.Common;
using ColorControl.Shared.Contracts;
using ColorControl.Shared.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ColorControl.Shared.Forms
{
    static class ServiceFormUtils
    {
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

        public static void BuildServicePresetsMenu<T>(ToolStripMenuItem menu, ServiceBase<T> service, string name, EventHandler eventHandler) where T : PresetBase, new()
        {
            menu.DropDownItems.Clear();

            foreach (var nvPreset in service?.GetPresets() ?? new List<T>())
            {
                var text = nvPreset.name;

                if (!string.IsNullOrEmpty(text))
                {
                    var item = menu.DropDownItems.AddCustom(text);
                    item.Tag = nvPreset;
                    item.Click += eventHandler;
                }
            }

            menu.Visible = service != null;

            menu.Text = menu.DropDownItems.Count > 0 ? $"{name} presets" : $"{name} presets (no named presets found)";
        }

        public static void UpdateShortcutTextBox(TextBox edtShortcut, PresetBase preset)
        {
            if (preset == null || string.IsNullOrEmpty(edtShortcut.Text))
            {
                edtShortcut.ForeColor = FormUtils.CurrentForeColor;
            }
            else
            {
                //edtShortcutLg.ForeColor = ShortCutExists(text, preset.id) ? Color.Red : SystemColors.WindowText;
            }
        }
    }
}
