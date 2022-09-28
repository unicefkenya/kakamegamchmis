using System.ComponentModel.DataAnnotations;

namespace MCHMIS.Models
{
    public class MailSetting
    {
        [Key]
        public string SMTPServerAddress { get; set; }

        public int SMTPServerPort { get; set; }
        public string SMTPServerUserName { get; set; }
        public string SMTPServerPassword { get; set; }
        public string SenderName { get; set; }
        public string SenderEmailAddress { get; set; }
    }
}