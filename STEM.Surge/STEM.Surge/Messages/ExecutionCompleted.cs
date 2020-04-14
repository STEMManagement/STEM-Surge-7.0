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
using System.ComponentModel;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace STEM.Surge.Messages
{
    /// <summary>
    /// A message sent from BranchManagers to DeploymentManagers informing of InstructionSet execution completion
    /// </summary>
    public class ExecutionCompleted : InstructionMessage
    {
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
                Exceptions.Clear();

                if (value != null)
                    foreach (XElement n in value.Elements())
                        Exceptions.Add(new Exception(n.Value));
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        public List<Exception> Exceptions { get; set; }
        public string InitiationSource { get; set; }
        public string ProcessName { get; set; }
        public DateTime TimeCompleted { get; set; }
        public bool CachePostMortem { get; set; }

        STEM.Surge._InstructionSet _InstructionSet = null;

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

        public ExecutionCompleted()
        {
            Exceptions = new List<Exception>();
            TimeCompleted = DateTime.MinValue;
            _InstructionSet = null;
            CachePostMortem = false;
        }

        public ExecutionCompleted(_InstructionSet iSet)
            : base(iSet)
        {
            if (iSet == null)
                throw new ArgumentNullException(nameof(iSet));
            
            ProcessName = iSet.ProcessName;
            InitiationSource = iSet.InitiationSource;
            TimeCompleted = iSet.Completed;

            Exceptions = new List<Exception>();

            foreach (Instruction i in iSet.Instructions)
                Exceptions.AddRange(i.Exceptions);

            _InstructionSet = iSet;
            CachePostMortem = iSet.CachePostMortem;
        }
    }
}

