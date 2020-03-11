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
    [DisplayName("Log Process Event")]
    [Description("Logs an event, independent of any object, through ILogger.GetLogger(LoggerName) and records the resulting eventID in InstructionSet.InstructionSetContainer[EventName].")]
    public class LogProcessEvent : Instruction
    {
        [DisplayName("Logger Name"), DescriptionAttribute("What is the LoggerName of the Logger in Session to be used for this operation?")]
        public string LoggerName { get; set; }

        [DisplayName("Event Name"), DescriptionAttribute("What is the event being recorded?")]
        public string EventName { get; set; }

        public LogProcessEvent()
        {
            LoggerName = "EventLog";
            EventName = "NewEvent";
        }

        protected override void _Rollback()
        {
        }

        protected override bool _Run()
        {
            List<Exception> exceptions = null;

            try
            {
                Guid eventID = ILogger.GetLogger(LoggerName).LogEvent(Guid.Empty, EventName, InstructionSet.ProcessName, out exceptions);

                if (eventID == Guid.Empty)
                    throw new Exception("Failed to record process event.");

                InstructionSet.InstructionSetContainer[EventName] = eventID;
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
