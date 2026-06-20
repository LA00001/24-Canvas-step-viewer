using System;
using System.Windows.Forms;

namespace InventorIptOrg
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            try
            {
                Application.ThreadException += delegate(object sender, System.Threading.ThreadExceptionEventArgs e)
                {
                    try { AppLogger.LogException("Application.ThreadException", e.Exception); } catch { }
                    MessageBox.Show(e.Exception.ToString(), "Inventor IPT Organizer fatal UI exception");
                };

                AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs e)
                {
                    try { AppLogger.Log("UNHANDLED_EXCEPTION", "AppDomain.CurrentDomain.UnhandledException", e.ExceptionObject == null ? string.Empty : e.ExceptionObject.ToString()); } catch { }
                };

                AppLogger.Initialize();
                AppLogger.Log("APP_MAIN_START", nameof(Main), "Application starting");

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());

                AppLogger.Log("APP_MAIN_END", nameof(Main), "Application closed");
            }
            catch (Exception ex)
            {
                try { AppLogger.LogException("Program.Main", ex); } catch { }
                MessageBox.Show(ex.ToString(), "Inventor IPT Organizer startup exception");
            }
        }
    }
}
