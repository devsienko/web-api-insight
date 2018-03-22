using WebApiInsight.Agent.Util;
using log4net;
using System.Threading;

namespace WebApiInsight.Agent
{
    class Program
    {
        static readonly ILog _logger = LogHelper.GetLogger();

        static void Main()
        {
            //var iisPoolPid = ProcessHelper.GetIisProcessID(PoolName);
            //var instanseName = ProcessHelper.GetInstanceNameForProcessId(iisPoolPid);
            ////instanseName = "_LM_W3SVC_1_ROOT_MySite";
            //var requestCounter = new PerformanceCounter(".NET CLR Memory", "% Time in GC", instanseName);
            ////var requestCounter = new PerformanceCounter("ASP.NET Applications", "Requests/Sec", instanseName);
            //var instanceNames = new PerformanceCounterCategory("ASP.NET Applications")
            //         .GetInstanceNames()
            //         .OrderBy(x => x);
            //while (true)
            //{
            //    var value = requestCounter.NextValue();
            //    _logger.InfoFormat(value);

            //    Thread.Sleep(ReadingInterval);
            //}
            var influxDbManager = new InfluxDbManager(_logger);
            var collector = new CpuAndMemoryCollector(_logger, influxDbManager);
            var collectorThread = new Thread(collector.Start);
            collectorThread.Start();
        }
    }
}
