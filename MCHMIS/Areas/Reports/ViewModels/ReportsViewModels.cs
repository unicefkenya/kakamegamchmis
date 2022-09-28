using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using MCHMIS.Models;
using X.PagedList;

namespace MCHMIS.Areas.Reports.ViewModels
{
    public class DisbursementViewModel
    {
        public int Id { get; set; }
        public int HealthFacilityId { get; set; }
        public string Facility { get; set; }
        public string SubCounty { get; set; }
        public string Ward { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0}")]
        [DisplayName("Mothers")]
        public int Beneficiaries { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0}")]
        public decimal Amount { get; set; }
    }

    public class HouseholdsReportViewModel
    {
        public IPagedList<HouseholdReg> HouseholdRegs { get; set; }
        public ICollection<HouseholdReg> HouseholdRegList { get; set; }
        public HouseholdReg Household { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        [DisplayFormat(DataFormatString = "{0:###,##0}")] public int Total { get; set; }

        [DisplayName("Status")]
        public int? StatusId { get; set; }

        [DisplayName("Sub-County")]
        public int? SubCountyId { get; set; }

        [DisplayName("Ward")]
        public int? WardId { get; set; }

        [DisplayName("Heath-Facility")]
        public int? HealthFacilityId { get; set; }

        [DisplayName("Mother's Unique ID")]
        public string UniqueId { get; set; }

        [DisplayName("Identification Document No")]
        public string IdNumber { get; set; }

        [DisplayName("Any One Name")]
        public string Name { get; set; }

        public ICollection<Ward> Wards { get; set; }

        public ICollection<HealthFacility> HealthFacility { get; set; }

        [DisplayName("Report Type")]
        public int? ReportTypeId { get; set; }

        public HHSummaryViewModel Summary { get; set; }
        public IEnumerable<HHSummaryViewModel> Summaries { get; set; }

        public string Option { get; set; }

        [DisplayName("Start Date")]
        public string StartDate { get; set; }

        [DisplayName("End Date")]
        public string EndDate { get; set; }
    }

    public class MismatchReportViewModel
    {
        public IPagedList<EnrolmentDetail> Details { get; set; }

        public EnrolmentDetail EnrolmentDetail { get; set; }
        public HouseholdReg Household { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        [DisplayFormat(DataFormatString = "{0:###,##0}")] public int Total { get; set; }

        [DisplayName("Status")]
        public int? StatusId { get; set; }

        [DisplayName("Sub-County")]
        public int? SubCountyId { get; set; }

        [DisplayName("Ward")]
        public int? WardId { get; set; }

        [DisplayName("Heath-Facility")]
        public int? HealthFacilityId { get; set; }

        [DisplayName("Mother's Unique ID")]
        public string UniqueId { get; set; }

        [DisplayName("Identification Document No")]
        public string IdNumber { get; set; }

        [DisplayName("Any One Name")]
        public string Name { get; set; }

        public ICollection<Ward> Wards { get; set; }

        public ICollection<HealthFacility> HealthFacility { get; set; }

        [DisplayName("Report Type")]
        public int? ReportTypeId { get; set; }

        public ValidationViewModel Summary { get; set; }
        public IEnumerable<ValidationViewModel> Summaries { get; set; }

        public string Option { get; set; }

        [DisplayName("Start Date")]
        public string StartDate { get; set; }

        [DisplayName("End Date")]
        public string EndDate { get; set; }
    }

    public class NotEnrolledListViewModel
    {
        public ICollection<HouseholdReg> HouseholdRegs { get; set; }
        public HouseholdReg Household { get; set; }
        public int? Page { get; set; }
        public int? PageNo { get; set; }

        [DisplayName("Status")]
        public int? StatusId { get; set; }

        [DisplayName("Sub-County")]
        public int? SubCountyId { get; set; }

        [DisplayName("Ward")]
        public int? WardId { get; set; }

        [DisplayName("Heath-Facility")]
        public int? HealthFacilityID { get; set; }

        [DisplayName("Mother's Unique ID")]
        public string UniqueId { get; set; }

        [DisplayName("Identification Document No")]
        public string IdNumber { get; set; }

        [DisplayName("Any One Name")]
        public string Name { get; set; }

        public ICollection<Ward> Wards { get; set; }

        public ICollection<HealthFacility> HealthFacility { get; set; }
    }

    public class EnrolledListViewModel
    {
        [DisplayName("Report Type")]
        public int? ReportTypeId { get; set; }

        public IPagedList<Beneficiary> Beneficiaries { get; set; }
        public Beneficiary Beneficiary { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public int? PostBack { get; set; }

        [DisplayName("Status")]
        public int? StatusId { get; set; }

        [DisplayName("Sub-County")]
        public int? SubCountyId { get; set; }

        [DisplayName("Ward")]
        public int? WardId { get; set; }

        [DisplayName("Heath-Facility")]
        public int? HealthFacilityId { get; set; }

        [DisplayName("Mother's Unique ID")]
        public string UniqueId { get; set; }

        [DisplayName("Identification Document No")]
        public string IdNumber { get; set; }

        [DisplayName("Any One Name")]
        public string Name { get; set; }

        public ICollection<Ward> Wards { get; set; }

        public ICollection<HealthFacility> HealthFacility { get; set; }

        public HHSummaryViewModel Summary { get; set; }

        public IEnumerable<HHSummaryViewModel> Summaries { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0}")] public int Total { get; set; }
        public string Option { get; set; }

        [DisplayName("Start Date")]
        public string StartDate { get; set; }

        [DisplayName("End Date")]
        public string EndDate { get; set; }
    }

    public class ComplaintsViewModels
    {
        [DisplayName("CategoryId")]
        public int? CategoryId { get; set; }

        [DisplayName("Complaint-Type")]
        public int? ComplaintTypeId { get; set; }

        [DisplayName("Sub-County")]
        public int? SubCountyId { get; set; }

        [DisplayName("Ward")]
        public int? WardId { get; set; }

        [DisplayName("Heath-Facility")]
        public int? HealthFacilityID { get; set; }

        public string ActionTaken { get; set; }
        public ICollection<ComplaintNote> ComplaintNotes { get; set; }
        public ICollection<ComplaintStatus> Status { get; set; }
        public ICollection<Ward> Wards { get; set; }
        public ICollection<HealthFacility> HealthFacility { get; set; }
        public ICollection<ComplaintType> ComplaintTypes { get; set; }
    }

    public class CaseManagementListViewModel
    {
        public IPagedList<Pregnancy> Pregnancies { get; set; }
        public Pregnancy Pregnancy { get; set; }

        public IPagedList<MotherClinicVisit> MotherClinicVisits { get; set; }
        public MotherClinicVisit MotherClinicVisit { get; set; }

        public IPagedList<Delivery> Deliveries { get; set; }
        public Delivery Delivery { get; set; }

        public IPagedList<Child> Children { get; set; }
        public Child Child { get; set; }

        public IPagedList<PostNatalExamination> PostNatalExaminations { get; set; }
        public PostNatalExamination PostNatalExamination { get; set; }

        public HouseholdReg Household { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        [DisplayFormat(DataFormatString = "{0:###,##0}")] public int Total { get; set; }

        [DisplayName("Status")]
        public int? StatusId { get; set; }

        [DisplayName("Case Status")]
        public int? CaseStatusId { get; set; }

        [DisplayName("Sub-County")]
        public int? SubCountyId { get; set; }

        [DisplayName("Ward")]
        public int? WardId { get; set; }

        [DisplayName("Pregnancy Outcome")]
        public int? PregnancyOutcomeId { get; set; }

        [DisplayName("Family Planning Method")]
        public int? FPMethodId { get; set; }

        [DisplayName("Mother's Unique ID")]
        public string UniqueId { get; set; }

        [DisplayName("Identification Document No")]
        public string IdNumber { get; set; }

        [DisplayName("Any One Name")]
        public string Name { get; set; }

        public ICollection<Ward> Wards { get; set; }

        [DisplayName("Delivery Heath Facility")]
        public int? DeliveryHealthFacilityId { get; set; }

        [DisplayName("Heath Facility")]
        public int? HealthFacilityId { get; set; }

        [DisplayName("Report Type")]
        public int? ReportTypeId { get; set; }

        [DisplayName("Exit Reason")]
        public int? ReasonId { get; set; }

        [DisplayName("Clinic Visit")]
        public int? ClinicVisitId { get; set; }

        public string Option { get; set; }

        [DisplayName("Start Date")]
        public string StartDate { get; set; }

        [DisplayName("End Date")]
        public string EndDate { get; set; }

        public FacilitySummaryViewModel Summary { get; set; }
        public IEnumerable<FacilitySummaryViewModel> Summaries { get; set; }

        public HHSummaryViewModel WardSummary { get; set; }
        public IEnumerable<HHSummaryViewModel> WardSummaries { get; set; }

        public IPagedList<Pregnancy> Cases { get; set; }
        public CaseManagement CaseManagement { get; set; }

        public ExitedViewModel ExitedSummary { get; set; }
        public IEnumerable<ExitedViewModel> ExitedSummaries { get; set; }

        public IPagedList<Change> Changes { get; set; }
        public Change Change { get; set; }

        public RepeatCasesViewModel RepeatCase { get; set; }
        public IPagedList<RepeatCasesViewModel> RepeatCases { get; set; }
        public ChildrenBreastFeedingViewModel BreastFeeding { get; set; }
        public IPagedList<ChildrenBreastFeedingViewModel> ChildrenBreastFeeding { get; set; }
    }

    public class ValidationViewModel
    {
        public string Facility { get; set; }
        public string Exception { get; set; }

        [DisplayName("Mothers")]
        [DisplayFormat(DataFormatString = "{0:###,##0}")]
        public int Beneficiaries { get; set; }
    }

    public class FacilitySummaryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Facility { get; set; }

        [DisplayName("Mothers")]
        [DisplayFormat(DataFormatString = "{0:###,##0}")]
        public int Beneficiaries { get; set; }
    }

    public class RepeatCasesViewModel
    {
        public string UniqueId { get; set; }
        public string BeneficiaryName { get; set; }
        public string RecipientPhone { get; set; }
        public string RecipientName { get; set; }
        public string HealthFacility { get; set; }
        public string SubCounty { get; set; }
        public string Ward { get; set; }
        public string Village { get; set; }
    }

    public class ExitedViewModel
    {
        public string Facility { get; set; }
        public string Reason { get; set; }

        [DisplayName("Mothers")]
        [DisplayFormat(DataFormatString = "{0:###,##0}")]
        public int Beneficiaries { get; set; }
    }

    public class HHSummaryViewModel
    {
        public int Id { get; set; }
        public int HealthFacilityId { get; set; }
        public string Facility { get; set; }
        public string SubCounty { get; set; }
        public string Ward { get; set; }

        [DisplayName("Mothers")]
        [DisplayFormat(DataFormatString = "{0:###,##0}")]
        public int Beneficiaries { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0}")]
        public decimal Amount { get; set; }
    }

    public class VarianceReportViewModel
    {
        public string Id { get; set; }
        public CVList CVList { get; set; }
        public ICollection<HouseholdReg> HouseholdRegs { get; set; }
        public IPagedList<CVListDetail> Details { get; set; }
        public HouseholdReg Household { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        [DisplayFormat(DataFormatString = "{0:###,##0}")] public int Total { get; set; }

        [DisplayName("Status")]
        public int? StatusId { get; set; }

        [DisplayName("Sub-County")]
        public int? SubCountyId { get; set; }

        [DisplayName("Ward")]
        public int? WardId { get; set; }

        [DisplayName("Mother's Unique ID")]
        public string UniqueId { get; set; }

        [DisplayName("Identification Document No")]
        public string IdNumber { get; set; }

        [DisplayName("Any One Name")]
        public string Name { get; set; }

        public ICollection<Ward> Wards { get; set; }

        [DisplayName("Heath-Facility")]
        public int? HealthFacilityId { get; set; }

        [DisplayName("Report Type")]
        public int? ReportTypeId { get; set; }

        [DisplayName("Variance Category")]
        public int? VarianceCategoryId { get; set; }

        public string Option { get; set; }

        public VarianceSummaryViewModel Summary { get; set; }

        public IEnumerable<VarianceSummaryViewModel> Summaries { get; set; }
    }

    public class VarianceSummaryViewModel
    {
        public int Id { get; set; }
        public int HealthFacilityId { get; set; }
        public string Facility { get; set; }
        public string SubCounty { get; set; }
        public string Ward { get; set; }
        public int Low { get; set; }
        public int Medium { get; set; }
        public int High { get; set; }
        public decimal Amount { get; set; }
    }

    public class ChildrenBreastFeedingViewModel
    {
        public string UniqueId { get; set; }
        public string MotherName { get; set; }
        public string CommonName { get; set; }
        public string Phone { get; set; }
        public string HealthFacility { get; set; }
        public string SubCounty { get; set; }
        public string Ward { get; set; }
        public string Village { get; set; }

        public string ChildName { get; set; }
        public DateTime? DOB { get; set; }
    }

    public class ForecastViewModel
    {
        public string FinancialYear { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,###}")]
        public decimal Amount { get; set; }
    }
}