using System;
using System.Collections.Generic;

namespace WebApiMonitor.Administrator.Models
{
    public class Application
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<string> Machines { get; set; }
        public DateTime Added { get; set; }
        public string DashboardUrl { get; set; }
    }
}