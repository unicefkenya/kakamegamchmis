using System.ComponentModel.DataAnnotations;

namespace MCHMIS.Models
{
    public class SystemSetting
    {
        public int Id { get; set; }

        [Required]
        public string key { get; set; }

        [DataType(DataType.MultilineText), Required]
        public string Value { get; set; }
    }
}