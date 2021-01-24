using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
