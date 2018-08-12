using System;
using System.Collections.Generic;
using System.Web.Mvc;
using WebApiMonitor.Administrator.Models;

namespace WebApiMonitor.Administrator.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        public ActionResult Index()
        {
            var app = new Application
            {
                Id = 1,
                Name = "Neo Qa",
                Machines = new List<string> { "aws" },
                DashboardUrl = "http://localhost:3001/d/23jK1dNmz/mashina?orgId=1&from=now-5m&to=now&theme=light",
                Added = DateTime.Now
            };
            return View(app);
        }
    }
}