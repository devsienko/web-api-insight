using System.Web.Http;

namespace WebApiInsight.Agent.Rest
{
    public class PingController : ApiController
    {
        [HttpGet]
        public string Ping()
        {
            return "it's web monitor agent";
        }
    }
}