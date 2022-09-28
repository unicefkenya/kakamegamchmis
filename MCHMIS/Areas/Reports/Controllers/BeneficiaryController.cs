using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MCHMIS.Areas.Reports.ViewModels;
using MCHMIS.Data;
using MCHMIS.Interfaces;
using MCHMIS.Models;
using MCHMIS.Services;
using MCHMIS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using CommunityValidationListViewModel = MCHMIS.Areas.Reports.ViewModels.CommunityValidationListViewModel;
using PaymentTransactionsViewModel = MCHMIS.Areas.Reports.ViewModels.PaymentTransactionsViewModel;

namespace MCHMIS.Areas.Reports.Controllers
{
    [Area("Reports")]
    [AllowAnonymous]
    public class BeneficiaryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IDBService _dbService;

        public BeneficiaryController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment
            , IUnitOfWork uow
            , IExportService exportService
            , IDBService dbService)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _uow = uow;
            _dbService = dbService;
            _exportService = exportService;
        }

        public async Task<IActionResult> Index(HouseholdsReportViewModel vm)
        {
            ViewData["ReportTypeId"] = new SelectList(_context.ReportTypes, "Id", "Name", vm.ReportTypeId);
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Name", vm.StatusId);
            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name", vm.SubCountyId);
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities, "Id", "Name", vm.HealthFacilityId);
            vm.Wards = await _context.Wards.ToListAsync();
            var reportTitle = (vm.ReportTypeId == 1 ? "Summary of" : "List of") + " Registered Mothers";

            if (vm.HealthFacilityId != null)
            {
                reportTitle = reportTitle + " - " + _context.HealthFacilities.Find(vm.HealthFacilityId).Name;
            }
            if (vm.SubCountyId != null)
            {
                reportTitle = reportTitle + " - " + _context.SubCounties.Find(vm.SubCountyId).Name + " Sub County";
            }
            if (vm.WardId != null)
            {
                reportTitle = reportTitle + " - " + _context.Wards.Find(vm.WardId).Name + " Ward";
            }

            ViewBag.Title = reportTitle;

            if (vm.Option != null && vm.Option.Equals("export"))
            {
                string queryString = _exportService.GetQueryString(vm);
                var url = "beneficiary?" + queryString;
                var file = _exportService.ExportToPDF(url);
                return File(file, "application/pdf", reportTitle.Replace(" ", "-") + ".pdf");
            }

            if (vm.ReportTypeId != null)
            {
                if (vm.ReportTypeId == 1)
                {
                    SqlParameter[] parms = new SqlParameter[]
                    {
                         new SqlParameter("@StatusId", vm.StatusId ?? (object) DBNull.Value),
                        new SqlParameter("@HealthFacilityId", vm.HealthFacilityId ?? (object)DBNull.Value),
                        new SqlParameter("@SubCountyId", vm.SubCountyId ?? (object)DBNull.Value),
                        new SqlParameter("@WardId", vm.WardId ?? (object)DBNull.Value),
                        new SqlParameter("@Option","Registration"),
                        new SqlParameter("@StartDate",vm.StartDate ?? (object)DBNull.Value),
                        new SqlParameter("@EndDate",vm.EndDate ?? (object)DBNull.Value),
                    };
                    var result =
                        SQLExtensions.GetModelFromQuery<HHSummaryViewModel>(_context,
                            "EXEC [HouseholdsSummary] @StatusId,@HealthFacilityId,@SubCountyId,@WardId,@Option,@StartDate,@EndDate", parms);
                    vm.Summaries = result;
                    return View("RegisteredSummary", vm);
                }
                else
                {
                    var households = _context.HouseholdRegs
                        .Include(r => r.Village.Ward.SubCounty)
                        .Include(r => r.Mother)
                        .Include(r => r.Status)
                        .Include(r => r.HealthFacility)
                        .Where(r => r.TypeId == 1)
                        .OrderBy(r => r.Mother.FirstName)
                        .AsQueryable();

                    if (!string.IsNullOrEmpty(vm.UniqueId))
                    {
                        households = households.Where(h => h.UniqueId == vm.UniqueId);
                    }
                    if (!string.IsNullOrEmpty(vm.IdNumber))
                    {
                        households = households.Where(h => h.Mother.IdNumber.Contains(vm.IdNumber));
                    }
                    if (!string.IsNullOrEmpty(vm.Name))
                    {
                        households = households.Where(h =>
                            h.Mother.FirstName.Contains(vm.Name)
                            || h.Mother.MiddleName.Contains(vm.Name)
                            || h.Mother.Surname.Contains(vm.Name)
                            || h.Mother.Surname.Contains(vm.Name)
                        );
                    }
                    if (vm.StatusId != null)
                    {
                        households = households.Where(h => h.StatusId == vm.StatusId);
                    }
                    if (vm.WardId != null)
                    {
                        households = households.Where(h => h.Village.WardId == vm.WardId);
                    }
                    if (vm.SubCountyId != null)
                    {
                        households = households.Where(h => h.Village.Ward.SubCountyId == vm.SubCountyId);
                    }
                    if (vm.HealthFacilityId != null)
                    {
                        households = households.Where(h => h.HealthFacility.Id == vm.HealthFacilityId);
                    }
                    if (vm.StartDate != null)
                    {
                        if (vm.StatusId != null && vm.StatusId == 19)
                            households = households.Where(h => h.DateCreated >= DateTime.Parse(vm.StartDate));
                        else
                            households = households.Where(h => h.DateCreated >= DateTime.Parse(vm.StartDate));
                    }
                    if (vm.EndDate != null)
                    {
                        households = households.Where(h => h.DateCreated <= DateTime.Parse(vm.EndDate));
                    }
                    var page = vm.Page ?? 1;
                    var pageSize = vm.PageSize ?? 20;
                    // var onePageOfProducts = households.ToPagedList(page, pageSize);
                    vm.Total = households.Count();
                    vm.HouseholdRegs = households.ToPagedList(page, pageSize);
                }
            }

            return View(vm);
        }

        public async Task<IActionResult> UnderAge(HouseholdsReportViewModel vm)
        {
            ViewData["ReportTypeId"] = new SelectList(_context.ReportTypes, "Id", "Name", vm.ReportTypeId);
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Name", vm.StatusId);
            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name", vm.SubCountyId);
            ViewData["HealthFacilityID"] = new SelectList(_context.HealthFacilities, "Id", "Name", vm.HealthFacilityId);
            vm.Wards = await _context.Wards.ToListAsync();
            var reportTitle = (vm.ReportTypeId == 1 ? "Summary of" : "List of") + " Under Age Mothers";

            if (vm.HealthFacilityId != null)
            {
                reportTitle = reportTitle + " - " + _context.HealthFacilities.Find(vm.HealthFacilityId).Name;
            }
            if (vm.SubCountyId != null)
            {
                reportTitle = reportTitle + " - " + _context.SubCounties.Find(vm.SubCountyId).Name + " Sub County";
            }
            if (vm.WardId != null)
            {
                reportTitle = reportTitle + " - " + _context.Wards.Find(vm.WardId).Name + " Ward";
            }

            ViewBag.Title = reportTitle;

            if (vm.Option != null && vm.Option.Equals("export"))
            {
                string queryString = _exportService.GetQueryString(vm);
                var url = "beneficiary/UnderAge?" + queryString;
                var file = _exportService.ExportToPDF(url);
                return File(file, "application/pdf", reportTitle.Replace(" ", "-") + ".pdf");
            }

            if (vm.ReportTypeId != null)
            {
                if (vm.ReportTypeId == 1)
                {
                    SqlParameter[] parms = new SqlParameter[]
                    {
                         new SqlParameter("@StatusId", vm.StatusId ?? (object) DBNull.Value),
                        new SqlParameter("@HealthFacilityId", vm.HealthFacilityId ?? (object)DBNull.Value),
                        new SqlParameter("@SubCountyId", vm.SubCountyId ?? (object)DBNull.Value),
                        new SqlParameter("@WardId", vm.WardId ?? (object)DBNull.Value),
                        new SqlParameter("@Option","UnderAge"),
                        new SqlParameter("@StartDate",vm.StartDate ?? (object)DBNull.Value),
                        new SqlParameter("@EndDate",vm.EndDate ?? (object)DBNull.Value),
                    };
                    var result =
                        SQLExtensions.GetModelFromQuery<HHSummaryViewModel>(_context,
                            "EXEC [HouseholdsSummary] @StatusId,@HealthFacilityId,@SubCountyId,@WardId,@Option,@StartDate,@EndDate", parms);
                    vm.Summaries = result;
                    return View("UnderAgeSummary", vm);
                }

                var currentDate = DateTime.UtcNow;
                DbFunctions dfunc = null;
                var households = _context.HouseholdRegs
                    .Include(r => r.Village.Ward.SubCounty)
                    .Include(r => r.Mother)
                    .Include(r => r.Status)
                    .Include(r => r.HealthFacility)
                    .Where(r => r.TypeId == 1
                                && SqlServerDbFunctionsExtensions.DateDiffYear(dfunc, r.Mother.DOB, currentDate) < 18)
                    .OrderBy(r => r.Mother.FirstName)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(vm.UniqueId))
                {
                    households = households.Where(h => h.UniqueId == vm.UniqueId);
                }
                if (!string.IsNullOrEmpty(vm.IdNumber))
                {
                    households = households.Where(h => h.Mother.IdNumber.Contains(vm.IdNumber));
                }
                if (!string.IsNullOrEmpty(vm.Name))
                {
                    households = households.Where(h =>
                        h.Mother.FirstName.Contains(vm.Name)
                        || h.Mother.MiddleName.Contains(vm.Name)
                        || h.Mother.Surname.Contains(vm.Name)
                        || h.Mother.Surname.Contains(vm.Name)
                    );
                }
                if (vm.StatusId != null)
                {
                    households = households.Where(h => h.StatusId == vm.StatusId);
                }
                if (vm.WardId != null)
                {
                    households = households.Where(h => h.Village.WardId == vm.WardId);
                }
                if (vm.SubCountyId != null)
                {
                    households = households.Where(h => h.Village.Ward.SubCountyId == vm.SubCountyId);
                }
                if (vm.HealthFacilityId != null)
                {
                    households = households.Where(h => h.HealthFacility.Id == vm.HealthFacilityId);
                }
                if (vm.StartDate != null)
                {
                    households = households.Where(h => h.DateCreated >= DateTime.Parse(vm.StartDate));
                }
                if (vm.EndDate != null)
                {
                    households = households.Where(h => h.DateCreated <= DateTime.Parse(vm.EndDate));
                }
                var page = vm.Page ?? 1;
                var pageSize = vm.PageSize ?? 20;
                vm.Total = households.Count();
                vm.HouseholdRegs = households.ToPagedList(page, pageSize);
            }

            return View(vm);
        }

        public async Task<IActionResult> WithoutIds(HouseholdsReportViewModel vm)
        {
            ViewData["ReportTypeId"] = new SelectList(_context.ReportTypes, "Id", "Name", vm.ReportTypeId);
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Name", vm.StatusId);
            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name", vm.SubCountyId);
            ViewData["HealthFacilityID"] = new SelectList(_context.HealthFacilities, "Id", "Name", vm.HealthFacilityId);
            vm.Wards = await _context.Wards.ToListAsync();
            var reportTitle = (vm.ReportTypeId == 1 ? "Summary of" : "List of") + " mothers without ID cards (using letter from the chief)";

            if (vm.HealthFacilityId != null)
            {
                reportTitle = reportTitle + " - " + _context.HealthFacilities.Find(vm.HealthFacilityId).Name;
            }
            if (vm.SubCountyId != null)
            {
                reportTitle = reportTitle + " - " + _context.SubCounties.Find(vm.SubCountyId).Name + " Sub County";
            }
            if (vm.WardId != null)
            {
                reportTitle = reportTitle + " - " + _context.Wards.Find(vm.WardId).Name + " Ward";
            }

            ViewBag.Title = reportTitle;

            if (vm.Option != null && vm.Option.Equals("export"))
            {
                string queryString = _exportService.GetQueryString(vm);
                var url = "beneficiary/WithoutIds?" + queryString;
                var file = _exportService.ExportToPDF(url);
                return File(file, "application/pdf", reportTitle.Replace(" ", "-") + ".pdf");
            }

            if (vm.ReportTypeId != null)
            {
                if (vm.ReportTypeId == 1)
                {
                    SqlParameter[] parms = new SqlParameter[]
                    {
                         new SqlParameter("@StatusId", vm.StatusId ?? (object) DBNull.Value),
                        new SqlParameter("@HealthFacilityId", vm.HealthFacilityId ?? (object)DBNull.Value),
                        new SqlParameter("@SubCountyId", vm.SubCountyId ?? (object)DBNull.Value),
                        new SqlParameter("@WardId", vm.WardId ?? (object)DBNull.Value),
                        new SqlParameter("@Option","WithoutIds"),
                        new SqlParameter("@StartDate",vm.StartDate ?? (object)DBNull.Value),
                        new SqlParameter("@EndDate",vm.EndDate ?? (object)DBNull.Value),
                    };
                    var result =
                        SQLExtensions.GetModelFromQuery<HHSummaryViewModel>(_context,
                            "EXEC [HouseholdsSummary] @StatusId,@HealthFacilityId,@SubCountyId,@WardId,@Option,@StartDate,@EndDate", parms);
                    vm.Summaries = result;
                    return View("WithoutIdsSummary", vm);
                }
                var households = _context.HouseholdRegs
                      .Include(r => r.Village.Ward.SubCounty)
                      .Include(r => r.Mother)
                      .Include(r => r.Status)
                      .Include(r => r.HealthFacility)
                      .Where(r => r.TypeId == 1 && r.Mother.IdentificationFormId == 195).OrderBy(r => r.Mother.FirstName)
                      .AsQueryable();

                if (!string.IsNullOrEmpty(vm.UniqueId))
                {
                    households = households.Where(h => h.UniqueId == vm.UniqueId);
                }
                if (!string.IsNullOrEmpty(vm.IdNumber))
                {
                    households = households.Where(h => h.Mother.IdNumber.Contains(vm.IdNumber));
                }
                if (!string.IsNullOrEmpty(vm.Name))
                {
                    households = households.Where(h =>
                        h.Mother.FirstName.Contains(vm.Name)
                        || h.Mother.MiddleName.Contains(vm.Name)
                        || h.Mother.Surname.Contains(vm.Name)
                        || h.Mother.Surname.Contains(vm.Name)
                    );
                }
                if (vm.StatusId != null)
                {
                    households = households.Where(h => h.StatusId == vm.StatusId);
                }
                if (vm.WardId != null)
                {
                    households = households.Where(h => h.Village.WardId == vm.WardId);
                }
                if (vm.SubCountyId != null)
                {
                    households = households.Where(h => h.Village.Ward.SubCountyId == vm.SubCountyId);
                }
                if (vm.HealthFacilityId != null)
                {
                    households = households.Where(h => h.HealthFacility.Id == vm.HealthFacilityId);
                }
                if (vm.StartDate != null)
                {
                    households = households.Where(h => h.DateCreated >= DateTime.Parse(vm.StartDate));
                }
                if (vm.EndDate != null)
                {
                    households = households.Where(h => h.DateCreated <= DateTime.Parse(vm.EndDate));
                }
                var page = vm.Page ?? 1;
                var pageSize = vm.PageSize ?? 20;
                vm.Total = households.Count();
                vm.HouseholdRegs = households.ToPagedList(page, pageSize);
            }

            return View(vm);
        }

        public async Task<IActionResult> NonResident(HouseholdsReportViewModel vm)
        {
            ViewData["ReportTypeId"] = new SelectList(_context.ReportTypes, "Id", "Name", vm.ReportTypeId);
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Name", vm.StatusId);
            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name", vm.SubCountyId);
            ViewData["HealthFacilityID"] = new SelectList(_context.HealthFacilities, "Id", "Name", vm.HealthFacilityId);
            vm.Wards = await _context.Wards.ToListAsync();
            var reportTitle = (vm.ReportTypeId == 1 ? "Summary of" : "List of") + " Mothers from other counties";

            ViewBag.Title = reportTitle;

            if (vm.Option != null && vm.Option.Equals("export"))
            {
                string queryString = _exportService.GetQueryString(vm);
                var url = "beneficiary/NonResident?" + queryString;
                var file = _exportService.ExportToPDF(url);
                return File(file, "application/pdf", reportTitle.Replace(" ", "-") + ".pdf");
            }

            if (vm.ReportTypeId != null)
            {
                if (vm.ReportTypeId == 1)
                {
                    SqlParameter[] parms = new SqlParameter[]
                    {
                         new SqlParameter("@StatusId", vm.StatusId ?? (object) DBNull.Value),
                        new SqlParameter("@HealthFacilityId", vm.HealthFacilityId ?? (object)DBNull.Value),
                        new SqlParameter("@SubCountyId", vm.SubCountyId ?? (object)DBNull.Value),
                        new SqlParameter("@WardId", vm.WardId ?? (object)DBNull.Value),
                        new SqlParameter("@Option","NonResident"),
                        new SqlParameter("@StartDate",vm.StartDate ?? (object)DBNull.Value),
                        new SqlParameter("@EndDate",vm.EndDate ?? (object)DBNull.Value),
                    };
                    var result =
                        SQLExtensions.GetModelFromQuery<HHSummaryViewModel>(_context,
                            "EXEC [HouseholdsSummary] @StatusId,@HealthFacilityId,@SubCountyId,@WardId,@Option,@StartDate,@EndDate", parms);
                    vm.Summaries = result;
                    return View("NonResidentSummary", vm);
                }
                var households = _context.HouseholdRegs
                      .Include(r => r.OtherCounty)
                    .Include(r => r.Mother)
                      .Where(r => r.TypeId == 1 && r.CountyId == 9001).OrderBy(r => r.Mother.FirstName)
                      .AsQueryable();

                if (vm.HealthFacilityId != null)
                {
                    households = households.Where(h => h.HealthFacility.Id == vm.HealthFacilityId);
                }
                if (vm.StartDate != null)
                {
                    households = households.Where(h => h.DateCreated >= DateTime.Parse(vm.StartDate));
                }
                if (vm.EndDate != null)
                {
                    households = households.Where(h => h.DateCreated <= DateTime.Parse(vm.EndDate));
                }
                var page = vm.Page ?? 1;
                var pageSize = vm.PageSize ?? 20;
                vm.Total = households.Count();
                vm.HouseholdRegs = households.ToPagedList(page, pageSize);
            }

            return View(vm);
        }

        public async Task<IActionResult> MisMatch(MismatchReportViewModel vm)
        {
            ViewData["ReportTypeId"] = new SelectList(_context.ReportTypes, "Id", "Name", vm.ReportTypeId);
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Name", vm.StatusId);
            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name", vm.SubCountyId);
            ViewData["HealthFacilityID"] = new SelectList(_context.HealthFacilities, "Id", "Name", vm.HealthFacilityId);
            vm.Wards = await _context.Wards.ToListAsync();
            var reportTitle = (vm.ReportTypeId == 1 ? "Summary of" : "List of") + " Mothers with numbers that did not match Safaricom's";
            if (vm.HealthFacilityId != null)
            {
                reportTitle = reportTitle + " - " + _context.HealthFacilities.Find(vm.HealthFacilityId).Name;
            }
            if (vm.SubCountyId != null)
            {
                reportTitle = reportTitle + " - " + _context.SubCounties.Find(vm.SubCountyId).Name + " Sub County";
            }
            if (vm.WardId != null)
            {
                reportTitle = reportTitle + " - " + _context.Wards.Find(vm.WardId).Name + " Ward";
            }
            ViewBag.Title = reportTitle;

            if (vm.Option != null && vm.Option.Equals("export"))
            {
                string queryString = _exportService.GetQueryString(vm);
                var url = "beneficiary/MisMatch?" + queryString;
                var file = _exportService.ExportToPDF(url);
                return File(file, "application/pdf", reportTitle.Replace(" ", "-") + ".pdf");
            }

            if (vm.ReportTypeId != null)
            {
                if (vm.ReportTypeId == 1)
                {
                    SqlParameter[] parms = new SqlParameter[]
                    {
                         new SqlParameter("@StatusId", vm.StatusId ?? (object) DBNull.Value),
                        new SqlParameter("@HealthFacilityId", vm.HealthFacilityId ?? (object)DBNull.Value),
                        new SqlParameter("@SubCountyId", vm.SubCountyId ?? (object)DBNull.Value),
                        new SqlParameter("@WardId", vm.WardId ?? (object)DBNull.Value),
                        new SqlParameter("@Option","MpesaValidation"),
                        new SqlParameter("@StartDate",vm.StartDate ?? (object)DBNull.Value),
                        new SqlParameter("@EndDate",vm.EndDate ?? (object)DBNull.Value),
                    };
                    var result =
                        SQLExtensions.GetModelFromQuery<ValidationViewModel>(_context,
                            "EXEC [HouseholdsSummary] @StatusId,@HealthFacilityId,@SubCountyId,@WardId,@Option,@StartDate,@EndDate", parms);
                    vm.Summaries = result;
                    return View("MismatchSummary", vm);
                }

                var households = _context.EnrolmentDetails
                    .Where(i => i.Household.StatusId == 17 || i.Household.StatusId == 18)
                    .Include(e => e.Enrolment)
                    .Include(e => e.Household.Village.Ward.SubCounty)
                    .Include(e => e.Status)
                    .AsQueryable();

                if (vm.HealthFacilityId != null)
                {
                    households = households.Where(h => h.Household.HealthFacility.Id == vm.HealthFacilityId);
                }
                if (vm.StartDate != null)
                {
                    households = households.Where(h => h.Household.DateCreated >= DateTime.Parse(vm.StartDate));
                }
                if (vm.EndDate != null)
                {
                    households = households.Where(h => h.Household.DateCreated <= DateTime.Parse(vm.EndDate));
                }
                var page = vm.Page ?? 1;
                var pageSize = vm.PageSize ?? 20;
                vm.Details = households.ToPagedList(page, pageSize);
                vm.Total = households.Count();
            }

            return View(vm);
        }

        public async Task<IActionResult> CommunityValidation(CommunityValidationListViewModel vm)
        {
            var statusIds = new[] { 9, 3, 4, 10, 5 };
            ViewData["StatusId"] = new SelectList(_context.Status
                .Where(i => statusIds.Contains(i.Id)), "Id", "Name", vm.StatusId);
            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name", vm.SubCountyId);
            ViewData["HealthFacilityID"] = new SelectList(_context.HealthFacilities, "Id", "Name", vm.HealthFacilityId);
            vm.Wards = await _context.Wards.ToListAsync();
            ViewData["ReportTypeId"] = new SelectList(_context.ReportTypes, "Id", "Name", vm.ReportTypeId);

            var reportTitle = (vm.ReportTypeId == 1 ? "Summary of" : "List of") + " Community Validation";
            if (vm.HealthFacilityId != null)
            {
                reportTitle = reportTitle + " - " + _context.HealthFacilities.Find(vm.HealthFacilityId).Name;
            }
            if (vm.SubCountyId != null)
            {
                reportTitle = reportTitle + " - " + _context.SubCounties.Find(vm.SubCountyId).Name + " Sub County";
            }
            if (vm.WardId != null)
            {
                reportTitle = reportTitle + " - " + _context.Wards.Find(vm.WardId).Name + " Ward";
            }

            ViewBag.Title = reportTitle;

            if (vm.Option != null && vm.Option.Equals("export"))
            {
                string queryString = _exportService.GetQueryString(vm);
                var url = "beneficiary/communityvalidation?" + queryString;
                var file = _exportService.ExportToPDF(url);
                return File(file, "application/pdf", reportTitle.Replace(" ", "-") + ".pdf");
            }

            if (vm.ReportTypeId != null)
            {
                if (vm.ReportTypeId == 1)
                {
                    if (vm.StatusId == 1)
                        vm.StatusId = null;

                    SqlParameter[] parms = new SqlParameter[]
                    {
                         new SqlParameter("@StatusId", vm.StatusId ?? (object) DBNull.Value),
                        new SqlParameter("@HealthFacilityId", vm.HealthFacilityId ?? (object)DBNull.Value),
                        new SqlParameter("@SubCountyId", vm.SubCountyId ?? (object)DBNull.Value),
                        new SqlParameter("@WardId", vm.WardId ?? (object)DBNull.Value),
                    };
                    var result =
                        SQLExtensions.GetModelFromQuery<HHSummaryViewModel>(_context,
                            "EXEC [ReportCVSummary] @StatusId,@HealthFacilityId,@SubCountyId,@WardId", parms);
                    vm.Summaries = result;

                    return View("CommunityValidationSummary", vm);
                }
                else
                {
                    IQueryable<CVListDetail> details = _context.CvListDetails
                        .Include(r => r.Household.Village.Ward.SubCounty)
                        .Include(r => r.Household.CommunityArea)
                        .Include(r => r.Household.Status)
                        .Include(r => r.Status)
                        .Include(r => r.CVHouseHold)
                        .Include(r => r.Household.HealthFacility)
                        .Include(r => r.Household.Mother)
                        .Include(r => r.VarianceCategory)
                        .Include(r => r.Enumerator)
                        .OrderBy(i => i.Household.Mother.FirstName).Where(i => i.List.ListTypeId == 211);

                    if (vm.StatusId != null)
                    {
                        if (vm.StatusId == 1)
                            vm.StatusId = null;
                        details = details.Where(h => h.StatusId == vm.StatusId);
                    }
                    if (vm.WardId != null)
                    {
                        details = details.Where(h => h.Household.Village.WardId == vm.WardId);
                    }
                    if (vm.SubCountyId != null)
                    {
                        details = details.Where(h => h.Household.Village.Ward.SubCountyId == vm.SubCountyId);
                    }
                    if (vm.HealthFacilityId != null)
                    {
                        details = details.Where(h => h.Household.HealthFacility.Id == vm.HealthFacilityId);
                    }

                    var page = vm.Page ?? 1;
                    var pageSize = vm.PageSize ?? 20;
                    vm.Total = details.Count();
                    vm.Details = details.ToPagedList(page, pageSize);
                }
            }

            return View(vm);
        }

        public async Task<IActionResult> MovedOut(CommunityValidationListViewModel vm)
        {
            ViewData["StatusId"] = new SelectList(_context.ApprovalStatus.Where(i => i.Id < 5), "Id", "Name", vm.StatusId);
            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name", vm.SubCountyId);
            ViewData["HealthFacilityID"] = new SelectList(_context.HealthFacilities, "Id", "Name", vm.HealthFacilityId);
            vm.Wards = await _context.Wards.ToListAsync();
            ViewData["ReportTypeId"] = new SelectList(_context.ReportTypes, "Id", "Name", vm.ReportTypeId);

            var reportTitle = (vm.ReportTypeId == 1 ? "Summary of" : "List of") + " Mothers who have moved out of the county after registration";
            if (vm.HealthFacilityId != null)
            {
                reportTitle = reportTitle + " - " + _context.HealthFacilities.Find(vm.HealthFacilityId).Name;
            }
            if (vm.SubCountyId != null)
            {
                reportTitle = reportTitle + " - " + _context.SubCounties.Find(vm.SubCountyId).Name + " Sub County";
            }
            if (vm.WardId != null)
            {
                reportTitle = reportTitle + " - " + _context.Wards.Find(vm.WardId).Name + " Ward";
            }

            ViewBag.Title = reportTitle;

            if (vm.Option != null && vm.Option.Equals("export"))
            {
                string queryString = _exportService.GetQueryString(vm);
                var url = "beneficiary/movedout?" + queryString;
                var file = _exportService.ExportToPDF(url);
                return File(file, "application/pdf", reportTitle.Replace(" ", "-") + ".pdf");
            }

            if (vm.ReportTypeId != null)
            {
                if (vm.ReportTypeId == 1)
                {
                    if (vm.StatusId == 1)
                        vm.StatusId = null;

                    SqlParameter[] parms = new SqlParameter[]
                    {
                         new SqlParameter("@StatusId", vm.StatusId ?? (object) DBNull.Value),
                        new SqlParameter("@HealthFacilityId", vm.HealthFacilityId ?? (object)DBNull.Value),
                        new SqlParameter("@SubCountyId", vm.SubCountyId ?? (object)DBNull.Value),
                        new SqlParameter("@WardId", vm.WardId ?? (object)DBNull.Value),
                        new SqlParameter("@Option", "MovedOut"),
                    };
                    var result =
                        SQLExtensions.GetModelFromQuery<HHSummaryViewModel>(_context,
                            "EXEC [ReportCVSummary] @StatusId,@HealthFacilityId,@SubCountyId,@WardId,@Option", parms);
                    vm.Summaries = result;

                    return View("MovedOutSummary", vm);
                }

                IQueryable<CVListDetail> details = _context.CvListDetails
                    .Include(r => r.Household.Village.Ward.SubCounty)
                    .Include(r => r.Household.CommunityArea)
                    .Include(r => r.Household.Status)
                    .Include(r => r.Status)
                    .Include(r => r.CVHouseHold)
                    .Include(r => r.Household.HealthFacility)
                    .Include(r => r.Household.Mother)
                    .Include(r => r.VarianceCategory)
                    .Include(r => r.Enumerator)
                    .OrderBy(i => i.Household.Mother.FirstName).Where(i => i.List.ListTypeId == 211 && i.CannotFindHouseholdReasonId == 353);

                if (vm.StatusId != null)
                {
                    if (vm.StatusId == 1)
                        vm.StatusId = null;
                    details = details.Where(h => h.StatusId == vm.StatusId);
                }
                if (vm.WardId != null)
                {
                    details = details.Where(h => h.Household.Village.WardId == vm.WardId);
                }
                if (vm.SubCountyId != null)
                {
                    details = details.Where(h => h.Household.Village.Ward.SubCountyId == vm.SubCountyId);
                }
                if (vm.HealthFacilityId != null)
                {
                    details = details.Where(h => h.Household.HealthFacility.Id == vm.HealthFacilityId);
                }

                var page = vm.Page ?? 1;
                var pageSize = vm.PageSize ?? 20;
                vm.Total = details.Count();
                vm.Details = details.ToPagedList(page, pageSize);
            }

            return View(vm);
        }

        public async Task<IActionResult> Variance(VarianceReportViewModel vm)
        {
            ViewData["ReportTypeId"] = new SelectList(_context.ReportTypes, "Id", "Name", vm.ReportTypeId);
            ViewData["VarianceCategoryId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Variance Category"), "Id", "Code", vm.VarianceCategoryId);

            ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails
                .Where(i => i.SystemCode.Code == "Beneficiary Status"), "Id", "Code", vm.StatusId);
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities, "Id", "Name", vm.HealthFacilityId);
            ViewData["SubCountyId"] = new SelectList(_context.Constituencies, "Id", "Name", vm.SubCountyId);
            vm.Wards = await _context.Wards.ToListAsync();
            vm.CVList = _context.CVLists.Find(vm.Id);

            var reportTitle = (vm.ReportTypeId == 1 ? "Summary of" : "List of");

            if (vm.VarianceCategoryId != null)
            {
                reportTitle = reportTitle + " - " + _context.SystemCodeDetails.Find(vm.VarianceCategoryId).Code;
            }

            reportTitle = reportTitle + " Variance Report";

            if (vm.HealthFacilityId != null)
            {
                reportTitle = reportTitle + " - " + _context.HealthFacilities.Find(vm.HealthFacilityId).Name;
            }
            if (vm.SubCountyId != null)
            {
                reportTitle = reportTitle + " - " + _context.SubCounties.Find(vm.SubCountyId).Name + " Sub County";
            }

            if (vm.WardId != null)
            {
                reportTitle = reportTitle + " - " + _context.Wards.Find(vm.WardId).Name + " Ward";
            }

            ViewBag.Title = reportTitle;
            if (vm.ReportTypeId != null)
            {
                if (vm.Option != null && vm.Option.Equals("export"))
                {
                    string queryString = _exportService.GetQueryString(vm);
                    var url = "beneficiary/variance?" + queryString;
                    var file = _exportService.ExportToPDF(url);
                    return File(file, "application/pdf", reportTitle.Replace(" ", "-") + ".pdf");
                }
                if (vm.ReportTypeId == 1)
                {
                    if (vm.StatusId == 1)
                        vm.StatusId = null;

                    SqlParameter[] parms = new SqlParameter[]
                    {
                         new SqlParameter("@StatusId", vm.StatusId ?? (object) DBNull.Value),
                        new SqlParameter("@HealthFacilityId", vm.HealthFacilityId ?? (object)DBNull.Value),
                        new SqlParameter("@SubCountyId", vm.SubCountyId ?? (object)DBNull.Value),
                        new SqlParameter("@WardId", vm.WardId ?? (object)DBNull.Value),
                        new SqlParameter("@VarianceCategoryId", vm.VarianceCategoryId ?? (object)DBNull.Value),
                    };
                    var result =
                        SQLExtensions.GetModelFromQuery<VarianceSummaryViewModel>(_context,
                            "EXEC [ReportCVVarianceSummary] @StatusId,@HealthFacilityId,@SubCountyId,@WardId,@VarianceCategoryId", parms);
                    vm.Summaries = result;

                    return View("VarianceSummary", vm);
                }
                IQueryable<CVListDetail> details = _context.CvListDetails
                .Include(r => r.Household.Status)
                .Include(r => r.CVHouseHold)
                .Include(r => r.Household.Village.Ward.SubCounty)
                .Include(r => r.Household.CommunityArea)
                .Include(r => r.Household.HealthFacility)
                .Include(r => r.Household.Mother)
                .Include(r => r.VarianceCategory)
                    .OrderBy(i => i.Household.Mother.FirstName)
                .Where(i => i.VarianceCategoryId != null && 
                            (i.List.ListTypeId==211 || i.List.ListTypeId == 357)); //CHV List, Top-up List
                var healthFacilityId = _dbService.GetHealthFacilityId();
                if (healthFacilityId != 0)
                {
                    bool isGlobal = await _dbService.IsGlobal();
                    details = details.Where(i => i.Household.HealthFacilityId == healthFacilityId || isGlobal);
                }
                if (vm.HealthFacilityId != null)
                {
                    details = details.Where(i => i.Household.HealthFacilityId == vm.HealthFacilityId);
                }
                if (vm.WardId != null)
                {
                    details = details.Where(h => h.Household.Village.WardId == vm.WardId);
                }
                if (vm.SubCountyId != null)
                {
                    details = details.Where(h => h.Household.Village.Ward.SubCountyId == vm.SubCountyId);
                }
                if (vm.VarianceCategoryId != null)
                {
                    details = details.Where(h => h.VarianceCategoryId == vm.VarianceCategoryId);
                }
                var page = vm.Page ?? 1;
                var pageSize = vm.PageSize ?? 20;
                // var onePageOfProducts = households.ToPagedList(page, pageSize);
                vm.Total = details.Count();
                vm.Details = details.ToPagedList(page, pageSize);
            }

            return View(vm);
        }

        public async Task<IActionResult> NotEnrolled(HouseholdsReportViewModel vm)
        {
            ViewData["ReportTypeId"] = new SelectList(_context.ReportTypes, "Id", "Name", vm.ReportTypeId);
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Name", vm.StatusId);
            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name", vm.SubCountyId);
            ViewData["HealthFacilityID"] = new SelectList(_context.HealthFacilities, "Id", "Name", vm.HealthFacilityId);
            vm.Wards = await _context.Wards.ToListAsync();
            var reportTitle = (vm.ReportTypeId == 1 ? "Summary of" : "List of") + " Beneficiaries Not Enrolled";

            if (vm.HealthFacilityId != null)
            {
                reportTitle = reportTitle + " - " + _context.HealthFacilities.Find(vm.HealthFacilityId).Name;
            }
            if (vm.SubCountyId != null)
            {
                reportTitle = reportTitle + " - " + _context.SubCounties.Find(vm.SubCountyId).Name + " Sub County";
            }
            if (vm.WardId != null)
            {
                reportTitle = reportTitle + " - " + _context.Wards.Find(vm.WardId).Name + " Ward";
            }

            ViewBag.Title = reportTitle;

            if (vm.Option != null && vm.Option.Equals("export"))
            {
                string queryString = _exportService.GetQueryString(vm);
                var url = "beneficiary/notenrolled?" + queryString;
                var file = _exportService.ExportToPDF(url);
                return File(file, "application/pdf", reportTitle.Replace(" ", "-") + ".pdf");
            }

            if (vm.ReportTypeId != null)
            {
                if (vm.ReportTypeId == 1)
                {
                    SqlParameter[] parms = new SqlParameter[]
                    {
                         new SqlParameter("@StatusId", vm.StatusId ?? (object) DBNull.Value),
                        new SqlParameter("@HealthFacilityId", vm.HealthFacilityId ?? (object)DBNull.Value),
                        new SqlParameter("@SubCountyId", vm.SubCountyId ?? (object)DBNull.Value),
                        new SqlParameter("@WardId", vm.WardId ?? (object)DBNull.Value),
                    };
                    var result =
                        SQLExtensions.GetModelFromQuery<HHSummaryViewModel>(_context,
                            "EXEC [ReportsNotEnrolledSummary] @StatusId,@HealthFacilityId,@SubCountyId,@WardId", parms);
                    vm.Summaries = result;
                    return View("NotEnrolledSummary", vm);
                }
                else
                {
                    var households = _context.HouseholdRegs
                        .Include(r => r.Village.Ward.SubCounty)
                        .Include(r => r.Mother)
                        .Include(r => r.Status)
                        .Include(r => r.HealthFacility)
                        .Where(r => r.TypeId == 1 && r.StatusId == 10) // Ready for enrolment
                        .OrderBy(r => r.Mother.FirstName)
                        .AsQueryable();

                    if (!string.IsNullOrEmpty(vm.UniqueId))
                    {
                        households = households.Where(h => h.UniqueId == vm.UniqueId);
                    }
                    if (!string.IsNullOrEmpty(vm.IdNumber))
                    {
                        households = households.Where(h => h.Mother.IdNumber.Contains(vm.IdNumber));
                    }
                    if (!string.IsNullOrEmpty(vm.Name))
                    {
                        households = households.Where(h =>
                            h.Mother.FirstName.Contains(vm.Name)
                            || h.Mother.MiddleName.Contains(vm.Name)
                            || h.Mother.Surname.Contains(vm.Name)
                            || h.Mother.Surname.Contains(vm.Name)
                        );
                    }
                    if (vm.StatusId != null)
                    {
                        households = households.Where(h => h.StatusId == vm.StatusId);
                    }
                    if (vm.WardId != null)
                    {
                        households = households.Where(h => h.Village.WardId == vm.WardId);
                    }
                    if (vm.SubCountyId != null)
                    {
                        households = households.Where(h => h.Village.Ward.SubCountyId == vm.SubCountyId);
                    }
                    if (vm.HealthFacilityId != null)
                    {
                        households = households.Where(h => h.HealthFacility.Id == vm.HealthFacilityId);
                    }
                    var page = vm.Page ?? 1;
                    var pageSize = vm.PageSize ?? 20;
                    vm.Total = households.Count();
                    vm.HouseholdRegs = households.ToPagedList(page, pageSize);
                }
            }

            return View(vm);
        }

        public async Task<IActionResult> Enrolled(EnrolledListViewModel vm)
        {
            ViewData["ReportTypeId"] = new SelectList(_context.ReportTypes, "Id", "Name", vm.ReportTypeId);
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Name", vm.StatusId);
            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name", vm.SubCountyId);
            ViewData["HealthFacilityID"] = new SelectList(_context.HealthFacilities, "Id", "Name", vm.HealthFacilityId);
            vm.Wards = await _context.Wards.ToListAsync();
            var reportTitle = (vm.ReportTypeId == 1 ? "Summary of" : "List of") + " Enrolled Beneficiaries";

            if (vm.HealthFacilityId != null)
            {
                reportTitle = reportTitle + " - " + _context.HealthFacilities.Find(vm.HealthFacilityId).Name;
            }
            if (vm.SubCountyId != null)
            {
                reportTitle = reportTitle + " - " + _context.SubCounties.Find(vm.SubCountyId).Name + " Sub County";
            }
            if (vm.WardId != null)
            {
                reportTitle = reportTitle + " - " + _context.Wards.Find(vm.WardId).Name + " Ward";
            }

            ViewBag.Title = reportTitle;

            if (vm.Option != null && vm.Option.Equals("export"))
            {
                string queryString = _exportService.GetQueryString(vm);
                var url = "beneficiary/enrolled?" + queryString;
                var file = _exportService.ExportToPDF(url);
                return File(file, "application/pdf", reportTitle.Replace(" ", "-") + ".pdf");
            }
            if (vm.ReportTypeId == 1)
            {
                SqlParameter[] parms = new SqlParameter[]
                {
                         new SqlParameter("@StatusId", vm.StatusId ?? (object) DBNull.Value),
                        new SqlParameter("@HealthFacilityId", vm.HealthFacilityId ?? (object)DBNull.Value),
                        new SqlParameter("@SubCountyId", vm.SubCountyId ?? (object)DBNull.Value),
                        new SqlParameter("@WardId", vm.WardId ?? (object)DBNull.Value),
                        new SqlParameter("@StartDate",vm.StartDate ?? (object)DBNull.Value),
                        new SqlParameter("@EndDate",vm.EndDate ?? (object)DBNull.Value),
                };
                var result =
                    SQLExtensions.GetModelFromQuery<HHSummaryViewModel>(_context,
                        "EXEC [ReportsBeneficiarySummary] @StatusId,@HealthFacilityId,@SubCountyId,@WardId,@StartDate,@EndDate", parms);
                vm.Summaries = result;
                return View("EnrolledSummary", vm);
            }

            if (vm.ReportTypeId == 2)
            {
                var beneficiaries = _context.Beneficiaries
                    .Include(r => r.Household.Village.Ward.SubCounty)
                    .Include(r => r.Status)
                    .Include(r => r.HealthFacility)
                    .OrderBy(r => r.Mother.FirstName)
                    .AsQueryable();
                var healthFacilityId = _dbService.GetHealthFacilityId();
                if (healthFacilityId != 0)
                {
                    bool isGlobal = await _dbService.IsGlobal();
                    beneficiaries =
                        beneficiaries.Where(i => i.Household.HealthFacilityId == healthFacilityId || isGlobal);
                }

                if (vm.HealthFacilityId != null)
                {
                    beneficiaries = beneficiaries.Where(i => i.Household.HealthFacilityId == vm.HealthFacilityId);
                }

                if (!string.IsNullOrEmpty(vm.UniqueId))
                {
                    beneficiaries = beneficiaries.Where(h => h.UniqueId == vm.UniqueId);
                }

                if (!string.IsNullOrEmpty(vm.IdNumber))
                {
                    beneficiaries = beneficiaries.Where(h => h.IdNumber.Contains(vm.IdNumber));
                }

                if (!string.IsNullOrEmpty(vm.Name))
                {
                    beneficiaries = beneficiaries.Where(h =>
                        h.BeneficiaryName.Contains(vm.Name)
                    );
                }
                if (vm.StartDate != null)
                {
                    beneficiaries = beneficiaries.Where(h => h.DateEnrolled >= DateTime.Parse(vm.StartDate));
                }
                if (vm.EndDate != null)
                {
                    beneficiaries = beneficiaries.Where(h => h.DateEnrolled <= DateTime.Parse(vm.EndDate));
                }
                if (vm.StatusId != null)
                {
                    beneficiaries = beneficiaries.Where(h => h.StatusId == vm.StatusId);
                }

                if (vm.WardId != null)
                {
                    beneficiaries = beneficiaries.Where(h => h.Household.Village.WardId == vm.WardId);
                }

                if (vm.SubCountyId != null)
                {
                    beneficiaries = beneficiaries.Where(h => h.Household.Village.Ward.SubCountyId == vm.SubCountyId);
                }

                if (vm.HealthFacilityId != null)
                {
                    beneficiaries = beneficiaries.Where(h => h.HealthFacilityId == vm.HealthFacilityId);
                }

                var page = vm.Page ?? 1;
                var pageSize = vm.PageSize ?? 20;
                vm.Total = beneficiaries.Count();
                vm.Beneficiaries = beneficiaries.ToPagedList(page, pageSize);
            }

            return View(vm);
        }

        public async Task<IActionResult> Nominees(HouseholdsReportViewModel vm)
        {
            ViewData["ReportTypeId"] = new SelectList(_context.ReportTypes, "Id", "Name", vm.ReportTypeId);
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Name", vm.StatusId);
            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name", vm.SubCountyId);
            ViewData["HealthFacilityID"] = new SelectList(_context.HealthFacilities, "Id", "Name", vm.HealthFacilityId);
            vm.Wards = await _context.Wards.ToListAsync();
            var reportTitle = (vm.ReportTypeId == 1 ? "Summary of" : "List of") + " mothers using nominees to get their cash transfer";

            if (vm.HealthFacilityId != null)
            {
                reportTitle = reportTitle + " - " + _context.HealthFacilities.Find(vm.HealthFacilityId).Name;
            }
            if (vm.SubCountyId != null)
            {
                reportTitle = reportTitle + " - " + _context.SubCounties.Find(vm.SubCountyId).Name + " Sub County";
            }
            if (vm.WardId != null)
            {
                reportTitle = reportTitle + " - " + _context.Wards.Find(vm.WardId).Name + " Ward";
            }

            ViewBag.Title = reportTitle;

            if (vm.Option != null && vm.Option.Equals("export"))
            {
                string queryString = _exportService.GetQueryString(vm);
                var url = "beneficiary/Nominees?" + queryString;
                var file = _exportService.ExportToPDF(url);
                return File(file, "application/pdf", reportTitle.Replace(" ", "-") + ".pdf");
            }

            if (vm.ReportTypeId != null)
            {
                if (vm.ReportTypeId == 1)
                {
                    SqlParameter[] parms = new SqlParameter[]
                    {
                         new SqlParameter("@StatusId", vm.StatusId ?? (object) DBNull.Value),
                        new SqlParameter("@HealthFacilityId", vm.HealthFacilityId ?? (object)DBNull.Value),
                        new SqlParameter("@SubCountyId", vm.SubCountyId ?? (object)DBNull.Value),
                        new SqlParameter("@WardId", vm.WardId ?? (object)DBNull.Value),
                        new SqlParameter("@Option","Nominees"),
                        new SqlParameter("@StartDate",vm.StartDate ?? (object)DBNull.Value),
                        new SqlParameter("@EndDate",vm.EndDate ?? (object)DBNull.Value),
                    };
                    var result =
                        SQLExtensions.GetModelFromQuery<HHSummaryViewModel>(_context,
                            "EXEC [HouseholdsSummary] @StatusId,@HealthFacilityId,@SubCountyId,@WardId,@Option,@StartDate,@EndDate", parms);
                    vm.Summaries = result;
                    return View("NomineesSummary", vm);
                }

                var households = _context.HouseholdRegs
                    .Include(r => r.Village.Ward.SubCounty)
                    .Include(r => r.Mother)
                    .Include(r => r.Status)
                    .Include(r => r.HealthFacility)
                    .Where(r => r.TypeId == 1 && r.HasProxy == true).OrderBy(r => r.Mother.FirstName)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(vm.UniqueId))
                {
                    households = households.Where(h => h.UniqueId == vm.UniqueId);
                }
                if (!string.IsNullOrEmpty(vm.IdNumber))
                {
                    households = households.Where(h => h.Mother.IdNumber.Contains(vm.IdNumber));
                }
                if (!string.IsNullOrEmpty(vm.Name))
                {
                    households = households.Where(h =>
                        h.Mother.FirstName.Contains(vm.Name)
                        || h.Mother.MiddleName.Contains(vm.Name)
                        || h.Mother.Surname.Contains(vm.Name)
                        || h.Mother.Surname.Contains(vm.Name)
                    );
                }
                if (vm.StatusId != null)
                {
                    households = households.Where(h => h.StatusId == vm.StatusId);
                }
                if (vm.WardId != null)
                {
                    households = households.Where(h => h.Village.WardId == vm.WardId);
                }
                if (vm.SubCountyId != null)
                {
                    households = households.Where(h => h.Village.Ward.SubCountyId == vm.SubCountyId);
                }
                if (vm.HealthFacilityId != null)
                {
                    households = households.Where(h => h.HealthFacility.Id == vm.HealthFacilityId);
                }
                if (vm.StartDate != null)
                {
                    households = households.Where(h => h.DateCreated >= DateTime.Parse(vm.StartDate));
                }
                if (vm.EndDate != null)
                {
                    households = households.Where(h => h.DateCreated <= DateTime.Parse(vm.EndDate));
                }
                var page = vm.Page ?? 1;
                var pageSize = vm.PageSize ?? 20;
                vm.Total = households.Count();
                vm.HouseholdRegs = households.ToPagedList(page, pageSize);
            }

            return View(vm);
        }

        public async Task<IActionResult> Search(RegistrationListViewModel vm, string postBack)
        {
            if (!string.IsNullOrEmpty(postBack))
            {
                var households = _context.HouseholdRegs
                    .Include(r => r.Village.Ward.SubCounty)
                    .Include(r => r.Mother)
                    .Include(r => r.Status)
                    .Include(r => r.HealthFacility)
                    .Where(r => r.TypeId == 1).OrderBy(r => r.Mother.FirstName)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(vm.UniqueId))
                {
                    households = households.Where(h => h.UniqueId == vm.UniqueId);
                }
                if (!string.IsNullOrEmpty(vm.IdNumber))
                {
                    households = households.Where(h => h.Mother.IdNumber.Contains(vm.IdNumber));
                }
                if (!string.IsNullOrEmpty(vm.Name))
                {
                    households = households.Where(h =>
                        h.Mother.FirstName.Contains(vm.Name)
                        || h.Mother.MiddleName.Contains(vm.Name)
                        || h.Mother.Surname.Contains(vm.Name)
                        || h.Mother.Surname.Contains(vm.Name)
                    );
                }

                vm.HouseholdRegs = await households.ToListAsync();
                if (vm.HouseholdRegs.Count == 1)
                {
                    return RedirectToAction("Statement", new { id = vm.HouseholdRegs.First().Id });
                }
            }

            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Name", vm.StatusId);

            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name", vm.SubCountyId);
            vm.Wards = await _context.Wards.ToListAsync();
            return View(vm);
        }

        public async Task<IActionResult> Statement(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.Id = id;
            var houseHold = _context.HouseholdRegs
                .Include(i => i.HealthFacility)
                .Include(i => i.Status)
                .Include(i => i.Mother)
                .Include(i => i.Notes)
                .Include(i => i.Village.Ward.SubCounty.County)
                .Include(i => i.SubLocation.Location)
                .SingleOrDefault(h => h.Id == id);
            if (houseHold == null)
            {
                return NotFound();
            }
            var vm = new BeneficiaryStatementViewModel();
            vm.Household = houseHold;
            //vm.SupportStatus = _context.HouseholdRegMembers
            //    .Include(m => m.SupportStatus)
            //    .Single(i => i.Id == houseHold.MotherId).SupportStatus;

            vm.Disabilities = _context.HouseholdRegMemberDisabilities
                .Include(c => c.Disability)
                .Where(i => i.HouseholdRegMemberId == houseHold.MotherId)
                .ToList();
            // var emailBody = await this.RenderViewAsync("~/Areas/Export/Views/Beneficiary/Statement.cshtml", vm);
            return View(vm);
        }

        public IActionResult Changes(ChangesListViewModel vm, string id)
        {
            ViewBag.Id = id;
            var changes = _context.Changes
                .OrderBy(c => c.Household.Mother.FirstName)
                .Include(c => c.ActionedBy)
                .Include(c => c.ChangeType)
                .Include(c => c.CreatedBy)
                .Include(c => c.Household.Mother)
                .Include(c => c.Household.Village.Ward.SubCounty)
                .Include(c => c.MPESACheckStatus)
                .Include(c => c.Status).Where(i => i.HouseholdId == id);

            var page = vm.Page ?? 1;
            var pageSize = vm.PageSize ?? 20;

            vm.Changes = changes.ToPagedList(page, pageSize);

            return View(vm);
        }

        public IActionResult Complaints(ComplaintListViewModel vm, string id)
        {
            ViewBag.Id = id;
            var uniqueId = _context.HouseholdRegs.Find(id).UniqueId;
            var complaints = _context.Complaints
                .Include(c => c.ApprovedBy)
                .Include(c => c.ClosedBy)
                .Include(c => c.Category)
                .Include(c => c.ComplaintType)
                .Include(c => c.CreatedBy)
                .Include(c => c.EscalatedBy)
                .Include(c => c.HealthFacility)
                .Include(c => c.ResolvedBy)
                .Include(c => c.Status)
                .Include(c => c.Village.Ward)
                .Where(i => i.UniqueId == uniqueId)
                .OrderBy(i => i.Name);

            var page = vm.Page ?? 1;
            var pageSize = vm.PageSize ?? 20;
            vm.Complaints = complaints.ToPagedList(page, pageSize);
            return View(vm);
        }

        public IActionResult Payments(PaymentTransactionsViewModel vm, string id)
        {
            ViewBag.Id = id;
            var details = _context.PaymentTransactions
                .Include(i => i.Beneficiary.HealthFacility)
                .Include(r => r.Village.Ward.SubCounty)
                .Include(r => r.PaymentPoint)
                .Include(r => r.FundRequest.Cycle)
                .Include(r => r.Status)
                .Where(i => i.Beneficiary.HouseholdId == id);

            var page = vm.Page ?? 1;
            var pageSize = vm.PageSize ?? 20;
            vm.Details = details.ToPagedList(page, pageSize);

            return View(vm);
        }

        public async Task<IActionResult> ExportStatement(string id)
        {
            var hh = _context.HouseholdRegs.Include(i => i.Mother).Single(i => i.Id == id);
            var url = "beneficiary/statement/" + id;
            var file = _exportService.ExportToPDF(url);
            return File(file, "application/pdf", "Beneficiary Statement-" + hh.Mother.FullName.Replace(" ", "-") + ".pdf");
            // await _exportService.ExportToPDFAsync(url, "Beneficiary Statement-" + hh.Mother.FullName.Replace(" ", "-"));

            return RedirectToAction("Statement", new { id });
        }
    }

    public static class ControllerExtensions
    {
        public static async Task<string> RenderViewAsync<TModel>(this Controller controller, string viewName, TModel model, bool isPartial = false)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                viewName = controller.ControllerContext.ActionDescriptor.ActionName;
            }

            controller.ViewData.Model = model;

            using (var writer = new StringWriter())
            {
                IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                ViewEngineResult viewResult = GetViewEngineResult(controller, viewName, isPartial, viewEngine);

                if (viewResult.Success == false)
                {
                    throw new System.Exception($"A view with the name {viewName} could not be found");
                }

                ViewContext viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);

                return writer.GetStringBuilder().ToString();
            }
        }

        private static ViewEngineResult GetViewEngineResult(Controller controller, string viewName, bool isPartial, IViewEngine viewEngine)
        {
            if (viewName.StartsWith("~/"))
            {
                var hostingEnv = controller.HttpContext.RequestServices.GetService(typeof(IHostingEnvironment)) as IHostingEnvironment;
                return viewEngine.GetView(hostingEnv.WebRootPath, viewName, !isPartial);
            }
            else
            {
                return viewEngine.FindView(controller.ControllerContext, viewName, !isPartial);
            }
        }
    }
}