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
using System.ComponentModel;
using System.Xml;

namespace STEM.Surge.Messages
{
    /// <summary>
    /// This is a message sent from DeploymentManagers to Branch Managers to assign work
    /// </summary>
    public class AssignInstructionSet : STEM.Sys.Messaging.Message
    {
        [Browsable(false)]
        [XmlIgnore]
        public DateTime ExecutionCompleted { get; set; }

        public string DeploymentControllerID { get; set; }
        public string DeploymentManagerIP { get; set; }
        public string BranchIP { get; set; }
        public Guid InstructionSetID { get; set; }
        public bool IsStatic { get; set; }
        public bool ContinuousExecution { get; set; }
        public bool ExecuteStaticInSandboxes { get; set; }        
        public DateTime LastWriteTime { get; set; }
        public string InitiationSource { get; set; }

        [Browsable(false)]
        public string SandboxID { get; set; }

        [Browsable(false)]
        public string SandboxAppConfigXml { get; set; }

        [Browsable(false)]
        public string AlternateAssemblyStore { get; set; }

        [Browsable(false)]
        public int MinutesOfInactivity { get; set; }

        string _InstructionSetXml = null;
        [Browsable(false)]
        public string InstructionSetXml
        {
            get
            {
                if (_InstructionSetXml == null && _InstructionSet != null)
                {
                    _InstructionSetXml = _InstructionSet.Serialize();
                }

                return _InstructionSetXml;
            }

            set
            {
                _InstructionSetXml = value;
            }
        }


        STEM.Surge._InstructionSet _InstructionSet = null;
        /// <summary>
        /// The deserialized InstructionSet
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public STEM.Surge._InstructionSet InstructionSet
        {
            set
            {
                lock (this)
                {
                    _InstructionSet = value;
                    _InstructionSetXml = null;
                }
            }

            get
            {
                lock (this)
                    if (_InstructionSet == null)
                    {
                        try
                        {
                            if (_InstructionSetXml != null)
                                _InstructionSet = STEM.Surge._InstructionSet.Deserialize(_InstructionSetXml) as STEM.Surge._InstructionSet;
                        }
                        catch (Exception ex)
                        {
                            STEM.Sys.EventLog.WriteEntry("InstructionSet:get", ex.ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
                            _InstructionSet = null;
                        }
                    }

                return _InstructionSet;
            }
        }
        
        public AssignInstructionSet()
        {
            ExecutionCompleted = DateTime.MinValue;
        }

        public AssignInstructionSet(STEM.Surge._InstructionSet iSet, string deploymentManagerIP, string deploymentControllerID, string branchIP, string sandboxID, string sandboxAppConfigXml, string alternateAssemblyStore, int minutesOfInactivity)
        {
            ExecutionCompleted = DateTime.MinValue;
            InitiationSource = iSet.InitiationSource;
            DeploymentManagerIP = deploymentManagerIP;
            DeploymentControllerID = deploymentControllerID;
            BranchIP = branchIP;
            IsStatic = false;
            LastWriteTime = DateTime.UtcNow;            
            InstructionSet = iSet;
            InstructionSetID = iSet.ID;
            SandboxID = sandboxID;
            SandboxAppConfigXml = sandboxAppConfigXml;
            AlternateAssemblyStore = alternateAssemblyStore;
            MinutesOfInactivity = minutesOfInactivity;
            ContinuousExecution = false;
        }

        public AssignInstructionSet(STEM.Surge._InstructionSet iSet, string deploymentManagerIP, string deploymentManagerID, string branchIP, string sandboxID, string sandboxAppConfigXml, string alternateAssemblyStore, int minutesOfInactivity, bool isStatic, DateTime lastWriteTime)
        {
            ExecutionCompleted = DateTime.MinValue;
            InitiationSource = iSet.InitiationSource;
            DeploymentManagerIP = deploymentManagerIP;
            DeploymentControllerID = deploymentManagerID;
            BranchIP = branchIP;
            InstructionSet = iSet;
            InstructionSetID = iSet.ID;
            SandboxID = sandboxID;
            SandboxAppConfigXml = sandboxAppConfigXml;
            AlternateAssemblyStore = alternateAssemblyStore;
            MinutesOfInactivity = minutesOfInactivity;

            IsStatic = isStatic;
            LastWriteTime = lastWriteTime;
            ContinuousExecution = iSet.ContinuousExecution;
            ExecuteStaticInSandboxes = iSet.ExecuteStaticInSandboxes;
        }
    }
}
