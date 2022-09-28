namespace MCHMIS.ViewModels
{
  
    public class ViewReportsDisbursements
    {
        public string UniqueId { get; set; }
        public string IdNumber { get; set; }
        public string BeneficiaryName { get; set; }
        public string RecipientName { get; set; }
        public string RecipientPhone { get; set; }
        public string HealthFacility { get; set; }
        public string PaymentPoint { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public int FundRequestId { get; set; }
        public int PaymentPointId { get; set; }
        public int HealthFacilityId { get; set; }

    }
}