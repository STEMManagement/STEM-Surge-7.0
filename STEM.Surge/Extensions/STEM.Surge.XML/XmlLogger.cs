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
using System.IO;
using STEM.Surge.Logging;

namespace STEM.Surge.XML
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("XmlLogger")]
    [Description("Plants a named ILogger in session that is backed by an Xml Document Store. " +
        "This is used by the STEM.Surge.ObjectTracking extension or directly through ILogger.GetLogger(loggerName).")]
    public class XmlLogger : ILogger
    {
        [Category("Logger")]
        [DisplayName("Xml Document Store"), DescriptionAttribute("The directory where XmlDocuments will be written.")]
        public string XmlDocumentStore { get; set; }

        public XmlLogger()
        {
            XmlDocumentStore = @"D:\STEM.Workforce\XmlLogger";
        }

        bool WriteData(string data, out List<Exception> exceptions)
        {
            exceptions = new List<Exception>();
            
            XmlDocumentStore = STEM.Sys.IO.Path.AdjustPath(XmlDocumentStore);

            try
            {
                if (!Directory.Exists(XmlDocumentStore))
                    Directory.CreateDirectory(XmlDocumentStore);

                File.WriteAllText(Path.Combine(XmlDocumentStore, Guid.NewGuid() + ".xml"), data);

                return true;
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

            return false;
        }

        public override bool BulkLoad(List<EventData> events, out List<Exception> exceptions)
        {
            exceptions = new List<Exception>();
            try
            {
                return WriteData(STEM.Sys.Serializable.Serialize(events), out exceptions);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

            return false;
        }

        public override bool BulkLoad(List<ObjectData> objects, out List<Exception> exceptions)
        {
            exceptions = new List<Exception>();
            try
            {
                return WriteData(STEM.Sys.Serializable.Serialize(objects), out exceptions);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

            return false;
        }

        public override bool BulkLoad(List<EventMetadata> meta, out List<Exception> exceptions)
        {
            exceptions = new List<Exception>();
            try
            {
                return WriteData(STEM.Sys.Serializable.Serialize(meta), out exceptions);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

            return false;
        }

        public override Guid LogEvent(Guid objectID, string eventName, string processName, DateTime eventTime, out List<Exception> exceptions)
        {
            exceptions = new List<Exception>();

            Guid eventID = Guid.NewGuid();
            bool success = BulkLoad(new List<EventData>(new EventData[]{ new EventData {
                EventID = eventID,
                ObjectID = objectID,
                ObjectName = "",
                EventName = eventName,
                ProcessName = processName,
                MachineName = STEM.Sys.IO.Net.MachineName(),
                EventTime = eventTime } }), out exceptions);

            if (success)
                return eventID;

            return Guid.Empty;
        }

        public override Guid LogEvent(string objectName, string eventName, string processName, DateTime eventTime, out List<Exception> exceptions)
        {
            exceptions = new List<Exception>();

            Guid eventID = Guid.NewGuid();
            bool success = BulkLoad(new List<EventData>(new EventData[]{ new EventData {
                EventID = eventID,
                ObjectID = Guid.Empty,
                ObjectName = objectName,
                EventName = eventName,
                ProcessName = processName,
                MachineName = STEM.Sys.IO.Net.MachineName(),
                EventTime = eventTime } }), out exceptions);

            if (success)
                return eventID;

            return Guid.Empty;
        }

        public override bool LogEventMetadata(Guid eventID, string metadata, out List<Exception> exceptions)
        {
            exceptions = new List<Exception>();

            return BulkLoad(new List<EventMetadata>(new EventMetadata[] { new EventMetadata {
                EventID = eventID,
                Metadata = metadata } }), out exceptions);
        }

        public override bool SetObjectInfo(Guid objectID, string objectName, DateTime creationTime, out List<Exception> exceptions)
        {
            exceptions = new List<Exception>();

            return BulkLoad(new List<ObjectData>(new ObjectData[] { new ObjectData {
                ID = objectID,
                Name = objectName,
                CreationTime = creationTime } }), out exceptions);
        }
    }
}
