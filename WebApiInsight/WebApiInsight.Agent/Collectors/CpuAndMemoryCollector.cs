using log4net;
using System;
using System.Diagnostics;
using System.Threading;

namespace WebApiInsight.Agent
{
    public class CpuAndMemoryCollector
    {
        readonly ILog _logger;
        readonly IDbManager _dbManager;

        public CpuAndMemoryCollector(ILog logger, IDbManager dbManager)
        {
            _logger = logger;
            _dbManager = dbManager;
        }

        public void Start()
        {
            _logger.InfoFormat("Started metrics reading (for the pool {0})", Settings.PoolName);
            while (true)
            {
                try
                {
                    SaveMetrics();
                }
                catch (InvalidOperationException ex)
                {
                    _logger.Error("Presumably go to sleep. Detail: {0}", ex);
                }
                catch (Exception ex)
                {
                    _logger.Error("Error: {0}", ex);
                }
            }
        }

        private void SaveMetrics()
        {
            PerformanceCounter memoryCounter = null, cpuCounter = null;
            try
            {
                var instanceName = GetW3pInstanceName();
                memoryCounter = new PerformanceCounter("Process", "Working Set - Private", instanceName);
                cpuCounter = new PerformanceCounter("Process", "% Processor Time", instanceName);
                while (true)
                {
                    var memSize = (double)Convert.ToInt32(memoryCounter.NextValue()) / 1024;
                    var cpu = cpuCounter.NextValue() / Environment.ProcessorCount;

                    _dbManager.WriteMetrics("cpu", cpu);
                    _dbManager.WriteMetrics("memory_usage", memSize);

                    Thread.Sleep(Settings.ReadingInterval);
                }
            }
            finally
            {
                if (memoryCounter != null)
                    memoryCounter.Dispose();
                if (cpuCounter != null)
                    cpuCounter.Dispose();
            }
        }

        private string GetW3pInstanceName()
        {
            var result = string.Empty;
            var iisPoolPid = ProcessHelper.GetIisProcessID(Settings.PoolName);
            if (!ProcessHelper.IsPoolAlive(iisPoolPid))
                _logger.InfoFormat("Waiting of pool activation. Pool name: {0}", Settings.PoolName);
            //it's necessary to write zero value to influx by Metrics.Write
            //I suppose it's issue of InfluxDB.Collector
            double zeroUsage = 0;
            while (!ProcessHelper.IsPoolAlive(iisPoolPid))
            {
                _dbManager.WriteMetrics("cpu", zeroUsage);
                _dbManager.WriteMetrics("memory_usage", zeroUsage);
                Thread.Sleep(Settings.ReadingInterval * 2);
                iisPoolPid = ProcessHelper.GetIisProcessID(Settings.PoolName);
            }
            result = ProcessHelper.GetInstanceNameForProcessId(iisPoolPid);
            _logger.InfoFormat("Pool'{0}' is active.", Settings.PoolName);
            return result;
        }
    }
}
