using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace ColorControl
{
    class MessageForms
    {
        public static Form MainForm;

        public static void WarningOk(string text)
        {
            MessageBox.Show(text, MainForm?.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void ErrorOk(string text)
        {
            MessageBox.Show(text, MainForm?.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void InfoOk(string text)
        {
            MessageBox.Show(text, MainForm?.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static DialogResult QuestionYesNo(string text)
        {
            return MessageBox.Show(text, MainForm?.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
        }

        public static List<string> ShowDialog(string caption, IEnumerable<string> labels, Func<List<string>, string> validateFunc = null)
        {
            var values = new List<string>();

            var prompt = new Form()
            {
                Width = 500,
                Height = 90 + (labels.Count() * 55),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };

            var top = 10;
            var boxes = new List<TextBoxBase>();

            foreach (var label in labels)
            {
                var textLabel = new Label() { Left = 40, Top = top, Text = label };
                top += 25;
                prompt.Controls.Add(textLabel);

                var textBox = new MaskedTextBox() { Left = 40, Top = top, Width = 400 };

                if (label.Contains("Ip-address"))
                {
                    textBox.Mask = "###.###.###.###";
                    textBox.ValidatingType = typeof(System.Net.IPAddress);
                    textBox.Culture = CultureInfo.InvariantCulture;
                }
                else if (label.Contains("MAC-address"))
                {
                    textBox.Mask = "AA-AA-AA-AA-AA-AA";
                    textBox.Culture = CultureInfo.InvariantCulture;
                }

                top += 30;
                prompt.Controls.Add(textBox);
                boxes.Add(textBox);
            }

            var confirmation = new Button() { Text = "Add", Left = 365, Width = 75, Top = prompt.ClientRectangle.Height - 30 };
            confirmation.Click += (sender, e) =>
            {
                values.AddRange(boxes.Select(b => b.Text.Trim()));

                if (validateFunc != null)
                {
                    var message = validateFunc(values);

                    if (!string.IsNullOrEmpty(message))
                    {
                        WarningOk(message);
                        values.Clear();
                        return;
                    }
                }
                prompt.Close();
            };
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

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