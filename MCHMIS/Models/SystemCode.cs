using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCHMIS.Models
{
    public class SystemCode
    {
        public int Id { get; set; }

        public string Code { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [JsonIgnore]
        public ICollection<SystemCodeDetail> SystemCodeDetails { get; set; }
        [DisplayName("System Module")]
        public int? SystemModuleId { get; set; }
        public SystemModule SystemModule { get; set; }
    }
}