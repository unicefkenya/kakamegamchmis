using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCHMIS.Models
{
    public class CVList : CreateApprovalFields
    {
        public string Id { get; set; }

        public int Households { get; set; }

        [DisplayName("Validated")]
        public int Captured { get; set; }

        public int StatusId { get; set; }
        public ApprovalStatus Status { get; set; }
        public string Notes { get; set; }
        public string ApprovalNotes { get; set; }
        public string CaptureNotes { get; set; }

        public int? ListTypeId { get; set; }
        public SystemCodeDetail ListType { get; set; }
        [DisplayName("Health Facility")]
        public int? HealthFacilityId { get; set; }
        public HealthFacility HealthFacility { get; set; }
    }

    public class CVListDetail
    {
        public string Id { get; set; }

        public string ListId { get; set; }
        public CVList List { get; set; }

        public string HouseholdId { get; set; }
        public HouseholdReg Household { get; set; }

        public string CVHouseHoldId { get; set; }
        public HouseholdReg CVHouseHold { get; set; }

        public int? StatusId { get; set; }

        public Status Status { get; set; }

        [DisplayFormat(DataFormatString = "{0:##0.##}")]
        public float? Variance { get; set; }

        public int? VarianceCategoryId { get; set; }
        public SystemCodeDetail VarianceCategory { get; set; }

        public int? ApprovalStatusId { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }

        public string ActionedById { get; set; }
        public ApplicationUser ActionedBy { get; set; }
        public DateTime? DateActioned { get; set; }
        [DisplayName("Date Validated")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? DateSubmitedByCHV { get; set; }

        public int? EnumeratorId { get; set; }
        public Enumerator Enumerator { get; set; }
        public string Notes { get; set; }

        [DisplayName("Interview Status")]
        public int? InterviewStatusId { get; set; }

        public SystemCodeDetail InterviewStatus { get; set; }

        [DisplayName("Interview Result")]
        public int? InterviewResultId { get; set; }

        public SystemCodeDetail InterviewResult { get; set; }

        [DisplayName("Reason")]
        public int? CannotFindHouseholdReasonId { get; set; }

        public SystemCodeDetail CannotFindHouseholdReason { get; set; }

        public string ValidationNotes { get; set; }

        public bool? AllowedToEnroll { get; set; }
        public bool AllowedToEnrollDisplay { get
            {
                if(AllowedToEnroll==true)
                    return true;
                return false;
            } }
        public string AllowedReason { get; set; }
    }
}