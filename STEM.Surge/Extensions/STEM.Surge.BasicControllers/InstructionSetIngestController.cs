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
using System.IO;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace STEM.Surge.BasicControllers
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("InstructionSetIngestController")]
    [Description("Ingest an InstructionSet from a file and assign it to a Branch. " +
        "This is a specialized utility controller used to assign manually generated InstructionSets to Branches.")]
    public class InstructionSetIngestController : STEM.Surge.FileDeploymentController
    {
        public InstructionSetIngestController()
        {
        }

        public override DeploymentDetails GenerateDeploymentDetails(IReadOnlyList<string> listPreprocessResult, string initiationSource, string recommendedBranchIP, IReadOnlyList<string> limitedToBranches)
        {
            try
            {
                string xml = File.ReadAllText(initiationSource);

                InstructionSet iSet = (InstructionSet)InstructionSet.Deserialize(xml);
                
                CustomizeInstructionSet(iSet, TemplateKVP, recommendedBranchIP, initiationSource, true);

                foreach (Instruction ins in iSet.Instructions)
                {
                    try
                    {
                        foreach (PropertyInfo prop in ins.GetType().GetProperties().Where(p => p.PropertyType.IsSubclassOf(typeof(STEM.Sys.Security.IAuthentication))))
                        {
                            try
                            {
                                STEM.Sys.Security.IAuthentication a = prop.GetValue(ins) as STEM.Sys.Security.IAuthentication;
                                STEM.Sys.Security.IAuthentication b = GetAuthentication(a.ConfigurationName);

                                if (b != null)
                                    a.PopulateFrom(b);
                            }
                            catch { }
                        }
                    }
                    catch { }
                }

                File.Delete(initiationSource);

                return new DeploymentDetails(iSet, recommendedBranchIP);
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("InstructionSetIngestController.GenerateDeploymentDetails", new Exception(initiationSource, ex).ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);
            }

            return null;
        }
    }
}
