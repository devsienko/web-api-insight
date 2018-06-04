using System;
using System.IO;
using System.Reflection;
using System.Web.Script.Serialization;

namespace WebApiInsight.Agent
{
    public class MetricsConfigManager
    {
        private static readonly string ConfigPath = Path.Combine(GetAssemblyLocation(), "MetricsConfig.json");

        public static string GetMetricsConfig()
        {
            var result = File.ReadAllText(ConfigPath);
            return result;
        }

        public static void UpdateMetricsConfig(MetricsConfigContainer config)
        {  
            var json = new JavaScriptSerializer().Serialize(config);
            File.WriteAllText(ConfigPath, json);
        }

        public static MetricsConfigContainer ReadMetricsConfig()
        {
            var json = File.ReadAllText(ConfigPath);
            var result = new JavaScriptSerializer().Deserialize<MetricsConfigContainer>(json);
            return result;
        }

        private static string GetAssemblyLocation()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var codebase = new Uri(assembly.CodeBase);
            var result = Path.GetDirectoryName(codebase.LocalPath);
            return result;
        }
    }

    public class MetricsConfigContainer
    {
        public MetricConfigItem[] AspNetMetricsConfig { get; set; }
        public MetricConfigItem[] ProccessMetricsConfig { get; set; }
    }
}
