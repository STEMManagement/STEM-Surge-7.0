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
    [DisplayName("ExecuteCommandLine")]
    [Description("Execute a command line tool with optional command line arguments.")]
    public class ExecuteCommandLine : Instruction
    {
        [DisplayName("Executable Name"), DescriptionAttribute("What is the command line executable name (use the full path if necessary)?")]
        public string ProcessName { get; set; }
        [DisplayName("Command Line Arguments"), DescriptionAttribute("What would be typed after the command line executable name if you were running this manually?")]
        public string Arguments { get; set; }
        [DisplayName("Number of retries"), DescriptionAttribute("How many times should each operation be attempted?")]
        public int Retry { get; set; }
        [DisplayName("Seconds between retries"), DescriptionAttribute("How many seconds should be waited between retries?")]
        public double RetryDelaySeconds { get; set; }

        protected override bool _Run()
        {
            int retry = 0;
            while (retry++ <= Retry && !Stop)
            {
                try
                {
                    Process p = new Process();
                    p.StartInfo = new ProcessStartInfo(ProcessName, Arguments);
                    p.Start();

                    Message = ProcessName + " executed.";
                    return true;
                }
                catch (Exception ex)
                {
                    if (retry >= Retry)
                    {
                        Exceptions.Add(ex);
                        Message = ex.Message;
                        break;
                    }
                    else
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(RetryDelaySeconds));
                    }
                }
            }

            return false;
        }

        protected override void _Rollback()
        {
            if (Stage == STEM.Surge.Stage.Ready)
                return;

            foreach (Process p in Process.GetProcessesByName(ProcessName))
            {
                int retry = 0;
                while (retry <= Retry && !Stop)
                {
                    try
                    {
                        p.Kill();
                        Message = ProcessName + " killed.";
                        return;
                    }
                    catch (Exception ex)
                    {
                        if (retry >= Retry)
                        {
                            Exceptions.Add(ex);
                            Message += ex.Message;
                            return;
                        }
                        else
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(RetryDelaySeconds));
                        }
                    }

                    retry++;
                }
            }
        }
    }
}
