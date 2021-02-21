using System;
using System.Collections.Concurrent;

using MultiLogger.Interfaces;
using MultiLogger.Model;

using Microsoft.Extensions.Logging;

namespace MultiLogger
{
    public class MSLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, MSLogger> _loggers = new ConcurrentDictionary<string, MSLogger>();
        private readonly Interfaces.ILogger<DetailedLogEntry> _MultiLogger;
        public MSLoggerProvider ( Interfaces.ILogger<DetailedLogEntry> MultiLogger)
        {
            _MultiLogger = MultiLogger;
            _MultiLogger.Start();
        }
        
        public ILogger CreateLogger ( string categoryName )
        {
            return _loggers.GetOrAdd(categoryName, new MSLogger(this, categoryName));
        }

        public void WriteLog ( MSLogEntry info )
        {
            _MultiLogger.MSLog(info);
        }

        public void Dispose ()
        {
            _MultiLogger.Stop();
            _loggers.Clear();
        }
    }
}
