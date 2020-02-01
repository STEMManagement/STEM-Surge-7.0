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
using System.Xml.Linq;
using System.Linq;
using System.Data;

namespace STEM.Surge.PostGreSQL
{
    public abstract class ImportPostMortem : PostGresBaseInstruction
    {
        protected DataTable Build_ISetTable()
        {
            DataTable iSet = new DataTable("postmortem.instruction_set");
            iSet.Columns.Add(new DataColumn { ColumnName = "isid", DataType = Type.GetType("System.Guid") });
            iSet.Columns.Add(new DataColumn { ColumnName = "branch_ip", DataType = Type.GetType("System.String") });
            iSet.Columns.Add(new DataColumn { ColumnName = "deployment_controller_id", DataType = Type.GetType("System.String") });
            iSet.Columns.Add(new DataColumn { ColumnName = "deployment_manager_ip", DataType = Type.GetType("System.String") });
            iSet.Columns.Add(new DataColumn { ColumnName = "deployment_controller", DataType = Type.GetType("System.String") });
            iSet.Columns.Add(new DataColumn { ColumnName = "instruction_set_template", DataType = Type.GetType("System.String") });
            iSet.Columns.Add(new DataColumn { ColumnName = "initiation_source", DataType = Type.GetType("System.String") });
            iSet.Columns.Add(new DataColumn { ColumnName = "process_name", DataType = Type.GetType("System.String") });
            iSet.Columns.Add(new DataColumn { ColumnName = "assigned", DataType = Type.GetType("System.DateTime") });
            iSet.Columns.Add(new DataColumn { ColumnName = "received", DataType = Type.GetType("System.DateTime") });
            iSet.Columns.Add(new DataColumn { ColumnName = "started", DataType = Type.GetType("System.DateTime") });
            iSet.Columns.Add(new DataColumn { ColumnName = "completed", DataType = Type.GetType("System.DateTime") });

            return iSet;
        }
        protected DataTable Build_InstructionTable()
        {
            DataTable instructions = new DataTable("postmortem.instruction");
            instructions.Columns.Add(new DataColumn { ColumnName = "isid", DataType = Type.GetType("System.Guid") });
            instructions.Columns.Add(new DataColumn { ColumnName = "is_assigned", DataType = Type.GetType("System.DateTime") });
            instructions.Columns.Add(new DataColumn { ColumnName = "iid", DataType = Type.GetType("System.Guid") });
            instructions.Columns.Add(new DataColumn { ColumnName = "type", DataType = Type.GetType("System.String") });
            instructions.Columns.Add(new DataColumn { ColumnName = "start", DataType = Type.GetType("System.DateTime") });
            instructions.Columns.Add(new DataColumn { ColumnName = "finish", DataType = Type.GetType("System.DateTime") });
            instructions.Columns.Add(new DataColumn { ColumnName = "stage", DataType = Type.GetType("System.String") });
            instructions.Columns.Add(new DataColumn { ColumnName = "exceptions", DataType = Type.GetType("System.Boolean") });

            return instructions;
        }

        protected void IngestInstructionSet(string iSetXml, DataTable iSetTable, DataTable instructionTable)
        {
            XDocument doc = XDocument.Parse(iSetXml);

            Guid isid = Guid.Parse(doc.Root.Elements().FirstOrDefault(i => i.Name.LocalName == "ID").Value);

            lock (iSetTable)
                iSetTable.Rows.Add(
                    isid,
                    doc.Root.Elements().FirstOrDefault(i => i.Name.LocalName == "BranchIP").Value,
                    doc.Root.Elements().FirstOrDefault(i => i.Name.LocalName == "DeploymentControllerID").Value,
                    doc.Root.Elements().FirstOrDefault(i => i.Name.LocalName == "DeploymentManagerIP").Value,
                    doc.Root.Elements().FirstOrDefault(i => i.Name.LocalName == "DeploymentController").Value.ToLower(),
                    STEM.Sys.IO.Path.GetFileNameWithoutExtension(doc.Root.Elements().FirstOrDefault(i => i.Name.LocalName == "InstructionSetTemplate").Value.ToLower()),
                    doc.Root.Elements().FirstOrDefault(i => i.Name.LocalName == "InitiationSource").Value.ToLower(),
                    doc.Root.Elements().FirstOrDefault(i => i.Name.LocalName == "ProcessName").Value.ToLower(),
                    DateTime.Parse(doc.Root.Elements().FirstOrDefault(i => i.Name.LocalName == "Assigned").Value),
                    DateTime.Parse(doc.Root.Elements().FirstOrDefault(i => i.Name.LocalName == "Received").Value),
                    DateTime.Parse(doc.Root.Elements().FirstOrDefault(i => i.Name.LocalName == "Started").Value),
                    DateTime.Parse(doc.Root.Elements().FirstOrDefault(i => i.Name.LocalName == "Completed").Value)
                    );

            DateTime is_assigned = DateTime.Parse(doc.Root.Elements().FirstOrDefault(i => i.Name.LocalName == "Assigned").Value);

            XElement root = doc.Root.Descendants().FirstOrDefault(i => i.Name.LocalName == "Instructions");

            foreach (XElement n in root.Elements())
            {
                string aName = n.Descendants().FirstOrDefault(i => i.Name.LocalName == "AssemblyName" && i.Parent.Name.LocalName == "VersionDescriptor").Value;
                string tName = n.Descendants().FirstOrDefault(i => i.Name.LocalName == "TypeName" && i.Parent.Name.LocalName == "VersionDescriptor").Value;

                lock (iSetTable)
                    instructionTable.Rows.Add(
                            isid,
                            is_assigned,
                            Guid.Parse(n.Elements().FirstOrDefault(i => i.Name.LocalName == "ID").Value),
                            tName.ToLower(),
                        DateTime.Parse(n.Elements().FirstOrDefault(i => i.Name.LocalName == "Start").Value),
                        DateTime.Parse(n.Elements().FirstOrDefault(i => i.Name.LocalName == "Finish").Value),
                        n.Elements().FirstOrDefault(i => i.Name.LocalName == "Stage").Value.ToLower(),
                        (n.Descendants().FirstOrDefault(i => i.Name.LocalName == "Exceptions").Elements().Count() > 0)
                        );
            }
        }
    }
}
