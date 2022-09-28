using MCHMIS.Models;
using X.PagedList;
using System.Collections.Generic;
using System.ComponentModel;

namespace MCHMIS.Areas.Reports.ViewModels
{
    public class BeneficiaryStatementViewModel
    {
        [DisplayName("Status")]
        public int? StatusId { get; set; }

        [DisplayName("Health Facility")]
        public int? HealthFacilityId { get; set; }

        [DisplayName("Sub-County")]
        public int? SubCountyId { get; set; }

        [DisplayName("Ward")]
        public int? WardId { get; set; }

        public string Id { get; set; }
        public HouseholdReg Household { get; set; }

        public SystemCodeDetail SupportStatus { get; set; }
        public ICollection<HouseholdRegMemberDisability> Disabilities { get; set; }

        public Change Change { get; set; }
        public List<Change> Changes { get; set; }

        public Complaint Complaint { get; set; }
        public List<Complaint> Complaints { get; set; }

        public List<PaymentTransaction> Details { get; set; }
        public PaymentTransaction PaymentTransaction { get; set; }
    }

    public class CommunityValidationListViewModel
    {
        [DisplayName("Health Facility")]
        public int? HealthFacilityId { get; set; }

        [DisplayName("Report Type")]
        public int? ReportTypeId { get; set; }

        public string Id { get; set; }
        public CVList CVList { get; set; }

        public IPagedList<CVListDetail> Details { get; set; }
        public IEnumerable<CVListDetail> DetailList { get; set; }

        public HouseholdReg Household { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public int? Total { get; set; }

        [DisplayName("Status")]
        public int? StatusId { get; set; }

        [DisplayName("Sub-County")]
        public int? SubCountyId { get; set; }

        [DisplayName("Ward")]
        public int? WardId { get; set; }

        [DisplayName("Village")]
        public int? VillageId { get; set; }

        [DisplayName("Community Area")]
        public int? CommunityAreaId { get; set; }

        [DisplayName("Mother's Unique ID")]
        public string UniqueId { get; set; }

        [DisplayName("Identification Document No")]
        public string IdNumber { get; set; }

        [DisplayName("Any One Name")]
        public string Name { get; set; }

        public ICollection<Ward> Wards { get; set; }
        public ICollection<Village> Villages { get; set; }
        public ICollection<CommunityArea> CommunityAreas { get; set; }

        [DisplayName("Heath-Facility")]
        public int? HealthFacilityID { get; set; }

        public HHSummaryViewModel Summary { get; set; }
        public IEnumerable<HHSummaryViewModel> Summaries { get; set; }

        public string Option { get; set; }
    }
}