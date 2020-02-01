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
using System.Xml.Linq;
using System.Xml.Serialization;
using System.ComponentModel;

namespace STEM.Surge
{
    /// <summary>
    /// The object returned from GenerateDeploymentDetails() in every DeploymentController
    /// Used in bookkeeping of outstanding assignments from a DeploymentManager
    /// </summary>
    public class DeploymentDetails
    {
        [XmlIgnore]
        public _InstructionSet ISet { get; set; }

        public string BranchIP { get; set; }
        public string DeploymentManagerIP { get; set; }
        public string InitiationSource { get; set; }
        public Guid InstructionSetID { get; set; }
        public string DeploymentControllerID { get; set; }
        public string SwitchboardRowID { get; set; }
        public string DeploymentController { get; set; }
        public DateTime Issued { get; set; }
        public DateTime Received { get; set; }
        public DateTime Completed { get; set; }
        public DateTime LastModified { get; set; }

        /// <summary>
        /// For serialization use
        /// </summary>
        [Browsable(false)]
        public XElement ExceptionsXml
        {
            get
            {
                string xml = "<Exceptions>";
                foreach (Exception e in Exceptions)
                {
                    string m = e.ToString();
                    xml += "<Exception>" + System.Security.SecurityElement.Escape(m) + "</Exception>";
                }

                xml += "</Exceptions>";

                XDocument doc = XDocument.Parse(xml);

                return doc.Root;
            }

            set
            {
                if (value != null)
                    foreach (XElement n in value.Elements())
                        Exceptions.Add(new Exception(n.Value));
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        public List<Exception> Exceptions { get; set; }

        public DeploymentDetails()
        {
            ISet = null;
            BranchIP = "0.0.0.0";
            DeploymentManagerIP = "0.0.0.0";
            InitiationSource = "";
            InstructionSetID = Guid.Empty;
            DeploymentControllerID = "";
            SwitchboardRowID = "";
            Issued = DateTime.MinValue;
            Received = DateTime.MinValue;
            Completed = DateTime.MinValue;
            LastModified = DateTime.MinValue;
            DeploymentController = "";
            Exceptions = new List<Exception>();
        }

        /// <summary>
        /// Typically used from GenerateDeploymentDetails
        /// </summary>
        /// <param name="iSet">The InstructionSet being deployed</param>
        /// <param name="branchIP">The Branch to which it should be delivered</param>
        public DeploymentDetails(_InstructionSet iSet, string branchIP)
        {
            if (iSet == null)
                throw new System.ArgumentNullException(nameof(iSet));

            if (System.String.IsNullOrEmpty(branchIP))
                throw new System.ArgumentNullException(nameof(branchIP));

            ISet = iSet;
            BranchIP = branchIP;
            DeploymentManagerIP = "0.0.0.0";
            InitiationSource = ISet.InitiationSource;
            InstructionSetID = ISet.ID;
            DeploymentControllerID = ISet.DeploymentControllerID;
            SwitchboardRowID = "";
            Issued = DateTime.MinValue;
            Received = DateTime.MinValue;
            Completed = DateTime.MinValue;
            LastModified = DateTime.MinValue;
            DeploymentController = "";

            Exceptions = new List<Exception>();
        }

        public void CopyFrom(object source)
        {
            if (source is DeploymentDetails)
            {
                DeploymentDetails s = source as DeploymentDetails;

                if (s != null)
                {
                    ISet = s.ISet;
                    BranchIP = s.BranchIP;
                    DeploymentManagerIP = s.DeploymentManagerIP;
                    InitiationSource = s.InitiationSource;
                    InstructionSetID = s.InstructionSetID;
                    DeploymentControllerID = s.DeploymentControllerID;
                    SwitchboardRowID = s.SwitchboardRowID;
                    DeploymentController = s.DeploymentController;

                    Exceptions = s.Exceptions.ToList();

                    if (Issued > s.Issued || Received > s.Received || Completed > s.Completed)
                        return;

                    Issued = s.Issued;
                    Received = s.Received;
                    Completed = s.Completed;
                    LastModified = s.LastModified;
                }
            }
        }
    }
}
