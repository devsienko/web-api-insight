using System;
using System.Web.Configuration;
using System.Web.Mvc;

namespace WebApiMonitor.Administrator.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.GrafanaUrl = WebConfigurationManager.AppSettings["grafana-url"];

            var dashboardManager = new DashboardManager();
            var items = dashboardManager.GetItems();
            return View(items);
        }

        public ActionResult Edit()
        {
            var dashboardItems = new DashboardManager();
            ViewBag.Agents = dashboardItems.GetItems();
            return View();
        }

        [HttpPost]
        public ActionResult AddItem(DashboardItem item)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Неправильные данные формы создания.");
                return RedirectToAction("Edit");
            }
            var dashboardManager = new DashboardManager();
            dashboardManager.Add(new DashboardItem { Url = item.Url });
            return RedirectToAction("Edit");
        }

        [HttpPost]
        public ActionResult SaveItem(DashboardItem item)
        {
            var dashboardManager = new DashboardManager();
            dashboardManager.UpdateUrl(item.Id, item.Url);
            return RedirectToAction("Edit");
        }

        [HttpPost]
        public ActionResult RemoveItem(DashboardItem item)
        {
            var dashboardManager = new DashboardManager();
            dashboardManager.RemoveByIds(new [] { item.Id });
            return RedirectToAction("Edit");
        }

        public ActionResult About()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            return View();
        }
    }
}