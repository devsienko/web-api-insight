using Microsoft.Web.Administration;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace WebApiMonitor.Agent
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
            var appInfo = GetAppInfo(appName, poolName);
            if (appInfo.CurrentApp != null && appInfo.CurrentSite != null)
            {
                var result = string.Format("_LM_W3SVC_{0}_ROOT_{1}",
                                appInfo.CurrentSite.Id,
                                GetAppNameByPath(appInfo.CurrentApp.Path));
                return result;
            }
            else if (appInfo.CurrentSite != null)
            {
                var result = string.Format("_LM_W3SVC_{0}_ROOT", appInfo.CurrentSite.Id);
                return result;
            }
            else
                throw new InvalidOperationException("Application instance name not found.");
        }

        public static string GetLogsPath(string appName, string poolName)
        {
            var appInfo = GetAppInfo(appName, poolName);
            var result = string.Format(@"{0}\W3SVC{1}", appInfo.CurrentSite.LogFile.Directory, appInfo.CurrentSite.Id);
            result = Environment.ExpandEnvironmentVariables(result);
            return result;
        }

        private static AppInfo GetAppInfo(string appName, string poolName)
        {
            var result = new AppInfo();
            using (var serverManager = ServerManager.OpenRemote("localhost"))
            {
                foreach (var site in serverManager.Sites)
                {
                    if (site.Name.ToUpper() == appName.ToUpper()
                        && site.ApplicationDefaults.ApplicationPoolName.ToUpper() == poolName.ToUpper())
                    {
                        result.CurrentSite = site;
                        return result;
                    }
                    foreach (var app in site.Applications)
                    {
                        if (app.Path.Equals("/" + appName) && app.ApplicationPoolName == poolName)
                        {
                            result.CurrentSite = site;
                            result.CurrentApp = app;
                            return result;
                        }
                    }
                }
            }
            return result;
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
