using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCHMIS.Models
{
    public class CaseManagement : CreateFields
    {
        [Key]
        public string Id { get; set; }

        public string HouseholdId { get; set; }
        public HouseholdReg Household { get; set; }

        [DisplayName("Status")]
        public int? StatusId { get; set; }

        [DisplayName("Exit Reason")]
        public int? ReasonId { get; set; }

        public Reason Reason { get; set; }

        [DisplayName("Date Exited")]
        public DateTime? DateExited { get; set; }

        public CaseManagementStatus Status { get; set; }

        [DisplayName("Last Visit"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? LastVisit { get; set; }

        [DisplayName("Next Visit"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? NextVisit { get; set; }

        public int? NextVisitClinicId { get; set; }

        public int MissedVisits { get; set; }
    }

    public class CaseManagementStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Pregnancy : CreateModifyFields
    {
        [Key]
        public string Id { get; set; }

        public string CaseManagementId { get; set; }
        public CaseManagement CaseManagement { get; set; }

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

        [DisplayName("Infant feeding counseling done")]
        public int? InfantFeedingCounselingDoneId { get; set; }

        public SystemCodeDetail InfantFeedingCounselingDone { get; set; }

        [DisplayName("Counseling on exclusive breastfeeding done ")]
        public int? BreastfeedingCounselingDoneId { get; set; }

        public SystemCodeDetail BreastfeedingCounselingDone { get; set; }

        [DisplayName("Status")]
        public int? StatusId { get; set; }

        public CaseManagementStatus Status { get; set; }

        [DisplayName("Exit Reason")]
        public int? ReasonId { get; set; }

        public Reason Reason { get; set; }

        [DisplayName("Date Exited"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? DateExited { get; set; }

        [DisplayName("Last Visit"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? LastVisit { get; set; }

        [DisplayName("Next Visit"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? NextVisit { get; set; }

        public int? NextVisitClinicId { get; set; }

        public int MissedVisits { get; set; }

        public int NextVisitDuration
        {
            get
            {
                if (NextVisit != null && LastVisit != null)
                    return ((DateTime)NextVisit).Subtract(((DateTime)DateTime.UtcNow)).Days;
                else
                {
                    return 8;
                }
            }
        }

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

    public class MotherClinicVisit : CreateFields
    {
        public string Id { get; set; }
        public string HouseholdId { get; set; }

        public CaseManagement CaseManagement { get; set; }
        public string PregnancyId { get; set; }

        public Pregnancy Pregnancy { get; set; }
        public int ClinicVisitId { get; set; }

        public ClinicVisit ClinicVisit { get; set; }
        public int TypeId { get; set; }
        public int? HealthFacilityId { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? VisitDate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? DueDate { get; set; }

        [DisplayName("Requires Fingerprint Verification?")]
        public bool RequiresFingerPrint { get; set; }

        [DisplayName("Fingerprint Verified?")]
        public bool Verified { get; set; }

        public bool IsVerifying { get; set; }

        //  public int? StatusId { get; set; }
    }

    public class PregnancyData : CreateFields
    {
        [Key]
        public string Id { get; set; }

        public string PregnancyId { get; set; }
        public Pregnancy Pregnancy { get; set; }

        [DisplayName("Clinic Visit")]
        public int? ClinicVisitId { get; set; }

        public ClinicVisit ClinicVisit { get; set; }

        public string HouseholdId { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? VisitDate { get; set; }

        public string MotherClinicVisitId { get; set; }
        public MotherClinicVisit MotherClinicVisit { get; set; }

        public decimal? Weight { get; set; }
        public string BloodPressure { get; set; }
        public ICollection<MotherPreventiveService> MotherPreventiveServices { get; set; }

        [DisplayName("Next Visit"), Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? NextVisit { get; set; }

        public string Notes { get; set; }
    }

    public class DrugsAdministered
    {
        public int Id { get; set; }
        public string DeliveryId { get; set; }
        public int DrugId { get; set; }
        public SystemCodeDetail Drug { get; set; }
        public int RecipientTypeId { get; set; }
        public string RecipientId { get; set; }
    }

    public class MotherPreventiveService
    {
        [Key, Column(Order = 1)]
        public string PregnancyDataId { get; set; }

        public PregnancyData PregnancyData { get; set; }

        [DisplayName("Preventive Service")]
        [Key, Column(Order = 2)]
        public int PreventiveServiceId { get; set; }

        public SystemCodeDetail PreventiveService { get; set; }
    }

    public class Delivery : CreateFields
    {
        public string Id { get; set; }
        public string PregnancyId { get; set; }
        public Pregnancy Pregnancy { get; set; }

        [DisplayName("Pregnancy Outcome")]
        public int? PregnancyOutcomeId { get; set; }

        public SystemCodeDetail PregnancyOutcome { get; set; }

        [DisplayName("Pregnancy Duration")]
        public int? PregnancyDuration { get; set; }

        public SystemCodeDetail HIVTested { get; set; }
        public int? SupportStatusId { get; set; }
        public SystemCodeDetail SupportStatus { get; set; }

        [DisplayName("Mode of Delivery")]
        public int? DeliveryModeId { get; set; }

        public SystemCodeDetail DeliveryMode { get; set; }

        [DisplayName("Date of delivery")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime DeliveryDate { get; set; }

        [DisplayName("Blood Loss")]
        public int? BloodLossId { get; set; }

        public SystemCodeDetail BloodLoss { get; set; }

        //[DisplayName("Pre-eclampsia")]
        //public int PreEclampsiaId { get; set; }

        //public SystemCodeDetail PreEclampsia { get; set; }

        //[DisplayName("Eclampsia")]
        //public int EclampsiaId { get; set; }

        //public SystemCodeDetail Eclampsia { get; set; }

        [DisplayName("Obstructed Labour")]
        public int? ObstructedLabourId { get; set; }

        public SystemCodeDetail ObstructedLabour { get; set; }

        [DisplayName("Condition of mother")]
        public string MotherCondition { get; set; }

        //[DisplayName("Apgar Score")]
        //public string ApgarScore { get; set; }

        [DisplayName("Rescuscitation Done?")]
        public int? RescuscitationDoneId { get; set; }

        public SystemCodeDetail RescuscitationDone { get; set; }

        [DisplayName("Meconium stained liquor (grade)")]
        public int? MeconiumStainedLiquorId { get; set; }

        public SystemCodeDetail MeconiumStainedLiquor { get; set; }

        public string MotherClinicVisitId { get; set; }
        public MotherClinicVisit MotherClinicVisit { get; set; }

        public DateTime? NextVisit { get; set; }
        public string Notes { get; set; }
    }

    public class Child
    {
        public string Id { get; set; }

        [DisplayName("Child Name")]
        public string Name { get; set; }

        [DisplayName("Sex of child")]
        public int? GenderId { get; set; }

        public SystemCodeDetail Gender { get; set; }

        [DisplayName("Date of Birth")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? DOB { get; set; }

        [DisplayName("Date First Seen")]
        public string DateFirstSeen { get; set; }

        public string DeliveryId { get; set; }

        public Delivery Delivery { get; set; }

        [DisplayName("Gestation at birth (in weeks)")]
        public int? GestationAtBirth { get; set; }

        [DisplayName("Birth Weight")]
        public string BirthWeight { get; set; }

        [DisplayName("Birth Length in cm")]
        public string BirthLength { get; set; }

        [DisplayName("Head circumference")]
        public string HeadCircumference { get; set; }

        [DisplayName("Other Birth Characteristics")]
        public string OtherBirthCharacteristics { get; set; }

        [DisplayName("Birth Order in Family (e.g. 1st, 2nd, 3rd born)")]
        public string BirthOrder { get; set; }

        public int? StatusId { get; set; }
        public SystemCodeDetail Status { get; set; }

        [DisplayName("Place of delivery")]
        public int? DeliveryPlaceId { get; set; }

        public SystemCodeDetail DeliveryPlace { get; set; }

        [DisplayName("Delivery Health Facility")]
        public int? DeliveryHealthFacilityId { get; set; }

        public string OtherDeliveryHealthFacility { get; set; }

        public HealthFacility DeliveryHealthFacility { get; set; }

        public string OtherDeliveryPlace { get; set; }

        [DisplayName("Conducted by")]
        public int? DeliveryAssistantId { get; set; }

        public SystemCodeDetail DeliveryAssistant { get; set; }
        public string OtherDeliveryAssistant { get; set; }

        public ChildHealthRecord ChildHealthRecord { get; set; }

        [DisplayName("Immunized?")]
        public int? ImmunizedId { get; set; }

        public SystemCodeDetail Immunized { get; set; }

        public string DisplayName
        {
            get
            {
                if (Name != null)
                    return Name;
                return BirthOrder;
            }
        }
    }

    public class FamilyPlanning : CreateFields
    {
        public string Id { get; set; }

        public string CaseManagementId { get; set; }
        public string PregnancyId { get; set; }
        public Pregnancy Pregnancy { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        [DisplayName("Date")]
        public DateTime VisitDate { get; set; }

        [DisplayName("Clinical Notes")]
        public string Notes { get; set; }

        [DisplayName("Next Visit")]
        public DateTime? NextVisit { get; set; }
    }

    public class PostNatalExamination : CreateFields
    {
        public string Id { get; set; }

        public string CaseManagementId { get; set; }
        public string PregnancyId { get; set; }
        public Pregnancy Pregnancy { get; set; }

        [DisplayName("Visit Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime VisitDate { get; set; }

        [DisplayName("Next Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? NextDate { get; set; }

        [DisplayName("PNC Visit")]
        public int ClinicVisitId { get; set; }

        public ClinicVisit ClinicVisit { get; set; }
        public string BloodPressure { get; set; }
        public string GeneralCondition { get; set; }

        [DisplayName("Counseling on family planning done?")]
        public int? FPCounselingId { get; set; }

        public SystemCodeDetail FPCounseling { get; set; }

        [DisplayName("HIV Status")]
        public int? SupportStatusId { get; set; }

        public SystemCodeDetail SupportStatus { get; set; }

        [DisplayName("FP Method")]
        public int? FPMethodId { get; set; }

        public SystemCodeDetail FPMethod { get; set; }
        public IEnumerable<PostNatalExaminationDetail> Details { get; set; }

        public string MotherClinicVisitId { get; set; }
        public MotherClinicVisit MotherClinicVisit { get; set; }
        public string Notes { get; set; }
    }

    public class PostNatalExaminationDetail
    {
        [Key]
        public string Id { get; set; }

        public string ChildId { get; set; }
        public Child Child { get; set; }

        public string PostNatalExaminationId { get; set; }

        public PostNatalExamination PostNatalExamination { get; set; }

        [DisplayName("Baby’s general condition well…unwell…")]
        public string BabyGeneralCondition { get; set; }

        [DisplayName("Baby’s feeding method")]
        public int FeedingMethodId { get; set; }

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
        public ICollection<PostNatalMilestone> PostNatalMilestones { get; set; }
    }

    public class PostNatalMilestone
    {
        [Key, Column(Order = 1)]
        public string PostNatalExaminationDetailId { get; set; }

        [Key, Column(Order = 2)]
        public int KeyMilestoneId { get; set; }

        public KeyMilestone KeyMilestone { get; set; }
    }

    public class ChildHealthRecord : CreateFields
    {
        [Key]
        public string ChildId { get; set; }

        public string CaseManagementId { get; set; }
        public string PregnancyId { get; set; }
        public Pregnancy Pregnancy { get; set; }

        [DisplayName("Place of delivery"), Required]
        public int? DeliveryPlaceId { get; set; }

        public SystemCodeDetail DeliveryPlace { get; set; }
        public string OtherDeliveryPlace { get; set; }

        [DisplayName("Health Facility Name")]
        public int? HealthFacilityId { get; set; }

        public HealthFacility HealthFacility { get; set; }
        public string HealthFacilityName { get; set; }

        [DisplayName("Birth Notification Number")]
        public string BirthNotificationNumber { get; set; }

        [DisplayName("Permanent Register Number")]
        public string PermanentRegisterNumber { get; set; }

        [DisplayName("Child Welfare Clinic (CWC) Number")]
        public string CWCNumber { get; set; }

        [DisplayName("Master Facility List  (MFL) Number"), Required]
        public string MFLNumber { get; set; }
    }

    public class ChildFeedingInformation
    {
        [Key]
        public string ChildId { get; set; }

        public string CaseManagementId { get; set; }
        public string PregnancyId { get; set; }
        public Pregnancy Pregnancy { get; set; }

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

    public class CivilRegistration
    {
        [Key]
        public string ChildId { get; set; }

        public string CaseManagementId { get; set; }
        public string PregnancyId { get; set; }
        public Pregnancy Pregnancy { get; set; }

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

    public class PMTCTService : CreateFields
    {
        public string Id { get; set; }

        public string CaseManagementId { get; set; }
        public string PregnancyId { get; set; }

        public Pregnancy Pregnancy { get; set; }
        public int RecipientTypeId { get; set; }

        [DisplayName("Date Started / Date Given"), Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime DateGiven { get; set; }

        public ICollection<PMTCTIntervention> PMTCTInterventions { get; set; }
    }

    public class PMTCTIntervention
    {
        [Key, Column(Order = 1)]
        public string PMTCTServiceId { get; set; }

        public PMTCTService PMTCTService { get; set; }

        [Key, Column(Order = 2)]
        [DisplayName("Intervention")]
        public int InterventionId { get; set; }

        public SystemCodeDetail Intervention { get; set; }
    }

    public class KeyMilestone
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }

        [DisplayName("Clinic Visit")]
        public int? ClinicVisitId { get; set; }

        public ClinicVisit ClinicVisit { get; set; }
    }

    public class DevelopmentMilestone
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NormalLimits { get; set; }
        public int UpperLimit { get; set; }
        public string LimitTag { get; set; }
    }

    public class ChildDevelopmentMilestone
    {
        [Key, Column(Order = 1)]
        public string ChildId { get; set; }

        [Key, Column(Order = 2)]
        public int DevelopmentMilestoneId { get; set; }

        public int AgeAchieved { get; set; }
    }
}