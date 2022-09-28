using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCHMIS.Models
{
    public class SystemCodeDetail
    {
        public int Id { get; set; }
        [DisplayName("System Code")]
        public int SystemCodeId { get; set; }

        public string Code { get; set; }

       

        [DisplayFormat(DataFormatString = "{0:0.##}")]
        [Required]
        public decimal? OrderNo { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
       // [JsonIgnore]
        public SystemCode SystemCode { get; set; }

        public string OrderNoNumber
        {
            get
            {
                return ((decimal)OrderNo).ToString("0");
             
            }
        }
        public string DisplayName
        {
            get
            {
                if (OrderNo != null)
                    return ((decimal)OrderNo).ToString("0.##") + ". " + Code;
                return "";
            }
        }
    }
}