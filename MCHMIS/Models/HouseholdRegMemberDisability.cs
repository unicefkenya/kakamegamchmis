using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCHMIS.Models
{
    public class HouseholdRegMemberDisability
    {
        [Key, Column(Order = 1)]
        public string HouseholdRegMemberId { get; set; }

        [Key, Column(Order = 2)]
        public int DisabilityId { get; set; }

        public SystemCodeDetail Disability { get; set; }
        public HouseholdRegMember HouseholdRegMember { get; set; }
    }
}