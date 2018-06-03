using System.Web.Http;

namespace WebApiInsight.Agent.Rest
{
    public class ConfigurationController : ApiController
    {
        public Configuration GetConfiguration()
        {
            var result = new Configuration();
            result.JsonConfig = Settings.GetMetricsConfig();
            return result;
        }

        public bool Post(Configuration newConfig)
        {
            Settings.UpdateMetricsConfig(newConfig);
            //todo: force agent read new metrics
            return true;
        }

        //todo: pause agent

        //todo: resume agent

        //todo: use credentials for rest comand executing
    }
}
