using System;

using MultiLogger.Model;

namespace MultiLogger.Interfaces
{
    public interface ILogger<T>
    {
        void Start ();
        void Stop ();

        void MSLog ( MSLogEntry e );

        void Debug ( string project, string actor, string component, string operation, string message, params object[] values );
        void Information ( string project, string actor, string component, string operation, string message, params object[] values );
        void Error ( string project, string actor, string component, string operation, string message, params object[] values );
        void Warning ( string project, string actor, string component, string operation, string message, params object[] values );
        void Log ( string project, string actor, LogLevel logLevel, string component, string operation, string message, params object[] values );
        void Log ( T logEntry );
        void UnhandledException ( Exception e);
    }
}
