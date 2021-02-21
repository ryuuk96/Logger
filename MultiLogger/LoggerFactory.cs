using System;
using System.Collections.Generic;
using System.Linq;

using MultiLogger.Interfaces;
using MultiLogger.Model;

using Utilities.Extensions;

namespace MultiLogger
{
    public class LoggerFactory : ILogger<DetailedLogEntry>
    {
        private readonly IEnumerable<IMultiLogger<DetailedLogEntry>> _loggers;
        public LoggerFactory ( IEnumerable<IMultiLogger<DetailedLogEntry>> loggers )
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;
            _loggers = loggers;
        }

        private void UnhandledException ( object sender, UnhandledExceptionEventArgs e )
        {
            UnhandledException((Exception)e.ExceptionObject);
        }

        public void UnhandledException ( Exception e )
        {
            DetailedLogEntry logEntry = ExceptionToLogEntry(e);
            Log(logEntry);
            throw e;
        }

        public void Debug ( string project, string actor, string component, string operation, string message, params object[] values )
        {
            Log(project, actor, LogLevel.Debug, component, operation, message, values);
        }

        public void Error ( string project, string actor, string component, string operation, string message, params object[] values )
        {
            Log(project, actor, LogLevel.Error, component, operation, message, values);
        }

        public void Information ( string project, string actor, string component, string operation, string message, params object[] values )
        {
            Log(project, actor, LogLevel.Information, component, operation, message, values);
        }

        public void Warning ( string project, string actor, string component, string operation, string message, params object[] values )
        {
            Log(project, actor, LogLevel.Warning, component, operation, message, values);
        }

        public void Log ( string project, string actor, LogLevel logLevel, string component, string operation, string message, params object[] values )
        {
            var messageProps = message.FormatPlaceholder(values);
            Log(new DetailedLogEntry()
            {
                Project = project,
                Actor = actor,
                Component = component,
                LogLevel = logLevel,
                Operation = operation,
                LogMessage = messageProps.Item1,
                LogProperties = messageProps.Item2
            });
        }

        public void Log ( DetailedLogEntry logEntry )
        {
            foreach (IMultiLogger<DetailedLogEntry> logger in _loggers)
            {
                logger.Log(logEntry);
            }
        }

        public void MSLog ( MSLogEntry msLogEntry )
        {
            DetailedLogEntry logEntry = MSLogEntryToAdvancedLogEntry(msLogEntry);
            Log(logEntry);
        }

        private DetailedLogEntry MSLogEntryToAdvancedLogEntry ( MSLogEntry msLogEntry )
        {
            if (msLogEntry.Exception != null)
                return ExceptionToLogEntry(msLogEntry.Exception);

            DetailedLogEntry logEntry = new DetailedLogEntry();
            string project = "Microsoft";
            string actor = "Application";
            string component = string.Empty;
            string message = string.Empty;
            LogLevel logLevel = MSLogLevelToLogLevel(msLogEntry.Level);
            Dictionary<string, string> properties = null;
            component = msLogEntry.Category;
            if (msLogEntry.Category.Contains("Microsoft", StringComparison.OrdinalIgnoreCase))
            {
                component= msLogEntry.Category.Replace("Microsoft.", string.Empty);
                if (component.Contains("AspNetCore", StringComparison.OrdinalIgnoreCase))
                    component = component.Replace("AspNetCore.", string.Empty);
            }

            message = msLogEntry.State.ToString();
            properties = msLogEntry.StateProperties.Select(dict => new KeyValuePair<string, string>(dict.Key, dict.Value?.ToString())).ToDictionary(dict => dict.Key, dict => dict.Value);

            logEntry.Project = project;
            logEntry.Component = component;
            logEntry.Actor = actor;
            logEntry.LogLevel = logLevel;
            logEntry.LogMessage = message;
            logEntry.LogProperties = properties;
            logEntry.Operation = "INTERNAL";

            return logEntry;
        }

        private LogLevel MSLogLevelToLogLevel ( Microsoft.Extensions.Logging.LogLevel level )
        {
            switch (level)
            {
                case Microsoft.Extensions.Logging.LogLevel.Trace:
                    return LogLevel.Trace;
                case Microsoft.Extensions.Logging.LogLevel.Debug:
                    return LogLevel.Debug;
                case Microsoft.Extensions.Logging.LogLevel.Information:
                    return LogLevel.Information;
                case Microsoft.Extensions.Logging.LogLevel.Warning:
                    return LogLevel.Warning;
                case Microsoft.Extensions.Logging.LogLevel.Error:
                    return LogLevel.Error;
                case Microsoft.Extensions.Logging.LogLevel.Critical:
                    return LogLevel.Critical;
                default:
                    return LogLevel.None;
            }
        }

        private DetailedLogEntry ExceptionToLogEntry ( Exception exception )
        {
            string exptnMsg = "[Error Message]: " + exception.Message +
                "[StackTrace]: " + exception.StackTrace;
            if (exception.InnerException != null)
                exptnMsg +=
                    "[InnerException Message]: " + exception.InnerException.Message +
                    "[InnerException StackTrace]: " + exception.InnerException.StackTrace;
            string component = exception.TargetSite?.DeclaringType?.FullName ?? "UNKNOWN";
            DetailedLogEntry logEntry = new DetailedLogEntry()
            {
                Project = exception.Source ?? string.Empty,
                Actor = "Application",
                Component = component,
                LogMessage = exptnMsg,
                LogLevel = LogLevel.Critical,
                Operation = exception.TargetSite?.Name ?? string.Empty
            };
            return logEntry;
        }

        public void Start ()
        {
            foreach (var logger in _loggers)
            {
                logger.Start();
            }
        }

        public void Stop ()
        {
            foreach (var logger in _loggers)
            {
                logger.Stop();
            }
            AppDomain.CurrentDomain.UnhandledException -= UnhandledException;
        }
    }
}
