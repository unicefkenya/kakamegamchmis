using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCHMIS.Models
{
    public class ClinicVisit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [DisplayName("Visit Type")] public int? VisitTypeId { get; set; }
        public SystemCodeDetail VisitType { get; set; }
        [DisplayName("Payment Point")] public int? PaymentPointId { get; set; }
        public PaymentPoint PaymentPoint { get; set; }
        public int Order { get; set; }
    }

    public class PaymentPoint
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [DisplayName("Amount to be paid to the mother")]
        [DisplayFormat(DataFormatString = "{0:###,##0}")]
        public decimal Amount { get; set; }
    }

    public class BeneficiaryPaymentPoint : CreateFields
    {
        public int Id { get; set; }
        public int PaymentPointId { get; set; }
        public PaymentPoint PaymentPoint { get; set; }
        public string HouseholdId { get; set; }
        public HouseholdReg Household { get; set; }
        public string CaseManagementId { get; set; }
        public int? StatusId { get; set; }
        public PaymentStatus Status { get; set; }
    }

    public class SMS
    {
        public int Id { get; set; }

        [DisplayName("Clinic Visit")]
        public int? ClinicVisitId { get; set; }

        public ClinicVisit ClinicVisit { get; set; }

        [DisplayName("Other SMS Trigger Event")]
        public string TriggerEvent { get; set; }

        [DataType(DataType.MultilineText)]
        public string Message { get; set; }

        public int Order { get; set; }
    }
}