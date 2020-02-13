using System;
using System.Collections.Generic;
using System.ComponentModel;
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

using System.Linq;
using System.IO;
using System.Reflection;

namespace STEM.Surge.BasicControllers
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("GroupingController")]
    [Description("Populate List<string> properties in the InstructionSet Template with lists of items up to 'Group Size'. ")]
    public class GroupingController : DeploymentController
    {
        [Category("Grouping")]
        [DisplayName("Group Size"), DescriptionAttribute("How big should initiationSource groups be (max).")]
        public int GroupSize { get; set; }

        [Category("Grouping")]
        [DisplayName("List<string> Property Names"), DescriptionAttribute("These are the lists to be populated with initiationSources in groups.")]
        public List<string> ListPropertyNames { get; set; }

        [Category("Grouping")]
        [DisplayName("Randomize List"), DescriptionAttribute("Should the Controller randomize the listing in ListPreprocess?")]
        public bool RandomizeList { get; set; }

        IReadOnlyList<string> _LastList;
        Dictionary<string, int> _ListMap = new Dictionary<string, int>();

        public GroupingController()
        {
            ListPropertyNames = new List<string>();
            GroupSize = 10;
            RandomizeList = false;
        }

        public override List<string> ListPreprocess(IReadOnlyList<string> list)
        {
            _LastList = list;
            if (RandomizeList)
            {
                Random rnd = new Random();
                _LastList = list.OrderBy(i => rnd.Next()).ToList();
            }

            if (_LastList.Count == 0)
                return new List<string>();

            _ListMap = new Dictionary<string, int>();

            int groups = (_LastList.Count + GroupSize - 1) / GroupSize;

            for (int x = 0; x < groups; x++)
            {
                string mapKey = Path.Combine(STEM.Sys.IO.Path.GetDirectoryName(_LastList[0]), Guid.NewGuid().ToString());
                _ListMap[mapKey] = x;
            }

            return _ListMap.Keys.ToList();
        }

        public override DeploymentDetails GenerateDeploymentDetails(IReadOnlyList<string> listPreprocessResult, string initiationSource, string recommendedBranchIP, IReadOnlyList<string> limitedToBranches)
        {
            DeploymentDetails ret = null;
            List<string> locks = new List<string>();
            Guid iSetID = Guid.Empty;
            
            try
            {
                int iter = _ListMap[initiationSource];

                InstructionSet clone = GetTemplateInstance(true);

                CustomizeInstructionSet(clone, TemplateKVP, recommendedBranchIP, initiationSource, true);

                iSetID = clone.ID;

                List<List<string>> iSetLists = new List<List<string>>();
                foreach (Instruction i in clone.Instructions)
                {
                    foreach (PropertyInfo pi in i.GetType().GetProperties())
                    {
                        if (ListPropertyNames.Contains(pi.Name))
                        {
                            List<string> li = pi.GetValue(i) as List<string>;

                            if (li != null)
                                iSetLists.Add(li);
                        }
                    }
                }

                if (iSetLists.Count == 0)
                    throw new Exception("No fillable properties found in InstructionSet.");
                
                if (CoordinatedKeyManager != null)
                {
                    for (int x = (iter * GroupSize); x < ((iter * GroupSize) + GroupSize); x++)
                    {
                        if (_LastList.Count <= x)
                            break;

                        string s = ApplyKVP(_LastList[x], TemplateKVP, recommendedBranchIP, initiationSource, true);

                        if (CoordinatedKeyManager.Lock(s, CoordinateWith))
                        {
                            locks.Add(s);
                            
                            foreach (List<string> li in iSetLists)
                                li.Add(s);
                        }
                    }
                }

                if (locks.Count == 0)
                    return null;

                ret = new DeploymentDetails(clone, recommendedBranchIP);
            }
            catch (Exception ex)
            {
                STEM.Sys.EventLog.WriteEntry("GroupingController.GenerateDeploymentDetails", new Exception(InstructionSetTemplate + ": " + initiationSource, ex).ToString(), STEM.Sys.EventLog.EventLogEntryType.Error);

                foreach (string s in locks)
                    CoordinatedKeyManager.Unlock(s);

                locks.Clear();
            }
            finally
            {
                if (locks.Count > 0 && ret != null)
                    STEM.Sys.State.Containers.Session[iSetID.ToString()] = locks;
            }

            return ret;
        }

        public override void ExecutionComplete(DeploymentDetails details, List<Exception> exceptions)
        {
            base.ExecutionComplete(details, exceptions);

            List<string> locks = STEM.Sys.State.Containers.Session[details.InstructionSetID.ToString()] as List<string>;

            if (locks != null)
            {
                foreach (string s in locks)
                    CoordinatedKeyManager.Unlock(s);

                STEM.Sys.State.Containers.Session[details.InstructionSetID.ToString()] = null;
            }
        }
    }
}
