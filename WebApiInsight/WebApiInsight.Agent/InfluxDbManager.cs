using InfluxDB.Collector;
using log4net;
using System;
using System.Collections.Generic;

namespace WebApiInsight.Agent
{
    public class InfluxDbManager : IDbManager
    {
        readonly ILog _logger;
        readonly object _syncObject = new object();//todo: research InfluxDB.Collector sources, is it thread safe?

        public InfluxDbManager(ILog logger)
        {
            _logger = logger;
            InitStorage();
        }

        private void InitStorage()
        {
            Metrics.Collector = new CollectorConfiguration()
               .Tag.With("pool-name", Settings.PoolName)
               .Tag.With("host", Environment.GetEnvironmentVariable("COMPUTERNAME"))
               .Tag.With("os", Environment.GetEnvironmentVariable("OS"))
               .Batch.AtInterval(TimeSpan.FromSeconds(2))
               .WriteTo.InfluxDB(Settings.DbAddress, Settings.DbName)
               .CreateCollector();
            _logger.InfoFormat("Storage was inited (db address: {0}, db name: {1})", Settings.DbAddress, Settings.DbName);
        }
        
        public void WriteMetrics(string measurement, object value)
        {
            lock (_syncObject)
            {
                Metrics.Write(measurement, new Dictionary<string, object> { { "value", value } });
            }
        }
    }
}
