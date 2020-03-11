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
    [DisplayName("Record Object Info")]
    [Description("Records or updates object metadata through ILogger.GetLogger(LoggerName) and records the Object ID in InstructionSet.InstructionSetContainer[\"ObjectID\"].")]
    public class RecordObjectInfo : Instruction
    {
        [DisplayName("Logger Name"), DescriptionAttribute("What is the LoggerName of the Logger in Session to be used for this operation?")]
        public string LoggerName { get; set; }

        [DisplayName("Object ID"), DescriptionAttribute("What is the Guid of the object?")]
        public string ObjectID { get; set; }

        [DisplayName("Object Name"), DescriptionAttribute("What is the name of the object?")]
        public string ObjectName { get; set; }

        [DisplayName("Object Creation Time"), DescriptionAttribute("What is the creation time of the object?")]
        public string CreationTime { get; set; }

        public RecordObjectInfo()
        {
            ObjectID = "[ISetID]";
            ObjectName = "[TargetName]";
            CreationTime = "[UtcNow]";
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

                if (!ILogger.GetLogger(LoggerName).SetObjectInfo(objectID, ObjectName, DateTime.Parse(CreationTime), out exceptions))
                    throw new Exception("Failed to record object info.");
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