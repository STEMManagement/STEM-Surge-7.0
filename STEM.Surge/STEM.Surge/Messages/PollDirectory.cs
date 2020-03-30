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
using System.Collections.Generic;
using System.Linq;
using STEM.Sys.Messaging;
using STEM.Sys.IO.TCP;

namespace STEM.Surge.Messages
{    
    public class PollDirectory : STEM.Sys.Messaging.Message
    {
        public enum PollerTarget { Files, Directories }

        public string PollerServerIP { get; set; }
        public string DeploymentControllerID { get; set; }
        public string Directory { get; set; }
        public string DirectoryFilter { get; set; }
        public string FileFilter { get; set; }
        public bool Recurse { get; set; }
        public STEM.Sys.Serialization.TimeSpan PollInterval { get; set; }
        public PollerTarget TargetType { get; set; }

        internal List<MessageConnection> DeploymentManagers = new List<MessageConnection>();

        public PollDirectory()
        {
            PollInterval = new Sys.Serialization.TimeSpan(5000);
        }

        public void AddDeploymentManager(MessageConnection connection)
        {
            lock (DeploymentManagers)
                if (!DeploymentManagers.Exists(i => i == connection))
                {
                    DeploymentManagers.Add(connection);
                }
        }

        public void RemoveDeploymentManager(MessageConnection connection)
        {
            lock (DeploymentManagers)
            {
                MessageConnection e = DeploymentManagers.FirstOrDefault(i => i == connection);

                if (e != null)
                    DeploymentManagers.Remove(e);
            }
        }

        public void Execute(SurgeBranchManager branchManager)
        {
            try
            {
                DateTime start = DateTime.UtcNow;

                List<MessageConnection> managers;
                lock (DeploymentManagers)
                    managers = DeploymentManagers.ToList();

                List<string> results = null;

                foreach (string dir in STEM.Sys.IO.Path.ExpandRangedPath(Directory))
                {
                    if (System.IO.Directory.Exists(dir))
                    {
                        if (TargetType == PollerTarget.Files)
                            results = STEM.Sys.IO.Directory.STEM_GetFiles(dir, FileFilter, DirectoryFilter, Recurse ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly, false);
                        else
                            results = STEM.Sys.IO.Directory.STEM_GetDirectories(dir, DirectoryFilter, Recurse ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly, false);

                        STEM.Sys.Serialization.Dictionary<string, List<string>> ret = new Sys.Serialization.Dictionary<string, List<string>>();
                        foreach (string d in results.Select(i => System.IO.Path.GetDirectoryName(i)).Distinct())
                            ret[d] = results.Where(i => System.IO.Path.GetDirectoryName(i) == d).Select(i => System.IO.Path.GetFileName(i)).ToList();

                        PollResult p = new PollResult() { PollTimeMilliseconds = (DateTime.UtcNow - start).TotalMilliseconds, Listing = ret, DeploymentControllerID = DeploymentControllerID, PollError = "" };

                        foreach (MessageConnection d in managers)
                            d.Send(p);
                    }
                    else
                    {
                        PollResult p = new PollResult() { PollTimeMilliseconds = (DateTime.UtcNow - start).TotalMilliseconds, Listing = new Sys.Serialization.Dictionary<string, List<string>>(), DeploymentControllerID = DeploymentControllerID, PollError = dir + " does not exist." };

                        foreach (MessageConnection d in managers)
                            d.Send(p);
                    }
                }
            }
            catch { }
        }
    }
}
