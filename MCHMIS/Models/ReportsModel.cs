using System.ComponentModel;

namespace MCHMIS.Models
{
    public class Report
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Keyword { get; set; }

        [DisplayName("Report Category")]
        public int ReportCategoryId { get; set; }

        public ReportCategory ReportCategory { get; set; }
        public int Order { get; set; }
    }

    public class ReportCategory
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class ReportType
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}