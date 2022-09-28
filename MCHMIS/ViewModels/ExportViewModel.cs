using System;
using System.ComponentModel;

namespace MCHMIS.ViewModels
{
    public class ExportViewModel
    {
        [DisplayName("Select Module")]
        public int ModuleId { get; set; }
        [DisplayName("Select Data Type")]
        public int? ReportId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}