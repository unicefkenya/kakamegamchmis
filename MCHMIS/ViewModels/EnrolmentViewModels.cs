using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace MCHMIS.ViewModels
{
    public class EnrolmentListExportViewModel
    {
        public string CreditIdentityStringType { get; set; }
        public string CreditIdentityString { get; set; }
        public string ValidationIdentityStringType { get; set; }
        public string ValidationIdentityString { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0}")]
        public string Amount { get; set; }

        public string Comment { get; set; }
    }

    public class EnrolmentListMismatchExportViewModel
    {
        public string MotherUniqueId { get; set; }
        public string RecipientPhone { get; set; }
        public string RecipientNames { get; set; }
        public string CustomerName { get; set; }
        public string CustomerType { get; set; }
        public string HealthFacility { get; set; }
        public string SubCounty { get; set; }
        public string Ward { get; set; }
        public string VillageUnit { get; set; }
        public string CommunityArea { get; set; }
        public string Issue { get; set; }
    }

    public class EnrolmentListImportViewModel
    {
        public DataTable Table { get; set; }
        public int Rows { get; set; }
        public string Errors { get; set; }
        public int Id { get; set; }
    }

    public class EnrolmentListSummaryViewModel
    {
        public int Id { get; set; }
        public string Facility { get; set; }
        public string SubCounty { get; set; }
        public string Ward { get; set; }
        public int Generated { get; set; }
        public int Validated { get; set; }
    }

    public class EnrolmentListDetailsViewModel
    {
        public int Id { get; set; }
        public IEnumerable<EnrolmentDetail> EnrolmentDetails { get; set; }
        public Enrolment Enrolment { get; set; }
        public EnrolmentDetail EnrolmentDetail { get; set; }
        public int ActionId { get; set; }
        public int[] Ids { get; set; }
    }

    public class EnrolmentApprovalsViewModel
    {
        public int Id { get; set; }
        public Enrolment Enrolment { get; set; }
        public int Count { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        public int StatusId { get; set; }
        public string Option { get; set; }

        [DisplayName("Phone Not Registered / Inactive")]
        public int NotRegistered { get; set; }

        [DisplayName("Names Mismatch")]
        public int NamesMismatch { get; set; }

        public int Validated { get; set; }
    }

    public class EnrolmentGenerateListViewModel
    {
        [DisplayName("No. of mothers to be generated"), Required]
        public int? NoToGenerate { get; set; }

        public string Notes { get; set; }

        public int[] StatusIds { get; set; }
    }
}