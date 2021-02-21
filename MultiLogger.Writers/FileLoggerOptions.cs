using System.IO;

using MultiLogger.Model;

using Utilities.Extensions;

namespace MultiLogger.Writers
{
    public class FileLoggerOptions
    {
        public string DefaultLogLevel { get; set; } 
        internal LogLevel LogLevel { get => DefaultLogLevel.ToEnum(LogLevel.Information);  }
        public int MaxFileSizeInMB { get; set; } = 2;
        public int RetainFileCount { get; set; } = 5;
        public string FileOutputType { get; set; }
        internal FileTypeFormat FileType
        {
            get
            {
                return FileOutputType.ToEnum(FileTypeFormat.Text);
            }
        }
        public int WriteIntervalInMilliSeconds { get; set; } = 500;
        private string folderPath;
        public string Folder
        {
            get
            {
                if (string.IsNullOrWhiteSpace(folderPath))
                    folderPath = Path.GetDirectoryName(this.GetType().Assembly.Location);
                return folderPath;
            }
            set { folderPath = value; }
        }
    
        internal enum FileTypeFormat
        {
            CSV,
            Text
        }
    }
}
