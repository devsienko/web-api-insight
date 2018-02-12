using System.Configuration;

namespace WebApiInsight.Agent
{
    public class Settings
    {
        public static string DbAddress => ConfigurationManager.AppSettings["DbAddress"];
        public static string DbName => ConfigurationManager.AppSettings["DbName"];
        public static int ReadingInterval => int.Parse(ConfigurationManager.AppSettings["ReadingInterval"]);
    }
}
