using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MCHMIS.Models;
using X.PagedList;

namespace MCHMIS.ViewModels
{
    public class GeneratePrePayrollViewModel
    {
        public int PaymentCycleId { get; set; }

        [DisplayName("Expected Amount Per Beneficiary")]
        public decimal ExpectedAmount { get; set; }
    }

    public class PrePayrollListViewModel
    {
        public ICollection<PrePayrollCheck> PrePayrollChecks { get; set; }

        [DisplayName("Payment Cycle")]
        public int? PaymentCycleId { get; set; }
    }

    public class PrePayrollActionViewModel
    {
        public int Id { get; set; }
        public int? Page { get; set; }
        public PrePayrollChecksDetail ExceptionDetail { get; set; }
        public Beneficiary Beneficiary { get; set; }

        [DisplayName("Action")]
        public int PayrollActionId { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        public int? FacilityId { get; set; }
    }

    public class PrePayrollSummaryViewModel
    {
        public int Id { get; set; }
        public PrePayrollCheck PrePayrollCheck { get; set; }
        public IEnumerable<PrePayrollChecksDetail> PrePayrollChecksDetails { get; set; }
        public IEnumerable<PayrollException> Exceptions { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
    }

    public class PrePayrollBatchActionViewModel
    {
        public int ActionId { get; set; }
        public int[] Ids { get; set; }
        public int Id { get; set; }
        public int ExceptionId { get; set; }
        public bool ApproveAll { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        public int? HealthFacilityId { get; set; }
    }

    public class PrePayrollFilterViewModel
    {
        [Required]
        public int? Id { get; set; }

        public int? CountyId { get; set; }

        [DisplayName(" Health Facility")]
        public int? HealthFacilityId { get; set; }

        public int StatusId { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public PrePayrollChecksDetail PrePayrollChecksDetail { get; set; }
        public IPagedList<PrePayrollChecksDetail> Details { get; set; }
        public Beneficiary Beneficiary { get; set; }

        [DisplayName("Sub-County")]
        public int? SubCountyId { get; set; }

        [DisplayName("Ward")]
        public int? WardId { get; set; }

        [DisplayName("Mother's Unique Id")]
        public string UniqueId { get; set; }

        [DisplayName("Identification Document No")]
        public string IdNumber { get; set; } 
        
        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }

        [DisplayName("Any One Name")]
        public string Name { get; set; }

        public ICollection<Ward> Wards { get; set; }
        public IEnumerable<PrePayrollTransaction> PrePayrollTransactions { get; set; }

        public bool Imported { get; set; }
    }

    public class PrePayrollApprovalActionViewModel
    {
        public int Id { get; set; }
        public int StatusId { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        public string Action { get; set; }
        public PrePayrollCheck PrePayrollCheck { get; set; }
    }
}