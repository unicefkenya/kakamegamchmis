using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCHMIS.Models
{
    public class HouseholdReg
    {
        public string Id { get; set; }

        [DisplayName("Mother Unique Id")]
        public string UniqueId { get; set; }

        public string OldUniqueId { get; set; }

        public int? SubLocationId { get; set; }
        public SubLocation SubLocation { get; set; }

        [DisplayName("Village")]
        public int? VillageId { get; set; }

        public Village Village { get; set; }

        [DisplayName("Community Area")]
        public int? CommunityAreaId { get; set; }

        public int? WardId { get; set; }
        public Ward Ward { get; set; }
        public string TempWard { get; set; }

        public CommunityArea CommunityArea { get; set; }

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

        [DisplayName("Status")]
        public int? ReasonId { get; set; }

        public Reason Reason { get; set; }

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

        [DisplayName("Mother's Common Name")]
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

        [DisplayName("Next of Kin National ID number")]
        public string NOKIdNumber { get; set; }

        public bool? HasProxy { get; set; }

        [DisplayName("Health Facility")]
        public int HealthFacilityId { get; set; }

        public HealthFacility HealthFacility { get; set; }

        public string SupportingDocument { get; set; }
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
        public byte[] RawFingerPrint { get; set; }

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

        public Programme Programme { get; set; }

        public ICollection<HouseholdRegAsset> HouseholdRegAssets { get; set; }

        public ICollection<HouseholdRegMember> HouseholdRegMembers { get; set; }

        public ICollection<HouseholdRegOtherProgramme> HouseholdRegOtherProgrammes { get; set; }

        public ICollection<Notes> Notes { get; set; }

        [DisplayName("PMT Score")]
        [Column(TypeName = "decimal(18, 10)")]
        public decimal? PMTScore { get; set; }

        [DisplayName("PMT Score")]
        [Column(TypeName = "decimal(18, 10)")]
        public decimal? PMTScoreFinal { get; set; }

        [DisplayName("LCS Score")]
        [Column(TypeName = "decimal(18, 10)")]
        public decimal? LCS { get; set; }

        [DisplayName("LCS Score")]
        [Column(TypeName = "decimal(18, 10)")]
        public decimal? LCSFinal { get; set; }
        public decimal? PMTCutOffUsed { get; set; }
        public int? TypeId { get; set; }
        public string ParentId { get; set; }
        public bool? RequiresHealthVerification { get; set; }

        public bool RequiresIPRSECheck { get; set; }

        [DisplayName("IPRS Verified?")]
        public bool IPRSVerified { get; set; }

        [DisplayName("IPRS Verification Passed?")]
        public bool? IPRSPassed { get; set; }

        public int? IPRSExceptionId { get; set; }

        public SystemCodeDetail IPRSException { get; set; }
        public int? ExitReasonId { get; set; }
        public SystemCodeDetail ExitReason { get; set; }
        public bool? RequiresMPESACheck { get; set; }

        [DisplayName("Date Registered"), DataType(DataType.Text)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime DateCreated { get; set; }

        [DisplayName("Created By"), Required]
        public string CreatedById { get; set; }

        //[DisplayName("Created By")]
        //public virtual ApplicationUser CreatedBy { get; set; }

        [DisplayName("Date Approved")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? DateApproved { get; set; }

        [DisplayName("Approved By")]
        public string ApprovedById { get; set; }

        [DisplayName("Approved By")]
        public ApplicationUser ApprovedBy { get; set; }

        public string BlackListReason { get; set; }

        public bool IsBeneficiary
        {
            get
            {
                // Non Resident, Residence Less than 6 Months, Exited, Repeat Case
                if (StatusId == 20 || StatusId == 23 || StatusId == 27 || StatusId == 22)
                    return false;
                return true;
            }
        }
    }
}