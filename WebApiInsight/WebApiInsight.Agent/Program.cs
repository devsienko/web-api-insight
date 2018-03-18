using System;
using System.Diagnostics;
using System.Threading;
using InfluxDB.Collector;
using System.Collections.Generic;

namespace WebApiInsight.Agent
{
    class Program
    {
        //static readonly Dictionary<MetricInfo, MetricSource> MetricsList = new Dictionary<MetricInfo, MetricSource>();
        
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
            //    Console.WriteLine(value);

            //    Thread.Sleep(ReadingInterval);
            //}
            InitStorage();
            Console.WriteLine("Started metrics reading (for the pool {0})", Settings.PoolName);
            while (true)
            {
                try
                {
                    SaveMetrics();
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine("Presumably go to sleep. Detail: {0}", ex);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: {0}", ex);
                }
            }
        }

        static void SaveMetrics()
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

                    WriteMetrics("cpu", cpu);
                    WriteMetrics("memory_usage", memSize);

                    Thread.Sleep(Settings.ReadingInterval);
                }
            }
            finally{
                if(memoryCounter != null)
                    memoryCounter.Dispose();
                if (cpuCounter != null)
                    cpuCounter.Dispose();
            }
        }

        static string GetW3pInstanceName()
        {
            var result = string.Empty;
            using (var pHelper = new ProcessHelper())
            {
                var iisPoolPid = pHelper.GetIisProcessID(Settings.PoolName);
                if (!pHelper.IsPoolAlive(iisPoolPid))
                    Console.WriteLine("Waiting of pool activation. Pool name: {0}", Settings.PoolName);
                //it's necessary to write zero value to influx by Metrics.Write
                //I suppose it's issue of InfluxDB.Collector
                double zeroUsage = 0;
                while (!pHelper.IsPoolAlive(iisPoolPid))
                {
                    WriteMetrics("cpu", zeroUsage);
                    WriteMetrics("memory_usage", zeroUsage);
                    Thread.Sleep(Settings.ReadingInterval * 2);
                    iisPoolPid = pHelper.GetIisProcessID(Settings.PoolName);
                }
                result = pHelper.GetInstanceNameForProcessId(iisPoolPid);
            }
            Console.WriteLine("Pool'{0}' is active.", Settings.PoolName);
            return result;
        }

        static void WriteMetrics(string measurement, object value)
        {
            Metrics.Write(measurement, new Dictionary<string, object> { { "value", value } });
        }

        static void InitStorage()
        {
            Metrics.Collector = new CollectorConfiguration()
               .Tag.With("pool-name", Settings.PoolName)
               .Tag.With("host", Environment.GetEnvironmentVariable("COMPUTERNAME"))
               .Tag.With("os", Environment.GetEnvironmentVariable("OS"))
               .Batch.AtInterval(TimeSpan.FromSeconds(2))
               .WriteTo.InfluxDB(Settings.DbAddress, Settings.DbName)
               .CreateCollector();
            Console.WriteLine("Storage was inited (db address: {0}, db name: {1})", Settings.DbAddress, Settings.DbName);
        }
    }
}
