using System;
using System.Net.Http;
using System.Web.Mvc;
using WebApiInsight.Administrator.Models;

namespace WebApiInsight.Administrator.Controllers
{
    [Authorize]
    public class AgentsController : Controller
    {
        const string AgentPingResponse = "it's web monitor agent";

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(AddAgentViewModel model)
        {
            ModelState.AddModelError("", "test");
            var agentResponse = PingAgent(string.Format("http://{0}:{1}", model.IpAddress, model.Port));
            if (agentResponse.Equals(AgentPingResponse))
                return RedirectToAction("Index", "Agents");
            ModelState.AddModelError("", "Не удалось установить соединение с агентом.");
            return View();
        }

        public ActionResult Settings(int id)
        {
            //ViewBag.AgentConfiguration = GetAgentConfiguration("http://localhost:4545/", id.ToString());
            var model = new AgentSettingsModel
            {
                JsonConfig = @"{""AspNetMetricsConfig"":[{""Measurement"":""req - per - sec"",""CategoryName"":""ASP.NET Applications"",""CounterName"":""Requests / Sec""}],""ProccessMetricsConfig"": [{""Measurement"":""req - per - sec"",""CategoryName"":""ASP.NET Applications"",""CounterName"":""Requests / Sec""}]}",
                Server = "localhost:4545",
                Status = "запущен",
                CreationDate = new DateTime(2018, 06, 06)
            };
            ViewBag.AgentId = id;
            return View(model);
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

        public string PingAgent(string agentBaseAddress)
        {
            var SecurityToken = string.Empty;//todo: user token for the api requests
            using (var client = new HttpClient { BaseAddress = new Uri(agentBaseAddress) })
            {
                if (!string.IsNullOrEmpty(SecurityToken))
                    client.DefaultRequestHeaders.Add("Authorization", SecurityToken);
                var relativeUrl = "api/Ping";
                var response = client.GetAsync(relativeUrl).Result;
                if (!response.IsSuccessStatusCode)
                {
                    return string.Empty;
                }
                var result = response.Content.ReadAsAsync<string>().Result;
                return result;
            }
        }
        
        public MetricsConfigContainer GetAgentConfiguration(string agentBaseAddress, string agentId)
        {
            var SecurityToken = string.Empty;//todo: user token for the api requests
            using (var client = new HttpClient { BaseAddress = new Uri(agentBaseAddress) })
            {
                if (!string.IsNullOrEmpty(SecurityToken))
                    client.DefaultRequestHeaders.Add("Authorization", SecurityToken);
                var relativeUrl = "api/Ping";
                var response = client.GetAsync(relativeUrl).Result;
                EnsureSuccess(response, agentId);
                var result = response.Content.ReadAsAsync<MetricsConfigContainer>().Result;
                return result;
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

    public class MetricsConfigContainer//todo: to shared assembly
    {
        public MetricConfigItem[] AspNetMetricsConfig { get; set; }
        public MetricConfigItem[] ProccessMetricsConfig { get; set; }
    }

    public class MetricConfigItem
    {
        public string Measurement { get; set; }
        public string CategoryName { get; set; }
        public string CounterName { get; set; }
    }
}