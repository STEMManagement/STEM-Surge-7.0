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

USE [SurgeEventLog]

CREATE TABLE [dbo].[Objects](
	[ID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[CreationTime] [datetime] NOT NULL,
	[NameHash] [bigint] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


CREATE UNIQUE NONCLUSTERED INDEX[PK_Objects] ON[dbo].[Objects]
(
  [ID] ASC
) WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = ON, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]

CREATE NONCLUSTERED INDEX[ix_objects_namehash] ON[dbo].[Objects]
(
  [NameHash] ASC
) WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]



CREATE TABLE [dbo].[Events](
	[ID] [uniqueidentifier] NOT NULL,
	[ObjectID] [uniqueidentifier] NOT NULL,
	[EventName] [nvarchar](50) NOT NULL,
	[MachineName] [nvarchar](50) NOT NULL,
	[ProcessName] [nvarchar](50) NOT NULL,
	[EventTime] [datetime] NOT NULL
) ON [PRIMARY]

CREATE UNIQUE NONCLUSTERED INDEX [PK_Events] ON [dbo].[Events]
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = ON, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]



CREATE TABLE [dbo].[EventMetadata](
	[EventID] [uniqueidentifier] NOT NULL,
	[Metadata] [nvarchar](max) NOT NULL,
	[MetadataHash] [bigint] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE UNIQUE NONCLUSTERED INDEX [ix_eventmetadata_metadatahash] ON [dbo].[EventMetadata]
(
	[EventID] ASC,
	[MetadataHash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = ON, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


-- Sample LogObjectSql
IF EXISTS (SELECT 0 FROM Objects WHERE ID = '[ObjectID]')
BEGIN
UPDATE Objects SET CreationTime = '[ObjectCreationTime]', Name = '[ObjectName]', NameHash = [ObjectNameHash] WHERE ID = '[ObjectID]'
END
ELSE IF EXISTS (SELECT 0 FROM Objects WHERE ID = '[ObjectID]' OR NameHash = [ObjectNameHash])
BEGIN
UPDATE Objects SET CreationTime = '[ObjectCreationTime]' WHERE ID = '[ObjectID]' OR NameHash = [ObjectNameHash]
END
ELSE
BEGIN
INSERT Objects
SELECT '[ObjectID]', '[ObjectName]', '[ObjectCreationTime]', [ObjectNameHash]
END


-- Sample LogEventSql
INSERT Events 
SELECT '[EventID]', [Objects].ID, '[EventName]', '[MachineName]', '[ProcessName]', '[EventTime]'
FROM [Objects]
WHERE ([Objects].ID = '[ObjectID]') OR ('[ObjectID]' = '' AND [Objects].NameHash = [ObjectNameHash])

-- Sample LogMetaSql
IF NOT EXISTS (SELECT 0 FROM EventMetadata WHERE EventID = '[EventID]' AND MetadataHash = [EventMetadataHash])
BEGIN
INSERT EventMetadata
SELECT '[EventID]', '[EventMetadata]', [EventMetadataHash]
END

*/


using System;
using System.Collections.Generic;
using System.ComponentModel;
using STEM.Surge.Logging;

namespace STEM.Surge.SQLServer
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("SQL Server Logger")]
    [Description("Plants a named ILogger in session that is backed by a SQL Server Database. " +
        "This is used by the STEM.Surge.ObjectTracking extension or directly through ILogger.GetLogger(loggerName).")]
    public class SQLLogger : ILogger
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

        public SQLLogger()
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
                    map["[EventTime]"] = e.EventTime.ToString("G");

                    cat += "\r\n" + STEM.Surge.KVPMapUtils.ApplyKVP(sql, map, false);
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
                    map["[ObjectCreationTime]"] = o.CreationTime.ToString("G");

                    cat += "\r\n" + STEM.Surge.KVPMapUtils.ApplyKVP(sql, map, false);
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

                    cat += "\r\n" + STEM.Surge.KVPMapUtils.ApplyKVP(sql, map, false);
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
