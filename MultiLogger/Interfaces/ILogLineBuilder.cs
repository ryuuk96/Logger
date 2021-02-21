namespace MultiLogger.Interfaces
{
    /// <summary>
    /// Building the Log line from the object
    /// </summary>
    /// <typeparam name="T1">Value to return by formatting the logEntry, can be string or any other object</typeparam>
    /// <typeparam name="T2">LogEntry object</typeparam>
    public interface ILogLineBuilder<T1, T2>
    {
        T1 BuildLogLine ( T2 logEntry );
    }
}
