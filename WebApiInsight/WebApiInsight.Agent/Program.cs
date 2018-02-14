using System;
using System.Diagnostics;
using System.Configuration;
using System.Threading;
using InfluxDB.Collector;
using System.Collections.Generic;
using System.Linq;

namespace WebApiInsight.Agent
{
    class Program
    {
        static readonly int ReadingInterval = Settings.ReadingInterval;
        static readonly string PoolName = ConfigurationManager.AppSettings["PoolName"];

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
            InitMetrics();
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
            // ReSharper disable once FunctionNeverReturns
        }

        static void SaveMetrics()
        {
            var iisPoolPid = ProcessHelper.GetIisProcessID(PoolName);
            while (!ProcessHelper.IsPoolAlive(iisPoolPid))
            {
                double zeroUsage = 0;
                WriteMetrics("cpu", zeroUsage);
                WriteMetrics("memory_usage", zeroUsage);
                Thread.Sleep(ReadingInterval * 2);
                iisPoolPid = ProcessHelper.GetIisProcessID(PoolName);
            }
            var instanseName = ProcessHelper.GetInstanceNameForProcessId(iisPoolPid);

            var memoryCounter = new PerformanceCounter("Process", "Working Set - Private", instanseName);
            var cpuCounter = new PerformanceCounter("Process", "% Processor Time", instanseName);

            while (true)
            {
                var memSize = (double)Convert.ToInt32(memoryCounter.NextValue()) / 1024;
                var cpu = cpuCounter.NextValue() / Environment.ProcessorCount;

                // ReSharper disable once SpecifyACultureInStringConversionExplicitly
                WriteMetrics("cpu", cpu);
                WriteMetrics("memory_usage", memSize);

                Thread.Sleep(ReadingInterval);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        static void WriteMetrics(string measurement, object value)
        {
            Metrics.Write(measurement, new Dictionary<string, object> { { "value", value } });
        }

        static void InitMetrics()
        {
            Metrics.Collector = new CollectorConfiguration()
               .Tag.With("pool-name", PoolName)
               .Tag.With("host", Environment.GetEnvironmentVariable("COMPUTERNAME"))
               .Tag.With("os", Environment.GetEnvironmentVariable("OS"))
               .Batch.AtInterval(TimeSpan.FromSeconds(2))
               .WriteTo.InfluxDB(Settings.DbAddress, Settings.DbName)
               .CreateCollector();
        }
    }
}
