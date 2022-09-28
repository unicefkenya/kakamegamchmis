using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCHMIS.Models
{
    public class Payroll : CreateApprovalFields
    {
        public int Id { get; set; }

        public int FundRequestId { get; set; }
        public FundRequest FundRequest { get; set; }

        public int CycleId { get; set; }
        public PaymentCycle Cycle { get; set; }

        public bool Reconciled { get; set; }

        [DisplayName("Date Reconciled")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? DateReconciled { get; set; }

        [DisplayName("Reconciled By")]
        public string ReconciledById { get; set; }

        public ApplicationUser ReconciledBy { get; set; }

        [DisplayName("Date Reconciled")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? ReconciliationDateApproved { get; set; }

        [DisplayName("Reconciliation Approved By")]
        public string ReconciliationApprovedById { get; set; }

        public ApplicationUser ReconciliationApprovedBy { get; set; }
        public int? ReconciliationStatusId { get; set; }
        public ApprovalStatus ReconciliationStatus { get; set; }
        public string ReconciliationNotes { get; set; }
        public ICollection<Payment> Payments { get; set; }

        public int StatusId { get; set; }
        public ApprovalStatus Status { get; set; }
        public string Notes { get; set; }

        // Summaries
        [DisplayFormat(DataFormatString = "{0:###,##0}")]
        public decimal Amount { get; set; }

        [DisplayName("No. of Beneficiaries")]
        public int Beneficiaries { get; set; }
    }

    public class Payment
    {
        public int Id { get; set; }

        public int PayrollId { get; set; }
        public Payroll Payroll { get; set; }

        public int FundRequestId { get; set; }
        public FundRequest FundRequest { get; set; }

        public int CycleId { get; set; }
        public PaymentCycle Cycle { get; set; }
        public string BeneficiaryId { get; set; }
        public Beneficiary Beneficiary { get; set; }

        [DisplayName("Beneficiary Name")]
        public string BeneficiaryName { get; set; }

        [DisplayName("ID Number")]
        public string IdNumber { get; set; }

        [DisplayName("Recipient Name")]
        public string RecipientName { get; set; }

        [DisplayName("Recipient Phone")]
        public string RecipientPhone { get; set; }

        public int? VillageId { get; set; }
        public Village Village { get; set; }

        public int? WardId { get; set; }
        public Ward Ward { get; set; }

        [DisplayName("Sub-County")]
        public int? SubCountyId { get; set; }

        public SubCounty SubCounty { get; set; }

        [DisplayName("Health Facility")]
        public int HealthFacilityId { get; set; }

        public HealthFacility HealthFacility { get; set; }

        [DisplayName("Payment Status")]
        public int StatusId { get; set; }

        public PaymentStatus Status { get; set; }
        public string FailureReason { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0}")]
        public decimal Amount { get; set; }

        public bool Reconciled { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string ReceiverPartyPublicName { get; set; }

        // Mpesa Details
        [DisplayName("Mpesa Code")]
        public string TransactionReceipt { get; set; }

        [DisplayName("Payment Error Message")]
        public string PaymentErrorMessage { get; set; }

        public string TransactionId { get; set; }
    }

    public class PaymentCycle : CreateFields
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        [DataType(DataType.Text)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Text)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime EndDate { get; set; }

        public bool Closed { get; set; }
    }

    public class PaymentStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}