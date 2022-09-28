using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MCHMIS.Models;

namespace MCHMIS.ViewModels
{
    public class RegistrationViewModel
    {
        public string Id { get; set; }

        public byte ProgrammeId { get; set; }

        [DisplayName("County")] public int? CountyId { get; set; }

        [DisplayName("Other County")] public int? OtherCountyId { get; set; }

        [DisplayName("Sub-County")] public int? SubCountyId { get; set; }

        [DisplayName("Constituency")] public int? ConstituencyId { get; set; }

        [DisplayName("Location")] public int? LocationId { get; set; }

        [DisplayName("Sub-Location")] public int? SubLocationId { get; set; }

        [DisplayName("Village")] public int? VillageId { get; set; }

        [DisplayName("Village")] public string Village { get; set; }

        [DisplayName("Community Area")] public int? CommunityAreaId { get; set; }

        [DisplayName("Physical Address")] public string PhysicalAddress { get; set; }

        [DisplayName("Nearest church / mosque")]
        public string NearestReligiousBuilding { get; set; }

        [DisplayName("Nearest school")] public string NearestSchool { get; set; }

        [DisplayName("Duration of residence in Kakamega County: Years")]
        public int? ResidenceDurationYears { get; set; }

        [DisplayName("Months")] public int? ResidenceDurationMonths { get; set; }

        public int StatusId { get; set; }

        public DateTime? DwellingStartDate { get; set; }

        public DateTime CaptureStartDate { get; set; }

        public DateTime CaptureEndDate { get; set; }

        public float? Longitude { get; set; }

        public float? Latitude { get; set; }

        public float? Elevation { get; set; }

        public int RegGroupId { get; set; }

        public int HabitableRooms { get; set; }

        public string SerialNo { get; set; }

        public string DeviceId { get; set; }

        public string VersionId { get; set; }

        public DateTime? SyncDate { get; set; }
        public DateTime DateCreated { get; set; }

        public ApplicationUser CreatedBy { get; set; }

        public string CreatedById { get; set; }
        public string UniqueId { get; set; }
        public string MotherId { get; set; }

        public int TypeId { get; set; }

        public DateTime CreatedOn { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        /*Mothers Detail*/

        [DisplayName("First Name")] public string FirstName { get; set; }

        [DisplayName("Middle Name")] public string MiddleName { get; set; }

        [DisplayName("Surname")] public string Surname { get; set; }

        [DisplayName("Mother's Common Name")] public string CommonName { get; set; }

        [DisplayName("Is Mother Over 18 Years Old?")]
        public bool Over18 { get; set; }

        [DisplayName("Form of Identification")]
        public int IdentificationFormId { get; set; }

        [DisplayName("National ID Card Number")]
        public string IdNumber { get; set; }

        public int GenderId { get; set; }

        [DisplayName("DOB")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? DOB { get; set; }

        [DisplayName("Marital Status")] public int MaritalStatusId { get; set; }

        [DisplayName("NHIF Number")] public string NHIFNo { get; set; }

        [DisplayName("Phone Number")]
        [StringLength(10, ErrorMessage = "The {0} must be at 10 characters long", MinimumLength = 10)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "The {0} must be numeric")]
        // [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Phone can only have letters and numbers!")]
        public string Phone { get; set; }

        [DisplayName("Does the phone belong to the mother?")]
        public bool OwnsPhone { get; set; }

        [Required, DisplayName("Does the phone belong to the mother?")]
        public int? OwnsPhoneId { get; set; }

        [DisplayName("Nominee First Name (As registered with MPESA)")]
        public string NomineeFirstName { get; set; }

        [DisplayName("Nominee Middle Name")] public string NomineeMiddleName { get; set; }

        [DisplayName("Nominee Surname")] public string NomineeSurname { get; set; }

        [DisplayName("Nominee National ID number")]
        [StringLength(8, ErrorMessage = "The {0} must be at 8 characters long")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "The {0} must be numeric")]
        public string NomineeIdNumber { get; set; }

        [DisplayName("Recipient Names (If the Phone Number Does Not Belong to the Mother)")]
        public string RecipientNames { get; set; }

        [DisplayName("Next of Kin First Name (As recorded in the Health Handbook)")]
        public string NOKFirstName { get; set; }

        [DisplayName("Next of Kin Middle Name")]
        public string NOKMiddleName { get; set; }

        [DisplayName("Next of Kin Surname")] public string NOKSurname { get; set; }

        [DisplayName("Next of Kin Phone Number ")]
        [StringLength(10, ErrorMessage = "The {0} must be at 8 characters long", MinimumLength = 10)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "The {0} must be numeric")]
        public string NOKPhone { get; set; }

        [DisplayName("Next of Kin National ID number")]
        [StringLength(8, ErrorMessage = "The {0} must be at 8 characters long")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "The {0} must be numeric")]
        public string NOKIdNumber { get; set; }

        [DisplayName("Ward")] public int? WardId { get; set; }

        [DisplayName("Health Facility")] public int HealthFacilityId { get; set; }

        public string Institution { get; set; }

        [DisplayName("Upload Mother's Photo")] public string PhotoUrl { get; set; }

        [DisplayName("Parity")] public string Para { get; set; }

        public int? Gravida { get; set; }

        public string Parity { get; set; }

        [DisplayName("LMP -  Last Menstrual Period")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? LMP { get; set; }

        [DisplayName("LMP Unknown")] public bool LMPUnknown { get; set; }

        [DisplayName("Estimated months of pregnancy")]
        public int? EPM { get; set; }

        [DisplayName("EDD -  Expected Date of Delivery")]
        [DataType(DataType.Text)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? EDD { get; set; }

        [Required(ErrorMessage = "* Required")]
        public int HasBeenInMCHProgramId { get; set; }

        public string SupportingDocument { get; set; }
        public byte[] FingerPrint { get; set; }
        public bool? RequiresHealthVerification { get; set; }

        [DisplayName("Take Fingerprint")] public bool VerifyingFingerPrint { get; set; }

        public string DisplayName => FirstName + " " + MiddleName + " " + Surname;
        /*Mothers Detail*/

        public SystemCodeDetail RegGroup { get; set; }
        public SystemCodeDetail Status { get; set; }
        public Programme Programme { get; set; }
        public SubLocation SubLocation { get; set; }
        public HouseholdRegCharacteristic HouseholdRegCharacteristic { get; set; }
        public ICollection<HouseholdRegAsset> HouseholdRegAssets { get; set; }
        public ICollection<HouseholdRegMember> HouseholdRegMembers { get; set; }
        public ICollection<HouseholdRegOtherProgramme> HouseholdRegOtherProgrammes { get; set; }

        public ICollection<Location> Locations { get; set; }
        public ICollection<SubLocation> SubLocations { get; set; }
        public ICollection<Village> Villages { get; set; }
        public ICollection<Ward> Wards { get; set; }
        public ICollection<CommunityArea> CommunityAreas { get; set; }

        public string MinimumAge { get; set; }

        public bool RequiresIPRSECheck { get; set; }
        public bool IPRSVerified { get; set; }
        public bool? IPRSPassed { get; set; }
    }

    public class HouseholdRegMemberViewModel
    {
        public string Id { get; set; }

        public string HouseholdId { get; set; }

        [DisplayName("First Name")] public string FirstName { get; set; }

        [DisplayName("Middle Name")] public string MiddleName { get; set; }

        public string Surname { get; set; }

        [DisplayName("Identification Document Number")]
        public string IdNumber { get; set; }

        [DisplayName("Identification Document")]
        public int? IdentificationFormId { get; set; }

        [DisplayName("Relationship to the head of this household")]
        public int RelationshipId { get; set; }

        [DisplayName("Gender")] public int GenderId { get; set; }

        [DisplayName("Date of Birth")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? DOB { get; set; }

        [DisplayName("Marital Status")] public int MaritalStatusId { get; set; }

        [DisplayName("Does Spouse live in this household?")]
        public int? SpouseInHouseholdId { get; set; }

        public SystemCodeDetail SpouseInHousehold { get; set; }

        [DisplayName("Is father of the member alive?")]
        public int FatherAliveId { get; set; }

        [DisplayName("Is mother of the member alive?")]
        public int MotherAliveId { get; set; }

        [DisplayName("Suffers from chronic illness")]
        public int ChronicIllnessId { get; set; }

        public SystemCodeDetail ChronicIllness { get; set; }

        [DisplayName("Disability Type")] public int? DisabilityTypeId { get; set; }

        [DisplayName("Does Disability require 24-hour care?")]
        public int? DisabilityRequires24HrCareId { get; set; }

        [DisplayName("HIV Status")] public string SupportStatusId { get; set; }

        [DisplayName("MUAC")]
        [RegularExpression(@"^\d+.\d{0,2}$", ErrorMessage = "MUAC should be a decimal to 2 decimal places")]
        public decimal? MUAC { get; set; }

        public SystemCodeDetail SupportStatus { get; set; }

        [DisplayName("Main Care Giver")] public string DisabilityCaregiverId { get; set; }

        public int? MainCaregiverId { get; set; }

        [DisplayName("Learning Institution attendance status")]
        public int EducationAttendanceId { get; set; }

        [DisplayName("Highest Std/Form/Level reached by the member?")]
        public int? EducationLevelId { get; set; }

        [DisplayName("Is the school Public or Private?")]
        public int? SchoolTypeId { get; set; }

        [DisplayName("What was the member doing during the last seven days?")]
        public int OccupationTypeId { get; set; }

        [DisplayName("Does member work on a formal job, teaching, public sector, NGO/FBO?")]
        public int FormalJobTypeId { get; set; }

        public bool IsMother { get; set; }
        public DateTime CreatedOn { get; set; }

        public List<int> DisabilityTypes { get; set; }

        public HouseholdReg Household { get; set; }
        public SystemCodeDetail IdentificationType { get; set; }
        public SystemCodeDetail Relationship { get; set; }
        public SystemCodeDetail Sex { get; set; }
        public SystemCodeDetail MaritalStatus { get; set; }
        public SystemCodeDetail OrphanhoodType { get; set; }
        public SystemCodeDetail FatherAlive { get; set; }
        public SystemCodeDetail MotherAlive { get; set; }
        public SystemCodeDetail IllnessType { get; set; }
        public SystemCodeDetail DisabilityType { get; set; }
        public HouseholdRegMember DisabilityCaregiver { get; set; }
        public SystemCodeDetail DisabilityRequires24HrCare { get; set; }

        public SystemCodeDetail EducationAttendance { get; set; }
        public SystemCodeDetail EducationLevel { get; set; }
        public SystemCodeDetail OccupationType { get; set; }
        public SystemCodeDetail FormalJobType { get; set; }
    }

    public class BlacklistViewModel
    {
        public string Id { get; set; }
        public string Notes { get; set; }
    }
}