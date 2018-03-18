using Microsoft.Web.Administration;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace WebApiInsight.Agent
{
    public class ProcessHelper : IDisposable
    {
        const int NOT_RUNNING_PID = -1;
        private ServerManager serverManager = new ServerManager();

        public void Dispose()
        {
            serverManager.Dispose();
        }

        public int GetIisProcessID(string appPoolName)
        {
            serverManager.CommitChanges(); //to update state of iis
            foreach (var workerProcess in serverManager.WorkerProcesses)
            {
                if (workerProcess.AppPoolName.Equals(appPoolName))
                    return workerProcess.ProcessId;
            }
            return NOT_RUNNING_PID;
        }

        public string GetInstanceNameForProcessId(int processId)
        {
            string result = null;
            var process = Process.GetProcessById(processId);
            var processName = Path.GetFileNameWithoutExtension(process.ProcessName);

            var performanceCat = new PerformanceCounterCategory("Process");
            var instances = performanceCat.GetInstanceNames()
                .Where(inst => inst.StartsWith(processName))
                .ToArray();
            foreach (string instance in instances)
            {
                using (var cnt = new PerformanceCounter("Process", "ID Process", instance, true))
                {
                    int val = (int)cnt.RawValue;
                    if (val == processId)
                    {
                        result = instance;
                        break;
                    }
                }
            }
            return result;
        }

        public bool IsPoolAlive(int pid)
        {
            var result = pid != NOT_RUNNING_PID;
            return result;
        }
    }
}
