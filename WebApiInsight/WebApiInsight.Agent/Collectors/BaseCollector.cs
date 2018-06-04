using log4net;
using System.Collections.Generic;

namespace WebApiInsight.Agent
{
    public abstract class BaseCollector
    {
        protected ILog Logger;
        protected IDbManager DbManager;
        protected string InstanceName; 
        protected List<MetricConfigItem> MetricsConfig = new List<MetricConfigItem>();

        public BaseCollector()
        {

        }

        public BaseCollector(ILog logger, IDbManager dbManager)
        {
            Logger = logger;
            DbManager = dbManager;

            InitMetricRecords();
        }

        protected abstract void InitMetricRecords();

        public abstract void Start();

        protected MetricConfigItem CreateMetricRecord(string measurement, string categoryName, string counterName)
        {
            var result = new MetricConfigItem
            {
                Measurement = measurement,
                CategoryName = categoryName,
                CounterName = counterName
            };
            return result;
        }

        protected void WriteRecord(MeasurementData md)
        {
            var value = md.Counter.NextValue();
            DbManager.WriteMetricsValue(md.Measurement, value);
        }
    }
}
