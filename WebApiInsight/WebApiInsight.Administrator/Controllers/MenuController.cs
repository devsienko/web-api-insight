using System.Web.Mvc;

namespace WebApiInsight.Administrator.Controllers
{
    [Authorize]
    public class MenuController : Controller
    {
        public PartialViewResult Index()
        {
            ViewBag.AgentsCount = new AgentsManager().GetAgents().Count;
            return PartialView();
        }
    }
}