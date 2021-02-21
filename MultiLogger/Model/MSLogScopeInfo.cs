using System.Collections.Generic;

namespace MultiLogger.Model
{
    public class MSLogScopeInfo
    {
        public string Text { get; set; }
        public Dictionary<string, object> Properties { get; set; }
    }
}
