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
    }
}