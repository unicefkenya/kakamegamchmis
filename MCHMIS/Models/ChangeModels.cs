using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCHMIS.Models
{
    public class Change
    {
        public string Id { get; set; }

        public string HouseholdId { get; set; }
        public HouseholdReg Household { get; set; }

        [DisplayName("Change Type"), Required]
        public int? ChangeTypeId { get; set; }

        public SystemCodeDetail ChangeType { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [DataType(DataType.MultilineText)]
        [DisplayName("Approval Notes")]
        public string ApprovalNotes { get; set; }

        [DisplayName("Notification Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? NotificationDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [DisplayName("Death Date")]
        public DateTime? DeathDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DateCreated { get; set; }

        [DisplayName("Created By")]
        public string CreatedById { get; set; }

        [DisplayName("Created By")]
        public ApplicationUser CreatedBy { get; set; }

        [DisplayName("Date Actioned")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DateActioned { get; set; }

        [DisplayName("Actioned By")]
        public string ActionedById { get; set; }

        public ApplicationUser ActionedBy { get; set; }

        [DisplayName("Status")]
        public int StatusId { get; set; }

        public ApprovalStatus Status { get; set; }
        public ICollection<ChangeNote> ChangeNotes { get; set; }

        [DisplayName("Attach Supporting Document")]
        public string SupportingDocument { get; set; }

        public string Phone { get; set; }

        [DisplayName("Does the phone belong to the mother?")]
        public bool OwnsPhone { get; set; }

        [Required, DisplayName("Does the phone belong to the mother?")]
        public int OwnsPhoneId { get; set; }

        [DisplayName("First Name")]
        public string NomineeFirstName { get; set; }

        [DisplayName("Nominee Middle Name")]
        public string NomineeMiddleName { get; set; }

        [DisplayName("Nominee Surname")]
        public string NomineeSurname { get; set; }

        [DisplayName("Nominee National ID number")]
        public string NomineeIdNumber { get; set; }

        [DisplayName("First Name")]
        public string NOKFirstName { get; set; }

        [DisplayName("Middle Name")]
        public string NOKMiddleName { get; set; }

        [DisplayName("Surname")]
        public string NOKSurname { get; set; }

        [DisplayName("Phone Number ")]
        [StringLength(10, ErrorMessage = "The {0} must be at 10 characters long", MinimumLength = 10)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "The {0} must be numeric")]
        public string NOKPhone { get; set; }

        [DisplayName("ID number")]
        public string NOKIdNumber { get; set; }

        [DisplayName("Child")]
        public string ChildId { get; set; }

        public Child Child { get; set; }
        public bool FingerPrintVerified { get; set; }
        public bool TakingFingerPrint { get; set; }

        public byte[] FingerPrint { get; set; }
        public byte[] RawFingerPrint { get; set; }

        public bool RequiresIPRSCheck { get; set; }

        [DisplayName("IPRS Verified?")]
        public bool? IPRSVerified { get; set; }

        [DisplayName("IPRS Verification Passed?")]
        public bool? IPRSPassed { get; set; }

        public int? IPRSExceptionId { get; set; }

        // Download Fi
        public bool RequiresMPESACheck { get; set; }

        [DisplayName("Status")]
        public int? MPESACheckStatusId { get; set; }

        public DateTime? DateMPESAVerified { get; set; }
        public Status MPESACheckStatus { get; set; }

        public string CustomerType { get; set; }
        public string CustomerName { get; set; }
        public string RecipientNames { get; set; }
        [DisplayName("Initial Health Facility")]
        public int? CurrentHealthFacilityId { get; set; }
        public HealthFacility CurrentHealthFacility { get; set; }
        [DisplayName("Destination Health Facility")]
        public int? DestinationHealthFacilityId { get; set; }
        public HealthFacility DestinationHealthFacility { get; set; }
    }

    public class ChangeNote
    {
        public int Id { get; set; }
        public string Note { get; set; }
        public string ChangeId { get; set; }
        public string CreatedById { get; set; }
        public ApplicationUser CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
    }
}