using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebApiMonitor.Administrator.Models;

namespace WebApiMonitor.Administrator.Controllers
{
    [Authorize]
    public class AgentsController : Controller
    {
        const string AgentPingResponse = "it's web monitor agent";

        public ActionResult Index()
        {
            var agentManager = new AgentsManager();
            var agents = agentManager.GetAgents();
            return View(agents);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Stop(int id)
        {
            var agentManager = new AgentsManager();
            var agent = agentManager.FindById(id);
            agentManager.ChangeStatus(id, AgentStatus.Stopped);
            RunAgentCommand(string.Format("http://{0}:{1}", agent.IpAddress, agent.Port), "Stop");
            return Json(new
            {
                msg = "Готово"
            });
        }

        [HttpPost]
        public JsonResult Start(int id)
        {
            var agentManager = new AgentsManager();
            var agent = agentManager.FindById(id);
            RunAgentCommand(string.Format("http://{0}:{1}", agent.IpAddress, agent.Port), "Run");
            agentManager.ChangeStatus(id, AgentStatus.Working);
            return Json(new
            {
                msg = "Готово"
            });
        }

        [HttpPost]
        public JsonResult Sync(AgentSettings agent)
        {
            var agentManager = new AgentsManager();
            var dbAgent = agentManager.FindById(agent.Id);
            var deserializer = new JavaScriptSerializer();
            var config = deserializer.Deserialize<MetricsConfigContainer>(agent.JsonConfig);
            SendAgentSettings(config, string.Format("http://{0}:{1}", dbAgent.IpAddress, dbAgent.Port));
            return Json(new
            {
                msg = "Готово"
            });
        }

        [HttpPost]
        public JsonResult Delete(int[] ids)
        {
            try
            {
                var agentManager = new AgentsManager();
                agentManager.RemoveByIds(ids);
                return Json(new
                {
                    msg = "Готово"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    msg = "Непредвиденная ошибка сервера. Пожалуйста попробутей позже."
                });
            }
        }

        [HttpPost]
        public ActionResult Add(AddAgentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Неправильные данные формы создания.");
                return View();
            }
            var agentManager = new AgentsManager();
            var existedAgent = agentManager
                .GetAgents()
                .FirstOrDefault(a => a.IpAddress == model.IpAddress && a.Port == model.Port);
            if (existedAgent != null)
            {
                ModelState.AddModelError("", "Агент уже зарегистрирован.");
                return View();
            }
            var agentAddress = string.Format("http://{0}:{1}", model.IpAddress, model.Port);
            var agentResponse = PingAgent(agentAddress);
            if (agentResponse.Equals(AgentPingResponse))
            {
                //var agentSettings = GetAgentConfig(agentAddress);
                var agent = new AgentSettings
                {
                    Status = AgentStatus.Working,
                    CreationDate = DateTime.UtcNow,
                    IpAddress = model.IpAddress,
                    Port = model.Port
                };
                agentManager.Add(agent);
                return RedirectToAction("Index", "Agents");
            }
            ModelState.AddModelError("", "Не удалось установить соединение с агентом.");
            return View();
        }

        public ActionResult Settings(int id)
        {
            //todo: get more confirmed status from agent direcly, not local database to avoid desynchronization
            var agentManager = new AgentsManager();
            var agent = agentManager.FindById(id);
            if(agent == null)
            {
                ModelState.AddModelError("", string.Format("Агент с Id={0} не найден.", id));
                return View();
            }

            var config = GetAgentConfig(string.Format("http://{0}:{1}/", agent.IpAddress, agent.Port));
            var json = JsonConvert.SerializeObject(config, Formatting.Indented); ;//new JavaScriptSerializer().Serialize(config, Formatting.Indented);
            agent.JsonConfig = json;
            return View(agent);

        }

        private void SendAgentSettings(MetricsConfigContainer config, string agentBaseAddress)
        {
            var SecurityToken = string.Empty;
            using (var client = new HttpClient { BaseAddress = new Uri(agentBaseAddress) })
            {

                if (!string.IsNullOrEmpty(SecurityToken))
                    client.DefaultRequestHeaders.Add("Authorization", SecurityToken);
                var relativeUrl = "api/Configuration/Post";
                var response = client.PostAsJsonAsync(relativeUrl, config).Result;
                EnsureSuccess(response);
                var result = response.Content.ReadAsAsync<bool>().Result;
                if(!result)
                    throw new Exception("Ошибка вызова интерфейса конфигурации агента.");
            }
        }

        public string PingAgent(string agentBaseAddress)
        {
            var SecurityToken = string.Empty;//todo: user token for the api requests
            using (var client = new HttpClient { BaseAddress = new Uri(agentBaseAddress) })
            {
                if (!string.IsNullOrEmpty(SecurityToken))
                    client.DefaultRequestHeaders.Add("Authorization", SecurityToken);
                var relativeUrl = "api/Command/Ping";
                try
                {
                    var response = client.GetAsync(relativeUrl).Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        return string.Empty;
                    }
                    var result = response.Content.ReadAsAsync<string>().Result;
                    return result;
                }
                catch(Exception ex)
                {
                    return string.Empty;
                }
            }
        }

        public string RunAgentCommand(string agentBaseAddress, string command)
        {
            var SecurityToken = string.Empty;//todo: user token for the api requests
            using (var client = new HttpClient { BaseAddress = new Uri(agentBaseAddress) })
            {
                if (!string.IsNullOrEmpty(SecurityToken))
                    client.DefaultRequestHeaders.Add("Authorization", SecurityToken);
                var relativeUrl = "api/Command/" + command;
                var response = client.GetAsync(relativeUrl).Result;
                EnsureSuccess(response);
                var result = response.Content.ReadAsAsync<string>().Result;
                return result;
            }
        }

        public MetricsConfigContainer GetAgentConfig(string agentBaseAddress)
        {
            var SecurityToken = string.Empty;//todo: user token for the api requests
            using (var client = new HttpClient { BaseAddress = new Uri(agentBaseAddress) })
            {
                if (!string.IsNullOrEmpty(SecurityToken))
                    client.DefaultRequestHeaders.Add("Authorization", SecurityToken);
                var relativeUrl = "api/Configuration/Get";
                var response = client.GetAsync(relativeUrl).Result;
                EnsureSuccess(response);
                var result = response.Content.ReadAsAsync<MetricsConfigContainer>().Result;
                return result;
            }
        }
        
        private static void EnsureSuccess(HttpResponseMessage result)
        {
            if (!result.IsSuccessStatusCode)
            {
                throw new Exception(string.Format("Ошибка вызова интерфейса конфигурации агента."));
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