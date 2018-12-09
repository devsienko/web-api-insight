using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using log4net;
using Microsoft.Web.Administration;

namespace WebApiMonitor.Agent.Util
{
    public class ProcessHelper
    {
        const int NotRunningPid = -1;
        static readonly object SyncObject = new object();//todo: refactor, consumer producer template
        private static readonly ILog Logger = LogHelper.GetLogger();

        public static int GetIisProcessId(string appPoolName)
        {
            lock (SyncObject)
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
            return NotRunningPid;
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
            if (appInfo.CurrentSite != null)
            {
                var result = string.Format("_LM_W3SVC_{0}_ROOT", appInfo.CurrentSite.Id);
                return result;
            }
            throw new InvalidOperationException("Application instance name not found.");
        }

        public static string GetLogsPath(string appName, string poolName)
        {
            var appInfo = GetAppInfo(appName, poolName);
            var result = string.Format(@"{0}\W3SVC{1}", appInfo.CurrentSite.LogFile.Directory, appInfo.CurrentSite.Id);
            Logger.Info(string.Format("iis log path: {0}", result));
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
            var result = pid != NotRunningPid;
            return result;
        }
    }
}
