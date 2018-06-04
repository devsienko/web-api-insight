using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace WebApiInsight.Agent
{
    public class ProcessCollector : BaseCollector
    {
        public ProcessCollector(ILog logger, IDbManager dbManager)
            : base(logger, dbManager)
        {
        }

        protected override void InitMetricRecords()
        {
            //todo: for the memory - var memSize = (double)Convert.ToInt32(memoryCounter.NextValue()) / 1024;
            //todo: for the cpu - var cpu = cpuCounter.NextValue() / Environment.ProcessorCount;
            MetricsConfig = MetricsConfigManager.ReadMetricsConfig()
                .ProccessMetricsConfig
                .ToList();
        }

        public override void Start()
        {
            Logger.InfoFormat("Started process metrics reading (for the pool {0})", Settings.PoolName);
            while (true)
            {
                try
                {
                    SaveMetrics();
                }
                catch (InvalidOperationException ex)
                {
                    Logger.Error("Presumably go to sleep. Detail: {0}", ex);
                }
                catch (Exception ex)
                {
                    Logger.Error("Error: {0}", ex);
                }
            }
        }

        private void SaveMetrics()
        {
            InstanceName = GetW3pInstanceName();
            var counters = MetricsConfig
                .Select(c => new MeasurementData
                {
                    Measurement = c.Measurement,
                    Counter = new PerformanceCounter
                    {
                        CategoryName = c.CategoryName,
                        CounterName = c.CounterName,
                        InstanceName = InstanceName
                    }
                })
                .ToList();
            try
            {
             
                while (true)
                {
                    counters.ForEach(WriteRecord);
                    Thread.Sleep(Settings.ReadingInterval);
                }
            }
            finally
            {
                counters.ForEach(c =>
                {
                    if(c.Counter != null)
                        c.Counter.Dispose();
                });
            }
        }

        private string GetW3pInstanceName()
        {
            var result = string.Empty;
            var iisPoolPid = ProcessHelper.GetIisProcessID(Settings.PoolName);
            if (!ProcessHelper.IsPoolAlive(iisPoolPid))
                Logger.InfoFormat("Waiting of pool activation. Pool name: {0}", Settings.PoolName);
            //it's necessary to write zero value to influx by Metrics.Write
            //I suppose it's issue of InfluxDB.Collector
            double zeroUsage = 0;
            while (!ProcessHelper.IsPoolAlive(iisPoolPid))
            {
                DbManager.WriteMetricsValue("cpu", zeroUsage);
                DbManager.WriteMetricsValue("memory_usage", zeroUsage);
                Thread.Sleep(Settings.ReadingInterval * 2);
                iisPoolPid = ProcessHelper.GetIisProcessID(Settings.PoolName);
            }
            result = ProcessHelper.GetInstanceNameForProcessId(iisPoolPid);
            Logger.InfoFormat("Pool'{0}' is active.", Settings.PoolName);
            return result;
        }
    }
}
