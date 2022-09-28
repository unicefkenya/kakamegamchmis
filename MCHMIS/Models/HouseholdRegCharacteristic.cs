using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCHMIS.Models
{
    public class HouseholdRegCharacteristic
    {
        [Key]
        public string HouseholdId { get; set; }

        [DisplayName("No. of Habitable Rooms")]
        public int? HabitableRoomsNo { get; set; }

        public bool IsOwnHouse { get; set; }

        [DisplayName("Tenure Status")]
        public int TenureStatusId { get; set; }

        [Required(ErrorMessage = "* Required")]
        public int? TenureOwnerOccupiedId { get; set; }

        public SystemCodeDetail TenureOwnerOccupied { get; set; }

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
        public bool IsRecievingNSNPBenefit { get; set; }

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
}