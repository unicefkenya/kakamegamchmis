using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCHMIS.Models
{
    public class FingerPrintVerification : CreateFields
    {
        [Key, Column(Order = 1)]
        public string Id { get; set; }

        [Key, Column(Order = 2)]
        public string HouseholdId { get; set; }

        public DateTime VisitDate { get; set; }
        public int HealthFacilityId { get; set; }
        public bool Verified { get; set; }
        public bool IsVerifying { get; set; }
        public string TypeId { get; set; }
    }
}