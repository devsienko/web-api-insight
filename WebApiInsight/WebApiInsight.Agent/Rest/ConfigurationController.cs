using System.Web.Http;

namespace WebApiInsight.Agent.Rest
{
    public class ConfigurationController : ApiController
    {
        public Configuration GetConfiguration()
        {
            return new Configuration {  };
        }

        public bool Post(Configuration product)
        {
            return true;
        }
    }
}
