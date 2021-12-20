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
using System.Xml.Serialization;
using STEM.Surge.Messages;

namespace STEM.Surge
{
    /// <summary>
    /// The definition for a DeploymentControllers poller
    /// </summary>
    public class PollerEntry
    {
        public PollerEntry(string sourceIP)
        {
            PollTimeMilliseconds = 0;

            PingableSource = false;

            LimitBranchIPs = "";
            SandboxID = "";

            Backlog = new Backlogs.Entry();

            _SourceIP = sourceIP;
        }
        
        public double PollTimeMilliseconds { get; set; }

        public string DeploymentManagerIP { get; set; }
        public bool AllowCrossBranchAssignment { get; set; }

        [XmlIgnore]
        public string AuthenticationConfiguration { get; set; }
                
        public string SourceDirectory { get; set; }


        public string FileFilter { get; set; }
        public string DirectoryFilter { get; set; }
        public bool Recurse { get; set; }
        public bool PingableSource { get; set; }
        public TimeSpan PollInterval { get; set; }
        
        string _LimitBranchIPs = "";
        public string LimitBranchIPs
        {
            set
            {
                string ret = "";

                if (value != null)
                    foreach (string addr in STEM.Sys.IO.Path.ExpandRangedIP(value))
                    {
                        string parsed = STEM.Sys.IO.Net.MachineAddress(addr);

                        if (parsed != System.Net.IPAddress.None.ToString() && parsed != null)
                        {
                            if (String.IsNullOrEmpty(ret))
                                ret = parsed.ToString();
                            else
                                ret = ret + "#" + parsed.ToString();
                        }
                    }

                _LimitBranchIPs = ret;
            }

            get
            {
                return _LimitBranchIPs;
            }
        }

        public string SandboxID { get; set; }

        public Backlogs.Entry Backlog { get; set; }

        string _SourceIP = null;
        public string SourceIP
        {
            get
            {
                if (SourceDirectory != null && _SourceIP == null)
                    _SourceIP = STEM.Sys.IO.Path.IPFromPath(SourceDirectory);

                return _SourceIP;
            }

            set
            {
            }
        }
        
        string _SiblingIPs = "";
        public string SiblingIPs
        {
            set
            {
                string ret = "";

                if (value != null)
                    foreach (string addr in STEM.Sys.IO.Path.ExpandRangedIP(value))
                    {
                        string parsed = STEM.Sys.IO.Net.MachineAddress(addr);

                        if (parsed != System.Net.IPAddress.None.ToString() && parsed != null)
                        {
                            if (String.IsNullOrEmpty(ret))
                                ret = parsed.ToString();
                            else
                                ret = ret + "#" + parsed.ToString();
                        }
                    }

                _SiblingIPs = ret;
            }

            get
            {
                return _SiblingIPs;
            }
        }
    }
}
