using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MCHMIS.Models;

namespace MCHMIS.ViewModels
{
    public class PMTViewModel
    {
        public string Id { get; set; }

        public byte ProgrammeId { get; set; }

        [DisplayName("(1.01) County")]
        public int CountyId { get; set; }

        [DisplayName("(1.02) Sub-County")]
        public int SubCountyId { get; set; }

        [DisplayName("(1.03) Constituency")]
        public int ConstituencyId { get; set; }

        [DisplayName("(1.04) Location")]
        public int LocationId { get; set; }

        [DisplayName("(1.05) Sub-Location")]
        public int SubLocationId { get; set; }

        [DisplayName("(1.06) Village")]
        public int? VillageId { get; set; }

        [DisplayName("(1.06) PHYSICAL ADDRESS")]
        public string PhysicalAddress { get; set; }

        [DisplayName("(1.08) NEAREST CHURCH/MOSQUE")]
        public string NearestReligiousBuilding { get; set; }

        [DisplayName("(1.09) NEAREST SCHOOL")]
        public string NearestSchool { get; set; }

        [DisplayName("(1.07) Duration of residence in this place: Years")]
        public int? ResidenceDurationYears { get; set; }

        [DisplayName("Months")]
        public int? ResidenceDurationMonths { get; set; }

        [DisplayName("Village")]
        public string Village { get; set; }

        public string HouseholdId { get; set; }

        public string CVListDetailsId { get; set; }

        [Required(ErrorMessage = "* Required")]
        public int? HabitableRoomsNo { get; set; }

        [Required(ErrorMessage = "* Required")]
        public bool IsOwnHouse { get; set; }

        [Required(ErrorMessage = "* Required")]
        public int TenureStatusId { get; set; }

        public string TenureStatusOther { get; set; }

        [Required(ErrorMessage = "* Required")]
        public int RoofMaterialId { get; set; }

        public string RoofMaterialOther { get; set; }

        [Required(ErrorMessage = "* Required")]
        public int WallMaterialId { get; set; }

        public string WallMaterialOther { get; set; }

        [Required(ErrorMessage = "* Required")]
        public int FloorMaterialId { get; set; }

        public string FloorMaterialOther { get; set; }

        [Required(ErrorMessage = "* Required")]
        public int UnitRiskId { get; set; }

        public string UnitRiskOther { get; set; }

        [Required(ErrorMessage = "* Required")]
        public int WaterSourceId { get; set; }

        public string WaterSourceOther { get; set; }

        [Required(ErrorMessage = "* Required")]
        public int ToiletTypeId { get; set; }

        public string ToiletTypeOther { get; set; }

        [Required(ErrorMessage = "* Required")]
        public int CookingFuelSourceId { get; set; }

        public string CookingFuelSourceOther { get; set; }

        [Required(ErrorMessage = "* Required")]
        public int LightingSourceId { get; set; }

        public string LightingSourceOther { get; set; }

        public int[] HouseholdAssets { get; set; }
        public List<HouseholdRegAsset> HouseholdLivestock { get; set; }

        [Required(ErrorMessage = "* Required")]
        public int HouseholdConditionId { get; set; }

        [Required(ErrorMessage = "* Required")]
        public int? LiveBirths { get; set; }

        [Required(ErrorMessage = "* Required")]
        public int? Deaths { get; set; }

        [Required(ErrorMessage = "* Required")]
        public int HasSkippedMealId { get; set; }

        [Required(ErrorMessage = "* Required")]
        public bool IsRecievingNSNPBenefit { get; set; }

        [Required(ErrorMessage = "* Required")]
        public int? IsReceivingOtherBenefitId { get; set; }

        public int[] OtherSPProgrammes { get; set; }

        public string OtherProgrammes { get; set; }

        [Required(ErrorMessage = "* Required")]
        public int? OtherProgrammesBenefitTypeId { get; set; }

        public decimal? OtherProgrammesBenefitAmount { get; set; }

        public string OtherProgrammesInKindBenefit { get; set; }

        [Required(ErrorMessage = "* Required")]
        public int? BenefitFromFriendsRelativeId { get; set; }

        public ICollection<SystemCode> SystemCodes { get; set; }
        public ICollection<HouseholdRegAsset> Assets { get; set; }

        public ICollection<Location> Locations { get; set; }
        public ICollection<SubLocation> SubLocations { get; set; }
        public ICollection<Village> Villages { get; set; }
        public ICollection<Ward> Wards { get; set; }
    }
}