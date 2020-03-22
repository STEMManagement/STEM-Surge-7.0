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
using System.Data;
using System.Collections.Generic;
using System.ComponentModel;

namespace STEM.Surge.SQLServer
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("Sql Server Logger Default Schema")]
    [Description("Plants a named ILogger in session that is backed by a SQL Server Database with the STEM Logging Database Schema. " +
        "This is used by the STEM.Surge.ObjectTracking extension or directly through ILogger.GetLogger(loggerName).")]
    public class SQLLoggerDefaultSchema : SQLLogger
    {
        public SQLLoggerDefaultSchema()
        {
        }

        public override bool BulkLoad(List<ObjectData> objects, out List<Exception> exceptions)
        {
            exceptions = new List<Exception>();

            if (objects.Count == 1)
                return base.BulkLoad(objects, out exceptions);

            try
            {
                DataTable ot = Build_ObjectsTable();

                foreach (ObjectData o in objects)
                    ot.Rows.Add(new object[] { o.ID.ToString(), o.Name, o.CreationTime.ToString("G"), STEM.Sys.State.KeyManager.GetHash(o.Name) });

                ExecuteNonQuery enq = new ExecuteNonQuery();

                enq.ImportDataTable(Authentication, ot, ot.TableName, TimeoutRetryAttempts);

                return exceptions.Count == 0;
            }
            catch
            {
                return base.BulkLoad(objects, out exceptions);
            }
        }

        public override bool BulkLoad(List<EventData> events, out List<Exception> exceptions)
        {
            exceptions = new List<Exception>();

            if (events.Count == 1)
                return base.BulkLoad(events, out exceptions);

            try
            {
                DataTable et = Build_EventTable();

                foreach (EventData e in events)
                    et.Rows.Add(new object[] { e.EventID.ToString(), e.ObjectID.ToString(), e.EventName, e.MachineName, e.ProcessName, e.EventTime.ToString("G") });

                ExecuteNonQuery enq = new ExecuteNonQuery();

                enq.ImportDataTable(Authentication, et, et.TableName, TimeoutRetryAttempts);

                return exceptions.Count == 0;
            }
            catch
            {
                return base.BulkLoad(events, out exceptions);
            }
        }

        public override bool BulkLoad(List<EventMetadata> meta, out List<Exception> exceptions)
        {
            exceptions = new List<Exception>();

            if (meta.Count == 1)
                return base.BulkLoad(meta, out exceptions);

            try
            {
                DataTable mt = Build_MetadataTable();

                foreach (EventMetadata m in meta)
                    mt.Rows.Add(new object[] { m.EventID.ToString(), m.Metadata, STEM.Sys.State.KeyManager.GetHash(m.Metadata) });

                ExecuteNonQuery enq = new ExecuteNonQuery();

                enq.ImportDataTable(Authentication, mt, mt.TableName, TimeoutRetryAttempts);

                return exceptions.Count == 0;
            }
            catch
            {
                return base.BulkLoad(meta, out exceptions);
            }
        }
                     
        protected DataTable Build_ObjectsTable()
        {
            DataTable t = new DataTable("Objects");
            t.Columns.Add(new DataColumn { ColumnName = "ID", DataType = Type.GetType("System.Guid") });
            t.Columns.Add(new DataColumn { ColumnName = "Name", DataType = Type.GetType("System.String") });
            t.Columns.Add(new DataColumn { ColumnName = "CreationTime", DataType = Type.GetType("System.DateTime") });
            t.Columns.Add(new DataColumn { ColumnName = "NameHash", DataType = Type.GetType("System.Int64") });

            return t;
        }
        protected DataTable Build_EventTable()
        {
            DataTable t = new DataTable("Events");
            t.Columns.Add(new DataColumn { ColumnName = "ID", DataType = Type.GetType("System.Guid") });
            t.Columns.Add(new DataColumn { ColumnName = "ObjectID", DataType = Type.GetType("System.Guid") });
            t.Columns.Add(new DataColumn { ColumnName = "EventName", DataType = Type.GetType("System.String") });
            t.Columns.Add(new DataColumn { ColumnName = "MachineName", DataType = Type.GetType("System.String") });
            t.Columns.Add(new DataColumn { ColumnName = "ProcessName", DataType = Type.GetType("System.String") });
            t.Columns.Add(new DataColumn { ColumnName = "EventTime", DataType = Type.GetType("System.DateTime") });

            return t;
        }
        protected DataTable Build_MetadataTable()
        {
            DataTable t = new DataTable("EventMetadata");
            t.Columns.Add(new DataColumn { ColumnName = "EventID", DataType = Type.GetType("System.Guid") });
            t.Columns.Add(new DataColumn { ColumnName = "Metadata", DataType = Type.GetType("System.String") });
            t.Columns.Add(new DataColumn { ColumnName = "MetadataHash", DataType = Type.GetType("System.Int64") });

            return t;
        }
    }
}
