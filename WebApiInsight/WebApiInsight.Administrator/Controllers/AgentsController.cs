using System;
using System.Net.Http;
using System.Web.Mvc;

namespace WebApiInsight.Administrator.Controllers
{
    [Authorize]
    public class AgentsController : Controller
    {
        public ActionResult Index()
        {
            Test2("http://localhost:4545/", "11");
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

        public ActionResult Settings(int id)
        {
            id = 11;//todo: use the real number
            ViewBag.AgentId = id;
            return View();
        }

        private void Test(string agentBaseAddress)
        {
            //var SecurityToken = string.Empty;
            //using (var client = new HttpClient { BaseAddress = new Uri(agentBaseAddress) })
            //{

            //    if (!string.IsNullOrEmpty(SecurityToken))
            //        client.DefaultRequestHeaders.Add("Authorization", SecurityToken);

            //    var relativeUrl = "api/Configuration";


            //    var result = client.PostAsJsonAsync(relativeUrl, requestDto).Result;

            //    EnsureSuccess(result);

            //    return result.Content.ReadAsAsync<ApiResponse>().Result;

            //}
        }

        public void Test2(string agentBaseAddress, string agentId)
        {
            var SecurityToken = string.Empty;
            using (var client = new HttpClient { BaseAddress = new Uri(agentBaseAddress) })
            {

                if (!string.IsNullOrEmpty(SecurityToken))
                    client.DefaultRequestHeaders.Add("Authorization", SecurityToken);

                var relativeUrl = "api/Configuration";
                var result = client.GetAsync(relativeUrl).Result;

                EnsureSuccess(result, agentId);

                var conf = result.Content.ReadAsAsync<Configuration>().Result;

            }
        }
        
        private static void EnsureSuccess(HttpResponseMessage result, string agentId)
        {
            if (!result.IsSuccessStatusCode)
            {
                throw new Exception(string.Format("Ошибка вызова интерфейса конфигурации агента: {0}", agentId));
            }
        }
    }

    public class Configuration //todo: to shared assembly
    {
        public string JsonConfig { get; set; }
    }
}