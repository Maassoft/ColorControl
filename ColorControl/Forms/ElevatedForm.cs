using ColorControl.Native;
using NWin32;
using System;
using System.Drawing;
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
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            Load += ElevatedForm_Load;
            Shown += ElevatedForm_Shown;

            var filterStatus = new WinApi.CHANGEFILTERSTRUCT();
            filterStatus.size = (uint)Marshal.SizeOf(filterStatus);
            filterStatus.info = 0;

            WinApi.ChangeWindowMessageFilterEx(Handle, NativeConstants.WM_COPYDATA, WinApi.ChangeWindowMessageFilterExAction.Allow, ref filterStatus);

            Task.Run(CheckMutexAsync);
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

                HandleMessage(message);
            }

            base.WndProc(ref m);
        }

        private void HandleMessage(string message)
        {
            var args = message.Split(' ');

            var startUpParams = StartUpParams.Parse(args);

            try
            {
                var _ = CommandLineHandler.HandleStartupParams(startUpParams, null);
            }
            catch (Exception)
            {

            }
        }

        private void CheckMutexAsync()
        {
            var mutex = new Mutex(false, Program.MutexId, out var created);
            try
            {
                mutex.WaitOne();
            }
            catch (AbandonedMutexException)
            {
            }
            BeginInvoke(Application.Exit);
        }
    }
}
