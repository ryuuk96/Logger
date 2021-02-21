namespace MultiLogger.Interfaces
{
    public interface ILogWriter<T>
    {
        void Start ();
        void Stop ();
        void AddLog (T logEntry);
        void WriteLog (T logEntry);
    }
}
