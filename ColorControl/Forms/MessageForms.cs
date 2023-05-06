using ColorControl.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace ColorControl.Forms
{
    class MessageForms
    {
        public enum FieldType
        {
            Text,
            Numeric,
            DropDown,
            Flags,
            Shortcut,
            CheckBox,
            TrackBar
        }

        public class FieldDefinition
        {
            public string Label { get; set; }
            public string SubLabel { get; set; }
            public FieldType FieldType { get; set; } = FieldType.Text;
            public IEnumerable<string> Values { get; set; }
            public decimal MinValue { get; set; }
            public decimal MaxValue { get; set; }
            public int NumberOfValues { get; set; }
            public int StepSize { get; set; }
            public object Value { get; set; }

            public int ValueAsInt => int.Parse(Value.ToString());
            public uint ValueAsUInt => uint.Parse(Value.ToString());
            public bool ValueAsBool => bool.Parse(Value.ToString());
        }

        public class MessageForm : Form
        {
            private ToolTip _toolTip;
            public Dictionary<Control, Control> _controls = new();

            public void edtShortcut_KeyDown(object sender, KeyEventArgs e)
            {
                ((TextBox)sender).Text = Utils.FormatKeyboardShortcut(e);
            }

            public void edtShortcut_KeyUp(object sender, KeyEventArgs e)
            {
                Utils.HandleKeyboardShortcutUp(e);
            }

            public void TrackBar_Scroll(object sender, EventArgs e)
            {
                var trackBar = sender as TrackBar;
                var edit = (NumericUpDown)_controls[trackBar];

                var indexDbl = (trackBar.Value * 1.0) / trackBar.TickFrequency;
                var index = Convert.ToInt32(Math.Round(indexDbl));

                var newValue = trackBar.Value < trackBar.Maximum ? trackBar.TickFrequency * index : trackBar.Value;
                newValue = Math.Max(newValue, trackBar.Minimum);
                newValue = Math.Min(newValue, trackBar.Maximum);

                trackBar.Value = newValue;

                edit.Value = trackBar.Value;

                _toolTip ??= new ToolTip();
                _toolTip.Show(trackBar.Value.ToString(), trackBar, 50, 10, 1500);
            }

            public void TrackBarEdit_ValueChanged(object sender, EventArgs e)
            {
                var edit = (NumericUpDown)sender;
                var trackBar = (TrackBar)_controls.First(c => c.Value == edit).Key;

                trackBar.Value = (int)edit.Value;
            }
        }

        public static Form MainForm;

        public static void WarningOk(string text)
        {
            MessageBox.Show(text, MainForm?.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void ErrorOk(string text)
        {
            MessageBox.Show(text, MainForm?.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void InfoOk(string text, string title = null, string url = null)
        {
            if (url != null)
            {
                MessageBox.Show(text, title ?? MainForm?.Text, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, 0, url);
            }
            else
            {
                MessageBox.Show(text, title ?? MainForm?.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public static DialogResult QuestionYesNo(string text)
        {
            return MessageBox.Show(text, MainForm?.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
        }

        public static List<FieldDefinition> ShowDialog(string caption, IEnumerable<string> labels, Func<IEnumerable<FieldDefinition>, string> validateFunc = null)
        {
            var fieldDefinitions = labels.Select(l => new FieldDefinition
            {
                Label = l
            }).ToList();

            return ShowDialog(caption, fieldDefinitions, validateFunc);
        }

        public static List<FieldDefinition> ShowDialog(string caption, IEnumerable<FieldDefinition> fields, Func<IEnumerable<FieldDefinition>, string> validateFunc = null, string okButtonText = "OK")
        {
            var values = new List<FieldDefinition>();

            var prompt = new MessageForm()
            {
                Width = 460,
                Height = 106,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };

            var groupBox = new Panel();
            groupBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            groupBox.Top = 0;
            groupBox.Left = 0;
            groupBox.Width = prompt.Width;
            groupBox.Height = prompt.Height - 80;
            groupBox.BackColor = SystemColors.Window;
            //groupBox.Margin = new Padding(20);

            prompt.Controls.Add(groupBox);

            var top = 10;
            var boxes = new List<Control>();
            var counter = 1;
            const int DefaultLeft = 20;

            foreach (var field in fields)
            {
                var label = field.Label;

                if (field.FieldType != FieldType.CheckBox)
                {
                    var textLabel = new Label() { Left = DefaultLeft, Top = top, Text = label, AutoSize = true };
                    top += 20;
                    groupBox.Controls.Add(textLabel);
                }

                if (field.SubLabel != null)
                {
                    var textSubLabel = new Label() { Left = DefaultLeft, Top = top, Text = field.SubLabel, AutoSize = true };
                    top += 20;
                    groupBox.Controls.Add(textSubLabel);
                }

                Control control;

                switch (field.FieldType)
                {
                    case FieldType.Text:
                        var textBox = new MaskedTextBox() { Left = DefaultLeft, Top = top, Width = 400 };

                        if (label.Contains("Ip-address"))
                        {
                            //textBox.Mask = "990.990.990.990";
                            //textBox.ValidatingType = typeof(System.Net.IPAddress);
                            textBox.Culture = CultureInfo.InvariantCulture;
                        }
                        else if (label.Contains("MAC-address"))
                        {
                            textBox.Mask = "AA-AA-AA-AA-AA-AA";
                            textBox.Culture = CultureInfo.InvariantCulture;
                        }

                        control = textBox;
                        break;

                    case FieldType.Shortcut:
                        var shortcutTextBox = new TextBox()
                        {
                            Left = DefaultLeft,
                            Top = top,
                            Width = 200,
                            Name = $"edtShortcut_{counter}",
                            ReadOnly = true
                        };

                        shortcutTextBox.KeyDown += prompt.edtShortcut_KeyDown;
                        shortcutTextBox.KeyUp += prompt.edtShortcut_KeyUp;

                        if (field.Value != null)
                        {
                            shortcutTextBox.Text = field.Value.ToString();
                        }

                        control = shortcutTextBox;
                        break;

                    case FieldType.Numeric:
                        var numericEdit = new NumericUpDown() { Left = DefaultLeft, Top = top, Width = 200 };

                        if (field.MinValue != field.MaxValue)
                        {
                            numericEdit.Minimum = field.MinValue;
                            numericEdit.Maximum = field.MaxValue;
                        }

                        if (field.Value != null)
                        {
                            numericEdit.Text = field.Value.ToString();
                        }

                        control = numericEdit;
                        break;

                    case FieldType.DropDown:
                        var comboBox = new ComboBox
                        {
                            Left = DefaultLeft,
                            Top = top,
                            Width = 400,
                            DropDownStyle = ComboBoxStyle.DropDownList
                        };

                        if (field.Values != null && field.Values.Any())
                        {
                            comboBox.Items.AddRange(field.Values.ToArray());
                            comboBox.SelectedIndex = field.Value != null ? comboBox.Items.IndexOf(field.Value.ToString()) : 0;

                            if (comboBox.SelectedIndex == -1)
                            {
                                comboBox.SelectedIndex = 0;
                            }
                        }

                        control = comboBox;
                        break;

                    case FieldType.CheckBox:

                        top += 4;

                        var checkBox = new CheckBox
                        {
                            Left = DefaultLeft,
                            Top = top,
                            AutoSize = true,
                            Text = label
                        };

                        checkBox.Checked = field.Value is bool boolValue && boolValue;

                        control = checkBox;
                        break;

                    case FieldType.Flags:
                        var checkedListBox = new CheckedListBox
                        {
                            Left = DefaultLeft,
                            Top = top,
                            Width = 400,
                            Height = 100,
                            MultiColumn = true
                        };

                        top += 80;
                        prompt.Height += 80;

                        if (field.Values != null && field.Values.Any())
                        {
                            var compoundValue = field.Value is uint ? (int)(uint)field.Value : (int)field.Value;
                            var enumValue = 1;
                            foreach (var value in field.Values)
                            {
                                var isChecked = (compoundValue & enumValue) == enumValue;

                                checkedListBox.Items.Add(value, isChecked);

                                enumValue *= 2;
                            }
                        }

                        control = checkedListBox;
                        break;

                    case FieldType.TrackBar:
                        var trackBar = new TrackBar { Left = DefaultLeft, Top = top, Width = 300 };
                        var trackBarEdit = new NumericUpDown() { Left = trackBar.Left + trackBar.Width + 10, Top = top + 10, Width = 90 };

                        trackBarEdit.ValueChanged += prompt.TrackBarEdit_ValueChanged;

                        prompt._controls.Add(trackBar, trackBarEdit);

                        trackBar.Orientation = Orientation.Horizontal;
                        trackBar.TickStyle = TickStyle.Both;
                        trackBar.Scroll += prompt.TrackBar_Scroll;

                        if (field.MinValue != field.MaxValue)
                        {
                            trackBar.Minimum = (int)field.MinValue;
                            trackBar.Maximum = (int)field.MaxValue;

                            if (field.StepSize > 0)
                            {
                                trackBar.TickFrequency = field.StepSize;
                            }
                            else
                            {
                                var tick = (trackBar.Maximum - trackBar.Minimum);
                                trackBar.TickFrequency = tick >= 100000 ? 5000 : 1;
                            }

                            trackBarEdit.Minimum = field.MinValue;
                            trackBarEdit.Maximum = field.MaxValue;
                        }

                        if (field.Value != null)
                        {
                            trackBar.Value = field.Value is uint ? (int)(uint)field.Value : (int)field.Value;
                            trackBarEdit.Value = trackBar.Value;
                        }

                        top += trackBar.Height - 25;
                        prompt.Height += trackBar.Height - 25;

                        groupBox.Controls.Add(trackBarEdit);

                        control = trackBar;
                        break;


                    default:
                        continue;
                }

                top += 30;
                groupBox.Controls.Add(control);
                boxes.Add(control);
                control.Tag = field;
                counter++;
            }

            var maxWidth = boxes.Max(c => c.Width + c.Left) + 20;

            if (groupBox.Width < maxWidth)
            {
                groupBox.Width = maxWidth;
                prompt.Width = groupBox.Width;
            }

            var maxHeight = boxes.Max(c => c.Height + c.Top) + 20;

            if (groupBox.Height < maxHeight)
            {
                prompt.Height = maxHeight + 80;
                groupBox.Height = maxHeight;
            }

            var confirmation = new Button() { Text = okButtonText, Left = prompt.ClientRectangle.Width - 75 - 20, Width = 75, Top = prompt.ClientRectangle.Height - 30 };
            confirmation.Click += (sender, e) =>
            {
                foreach (var box in boxes)
                {
                    var field = (FieldDefinition)box.Tag;

                    switch (field.FieldType)
                    {
                        case FieldType.Flags:
                            var checkedListBox = (CheckedListBox)box;

                            var compoundValue = 0;
                            var enumValue = 1;
                            for (var i = 0; i < checkedListBox.Items.Count; i++)
                            {
                                compoundValue += checkedListBox.GetItemChecked(i) ? enumValue : 0;
                                enumValue *= 2;
                            }

                            field.Value = compoundValue;
                            break;

                        case FieldType.CheckBox:
                            var checkBox = (CheckBox)box;

                            field.Value = checkBox.Checked;

                            break;

                        case FieldType.TrackBar:
                            var trackBar = (TrackBar)box;

                            field.Value = trackBar.Value;

                            break;

                        default:
                            field.Value = box.Text.Trim();
                            break;
                    }
                }


                if (validateFunc != null)
                {
                    var message = validateFunc(fields);

                    if (!string.IsNullOrEmpty(message))
                    {
                        WarningOk(message);
                        return;
                    }
                }

                values.AddRange(fields);

                prompt.Close();
            };
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

            prompt.UpdateTheme(onlyIfDark: true);

            prompt.ShowDialog(MainForm);

            return values;
        }

        public static Form ShowProgress(string caption)
        {
            var prompt = new Form()
            {
                Width = 300,
                Height = 100,
                FormBorderStyle = FormBorderStyle.FixedToolWindow,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen,
                ControlBox = false
            };
            var textLabel = new Label() { Left = 20, Top = 25, Text = caption, AutoSize = true };
            prompt.Controls.Add(textLabel);

            prompt.Show(MainForm);

            return prompt;
        }
    }
}