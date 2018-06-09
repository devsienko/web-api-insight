using log4net;
using System;
using System.Collections.Generic;
using System.Threading;

namespace WebApiMonitor.Agent
{
    public abstract class BaseCollector
    {
        protected ManualResetEvent PauseOrStartEvent;
        protected ILog Logger;
        protected IDbManager DbManager;
        protected string InstanceName; 
        protected List<MetricConfigItem> MetricsConfig = new List<MetricConfigItem>();

        public BaseCollector()
        {

        }

        public BaseCollector(ILog logger, IDbManager dbManager, ManualResetEvent pauseOrStartEvent)
        {
            Logger = logger;
            DbManager = dbManager;
            PauseOrStartEvent = pauseOrStartEvent;

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
            if (md.Measurement == "cpu")
                value = value / Environment.ProcessorCount;
            DbManager.WriteMetricsValue(md.Measurement, value);
        }
    }
}
