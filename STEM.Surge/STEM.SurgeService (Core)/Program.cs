/*
 * Copyright 2019 STEM Management
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 */

using System;
using System.Linq;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace STEM.SurgeService
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            System.AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            System.Environment.CurrentDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

            bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);

            int pid = System.Diagnostics.Process.GetCurrentProcess().Id;

            //System.Threading.Thread.Sleep(10000);

            bool isRestart = args.Length == 1 && args[0].Equals("-r", StringComparison.InvariantCultureIgnoreCase);

            //if ((System.Environment.UserInteractive && args.Length == 0) || (args.Length > 0 && args[0].Equals("-g", StringComparison.InvariantCultureIgnoreCase)))
            //{
            //}
            //else
            {
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
                            foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcessesByName("dotnet"))
                            {
                                if (p.Id == pid)
                                    continue;

                                foreach (ProcessModule m in p.Modules)
                                {
                                    if (m.ModuleName.Equals("STEM.SurgeService.dll", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        running = true;
                                        break;
                                    }
                                }

                                if (running)
                                    break;
                            }

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
                        si.UseShellExecute = false;

                        if (!isWindows)
                        {
                            si.FileName = "systemctl";
                            si.Arguments = "start STEM.SurgeService.service";
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

                IHostBuilder hostBuilder = null;

                if (isWindows)
                {
                    hostBuilder = Host.CreateDefaultBuilder(args)
                            .UseWindowsService()
                            .ConfigureServices((hostContext, services) =>
                            {
                                services.AddHostedService<Worker>();
                            });
                }
                else
                {
                    hostBuilder = Host.CreateDefaultBuilder(args)
                            .UseSystemd()
                            .ConfigureServices((hostContext, services) =>
                            {
                                services.AddHostedService<Worker>();
                            });
                }

                hostBuilder.Build().Run();
            }
        }
        
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            System.Diagnostics.EventLog.WriteEntry("STEM.SurgeService", ((Exception)e.ExceptionObject).ToString(), System.Diagnostics.EventLogEntryType.Error);
        }
    }
}
