using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MCHMIS.Models
{
    public class HouseholdRegOtherProgramme
    {
        [Key, Column(Order = 1)]
        public string HouseholdId { get; set; }

        [Key, Column(Order = 2)]
        public int OtherProgrammeId { get; set; }

        public SystemCodeDetail OtherProgramme { get; set; }
        public HouseholdReg Household { get; set; }
    }
}