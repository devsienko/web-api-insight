using System;
using System.Collections.Generic;
using System.Web.Mvc;
using WebApiMonitor.Administrator.Models;

namespace WebApiMonitor.Administrator.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var applications = new List<Application>
            {
                new Application
                {
                    Id = 1,
                    Name = "Neo Qa",
                    Machines = new List<string> {"aws"},
                    DashboardUrl = "http://localhost:3001/d/23jK1dNmz/mashina?orgId=1&from=now-5m&to=now&theme=light",
                    Added = DateTime.Now
                },
                new Application
                {
                    Id = 2,
                    Name = "Bangor Qa",
                    Machines = new List<string> {"hostway"},
                    DashboardUrl = "http://localhost:3001/d/23jK1dNmz/mashina?orgId=1&from=now-5m&to=now&theme=light",
                    Added = DateTime.Now
                }
            };
            return View(applications);
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