using System.Collections.Generic;
using MCHMIS.Models;

namespace MCHMIS.Areas.Dashboard.ViewModels
{
    public class DashboardsFilterViewModel
    {
        public int IndicatorId { get; set; }
        public int PaymentCycleId { get; set; }
    }
    public class TrendsViewModel
    {
        public int? HealthFacilityId { get; set; }
        public int IndicatorId { get; set; }
        public int? StartPeriodId { get; set; }
        public int? EndPeriodId { get; set; }
        public ICollection<ReportingPeriod> ReportingPeriods { get; set; }
        public ICollection<MonthViewModel> TrendsSummary { get; set; }
    }

    public class TrendItem
    {
        public string Name { get; private set; }
        public string Public { get; private set; }
    }

    public class RegistrationTrend
    {
        public int PeriodId { get; private set; }
        public int Registered { get; private set; }
        public int Validated { get; private set; }
        public int Enrolled { get; private set; }
    }

    public class MonthViewModel
    {
        public string Category { get; set; }
        public List<int> Values { get; set; }
    }
}