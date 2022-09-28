using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MCHMIS.Models;

public class Enrolment : CreateApprovalFields
{
    [DisplayName("Batch #")]
    public int Id { get; set; }

    [DisplayName("No. Generated")]
    public int HouseholdsGenerated { get; set; }

    [DisplayName("No. Validated")]
    public int HouseholdsValidated { get; set; }

    public int StatusId { get; set; }
    public ApprovalStatus Status { get; set; }
    public string Notes { get; set; }

    [DisplayName("Approval Notes")]
    public string ApprovalNotes { get; set; }

    [DisplayName("Date Imported")]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
    public DateTime? DateImported { get; set; }

    [DisplayName("Imported By")]
    public string ImportedById { get; set; }

    public ApplicationUser ImportedBy { get; set; }

    [DisplayName("Import Status")]
    public int? ImportStatusId { get; set; }

    [DisplayName("Import Status")]
    public ApprovalStatus ImportStatus { get; set; }

    [DisplayName("Import Approved By")]
    public string ImportApprovedById { get; set; }

    public ApplicationUser ImportApprovedBy { get; set; }

    [DisplayName("Import Approval Date")]
    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]
    public DateTime? ImportDateApproved { get; set; }

    [DisplayName("Import Approval Notes")]
    public string ImportApprovalNotes { get; set; }
}

public class EnrolmentDetail
{
    public int Id { get; set; }

    [DisplayName("Enrolment Id")]
    public int EnrolmentId { get; set; }

    public Enrolment Enrolment { get; set; }

    public string HouseholdId { get; set; }
    public HouseholdReg Household { get; set; }

    [DisplayName("Recipient Names")]
    public string RecipientNames { get; set; }

    [DisplayName("Customer Name")]
    public string CustomerName { get; set; }

    [DisplayName("Recipient Phone")]
    public string RecipientPhone { get; set; }

    [DisplayName("Customer Type")]
    public string CustomerType { get; set; }

    [DisplayName("Has Proxy")]
    public bool HasProxy { get; set; }

    public int? StatusId { get; set; }
    public Status Status { get; set; }
}