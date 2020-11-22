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
            var currentDomain = AppDomain.CurrentDomain;
            // Handler for unhandled exceptions.
            currentDomain.UnhandledException += GlobalUnhandledExceptionHandler;
            // Handler for exceptions in threads behind forms.
            Application.ThreadException += GlobalThreadExceptionHandler;

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
                    try
                    {
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Application.Run(new MainForm(startUpParams));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error while initializing application: " + ex.ToLogString(Environment.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
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

        private static void GlobalUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            MessageBox.Show("Unhandled exception: " + ex.ToLogString(Environment.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void GlobalThreadExceptionHandler(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            var ex = e.Exception;
            MessageBox.Show("Exception in thread: " + ex.ToLogString(Environment.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
