using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCHMIS.Models
{
    public class PMTScore
    {
        [Key]
        public string HouseholdId { get; set; }

        [ForeignKey("HouseholdId")]
        public HouseholdReg Household { get; set; }

        public int ConstantId { get; set; }

        public float Score { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public SystemCodeDetail Locality { get; set; }
        public SystemCodeDetail Constant { get; set; }

        [ForeignKey("CreatedBy")]
        public ApplicationUser CreatedByUser { get; set; }
    }
}