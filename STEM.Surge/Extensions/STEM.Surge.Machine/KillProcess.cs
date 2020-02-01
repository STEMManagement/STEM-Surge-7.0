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
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using STEM.Surge;

namespace STEM.Surge.Machine
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("KillProcess")]
    [Description("Kill the process named 'Executable Name' on the machine 'Machine IP'.")]
    public class KillProcess : Instruction
    {
        [DisplayName("Executable Name"), DescriptionAttribute("What is the executing program name?")]
        public string ProcessName { get; set; }
        [DisplayName("Machine IP"), DescriptionAttribute("What machine is the process running on?")]
        public string MachineIP { get; set; }
        [DisplayName("Number of retries"), DescriptionAttribute("How many times should each operation be attempted?")]
        public int Retry { get; set; }
        [DisplayName("Seconds between retries"), DescriptionAttribute("How many seconds should be waited between retries?")]
        public double RetryDelaySeconds { get; set; }

        public KillProcess() : base()
        {
            Retry = 1;
            RetryDelaySeconds = 2;
            ProcessName = "ProcessName";
            MachineIP = "10.0.0.100";
        }

        public KillProcess(string processName, string machineIP, int retry, double retryDelaySeconds)
            : base()
        {
            ProcessName = processName;
            MachineIP = machineIP;
            Retry = retry;
            RetryDelaySeconds = retryDelaySeconds;
        }

        protected override bool _Run()
        {
            foreach (string ip in STEM.Sys.IO.Path.ExpandRangedIP(MachineIP))
            {
                foreach (Process p in Process.GetProcessesByName(ProcessName, ip))
                {
                    int retry = 0;
                    while (retry++ <= Retry && !Stop)
                    {
                        try
                        {
                            p.Kill();
                            Message += ProcessName + " on " + ip + " killed.\r\n";
                            break;
                        }
                        catch (Exception ex)
                        {
                            if (retry >= Retry)
                            {
                                Exceptions.Add(ex);
                                Message += ex.Message + "\r\n";
                                break;
                            }
                            else
                            {
                                Thread.Sleep(TimeSpan.FromSeconds(RetryDelaySeconds));
                            }
                        }
                    }
                }
            }

            return (Exceptions.Count == 0);
        }

        protected override void _Rollback()
        {
            throw new Exception("Kill Process cannot be rolled back.");
        }
    }
}
