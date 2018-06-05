using log4net.Appender;
using System;
using System.IO;
using System.Linq;

namespace WebApiMonitor.Agent.Util
{
    /// <summary>
    /// Appender remains only number of the log files that setup in configuration file. The oldest files that are out of board
    /// are deleted.
    /// </summary>
    public class LimitFilesNumberAppender : RollingFileAppender
    {
        public int MaximumFiles { get; set; }

        protected override void OpenFile(string fileName, bool append)
        {
            base.OpenFile(fileName, append);
            CheckFilesNumber();
        }

        private void CheckFilesNumber()
        {
            try
            {
                if (MaximumFiles <= 0)
                    return;

                var directory = Path.GetDirectoryName(File);
                DirectoryInfo dir = new DirectoryInfo(directory);
                var files = dir.GetFiles();
                var count = files.Count();
                var toDeleteNumber = count - MaximumFiles;
                if (toDeleteNumber > 0)
                {
                    var filesToDelete = files.OrderBy(x => x.LastWriteTime).Take(toDeleteNumber).ToArray();
                    for (int i = 0; i < toDeleteNumber; i++)
                        filesToDelete[i].Delete();

                    Writer.WriteLine("LimitFilesNumberAppender: deleted " + toDeleteNumber + " files.");
                }
            }
            catch (Exception e)
            {
                Writer.WriteLine("LimitFilesNumberAppender: cannot verify and/or delete log files. " + e.Message);
            }
        }
    }
}