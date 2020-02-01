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

namespace STEM.Surge.Messages
{
    /// <summary>
    /// A list of Active Deployments
    /// </summary>
    public class ActiveDeployments : Message
    {
        public class Entry
        {
            public string BranchIP { get; set; }
            public string DeploymentManagerIP { get; set; }
            public string InitiationSource { get; set; }
            public Guid InstructionSetID { get; set; }
            public string DeploymentControllerID { get; set; }
            public string SwitchboardRowID { get; set; }
            public int Exceptions { get; set; }
            public string DeploymentController { get; set; }
            public DateTime Issued { get; set; }
            public DateTime Received { get; set; }
            public DateTime Completed { get; set; }
            public DateTime LastModified { get; set; }
            public double ExecutionTime { get; set; }

            public Entry()
            {
                Exceptions = 0;
            }

            public Entry(DeploymentDetails details)
            {
                Exceptions = 0;
                CopyFrom(details);
            }

            public void CopyFrom(DeploymentDetails source)
            {
                if (source != null)
                {
                    BranchIP = source.BranchIP; 
                    DeploymentManagerIP = source.DeploymentManagerIP;
                    InitiationSource = source.InitiationSource;
                    InstructionSetID = source.InstructionSetID;
                    DeploymentControllerID = source.DeploymentControllerID;
                    SwitchboardRowID = source.SwitchboardRowID;
                    DeploymentController = source.DeploymentController;
                    Issued = source.Issued;
                    Received = source.Received;
                    Completed = source.Completed;
                    LastModified = source.LastModified;

                    if (Completed > DateTime.MinValue)
                        ExecutionTime = (Completed - Received).TotalSeconds;
                    else if (Received > DateTime.MinValue)
                        ExecutionTime = (DateTime.UtcNow - Received).TotalSeconds;
                    else
                        ExecutionTime = (DateTime.UtcNow - Issued).TotalSeconds;

                    Exceptions = source.Exceptions.Count;
                }
            }

            public void CopyFrom(Entry source)
            {
                if (source != null)
                {
                    BranchIP = source.BranchIP; 
                    DeploymentManagerIP = source.DeploymentManagerIP;
                    InitiationSource = source.InitiationSource;
                    InstructionSetID = source.InstructionSetID;
                    DeploymentControllerID = source.DeploymentControllerID;
                    SwitchboardRowID = source.SwitchboardRowID;
                    Exceptions = source.Exceptions;
                    DeploymentController = source.DeploymentController;
                    Issued = source.Issued;
                    Received = source.Received;
                    Completed = source.Completed;
                    LastModified = source.LastModified;
                    ExecutionTime = source.ExecutionTime;
                }
            }
        }

        public List<Entry> Entries { get; set; }
        public List<Guid> Active { get; set; }

        public ActiveDeployments()
        {
            Entries = new List<Entry>();
            Active = new List<Guid>();
        }

        public ActiveDeployments(List<DeploymentDetails> details, List<Guid> active)
        {
            Entries = details.Select(i => new Entry(i)).ToList();
            Active = active.ToList();
        }

        public override void CopyFrom(Message source)
        {
            base.CopyFrom(source);

            ActiveDeployments s = source as ActiveDeployments;

            if (s != null)
            {
                Entries = s.Entries.ToList();
                Active = s.Active.ToList();
            }
        }
    }
}
