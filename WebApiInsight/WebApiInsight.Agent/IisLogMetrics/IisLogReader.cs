using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

// ReSharper disable once CheckNamespace
namespace WebApiInsight.Agent
{
    public class IisLogReader
    {
        readonly ILog _logger;
        readonly IDbManager _dbManager;

        private volatile string _logFilePath = string.Empty;
        private FileSystemWatcher _watcher = new FileSystemWatcher();

        private object _locker = new object();

        public readonly List<W3CEvent> Events = new List<W3CEvent>();

        public IisLogReader(ILog logger, IDbManager dbManager, string logsPath)
        {
            _logger = logger;
            _dbManager = dbManager;
            _logFilePath = new DirectoryInfo(logsPath)
                .GetFiles()
                .OrderByDescending(f => f.LastWriteTime)
                .First().FullName;
            InitLogFileWatcher(logsPath);
        }

        private void InitLogFileWatcher(string logsPath)
        {
            _watcher = new FileSystemWatcher();
            _watcher.Path = logsPath;
            _watcher.Created += (sender, e) => { lock (_locker) { _logFilePath = e.FullPath; } };
            _watcher.EnableRaisingEvents = true;
        }

        //todo: refactor using of poolName
        //todo: exceptions handling + logging
        //todo: add unit test
        /// <summary>
        /// It reads the lastest log file till the end and is triggered again after appearance the new log file.
        /// </summary>
        public void Process()
        {
            _logger.InfoFormat("Started IIS log reading. App name: {0}.", Settings.AppName);
            var currentLogFilePath = _logFilePath; //to ensure the last reading
            var currentCursor = 0;
            while (true)
            {
                var logRecords = W3CEnumerable.FromFile(currentLogFilePath).ToArray();
                if (!logRecords.Any())
                {
                    Thread.Sleep(Settings.ReadingInterval);
                    continue;
                }
                if (currentCursor == logRecords.Length)
                {
                    lock (_locker)
                    {
                        EnsureLastReading(ref currentLogFilePath, ref currentCursor);
                    }
                    Thread.Sleep(Settings.ReadingInterval);
                    continue;
                }
                var newRecords = logRecords.Skip(currentCursor);
                foreach (var record in newRecords)
                    Events.Add(record);
                _logger.InfoFormat("Added {0} records. App name: {1}", logRecords.Length - currentCursor, Settings.AppName);
                currentCursor = logRecords.Length;
                Thread.Sleep(Settings.ReadingInterval);
            }
        }

        private void EnsureLastReading(ref string logPath, ref int cursor)
        {
            var isNewLogFileReady = logPath != null && logPath != _logFilePath;
            if (isNewLogFileReady)
            {

                var logRecords = W3CEnumerable.FromFile(logPath).ToArray();
                var newRecords = logRecords.Skip(cursor);
                foreach (var record in newRecords)
                    Events.Add(record);
                _logger.InfoFormat("Switched to new file {0} -> {1}. Got {2} records. App name: {3}.",
                    logPath, _logFilePath, cursor, Settings.AppName);
                logPath = _logFilePath;
                cursor = 0;
            }
        }
    }
}
