using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCHMIS.ViewModels
{
    public class AuditTrailExport
    {
        public int Id { get; set; }

        [DisplayName("User")]
        public string User { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy hh:mm }")]
        [DisplayName("Date")]
        public DateTime Date { get; set; }

        public DateTime DateNow => Date.AddHours(3);

        [DisplayName("User Agent")]
        public string UserAgent { get; set; }

        [DisplayName("Request Ip Address")]
        public string RequestIpAddress { get; set; }

        [DisplayName("Change Type")]
        public string ChangeType { get; set; }

        public string TableName { get; set; }

        [DisplayName("Record Id")]
        public string RecordId { get; set; }

        [DisplayName("Old Value")]
        public string OldValue { get; set; }

        [DisplayName("New Record")]
        public string NewValue { get; set; }

        [DisplayName("PC Name")]
        public string PCName { get; set; }

        public string Description { get; set; }
    }
}