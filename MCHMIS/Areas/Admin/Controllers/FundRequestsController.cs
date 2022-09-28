using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MCHMIS.Data;
using MCHMIS.Extensions;
using MCHMIS.ViewModels;
using X.PagedList;
using MCHMIS.Services;

namespace MCHMIS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Permission("Fund Request:View Fund Request")]
    public class FundRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IExportService _exportService;

        public FundRequestsController(ApplicationDbContext context, IExportService exportService)
        {
            _context = context;
            _exportService = exportService;
        }

        // GET: Admin/FundRequests
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.FundRequests
                .Include(f => f.ApprovedBy)
                .Include(f => f.CreatedBy)
                .Include(f => f.Cycle)
                .Include(f => f.Status)
                .OrderByDescending(i=>i.Id);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/FundRequests/Details/5
        public async Task<IActionResult> Details(PaymentTransactionsViewModel vm)
        {
            if (vm.Option != null && vm.Option.Equals("export"))
            {
                string queryString = _exportService.GetQueryString(vm);
                var url = "payments/fundrequest?" + queryString;
                var file = _exportService.ExportToPDF(url);
                var reportTitle = _context.FundRequests.Include(i => i.Cycle).Single(i => i.Id == vm.Id).Cycle.Name + " Fund Request";
                return File(file, "application/pdf", reportTitle.Replace(" ", "-") + ".pdf");
            }
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

            vm.Total = details.Sum(i => i.Amount);
            var page = vm.Page ?? 1;
            var pageSize = vm.PageSize ?? 20;
            vm.Details = details.ToPagedList(page, pageSize);
            vm.FundRequest = _context.FundRequests.Find(vm.Id);
            vm.Wards = _context.Wards.ToList();
            ViewBag.HealthFacilityId = new SelectList(_context.HealthFacilities.OrderBy(i => i.Name), "Id", "Name", vm.HealthFacilityId);
            ViewBag.SubCountyId = new SelectList(_context.SubCounties.OrderBy(i => i.Name), "Id", "Name", vm.SubCountyId);
            return View(vm);
        }

        // GET: Admin/FundRequests/Create
        public IActionResult Create()
        {
            ViewBag.PaymentCycleId =
       new SelectList(_context.PaymentCycles
       .Where(c => c.Closed == false && !_context.FundRequests.Select(j => j.CycleId).Contains(c.Id))
       .OrderByDescending(c => c.Id).ToList(), "Id",
           "Name");

            return View();
        }

        // POST: Admin/FundRequests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int paymentCycleId)
        {
            var prepayrollChecks = _context.PrePayrollChecks.SingleOrDefault(i => i.PaymentCycleId == paymentCycleId);
            if (prepayrollChecks == null)
            {
                TempData["info"] = "Pre-payroll checks for the payment cycle needs to be generated first.";
            }
            else if (prepayrollChecks.StatusId != 3)
            {
                TempData["info"] = "Pre-payroll checks for the payment cycle needs to be approved first.";
            }
            else
            {
                var userId = User.GetUserId();
                SqlParameter[] @params =
                {
                    new SqlParameter("cycleId", paymentCycleId),
                    new SqlParameter("createdById", userId),
                    new SqlParameter("OutFundRequestId", SqlDbType.Int) {Direction = ParameterDirection.Output},
                };

                int data =
                    _context.Database.ExecuteSqlCommand(";Exec FundsRequestGenerate @cycleId,@createdById,@OutFundRequestId OUT",
                        @params
                    );
                // _context.SaveChanges();
                var fundRequestId = @params[2].Value;
                if (data < 0)
                {
                    var paymentName = _context.PaymentCycles.Find(paymentCycleId).Name;
                    TempData["info"] = "Fund request for " + paymentName + " already generated.";
                    return RedirectToAction("Index");
                }

                TempData["success"] = "Fund request generated.";
                return RedirectToAction("Summary", new { id = fundRequestId });
            }
            return RedirectToAction("Index");
        }

        public IActionResult Summary(int id)
        {
            SqlParameter[] parms = new SqlParameter[] { new SqlParameter("@EnrolmentId", id) };
            var result = SQLExtensions.GetModelFromQuery<FRSummaryViewModel>(_context, "EXEC [FundRequestSummary] @EnrolmentId", parms);
            ViewBag.Id = id;
            return View(result);
        }

        public IActionResult SendForApproval(int id)
        {
            //var query = from p in _context.PaymentTransactions
            //    group p by p.FundRequestId into g
            //    select new
            //    {
            //        name = g.Key,
            //        count = g.Sum(i=>i.Amount)
            //    };

            var query = _context.PaymentTransactions.Where(i => i.FundRequestId == id)
                .GroupBy(p => p.HealthFacilityId)
                .Select(g => new FRApprovalSummaryViewModel
                {
                    Beneficiaries = g.Select(i => i.BeneficiaryId).Distinct().Count(),
                    Amount = g.Sum(i => i.Amount)
                }).ToList();

            var vm = new FRApprovalSummaryViewModel
            {
                Id = id,
                Beneficiaries = query.Sum(i => i.Beneficiaries),
                Amount = query.Sum(i => i.Amount),
                Facilities = query.Count(),
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<ActionResult> Send(FRApprovalSummaryViewModel vm)
        {
            var list = _context.FundRequests
                .Include(l => l.CreatedBy)
                .SingleOrDefault(l => l.Id == vm.Id);
            list.StatusId = 2;
            await _context.SaveChangesAsync();
            TempData["Message"] = "Fund Request sent for approval";
            return RedirectToAction("Index");
        }

        public IActionResult Action(int id)
        {
            var query = _context.PaymentTransactions.Where(i => i.FundRequestId == id)
                .GroupBy(p => p.HealthFacilityId)
                .Select(g => new FRApprovalSummaryViewModel
                {
                    Beneficiaries = g.Select(i => i.BeneficiaryId).Distinct().Count(),
                    Amount = g.Sum(i => i.Amount)
                }).ToList();

            var vm = new FRApprovalSummaryViewModel
            {
                Id = id,
                Beneficiaries = query.Sum(i => i.Beneficiaries),
                Amount = query.Sum(i => i.Amount),
                Facilities = query.Count,
                FundRequest = _context.FundRequests
                    .Include(l => l.CreatedBy)
                    .SingleOrDefault(l => l.Id == id)
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<ActionResult> ActionSave(FRApprovalSummaryViewModel vm)
        {
            var list = _context.FundRequests
                .SingleOrDefault(l => l.Id == vm.Id);
            list.StatusId = 3;
            list.Notes = vm.Notes;
            list.DateApproved = DateTime.UtcNow.AddHours(3);
            list.ApprovedById = User.GetUserId();
            await _context.SaveChangesAsync();

            TempData["Message"] = "Fund Request approved.";
            return RedirectToAction("Index");
        }

        private bool FundRequestExists(int id)
        {
            return _context.FundRequests.Any(e => e.Id == id);
        }
    }
}