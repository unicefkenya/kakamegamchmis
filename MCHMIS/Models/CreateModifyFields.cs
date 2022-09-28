using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCHMIS.Models
{
    public class CreateFields
    {
        [DisplayName("Date Created"), DataType(DataType.Text)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime DateCreated { get; set; }

        [DisplayName("Created By")]
        public string CreatedById { get; set; }

        [DisplayName("Created By")]
        public ApplicationUser CreatedBy { get; set; }
    }

    public class CreateApprovalFields : CreateFields
    {
        [DisplayName("Date Approved")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? DateApproved { get; set; }

        [DisplayName("Approved By")]
        public string ApprovedById { get; set; }

        [DisplayName("Approved By")]
        public ApplicationUser ApprovedBy { get; set; }
    }

    public class ApprovalFields
    {
        [DisplayName("Date Approved")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? DateApproved { get; set; }

        [DisplayName("Approved By")]
        public string ApprovedById { get; set; }

        public ApplicationUser ApprovedBy { get; set; }
    }

    public class CreateModifyFields
    {
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? DateCreated { get; set; }

        public string CreatedById { get; set; }
        public ApplicationUser CreatedBy { get; set; }

        public DateTime? DateModified { get; set; }
        public string ModifiedById { get; set; }
        public ApplicationUser ModifiedBy { get; set; }
    }
}