using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCHMIS.Models
{
    public class Complaint : CreateApprovalFields
    {
        public string Id { get; set; }

        [DisplayName("Is Complainant Anonymous?")]
        public int IsComplainantAnonymousId { get; set; }

        public SystemCodeDetail IsComplainantAnonymous { get; set; }

        [DisplayName("Is Complainant Beneficiary?")]
        public int? IsComplainantBeneficiaryId { get; set; }

        public SystemCodeDetail IsComplainantBeneficiary { get; set; }

        [DisplayName("Beneficiary Unique ID")]
        public string UniqueId { get; set; }

        [DisplayName("Category")]
        public int? CategoryId { get; set; }

        public SystemCodeDetail Category { get; set; }

        [DisplayName("Complaint Type")]
        public int? ComplaintTypeId { get; set; }

        public ComplaintType ComplaintType { get; set; }
        public string OtherComplaintType { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}"), DataType(DataType.Text)]
        [DisplayName("Complaint Date")]
        public DateTime? ComplaintDate { get; set; }

        [DisplayName("Name of Complainant")]
        public string Name { get; set; }

        [DisplayName("ID Number")]
        public string IdNumber { get; set; }

        [DisplayName("Phone Number")]
        [StringLength(10, ErrorMessage = "The {0} must be at 10 characters long", MinimumLength = 10)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "The {0} must be numeric")]
        public string Phone { get; set; }

        [DisplayName("Next of Kin Name")]
        public string NOKName { get; set; }

        [DisplayName("Next of Kin Number")]
        [StringLength(10, ErrorMessage = "The {0} must be at 10 characters long", MinimumLength = 10)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "The {0} must be numeric")]
        public string NOKPhone { get; set; }

        [DisplayName("Next of Kin ID")]
        public string NOKIdNumber { get; set; }

        [DisplayName("Facility")]
        public int HealthFacilityId { get; set; }

        public HealthFacility HealthFacility { get; set; }

        [DisplayName("Village")]
        public int VillageId { get; set; }

        public Village Village { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public string ResolvedById { get; set; }
        public ApplicationUser ResolvedBy { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? DateResolved { get; set; }

        public string ClosedById { get; set; }
        public ApplicationUser ClosedBy { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? DateClosed { get; set; }

        /* Escalation Fields */

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? EscalationDate { get; set; }

        public string EscalatedById { get; set; }

        [ForeignKey("EscalatedById")]
        public ApplicationUser EscalatedBy { get; set; }

        /* Escalation Fields */
        public string ActionTaken { get; set; }
        public ICollection<ComplaintNote> ComplaintNotes { get; set; }

        [DisplayName("Status")]
        public int StatusId { get; set; }

        public ComplaintStatus Status { get; set; }

        public string Form { get; set; }
    }

    public class ComplaintStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ComplaintNote : CreateFields
    {
        public string Id { get; set; }
        public string ComplaintId { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public SystemCodeDetail Category { get; set; }
    }

    public class ComplaintType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [DisplayName("Category")]
        public int CategoryId { get; set; }

        public SystemCodeDetail Category { get; set; }

        [DisplayName("SLA Resolution Time (Days)")]
        public int SLAResolutionTime { get; set; }
    }
}