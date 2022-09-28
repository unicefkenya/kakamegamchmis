using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MCHMIS.Models;
using X.PagedList;

namespace MCHMIS.ViewModels
{
    public class ComplaintListViewModel
    {
        [DisplayName("Status")]
        public int? StatusId { get; set; }

        [DisplayName("Health Facility")]
        public int? HealthFacilityId { get; set; }

        [DisplayName("Sub-County")]
        public int? SubCountyId { get; set; }

        [DisplayName("Ward")]
        public int? WardId { get; set; }

        [DisplayName("Mother's Unique Id")]
        public string UniqueId { get; set; }

        [DisplayName("Identification Document No")]
        public string IdNumber { get; set; }

        [DisplayName("Any One Name")]
        public string Name { get; set; }

        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public int Total { get; set; }
        public Complaint Complaint { get; set; }
        public IPagedList<Complaint> Complaints { get; set; }
        public ICollection<Ward> Wards { get; set; }
    }

    public class ComplaintDetailsViewModel
    {
        public string Id { get; set; }

        [Required]
        public string Notes { get; set; }

        public string Action { get; set; }
        public ICollection<ComplaintNote> ComplaintNotes { get; set; }
        public ComplaintNote ComplaintNote { get; set; }
        public Complaint Complaint { get; set; }
    }

    public class ComplaintViewModels : CreateApprovalFields
    {
        public string Id { get; set; }
        public int? ComplaintTypeId { get; set; }
        public string OtherComplaintType { get; set; }

        [DisplayName("Is Complainant Anonymous")]
        public int IsComplainantAnonymousId { get; set; }

        public SystemCodeDetail IsComplainantAnonymous { get; set; }

        [DisplayName("Is Complainant Beneficiary")]
        public int? IsComplainantBeneficiaryId { get; set; }

        public SystemCodeDetail IsComplainantBeneficiary { get; set; }

        [DisplayName("Beneficiary Unique ID")]
        public string UniqueId { get; set; }

        [DisplayName("Complaint Category"), Required]
        public int? CategoryId { get; set; }

        public SystemCodeDetail Category { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}"), DataType(DataType.Text)]
        [DisplayName("Complaint Date")]
        public DateTime? ComplaintDate { get; set; }

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

        [DisplayName("Ward")]
        public int? WardId { get; set; }

        public Ward Ward { get; set; }

        [DisplayName("Village")]
        public int VillageId { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DisplayName("Upload the complaint form")]
        public string Form { get; set; }

        public string ResolvedById { get; set; }
        public ApplicationUser ResolvedBy { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? DateResolved { get; set; }

        public string ClosedById { get; set; }
        public ApplicationUser ClosedBy { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? DateClosed { get; set; }

        public string ActionTaken { get; set; }
        public ICollection<ComplaintNote> ComplaintNotes { get; set; }
        public int StatusId { get; set; }
        public ComplaintStatus Status { get; set; }

        public IEnumerable<ComplaintType> ComplaintTypes { get; set; }
        public IEnumerable<Village> Villages { get; set; }
    }

    public class ComplaintsViewModels
    {
        [DisplayName("CategoryId")]
        public int? CategoryId { get; set; }

        [DisplayName("Complaint-Type")]
        public int? ComplaintTypeId { get; set; }

        [DisplayName("Sub-County")]
        public int? SubCountyId { get; set; }

        [DisplayName("Ward")]
        public int? WardId { get; set; }

        [DisplayName("Heath-Facility")]
        public int? HealthFacilityID { get; set; }

        public string ActionTaken { get; set; }
        public ICollection<ComplaintNote> ComplaintNotes { get; set; }
        public ICollection<ComplaintStatus> Status { get; set; }
        public ICollection<Ward> Wards { get; set; }
        public ICollection<HealthFacility> HealthFacility { get; set; }
        public ICollection<ComplaintType> ComplaintTypes { get; set; }
    }
}