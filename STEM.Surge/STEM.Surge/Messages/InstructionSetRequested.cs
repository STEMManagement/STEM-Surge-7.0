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
using STEM.Sys;
using STEM.Sys.Messaging;

namespace STEM.Surge.Messages
{
    
    public class InstructionSetRequested : STEM.Sys.Messaging.Message
    {
        public string BranchIP { get; set; }
        public Guid InstructionSetID { get; set; }
        public string DeploymentControllerID { get; set; }

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


        _InstructionSet _InstructionSet = null;
        /// <summary>
        /// The deserialized InstructionSet
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public _InstructionSet InstructionSet
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
                                _InstructionSet = _InstructionSet.Deserialize(_InstructionSetXml) as _InstructionSet;
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

        public InstructionSetRequested()
        {
        }

        public InstructionSetRequested(_InstructionSet iSet, string branchIP)
        {
            BranchIP = branchIP;
            InstructionSet = iSet;
            InstructionSetID = iSet.ID;
            DeploymentControllerID = iSet.DeploymentControllerID;
        }

        public InstructionSetRequested(string branchIP, Guid instructionSetID, string deploymentControllerID)
        {
            BranchIP = branchIP;
            InstructionSet = null;
            InstructionSetID = instructionSetID;
            DeploymentControllerID = deploymentControllerID;
        }
    }
}
