using MCHMIS.Models;
using X.PagedList;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCHMIS.Areas.Reports.ViewModels
{
    public class ChangesReportListViewModel
    {
        [DisplayName("Status")]
        public int? StatusId { get; set; }

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

        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public int AwaitingIPRS { get; set; }
        public int AwaitingMPesaVerification { get; set; }

        public Change Change { get; set; }
        public IPagedList<Change> Changes { get; set; }
        public ICollection<Ward> Wards { get; set; }

        public string Option { get; set; }

        [DisplayName("Report Type")]
        public int? ReportTypeId { get; set; }

        [DisplayName("Change Type")]
        public int? ChangeTypeId { get; set; }

        public int? Total { get; set; }

        [DisplayName("Start Date")]
        public string StartDate { get; set; }

        [DisplayName("End Date")]
        public string EndDate { get; set; }

        public ChangeSummaryViewModel Summary { get; set; }

        public IEnumerable<ChangeSummaryViewModel> Summaries { get; set; }
    }

    public class ChangeSummaryViewModel
    {
        public string Name { get; set; }
        public string Facility { get; set; }
        public string SubCounty { get; set; }
        public string Ward { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0}")]
        public int Count { get; set; }
    }
}