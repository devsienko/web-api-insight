using System.Web.Mvc;

namespace WebApiInsight.Administrator.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //var configPath = Server.MapPath(Url.Content("~/users.json"));
            //var users = new ApplicationUser[] {
            //    new ApplicationUser
            //    {
            //        UserName = "daniil"
            //    }
            //};
            //var json = new JavaScriptSerializer().Serialize(users);
            //System.IO.File.WriteAllText(configPath, json);
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}