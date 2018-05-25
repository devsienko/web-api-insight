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
            var instanceNames = new PerformanceCounterCategory("ASP.NET Applications")
                     .GetInstanceNames()
                     .OrderBy(x => x);
            while (true)
            {
                var value = requestCounter.NextValue();
                Console.WriteLine(value);

                Thread.Sleep(Settings.ReadingInterval);
            }
        }
    }
}
