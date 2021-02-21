using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CustomLogger.Interfaces;

using Microsoft.Extensions.Options;

using RestrictionsLimitPopup.Common.Extensions;

namespace AdvLogger
{
    public class FileLogWriter : ILogWriter
    {
        internal FileLoggerOptions LoggerOptions { get; private set; }
        internal string FilePath { get; private set; }
        private ConcurrentQueue<string> logLines = new ConcurrentQueue<string>();

        private bool writerTerminate = true;
        private IDisposable SettingsChangeToken = null;
        private int counter = 0;

        public FileLogWriter ( IOptionsMonitor<FileLoggerOptions> loggerOptions )
        {
            SettingsChangeToken = loggerOptions.OnChange(settings =>
            {
                LoggerOptions = settings;
                PrepareFile();
            });
        }

        public void AddLog ( string logEntry, params object[] values )
        {
            logLines.Enqueue(logEntry.FormatPlaceholder(values).Item1);
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
            FilePath = Path.Combine(LoggerOptions.Folder, "app_logs",
                DateTime.Now.ToString("dd-MMM-yyyy_HH-mm") + ".log");

            if (File.Exists(FilePath))
            {
                File.Create(FilePath);
                ApplyRetainPolicy();
            }
        }

        private void ApplyRetainPolicy ()
        {
            FileInfo FI;
            try
            {
                List<FileInfo> FileList = new DirectoryInfo(LoggerOptions.Folder)
                .GetFiles("*.log", SearchOption.TopDirectoryOnly)
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
                        ApplyRetainPolicy();
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
    }
}
