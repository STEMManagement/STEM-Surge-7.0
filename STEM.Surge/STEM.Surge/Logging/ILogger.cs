using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace STEM.Surge.Logging
{
    public abstract class ILogger : Instruction
    {
        [Category("Logger")]
        [DisplayName("Logger Name"), DescriptionAttribute("The name used to recall this logger from Session via GetLogger().")]
        public string LoggerName { get; set; }

        public static ILogger GetLogger(string loggerName)
        {
            ILogger ret = null;

            if (!String.IsNullOrEmpty(loggerName))
            {
                ret = STEM.Sys.Global.Session[loggerName] as ILogger;
            }

            if (ret == null)
                ret = new EmptyLogger();

            ret.LoggerName = loggerName;

            return ret;
        }

        public class EventData
        {
            public EventData() { }

            public EventData(Guid objectID, string eventName, string processName, DateTime eventTime)
            {
                EventID = Guid.NewGuid();
                ObjectID = objectID;
                ObjectName = "";
                EventName = eventName;
                ProcessName = processName;
                MachineName = STEM.Sys.IO.Net.MachineName();
                EventTime = eventTime;
            }

            public EventData(string objectName, string eventName, string processName, DateTime eventTime)
            {
                EventID = Guid.NewGuid();
                ObjectID = Guid.Empty;
                ObjectName = objectName;
                EventName = eventName;
                ProcessName = processName;
                MachineName = STEM.Sys.IO.Net.MachineName();
                EventTime = eventTime;
            }

            public Guid EventID { get; set; }
            public Guid ObjectID { get; set; }
            public string ObjectName { get; set; }
            public string EventName { get; set; }
            public string ProcessName { get; set; }
            public string MachineName { get; set; }
            public DateTime EventTime { get; set; }
        }

        public class EventMetadata
        {
            public EventMetadata() { }

            public EventMetadata(Guid eventID, string metadata)
            {
                EventID = eventID;
                Metadata = metadata;
            }

            public Guid EventID { get; set; }
            public string Metadata { get; set; }
        }

        public class ObjectData
        {
            public ObjectData() { }

            public ObjectData(Guid id, string name, DateTime creationTime)
            {
                ID = id;
                Name = name;
                CreationTime = creationTime;
            }

            public Guid ID { get; set; }
            public string Name { get; set; }
            public DateTime CreationTime { get; set; }
        }

        /// <summary>
        /// Log an event on an object
        /// </summary>
        /// <param name="eventID">The Guid of the event to which thismeta data belongs</param>
        /// <param name="meta">The metadata to record</param>
        /// <returns>The true on success.</returns>
        public abstract bool LogEventMetadata(Guid eventID, string metadata, out List<Exception> exceptions);

        /// <summary>
        /// Log an event on an object
        /// </summary>
        /// <param name="objectID">The Guid of the object upon which the event occurred</param>
        /// <param name="eventName">The name of the event</param>
        /// <param name="processName">The name of the proces logging the event</param>
        /// <param name="eventTime">The time of the event</param>
        /// <returns>The Guid of the Event recorded (Guid.Empty upon failure).</returns>
        public abstract Guid LogEvent(Guid objectID, string eventName, string processName, DateTime eventTime, out List<Exception> exceptions);

        /// <summary>
        /// Log an event on an object, defaulting eventTime to Utc Now
        /// </summary>
        /// <param name="objectID">The Guid of the object upon which the event occurred</param>
        /// <param name="eventName">The name of the event</param>
        /// <param name="processName">The name of the proces logging the event</param>
        /// <returns>The Guid of the Event recorded (Guid.Empty upon failure).</returns>
        public virtual Guid LogEvent(Guid objectID, string eventName, string processName, out List<Exception> exceptions)
        {
            return LogEvent(objectID, eventName, processName, DateTime.UtcNow, out exceptions);
        }

        /// <summary>
        /// Log an event on an object
        /// </summary>
        /// <param name="objectName">The name of the object upon which the event occurred (e.g. filename sans path)</param>
        /// <param name="eventName">The name of the event</param>
        /// <param name="processName">The name of the proces logging the event</param>
        /// <param name="eventTime">The time of the event</param>
        /// <returns>The Guid of the Event recorded (Guid.Empty upon failure).</returns>
        public abstract Guid LogEvent(string objectName, string eventName, string processName, DateTime eventTime, out List<Exception> exceptions);

        /// <summary>
        /// Log an event on an object, defaulting eventTime to Utc Now
        /// </summary>
        /// <param name="objectName">The name of the object upon which the event occurred (e.g. filename sans path)</param>
        /// <param name="eventName">The name of the event</param>
        /// <param name="processName">The name of the proces logging the event</param>
        /// <returns>The Guid of the Event recorded (Guid.Empty upon failure).</returns>
        public virtual Guid LogEvent(string objectName, string eventName, string processName, out List<Exception> exceptions)
        {
            return LogEvent(objectName, eventName, processName, DateTime.UtcNow, out exceptions);
        }

        /// <summary>
        /// Log an event independent of any specific object
        /// </summary>
        /// <param name="eventName">The name of the event</param>
        /// <param name="processName">The name of the proces logging the event</param>
        /// <param name="eventTime">The time of the event</param>
        /// <returns>The Guid of the Event recorded (Guid.Empty upon failure).</returns>
        public virtual Guid LogEvent(string eventName, string processName, DateTime eventTime, out List<Exception> exceptions)
        {
            return LogEvent(Guid.Empty, eventName, processName, eventTime, out exceptions);
        }

        /// <summary>
        /// Log an event independent of any specific object, defaulting eventTime to Utc Now
        /// </summary>
        /// <param name="eventName">The name of the event</param>
        /// <param name="processName">The name of the proces logging the event</param>
        /// <returns>The Guid of the Event recorded (Guid.Empty upon failure).</returns>
        public virtual Guid LogEvent(string eventName, string processName, out List<Exception> exceptions)
        {
            return LogEvent(Guid.Empty, eventName, processName, DateTime.UtcNow, out exceptions);
        }

        /// <summary>
        /// Set/Update metadata about an object
        /// </summary>
        /// <param name="objectID">The Guid of the object about which the metadata applies</param>
        /// <param name="objectName">The name of the object (e.g. filename sans path)</param>
        /// <param name="creationTime">The creation time of the object (e.g. file creation time UTC)</param>
        /// <returns>True if recorded</returns>
        public abstract bool SetObjectInfo(Guid objectID, string objectName, DateTime creationTime, out List<Exception> exceptions);

        /// <summary>
        /// Set/Update metadata about an object, defaulting creationTime to Utc Now
        /// </summary>
        /// <param name="objectID">The Guid of the object about which the metadata applies</param>
        /// <param name="objectName">The name of the object (e.g. filename sans path)</param>
        /// <returns>True if recorded</returns>
        public virtual bool SetObjectInfo(Guid objectID, string objectName, out List<Exception> exceptions)
        {
            return SetObjectInfo(objectID, objectName, DateTime.UtcNow, out exceptions);
        }

        public abstract bool BulkLoad(List<EventData> events, out List<Exception> exceptions);
        public abstract bool BulkLoad(List<ObjectData> objects, out List<Exception> exceptions);
        public abstract bool BulkLoad(List<EventMetadata> meta, out List<Exception> exceptions);

        protected sealed override void _Rollback()
        {
        }

        protected sealed override bool _Run()
        {
            // Open logger

            Open();

            return true;
        }

        protected virtual void Open()
        {
            lock (STEM.Sys.Global.Session)
            {
                if (STEM.Sys.Global.Session[LoggerName] != null && STEM.Sys.Global.Session[LoggerName] != this)
                    ((ILogger)STEM.Sys.Global.Session[LoggerName]).Close();

                STEM.Sys.Global.Session[LoggerName] = this;
            }
        }

        protected virtual void Close()
        {
            lock (STEM.Sys.Global.Session)
            {
                STEM.Sys.Global.Session[LoggerName] = null;
            }
        }
    }
}
