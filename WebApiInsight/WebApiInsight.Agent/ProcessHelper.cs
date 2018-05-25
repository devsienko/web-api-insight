using Microsoft.Web.Administration;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace WebApiInsight.Agent
{
    public class ProcessHelper
    {
        const int NOT_RUNNING_PID = -1;
        readonly static object _syncObject = new object();//todo: refactor, consumer producer template

        public static int GetIisProcessID(string appPoolName)
        {
            lock (_syncObject)
            {
                using (var serverManager = ServerManager.OpenRemote("localhost"))
                {
                    foreach (var workerProcess in serverManager.WorkerProcesses)
                    {
                        if (workerProcess.AppPoolName.Equals(appPoolName))
                            return workerProcess.ProcessId;
                    }
                }
            }
            return NOT_RUNNING_PID;
        }

        public static string GetInstanseName(string appName, string poolName)
        {
            using (var serverManager = ServerManager.OpenRemote("localhost"))
            {
                Site resultSite = null;
                Application resultApp = null;
                foreach (var site in serverManager.Sites)
                {
                    foreach (var app in site.Applications)
                    {
                        if (app.Path.Equals("/" + appName) && app.ApplicationPoolName == poolName)
                        {
                            resultSite = site;
                            resultApp = app;
                            break;
                        }
                    }
                }
                var result = string.Format("_LM_W3SVC_{0}_ROOT_Atrinova.Utilli.WebApi",
                    resultSite.Id,
                    GetAppNameByPath(resultApp.Path));
                return result;
            }
            
            //example of input: LM/Sites/Default Web Site/IisNodeName
            //example of result: _LM_W3SVC_1_ROOT_IisNodeName
            //var result = Settings.NodePath.Replace('/', '_');
            //result = "_LM_W3SVC_1_ROOT_Atrinova.Utilli.WebApi";

            //var test = serverManager.Sites.First();
            //var t2 = test.Applications.First(a => a.Path == "/" + "Atrinova.Utilli.WebApi");

        }

        public static string GetAppNameByPath(string path)
        {
            //we are using Last for the case of nested application, for example /Default Web Sites/app/nested-app
            var result = path.Split('/').Last(); 
            return result;
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
