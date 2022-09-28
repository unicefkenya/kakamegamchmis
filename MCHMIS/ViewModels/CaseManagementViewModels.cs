using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MCHMIS.Models;
using X.PagedList;

namespace MCHMIS.ViewModels
{
    public class CaseManagementViewModel : CreateFields
    {
        public string Id { get; set; }

        public string CaseManagementId { get; set; }
        public string HouseholdId { get; set; }
        public HouseholdReg Household { get; set; }

        [DisplayName("ANC")]
        public int ClinicVisitId { get; set; }

        public ClinicVisit ANC { get; set; }

        [DisplayName("Blood Group")]
        public int? BloodGroupId { get; set; }

        public SystemCodeDetail BloodGroup { get; set; }

        [DisplayName("Rhesus")]
        public int? RhesusId { get; set; }

        public SystemCodeDetail Rhesus { get; set; }

        [DisplayName("HIV Status"), Required]
        public int? SupportStatusId { get; set; }

        public SystemCodeDetail SupportStatus { get; set; }

        [DisplayName("Number of pregnancies"), Required]
        public int? PregnancyNo { get; set; }

        [DisplayName("Infant feeding counseling done?")]
        public int? InfantFeedingCounselingDoneId { get; set; }

        public SystemCodeDetail InfantFeedingCounselingDone { get; set; }

        [DisplayName("Counseling on exclusive breastfeeding done?")]
        public int? BreastfeedingCounselingDoneId { get; set; }

        public SystemCodeDetail BreastfeedingCounselingDone { get; set; }

        [DisplayName("Visit Date"), Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? VisitDate { get; set; }

        [Required]
        public decimal? Weight { get; set; }

        [Required]
        public string BloodPressure { get; set; }

        [DisplayName("Preventive Service Given")]
        public IEnumerable<int> PreventiveServices { get; set; }

        [DisplayName("Next Visit"), Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? NextVisit { get; set; }

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
    }

    public class PregnancyDataUpdateViewModel : CreateFields
    {
        public string Id { get; set; }

        public string PregnancyId { get; set; }
        public string HouseholdId { get; set; }
        public HouseholdReg Household { get; set; }

        [DisplayName("ANC Visit")]
        public int ClinicVisitId { get; set; }

        public int? UpdatingClinicVisitId { get; set; }

        public bool IsUpdating { get; set; }

        public ClinicVisit ANC { get; set; }

        [DisplayName("Blood Group")]
        public int? BloodGroupId { get; set; }

        public SystemCodeDetail BloodGroup { get; set; }

        [DisplayName("Rhesus")]
        public int? RhesusId { get; set; }

        public SystemCodeDetail Rhesus { get; set; }

        [DisplayName("HIV Status")]
        public int? SupportStatusId { get; set; }

        public SystemCodeDetail SupportStatus { get; set; }

        [DisplayName("Number of pregnancies")]
        public int? PregnancyNo { get; set; }

        [DisplayName("Infant feeding counseling done?")]
        public int? InfantFeedingCounselingDoneId { get; set; }

        public SystemCodeDetail InfantFeedingCounselingDone { get; set; }

        [DisplayName("Counseling on exclusive breastfeeding done?")]
        public int? BreastfeedingCounselingDoneId { get; set; }

        public SystemCodeDetail BreastfeedingCounselingDone { get; set; }

        [DisplayName("Visit Date"), Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? VisitDate { get; set; }

        [Required]
        public decimal? Weight { get; set; }

        [DisplayName("Blood Pressure"), Required]
        public string BloodPressure { get; set; }

        public PregnancyData PregnancyData { get; set; }
        public IEnumerable<PregnancyData> PregnancyDataList { get; set; }

        [DisplayName("Preventive Service Given")]
        public IEnumerable<int> PreventiveServices { get; set; }

        [DisplayName("Next Visit"), Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? NextVisit { get; set; }

        public int? StatusId { get; set; }

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
    }

    public class DeliveryViewModel : CreateFields
    {
        public string PregnancyId { get; set; }
        public string DeliveryId { get; set; }
        public string HouseholdId { get; set; }
        public string ChildId { get; set; }

        [Required]
        [DisplayName("Pregnancy Outcome")]
        public int? PregnancyOutcomeId { get; set; }

        [Required]
        [DisplayName("Duration of Pregnancy (Weeks)")]
        public int? PregnancyDuration { get; set; }

        public SystemCodeDetail HIVTested { get; set; }

        [DisplayName("HIV Status")]
        public int? SupportStatusId { get; set; }

        public int? PrevSupportStatusId { get; set; }

        public SystemCodeDetail SupportStatus { get; set; }

        [DisplayName("Mode of Delivery")]
        public string DeliveryModeId { get; set; }

        [DisplayName("Date of Delivery")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? DeliveryDate { get; set; }

        [DisplayName("Next Visit")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? NextVisit { get; set; }

        [DisplayName("Blood Loss")]
        public int BloodLossId { get; set; }

        public SystemCodeDetail BloodLoss { get; set; }

        [DisplayName("Pre-eclampsia")]
        public int PreEclampsiaId { get; set; }

        public SystemCodeDetail PreEclampsia { get; set; }

        [DisplayName("Eclampsia")]
        public int EclampsiaId { get; set; }

        public SystemCodeDetail Eclampsia { get; set; }

        [DisplayName("Obstructed Labour")]
        public int ObstructedLabourId { get; set; }

        public SystemCodeDetail ObstructedLabour { get; set; }

        [DisplayName("Condition of Mother")]
        public string MotherCondition { get; set; }

        [DisplayName("Apgar Score")]
        public string ApgarScore { get; set; }

        [DisplayName("Rescuscitation Done?")]
        public int RescuscitationDoneId { get; set; }

        public SystemCodeDetail RescuscitationDone { get; set; }

        [DisplayName("Meconium Stained Liquor (grade)")]
        public int MeconiumStainedLiquorId { get; set; }

        public SystemCodeDetail MeconiumStainedLiquor { get; set; }

        [DisplayName("Uterotonics")]
        public List<int> MotherDrugsAdministeredIds { get; set; }

        [DisplayName("Drugs administered immediately after delivery")]
        public List<int> BabyDrugsAdministeredIds { get; set; }

        [DisplayName("Place of delivery")]
        public int? DeliveryPlaceId { get; set; }

        [DisplayName("Delivery Health Facility")]
        public int? DeliveryHealthFacilityId { get; set; }

        [DisplayName("Specify Delivery Health Facility")]
        public string OtherDeliveryHealthFacility { get; set; }

        public SystemCodeDetail DeliveryPlace { get; set; }

        [DisplayName("Specify Delivery Place")]
        public string OtherDeliveryPlace { get; set; }

        [DisplayName("Conducted by")]
        public int? DeliveryAssistantId { get; set; }

        public SystemCodeDetail DeliveryAssistant { get; set; }

        [DisplayName("Specify who conducted the Delivery")]
        public string OtherDeliveryAssistant { get; set; }

        [DisplayName("Name of Child")]
        public string Name { get; set; }

        [DisplayName("Sex of Child")]
        public int ChildGenderId { get; set; }

        public SystemCodeDetail ChildGender { get; set; }

        [DisplayName("Date First Seen")]
        public string DateFirstSeen { get; set; }

        [DisplayName("Birth Weight in Kgs")]
        public string BirthWeight { get; set; }

        [DisplayName("Birth Length in cm")]
        public string BirthLength { get; set; }

        [DisplayName("Head circumference")]
        public string HeadCircumference { get; set; }

        [DisplayName("Other Birth Defects / Characteristics’")]
        public string OtherBirthCharacteristics { get; set; }

        [DisplayName("Birth Order in Family")]
        public string BirthOrder { get; set; }

        public bool IsUpdating { get; set; }
        public string Option { get; set; }
        public string OptionId { get; set; }
        public IEnumerable<Child> Children { get; set; }

        [DisplayName("Fingerprint Verified?")]
        public bool IsVerified { get; set; }

        public bool IsBeneficiary { get; set; }
        public int? StatusId { get; set; }
        public string Notes { get; set; }

        [DisplayName("Immunized?")]
        public int? ImmunizedId { get; set; }

        public SystemCodeDetail Immunized { get; set; }
    }

    public class PreventiveServicesViewModel
    {
        [DisplayName("Preventive Service")]
        public int PreventiveServiceId { get; set; }

        public string PregnancyId { get; set; }
        public string CaseManagementId { get; set; }

        [DisplayName("Date Given")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? DateGiven { get; set; }

        [DisplayName("Next Visit")]
        public DateTime? NextVisit { get; set; }

        public HouseholdReg Household { get; set; }
        public IEnumerable<SystemCodeDetail> Services { get; set; }
        public MotherPreventiveService MotherPreventiveService { get; set; }
        public IEnumerable<MotherPreventiveService> MotherPreventiveServices { get; set; }
        public bool IsUpdating { get; set; }
        public int UpdatingPreventiveServiceId { get; set; }
    }

    public class PostNatalViewModel
    {
        public string PregnancyId { get; set; }

        [DisplayName("Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? VisitDate { get; set; }

        [DisplayName("PNC Visit")]
        public int ClinicVisitId { get; set; }

        public int? UpdatingClinicVisitId { get; set; }

        [DisplayName("Blood Pressure")]
        public string BloodPressure { get; set; }

        public string GeneralCondition { get; set; }

        [DisplayName("Counseling on family planning")]
        public int? FPCounselingId { get; set; }

        public SystemCodeDetail FPCounseling { get; set; }

        [DisplayName("HIV Status")]
        public int? SupportStatusId { get; set; }

        public int? PrevSupportStatusId { get; set; }

        public SystemCodeDetail SupportStatus { get; set; }

        [DisplayName("FP Method")]
        public int? FPMethodId { get; set; }

        public SystemCodeDetail FPMethod { get; set; }

        [DisplayName("Child"), Required]
        public string ChildId { get; set; }

        [DisplayName("Baby’s general condition well…unwell…")]
        public string BabyGeneralCondition { get; set; }

        [DisplayName("Baby’s feeding method")]
        public string FeedingMethodId { get; set; }

        public SystemCodeDetail FeedingMethod { get; set; }

        [DisplayName("Baby immunization started?")]
        public int? ImmunizationStartedId { get; set; }

        public SystemCodeDetail ImmunizationStarted { get; set; }

        [DisplayName("HEI infant given ART prophylaxis")]
        public int? ARTProphylaxisGivenId { get; set; }

        public SystemCodeDetail ARTProphylaxisGiven { get; set; }

        [DisplayName("Infant Cotrimoxazole prophylaxis initiated")]
        public int? CPInitiatedId { get; set; }

        public SystemCodeDetail CPInitiated { get; set; }

        public PostNatalExamination PostNatalExamination { get; set; }
        public IEnumerable<PostNatalExamination> PostNatalExaminations { get; set; }
        public PostNatalExaminationDetail PostNatalExaminationDetail { get; set; }

        public string Option { get; set; }
        public string optionId { get; set; }

        [DisplayName("PNC Visit")]
        public string PostNatalExaminationId { get; set; }

        public IEnumerable<KeyMilestone> KeyMilestones { get; set; }
        public IEnumerable<PostNatalMilestone> PostNatalMilestones { get; set; }

        [DisplayName("Key Milestones")]
        public List<int> KeyMilestoneIds { get; set; }

        [DisplayName("Next Visit"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? NextVisit { get; set; }

        public int? StatusId { get; set; }

        public bool BirthCertificateObatined { get; set; }
    }

    public class FamilyPlanningViewModel : CreateFields
    {
        public string PregnancyId { get; set; }

        [DisplayName("Date"), Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? VisitDate { get; set; }

        [DisplayName("Clinical Notes"), Required]
        public string Notes { get; set; }

        [DisplayName("Next Visit"), Required]
        public DateTime? NextVisit { get; set; }

        public FamilyPlanning FamilyPlanning { get; set; }
        public IEnumerable<FamilyPlanning> FamilyPlannings { get; set; }
        public string OptionId { get; set; }
        public int? StatusId { get; set; }
    }

    public class ChildDataCommon
    {
        [DisplayName("Child")]
        public string ChildId { get; set; }

        public string PregnancyId { get; set; }
        public int? HealthFacilityId { get; set; }
        public int? DeliveryPlaceId { get; set; }

        public DateTime? DOB { get; set; }

        [DisplayName("Gestation at birth (in weeks)")]
        public int? GestationAtBirth { get; set; }

        public IEnumerable<Child> Children { get; set; }
        public IEnumerable<DevelopmentMilestone> DevelopmentMilestones { get; set; }
        public IEnumerable<ChildDevelopmentMilestone> ChildDevelopmentMilestones { get; set; }
    }

    public class ChildFeedingInformationViewModel : ChildDataCommon
    {
        [DisplayName("Breastfeeding")]
        public string BreastfeedingId { get; set; }

        public SystemCodeDetail Breastfeeding { get; set; }

        [DisplayName("Other food introduced below 6 months")]
        public string OtherFoodIntroducedId { get; set; }

        public SystemCodeDetail OtherFoodIntroduced { get; set; }
        public string OtherFoodIntroducedAge { get; set; }

        [DisplayName("Complimentary food: Other food introduced")]
        public string ComplimentaryFoodIntroducedId { get; set; }

        public SystemCodeDetail ComplimentaryFoodIntroduced { get; set; }

        [DisplayName("Retention of feeds/indigestion")]
        public string FeedRetention { get; set; }
    }

    public class ChildHealthRecordViewModel : ChildDataCommon
    {
        public string ChildId { get; set; }

        public string PregnancyId { get; set; }
        public Pregnancy Pregnancy { get; set; }

        [DisplayName("Place of delivery"), Required]
        public int? DeliveryPlaceId { get; set; }

        public SystemCodeDetail DeliveryPlace { get; set; }
        public string OtherDeliveryPlace { get; set; }

        [DisplayName("Health Facility Name")]
        public int? HealthFacilityId { get; set; }

        public DateTime? DOB { get; set; }

        [DisplayName("Gestation at birth (in weeks)")]
        public int? GestationAtBirth { get; set; }

        public HealthFacility HealthFacility { get; set; }
        public string HealthFacilityName { get; set; }

        [DisplayName("Birth Notification Number")]
        public string BirthNotificationNumber { get; set; }

        [DisplayName("Permanent Register Number"), Required]
        public string PermanentRegisterNumber { get; set; }

        [DisplayName("Child Welfare Clinic (CWC) Number"), Required]
        public string CWCNumber { get; set; }

        [DisplayName("Master Facility List  (MFL) Number"), Required]
        public string MFLNumber { get; set; }
    }

    public class DevelopmentalMilestonesViewModel : ChildDataCommon
    {
        public List<int> Ids { get; set; }
        public List<string> ChildAges { get; set; }
    }

    public class CivilRegistrationViewModel : ChildDataCommon
    {
        public bool BirthCertificateAvailable { get; set; }

        //CIVIL REGISTRATION – (CHILD DETAILS)
        [DisplayName("Birth Certificate Number")]
        public string BirthCertificateNumber { get; set; }

        [DisplayName("Date of Registration")]
        public DateTime? RegistrationDate { get; set; }

        [DisplayName("Place of Registration")]
        public string RegistrationPlace { get; set; }

        [DisplayName("Upload BirthCertificate")]
        public string BirthCertificate { get; set; }

        [DisplayName("Special Birth: Multiple birth e.g.Twin / Triplet")]
        public string SpecialBirth { get; set; }

        [DisplayName("Birth Mark")]
        public string BirthMark { get; set; }

        [DisplayName("Congenital abnormalities (cleft lip, clubbed foot etc.)")]
        public string CongenitalAbnormalities { get; set; }
    }

    public class ChildHealthMonitoringViewModel
    {
        public string PregnancyId { get; set; }

        [DisplayName("Child")]
        public string ChildId { get; set; }

        [DisplayName("Place of delivery"), Required]
        public int? DeliveryPlaceId { get; set; }

        public SystemCodeDetail DeliveryPlace { get; set; }
        public string OtherDeliveryPlace { get; set; }

        [DisplayName("Health Facility")]
        public int? HealthFacilityId { get; set; }

        public HealthFacility HealthFacility { get; set; }

        [DisplayName("Health Facility Name")]
        public string HealthFacilityName { get; set; }

        [DisplayName("Birth Notification Number")]
        public string BirthNotificationNumber { get; set; }

        [DisplayName("Permanent Register Number"), Required]
        public string PermanentRegisterNumber { get; set; }

        [DisplayName("Child Welfare Clinic (CWC) Number"), Required]
        public string CWCNumber { get; set; }

        [DisplayName("Master Facility List  (MFL) Number"), Required]
        public string MFLNumber { get; set; }

        [DisplayName("Breastfeeding")]
        public string BreastfeedingId { get; set; }

        public SystemCodeDetail Breastfeeding { get; set; }

        [DisplayName("Other food introduced below 6 months")]
        public string OtherFoodIntroducedId { get; set; }

        public SystemCodeDetail OtherFoodIntroduced { get; set; }

        [DisplayName("Feeds introduced at what age?")]
        public string OtherFoodIntroducedAge { get; set; }

        [DisplayName("Complimentary food: Other food introduced")]
        public string ComplimentaryFoodIntroducedId { get; set; }

        public SystemCodeDetail ComplimentaryFoodIntroduced { get; set; }

        [DisplayName("Retention of feeds/indigestion")]
        public string FeedRetention { get; set; }

        //CIVIL REGISTRATION – (CHILD DETAILS)

        public bool BirthCertificateAvailable { get; set; }

        [DisplayName("Birth Certificate Number")]
        public string BirthCertificateNumber { get; set; }

        [DisplayName("Date of Registration")]
        public DateTime? RegistrationDate { get; set; }

        [DisplayName("Place of Registration")]
        public string RegistrationPlace { get; set; }

        [DisplayName("Upload Birth Certificate")]
        public string BirthCertificate { get; set; }

        [DisplayName("Upload Birth Certificate")]
        public string BirthCertificateFile { get; set; }

        [DisplayName("Special Birth: Multiple birth e.g.Twin / Triplet")]
        public string SpecialBirth { get; set; }

        [DisplayName("Congenital abnormalities (cleft lip, clubbed foot etc.)")]
        public string CongenitalAbnormalities { get; set; }

        //Particulars of the child
        [DisplayName("Name of Child"), Required]
        public string Name { get; set; }

        [DisplayName("Sex of Child"), Required]
        public int? GenderId { get; set; }

        public SystemCodeDetail Gender { get; set; }

        [DisplayName("Date of Birth"), Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? DOB { get; set; }

        [DisplayName("Date First Seen"), Required]
        public string DateFirstSeen { get; set; }

        [DisplayName("Gestation at birth (in weeks)")]
        public int? GestationAtBirth { get; set; }

        [DisplayName("Birth Weight in Kgs")]
        public string BirthWeight { get; set; }

        [DisplayName("Birth Length in cm")]
        public string BirthLength { get; set; }

        [DisplayName("Head circumference")]
        public string HeadCircumference { get; set; }

        [DisplayName("Birth Defects / Characteristics  ")]
        public string OtherBirthCharacteristics { get; set; }

        [DisplayName("Birth Order in Family")]
        public string BirthOrder { get; set; }

        public IEnumerable<Child> Children { get; set; }
        public IEnumerable<DevelopmentMilestone> DevelopmentMilestones { get; set; }
        public IEnumerable<ChildDevelopmentMilestone> ChildDevelopmentMilestones { get; set; }
        public List<int> Ids { get; set; }
        public List<string> ChildAges { get; set; }
        public int? StatusId { get; set; }
    }

    public class ChildParticularsViewModel : ChildDataCommon
    {
        //Particulars of the child
        [DisplayName("Name of Child"), Required]
        public string Name { get; set; }

        [DisplayName("Sex of Child"), Required]
        public int? GenderId { get; set; }

        public SystemCodeDetail Gender { get; set; }

        [DisplayName("Child")]
        public string ChildId { get; set; }

        [DisplayName("Date of Birth"), Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? DOB { get; set; }

        [DisplayName("Date First Seen"), Required]
        public string DateFirstSeen { get; set; }

        [DisplayName("Gestation at birth (in weeks)")]
        public int? GestationAtBirth { get; set; }
    }

    public class FatherCivilRegistrationViewModel
    {
        public string HouseholdId { get; set; }

        [DisplayName("Father's Name")]
        public string FatherName { get; set; }

        [DisplayName("Father's Tel No")]
        public string FatherPhone { get; set; }

        [DisplayName("Mother's Name")]
        public string MotherName { get; set; }

        [DisplayName("Mother's Tel No")]
        public string MotherPhone { get; set; }

        [DisplayName("Guardian's Name")]
        public string GuardianName { get; set; }

        [DisplayName("Guardian's Tel No")]
        public string GuardianPhone { get; set; }

        [DisplayName("County")]
        public int? CountyId { get; set; }

        [DisplayName("District")]
        public string DistrictId { get; set; }

        [DisplayName("Division")]
        public string DivisionId { get; set; }

        [DisplayName("Location")]
        public string LocationId { get; set; }

        [DisplayName("Town/Trading Center")]
        public string Town { get; set; }

        [DisplayName("Estate & House No./Village")]
        public string Residence { get; set; }

        public IEnumerable<Division> Divisions { get; set; }
        public IEnumerable<Location> Locations { get; set; }
    }

    public class CaseManagementDetailsViewModel
    {
        public HouseholdReg Household { get; set; }
        public CaseManagement CaseManagement { get; set; }
        public Pregnancy Pregnancy { get; set; }
        public PregnancyData PregnancyData { get; set; }
        public IEnumerable<PregnancyData> PregnancyDataList { get; set; }
        public Delivery Delivery { get; set; }
        public Child Child { get; set; }
        public IEnumerable<DrugsAdministered> MotherDrugsAdministered { get; set; }
        public IEnumerable<DrugsAdministered> BabyDrugsAdministered { get; set; }
        public MotherPreventiveService MotherPreventiveService { get; set; }
        public IEnumerable<MotherPreventiveService> MotherPreventiveServices { get; set; }
        public FamilyPlanning FamilyPlanning { get; set; }
        public IEnumerable<FamilyPlanning> FamilyPlannings { get; set; }

        public PostNatalExamination PostNatalExamination { get; set; }
        public IEnumerable<PostNatalExamination> PostNatalExaminations { get; set; }
        public PostNatalExaminationDetail PostNatalExaminationDetail { get; set; }
        public IEnumerable<PostNatalExaminationDetail> PostNatalExaminationDetails { get; set; }

        public CivilRegistration CivilRegistration { get; set; }

        public ChildHealthRecord HealthRecord { get; set; }

        public ChildFeedingInformation ChildFeedingInformation { get; set; }

        public IEnumerable<PMTCTService> PMTCTServices { get; set; }
        public ICollection<PMTCTIntervention> PMTCTInterventions { get; set; }
        public PMTCTIntervention PMTCTIntervention { get; set; }
        public IEnumerable<Child> Children { get; set; }
        public IEnumerable<ChildHealthRecord> HealthRecords { get; set; }
        public IEnumerable<CivilRegistration> CivilRegistrations { get; set; }
        public IEnumerable<ChildFeedingInformation> FeedingInformation { get; set; }
    }

    public class PMTCTInterventionViewModel
    {
        public string PregnancyId { get; set; }

        [DisplayName("Intervention")]
        public int? InterventionId { get; set; }

        public int? UpdatingInterventionId { get; set; }

        [DisplayName("Recipient"), Required]
        public int? RecipientTypeId { get; set; }

        [DisplayName("Date Started / Date Given"), Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? DateGiven { get; set; }

        public IEnumerable<PMTCTService> PMTCTServices { get; set; }
        public PMTCTIntervention PMTCTIntervention { get; set; }

        [DisplayName("Intervention")]
        public int? PMTCTMotherId { get; set; }

        [DisplayName("Intervention")]
        public int? PMTCTInfantId { get; set; }

        public bool IsUpdating { get; set; }
        public List<int> InterventionIds { get; set; }

        //   public SystemCodeDetail Intervention { get; set; }
        public int? StatusId { get; set; }
    }

    public class CaseManagementListViewModel
    {
        public IPagedList<Pregnancy> Pregnancies { get; set; }
        public Pregnancy Pregnancy { get; set; }
        public HouseholdReg Household { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }

        [DisplayName("Status")]
        public int? StatusId { get; set; }

        [DisplayName("Sub-County")]
        public int? SubCountyId { get; set; }

        [DisplayName("Ward")]
        public int? WardId { get; set; }

        [DisplayName("Mother's Unique ID")]
        public string UniqueId { get; set; }

        [DisplayName("Identification Document No")]
        public string IdNumber { get; set; }

        [DisplayName("Phone")]
        public string Phone { get; set; }

        [DisplayName("Mother's Name")]
        public string Name { get; set; }

        public ICollection<Ward> Wards { get; set; }

        [DisplayName("Heath-Facility")]
        public int? HealthFacilityID { get; set; }

        public int ReminderOffset { get; set; }
    }

    public class SchedularViewModel
    {
        public string Id { get; set; }

        public string HouseholdId { get; set; }
        public DateTime? NextVisit { get; set; }
        public int? NextVisitClinicId { get; set; }
        public string CommonName { get; set; }
        public string FirstName { get; set; }
        public string Phone { get; set; }
    }
}