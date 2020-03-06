using System;
using System.Collections.Generic;
using System.Text;

namespace STEM.Surge.Logging
{
    public abstract class ILogger : Instruction
    {
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

        /// <summary>
        /// Log an event on an object
        /// </summary>
        /// <param name="eventID">The Guid of the event to which thismeta data belongs</param>
        /// <param name="meta">The metadata to record</param>
        /// <returns>The true on success.</returns>
        public abstract bool LogEventMetadata(Guid eventID, string metadata);

        /// <summary>
        /// Log an event on an object
        /// </summary>
        /// <param name="objectID">The Guid of the object upon which the event occurred</param>
        /// <param name="eventName">The name of the event</param>
        /// <param name="processName">The name of the proces logging the event</param>
        /// <param name="eventTime">The time of the event</param>
        /// <returns>The Guid of the Event recorded (Guid.Empty upon failure).</returns>
        public abstract Guid LogEvent(Guid objectID, string eventName, string processName, DateTime eventTime);

        /// <summary>
        /// Log an event on an object, defaulting eventTime to Utc Now
        /// </summary>
        /// <param name="objectID">The Guid of the object upon which the event occurred</param>
        /// <param name="eventName">The name of the event</param>
        /// <param name="processName">The name of the proces logging the event</param>
        /// <returns>The Guid of the Event recorded (Guid.Empty upon failure).</returns>
        public virtual Guid LogEvent(Guid objectID, string eventName, string processName)
        {
            return LogEvent(objectID, eventName, processName, DateTime.UtcNow);
        }

        /// <summary>
        /// Log an event on an object
        /// </summary>
        /// <param name="objectName">The name of the object upon which the event occurred (e.g. filename sans path)</param>
        /// <param name="eventName">The name of the event</param>
        /// <param name="processName">The name of the proces logging the event</param>
        /// <param name="eventTime">The time of the event</param>
        /// <returns>The Guid of the Event recorded (Guid.Empty upon failure).</returns>
        public abstract Guid LogEvent(string objectName, string eventName, string processName, DateTime eventTime);

        /// <summary>
        /// Log an event on an object, defaulting eventTime to Utc Now
        /// </summary>
        /// <param name="objectName">The name of the object upon which the event occurred (e.g. filename sans path)</param>
        /// <param name="eventName">The name of the event</param>
        /// <param name="processName">The name of the proces logging the event</param>
        /// <returns>The Guid of the Event recorded (Guid.Empty upon failure).</returns>
        public virtual Guid LogEvent(string objectName, string eventName, string processName)
        {
            return LogEvent(objectName, eventName, processName, DateTime.UtcNow);
        }

        /// <summary>
        /// Log an event independent of any specific object
        /// </summary>
        /// <param name="eventName">The name of the event</param>
        /// <param name="processName">The name of the proces logging the event</param>
        /// <param name="eventTime">The time of the event</param>
        /// <returns>The Guid of the Event recorded (Guid.Empty upon failure).</returns>
        public virtual Guid LogEvent(string eventName, string processName, DateTime eventTime)
        {
            return LogEvent(Guid.Empty, eventName, processName, eventTime);
        }

        /// <summary>
        /// Log an event independent of any specific object, defaulting eventTime to Utc Now
        /// </summary>
        /// <param name="eventName">The name of the event</param>
        /// <param name="processName">The name of the proces logging the event</param>
        /// <returns>The Guid of the Event recorded (Guid.Empty upon failure).</returns>
        public virtual Guid LogEvent(string eventName, string processName)
        {
            return LogEvent(Guid.Empty, eventName, processName, DateTime.UtcNow);
        }

        /// <summary>
        /// Set/Update metadata about an object
        /// </summary>
        /// <param name="objectID">The Guid of the object about which the metadata applies</param>
        /// <param name="objectName">The name of the object (e.g. filename sans path)</param>
        /// <param name="creationTime">The creation time of the object (e.g. file creation time UTC)</param>
        /// <returns>True if recorded</returns>
        public abstract bool SetObjectInfo(Guid objectID, string objectName, DateTime creationTime);

        /// <summary>
        /// Set/Update metadata about an object, defaulting creationTime to Utc Now
        /// </summary>
        /// <param name="objectID">The Guid of the object about which the metadata applies</param>
        /// <param name="objectName">The name of the object (e.g. filename sans path)</param>
        /// <returns>True if recorded</returns>
        public virtual bool SetObjectInfo(Guid objectID, string objectName)
        {
            return SetObjectInfo(objectID, objectName, DateTime.UtcNow);
        }
        
        protected sealed override void _Rollback()
        {
        }

        protected sealed override bool _Run()
        {
            // Plant logger

            STEM.Sys.Global.Session[LoggerName] = this;

            return true;
        }
    }
}
