using System;
using System.Diagnostics;
using Logger.IServices;
using Logger.Services;

namespace Logger
{
    public class DTrace
    {
        private static bool storeLog { get; set; }
        private static bool traceStart { get; set; }
        private string debugLogIdentifier = "DTrace ===> ";
        private static ILogger Log { get; set; }

        public static void Start ()
        {
            traceStart = true;
        }
        public static void End ()
        {
            traceStart = false;
            storeLog = false;
        }
        public static void Configure ( string filePath, FileType type )
        {
            storeLog = false;
            StoreLogInFormat(type, filePath);
        }

        private static void StoreLogInFormat ( FileType type, string path )
        {
            switch (type)
            {
                case FileType.csv:
                    StoreLogsInCsv(path);
                    break;
                case FileType.xml:
                    StoreLogInXml(path);
                    break;
            }
        }
        private static void StoreLogsInCsv ( string path )
        {
            Log = new CsvLogger(path);
        }
        private static void StoreLogInXml ( string path )
        {
            Log = new XmlLogger(path);
        }

        public static void Test ( object expected, object actual, string errorMessage )
        {
            Validator.ValidationResult result = Validator.Validate(expected, actual);
            if (result != Validator.ValidationResult.Passed)
            {
                LogToStorage(errorMessage, result, true);
            }
        }

        private static string GetClassNameFromTrace ()
        {
            var trace = new StackTrace().GetFrame(1).GetMethod();
            return trace.ReflectedType.Name;
        }

        public static void Info ( string logMessage )
        {
        }
        public static void Debug ( string logMessage )
        {
        }
        public static void Error ( Exception e )
        {
        }

        private static void LogToStorage (string message, Validator.ValidationResult result, bool isTesting = false)
        {
            if (!storeLog) return;
            string className = GetClassNameFromTrace();
            Log.Log(className, message, result, isTesting);
        }

        public enum FileType
        {
            csv,
            xml
        }
    }
}
