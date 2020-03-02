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
using System.Data;
using System.ComponentModel;

namespace STEM.Surge.XML
{
    [Serializable()]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("Container DataSet To Container Xml")]
    [Description("Replace a DataSet in a Container with an Xml String in the same Container.")]
    public class DataSetToXml : Instruction
    {
        [DisplayName("Container Data Key")]
        [Description("The key in the container where the DataSet is loaded.")]
        public string ContainerDataKey { get; set; }

        [DisplayName("Target Container")]
        [Description("The container with the DataSet to be transformed.")]
        public ContainerType TargetContainer { get; set; }
                
        [DisplayName("DataSet Name")]
        [Description("This will become the Root Node Name in the Xml Document.")]
        public string DataSetName { get; set; }

        [DisplayName("DataSet Table Name")]
        [Description("These will become the Node Names for the child rows from each Table in the DataSet (ordered list).")]
        public List<string> TableNames { get; set; }

        public DataSetToXml()
        {
            ContainerDataKey = "[TargetNameWithoutExt]";
            TargetContainer = ContainerType.InstructionSetContainer;
            TableNames = new List<string>();
        }

        protected override void _Rollback()
        {
            if (_DataSet != null)
                switch (TargetContainer)
                {
                    case ContainerType.InstructionSetContainer:

                        InstructionSet.InstructionSetContainer[ContainerDataKey] = _DataSet;

                        break;

                    case ContainerType.Session:

                        STEM.Sys.State.Containers.Session[ContainerDataKey] = _DataSet;

                        break;

                    case ContainerType.Cache:
                        
                        STEM.Sys.State.Containers.Cache[ContainerDataKey] = _DataSet;

                        break;
                }
        }

        DataSet _DataSet = null;
        protected override bool _Run()
        {
            try
            {
                switch (TargetContainer)
                {
                    case ContainerType.InstructionSetContainer:

                        if (!InstructionSet.InstructionSetContainer.ContainsKey(ContainerDataKey))
                            throw new Exception("ContainerDataKey (" + ContainerDataKey + ") does not exist.");

                        _DataSet = InstructionSet.InstructionSetContainer[ContainerDataKey] as DataSet;

                        break;

                    case ContainerType.Session:

                        if (!STEM.Sys.State.Containers.Session.ContainsKey(ContainerDataKey))
                            throw new Exception("ContainerDataKey (" + ContainerDataKey + ") does not exist.");

                        _DataSet = STEM.Sys.State.Containers.Session[ContainerDataKey] as DataSet;

                        break;

                    case ContainerType.Cache:

                        if (!STEM.Sys.State.Containers.Cache.ContainsKey(ContainerDataKey))
                            throw new Exception("ContainerDataKey (" + ContainerDataKey + ") does not exist.");

                        _DataSet = STEM.Sys.State.Containers.Cache[ContainerDataKey] as DataSet;

                        break;
                }

                string xml = null;
                if (_DataSet != null)
                {
                    _DataSet.DataSetName = DataSetName;

                    int x = 0;
                    foreach (DataTable t in _DataSet.Tables)
                        if (TableNames.Count > x)
                            t.TableName = TableNames[x++];

                    xml = _DataSet.GetXml();
                }

                if (xml != null)
                    switch (TargetContainer)
                    {
                        case ContainerType.InstructionSetContainer:

                            InstructionSet.InstructionSetContainer[ContainerDataKey] = xml;

                            break;

                        case ContainerType.Session:

                            STEM.Sys.State.Containers.Session[ContainerDataKey] = xml;

                            break;

                        case ContainerType.Cache:

                            STEM.Sys.State.Containers.Cache[ContainerDataKey] = xml;

                            break;
                    }
            }
            catch (Exception ex)
            {
                AppendToMessage(ex.Message);
                Exceptions.Add(ex);
            }

            return Exceptions.Count == 0;
        }
    }
}
