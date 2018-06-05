using System.Threading;
using System.Web.Http;

namespace WebApiInsight.Agent.Rest
{
    public class CommandController : ApiController
    {
        [HttpGet]
        [ActionName("Run")]
        public string Run()
        {
            var startEvent = Request.Properties["StartStopEvent"] as ManualResetEvent;
            if (startEvent != null)
                startEvent.Set();
            return "Готово";
        }

        [HttpGet]
        [ActionName("Stop")]
        public string Stop()
        {
            var startEvent = Request.Properties["StartStopEvent"] as ManualResetEvent;
            if (startEvent != null)
                startEvent.Reset();
            return "Готово";
        }

        [HttpGet]
        public string Ping()
        {
            return "it's web monitor agent";
        }
    }
}