using System.Web.Http;

namespace WebApiInsight.Agent.Rest
{
    public class CommandController : ApiController
    {
        [HttpGet]
        [ActionName("Run")]
        public string Run()
        {
            return "Run";
        }

        [HttpGet]
        [ActionName("Stop")]
        public string Stop()
        {
            return "stop";
        }

        [HttpGet]
        public string Ping()
        {
            return "it's web monitor agent";
        }
    }
}