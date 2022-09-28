using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MCHMIS.Models;
using X.PagedList;

namespace MCHMIS.ViewModels
{
    public class EnumeratorViewModel : CreateApprovalFields
    {
        public int Id { get; set; }
        [DisplayName("Enumerator Group")]
        public int EnumeratorGroupId { get; set; }
        public SystemCodeDetail EnumeratorGroup { get; set; }

        [DisplayName("PIN (4 Digits - Leave blank for the password to be auto generated.)")]
        [StringLength(6, ErrorMessage = "The {0} must be at 4 characters long", MinimumLength = 4)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "The {0} must be numeric")]
        public string Password { get; set; }

        public string Email { get; set; }
        [DisplayName("First Name"),Required]
        public string FirstName { get; set; }
        [DisplayName("Middle Name")]
        public string MiddleName { get; set; }
        [Required]
        public string Surname { get; set; }
        [DisplayName("National ID No"), Required]
        public string NationalIdNo { get; set; }
        [DisplayName("Mobile No"), Required]
        [StringLength(10, ErrorMessage = "The {0} must be at 10 characters long", MinimumLength = 10)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "The {0} must be numeric")]
        public string MobileNo { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LoginDate { get; set; }
        public DateTime? ActivityDate { get; set; }
        public int AccessFailedCount { get; set; }
        public bool IsLocked { get; set; }
        public int? DeactivatedById { get; set; }
        public DateTime? DeactivatedOn { get; set; }
        public string FullName => $"{FirstName} {MiddleName} {Surname}";

        [DisplayName("Sub County")]
        public int? SubCountyId { get; set; }

        [DisplayName("Ward")]
        public int? WardId { get; set; }

        [DisplayName("Village"),Required]
        public int? VillageId { get; set; }

        public ICollection<Location> Locations { get; set; }
        public ICollection<SubLocation> SubLocations { get; set; }
        public ICollection<Village> Villages { get; set; }
        public ICollection<Ward> Wards { get; set; }

        [Display(Name = "Reset PIN?")]
        public bool ResetPin { get; set; }
    }

    public class EnumeratorListViewModel
    {

        [DisplayName("Any One Name")]
        public string Name { get; set; }

        [DisplayName("Mobile No.")]
        public string MobileNo { get; set; } 
        [DisplayName("National ID No.")]
        public string NationalIdNo { get; set; }
        public string Email { get; set; }
        public Enumerator Enumerator { get; set; }
        public IPagedList<Enumerator> Enumerators { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}