using System;
using System.Collections.Generic;

namespace MultiLogger.Model
{
    public class DetailedLogEntry
    {
        public DetailedLogEntry ()
        {
            Timestamp = DateTime.UtcNow;
        }
        public string Project { get; set; }
        public string Actor { get; set; }
        public LogLevel LogLevel { get; set; }
        public DateTime Timestamp { get; private set; }
        public string Component { get; set; }
        public string Operation { get; set; }
        public string LogMessage { get; set; }
        public Dictionary<string, string> LogProperties { get; set; } = null;
    }
}
