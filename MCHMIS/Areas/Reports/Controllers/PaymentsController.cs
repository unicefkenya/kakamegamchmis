using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using MCHMIS.Areas.Reports.ViewModels;
using MCHMIS.Data;
using MCHMIS.Interfaces;
using MCHMIS.Services;
using MCHMIS.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using PaymentsViewModel = MCHMIS.Areas.Reports.ViewModels.PaymentsViewModel;

namespace MCHMIS.Areas.Reports.Controllers
{
    [Area("Reports")]
    public class PaymentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IDBService _dbService;

        public PaymentsController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment,
            IUnitOfWork uow,
            IExportService exportService,
            IDBService dbService)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _uow = uow;
            _dbService = dbService;
            _exportService = exportService;
        }

        public IActionResult Index(PaymentsViewModel vm)
        {
            ViewData["ReportTypeId"] = new SelectList(_context.ReportTypes, "Id", "Name", vm.ReportTypeId);
            ViewData["PayrollId"] = new SelectList(_context.Payrolls.OrderByDescending(i=>i.Id).Include(i => i.Cycle), "Id", "Cycle.Name", vm.ReportTypeId);
            ViewData["SubCountyId"] = new SelectList(_context.Constituencies, "Id", "Name", vm.SubCountyId);
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities, "Id", "Name", vm.HealthFacilityId);
            ViewData["PaymentStageId"] = new SelectList(_context.PaymentPoints, "Id", "Name", vm.PaymentStageId);
            vm.Wards = _context.Wards.ToList();
            vm.Wards = _context.Wards.ToList();

            var reportTitle = (vm.ReportTypeId == 1 ? "Summary of" : "List of") + " Payrolls Details";
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
                var url = "payments?" + queryString;
                var file = _exportService.ExportToPDF(url);
                return File(file, "application/pdf", reportTitle.Replace(" ", "-") + ".pdf");
            }

            if (vm.ReportTypeId != null)
            {
                if (vm.ReportTypeId == 1)
                {
                    SqlParameter[] parms = new SqlParameter[]
                    {
                        new SqlParameter("PayrollId", vm.PayrollId ?? (object)DBNull.Value),
                        new SqlParameter("PaymentPointId", vm.PaymentStageId ?? (object)DBNull.Value),
                        new SqlParameter("HealthFacilityId", vm.HealthFacilityId ?? (object)DBNull.Value),
                        new SqlParameter("SubCountyId", vm.SubCountyId ?? (object)DBNull.Value),
                        new SqlParameter("WardId", vm.WardId ?? (object)DBNull.Value),
                    };
                    var result =
                        SQLExtensions.GetModelFromQuery<DisbursementViewModel>(_context,
                            "EXEC [PayrollSummary] @PayrollId,@PaymentPointId,@HealthFacilityId,@SubCountyId,@WardId", parms);
                    vm.Total = result.Sum(i => i.Amount);
                    vm.Disbursements = result;
                }
                else
                {
                    var details = _context.Payments
                        .Include(i => i.Beneficiary.HealthFacility)
                        .Include(r => r.Village.Ward.SubCounty)
                        .Include(r => r.Status)
                        .Where(i => i.PayrollId == vm.PayrollId);

                    if (vm.WardId != null)
                    {
                        details = details.Where(h => h.Beneficiary.Village.WardId == vm.WardId);
                    }
                    if (vm.SubCountyId != null)
                    {
                        details = details.Where(h => h.Village.Ward.SubCountyId == vm.SubCountyId);
                    }
                    if (vm.HealthFacilityId != null)
                    {
                        details = details.Where(h => h.HealthFacilityId == vm.HealthFacilityId);
                    }

                    var page = vm.Page ?? 1;
                    var pageSize = vm.PageSize ?? 20;
                    vm.Total = details.Sum(i => i.Amount);
                    vm.Details = details.ToPagedList(page, pageSize);
                    return View("PayrollDetails", vm);
                }
            }

            return View(vm);
        }

        public IActionResult Disbursements(PaymentsViewModel vm)
        {
            ViewData["ReportTypeId"] = new SelectList(_context.ReportTypes, "Id", "Name", vm.ReportTypeId);
            ViewData["PayrollId"] = new SelectList(_context.Payrolls.Include(i => i.Cycle), "Id", "Cycle.Name", vm.ReportTypeId);
            ViewData["SubCountyId"] = new SelectList(_context.Constituencies, "Id", "Name", vm.SubCountyId);
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities, "Id", "Name", vm.HealthFacilityId);
            ViewData["PaymentStageId"] = new SelectList(_context.PaymentPoints, "Id", "Name", vm.PaymentStageId);
            vm.Wards = _context.Wards.ToList();

            var reportTitle = " of Grand Disbursements";
            if (vm.ReportTypeId != null && vm.ReportTypeId == 1)
            {
                reportTitle = "Summary " + reportTitle;
            }
            else
            {
                reportTitle = "Details " + reportTitle;
            }
            if (vm.PayrollId != null)
            {
                var payroll = _context.Payrolls.Include(i => i.Cycle).Single(i => i.Id == vm.PayrollId);
                reportTitle = reportTitle + " - " + payroll.Cycle.Name;
            }
            if (vm.PaymentStageId != null)
            {
                reportTitle = reportTitle + " - " + _context.PaymentPoints.Find(vm.PaymentStageId).Name;
            }
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
                var url = "payments/disbursements?" + queryString;
                var file = _exportService.ExportToPDF(url);
                return File(file, "application/pdf", reportTitle.Replace(" ", "-") + ".pdf");
            }

            if (vm.ReportTypeId != null)
            {
                if (vm.ReportTypeId == 1)
                {
                    SqlParameter[] parms = new SqlParameter[]
                    {
                        new SqlParameter("PayrollId", vm.PayrollId ?? (object) DBNull.Value),
                        new SqlParameter("PaymentPointId", vm.PaymentStageId ?? (object) DBNull.Value),
                        new SqlParameter("HealthFacilityId", vm.HealthFacilityId ?? (object) DBNull.Value),
                        new SqlParameter("SubCountyId", vm.SubCountyId ?? (object) DBNull.Value),
                        new SqlParameter("WardId", vm.WardId ?? (object) DBNull.Value),
                    };
                    vm.Disbursements = SQLExtensions.GetModelFromQuery<DisbursementViewModel>(_context,
                        "EXEC [ReportsDisbursements] @PayrollId,@PaymentPointId,@HealthFacilityId,@SubCountyId,@WardId",
                        parms);
                }
                else
                {
                    var details = _context.Query<ViewReportsDisbursements>().AsQueryable();

                    if (vm.PayrollId != null)
                    {
                        var frId = _context.Payrolls.Find(vm.PayrollId).Id;
                        details = details.Where(h => h.FundRequestId == frId);
                    }
                    if (vm.PaymentStageId != null)
                    {
                        details = details.Where(h => h.PaymentPointId == vm.PaymentStageId);
                    }
                   
                    if (vm.HealthFacilityId != null)
                    {
                        details = details.Where(h => h.HealthFacilityId == vm.HealthFacilityId);
                    }

                  
                    var page = vm.Page ?? 1;
                    var pageSize = vm.PageSize ?? 20;
                    vm.Total = details.Sum(i => i.Amount);
                    vm.DisbursementDetails = details.ToPagedList(page, pageSize);
                    return View("DisbursementsDetails", vm);
                }
            }

            return View(vm);
        }

        public IActionResult Due(PaymentsViewModel vm)
        {
            ViewData["ReportTypeId"] = new SelectList(_context.ReportTypes, "Id", "Name", vm.ReportTypeId);
            ViewData["PayrollId"] = new SelectList(_context.Payrolls.Include(i => i.Cycle), "Id", "Cycle.Name", vm.ReportTypeId);
            ViewData["SubCountyId"] = new SelectList(_context.Constituencies, "Id", "Name", vm.SubCountyId);
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities, "Id", "Name", vm.HealthFacilityId);
            ViewData["PaymentStageId"] = new SelectList(_context.PaymentPoints, "Id", "Name", vm.PaymentStageId);
            vm.Wards = _context.Wards.ToList();

            var reportTitle = (vm.ReportTypeId == 1 ? "Summary of" : "List of") + " Payments Due for Processing";
            if (vm.PaymentStageId != null)
            {
                reportTitle = reportTitle + " - " + _context.PaymentPoints.Find(vm.PaymentStageId).Name;
            }
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
                var url = "payments/due/?" + queryString;
                var file = _exportService.ExportToPDF(url);
                return File(file, "application/pdf", reportTitle.Replace(" ", "-") + ".pdf");
            }

            if (vm.ReportTypeId != null)
            {
                if (vm.ReportTypeId == 1)
                {
                    SqlParameter[] parms = new SqlParameter[]
                    {
                        new SqlParameter("PaymentPointId", vm.PaymentStageId ?? (object)DBNull.Value),
                        new SqlParameter("HealthFacilityId", vm.HealthFacilityId ?? (object)DBNull.Value),
                        new SqlParameter("SubCountyId", vm.SubCountyId ?? (object)DBNull.Value),
                        new SqlParameter("WardId", vm.WardId ?? (object)DBNull.Value),
                    };
                    var result =
                        SQLExtensions.GetModelFromQuery<DisbursementViewModel>(_context,
                            "EXEC [ReportsPaymentsDue] @PaymentPointId,@HealthFacilityId,@SubCountyId,@WardId", parms);
                    vm.Disbursements = result;
                }
                else
                {
                   
                    var details = _context.BeneficiaryPaymentPoints
                        .Include(i => i.PaymentPoint)
                        .Include(i => i.Household.Mother)
                        .Include(i => i.Household.HealthFacility)
                        .Include(r => r.Household.Village.Ward.SubCounty)
                        .Where(i => i.StatusId == 1 && 
                                    _context.Beneficiaries
                                        .Where(x => x.StatusId == 19 || x.StatusId== 27) // Enrolled or Exited
                                        .Select(x => x.HouseholdId)
                                        .Contains(i.HouseholdId));

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
                        details = details.Where(h => h.Household.HealthFacilityId == vm.HealthFacilityId);
                    }
                    if (vm.PaymentStageId != null)
                    {
                        details = details.Where(h => h.PaymentPointId == vm.PaymentStageId);
                    }
                    var page = vm.Page ?? 1;
                    var pageSize = vm.PageSize ?? 20;
                    vm.Total = details.Sum(i => i.PaymentPoint.Amount);
                    vm.BeneficiaryPaymentPoints = details.ToPagedList(page, pageSize);
                    return View("DueDetails", vm);
                }
            }

            return View(vm);
        }

        public IActionResult Forfeited(PaymentsViewModel vm)
        {
            ViewData["ReportTypeId"] = new SelectList(_context.ReportTypes, "Id", "Name", vm.ReportTypeId);
            ViewData["PayrollId"] = new SelectList(_context.Payrolls.Include(i => i.Cycle), "Id", "Cycle.Name", vm.ReportTypeId);
            ViewData["SubCountyId"] = new SelectList(_context.Constituencies, "Id", "Name", vm.SubCountyId);
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities, "Id", "Name", vm.HealthFacilityId);
            ViewData["PaymentStageId"] = new SelectList(_context.PaymentPoints, "Id", "Name", vm.PaymentStageId);
            vm.Wards = _context.Wards.ToList();

            var reportTitle = (vm.ReportTypeId == 1 ? "Summary of" : "List of") + " Forfeited Payments";
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
                var url = "payments/forfeited/?" + queryString;
                var file = _exportService.ExportToPDF(url);
                return File(file, "application/pdf", reportTitle.Replace(" ", "-") + ".pdf");
            }

            if (vm.ReportTypeId != null)
            {
                if (vm.ReportTypeId == 1)
                {
                    SqlParameter[] parms = new SqlParameter[]
                    {
                        new SqlParameter("PaymentPointId", vm.PaymentStageId ?? (object)DBNull.Value),
                        new SqlParameter("HealthFacilityId", vm.HealthFacilityId ?? (object)DBNull.Value),
                        new SqlParameter("SubCountyId", vm.SubCountyId ?? (object)DBNull.Value),
                        new SqlParameter("WardId", vm.WardId ?? (object)DBNull.Value),
                    };
                    var result =
                        SQLExtensions.GetModelFromQuery<DisbursementViewModel>(_context,
                            "EXEC [ReportsPaymentsForfeited] @PaymentPointId,@HealthFacilityId,@SubCountyId,@WardId", parms);
                    vm.Disbursements = result;
                }
                else
                {
                    var details = _context.BeneficiaryPaymentPoints
                        .Include(i => i.PaymentPoint)
                        .Include(i => i.Household.Mother)
                        .Include(i => i.Household.HealthFacility)
                        .Include(r => r.Household.Village.Ward.SubCounty)
                        .Where(i => i.StatusId == 5);

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
                        details = details.Where(h => h.Household.HealthFacilityId == vm.HealthFacilityId);
                    }
                    if (vm.PaymentStageId != null)
                    {
                        details = details.Where(h => h.PaymentPointId == vm.PaymentStageId);
                    }
                    var page = vm.Page ?? 1;
                    var pageSize = vm.PageSize ?? 20;
                    vm.Total = details.Sum(i => i.PaymentPoint.Amount);
                    vm.BeneficiaryPaymentPoints = details.ToPagedList(page, pageSize);
                    return View("ForfeitedDetails", vm);
                }
            }

            return View(vm);
        }

        public IActionResult Pending(PaymentsViewModel vm)
        {
            ViewData["ReportTypeId"] = new SelectList(_context.ReportTypes, "Id", "Name", vm.ReportTypeId);
            ViewData["PayrollId"] = new SelectList(_context.Payrolls.Include(i => i.Cycle), "Id", "Cycle.Name", vm.ReportTypeId);
            ViewData["SubCountyId"] = new SelectList(_context.Constituencies, "Id", "Name", vm.SubCountyId);
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities, "Id", "Name", vm.HealthFacilityId);
            ViewData["PaymentStageId"] = new SelectList(_context.PaymentPoints, "Id", "Name", vm.PaymentStageId);
            vm.Wards = _context.Wards.ToList();

            var reportTitle = (vm.ReportTypeId == 1 ? "Summary of" : "List of") + " Pending Payments";
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
                var url = "payments/pending/?" + queryString;
                var file = _exportService.ExportToPDF(url);
                return File(file, "application/pdf", reportTitle.Replace(" ", "-") + ".pdf");
            }

            if (vm.ReportTypeId != null)
            {
                if (vm.ReportTypeId == 2) // Detailed
                {
                    SqlParameter[] parms = new SqlParameter[]
                    {
                        new SqlParameter("PaymentPointId", vm.PaymentStageId ?? (object)DBNull.Value),
                        new SqlParameter("HealthFacilityId", vm.HealthFacilityId ?? (object)DBNull.Value),
                        new SqlParameter("SubCountyId", vm.SubCountyId ?? (object)DBNull.Value),
                        new SqlParameter("WardId", vm.WardId ?? (object)DBNull.Value),
                        new SqlParameter("Option", "Detailed"),
                    };
                    var result =
                        SQLExtensions.GetModelFromQuery<PendingPaymentsViewModel>(_context,
                            "EXEC [ReportsPaymentsPending] @PaymentPointId,@HealthFacilityId,@SubCountyId,@WardId,@Option", parms);
                    var page = vm.Page ?? 1;
                    var pageSize = vm.PageSize ?? 20;
                    vm.Total = result.Sum(i => i.Amount);
                    var queryable = result.AsQueryable();
                    vm.PendingPayments = queryable.ToPagedList(page, pageSize);
                    return View("PendingDetails", vm);
                }
                else // Summary
                {
                    SqlParameter[] parms = new SqlParameter[]
                    {
                        new SqlParameter("PaymentPointId", vm.PaymentStageId ?? (object)DBNull.Value),
                        new SqlParameter("HealthFacilityId", vm.HealthFacilityId ?? (object)DBNull.Value),
                        new SqlParameter("SubCountyId", vm.SubCountyId ?? (object)DBNull.Value),
                        new SqlParameter("WardId", vm.WardId ?? (object)DBNull.Value),
                        new SqlParameter("Option", "Summary"),
                    };
                    var result =
                        SQLExtensions.GetModelFromQuery<DisbursementViewModel>(_context,
                            "EXEC [ReportsPaymentsPending] @PaymentPointId,@HealthFacilityId,@SubCountyId,@WardId,@Option", parms).ToList();
                    var page = vm.Page ?? 1;
                    var pageSize = vm.PageSize ?? 20;
                    vm.Total = result.Sum(i => i.Amount);
                    vm.Disbursements = result;//.ToPagedList(page, pageSize);
                }
            }

            return View(vm);
        }

        public IActionResult Forecast(PaymentsViewModel vm)
        {
            ViewData["ReportTypeId"] = new SelectList(_context.ReportTypes, "Id", "Name", vm.ReportTypeId);
            ViewData["PayrollId"] = new SelectList(_context.Payrolls.Include(i => i.Cycle), "Id", "Cycle.Name", vm.ReportTypeId);
            ViewData["SubCountyId"] = new SelectList(_context.Constituencies, "Id", "Name", vm.SubCountyId);
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities, "Id", "Name", vm.HealthFacilityId);
            ViewData["PaymentStageId"] = new SelectList(_context.PaymentPoints, "Id", "Name", vm.PaymentStageId);
            vm.Wards = _context.Wards.ToList();

            var reportTitle = "Forecast Report";

            ViewBag.Title = reportTitle;

            if (vm.Option != null && vm.Option.Equals("export"))
            {
                string queryString = _exportService.GetQueryString(vm);
                var url = "payments/pending/?" + queryString;
                var file = _exportService.ExportToPDF(url);
                return File(file, "application/pdf", reportTitle.Replace(" ", "-") + ".pdf");
            }

            var page = vm.Page ?? 1;
            var pageSize = vm.PageSize ?? 20;
            SqlParameter[] parms;
            if (!string.IsNullOrEmpty(vm.Id))
            {
                ViewBag.Title = reportTitle + " - " + vm.Id;
                parms = new SqlParameter[]
               {
                    new SqlParameter("Option", "Details"),
                    new SqlParameter("year", vm.Id),
               };
                var data =
                    SQLExtensions.GetModelFromQuery<ForecastDetailViewModel>(_context,
                        "EXEC [ReportsForecast] @Option,@year", parms).ToList();

                vm.Total = data.Sum(i => i.Amount);
                vm.ForecastDetails = data.ToPagedList(page, pageSize);
                return View("ForecastDetailed", vm);
            }

            parms = new SqlParameter[]
            {
                //new SqlParameter("PaymentPointId", vm.PaymentStageId ?? (object)DBNull.Value),
                //new SqlParameter("HealthFacilityId", vm.HealthFacilityId ?? (object)DBNull.Value),
                // new SqlParameter("SubCountyId", vm.SubCountyId ?? (object)DBNull.Value),
                // new SqlParameter("WardId", vm.WardId ?? (object)DBNull.Value),
                new SqlParameter("Option", "Summary"),
            };
            var result =
                SQLExtensions.GetModelFromQuery<ForecastViewModel>(_context,
                    "EXEC [ReportsForecast] @Option", parms).ToList();
            vm.Total = result.Sum(i => i.Amount);
            vm.ForecastSummary = result;//.ToPagedList(page, pageSize);

            return View(vm);
        }
    }
}