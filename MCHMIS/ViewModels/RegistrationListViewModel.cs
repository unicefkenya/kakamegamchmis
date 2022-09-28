using System.Collections.Generic;
using System.ComponentModel;
using MCHMIS.Models;
using X.PagedList;

namespace MCHMIS.ViewModels
{
    public class RegistrationListViewModel
    {
        public ICollection<HouseholdReg> HouseholdRegs { get; set; }
        public HouseholdReg Household { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }

        [DisplayName("Status")]
        public int? StatusId { get; set; }

        [DisplayName("Sub-County")]
        public int? SubCountyId { get; set; }

        [DisplayName("Ward")]
        public int? WardId { get; set; }

        [DisplayName("Heath-Facility")]
        public int? HealthFacilityID { get; set; }

        [DisplayName("Mother's Unique ID")]
        public string UniqueId { get; set; }

        [DisplayName("Identification Document No")]
        public string IdNumber { get; set; }

        [DisplayName("Any One Name")]
        public string Name { get; set; }

        public ICollection<Ward> Wards { get; set; }

        public ICollection<HealthFacility> HealthFacility { get; set; }

        [DisplayName("Report Type")]
        public int? ReportTypeId { get; set; }
    }

    public class HouseholdsListViewModel
    {
        public IPagedList<HouseholdReg> HouseholdRegs { get; set; }
        public HouseholdReg Household { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }

        [DisplayName("Status")]
        public int? StatusId { get; set; }

        [DisplayName("Sub-County")]
        public int? SubCountyId { get; set; }

        [DisplayName("Ward")]
        public int? WardId { get; set; }

        [DisplayName("Heath-Facility")]
        public int? HealthFacilityId { get; set; }

        [DisplayName("Mother's Unique ID")]
        public string UniqueId { get; set; }

        [DisplayName("Identification Document No")]
        public string IdNumber { get; set; }

        [DisplayName("Mother's Name")]
        public string Name { get; set; }

        public string Phone { get; set; }
        public ICollection<Ward> Wards { get; set; }

        public ICollection<HealthFacility> HealthFacility { get; set; }
        public int AwaitingIPRS { get; set; }
        public string Option { get; set; }
    }

    public class BeneficiaryListViewModel
    {
        public IPagedList<Beneficiary> Beneficiaries { get; set; }
        public Beneficiary Beneficiary { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }

        public int ActiveMothers { get; set; }

        [DisplayName("Status")]
        public int? StatusId { get; set; }

        public string Status { get; set; }

        [DisplayName("Health Facility")]
        public int? HealthFacilityId { get; set; }

        [DisplayName("Sub-County")]
        public int? SubCountyId { get; set; }

        [DisplayName("Ward")]
        public int? WardId { get; set; }

        [DisplayName("Mother's Unique Id")]
        public string UniqueId { get; set; }

        [DisplayName("Identification Document No")]
        public string IdNumber { get; set; }

        [DisplayName("Any One Name")]
        public string Name { get; set; }

        public ICollection<Ward> Wards { get; set; }
    }
}