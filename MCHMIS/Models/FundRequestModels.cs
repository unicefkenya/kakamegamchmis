using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCHMIS.Models
{
    public class FundRequest : CreateApprovalFields
    {
        public int Id { get; set; }

        public int CycleId { get; set; }

        public PaymentCycle Cycle { get; set; }

        public int StatusId { get; set; }
        public ApprovalStatus Status { get; set; }

        public string Notes { get; set; }
        public ICollection<PaymentTransaction> PaymentTransactions { get; set; }

        // Summaries
        [DisplayFormat(DataFormatString = "{0:###,##0}")]
        public decimal Amount { get; set; }

        [DisplayName("No. of Beneficiaries")]
        public int Beneficiaries { get; set; }

        public string Name => Cycle?.Name;
    }

    public class PaymentTransaction
    {
        public int Id { get; set; }

        public string BeneficiaryId { get; set; }
        public Beneficiary Beneficiary { get; set; }

        [DisplayName("Payment Point")]
        public int PaymentPointId { get; set; }

        public PaymentPoint PaymentPoint { get; set; }

        public int? BeneficiaryPaymentPointId { get; set; }
        [DisplayName("Fund Request")]
        public int FundRequestId { get; set; }
        public FundRequest FundRequest { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0}")]
        public decimal Amount { get; set; }

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

        public int? StatusId { get; set; }
        public PaymentStatus Status { get; set; }
    }

    public class PaymentsForfeited
    {
        public int Id { get; set; }

        public string BeneficiaryId { get; set; }
        public Beneficiary Beneficiary { get; set; }

        [DisplayName("Payment Point")]
        public int PaymentPointId { get; set; }

        public PaymentPoint PaymentPoint { get; set; }

        public int FundRequestId { get; set; }
        public FundRequest FundRequest { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0}")]
        public decimal Amount { get; set; }

        [DisplayName("Beneficiary Name")]
        public string BeneficiaryName { get; set; }

        [DisplayName("ID Number")]
        public string IdNumber { get; set; }

        [DisplayName("Recipient Name")]
        public string RecipientName { get; set; }

        [DisplayName("Recipient Phone")]
        public string RecipientPhone { get; set; }

        public int VillageId { get; set; }
        public Village Village { get; set; }

        public int WardId { get; set; }
        public Ward Ward { get; set; }

        [DisplayName("Sub-County")]
        public int SubCountyId { get; set; }

        public SubCounty SubCounty { get; set; }

        [DisplayName("Health Facility")]
        public int HealthFacilityId { get; set; }

        public HealthFacility HealthFacility { get; set; }

        public int? StatusId { get; set; }
        public PaymentStatus Status { get; set; }

        public int DaysOverdue { get; set; }
    }
}