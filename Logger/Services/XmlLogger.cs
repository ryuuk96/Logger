using Logger.IServices;

namespace Logger.Services
{
    public class XmlLogger : ILogger
    {
        public XmlLogger ( string path )
        {
            Path=path;
        }

        public string Path { get; set; }

        public void Log ( string callingClass, string message, Validator.ValidationResult? result, bool conductedTest = false )
        {
            throw new System.NotImplementedException();
        }

        public string LogLineBuilder<T> ( T[] data )
        {
            return "";
        }
    }
}
