using System;
using System.Collections.Generic;
using System.Text;

namespace STEM.Surge.Logging
{
    public class EmptyLogger : ILogger
    {
        public override Guid LogEvent(Guid objectID, string eventName, string processName, DateTime eventTime)
        {
            return Guid.Empty;
        }

        public override Guid LogEvent(string objectName, string eventName, string processName, DateTime eventTime)
        {
            return Guid.Empty;
        }

        public override bool LogEventMetadata(Guid eventID, string metadata)
        {
            return false;
        }

        public override bool SetObjectInfo(Guid objectID, string objectName, DateTime creationTime)
        {
            return false;
        }
    }
}
