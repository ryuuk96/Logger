using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MultiLogger.Interfaces;
using MultiLogger.Model;

using Microsoft.Extensions.Options;
using Utilities.Extensions;

namespace MultiLogger.Writers
{
    public class FileLogger : IMultiLogger<DetailedLogEntry>,
        ILogLineBuilder<string, DetailedLogEntry>,
        ILogWriter<string>
    {
        public bool IsEnabled ( LogLevel logLevel ) =>
            logLevel >= LoggerOptions.LogLevel;

        internal FileLoggerOptions LoggerOptions { get; private set; }
        internal string FilePath { get; private set; }
        private ConcurrentQueue<string> logLines = new ConcurrentQueue<string>();

        private bool writerTerminate = true;
        private IDisposable SettingsChangeToken = null;
        private int counter = 0;

        public FileLogger ( IOptionsMonitor<FileLoggerOptions> loggerOptions )
        {
            LoggerOptions = loggerOptions.CurrentValue;
            SettingsChangeToken = loggerOptions.OnChange(settings =>
            {
                LoggerOptions = settings;
                PrepareFile();
            });
        }

        private string GetStringDelimiter ()
        {
            if (LoggerOptions.FileType == FileLoggerOptions.FileTypeFormat.CSV)
                return " , ";
            return " | ";
        }

        private string GetFileExtension ()
        {
            if (LoggerOptions.FileType == FileLoggerOptions.FileTypeFormat.CSV)
                return ".csv";
            return ".log";
        }

        public string BuildLogLine ( DetailedLogEntry logEntry )
        {
            // specific to text
            StringBuilder sb = new StringBuilder();
            string delimiter = GetStringDelimiter();

            sb.Append(logEntry.Project.TrimOrPad(FileLogPropLength.Project)); sb.Append(delimiter);
            sb.Append(logEntry.Timestamp.ToString("O").TrimOrPad(FileLogPropLength.Timestamp)); sb.Append(delimiter);
            sb.Append(logEntry.Actor.TrimOrPad(FileLogPropLength.Actor)); sb.Append(delimiter);
            sb.Append(logEntry.LogLevel.ToString().TrimOrPad(FileLogPropLength.LogLevel)); sb.Append(delimiter);
            sb.Append(logEntry.Component.TrimOrPad(FileLogPropLength.Component)); sb.Append(delimiter);
            sb.Append(logEntry.Operation.TrimOrPad(FileLogPropLength.Operation)); sb.Append(delimiter);
            sb.Append(logEntry.LogMessage);
            sb.Append("\r\n");

            return sb.ToString();
        }

        public void Log ( DetailedLogEntry logEntry )
        {
            if (IsEnabled(logEntry.LogLevel))
                AddLog(BuildLogLine(logEntry));
        }


        public void AddLog ( string logEntry )
        {
            logLines.Enqueue(logEntry);
        }

        public void Start ()
        {
            writerTerminate = false;
            PrepareFile();
            InitiateFileWriter();
        }

        private void PrepareFile ()
        {
            Directory.CreateDirectory(LoggerOptions.Folder);
            string fileExtension = GetFileExtension();

            FilePath = Path.Combine(LoggerOptions.Folder, DateTime.Now.ToString("dd-MMM-yyyy") + fileExtension);

            StringBuilder sb = new StringBuilder();
            string delimiter = GetStringDelimiter();

            sb.Append("Project".TrimOrPad(FileLogPropLength.Project)); sb.Append(delimiter);
            sb.Append("Timestamp".TrimOrPad(FileLogPropLength.Timestamp)); sb.Append(delimiter);
            sb.Append("Actor".TrimOrPad(FileLogPropLength.Actor)); sb.Append(delimiter);
            sb.Append("LogLevel".TrimOrPad(FileLogPropLength.LogLevel)); sb.Append(delimiter);
            sb.Append("Component".TrimOrPad(FileLogPropLength.Component)); sb.Append(delimiter);
            sb.Append("Operation".TrimOrPad(FileLogPropLength.Operation)); sb.Append(delimiter);
            sb.Append("Message");
            sb.Append("\r\n");

            if (LoggerOptions.FileType == FileLoggerOptions.FileTypeFormat.Text)
                foreach (char charCount in sb.ToString())
                {
                    sb.Append("-");
                }
            sb.Append("\r\n");

            File.WriteAllText(FilePath, sb.ToString());

            ApplyRetainPolicy();
        }

        // specific to text
        private void ApplyRetainPolicy ()
        {
            FileInfo FI;
            try
            {
                string fileExtenstion = GetFileExtension();
                List<FileInfo> FileList = new DirectoryInfo(LoggerOptions.Folder)
                .GetFiles("*" + fileExtenstion, SearchOption.TopDirectoryOnly)
                .OrderBy(fi => fi.CreationTime)
                .ToList();

                while (FileList.Count >= LoggerOptions.RetainFileCount)
                {
                    FI = FileList.First();
                    FI.Delete();
                    FileList.Remove(FI);
                }
            }
            catch { }
        }

        private void InitiateFileWriter ()
        {
            Task.Run(() =>
            {
                while (!writerTerminate)
                {
                    try
                    {
                        if (logLines.TryDequeue(out string log))
                            WriteLog(log);
                        Thread.Sleep(LoggerOptions.WriteIntervalInMilliSeconds);
                    }
                    catch (Exception) { }
                }
            });
        }

        public void Stop ()
        {
            FlushLogs();
            if (SettingsChangeToken != null)
            {
                SettingsChangeToken.Dispose();
                SettingsChangeToken = null;
            }
        }

        public void WriteLog ( string logLine )
        {
            counter++;
            if (counter % 100 == 0)
            {
                FileInfo f = new FileInfo(FilePath);
                if (f.Length > (1024 * 1024 * LoggerOptions.MaxFileSizeInMB))
                    PrepareFile();
            }
            File.AppendAllText(FilePath, logLine);
        }


        private void FlushLogs ()
        {
            while (!logLines.IsEmpty)
            {
                logLines.TryDequeue(out string log);
                WriteLog(log);
            }
        }

        internal class FileLogPropLength
        {
            internal const int Project = 10;
            internal const int Timestamp = 30;
            internal const int Actor = 12;
            internal const int LogLevel = 12;
            internal const int Component = 18;
            internal const int Operation = 10;
        }
    }
}
