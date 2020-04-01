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


/*
  
-- Sample Database Schema

CREATE TABLE `objects` (
  `id` varchar(36) NOT NULL,
  `name` varchar(200) NOT NULL DEFAULT '',
  `creationtime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `namehash` bigint NOT NULL,
  UNIQUE KEY `idx_objects_id` (`id`),
  KEY `idx_objects_namehash` (`namehash`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


CREATE TABLE `events` (
  `id` varchar(36) NOT NULL,
  `objectid` varchar(36) NOT NULL,
  `eventname` varchar(200) NOT NULL DEFAULT '',
  `machinename` varchar(200) NOT NULL DEFAULT '',
  `processname` varchar(200) NOT NULL DEFAULT '',
  `eventtime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  UNIQUE KEY `idx_events_id` (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


CREATE TABLE `eventmetadata` (
  `eventid` varchar(36) NOT NULL,
  `metadata` varchar(16000) NOT NULL DEFAULT '',
  `metadatahash` bigint NOT NULL,
  UNIQUE KEY `idx_eventmetadata_metadatahash` (`eventid`, `metadatahash`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Sample LogObjectSql

UPDATE objects SET creationtime = '[ObjectCreationTime]' 
WHERE id = '[ObjectID]' OR namehash = [ObjectNameHash] AND '[ObjectCreationTime]' <> '1970-01-01 00:00';
INSERT INTO objects
SELECT '[ObjectID]', '[ObjectName]', '[ObjectCreationTime]', [ObjectNameHash] 
WHERE NOT EXISTS (SELECT 0 FROM objects WHERE id = '[ObjectID]' OR namehash = [ObjectNameHash])

-- Sample LogEventSql

INSERT events 
SELECT '[EventID]', objects.id, '[EventName]', '[MachineName]', '[ProcessName]', '[EventTime]'
FROM objects
WHERE (objects.id = '[ObjectID]') OR ('[ObjectID]' = '' AND objects.name = '[ObjectName]')

-- Sample LogMetaSql

INSERT INTO eventmetadata
SELECT '[EventID]', '[EventMetadata]', [EventMetadataHash]
WHERE NOT EXISTS (SELECT 0 FROM eventmetadata WHERE eventid = '[EventID]' AND metadatahash = [EventMetadataHash])

*/


using System;
using System.Collections.Generic;
using System.ComponentModel;
using STEM.Surge.Logging;

namespace STEM.Surge.MySQL
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("MySql Logger")]
    [Description("Plants a named ILogger in session that is backed by a MySql Database. " +
        "This is used by the STEM.Surge.ObjectTracking extension or directly through ILogger.GetLogger(loggerName).")]
    public class MySqlLogger : ILogger
    {
        [Category("Logger")]
        [DisplayName("Authentication"), DescriptionAttribute("The authentication configuration to be used.")]
        public Authentication Authentication { get; set; }

        [Category("Logger")]
        [DisplayName("Timeout Retry Attempts"), DescriptionAttribute("How many times should an operation retry on database timeouts or deadlocks?")]
        public int TimeoutRetryAttempts { get; set; }

        [Category("Logger")]
        [DisplayName("Log Event Sql"), DescriptionAttribute("This is the Sql that will be executed for each LogEvent call.")]
        public virtual List<string> LogEventSql { get; set; }

        [Category("Logger")]
        [DisplayName("Log Event Metadata"), DescriptionAttribute("This is the Sql that will be executed for each LogEventMetadata call.")]
        public virtual List<string> LogMetaSql { get; set; }

        [Category("Logger")]
        [DisplayName("Log Object Sql"), DescriptionAttribute("This is the Sql that will be executed for each SetObjectInfo call.")]
        public virtual List<string> LogObjectSql { get; set; }

        [Category("Logger")]
        [DisplayName("Available Placeholders"), DescriptionAttribute("The placeholders available for use in your Sql.")]
        [ReadOnly(true)]
        public virtual List<string> AvailablePlaceholders 
        {
            get
            {
                List<string> ret = new List<string>();

                ret.Add("[EventID]");
                ret.Add("[EventMetadata]");
                ret.Add("[EventMetadataHash]");
                ret.Add("[ObjectID]");
                ret.Add("[ObjectName]");
                ret.Add("[ObjectNameHash]");
                ret.Add("[ObjectCreationTime]");
                ret.Add("[MachineName]");
                ret.Add("[ProcessName]");
                ret.Add("[EventName]");
                ret.Add("[EventTime]");

                return ret;
            }
        }

        public MySqlLogger()
        {
            Authentication = new Authentication();
            TimeoutRetryAttempts = 3;
            LogEventSql = new List<string>();
            LogObjectSql = new List<string>();
            LogMetaSql = new List<string>();
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

        public override bool SetObjectInfo(Guid objectID, string objectName, DateTime creationTime, out List<Exception> exceptions)
        {
            exceptions = new List<Exception>();

            return BulkLoad(new List<ObjectData>(new ObjectData[] { new ObjectData {
                ID = objectID,
                Name = objectName,
                CreationTime = creationTime } }), out exceptions);
        }

        public override bool LogEventMetadata(Guid eventID, string metadata, out List<Exception> exceptions)
        {
            exceptions = new List<Exception>();

            return BulkLoad(new List<EventMetadata>(new EventMetadata[] { new EventMetadata {
                EventID = eventID,
                Metadata = metadata } }), out exceptions);
        }

        public override bool BulkLoad(List<EventData> events, out List<Exception> exceptions)
        {
            exceptions = new List<Exception>();

            try
            {
                Dictionary<string, string> map = new Dictionary<string, string>();

                string sql = String.Join("\r\n", LogEventSql);

                if (sql.Trim() == "")
                    throw new EmptyLoggerReference("SQL string was empty in " + LoggerName + ".");

                string cat = "";

                foreach (EventData e in events)
                {
                    map["[EventID]"] = e.EventID.ToString();
                    map["[ObjectID]"] = e.ObjectID.ToString();
                    map["[ObjectName]"] = e.ObjectName;
                    map["[ObjectNameHash]"] = STEM.Sys.State.KeyManager.GetHash(e.ObjectName).ToString();
                    map["[MachineName]"] = e.MachineName;
                    map["[ProcessName]"] = e.ProcessName;
                    map["[EventName]"] = e.EventName;
                    map["[EventTime]"] = e.EventTime.ToString("yyyy-MM-dd HH:mm:ss");

                    cat += STEM.Surge.KVPMapUtils.ApplyKVP(sql, map, false) + ";\r\n";
                }

                ExecuteNonQuery enq = new ExecuteNonQuery();

                cat = STEM.Surge.KVPMapUtils.ApplyKVP(cat, map, false);

                enq.Execute(Authentication, cat, TimeoutRetryAttempts);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

            return exceptions.Count == 0;
        }

        public override bool BulkLoad(List<ObjectData> objects, out List<Exception> exceptions)
        {
            exceptions = new List<Exception>();

            try
            {
                Dictionary<string, string> map = new Dictionary<string, string>();

                string sql = String.Join("\r\n", LogObjectSql);

                if (sql.Trim() == "")
                    throw new EmptyLoggerReference("SQL string was empty in " + LoggerName + ".");

                string cat = "";

                foreach (ObjectData o in objects)
                {
                    map["[ObjectID]"] = o.ID.ToString();
                    map["[ObjectName]"] = o.Name;
                    map["[ObjectNameHash]"] = STEM.Sys.State.KeyManager.GetHash(o.Name).ToString();
                    map["[ObjectCreationTime]"] = o.CreationTime.ToString("yyyy-MM-dd HH:mm:ss");

                    cat += STEM.Surge.KVPMapUtils.ApplyKVP(sql, map, false) + ";\r\n";
                }

                ExecuteNonQuery enq = new ExecuteNonQuery();

                cat = STEM.Surge.KVPMapUtils.ApplyKVP(cat, map, false);

                enq.Execute(Authentication, cat, TimeoutRetryAttempts);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

            return exceptions.Count == 0;
        }

        public override bool BulkLoad(List<EventMetadata> meta, out List<Exception> exceptions)
        {
            exceptions = new List<Exception>();

            try
            {
                Dictionary<string, string> map = new Dictionary<string, string>();

                string sql = String.Join("\r\n", LogMetaSql);

                if (sql.Trim() == "")
                    throw new EmptyLoggerReference("SQL string was empty in " + LoggerName + ".");

                string cat = "";

                foreach (EventMetadata e in meta)
                {
                    map["[EventID]"] = e.EventID.ToString();
                    map["[EventMetadata]"] = e.Metadata;
                    map["[EventMetadataHash]"] = STEM.Sys.State.KeyManager.GetHash(e.Metadata).ToString();

                    cat += STEM.Surge.KVPMapUtils.ApplyKVP(sql, map, false) + ";\r\n";
                }

                ExecuteNonQuery enq = new ExecuteNonQuery();

                cat = STEM.Surge.KVPMapUtils.ApplyKVP(cat, map, false);

                enq.Execute(Authentication, cat, TimeoutRetryAttempts);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

            return exceptions.Count == 0;
        }
    }
}
