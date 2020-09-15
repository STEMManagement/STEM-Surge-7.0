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
using System.Collections.Generic;
using System.ComponentModel;
using STEM.Surge.Logging;

namespace STEM.Surge.PostGreSQL
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[DisplayName("PostGreSql Logger")]
	[Description("Plants a named ILogger in session that is backed by a PostGreSql Database. " +
		"This is used by the STEM.Surge.ObjectTracking extension or directly through ILogger.GetLogger(loggerName).")]
	public class PostGresLogger : ILogger
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
		public virtual  List<string> LogMetaSql { get; set; }

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

		public PostGresLogger()
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
			bool success = BulkLoad(new List<EventData>( new EventData[]{ new EventData {
				EventID = eventID,
				ObjectID = objectID,
				ObjectName = "",
				EventName = eventName,
				ProcessName = processName,
				MachineName = STEM.Sys.IO.Net.MachineName(),
				EventTime = eventTime } } ), out exceptions);

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
					map["[ObjectName]"] = e.ObjectName.Replace("\"", "'").Replace("''", "'").Replace("'", "''");
					map["[ObjectNameHash]"] = STEM.Sys.State.KeyManager.GetHash(map["[ObjectName]"]).ToString();
					map["[MachineName]"] = e.MachineName.Replace("\"", "'").Replace("''", "'").Replace("'", "''");
					map["[ProcessName]"] = e.ProcessName.Replace("\"", "'").Replace("''", "'").Replace("'", "''");
					map["[EventName]"] = e.EventName.Replace("\"", "'").Replace("''", "'").Replace("'", "''");
					map["[EventTime]"] = e.EventTime.ToString("G");

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
					map["[ObjectName]"] = o.Name.Replace("\"", "'").Replace("''", "'").Replace("'", "''");
					map["[ObjectNameHash]"] = STEM.Sys.State.KeyManager.GetHash(map["[ObjectName]"]).ToString();
					map["[ObjectCreationTime]"] = o.CreationTime.ToString("G");

					cat += STEM.Surge.KVPMapUtils.ApplyKVP(sql, map, false) + ";\r\n";
				}

				ExecuteNonQuery enq = new ExecuteNonQuery();

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
					map["[EventMetadata]"] = e.Metadata.Replace("\"", "'").Replace("''", "'").Replace("'", "''");
					map["[EventMetadataHash]"] = STEM.Sys.State.KeyManager.GetHash(map["[EventMetadata]"]).ToString();

					cat += STEM.Surge.KVPMapUtils.ApplyKVP(sql, map, false) + ";\r\n";
				}

				ExecuteNonQuery enq = new ExecuteNonQuery();

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
