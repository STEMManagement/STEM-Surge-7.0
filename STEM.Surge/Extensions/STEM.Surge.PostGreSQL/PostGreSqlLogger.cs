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

CREATE TABLE event_log.objects
(
   id uuid NOT NULL,
   name character varying COLLATE pg_catalog."default" NOT NULL,
   creationtime time without time zone NOT NULL
)
WITH (
   OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE event_log.objects
   OWNER to postgres;


CREATE TABLE event_log.events
(
    id uuid NOT NULL,
    objectid uuid NOT NULL,
    eventname character varying COLLATE pg_catalog."default" NOT NULL,
    machinename character varying COLLATE pg_catalog."default" NOT NULL,
    processname character varying COLLATE pg_catalog."default" NOT NULL,
    eventtime time without time zone NOT NULL
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
    metadata character varying COLLATE pg_catalog."default" NOT NULL
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE event_log.event_metadata
    OWNER to postgres;


-- Sample LogObjectSql
INSERT event_log.objects
SELECT '[ObjectID]', '[ObjectName]', '[ObjectCreationTime]'

-- Sample LogEventSql
INSERT event_log.events 
SELECT '[EventID]', objects.ID, '[EventName]', '[MachineName]', '[ProcessName]', '[EventTime]'
FROM event_log.objects
WHERE ('[ObjectName]' = '' AND objects.ID = '[ObjectID]') OR ('[ObjectID]' = '' AND objects.Name = '[ObjectName]')

-- Sample LogMetaSql
INSERT event_log.event_metadata
SELECT '[EventID]', '[EventMetadata]'

*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using STEM.Surge.Logging;

namespace STEM.Surge.PostGreSQL
{
   [TypeConverter(typeof(ExpandableObjectConverter))]
   [DisplayName("PostGreSqlLogger")]
   [Description("Plants a named ILogger in session that is backed by a PostGreSql Database. " +
       "This is used by the STEM.Surge.ObjectTracking extension or directly through ILogger.GetLogger(loggerName).")]
   public class PostGreSqlLogger : ILogger
   {
       [Category("PostGres Server")]
       [DisplayName("Authentication"), DescriptionAttribute("The authentication configuration to be used.")]
       public Authentication Authentication { get; set; }

       [DisplayName("Log Event Sql"), DescriptionAttribute("This is the Sql that will be executed for each LogEvent call.")]
       public List<string> LogEventSql { get; set; }

       [DisplayName("Log Event Metadata"), DescriptionAttribute("This is the Sql that will be executed for each LogEventMetadata call.")]
       public List<string> LogMetaSql { get; set; }

       [DisplayName("Log Object Sql"), DescriptionAttribute("This is the Sql that will be executed for each SetObjectInfo call.")]
       public List<string> LogObjectSql { get; set; }

       [DisplayName("Available Placeholders"), DescriptionAttribute("The placeholders available for use in your Sql.")]
       [ReadOnly(true)]
       public List<string> AvailablePlaceholders
       {
           get
           {
               List<string> ret = new List<string>();

               ret.Add("[EventID]");
               ret.Add("[EventMetadata]");
               ret.Add("[ObjectID]");
               ret.Add("[ObjectName]");
               ret.Add("[ObjectCreationTime]");
               ret.Add("[MachineName]");
               ret.Add("[ProcessName]");
               ret.Add("[EventName]");
               ret.Add("[EventTime]");

               return ret;
           }
       }

       public PostGreSqlLogger()
       {
           Authentication = new Authentication();
           LogEventSql = new List<string>();
           LogObjectSql = new List<string>();
           LogMetaSql = new List<string>();
       }

       public override Guid LogEvent(Guid objectID, string eventName, string processName, DateTime eventTime)
       {
           try
           {
               ExecuteNonQuery enq = new ExecuteNonQuery();

               Dictionary<string, string> map = new Dictionary<string, string>();

               Guid eventID = Guid.NewGuid();

               map["[EventID]"] = eventID.ToString();
               map["[ObjectID]"] = objectID.ToString();
               map["[ObjectName]"] = "";
               map["[MachineName]"] = STEM.Sys.IO.Net.MachineName();
               map["[ProcessName]"] = processName;
               map["[EventName]"] = eventName;
               map["[EventTime]"] = eventTime.ToString("G");

               string sql = String.Join("\r\n", LogEventSql);

               if (sql.Trim() == "")
                   return Guid.Empty;

               sql = STEM.Surge.KVPMapUtils.ApplyKVP(sql, map, false);

               enq.Execute(Authentication, sql, 3);
               return eventID;
           }
           catch (Exception ex)
           {
               Exceptions.Add(ex);
           }

           return Guid.Empty;
       }

       public override Guid LogEvent(string objectName, string eventName, string processName, DateTime eventTime)
       {
           try
           {
               ExecuteNonQuery enq = new ExecuteNonQuery();

               Dictionary<string, string> map = new Dictionary<string, string>();

               Guid eventID = Guid.NewGuid();

               map["[EventID]"] = eventID.ToString();
               map["[ObjectID]"] = "";
               map["[ObjectName]"] = objectName;
               map["[MachineName]"] = STEM.Sys.IO.Net.MachineName();
               map["[ProcessName]"] = processName;
               map["[EventName]"] = eventName;
               map["[EventTime]"] = eventTime.ToString("G");

               string sql = String.Join("\r\n", LogEventSql);

               if (sql.Trim() == "")
                   return Guid.Empty;

               sql = STEM.Surge.KVPMapUtils.ApplyKVP(sql, map, false);

               enq.Execute(Authentication, sql, 3);
               return eventID;
           }
           catch (Exception ex)
           {
               Exceptions.Add(ex);
           }

           return Guid.Empty;
       }

       public override bool SetObjectInfo(Guid objectID, string objectName, DateTime creationTime)
       {
           try
           {
               ExecuteNonQuery enq = new ExecuteNonQuery();

               Dictionary<string, string> map = new Dictionary<string, string>();

               map["[ObjectID]"] = objectID.ToString();
               map["[ObjectName]"] = objectName;
               map["[ObjectCreationTime]"] = creationTime.ToString("G");

               string sql = String.Join("\r\n", LogObjectSql);

               if (sql.Trim() == "")
                   return false;

               sql = STEM.Surge.KVPMapUtils.ApplyKVP(sql, map, false);

               enq.Execute(Authentication, sql, 3);
               return true;
           }
           catch (Exception ex)
           {
               Exceptions.Add(ex);
           }

           return false;
       }

       public override bool LogEventMetadata(Guid eventID, string metadata)
       {
           try
           {
               ExecuteNonQuery enq = new ExecuteNonQuery();

               Dictionary<string, string> map = new Dictionary<string, string>();

               map["[EventID]"] = eventID.ToString();
               map["[EventMetadata]"] = metadata;

               string sql = String.Join("\r\n", LogMetaSql);

               if (sql.Trim() == "")
                   return false;

               sql = STEM.Surge.KVPMapUtils.ApplyKVP(sql, map, false);

               enq.Execute(Authentication, sql, 3);
               return true;
           }
           catch (Exception ex)
           {
               Exceptions.Add(ex);
           }

           return false;
       }
   }
}
