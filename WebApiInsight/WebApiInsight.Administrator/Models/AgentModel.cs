using System;
using System.ComponentModel.DataAnnotations;

namespace WebApiInsight.Administrator.Models
{
    public class AddAgentViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        //[StringLength(15, ErrorMessage = "Поле {0} должно быть длинной {2} символов.", MinimumLength = 15)]
        [Display(Name = "IP-адрес")]
        public string IpAddress { get; set; }

        [Required]
        //[StringLength(4, ErrorMessage = "Поле {0} должно быть длинной {2} символов.", MinimumLength = 4)]
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
        public const string Working = "Запущен";
        public const string Stopped = "Остановлен";
        public const string NotResponding = "Не отвечает";
    }
}
