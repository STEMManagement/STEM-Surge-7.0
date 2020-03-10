using System;
using System.Collections.Generic;
using System.Text;

namespace STEM.Surge.Logging
{
    public class EmptyLoggerReference : Exception
    {
        public EmptyLoggerReference() { }
        public EmptyLoggerReference(string message) : base(message) { }
    }

    public class EmptyLogger : ILogger
    {
        public override Guid LogEvent(Guid objectID, string eventName, string processName, DateTime eventTime, out List<Exception> exceptions)
        {
            exceptions = new List<Exception>(new Exception[] { new EmptyLoggerReference("The EmptyLogger was called.") });
            return Guid.Empty;
        }

        public override Guid LogEvent(string objectName, string eventName, string processName, DateTime eventTime, out List<Exception> exceptions)
        {
            exceptions = new List<Exception>(new Exception[] { new EmptyLoggerReference("The EmptyLogger was called.") });
            return Guid.Empty;
        }

        public override bool LogEventMetadata(Guid eventID, string metadata, out List<Exception> exceptions)
        {
            exceptions = new List<Exception>(new Exception[] { new EmptyLoggerReference("The EmptyLogger was called.") });
            return false;
        }

        public override bool SetObjectInfo(Guid objectID, string objectName, DateTime creationTime, out List<Exception> exceptions)
        {
            exceptions = new List<Exception>(new Exception[] { new EmptyLoggerReference("The EmptyLogger was called.") });
            return false;
        }

        public override bool BulkLoad(List<EventData> events, out List<Exception> exceptions)
        {
            exceptions = new List<Exception>(new Exception[] { new EmptyLoggerReference("The EmptyLogger was called.") });
            return false;
        }

        public override bool BulkLoad(List<EventMetadata> meta, out List<Exception> exceptions)
        {
            exceptions = new List<Exception>(new Exception[] { new EmptyLoggerReference("The EmptyLogger was called.") });
            return false;
        }

        public override bool BulkLoad(List<ObjectData> objects, out List<Exception> exceptions)
        {
            exceptions = new List<Exception>(new Exception[] { new EmptyLoggerReference("The EmptyLogger was called.") });
            return false;
        }
    }
}
