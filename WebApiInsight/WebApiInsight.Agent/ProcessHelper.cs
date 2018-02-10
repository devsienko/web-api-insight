using Microsoft.Web.Administration;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace WebApiInsight.Agent
{
    class ProcessHelper
    {
        const int NOT_RUNNING_PID = -1;

        public static int GetIisProcessID(string appPoolName)
        {
            var serverManager = new ServerManager();
            foreach (var workerProcess in serverManager.WorkerProcesses)
            {
                if (workerProcess.AppPoolName.Equals(appPoolName))
                    return workerProcess.ProcessId;
            }
            return NOT_RUNNING_PID;
        }

        public static string GetInstanceNameForProcessId(int processId)
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

        public static bool IsPoolAlive(int pid)
        {
            var result = pid != NOT_RUNNING_PID;
            return result;
        }
    }
}
