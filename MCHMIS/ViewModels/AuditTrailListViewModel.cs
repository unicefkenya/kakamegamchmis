using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MCHMIS.Models;
using X.PagedList;
using DataType = System.ComponentModel.DataAnnotations.DataType;

namespace MCHMIS.ViewModels
{
    public class AuditTrailListViewModel
    {
        public IPagedList<AuditTrail> AuditTrails { get; set; }
        public AuditTrail AuditTrail { get; set; }

        [DisplayName("User")]
        public string UserId { get; set; }

        [DisplayName("Change Type")]
        public string ChangeType { get; set; }

        [DisplayName("Table Name")]
        public string TableName { get; set; }

        [DataType(DataType.Text)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Text)]
        public DateTime? EndDate { get; set; }

        public int? Page { get; set; }
        public int? PageSize { get; set; }

        [DisplayName("Field Name")]
        public string FieldName { get; set; }

        public string Option { get; set; }
    }

    public class TableViewModel
    {
        public string TableName { get; set; }
    }

    public class ChangeTypeViewModel
    {
        public string ChangeType { get; set; }
    }
}