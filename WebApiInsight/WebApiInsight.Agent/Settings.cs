using System.Configuration;

namespace WebApiInsight.Agent
{
    public class Settings
    {
        public static readonly string DbAddress = ConfigurationManager.AppSettings["DbAddress"];
        public static readonly string DbName = ConfigurationManager.AppSettings["DbName"];
        public static readonly int ReadingInterval = int.Parse(ConfigurationManager.AppSettings["ReadingInterval"]);
        public static readonly string PoolName = ConfigurationManager.AppSettings["PoolName"];
    }
}
