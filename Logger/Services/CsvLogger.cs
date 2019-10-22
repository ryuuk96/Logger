using System;
using Logger.IServices;

namespace Logger.Services
{
    public class CsvLogger : ILogger
    {
        public CsvLogger ( string path )
        {
            Path=path;
        }

        public string Path { get; set; }

        public void Log ( string callingClass, string message, Validator.ValidationResult? result, bool conductedTest = false )
        {
            throw new NotImplementedException();
        }

        public string LogLineBuilder<T> ( T[] data )
        {
            return "";
        }
    }
}
