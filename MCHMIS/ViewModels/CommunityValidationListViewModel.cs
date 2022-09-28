using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MCHMIS.Models;
using X.PagedList;

namespace MCHMIS.ViewModels
{
    public class CVListsViewModel
    {
        public int Id { get; set; }
        public IPagedList<CVList> Lists { get; set; }
        public CVList List { get; set; }

        public int? Page { get; set; }
        public int? PageNo { get; set; }

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

        [DisplayName("Any One Name")]
        public string Name { get; set; }

        public ICollection<Ward> Wards { get; set; }

        [DisplayName("Heath-Facility")]
        public int? HealthFacilityId { get; set; }
        public HealthFacility HealthFacility { get; set; }

        public int? PageSize { get; set; }
    }

    public class CommunityValidationListViewModel
    {
        public string Id { get; set; }
        public CVList CVList { get; set; }
        public ICollection<HouseholdReg> HouseholdRegs { get; set; }
        public CVListDetail Detail { get; set; }
        public IPagedList<CVListDetail> Details { get; set; }
        public IPagedList<CVListDetail> ListDetails { get; set; }
        public HouseholdReg Household { get; set; }
        public int? Page { get; set; }
        [DisplayName("Page Size")]
        public int? PageSize { get; set; }

        [DisplayName("Status")]
        public int? StatusId { get; set; }

        [DisplayName("Variance")]
        public int? VarianceCategoryId { get; set; }

        [DisplayName("Sub-County")]
        public int? SubCountyId { get; set; }

        [DisplayName("Ward")]
        public int? WardId { get; set; }

        [DisplayName("Village")]
        public int? VillageId { get; set; }

        [DisplayName("Community Area")]
        public int? CommunityAreaId { get; set; }

        [DisplayName("Health Facility")]
        public int? HealthFacilityId { get; set; }

        [DisplayName("Unique ID")]
        public string UniqueId { get; set; }

        [DisplayName("ID No")]
        public string IdNumber { get; set; }

        [DisplayName("Any One Name")]
        public string Name { get; set; }
        [DataType(DataType.Text)]
        [DisplayName("Start Date")]
        public DateTime? StartDate { get; set; }
        [DisplayName("End Date")]
        [DataType(DataType.Text)]
        public DateTime? EndDate { get; set; }
        public ICollection<Ward> Wards { get; set; }
        public ICollection<Village> Villages { get; set; }
        public ICollection<CommunityArea> CommunityAreas { get; set; }

        [DisplayName("Heath-Facility")]
        public int? HealthFacilityID { get; set; }

        [DisplayName("CHV")]
        public int? EnumeratorId { get; set; }

        [DisplayName("CHV")]
        public int? SelectedEnumeratorId { get; set; }
    }

    public class CVListExportViewModel
    {
        public string MotherUniqueId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
        public string CommonName { get; set; }
        public string IdNumber { get; set; }

        public string HealthFacility { get; set; }
        public string Phone { get; set; }

        public string SubCounty { get; set; }
        public string Ward { get; set; }
        public string VillageUnit { get; set; }
        public string CommunityArea { get; set; }
        public string CHV { get; set; }
    }

    public class AssignViewModel
    {
        public string Id { get; set; }
        public string[] Ids { get; set; }
        public int EnumeratorId { get; set; }
    }

    public class HouseholdDetailsViewModel
    {
        public string Id { get; set; }
        public CVListDetail Detail { get; set; }
        public HouseholdDwellingViewModel Dwelling { get; set; }
        public HouseholdReg Household { get; set; }
        public HouseholdReg ParentHousehold { get; set; }
        public ICollection<HouseholdRegMember> ParentMembers { get; set; }
        public ICollection<HouseholdRegMember> Members { get; set; }

        public SystemCodeDetail SupportStatus { get; set; }
        public ICollection<HouseholdRegMemberDisability> Disabilities { get; set; }

        public ICollection<HouseholdRegMemberDisability> ParentDisabilities { get; set; }
        public HouseholdRegMember Member { get; set; }
        public HouseholdRegCharacteristic Characteristic { get; set; }
        public HouseholdRegCharacteristic ParentCharacteristic { get; set; }
        public ICollection<HouseholdRegAsset> Assets { get; set; }

        public ICollection<HouseholdRegAsset> ParentAssets { get; set; }
        public string View { get; set; }

        public string Notes { get; set; }

        [DisplayName("I have verified Mother's disability / HIV Status / MUAC. The mother should therefore be enrolled in the programme without undergoing further poverty assessment.")]
        [Required]
        public bool RequiresHealthVerification { get; set; }

        [DisplayName("National Safety Net Programmes that someone in this household is receiving benefits from")]
        public ICollection<HouseholdRegOtherProgramme> OtherProgrammes { get; set; }

        public ICollection<HouseholdRegOtherProgramme> ParentOtherProgrammes { get; set; }

        public int? StatusId { get; set; }
        public bool? AllowedToEnroll { get; set; }
        [DisplayName("Reason")]
        public string AllowedReason { get; set; }
    }

    public class CVListGenerateViewModel
    {
        [DisplayName("No of mother to be generated"), Required]
        public int? NoToGenerate { get; set; }

        [DisplayName("Sub-County")]
        public int? SubCountyId { get; set; }

        [DisplayName("Ward")]
        public int? WardId { get; set; }

        public ICollection<Ward> Wards { get; set; }
        public int Ready { get; set; }
        public int Total { get; set; }
        public int NotIprsed { get; set; }
        public int NotMatched { get; set; }
        public string Notes { get; set; }

        public IList<int> StatusIds { get; set; }
    }
    public class TopupsViewModel
    {
        [DisplayName("No of mother to be generated"), Required]
        public int? NoToGenerate { get; set; }
        [DisplayFormat(DataFormatString = "{0:###,##0}")]
        public int Eligible { get; set; }
        [DisplayName("In Community Validation")]
        [DisplayFormat(DataFormatString = "{0:###,##0}")]
        public int InCommunityValidation { get; set; }
        [DisplayName("Ready for Enrolment")]
        [DisplayFormat(DataFormatString = "{0:###,##0}")]
        public int ReadyForEnrolment { get; set; }
        [DisplayName("Recommended for M&E Validation")]
        [DisplayFormat(DataFormatString = "{0:###,##0}")]
        public int RecommendedForME{ get; set; }
        [DisplayFormat(DataFormatString = "{0:###,##0}")]
        public int Total { get; set; }
        [DisplayFormat(DataFormatString = "{0:###,##0}")]
        public int Ineligible { get; set; }
        public string Notes { get; set; }
    }

    public class CommunityValidationApprovalsViewModel
    {
        public string Id { get; set; }
        public CVList List { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        public int Action { get; set; }
    }
}