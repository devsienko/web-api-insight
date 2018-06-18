using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;

namespace WebApiMonitor.Administrator
{
    public class DashboardManager
    {
        private readonly string ConfigPath = Path.Combine(PathHelper.GetAssemblyLocation(), "Conf/dashboard.json");

        public void Add(DashboardItem item)
        {
            var items = GetItems();
            var maxId = 0;
            if(items.Any())
                maxId = items.Max(a => a.Id);
            item.Id = ++maxId;
            items.Add(item);
            var json = new JavaScriptSerializer().Serialize(items);
            File.WriteAllText(ConfigPath, json);

        }

        public void UpdateUrl(int itemId, string newUrl)
        {
            var items = GetItems();
            var item = items.FirstOrDefault(i => i.Id == itemId);
            item.Url = newUrl;
            var json = new JavaScriptSerializer().Serialize(items);
            File.WriteAllText(ConfigPath, json);
        }

        public List<DashboardItem> GetItems()
        {
            var jsonConfig = File.ReadAllText(ConfigPath);
            var deserializer = new JavaScriptSerializer();
            var result = deserializer.Deserialize<DashboardItem[]>(jsonConfig);
            if (result == null)
                return new List<DashboardItem>();
            return result.ToList();
        }

        public void RemoveByIds(int[] ids)
        {
            var items = GetItems();
            var remain = items.Where(a => !ids.Contains(a.Id)).ToArray();
            var json = new JavaScriptSerializer().Serialize(remain);
            File.WriteAllText(ConfigPath, json);
        }
    }

    public class DashboardItem
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Url")]
        public string Url { get; set; }
    }
}
