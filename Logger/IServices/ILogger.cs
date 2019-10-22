using System;

namespace Logger.IServices
{
    public interface ILogger
    {
        string Path { get; set; }
        string LogLineBuilder<T> ( T[] data );
        void Log (string callingClass, string message, Validator.ValidationResult? result, bool conductedTest = false);
    }


}
