using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCHMIS.Models
{
    public class Beneficiary
    {
        public string Id { get; set; }

        public int? EnrolmentId { get; set; }
        public Enrolment Enrolment { get; set; }

        public string HouseholdId { get; set; }
        public HouseholdReg Household { get; set; }

        [DisplayName("Mother Unique Id")]
        public string UniqueId { get; set; }

        [DisplayName("Beneficiary Name")]
        public string BeneficiaryName { get; set; }

        [DisplayName("ID Number")]
        public string IdNumber { get; set; }

        [DisplayName("ID Document")]
        public int? IdentificationFormId { get; set; }

        public SystemCodeDetail IdentificationForm { get; set; }

        [DisplayName("Recipient Name")]
        public string RecipientName { get; set; }

        [DisplayName("Recipient Phone")]
        public string RecipientPhone { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? EDD { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime DOB { get; set; }

        public int? VillageId { get; set; }
        public Village Village { get; set; }

        public int? CommunityAreaId { get; set; }
        public CommunityArea CommunityArea { get; set; }

        public int? StatusId { get; set; }
        public Status Status { get; set; }

        [DisplayName("Health Facility")]
        public int HealthFacilityId { get; set; }

        public HealthFacility HealthFacility { get; set; }

        [DisplayName("Date Enrolled")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? DateEnrolled { get; set; }

        public string MotherId { get; set; }

        [ForeignKey("MotherId")]
        public HouseholdRegMember Mother { get; set; }
    }
}