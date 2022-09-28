using System;

namespace MCHMIS.Models
{
    public class ReportingPeriod
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class RegistrationSummary
    {
        public int Id { get; set; }
        public int PeriodId { get; set; }
        public int Registered { get; set; }
        public int Eligible { get; set; }
        public int Ineligible { get; set; }
        public int? WardId { get; set; }
        public int? HealthFacilityId { get; set; }
    }

    public class CommunityValidationSummary
    {
        public int Id { get; set; }
        public int PeriodId { get; set; }
        public int High { get; set; }
        public int Medium { get; set; }
        public int Low { get; set; }
        public int? WardId { get; set; }
        public int? HealthFacilityId { get; set; }
    }

    public class EnrolmentSummary
    {
        public int Id { get; set; }
        public int PeriodId { get; set; }
        public int Enrolled { get; set; }
        public int Active { get; set; }
        public int Peak { get; set; }
        public int Waiting { get; set; }
        public int? WardId { get; set; }
        public int? HealthFacilityId { get; set; }
    }

    public class PaymentsSummary
    {
        public int Id { get; set; }
        public int PeriodId { get; set; }
        public int IndicatorId { get; set; }
        public int Stage1 { get; set; }
        public int Stage2 { get; set; }
        public int Stage3 { get; set; }
        public int Stage4 { get; set; }
        public int Stage5 { get; set; }
        public int Stage6 { get; set; }
        public int Benevolent { get; set; }
        public int? WardId { get; set; }
        public int? HealthFacilityId { get; set; }
    }

    public class ComplaintsSummary
    {
        public int Id { get; set; }
        public int PeriodId { get; set; }
        public int OpenWithinSLA { get; set; }
        public int OpenOutsideSLA { get; set; }
        public int ResolvedWithinSLA { get; set; }
        public int ResolvedOutsideSLA { get; set; }
        public int? WardId { get; set; }
        public int? HealthFacilityId { get; set; }
    }

    public class CaseManagementSummary
    {
        public int Id { get; set; }
        public int PeriodId { get; set; }
        public int Updated { get; set; }
        public int Missed { get; set; }
        public int? WardId { get; set; }
        public int? HealthFacilityId { get; set; }
        public int? VisitTypeId { get; set; }
    }
}