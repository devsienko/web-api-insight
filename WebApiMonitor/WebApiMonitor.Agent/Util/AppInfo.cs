using Microsoft.Web.Administration;

namespace WebApiMonitor.Agent
{
    public class AppInfo
    {
        public Site CurrentSite { get; set; }
        public Application CurrentApp { get; set; }
    }
}