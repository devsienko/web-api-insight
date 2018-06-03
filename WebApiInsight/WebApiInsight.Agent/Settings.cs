using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Web.Script.Serialization;

namespace WebApiInsight.Agent
{
    public class Settings
    {
        public static readonly string DbAddress = ConfigurationManager.AppSettings["DbAddress"];
        public static readonly string DbName = ConfigurationManager.AppSettings["DbName"];
        public static readonly int ReadingInterval = int.Parse(ConfigurationManager.AppSettings["ReadingInterval"]);
        public static readonly string AppName = ConfigurationManager.AppSettings["AppName"];
        public static readonly string ServerPort = ConfigurationManager.AppSettings["ServerPort"];
        public static readonly string PoolName = ConfigurationManager.AppSettings["PoolName"];//todo: get it programmatically

        private static readonly string ConfigPath = Path.Combine(GetAssemblyLocation(), "MetricsConfig.json");

        public static string GetMetricsConfig()
        {
            var result = File.ReadAllText(ConfigPath);
            return result;
        }

        public static void UpdateMetricsConfig(WebApiInsight.Agent.Rest.Configuration config)
        {
            var json = new JavaScriptSerializer().Serialize(config.JsonConfig);
            File.WriteAllText(ConfigPath, json);
        }

        private static string GetAssemblyLocation()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var codebase = new Uri(assembly.CodeBase);
            var result = Path.GetDirectoryName(codebase.LocalPath);
            return result;
        }

    }
}
