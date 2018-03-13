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
        private volatile int _cursor;
        private volatile string _logFilePath = string.Format(@"C:\data\ten\test\u_ex{0}.log", "18021623");
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly FileSystemWatcher _watcher;
        private readonly string _poolName;

        private object _locker = new object();

        public readonly List<W3CEvent> Events = new List<W3CEvent>();

        public IisLogReader()
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            _watcher = new FileSystemWatcher();
            _watcher.Path = @"C:\data\ten\test";
            _watcher.Created += (sender, e) => { lock (_locker) { _logFilePath = e.FullPath; } };
            _watcher.EnableRaisingEvents = true;
        }

        //todo: refactor using of poolName
        //todo: exceptions handling + logging
        //todo: add unit test
        public void Process()
        {
            Console.WriteLine("Started IIS log reading. Pool name: {0}.", _poolName);
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
                Console.WriteLine("Added {0} records. Pool name: {1}", logRecords.Length - currentCursor, _poolName);
                currentCursor = logRecords.Length;

                Thread.Sleep(Settings.ReadingInterval);
            }
            // ReSharper disable once FunctionNeverReturns
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
                Console.WriteLine("Switched to new file {0} -> {1}. Got {2} records. Pool name: {3}.",
                    logPath, _logFilePath, cursor, _poolName);
                logPath = _logFilePath;
                cursor = 0;
            }
        }
    }
}
