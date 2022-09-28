using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MCHMIS.Models;

namespace MCHMIS.ViewModels
{
    public class HouseholdDwellingViewModel
    {
        public string Id { get; set; } // Household Xtics Id
        public string HouseholdId { get; set; }

        public string CVListDetailsId { get; set; }

        [Required(ErrorMessage = "* Required")]
        public int? HabitableRoomsNo { get; set; }

        [Required(ErrorMessage = "* Required")]
        public bool IsOwnHouse { get; set; }



        [DisplayName("Tenure Status")]
        public int TenureStatusId { get; set; }

        [Required(ErrorMessage = "* Required")]
        public int? TenureOwnerOccupiedId { get; set; }
        public SystemCodeDetail TenureOwnerOccupied { get; set; }


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

        public IEnumerable<int> HouseholdAssets { get; set; }
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

        public List<int> OtherSPProgrammes { get; set; }

        public string OtherProgrammes { get; set; }

        [Required(ErrorMessage = "* Required")]
        public int? OtherProgrammesBenefitTypeId { get; set; }

        public decimal? OtherProgrammesBenefitAmount { get; set; }

        public string OtherProgrammesInKindBenefit { get; set; }

        [Required(ErrorMessage = "* Required")]
        public int? BenefitFromFriendsRelativeId { get; set; }

        public ICollection<SystemCode> SystemCodes { get; set; }
        public ICollection<HouseholdRegAsset> Assets { get; set; }
        
    }

    public class DashboardViewModel
    {
        public int OpenComplaints { get; set; }
        public int ActionedComplaints { get; set; }
        public int ResolvedComplaints { get; set; }
        public int ClosedComplaints { get; set; }

        public int PendingUpdates { get; set; }
        public int ApprovedUpdates { get; set; }
        public int ActionedUpdates { get; set; }
        public int RejectedUpdates { get; set; }
    }
}