using Newtonsoft.Json;
using SQLite;
using System.Collections.Generic;
using System.Text;

namespace MCHMIS.Mobile.Database
{
    public class Registration
    {
        [PrimaryKey, Unique, AutoIncrement]
        public int Id { get; set; }

        public string ParentId { get; set; }

        public int UniqueId { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string RegistrationDate { get; set; }
        public int WardId { get; set; }

        [Ignore]
        public Ward Ward { get; set; }

        public int? VillageId { get; set; }

        [Ignore]
        public Village Village { get; set; }

        public int? CommunityAreaId { get; set; }

        [Ignore]
        public CommunityArea CommunityArea { get; set; }

        public int ResidenceDurationYears { get; set; }
        public int ResidenceDurationMonths { get; set; }
        public int EnumeratorId { get; set; }

        [Ignore]
        public Enumerator Enumerator { get; set; }

        public string Phone { get; set; }
        public string MotherId { get; set; }

        public int HouseholdMembers { get; set; }
        public int? StatusId { get; set; }

        [Ignore]
        public SystemCodeDetail Status { get; set; }

        public string CommonName { get; set; }
        // public string Village { get; set; }

        public string PhysicalAddress { get; set; }
        public string NearestReligiousBuilding { get; set; }
        public string NearestSchool { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public int? SyncEnumeratorId { get; set; }

        public string MiddleName { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }

        public string NOKMiddleName { get; set; }
        public string NOKFirstName { get; set; }
        public string NOKSurname { get; set; }
        public string NOKPhone { get; set; }

        public string DateOfBirth { get; set; }
        public int? GenderId { get; set; }
        public int IdentificationFormId { get; set; }
        public string IdNumber { get; set; }
        public string MothersFullName => $"{FirstName} {MiddleName} {Surname}";
        public string NOKsFullName => $"{NOKFirstName} {NOKMiddleName} {NOKSurname}";

        [Ignore]
        public Enumerator SyncEnumerator { get; set; }

        [Ignore, JsonIgnore]
        public string FormNo => $"Form No: #{UniqueId}".ToUpper();

        [Ignore, JsonIgnore]
        public string ReferenceNo => $"Reference No: #{Id}".ToUpper();

        [Ignore, JsonIgnore]
        public string WardName => $"{Ward?.Name}";

        [Ignore, JsonIgnore]
        public string VillageName => $"{Village?.Name}";

        [Ignore, JsonIgnore]
        public string CommunityAreaName => $"{CommunityArea?.Name}";

        [Ignore, JsonIgnore]
        public string DurationDisplay => $"{ResidenceDurationYears} Years, {ResidenceDurationMonths} Months";

        [Ignore, JsonIgnore]
        public string GeoPosition => $"[ {Longitude} , {Latitude} ]";

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
        public int? HasBeenInImarishaAfyaId { get; set; }
        public int? BenefitFromFriendsRelativeId { get; set; }
        public int? InterviewStatusId { get; set; }
        public int? InterviewResultId { get; set; }

        public string DownloadDate { get; set; }

        [Ignore]
        public ICollection<RegistrationMember> RegistrationMembers { get; set; }

        [Ignore]
        public ICollection<RegistrationMemberDisability> RegistrationMemberDisabilities { get; set; }

        [Ignore]
        public ICollection<RegistrationProgramme> RegistrationProgrammes { get; set; }

        public string RegDate1 { get; set; }
        public string RegDate2 { get; set; }
        public string RegDate3 { get; set; }

        private const string delimiter = " ";
        private string haystack;

        [Newtonsoft.Json.JsonIgnore]
        public string Haystack
        {
            get
            {
                if (haystack != null)
                {
                    return haystack;
                }

                var builder = new StringBuilder();
                builder.Append(delimiter);
                builder.Append(Id);
                builder.Append(delimiter);
                // builder.Append(Village);
                builder.Append(delimiter);
                builder.Append(PhysicalAddress);
                builder.Append(delimiter);
                builder.Append(NearestReligiousBuilding);
                builder.Append(delimiter);
                builder.Append(NearestSchool);
                haystack = builder.ToString();
                return haystack;
            }
        }

        [Ignore, JsonIgnore]
        public SystemCodeDetail BenefitFromFriendsRelative { get; set; }

        [Ignore, JsonIgnore]
        public SystemCodeDetail HasBeenInImarishaAfya { get; set; }

        [Ignore, JsonIgnore]
        public SystemCodeDetail IsTelevision { get; set; }

        [Ignore, JsonIgnore]
        public SystemCodeDetail IsMotorcycle { get; set; }

        [Ignore, JsonIgnore]
        public SystemCodeDetail IsTukTuk { get; set; }

        [Ignore, JsonIgnore]
        public SystemCodeDetail IsRefrigerator { get; set; }

        [Ignore, JsonIgnore]
        public SystemCodeDetail IsCar { get; set; }

        [Ignore, JsonIgnore]
        public SystemCodeDetail IsMobilePhone { get; set; }

        [Ignore, JsonIgnore]
        public SystemCodeDetail IsBicycle { get; set; }

        [Ignore, JsonIgnore]
        public SystemCodeDetail IsSkippedMeal { get; set; }

        [Ignore, JsonIgnore]
        public SystemCodeDetail IsReceivingSocial { get; set; }

        [Ignore, JsonIgnore]
        public SystemCodeDetail IsReceivingFamily { get; set; }

        [Ignore, JsonIgnore]
        public SystemCodeDetail InKindBenefit { get; set; }

        [Ignore, JsonIgnore]
        public SystemCodeDetail WasteDisposalMode { get; set; }

        [Ignore, JsonIgnore]
        public SystemCodeDetail WaterSource { get; set; }

        [Ignore, JsonIgnore]
        public SystemCodeDetail WallConstructionMaterial { get; set; }

        [Ignore, JsonIgnore]
        public SystemCodeDetail IsOwned { get; set; }

        [Ignore, JsonIgnore]
        public SystemCodeDetail TenureStatus { get; set; }

        [Ignore, JsonIgnore]
        public SystemCodeDetail RoofConstructionMaterial { get; set; }

        [Ignore, JsonIgnore]
        public SystemCodeDetail NsnpProgrammes { get; set; }

        [Ignore, JsonIgnore]
        public SystemCodeDetail OtherProgrammes { get; set; }

        [Ignore, JsonIgnore]
        public SystemCodeDetail LightingFuelType { get; set; }

        [Ignore, JsonIgnore]
        public SystemCodeDetail HouseHoldCondition { get; set; }

        [Ignore, JsonIgnore]
        public SystemCodeDetail FloorConstructionMaterial { get; set; }

        [Ignore, JsonIgnore]
        public SystemCodeDetail DwellingUnitRisk { get; set; }

        [Ignore, JsonIgnore]
        public SystemCodeDetail CookingFuelType { get; set; }

        [Ignore, JsonIgnore]
        public SystemCodeDetail BenefitType { get; set; }

        [Ignore, JsonIgnore]
        public SystemCodeDetail FamilyBenefitType { get; set; }

        [Ignore, JsonIgnore]
        public SystemCodeDetail InterviewStatus { get; set; }

        [Ignore]
        public SystemCodeDetail InterviewResult { get; set; }

        public int? CannotFindHouseholdReasonId { get; set; }
    }
}