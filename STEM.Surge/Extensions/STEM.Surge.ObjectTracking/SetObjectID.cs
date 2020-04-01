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
using STEM.Surge.Logging;

namespace STEM.Surge.ObjectTracking
{
    [Serializable()]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("Put the Object ID into the InstructionSetContainer")]
    [Description("Records the Object ID in InstructionSet.InstructionSetContainer[\"ObjectID\"].")]
    public class SetObjectID : Instruction
    {
        [DisplayName("Object ID"), DescriptionAttribute("What is the Guid of the object?")]
        public string ObjectID { get; set; }

        public SetObjectID()
        {
            ObjectID = "[ISetID]";
        }

        protected override void _Rollback()
        {
        }

        protected override bool _Run()
        {
            List<Exception> exceptions = null;

            try
            {
                Guid objectID = Guid.Parse(ObjectID);

                InstructionSet.InstructionSetContainer["ObjectID"] = objectID;
            }
            catch (Exception ex)
            {
                Exceptions.Add(ex);

                if (exceptions != null)
                    Exceptions.AddRange(exceptions);
            }

            return Exceptions.Count == 0;
        }
    }
}