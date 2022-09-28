using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCHAPP
{
    public class HouseholdReg
    {
        public string Id { get; set; }
        public byte[] FingerPrint { get; set; }
        public byte[] RawFingerPrint { get; set; }
        public bool VerifyingFingerPrint { get; set; }
        public string MotherId { get; set; }
        [DisplayName("Health Facility")] public int HealthFacilityId { get; set; }
        [ForeignKey("MotherId")] public HouseholdRegMember Mother { get; set; }
        public string CreatedById { get; set; }
    }

    public class HouseholdRegMember
    {
        public string Id { get; set; }

        public string HouseholdId { get; set; }

        [DisplayName("First Name")] public string FirstName { get; set; }

        [DisplayName("Middle Name")] public string MiddleName { get; set; }

        public string Surname { get; set; }

        [DisplayName("Identification Document")]
        public int? IdentificationFormId { get; set; }

        public string FullName => FirstName + " " + MiddleName + " " + Surname;
    }

    public class FingerPrintVerification
    {
        [Key, Column(Order = 1)] public string Id { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}"), DataType(DataType.Date)]
        [Key, Column(Order = 2)]
        public string HouseholdId { get; set; }

        public DateTime VisitDate { get; set; }
        public int HealthFacilityId { get; set; }
        public bool Verified { get; set; }
        public bool IsVerifying { get; set; }
        public string CreatedById { get; set; }
    }

    public class MotherClinicVisit
    {
        public string Id { get; set; }
        public string HouseholdId { get; set; }
        public string CaseManagementId { get; set; }
        public int TypeId { get; set; }
        public int HealthFacilityId { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime VisitDate { get; set; }

        [DisplayName("Requires Fingerprint Verification?")]
        public bool RequiresFingerPrint { get; set; }

        [DisplayName("Verified?")] public bool Verified { get; set; }
        public bool IsVerifying { get; set; }
    }

    public class ApplicationUser
    {
        public virtual DateTimeOffset? LockoutEnd { get; set; }

        public virtual bool TwoFactorEnabled { get; set; }

        public virtual bool PhoneNumberConfirmed { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual string ConcurrencyStamp { get; set; }

        public virtual string SecurityStamp { get; set; }

        public virtual string PasswordHash { get; set; }
        public virtual bool EmailConfirmed { get; set; }
        public virtual string NormalizedEmail { get; set; }
        public virtual string Email { get; set; }
        public string NormalizedUserName { get; set; }
        public string UserName { get; set; }
        public string Id { get; set; }
        public virtual bool LockoutEnabled { get; set; }
        public virtual int AccessFailedCount { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
        public DateTime? LastPasswordChangedDate { get; set; }
        public DateTime? LastActivityDate { get; set; }

        public string FullName => FirstName + " " + MiddleName + " " + Surname;

        public string Name => FirstName + " " + Surname;

        public string DisplayName => FirstName + " " + Surname;

        [DisplayName("Health Facility")] public int? HealthFacilityId { get; set; }
        public HealthFacility HealthFacility { get; set; }

        [DisplayName("Date Created"), DataType(DataType.Text)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime DateCreated { get; set; }

        [DisplayName("Created By")] public string CreatedById { get; set; }

        [DisplayName("Created By")] public ApplicationUser CreatedBy { get; set; }

        public string MacAddress { get; set; }
        public bool IsLoggedIn { get; set; }
    }

    public class HealthFacility
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [DisplayName("Sub County")]
        public int? SubCountyId { get; set; }

        public SubCounty SubCounty { get; set; }
    }

    public class Change
    {
        public string Id { get; set; }

        public string HouseholdId { get; set; }
        public HouseholdReg Household { get; set; }

        public int? ChangeTypeId { get; set; }

        [DisplayName("Created By")]
        public string CreatedById { get; set; }

        public bool TakingFingerPrint { get; set; }

        public bool FingerPrintVerified { get; set; }

        public byte[] FingerPrint { get; set; }

        public byte[] RawFingerPrint { get; set; }
    }
}