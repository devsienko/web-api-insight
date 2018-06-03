using System.Web.Mvc;

namespace WebApiInsight.Administrator.Controllers
{
    [Authorize]
    public class AgentsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Settings(int id)
        {
            id = 11;//todo: use the real number
            ViewBag.AgentId = id;
            return View();
        }
    }
}