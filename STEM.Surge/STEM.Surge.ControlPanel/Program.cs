using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STEM.Surge.ControlPanel
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            System.Environment.CurrentDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ControlPanel());
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            STEM.Sys.EventLog.WriteEntry("Surge.ControlPanel", ((Exception)e.ExceptionObject).ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
        }
    }
}
