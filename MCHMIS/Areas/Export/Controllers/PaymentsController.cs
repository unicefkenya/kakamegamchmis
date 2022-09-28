using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using MCHMIS.Areas.Reports.ViewModels;
using MCHMIS.Data;
using MCHMIS.Extensions;
using MCHMIS.Interfaces;
using MCHMIS.Services;
using MCHMIS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using PaymentsViewModel = MCHMIS.Areas.Reports.ViewModels.PaymentsViewModel;
using PaymentTransactionsViewModel = MCHMIS.Areas.Reports.ViewModels.PaymentTransactionsViewModel;

namespace MCHMIS.Areas.Export.Controllers
{
    [Area("Export")]
    [AllowAnonymous]
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
            var reportTitle = "Payrolls Details";
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

                    var count = details.Count();
                    var page = vm.Page ?? 1;
                    var pageSize = vm.PageSize ?? count;
                    vm.Total = details.Sum(i => i.Amount);
                    vm.Details = details.ToPagedList(page, pageSize);
                    return View("PayrollDetails", vm);
                }
            }

            return View(vm);
        }

        public IActionResult Disbursements(PaymentsViewModel vm)
        {
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
                    var details = _context.Query<ViewReportsDisbursements>()
                       .AsQueryable();

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
                    var pageSize = vm.PageSize ?? details.Count();
                    vm.Total = details.Sum(i => i.Amount);
                    vm.DisbursementDetails = details.ToPagedList(page, pageSize);
                    return View("DisbursementsDetails", vm);
                }
            }

            return View(vm);
        }

        public IActionResult Due(PaymentsViewModel vm)
        {
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
                    var beneficiaryHHIds = _context.Beneficiaries.Select(i => i.HouseholdId);
                    var details = _context.BeneficiaryPaymentPoints
                        .Include(i => i.PaymentPoint)
                        .Include(i => i.Household.Mother)
                        .Include(i => i.Household.HealthFacility)
                        .Include(r => r.Household.Village.Ward.SubCounty)
                        .Where(i => i.StatusId == 1 && beneficiaryHHIds.Contains(i.HouseholdId));

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
                    var pageSize = vm.PageSize ?? details.Count();
                    vm.Total = details.Sum(i => i.PaymentPoint.Amount);
                    vm.BeneficiaryPaymentPoints = details.ToPagedList(page, pageSize);
                    return View("DueDetails", vm);
                }
            }

            return View(vm);
        }

        public IActionResult Forfeited(PaymentsViewModel vm)
        {
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
                    var pageSize = vm.PageSize ?? details.Count();
                    vm.Total = details.Sum(i => i.PaymentPoint.Amount);
                    vm.BeneficiaryPaymentPoints = details.ToPagedList(page, pageSize);
                    return View("ForfeitedDetails", vm);
                }
            }

            return View(vm);
        }

        public IActionResult Pending(PaymentsViewModel vm)
        {
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
                    var pageSize = vm.PageSize ?? result.Count();
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

        public IActionResult FundRequest(PaymentTransactionsViewModel vm, string ti)
        {
            var details = _context.PaymentTransactions
                .Include(i => i.Beneficiary.HealthFacility)
                .Include(r => r.Village.Ward.SubCounty)
                .Include(r => r.PaymentPoint)
                .Include(r => r.Status)
                .OrderBy(i => i.Beneficiary.HealthFacility)
                .ThenBy(i => i.BeneficiaryName)
                .Where(i => i.FundRequestId == vm.Id);

            if (!string.IsNullOrEmpty(vm.UniqueId))
            {
                details = details.Where(h => h.Beneficiary.UniqueId == vm.UniqueId);
            }
            if (!string.IsNullOrEmpty(vm.IdNumber))
            {
                details = details.Where(h => h.Beneficiary.IdNumber.Contains(vm.IdNumber));
            }
            if (!string.IsNullOrEmpty(vm.Name))
            {
                details = details.Where(h =>
                    h.Beneficiary.BeneficiaryName.Contains(vm.Name)
                );
            }
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
            var reportTitle = _context.FundRequests.Include(i => i.Cycle).Single(i => i.Id == vm.Id).Cycle.Name + " Fund Request";
            ViewBag.Title = reportTitle;

            vm.Total = details.Sum(i => i.Amount);
            var page = vm.Page ?? 1;
            var pageSize = details.Count();
            vm.Details = details.ToPagedList(page, pageSize);
            vm.FundRequest = _context.FundRequests.Find(vm.Id);
            vm.Wards = _context.Wards.ToList();
            ViewBag.HealthFacilityId = new SelectList(_context.HealthFacilities.OrderBy(i => i.Name), "Id", "Name", vm.HealthFacilityId);
            ViewBag.SubCountyId = new SelectList(_context.SubCounties.OrderBy(i => i.Name), "Id", "Name", vm.SubCountyId);
            return View(vm);
        }
    }
}