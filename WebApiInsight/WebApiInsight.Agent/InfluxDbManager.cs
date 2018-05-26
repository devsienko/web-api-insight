using log4net;
using System;
using Vibrant.InfluxDB.Client;
using Vibrant.InfluxDB.Client.Rows;

namespace WebApiInsight.Agent
{
    public class InfluxDbManager : IDbManager
    {
        readonly ILog _logger;
        readonly object _syncObject = new object();//todo: research InfluxDB.Collector sources, is it thread safe?

        public InfluxDbManager(ILog logger)
        {
            _logger = logger;
            _logger.InfoFormat("Storage was inited (db address: {0}, db name: {1})", Settings.DbAddress, Settings.DbName);
        }
        
        public void WriteMetricsValue(string measurement, object value)
        {
            var dbRecord = new DynamicInfluxRow();
            dbRecord.Fields.Add("value", value);
            dbRecord.Tags.Add("pool-name", Settings.PoolName);
            dbRecord.Tags.Add("host", Environment.GetEnvironmentVariable("COMPUTERNAME"));
            dbRecord.Tags.Add("os", Environment.GetEnvironmentVariable("OS"));
            dbRecord.Timestamp = DateTime.UtcNow;
            using (var client = new InfluxClient(new Uri(Settings.DbAddress)))
            {
                var infos = new DynamicInfluxRow[] { dbRecord };
                client.WriteAsync(Settings.DbName, measurement, infos)
                    .GetAwaiter()
                    .GetResult();
            }
        }

        public void WriteMetrics/*<TInfluxRow>*/(string measurement, DynamicInfluxRow props)//, IEnumerable<TInfluxRow> rows) 
            //where TInfluxRow : new()
        {
            using (var client = new InfluxClient(new Uri(Settings.DbAddress)))
            {
                var infos = new DynamicInfluxRow[] { props };
                client.WriteAsync(Settings.DbName, measurement, infos)
                    .GetAwaiter()
                    .GetResult();
            }
        }
    }
}
