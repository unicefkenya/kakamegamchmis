using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MCHMIS.Areas.Legacy.Models;
using MCHMIS.Models;
using X.PagedList;

namespace MCHMIS.Areas.Legacy.ViewModels
{
    public class ListViewModel
    {
        public string Reason { get; set; }

        [DisplayName("Reason Value")]
        public string ReasonValue { get; set; }

        [DisplayName("Mother's Name")]
        public string Name { get; set; }

        [DisplayName("ID Number")]
        public string IdNumber { get; set; }

        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }

        [DisplayName("Status")]
        public int? StatusId { get; set; }

        [DisplayName("Approval Status")]
        public int? ApprovalStatusId { get; set; }

        public string Option { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public Mother Mother { get; set; }
        public IPagedList<Mother> Mothers { get; set; }
        public IPagedList<Dirty> DirtyList { get; set; }
        public int Pending { get; set; }
        public int AwaitingApproval { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; } 
        public int Migrated { get; set; }
    }

    public class LegacyDetailsViewModel
    {
        public int Id { get; set; }

        [DisplayName("Status")]
        public int StatusId { get; set; }

        public int? ApprovalStatusId { get; set; }

        public string Status { get; set; }

        [DisplayName("National ID")]
        public string NationalId { get; set; }

        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }

        [DisplayName("Date of Birth")]
        public string DOB { get; set; }

        [Required]
        public string Notes { get; set; }

        [DisplayName("Approval Notes")]
        public string ApprovalNotes { get; set; }

        public Mother Mother { get; set; }
        public string Exception { get; set; }

        [DisplayName("Supporting Document")]
        public string SupportingDocument { get; set; }
        public IEnumerable<Dirty>DirtyList { get; set; }
    }

    public class LegacyExportViewModel
    {
        public string Name { get; set; }

        public string IDNumber { get; set; }

        [DisplayName("Date of Birth")]
        public string DOB { get; set; }

        public string Ward { get; set; }

        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }

        public string Facility { get; set; }
        public string Reason { get; set; }
        public string ReasonValue { get; set; }
        public string Status { get; set; }
    }

    public class LegacyApprovalViewModel
    {
        public int Id { get; set; }
        public string ApprovalNotes { get; set; }
        public int StatusId { get; set; }
    }
}