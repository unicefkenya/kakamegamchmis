using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MCHMIS.Api.Models
{
    public class HouseholdReg : CreateApprovalFields

    {
        public string Id { get; set; }

        [DisplayName("Mother Unique Id")]
        public string UniqueId { get; set; }

        public int? SubLocationId { get; set; }
        public SubLocation SubLocation { get; set; }

        [DisplayName("Village")]
        public int? VillageId { get; set; }

        public Village Village { get; set; }

        [DisplayName("Village")]
        public int? CountyId { get; set; }

        public County County { get; set; }

        [DisplayName("County")]
        public int? OtherCountyId { get; set; }

        public County OtherCounty { get; set; }

        [DisplayName("PHYSICAL ADDRESS")]
        public string PhysicalAddress { get; set; }

        [DisplayName("NEAREST CHURCH/MOSQUE")]
        public string NearestReligiousBuilding { get; set; }

        [DisplayName("NEAREST SCHOOL")]
        public string NearestSchool { get; set; }

        [DisplayName("Duration of residence in this place: Years")]
        public int? ResidenceDurationYears { get; set; }

        [DisplayName("Months")]
        public int? ResidenceDurationMonths { get; set; }

        [DisplayName("Status")]
        public int StatusId { get; set; }

        public Status Status { get; set; }

        public DateTime? DwellingStartDate { get; set; }

        public DateTime? CaptureStartDate { get; set; }

        public DateTime? CaptureEndDate { get; set; }

        public float? Longitude { get; set; }

        public float? Latitude { get; set; }

        public float? Elevation { get; set; }

        public string SerialNo { get; set; }

        public string DeviceId { get; set; }

        public string VersionId { get; set; }

        public DateTime? SyncDate { get; set; }

        /*Mothers Detail*/
        public string CommonName { get; set; }

        [DisplayName("Phone Number")]
        public string Phone { get; set; }

        [DisplayName("Recipient Names (If the Phone Number Does Not Belong to the Mother)")]
        public string RecipientNames { get; set; }

        [DisplayName("Does the phone belong to the mother?")]
        public bool OwnsPhone { get; set; }

        [DisplayName("Nominee First Name (As registered with MPesa)")]
        public string NomineeFirstName { get; set; }

        [DisplayName("Nominee Middle Name")]
        public string NomineeMiddleName { get; set; }

        [DisplayName("Nominee Surname")]
        public string NomineeSurname { get; set; }

        [DisplayName("Nominee National ID number")]
        public string NomineeIdNumber { get; set; }

        [DisplayName("Next of Kin First Name (As recorded in the Health Handbook)")]
        public string NOKFirstName { get; set; }

        [DisplayName("Next of Kin Middle Name")]
        public string NOKMiddleName { get; set; }

        [DisplayName("Next of Kin Surname")]
        public string NOKSurname { get; set; }

        [DisplayName("Next of Kin Phone Number ")]
        [StringLength(10, ErrorMessage = "The {0} must be at 10 characters long", MinimumLength = 10)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "The {0} must be numeric")]
        public string NOKPhone { get; set; }

        [DisplayName("Next of KinNational ID number")]
        public string NOKIdNumber { get; set; }

        public bool? HasProxy { get; set; }

        [DisplayName("Health Facility")]
        public int HealthFacilityId { get; set; }

        public HealthFacility HealthFacility { get; set; }

        public string Institution { get; set; }
        public string PhotoUrl { get; set; }

        [DisplayName("NHIF Number")]
        public string NHIFNo { get; set; }

        [DisplayName("Parity")]
        public string Para { get; set; }

        public int? Gravida { get; set; }

        public string Parity { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        [DisplayName("LMP")]
        public DateTime? LMP { get; set; }

        [DisplayName("LMP Unknown")]
        public bool? LMPUnknown { get; set; }

        [DisplayName("Estimated month of pregnancy")]
        public int? EPM { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        [DisplayName("EDD")]
        public DateTime? EDD { get; set; }

        public byte[] FingerPrint { get; set; }

        [DisplayName("Take Fingerprint")]
        public bool VerifyingFingerPrint { get; set; }

        //public string CommunityValidationId { get; set; }
        //public CVHouseHold CommunityValidation { get; set; }

        //public string SecondCommunityValidationId { get; set; }
        //public CVHouseHold SecondCommunityValidation { get; set; }
        /*Mothers Detail*/
        public string MotherId { get; set; }

        [ForeignKey("MotherId")]
        public HouseholdRegMember Mother { get; set; }

        public ICollection<HouseholdRegAsset> HouseholdRegAssets { get; set; }

        public ICollection<HouseholdRegMember> HouseholdRegMembers { get; set; }

        public ICollection<HouseholdRegOtherProgramme> HouseholdRegOtherProgrammes { get; set; }

        public ICollection<Notes> Notes { get; set; }

        [DisplayName("PMT Score")]
        // [Column(TypeName = "decimal(18, 10)")]
        public decimal? PMTScore { get; set; }

        public int? TypeId { get; set; }
        public string ParentId { get; set; }
        public bool? RequiresHealthVerification { get; set; }

        public bool IPRSVerified { get; set; }
        public bool? IPRSPassed { get; set; }
        public bool RequiresIPRSECheck { get; set; }

        public int? ExitReasonId { get; set; }
        public SystemCodeDetail ExitReason { get; set; }
    }

    public class HouseholdRegMember
    {
        public string Id { get; set; }

        public string HouseholdId { get; set; }

        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Middle Name")]
        public string MiddleName { get; set; }

        public string Surname { get; set; }

        [DisplayName("Identification Document")]
        public int? IdentificationFormId { get; set; }

        public SystemCodeDetail IdentificationForm { get; set; }

        [DisplayName("ID Number")]
        public string IdNumber { get; set; }

        [DisplayName("Relationship to the head of this household")]
        public int? RelationshipId { get; set; }

        [DisplayName("Gender")]
        public int? GenderId { get; set; }

        [DisplayName("Date of Birth")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime DOB { get; set; }

        [DisplayName("Marital Status")]
        public int? MaritalStatusId { get; set; }

        [DisplayName("Does Spouse live in this household?")]
        public int? SpouseInHouseholdId { get; set; }

        public SystemCodeDetail SpouseInHousehold { get; set; }

        [DisplayName("Is father alive?")]
        public int? FatherAliveId { get; set; }

        [DisplayName("Is mother alive?")]
        public int? MotherAliveId { get; set; }

        [DisplayName("Suffers from chronic illness")]
        public int? ChronicIllnessId { get; set; }

        public SystemCodeDetail ChronicIllness { get; set; }

        [DisplayName("Main Care Giver")]
        public string DisabilityCaregiverId { get; set; }

        [DisplayName("Does Disability require 24-hour care?")]
        public int? DisabilityRequires24HrCareId { get; set; }

        public SystemCodeDetail DisabilityRequires24HrCare { get; set; }

        [DisplayName("Main Care Giver")]
        public HouseholdRegMember DisabilityCaregiver { get; set; }

        public bool? ExternalMember { get; set; }

        public int? MainCaregiverId { get; set; }

        [DisplayName("Learning Institution attendance status")]
        public int? EducationAttendanceId { get; set; }

        [DisplayName("Highest Std/Form/Level reached by the member?")]
        public int? EducationLevelId { get; set; }

        [DisplayName("School Type")]
        public int? SchoolTypeId { get; set; }

        public SystemCodeDetail SchoolType { get; set; }

        [DisplayName("What was the member doing during the last seven days?")]
        public int? OccupationTypeId { get; set; }

        [DisplayName("Does member work on a formal job, teaching, public sector, NGO/FBO?")]
        public int? FormalJobTypeId { get; set; }

        [DisplayName("HIV Status")]
        public string SupportStatusId { get; set; }

        [DisplayName("MUAC")]
        public decimal? MUAC { get; set; }
       
        public DateTime CreateOn { get; set; }
        public HouseholdReg Household { get; set; }

        public SystemCodeDetail Relationship { get; set; }
        public SystemCodeDetail Gender { get; set; }
        public SystemCodeDetail MaritalStatus { get; set; }

        public SystemCodeDetail FatherAlive { get; set; }
        public SystemCodeDetail MotherAlive { get; set; }

        public SystemCodeDetail EducationAttendance { get; set; }
        public SystemCodeDetail EducationLevel { get; set; }
        public SystemCodeDetail OccupationType { get; set; }
        public SystemCodeDetail FormalJobType { get; set; }
        public ICollection<HouseholdRegMemberDisability> HouseholdRegMemberDisabilities { get; set; }
        public string FullName => FirstName + " " + MiddleName + " " + Surname;
    }

    public class CVList : CreateApprovalFields
    {
        public string Id { get; set; }

        [DisplayName("Ward")]
        public int? WardId { get; set; }

        public Ward Ward { get; set; }

        [DisplayName("HealthFacility")]
        public int HealthFacilityId { get; set; }

        public int Households { get; set; }

        [DisplayName("Validated")]
        public int Captured { get; set; }

        public HealthFacility HealthFacility { get; set; }

        public int StatusId { get; set; }
        public ApprovalStatus Status { get; set; }
        public string Notes { get; set; }
        public string CaptureNotes { get; set; }

        public int? ListTypeId { get; set; }
        public SystemCodeDetail ListType { get; set; }
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

        public float? Variance { get; set; }
        public int? VarianceCategoryId { get; set; }
        public SystemCodeDetail VarianceCategory { get; set; }

        public int? ApprovalStatusId { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }

        public int? EnumeratorId { get; set; }
        public Enumerator Enumerator { get; set; }

        public int? InterviewStatusId { get; set; }
        public int? InterviewResultId { get; set; }

        public int? CannotFindHouseholdReasonId { get; set; }
        public SystemCodeDetail CannotFindHouseholdReason { get; set; }

        public DateTime? DateSubmitedByCHV { get; set; }
    }

    public class HouseholdRegMemberDisability
    {
        [Key, Column(Order = 1)]
        public string HouseholdRegMemberId { get; set; }

        [Key, Column(Order = 2)]
        public int DisabilityId { get; set; }

        public SystemCodeDetail Disability { get; set; }
        public HouseholdRegMember HouseholdRegMember { get; set; }
    }

    public class HouseholdRegOtherProgramme
    {
        [Key, Column(Order = 1)]
        public string HouseholdId { get; set; }

        [Key, Column(Order = 2)]
        public int OtherProgrammeId { get; set; }

        public SystemCodeDetail OtherProgramme { get; set; }
        public HouseholdReg Household { get; set; }
    }

    public class HealthFacility
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [DisplayName("Sub County")]
        public int SubCountyId { get; set; }

        public SubCounty SubCounty { get; set; }
    }

    public class Status
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ApprovalStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class HouseholdRegAsset
    {
        [Key, Column(Order = 1)]
        public string HouseholdId { get; set; }

        [Key, Column(Order = 2)]
        public int AssetId { get; set; }

        public int AssetTypeId { get; set; }
        public SystemCodeDetail AssetType { get; set; }

        public bool? HasItem { get; set; }

        public int? ItemCount { get; set; }

        public SystemCodeDetail Asset { get; set; }
        public HouseholdReg Household { get; set; }
    }

    public class HouseholdRegCharacteristic
    {
        [Key]
        public string HouseholdId { get; set; }

        [DisplayName("No. of Habitable Rooms")]
        public int? HabitableRoomsNo { get; set; }

        public bool IsOwnHouse { get; set; }

        [DisplayName("Tenure Status")]
        public int TenureStatusId { get; set; }

        public int? TenureOwnerOccupiedId { get; set; }

        public string TenureStatusOther { get; set; }

        [DisplayName("Roof")]
        public int RoofMaterialId { get; set; }

        public string RoofMaterialOther { get; set; }

        [DisplayName("Wall")]
        public int WallMaterialId { get; set; }

        public string WallMaterialOther { get; set; }

        [DisplayName("Floor")]
        public int FloorMaterialId { get; set; }

        public string FloorMaterialOther { get; set; }

        [DisplayName("Dwelling Unit Risk")]
        public int UnitRiskId { get; set; }

        public string UnitRiskOther { get; set; }

        [DisplayName("Main source of WATER")]
        public int WaterSourceId { get; set; }

        public string WaterSourceOther { get; set; }

        [DisplayName("Main mode of HUMAN WASTE DISPOSAL")]
        public int ToiletTypeId { get; set; }

        public string ToiletTypeOther { get; set; }

        [DisplayName("Main type of COOKING FUEL")]
        public int CookingFuelSourceId { get; set; }

        public string CookingFuelSourceOther { get; set; }

        [DisplayName("Main type of LIGHTING FUEL")]
        public int LightingSourceId { get; set; }

        public string LightingSourceOther { get; set; }

        [DisplayName("LIVE BIRTHS in this household in the last 12 months")]
        public int LiveBirths { get; set; }

        [DisplayName("DEATHS in this household in the last 12 months")]
        public int Deaths { get; set; }

        [DisplayName("Conditions of your household")]
        public int HouseholdConditionId { get; set; }

        [DisplayName("In the past 7 days, did anyone in this household cut the size of the meals or skip meals because of the lack of enough money?")]
        public int HasSkippedMealId { get; set; }

        public SystemCodeDetail HasSkippedMeal { get; set; }

        [DisplayName("Benefits from Social Assistance Programmes")]
        public int IsRecievingNSNPBenefit { get; set; }

        [DisplayName("Is anyone in this household receiving benefits from any other Social Assistance Programme or any other external support?")]
        public int? IsReceivingOtherBenefitId { get; set; }

        [DisplayName("Are you/ Have you ever been in the Imarisha Afya Mama Na Mtoto?")]
        public int HasBeenInMCHProgramId { get; set; }

        public SystemCodeDetail IsReceivingOtherBenefit { get; set; }

        [DisplayName("Name of the PROGRAMME(s)")]
        public string OtherProgrammes { get; set; }

        [DisplayName("Type of BENEFIT received")]
        public int? OtherProgrammesBenefitTypeId { get; set; }

        [DisplayName("Amount of BENEFIT in the last receipt")]
        public decimal? OtherProgrammesBenefitAmount { get; set; }

        [DisplayName("IN-KIND of benefit")]
        public string OtherProgrammesInKindBenefit { get; set; }

        [Required(ErrorMessage = "* Required")]
        [DisplayName("Receives other kind of support from relatives or friends?")]
        public int? BenefitFromFriendsRelativeId { get; set; }

        public SystemCodeDetail BenefitFromFriendsRelative { get; set; }

        // public HouseholdReg Household { get; set; }
        public SystemCodeDetail TenureStatus { get; set; }

        public SystemCodeDetail RoofMaterial { get; set; }
        public SystemCodeDetail WallMaterial { get; set; }
        public SystemCodeDetail FloorMaterial { get; set; }
        public SystemCodeDetail UnitRisk { get; set; }
        public SystemCodeDetail WaterSource { get; set; }
        public SystemCodeDetail ToiletType { get; set; }
        public SystemCodeDetail CookingFuelSource { get; set; }
        public SystemCodeDetail LightingSource { get; set; }
        public SystemCodeDetail HouseholdCondition { get; set; }
        public SystemCodeDetail OtherProgrammesBenefitType { get; set; }
    }

    public class NotesCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Notes : CreateFields
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public int CategoryId { get; set; }
        public NotesCategory Category { get; set; }

        public string HouseholdId { get; set; }
        public HouseholdReg Household { get; set; }
    }

    public class Programme
    {
        public byte Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }
    }
}