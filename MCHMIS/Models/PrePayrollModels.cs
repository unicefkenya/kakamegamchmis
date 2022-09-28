using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCHMIS.Models
{
    public class PrePayrollCheck : CreateApprovalFields
    {
        public int Id { get; set; }

        public int PaymentCycleId { get; set; }

        [DisplayName("Payment Cycle")]
        public PaymentCycle PaymentCycle { get; set; }

        public int StatusId { get; set; }
        public ApprovalStatus Status { get; set; }

        public ICollection<PrePayrollChecksDetail> PrePayrollChecksDetails { get; set; }
        public string Notes { get; set; }
        public Decimal ExpectedAmount { get; set; }
        public int? DuplicateIds { get; set; }
        public int? DuplicatePhoneNumbers { get; set; }
        public int? UnusualAmounts { get; set; }
        public int? RecipientChange { get; set; }
        public int? PhoneNumberIssues { get; set; }

        [DisplayName("Imported?")]
        public bool Imported { get; set; }
    }

    public class PrePayrollChecksDetail : ApprovalFields
    {
        public int Id { get; set; }
        public int PrePayrollCheckId { get; set; }
        public PrePayrollCheck PrePayrollCheck { get; set; }

        public string BeneficiaryId { get; set; }
        public Beneficiary Beneficiary { get; set; }

        public int ExceptionId { get; set; }
        public PayrollException Exception { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,###}")]
        public decimal? Amount { get; set; }

        public string RecipientName { get; set; }
        public string RecipientPhone { get; set; }
        public int? ActionId { get; set; }
        public PrePayrollAction Action { get; set; }

        public int? PaymentId { get; set; }
        public Payment Payment { get; set; }
        public string Notes { get; set; }

        public int? StatusId { get; set; }
        public ApprovalStatus Status { get; set; }

        public string CustomerName { get; set; }
        public string CustomerType { get; set; }
    }

    public class PrePayrollTransaction
    {
        public int Id { get; set; }
        public string BeneficiaryId { get; set; }
        public int PrePayrollCheckId { get; set; }

        [DisplayName("Payment Point")]
        public int PaymentPointId { get; set; }

        public PaymentPoint PaymentPoint { get; set; }
    }

    public class PrePayrollAction
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool CanProceed { get; set; }
    }

    public class PayrollException
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ExceptionTypeId { get; set; }
        public PayrollExceptionType ExceptionType { get; set; }
        public int Order { get; set; }
    }

    public class PayrollExceptionType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}