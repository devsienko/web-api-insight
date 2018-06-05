using System.Web.Http;

namespace WebApiInsight.Agent.Rest
{
    public class ConfigurationController : ApiController
    {
        public MetricsConfigContainer GetConfiguration()
        {
            var result = MetricsConfigManager.GetMetricsConfig();
            return result;
        }

        public bool Post(Configuration newConfig)
        {
            //MetricsConfigManager.UpdateMetricsConfig(newConfig);
            //todo: force agent read new metrics
            return true;
        }

        //todo: pause agent

        //todo: resume agent

        //todo: use credentials for rest comand executing
    }
}
