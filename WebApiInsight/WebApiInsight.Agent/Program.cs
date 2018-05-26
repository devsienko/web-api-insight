using WebApiInsight.Agent.Util;
using log4net;
using System.Threading;
using System.Collections.Generic;

namespace WebApiInsight.Agent
{
    class Program
    {
        static readonly ILog _logger = LogHelper.GetLogger();

        static void Main()
        {
            var influxDbManager = new InfluxDbManager(_logger);
            var cpuMemCollector = new CpuAndMemoryCollector(_logger, influxDbManager);
            var reqPerSecCollector = new RequestPerSecCollector(_logger, influxDbManager);
            var collectorThreads = new List<Thread>
            {
                new Thread(reqPerSecCollector.Start),
                new Thread(cpuMemCollector.Start),
            };
            collectorThreads.ForEach(t => t.Start());

            var iisReader = new IisLogReader(_logger, influxDbManager, ProcessHelper.GetLogsPath(Settings.AppName, Settings.PoolName));
            iisReader.Process();
        }
    }
}
