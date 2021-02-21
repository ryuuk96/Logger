using MultiLogger.Model;

namespace MultiLogger.Interfaces
{
    public interface IMultiLogger<T>
    {
        bool IsEnabled (LogLevel logLevel);
        void Log ( T logEntry );
        void Start ();
        void Stop ();
    }
}
