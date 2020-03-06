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
    [DisplayName("Log Object Event")]
    [Description("Logs an event against an object through ILogger.GetLogger(LoggerName) and records the resulting eventID in InstructionSet.InstructionSetContainer[EventName].")]
    public class LogObjectEvent : Instruction
    {
        [DisplayName("Logger Name"), DescriptionAttribute("What is the LoggerName of the Logger in Session to be used for this operation?")]
        public string LoggerName { get; set; }

        [DisplayName("Event Name"), DescriptionAttribute("What is the event being recorded?")]
        public string EventName { get; set; }

        [DisplayName("Object Name"), DescriptionAttribute("What is the name of the object against which this event is being recorded?")]
        public string ObjectName { get; set; }

        [DisplayName("Generate Object ID"), DescriptionAttribute("Should a new Object tracking Guid be created if one isn't found in the InstructionSetContainer?")]
        public bool GenerateObjectID { get; set; }

        [DisplayName("Update Object Name"), DescriptionAttribute("If the Object tracking Guid is present in the InstructionSetContainer, should the Object Name be updated at this time?")]
        public bool UpdateObjectName { get; set; }

        public LogObjectEvent()
        {
            LoggerName = "EventLog";
            EventName = "NewEvent";
            ObjectName = "[TargetName]";
            GenerateObjectID = false;
            UpdateObjectName = false;
        }

        protected override void _Rollback()
        {
        }

        protected override bool _Run()
        {
            try
            {
                Guid objectID = Guid.Empty;

                if (InstructionSet.InstructionSetContainer.ContainsKey("ObjectID"))
                    objectID = (Guid)InstructionSet.InstructionSetContainer["ObjectID"];

                if (objectID == Guid.Empty && GenerateObjectID)
                {
                    objectID = Guid.NewGuid();
                    if (!ILogger.GetLogger(LoggerName).SetObjectInfo(objectID, ObjectName))
                        throw new Exception("Failed to record object info.");

                    InstructionSet.InstructionSetContainer["ObjectID"] = objectID;
                }
                else if (UpdateObjectName)
                {
                    if (!ILogger.GetLogger(LoggerName).SetObjectInfo(objectID, ObjectName))
                        throw new Exception("Failed to record object info.");
                }

                Guid eventID = Guid.Empty;

                if (objectID == Guid.Empty)
                    eventID = ILogger.GetLogger(LoggerName).LogEvent(ObjectName, EventName, InstructionSet.ProcessName);
                else
                    eventID = ILogger.GetLogger(LoggerName).LogEvent(objectID, EventName, InstructionSet.ProcessName);

                if (eventID == Guid.Empty)
                {
                    if (objectID == Guid.Empty)
                        throw new Exception("Failed to record event against ObjectName.");
                    else
                        throw new Exception("Failed to record event against ObjectID.");
                }

                InstructionSet.InstructionSetContainer[EventName] = eventID;
            }
            catch (Exception ex)
            {
                Exceptions.Add(ex);
            }

            return Exceptions.Count == 0;
        }
    }
}
