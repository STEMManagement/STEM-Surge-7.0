using System;
using System.ServiceProcess;
using System.IO;
using System.Diagnostics;

namespace STEM.SurgeService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            System.AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            System.Environment.CurrentDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                       
            bool isRestart = args.Length == 1 && args[0].Equals("-r", StringComparison.InvariantCultureIgnoreCase);

            int pid = System.Diagnostics.Process.GetCurrentProcess().Id;

            bool isWindows = System.Runtime.InteropServices.RuntimeInformation.OSDescription.StartsWith("Microsoft Windows", StringComparison.InvariantCultureIgnoreCase);

            if (!isRestart)
            {
                foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcessesByName("STEM.SurgeService.R"))
                    try
                    {
                        p.Kill();
                    }
                    catch { }

                try
                {
                    string r = Path.Combine(System.Environment.CurrentDirectory, "STEM.SurgeService.R.exe");

                    while (File.Exists(r))
                        try
                        {
                            File.Delete(r);
                        }
                        catch { }
                }
                catch { }
            }
            else
            {
                try
                {
                    while (true)
                    {
                        bool running = false;
                        foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcessesByName("STEM.SurgeService"))
                        {
                            if (p.Id == pid)
                                continue;

                            running = true;
                            break;
                        }

                        if (!running)
                            break;

                        System.Threading.Thread.Sleep(1000);
                    }

                    ProcessStartInfo si = new ProcessStartInfo();
                    si.CreateNoWindow = true;
                    si.UseShellExecute = true;

                    if (!isWindows)
                    {
                        si.FileName = "systemctl";
                        si.Arguments = "start STEM.Surge";
                    }
                    else
                    {
                        si.FileName = "SC.EXE";
                        si.Arguments = "start STEM.Surge";
                    }

                    Process.Start(si);
                }
                catch { }

                return;
            }

            if (args.Length == 0)
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new SurgeService()
                };
                ServiceBase.Run(ServicesToRun);
            }
            else
            {
                SurgeService svc = new SurgeService();
                svc.Start(args);

                while (true)
                    System.Threading.Thread.Sleep(10000);
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            System.Diagnostics.EventLog.WriteEntry("STEM.SurgeService", ((Exception)e.ExceptionObject).ToString(), System.Diagnostics.EventLogEntryType.Error);
        }
    }
}
