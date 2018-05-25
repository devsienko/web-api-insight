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
            var influxDbManager = new InfluxDbManager(_logger);
            //var collector = new CpuAndMemoryCollector(_logger, influxDbManager);
            var collector = new RequestPerSecCollector(_logger, influxDbManager);
            var collectorThread = new Thread(collector.Start);
            collectorThread.Start();
        }
    }
}
