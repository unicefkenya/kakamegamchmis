using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MCHMIS.Models;
using X.PagedList;

namespace MCHMIS.Areas.Reports.ViewModels
{
    public class ComplaintsReportViewModel
    {
        [DisplayName("Report Type")]
        public int? ReportTypeId { get; set; }

        [DisplayName("Status")]
        public int? StatusId { get; set; }

        [DisplayName("Health Facility")]
        public int? HealthFacilityId { get; set; }

        [DisplayName("Complaint Type")]
        public int? ComplaintTypeId { get; set; }

        [DisplayName("Sub-County")]
        public int? SubCountyId { get; set; }

        [DisplayName("Ward")]
        public int? WardId { get; set; }

        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public Complaint Complaint { get; set; }
        public IPagedList<Complaint> Complaints { get; set; }
        public ICollection<Ward> Wards { get; set; }

        public ChangeSummaryViewModel Summary { get; set; }

        public IEnumerable<ChangeSummaryViewModel> Summaries { get; set; }

        public int Total { get; set; }

        public string Option { get; set; }
    }

    public class ComplaintSummaryViewModel
    {
        public string Facility { get; set; }
        public string SubCounty { get; set; }
        public string Ward { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0}")]
        public int Count { get; set; }
    }
}