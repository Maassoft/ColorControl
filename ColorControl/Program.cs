using System;
using System.Threading;
using System.Windows.Forms;

namespace ColorControl
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var startUpParams = StartUpParams.Parse(args);

            if (startUpParams.ActivateChromeFontFix || startUpParams.DeactivateChromeFontFix)
            {
                Utils.InstallChromeFix(startUpParams.ActivateChromeFontFix);
                return;
            }

            string mutexId = $"Global\\{typeof(MainForm).GUID}";
            bool mutexCreated;
            var mutex = new Mutex(true, mutexId, out mutexCreated);
            try
            {
                if (!mutexCreated)
                {
                    MessageBox.Show("Only one instance of this program can be active.", "ColorControl");
                }
                else
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new MainForm(startUpParams));
                }
            }
            finally
            {
                if (mutexCreated)
                {
                    mutex.Dispose();
                }
            }
        }
    }
}
