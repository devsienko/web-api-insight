using log4net;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace WebApiInsight.Agent
{
    public class RequestPerSecCollector
    {
        readonly ILog _logger;
        readonly IDbManager _dbManager;

        public RequestPerSecCollector(ILog logger, IDbManager dbManager)
        {
            _logger = logger;
            _dbManager = dbManager;
        }

        public void Start()
        {
            var iisPoolPid = ProcessHelper.GetIisProcessID(Settings.PoolName);
            var instanceName = ProcessHelper.GetInstanseName(Settings.AppName, Settings.PoolName);
            var requestCounter = new PerformanceCounter("ASP.NET Applications", "Requests/Sec", instanceName);
            var requestFailedCounter = new PerformanceCounter("ASP.NET Applications", "Requests Failed", instanceName);
            var instanceNames = new PerformanceCounterCategory("ASP.NET Applications")
                     .GetInstanceNames()
                     .OrderBy(x => x);
            while (true)
            {
                var value = requestCounter.NextValue();
                var valueReqFailed = requestFailedCounter.NextValue();
                _dbManager.WriteMetrics("req-per-sec", value);
                _dbManager.WriteMetrics("req-failed", valueReqFailed);

                Thread.Sleep(Settings.ReadingInterval);
            }
        }
    }
}
