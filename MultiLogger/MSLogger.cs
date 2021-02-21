using System;
using System.Collections.Generic;

using MultiLogger.Model;

using Microsoft.Extensions.Logging;

namespace MultiLogger
{
    internal class MSLogger : ILogger
    {
        public string Category { get; private set; }
        public MSLoggerProvider Provider { get; private set; }

        public MSLogger ( MSLoggerProvider provider, string Category )
        {
            this.Category = Category;
            this.Provider = provider;
        }

        bool ILogger.IsEnabled ( Microsoft.Extensions.Logging.LogLevel logLevel )
        {
            // TODO: Will need to handle the default true
            return true;
        }

        public IDisposable BeginScope<TState> ( TState state ) => default;

        void ILogger.Log<TState> ( Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId,
            TState state, Exception exception, Func<TState, Exception, string> formatter )
        {
            if (!(this as ILogger).IsEnabled(logLevel))
                return;

            MSLogEntry logInfo = new MSLogEntry();
            logInfo.Category = this.Category;
            logInfo.Level = logLevel;
            // well, the passed default formatter function 
            // does not take the exception into account
            // SEE: https://github.com/aspnet/Extensions/blob/master/src/Logging/Logging.Abstractions/src/LoggerExtensions.cs
            logInfo.Text = exception?.Message ?? state.ToString(); // formatter(state, exception)
            logInfo.Exception = exception;
            logInfo.EventId = eventId;
            logInfo.State = state;

            // well, you never know what it really is
            if (state is string)
            {
                logInfo.StateText = state.ToString();
            }
            // in case we have to do with a message template, 
            // let's get the keys and values (for Structured Logging providers)
            // SEE: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging#log-message-template
            // SEE: https://softwareengineering.stackexchange.com/questions/312197/benefits-of-structured-logging-vs-basic-logging
            else if (state is IEnumerable<KeyValuePair<string, object>> Properties)
            {
                logInfo.StateProperties = new Dictionary<string, object>();

                foreach (KeyValuePair<string, object> item in Properties)
                {
                    if (!logInfo.StateProperties.ContainsKey(item.Key))
                        logInfo.StateProperties.Add(item.Key, item.Value);
                    else
                        logInfo.StateProperties[item.Key] = item.Value;
                }
            }

            Provider.WriteLog(logInfo);
        }
    }
}
