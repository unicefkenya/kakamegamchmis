using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MCHMIS.Models;
using X.PagedList;

namespace MCHMIS.ViewModels
{
    public class ChangesListViewModel
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

        [DisplayName("Mother Name")]
        public string Name { get; set; }

        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public int AwaitingIPRS { get; set; }
        public int AwaitingMPesaVerification { get; set; }

        public Change Change { get; set; }
        public IPagedList<Change> Changes { get; set; }
        public ICollection<Ward> Wards { get; set; }

        public string Option { get; set; }

        [DisplayName("Report Type")]
        public int? ReportTypeId { get; set; }

        [DisplayName("Change Type")]
        public int? ChangeTypeId { get; set; }

        public int Total { get; set; }
    }
    public class BackupListViewModel
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public DatabaseBackupsList File { get; set; }
        
        public IPagedList<DatabaseBackupsList> Files { get; set; }
    }
        public class CreateChangeViewModel
    {
        public string Id { get; set; }
        public string HouseholdId { get; set; }
        public HouseholdReg Household { get; set; }

        [DisplayName("Change Type"), Required]
        public int? ChangeTypeId { get; set; }

        public SystemCodeDetail ChangeType { get; set; }
        
    

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [DisplayName("Notification Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}"), DataType(DataType.Text)]
        public DateTime? NotificationDate { get; set; }

        [DisplayName("Death Date")]
        public DateTime? DeathDate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}"), DataType(DataType.Text)]
        public DateTime? DateCreated { get; set; }

        [DisplayName("Created By")]
        public string CreatedById { get; set; }

        [DisplayName("Created By")]
        public ApplicationUser CreatedBy { get; set; }

        [DisplayName("Date Actioned")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}"), DataType(DataType.Text)]
        public DateTime? DateActioned { get; set; }

        public string ActionedById { get; set; }
        public ApplicationUser ActionedBy { get; set; }
        public int StatusId { get; set; }
        public ApprovalStatus Status { get; set; }
        public ICollection<ChangeNote> ChangeNotes { get; set; }

        [DisplayName("Attach Supporting Document")]
        public string SupportingDocument { get; set; }

        [DisplayName("Phone Number")]
        [StringLength(10, ErrorMessage = "The {0} must be at 10 characters long", MinimumLength = 10)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "The {0} must be numeric")]
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
        [StringLength(8, ErrorMessage = "The {0} must be at 8 characters long")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "The {0} must be numeric")]
        public string NomineeIdNumber { get; set; }

        [DisplayName("First Name")]
        public string NOKFirstName { get; set; }

        [DisplayName("Middle Name")]
        public string NOKMiddleName { get; set; }

        [DisplayName("Surname")]
        public string NOKSurname { get; set; }

        [DisplayName("Phone Number ")]
        [StringLength(10, ErrorMessage = "The {0} must be at 8 characters long", MinimumLength = 10)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "The {0} must be numeric")]
        public string NOKPhone { get; set; }

        [DisplayName("ID number")]
        [StringLength(8, ErrorMessage = "The {0} must be at 8 characters long")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "The {0} must be numeric")]
        public string NOKIdNumber { get; set; }

        [DisplayName("Child")]
        public string ChildId { get; set; }

        public Child Child { get; set; }

        [DisplayName("Current Health Facility")]
        public int? CurrentHealthFacilityId { get; set; }
        public HealthFacility CurrentHealthFacility { get; set; }
        [DisplayName("Destination Health Facility")]
        public int? DestinationHealthFacilityId { get; set; }
        public HealthFacility DestinationHealthFacility { get; set; }
    }

    public class ChangeDetailsViewModel
    {
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Middle Name")]
        public string MiddleName { get; set; }

        [DisplayName("Surname")]
        public string Surname { get; set; }

        [DisplayName("ID Number")]
        public string IdNumber { get; set; }

        public string Id { get; set; }
        public string HouseholdId { get; set; }
        public HouseholdReg Household { get; set; }
        public Change Change { get; set; }

        public string Notes { get; set; }
        public string Option { get; set; }
    }
}