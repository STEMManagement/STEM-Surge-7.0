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
using System.Data;
using System.Collections.Generic;
using System.ComponentModel;

namespace STEM.Surge.MySQL
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("MySql Logger Default Schema")]
    [Description("Plants a named ILogger in session that is backed by a MySql Database with the STEM Logging Database Schema. " +
        "This is used by the STEM.Surge.ObjectTracking extension or directly through ILogger.GetLogger(loggerName).")]
    public class MySqlLoggerDefaultSchema : MySqlLogger
    {
        public MySqlLoggerDefaultSchema()
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
                    ot.Rows.Add(new object[] { o.ID.ToString(), o.Name, o.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"), STEM.Sys.State.KeyManager.GetHash(o.Name) });

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
                    et.Rows.Add(new object[] { e.EventID.ToString(), e.ObjectID.ToString(), e.EventName, e.MachineName, e.ProcessName, e.EventTime.ToString("yyyy-MM-dd HH:mm:ss") });

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
            DataTable t = new DataTable("objects");
            t.Columns.Add(new DataColumn { ColumnName = "id", DataType = Type.GetType("System.Guid") });
            t.Columns.Add(new DataColumn { ColumnName = "name", DataType = Type.GetType("System.String") });
            t.Columns.Add(new DataColumn { ColumnName = "creationtime", DataType = Type.GetType("System.DateTime") });
            t.Columns.Add(new DataColumn { ColumnName = "namehash", DataType = Type.GetType("System.Int64") });

            return t;
        }
        protected DataTable Build_EventTable()
        {
            DataTable t = new DataTable("events");
            t.Columns.Add(new DataColumn { ColumnName = "id", DataType = Type.GetType("System.Guid") });
            t.Columns.Add(new DataColumn { ColumnName = "objectid", DataType = Type.GetType("System.Guid") });
            t.Columns.Add(new DataColumn { ColumnName = "eventname", DataType = Type.GetType("System.String") });
            t.Columns.Add(new DataColumn { ColumnName = "machinename", DataType = Type.GetType("System.String") });
            t.Columns.Add(new DataColumn { ColumnName = "processname", DataType = Type.GetType("System.String") });
            t.Columns.Add(new DataColumn { ColumnName = "eventtime", DataType = Type.GetType("System.DateTime") });

            return t;
        }
        protected DataTable Build_MetadataTable()
        {
            DataTable t = new DataTable("eventmetadata");
            t.Columns.Add(new DataColumn { ColumnName = "eventid", DataType = Type.GetType("System.Guid") });
            t.Columns.Add(new DataColumn { ColumnName = "metadata", DataType = Type.GetType("System.String") });
            t.Columns.Add(new DataColumn { ColumnName = "metadatahash", DataType = Type.GetType("System.Int64") });

            return t;
        }
    }
}
