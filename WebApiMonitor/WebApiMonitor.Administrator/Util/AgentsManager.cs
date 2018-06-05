using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using WebApiMonitor.Administrator.Models;

namespace WebApiMonitor.Administrator
{
    public class AgentsManager
    {
        private readonly string ConfigPath = Path.Combine(PathHelper.GetAssemblyLocation(), "Conf/agents.json");

        public void Add(AgentSettings agent)
        {
            var agents = GetAgents();
            var maxId = 0;
            if(agents.Any())
                maxId = agents.Max(a => a.Id);
            agent.Id = ++maxId;
            agents.Add(agent);
            var json = new JavaScriptSerializer().Serialize(agents);
            File.WriteAllText(ConfigPath, json);

        }

        public AgentSettings FindById(int agentId)
        {
            var agents = GetAgents();
            var result = agents.FirstOrDefault(a => a.Id == agentId);
            return result;
        }

        public void ChangeStatus(int agentId, string status)
        {
            var agents = GetAgents();
            var agent = agents.FirstOrDefault(a => a.Id == agentId);
            agent.Status = status;
            var json = new JavaScriptSerializer().Serialize(agents);
            File.WriteAllText(ConfigPath, json);
        }

        public List<AgentSettings> GetAgents()
        {
            var jsonConfig = File.ReadAllText(ConfigPath);
            var deserializer = new JavaScriptSerializer();
            var result = deserializer.Deserialize<AgentSettings[]>(jsonConfig);
            if (result == null)
                return new List<AgentSettings>();
            return result.ToList();
        }

        public void RemoveByIds(int[] ids)
        {
            var agents = GetAgents();
            var remain = agents.Where(a => !ids.Contains(a.Id)).ToArray();
            var json = new JavaScriptSerializer().Serialize(remain);
            File.WriteAllText(ConfigPath, json);
        }
    }
}
