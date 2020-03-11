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
    [DisplayName("Record Event Metadata")]
    [Description("Records eventmeta data through ILogger.GetLogger(LoggerName) against the eventID recorded in InstructionSet.InstructionSetContainer[EventName].")]
    public class RecordEventMetadata : Instruction
    {
        [DisplayName("Logger Name"), DescriptionAttribute("What is the LoggerName of the Logger in Session to be used for this operation?")]
        public string LoggerName { get; set; }

        [DisplayName("Event Name"), DescriptionAttribute("What is the event upon which metadata is being recorded?")]
        public string EventName { get; set; }

        [DisplayName("Metadata"), DescriptionAttribute("What is the name of the object?")]
        public string Metadata { get; set; }

        public RecordEventMetadata()
        {
            LoggerName = "EventLog";
            EventName = "NewEvent";
            Metadata = @"[TargetPath]\[TargetName] - [LastWriteTimeUtc]";
        }

        protected override void _Rollback()
        {
        }

        protected override bool _Run()
        {
            List<Exception> exceptions = null;

            try
            {
                if (InstructionSet.InstructionSetContainer.ContainsKey(EventName))
                {
                    Guid eventID = (Guid)InstructionSet.InstructionSetContainer[EventName];

                    if (!ILogger.GetLogger(LoggerName).LogEventMetadata(eventID, Metadata, out exceptions))
                        throw new Exception("Failed to record event metadata.");
                }
                else
                {
                    throw new Exception("The eventID wasn't present in the InstructionSetContainer.");
                }
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