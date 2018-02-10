using System;
using System.Diagnostics;
using System.Configuration;
using System.Threading;

namespace WebApiInsight.Agent
{
    class Program
    {
        static readonly int ReadingInterval = int.Parse(ConfigurationManager.AppSettings["Interval"]);

        static void Main()
        {
            var poolName = ConfigurationManager.AppSettings["PoolName"];
            while (true)
            {
                var iisPoolPid = ProcessHelper.GetIisProcessID(poolName);
                if (!ProcessHelper.IsPoolAlive(iisPoolPid))
                {
                    Console.WriteLine("sleeping");
                    Thread.Sleep(ReadingInterval);
                    continue;
                }
                ShowMetrics(iisPoolPid);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private static void ShowMetrics(int pid)
        {
            var instanseName = ProcessHelper.GetInstanceNameForProcessId(pid);
            var cpuCounter = new PerformanceCounter("Process", "% Processor Time", instanseName);
            var memoryCounter = new PerformanceCounter("Process", "Working Set - Private", instanseName);
            try
            {
                while (true)
                {
                    var metrics = new Metrics
                    {
                        Memory = Convert.ToInt32(memoryCounter.NextValue()) / 1024,
                        Cpu = cpuCounter.NextValue() / Environment.ProcessorCount
                    };
                    Console.WriteLine("memory: {0}, cpu: {1}", metrics.Memory, metrics.Cpu);
                    Thread.Sleep(ReadingInterval);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex);
            }
            finally
            {
                memoryCounter.Close();
                cpuCounter.Close();
            }
        }
    }
}
