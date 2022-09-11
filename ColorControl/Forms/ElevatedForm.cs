using ColorControl.Native;
using NWin32;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorControl.Forms
{
    public partial class ElevatedForm : Form
    {
        public ElevatedForm()
        {
            File.Delete("d:\\Msg.txt");
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            Load += ElevatedForm_Load;
            Shown += ElevatedForm_Shown;

            var filterStatus = new WinApi.CHANGEFILTERSTRUCT();
            filterStatus.size = (uint)Marshal.SizeOf(filterStatus);
            filterStatus.info = 0;

            var result = WinApi.ChangeWindowMessageFilterEx(Handle, NativeConstants.WM_COPYDATA, WinApi.ChangeWindowMessageFilterExAction.Allow, ref filterStatus);
            //File.AppendAllLines("d:\\Msg.txt", new[] { "ChangeWindowMessageFilterEx result: " + result });

            Task.Run(() => CheckMutexAsync());
        }

        private void ElevatedForm_Shown(object sender, EventArgs e)
        {
            Hide();
        }

        protected void ElevatedForm_Load(object sender, EventArgs e)
        {
            Size = new Size(0, 0);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeConstants.WM_COPYDATA)
            {
                var container = (WinApi.COPYDATASTRUCT)m.GetLParam(typeof(WinApi.COPYDATASTRUCT));
                var message = Marshal.PtrToStringAnsi(container.lpData);

                File.AppendAllLines("d:\\Msg.txt", new[] { "Message received: " + message });

                HandleMessage(message);

                //Utils.StartProcess("notepad.exe");

                //BackColor = Color.Red;
                //Text = "AHA!";
                //MessageForms.WarningOk("Received!");
            }

            base.WndProc(ref m);
        }

        private void HandleMessage(string message)
        {
            var args = message.Split(' ');

            var startUpParams = StartUpParams.Parse(args);

            Program.HandleStartupParams(startUpParams, null);
        }

        private void CheckMutexAsync()
        {
            var mutex = new Mutex(false, Program.MutexId, out var created);

            File.AppendAllLines("d:\\Msg.txt", new[] { "Mutex created: " + created });

            try
            {
                mutex.WaitOne();
                File.AppendAllLines("d:\\Msg.txt", new[] { "Mutex released?" });
            }
            catch (AbandonedMutexException)
            {
                File.AppendAllLines("d:\\Msg.txt", new[] { "Mutex abandoned" });
            }
            BeginInvoke(() => Application.Exit());
        }
    }
}
