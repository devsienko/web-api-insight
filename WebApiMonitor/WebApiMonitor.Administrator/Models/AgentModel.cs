using System;
using System.ComponentModel.DataAnnotations;

namespace WebApiMonitor.Administrator.Models
{
    public class AddAgentViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "IP-адрес")]
        public string IpAddress { get; set; }

        [Required]
        [StringLength(4, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [RegularExpression(@"\d{4}", ErrorMessage = "The Port field has incorrect format.")]
        [DataType(DataType.PostalCode)]
        [Display(Name = "Порт")]
        public string Port { get; set; }
    }

    public class AgentSettings
    {
        public int Id { get; set; }
        public string JsonConfig { get; set; }
        public string IpAddress { get; set; }
        public string Port { get; set; }
        public string Status { get; set; }
        public DateTime? CreationDate { get; set; }
    }

    public static class AgentStatus
    {
        public const string Working = "Working";
        public const string Stopped = "Stopped";
        public const string NotResponding = "Not responsable";
    }
}
