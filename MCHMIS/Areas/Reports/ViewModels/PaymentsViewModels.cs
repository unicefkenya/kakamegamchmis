using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MCHMIS.Models;
using MCHMIS.ViewModels;
using X.PagedList;

namespace MCHMIS.Areas.Reports.ViewModels
{
    public class PaymentsViewModel
    {
        public string Id { get; set; }

        [DisplayName("Payroll")]
        public int? PayrollId { get; set; }

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

        [DisplayName("Payment Stage")]
        public int? PaymentStageId { get; set; }

        [DisplayName("Report Type")]
        public int? ReportTypeId { get; set; }

        public ICollection<Ward> Wards { get; set; }
        public Payroll Payroll { get; set; }
        public ICollection<Payroll> Payrolls { get; set; }

        public IPagedList<Payment> Details { get; set; }
        public IPagedList<BeneficiaryPaymentPoint> BeneficiaryPaymentPoints { get; set; }
        public Payment Payment { get; set; }

        public DisbursementViewModel Disbursement { get; set; }
        public IEnumerable<DisbursementViewModel> Disbursements { get; set; }
        public IEnumerable<ForecastViewModel> ForecastSummary { get; set; }
        public ForecastViewModel Forecast { get; set; }
        public PaymentTransaction PaymentTransaction { get; set; }
        public IPagedList<PaymentTransaction> PaymentTransactions { get; set; }
        public ViewReportsDisbursements DisbursementDetail { get; set; }
        public IPagedList<ViewReportsDisbursements> DisbursementDetails { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0}")]
        public decimal Total { get; set; }

        public string Option { get; set; }

        public PendingPaymentsViewModel Pending { get; set; }
        public IPagedList<PendingPaymentsViewModel> PendingPayments { get; set; }
        public IPagedList<ForecastDetailViewModel> ForecastDetails { get; set; }
    }

    public class PendingPaymentsViewModel
    {
        public string UniqueId { get; set; }
        public string BeneficiaryName { get; set; }
        public string RecipientPhone { get; set; }
        public string RecipientName { get; set; }
        public string HealthFacility { get; set; }
        public string SubCounty { get; set; }
        public string Ward { get; set; }
        public string Village { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0}")]
        public int Amount { get; set; }
    }

    public class ForecastDetailViewModel
    {
        public string UniqueId { get; set; }
        public string BeneficiaryName { get; set; }
        public string RecipientPhone { get; set; }
        public string RecipientName { get; set; }
        public string HealthFacility { get; set; }
        public string SubCounty { get; set; }
        public string Ward { get; set; }
        public string Village { get; set; }
        public string PayPoint { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime DueDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0}")]
        public decimal Amount { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime EDD { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime DateCreated { get; set; }
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