using System.Threading;
using System.Web.Http;

namespace WebApiMonitor.Agent.Rest
{
    public class ConfigurationController : ApiController
    {
        public MetricsConfigContainer Get()
        {
            var result = MetricsConfigManager.GetMetricsConfig();
            return result;
        }

        public bool Post(MetricsConfigContainer newConfig)
        {
            MetricsConfigManager.UpdateMetricsConfig(newConfig);
            var reloadConfigIndicator = Request.Properties["ReloadConfigurationIndicator"] as ReloadConfigurationIndicator;
            if (reloadConfigIndicator != null)
                reloadConfigIndicator.SetFlag();
            return true;
        }
        //todo: use credentials for rest comand executing
    }
}
