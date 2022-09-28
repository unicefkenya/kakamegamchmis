using MCHMIS.Api.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace MCHMIS.Api.Models
{
    public class TabletHouseholdViewModel
    {
        [Required]
        public string HouseholdInfo { get; set; }

        [Required]
        public string DeviceInfo { get; set; }
    }

    public class RegistrationHHVm
    {
        public int Id { get; set; }
        public string ParentId { get; set; }
        public int? VillageId { get; set; }
        public string UniqueId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string RegistrationDate { get; set; }
        public int WardId { get; set; }

        public int? SubLocationId { get; set; }
        public int ResidenceDurationYears { get; set; }
        public int ResidenceDurationMonths { get; set; }
        public int Years { get; set; }
        public int Months { get; set; }
        public int EnumeratorId { get; set; }
        public string Phone { get; set; }
        public string MotherId { get; set; }
        public int HouseholdMembers { get; set; }
        public int StatusId { get; set; }

        public string Village { get; set; }
        public string PhysicalAddress { get; set; }
        public string NearestReligiousBuilding { get; set; }
        public string NearestSchool { get; set; }
        public float? Longitude { get; set; }
        public float? Latitude { get; set; }
        public int? SyncEnumeratorId { get; set; }
        public string MiddleName { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string NOKMiddleName { get; set; }
        public string NOKFirstName { get; set; }
        public string NOKSurname { get; set; }
        public string NOKPhone { get; set; }
        public string DateOfBirth { get; set; }
        public int GenderId { get; set; }
        public int IdentificationFormId { get; set; }
        public string IdNumber { get; set; }
        public string MothersFullName { get; set; }
        public string NOKsFullName { get; set; }
        public int IsTelevisionId { get; set; }

        public int IsMotorcycleId { get; set; }
        public int IsTukTukId { get; set; }
        public int IsRefrigeratorId { get; set; }
        public int IsCarId { get; set; }
        public int IsMobilePhoneId { get; set; }
        public int IsBicycleId { get; set; }

        public int ExoticCattle { get; set; }
        public int IndigenousCattle { get; set; }
        public int Sheep { get; set; }
        public int Goats { get; set; }
        public int Camels { get; set; }
        public int Donkeys { get; set; }
        public int Pigs { get; set; }
        public int Chicken { get; set; }
        public int LiveBirths { get; set; }
        public int Deaths { get; set; }

        public int IsSkippedMealId { get; set; }
        public int IsReceivingSocialId { get; set; }
        public int IsReceivingFamilyId { get; set; }
        public string Programmes { get; set; }
        public decimal LastReceiptAmount { get; set; }
        public string InKindBenefitId { get; set; }
        public int WasteDisposalModeId { get; set; }
        public int WaterSourceId { get; set; }
        public int WallConstructionMaterialId { get; set; }
        public int IsOwnedId { get; set; }
        public int TenureStatusId { get; set; }
        public int RoofConstructionMaterialId { get; set; }
        public string OtherProgrammeNames { get; set; }
        public int NsnpProgrammesId { get; set; }
        public int OtherProgrammesId { get; set; }
        public int LightingFuelTypeId { get; set; }
        public string IsMonetary { get; set; }
        public int HabitableRooms { get; set; }
        public int HouseHoldConditionId { get; set; }
        public int FloorConstructionMaterialId { get; set; }
        public int DwellingUnitRiskId { get; set; }
        public int CookingFuelTypeId { get; set; }
        public int BenefitTypeId { get; set; }
        public int FamilyBenefitTypeId { get; set; }
        public bool? HasBeenInImarishaAfyaId { get; set; }
        public int? BenefitFromFriendsRelativeId { get; set; }
        public int? InterviewStatusId { get; set; }
        public int InterviewResultId { get; set; }

        public string DownloadDate { get; set; }
        public string RegDate1 { get; set; }
        public string RegDate2 { get; set; }
        public string RegDate3 { get; set; }
        public List<RegistrationMember> RegistrationMembers { get; set; }

        public List<RegistrationMemberDisability> RegistrationMemberDisabilities { get; set; }

        public List<RegistrationProgramme> RegistrationProgrammes { get; set; }

        public int? CannotFindHouseholdReasonId { get; set; }
        public SystemCodeDetail CannotFindHouseholdReason { get; set; }


    }

    public class RegistrationMemberDisability
    {
        public int Id { get; set; }
        public string RegistrationMemberId { get; set; }
        public int DisabilityId { get; set; }

        public int RegistrationId { get; set; }
    }

    public class RegistrationProgramme
    {
        public int Id { get; set; }
        public int RegistrationId { get; set; }
        public int ProgrammeId { get; set; }
    }

    public class RegistrationMember
    {
        public int Id { get; set; }
        public string MemberId { get; set; }
        public string CareGiverId { get; set; }
        public string DateOfBirth { get; set; }
        public string FirstName { get; set; }
        public string IdentificationNumber { get; set; }
        public string PhoneNumber { get; set; }
        public int? SpouseInHouseholdId { get; set; }
        public string SpouseId { get; set; }
        public string Surname { get; set; }
        public int? ChronicIllnessStatusId { get; set; }

        public int? DisabilityCareStatusId { get; set; }

        public int? DisabilityTypeId { get; set; }

        public int? EducationLevelId { get; set; }
        public int? SchoolTypeId { get; set; }

        public int? FatherAliveStatusId { get; set; }

        public int? FormalJobNgoId { get; set; }

        public int? IdentificationDocumentTypeId { get; set; }

        public int? LearningStatusId { get; set; }

        public int? MaritalStatusId { get; set; }

        public int? MotherAliveStatusId { get; set; }

        public int? RelationshipId { get; set; }

        public int? SexId { get; set; }
        public int? WorkTypeId { get; set; }

        public string MiddleName { get; set; }

        public int RegistrationId { get; set; }

        public string AddTime { get; set; }
    }

    public class Enumerator
    {
        public int Id { get; set; }

        public int EnumeratorGroupId { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
        public string NationalIdNo { get; set; }
        public string MobileNo { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LoginDate { get; set; }
        public DateTime? ActivityDate { get; set; }
        public int AccessFailedCount { get; set; }
        public bool IsLocked { get; set; }
        public int? DeactivatedBy { get; set; }
        public DateTime? DeactivatedOn { get; set; }
        public string FullName => $"{FirstName} {MiddleName} {Surname}";
    }

    public class LoginVM
    {
        public string EmailAddress { get; set; }
        public string NationalId { get; set; }
        public string Pin { get; set; }
        public string Id { get; set; }
    }

    public class ApiStatus
    {
        public string Description { get; set; }
        public int? StatusId { get; set; }
        public int? Id { get; set; }
    }

    public class ResetPinVM
    {
        public string EmailAddress { get; set; }
        public string PreviousPin { get; set; }
        public string NewPin { get; set; }
        public string Id { get; set; }
        public string NationalIdNo { get; set; }
    }

    public class ChangePinVM
    {
        public string PreviousPin { get; set; }
        public string NewPin { get; set; }
        public string Id { get; set; }
        public string NationalIdNo { get; set; }
    }

    public class TabEnvironment
    {
        public int SyncEnumeratorId { get; set; }
        public string DeviceId { get; set; }
        public string DeviceModel { get; set; }
        public string DeviceManufacturer { get; set; }
        public string DeviceName { get; set; }
        public string Version { get; set; }
        public string VersionNumber { get; set; }
        public string AppVersion { get; set; }
        public string AppBuild { get; set; }
        public string Platform { get; set; }
        public string Idiom { get; set; }
        public bool IsDevice { get; set; }
    }

    public class EnumeratorLocation
    {
        public int Id { get; set; }
        public int EnumeratorId { get; set; }
        public int LocationId { get; set; }
        public Enumerator Enumerator { get; set; }

        public bool IsActive { get; set; }
    }

    public class AccountResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("userName")]
        public string Username { get; set; }

        [JsonProperty(".issued")]
        public string IssuedAt { get; set; }

        [JsonProperty(".expires")]
        public string ExpiresAt { get; set; }

        public string Error { get; set; }
        public bool Success { get; set; }
    }

    public class LoginRequest
    {
        public string AuthKey { get; set; }
    }

    public class LoginResponse
    {
        public int Success { get; set; }
    }

    public class CreateFields
    {
        [DisplayName("Date Created"), DataType(DataType.Text)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime DateCreated { get; set; }

        [DisplayName("Created By")]
        public string CreatedById { get; set; }

        [DisplayName("Created By")]
        public ApplicationUser CreatedBy { get; set; }
    }

    public class CreateApprovalFields : CreateFields
    {
        [DisplayName("Date Approved")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? DateApproved { get; set; }

        [DisplayName("Approved By")]
        public string ApprovedById { get; set; }

        [DisplayName("Approved By")]
        public ApplicationUser ApprovedBy { get; set; }
    }

    public class ApprovalFields
    {
        [DisplayName("Date Approved")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? DateApproved { get; set; }

        [DisplayName("Approved By")]
        public string ApprovedById { get; set; }

        public ApplicationUser ApprovedBy { get; set; }
    }

    public class CreateModifyFields
    {
        public DateTime? DateCreated { get; set; }
        public string CreatedById { get; set; }
        public ApplicationUser CreatedBy { get; set; }

        public DateTime? DateModified { get; set; }
        public string ModifiedById { get; set; }
        public ApplicationUser ModifiedBy { get; set; }
    }

    public class County : CreateModifyFields
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "County Code", Description = "County code defined by Bureau of Statistics")]
        [StringLength(20)]
        public string Code { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public ICollection<Constituency> Constituencies { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public GeoMaster GeoMaster { get; set; }

        [Required]
        [Display(Name = "GeoMaster", Description = "The name of the Geo Master")]
        public int GeoMasterId { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "County", Description = "The name of the county")]
        public string Name { get; set; }
    }

    public class SubCounty
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }
        public int CountyId { get; set; }
        public County County { get; set; }
    }

    public class Ward
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }

        [DisplayName("Sub-County")]
        public int? SubCountyId { get; set; }

        public SubCounty SubCounty { get; set; }

        [DisplayName("Constituency")]
        public int ConstituencyId { get; set; }

        public Constituency Constituency { get; set; }
    }

    public class Constituency
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Name { get; set; }
        public int CountyId { get; set; }
        public County County { get; set; }
    }

    public class District
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "District Id")]
        public int Id { get; set; }

        public GeoMaster GeoMaster { get; set; }

        [Display(Name = "Geo Master Id")]
        [Required]
        public int GeoMasterId { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "District Name")]
        public string Name { get; set; }

        public int ConstituencyId { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public Constituency Constituency { get; set; }
    }

    public class Division : CreateModifyFields
    {
        [Display(Name = "Division")]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Display(Name = "Division Name")]
        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public ICollection<Location> Locations { get; set; }

        public int DistrictId { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public District District { get; set; }
    }

    public class Location : CreateModifyFields
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Location")]
        public int Id { get; set; }

        [Display(Name = "Location Name")]
        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public ICollection<SubLocation> SubLocations { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public Division Division { get; set; }

        [Display(Name = "Division")]
        public int DivisionId { get; set; }
    }

    public class SubLocation : CreateModifyFields
    {
        [Display(Name = "Sub Location")]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(30)]
        [Required]
        [Display(Name = "Sub Location Name")]
        public string Name { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public Location Location { get; set; }

        [Display(Name = "Location")]
        public int LocationId { get; set; }

        public string RuralUrban { get; set; }
    }

    public class Village : CreateModifyFields
    {
        [Display(Name = "Village ID")]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Display(Name = "Village Name")]
        [StringLength(30), Required]
        public string Name { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public SubLocation SubLocation { get; set; }

        [Display(Name = "Sub Location")]
        public int? SubLocationId { get; set; }
    }

    public class CommunityArea
    {
        public int Id { get; set; }
        public int VillageId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class GeoMaster : CreateModifyFields
    {
        [JsonIgnore]
        [XmlIgnore]
        public ICollection<County> Counties { get; set; }

        [Display(Name = "Description", Description = "Short description about the Geo Master"), Required]
        [StringLength(100)]
        public string Description { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public ICollection<District> Districts { get; set; }

        [Display(Name = "Geo Master", Description = "Geo Master ID")]
        public int Id { get; set; }

        [Display(Name = "Default GeoMaster", Description = "Flag to set the Default GeoMaster in the System")]
        public bool IsDefault { get; set; }

        [Display(Name = "Geo Master", Description = "The name of the Geo Master")]
        [StringLength(20), Required]
        // [Index(IsUnique = true, IsClustered = false)]
        public string Name { get; set; }
    }

    public class ErrorVm
    {
        public string Error { get; set; }
    }

    public class CommValidationVm : ErrorVm
    {
        public Enumerator Enumerator { get; set; }
        public List<HouseholdRegVm> Registrations { get; set; }
    }

    public class CvSetupVm : ErrorVm
    {
        public List<SystemCodeVm> SystemCodes { get; set; }
        public Enumerator Enumerator { get; set; }
        public List<SystemCodeDetailVm> SystemCodeDetails { get; set; }

        public List<WardVm> Wards { get; set; }
        public List<VillageVm> Villages { get; set; }
        public List<CommunityAreaVm> CommunityAreas { get; set; }
        //    public List<EnumeratorLocation> EnumeratorLocations { get; set; }
    }

    public class SystemCodeDetail
    {
        public int Id { get; set; }

        [DisplayName("System Code")]
        public int SystemCodeId { get; set; }

        public string Code { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.##}")]
        [Required]
        public decimal? OrderNo { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public SystemCode SystemCode { get; set; }

        public string OrderNoNumber
        {
            get
            {
                return ((decimal)OrderNo).ToString("0");
            }
        }

        public string DisplayName
        {
            get
            {
                if (OrderNo != null)
                    return ((decimal)OrderNo).ToString("0.##") + ". " + Code;
                return "";
            }
        }
    }

    public class SystemCode
    {
        public int Id { get; set; }

        public string Code { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        // [JsonIgnore][XmlIgnore]
        //public ICollection<SystemCodeDetail> SystemCodeDetails { get; set; }
        [DisplayName("System Module")]
        public int? SystemModuleId { get; set; }

        // [JsonIgnore][XmlIgnore]
        //public SystemModule SystemModule { get; set; }
    }

    public class SystemModule
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class HouseholdRegVm
    {
        //public int? Id { get; set; }

        public string ParentId { get; set; }
        public int? WardId { get; set; }
        public int? SubLocationId { get; set; }
        public DateTime? DwellingStartDate { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? CommunityAreaId { get; set; }
        public int? VillageId { get; set; }
        public DateTime? DateApproved { get; set; }
        public string NearestReligiousBuilding { get; set; }
        public string NearestSchool { get; set; }
        public string PhysicalAddress { get; set; }
        public byte? ProgrammeId { get; set; }
        public DateTime? SyncDate { get; set; }
        public string VersionId { get; set; }
        public string CommonName { get; set; }
        public string Village { get; set; }
        public int HealthFacilityId { get; set; }
        public int? ResidenceDurationMonths { get; set; }
        public int? ResidenceDurationYears { get; set; }
        public string Phone { get; set; }
        public string MotherId { get; set; }
        public string UniqueId { get; set; }
        public int? StatusId { get; set; }
        public int? TypeId { get; set; }
        public string NOKMiddleName { get; set; }
        public string NOKFirstName { get; set; }
        public string NOKSurname { get; set; }
        public string NOKPhone { get; set; }
        public string HouseholdId { get; set; }
        public string DisabilityCaregiverId { get; set; }
        public int? DisabilityTypeId { get; set; }
        public string DateOfBirth { get; set; }
        public int? EducationAttendanceId { get; set; }
        public int? EducationLevelId { get; set; }
        public bool? ExternalMember { get; set; }
        public int? FatherAliveId { get; set; }
        public string FirstName { get; set; }
        public int? FormalJobTypeId { get; set; }
        public int? IllnessTypeId { get; set; }
        public int? MainCaregiverId { get; set; }
        public int? MaritalStatusId { get; set; }
        public string MiddleName { get; set; }
        public int? MotherAliveId { get; set; }
        public string IdNumber { get; set; }
        public int? OccupationTypeId { get; set; }
        public string Surname { get; set; }
        public int? GenderId { get; set; }
        public int? IdentificationFormId { get; set; }
        public DateTime? CreateOn { get; set; }
    }
}