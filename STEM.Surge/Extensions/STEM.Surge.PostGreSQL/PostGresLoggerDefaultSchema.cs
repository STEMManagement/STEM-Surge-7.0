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

-- Suggested Database Schema

CREATE SCHEMA event_log
    AUTHORIZATION postgres;

CREATE TABLE event_log.objects
(
    id uuid NOT NULL,
    name character varying COLLATE pg_catalog."default" NOT NULL,
    creationtime timestamp without time zone NOT NULL,
    namehash bigint NOT NULL
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE event_log.objects
    OWNER to postgres;

CREATE UNIQUE INDEX ix_objects_id
    ON event_log.objects USING btree
    (id ASC NULLS LAST)
    TABLESPACE pg_default;

CREATE INDEX ix_objects_namehash
    ON event_log.objects USING btree
    (namehash ASC NULLS LAST)
    TABLESPACE pg_default;

CREATE TABLE event_log.events
(
    id uuid NOT NULL,
    objectid uuid NOT NULL,
    eventname character varying COLLATE pg_catalog."default" NOT NULL,
    machinename character varying COLLATE pg_catalog."default" NOT NULL,
    processname character varying COLLATE pg_catalog."default" NOT NULL,
    eventtime timestamp without time zone NOT NULL
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE event_log.events
    OWNER to postgres;
       
CREATE TABLE event_log.event_metadata
(
    eventid uuid NOT NULL,
    metadata character varying COLLATE pg_catalog."default" NOT NULL,
    metadatahash bigint NOT NULL
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE event_log.event_metadata
    OWNER to postgres;

CREATE UNIQUE INDEX ix_eventmetadata_eventid
    ON event_log.event_metadata USING btree
    (eventid ASC NULLS LAST, metadatahash ASC NULLS LAST)
    TABLESPACE pg_default;

CREATE INDEX ix_eventmetadata_metadatahash
    ON event_log.event_metadata USING btree
    (metadatahash ASC NULLS LAST)
    TABLESPACE pg_default;


-- Sample LogObjectSql

DO $$
BEGIN
	IF EXISTS (SELECT 0 FROM event_log.objects WHERE namehash = [ObjectNameHash])
	THEN
		IF '[ObjectCreationTime]' <> '1970-01-01 00:00'
		THEN
			UPDATE event_log.objects SET creationtime = '[ObjectCreationTime]' WHERE namehash = [ObjectNameHash];
		END IF;
	ELSE
		INSERT INTO event_log.objects
		SELECT '[ObjectID]', '[ObjectName]', '[ObjectCreationTime]', [ObjectNameHash];
	END IF;
END $$;

-- Sample LogEventSql

INSERT INTO event_log.events 
SELECT '[EventID]', objects.ID, '[EventName]', '[MachineName]', '[ProcessName]', '[EventTime]'
FROM event_log.objects
WHERE (objects.ID = '[ObjectID]') OR ('[ObjectID]' = '' AND objects.Name = '[ObjectName]');


-- Sample LogMetaSql

DO $$
BEGIN
	IF NOT EXISTS (SELECT 0 FROM event_log.event_metadata WHERE eventid = '[EventID]' AND metadatahash = [EventMetadataHash])
	THEN
		INSERT INTO event_log.event_metadata 
		SELECT '[EventID]', '[EventMetadata]', [EventMetadataHash];	
	END IF;
END $$;



*/

using System;
using System.Data;
using System.Collections.Generic;
using System.ComponentModel;

namespace STEM.Surge.PostGreSQL
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("PostGres Logger Default Schema")]
    [Description("Plants a named ILogger in session that is backed by a PostGreSql Database with the STEM Logging Database Schema. " +
        "This is used by the STEM.Surge.ObjectTracking extension or directly through ILogger.GetLogger(loggerName).")]
    public class PostGresLoggerDefaultSchema : PostGresLogger
    {
        public PostGresLoggerDefaultSchema()
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

                enq.ImportDataTable(Authentication, ot, ot.TableName);

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

                enq.ImportDataTable(Authentication, et, et.TableName);

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

                enq.ImportDataTable(Authentication, mt, mt.TableName);

                return exceptions.Count == 0;
            }
            catch
            {
                return base.BulkLoad(meta, out exceptions);
            }
        }
                     
        protected DataTable Build_ObjectsTable()
        {
            DataTable t = new DataTable("event_log.objects");
            t.Columns.Add(new DataColumn { ColumnName = "id", DataType = Type.GetType("System.Guid") });
            t.Columns.Add(new DataColumn { ColumnName = "name", DataType = Type.GetType("System.String") });
            t.Columns.Add(new DataColumn { ColumnName = "creationtime", DataType = Type.GetType("System.DateTime") });
            t.Columns.Add(new DataColumn { ColumnName = "namehash", DataType = Type.GetType("System.Int64") });

            return t;
        }
        protected DataTable Build_EventTable()
        {
            DataTable t = new DataTable("event_log.events");
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
            DataTable t = new DataTable("event_log.event_metadata");
            t.Columns.Add(new DataColumn { ColumnName = "eventid", DataType = Type.GetType("System.Guid") });
            t.Columns.Add(new DataColumn { ColumnName = "metadata", DataType = Type.GetType("System.String") });
            t.Columns.Add(new DataColumn { ColumnName = "metadatahash", DataType = Type.GetType("System.Int64") });

            return t;
        }
    }
}
