using Microsoft.Web.Administration;

namespace WebApiInsight.Agent
{
    public class AppInfo
    {
        public Site CurrentSite { get; set; }
        public Application CurrentApp { get; set; }
    }
}