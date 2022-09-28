using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MCHMIS.Models;
using X.PagedList;

namespace MCHMIS.ViewModels
{
    public class FRSummaryViewModel
    {
        public int Id { get; set; }
        public int HealthFacilityId { get; set; }
        public string Facility { get; set; }
        public string SubCounty { get; set; }
        public string Ward { get; set; }
        public int Beneficiaries { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0}")]
        public decimal Amount { get; set; }
    }

    public class FRApprovalSummaryViewModel
    {
        public int Id { get; set; }
        public int Facilities { get; set; }
        public int Beneficiaries { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0}")]
        public decimal Amount { get; set; }

        public string Notes { get; set; }
        public string Option { get; set; }
        public FundRequest FundRequest { get; set; }
        public Payroll Payroll { get; set; }
    }

    public class PaymentTransactionsViewModel
    {
        public int? Id { get; set; }
        public int? CountyId { get; set; }

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

        public string Option { get; set; }

        public int StatusId { get; set; }

        [DisplayName("Payment Status")]
        public PaymentStatus Status { get; set; }

        public ICollection<Ward> Wards { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public IPagedList<PaymentTransaction> Details { get; set; }
        public PaymentTransaction PaymentTransaction { get; set; }
        public FundRequest FundRequest { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0}")]
        public decimal Total { get; set; }
    }
}