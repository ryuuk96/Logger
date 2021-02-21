using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

namespace MultiLogger.Model
{
    public class MSLogEntry
    {
        public MSLogEntry ()
        {
            TimeStampUtc = DateTime.UtcNow;
            UserName = Environment.UserName;
        }

        public static readonly string StaticHostName = System.Net.Dns.GetHostName();

        public string UserName { get; private set; }
        public string HostName { get { return StaticHostName; } }
        public DateTime TimeStampUtc { get; private set; }
        public string Category { get; set; }
        public Microsoft.Extensions.Logging.LogLevel Level { get; set; }
        public string Text { get; set; }
        public Exception Exception { get; set; }
        public EventId EventId { get; set; }
        public object State { get; set; }
        public string StateText { get; set; }
        public Dictionary<string, object> StateProperties { get; set; }
        public List<MSLogScopeInfo> Scopes { get; set; }
    }
}
