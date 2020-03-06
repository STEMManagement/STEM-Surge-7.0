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

CREATE TABLE [dbo].[EventMetadata](
	[EventID] [uniqueidentifier] NOT NULL,
	[Metadata] [nvarchar](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE TABLE [dbo].[Events](
	[ID] [uniqueidentifier] NOT NULL,
	[ObjectID] [uniqueidentifier] NOT NULL,
	[EventName] [nvarchar](50) NOT NULL,
	[MachineName] [nvarchar](50) NOT NULL,
	[ProcessName] [nvarchar](50) NOT NULL,
	[EventTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Events] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[Objects](
	[ID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[CreationTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Objects] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


-- Sample LogObjectSql
INSERT Objects
SELECT '[ObjectID]', '[ObjectName]', '[ObjectCreationTime]'

-- Sample LogEventSql
INSERT Events 
SELECT '[EventID]', [Objects].ID, '[EventName]', '[MachineName]', '[ProcessName]', '[EventTime]'
FROM [Objects]
WHERE ('[ObjectName]' = '' AND [Objects].ID = '[ObjectID]') OR ('[ObjectID]' = '' AND [Objects].Name = '[ObjectName]')

-- Sample LogMetaSql
INSERT EventMetadata
SELECT '[EventID]', '[EventMetadata]'

*/


using System;
using System.Collections.Generic;
using System.ComponentModel;
using STEM.Surge.Logging;

namespace STEM.Surge.SQLServer
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DisplayName("SQLServerLogger")]
    [Description("Plants a named ILogger in session that is backed by a SQL Server Database. " +
        "This is used by the STEM.Surge.ObjectTracking extension or directly through ILogger.GetLogger(loggerName).")]
    public class SQLServerLogger : ILogger
    {
        [Category("SQL Server")]
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

        public SQLServerLogger()
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

                sql = STEM.Surge.KVPMapUtils.ApplyKVP(sql, map, false);

                if (sql.Trim() == "")
                    return false;

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
