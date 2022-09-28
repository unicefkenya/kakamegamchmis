using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using MCHMIS.Data;
using MCHMIS.Interfaces;
using MCHMIS.Services;
using MCHMIS.Areas.Reports.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using Microsoft.AspNetCore.Authorization;

namespace MCHMIS.Areas.Export.Controllers
{
    [Area("Export")]
    [AllowAnonymous]
    public class ComplaintsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IDBService _dbService;

        public ComplaintsController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment,
            IExportService exportService,
            IUnitOfWork uow, IDBService dbService)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _uow = uow;
            _exportService = exportService;
            _dbService = dbService;
        }

        public async Task<IActionResult> Index(ComplaintsReportViewModel vm)
        {
            var reportTitle = (vm.ReportTypeId == 1 ? "Summary of" : "List of") + " Complaints";
            if (vm.ComplaintTypeId != null)
            {
                reportTitle = reportTitle + " - " + _context.ComplaintTypes.Find(vm.ComplaintTypeId).Name;
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

            if (vm.ReportTypeId == 1)
            {
                SqlParameter[] parms = new SqlParameter[]
                {
                    new SqlParameter("ComplaintTypeId", vm.ComplaintTypeId ?? (object)DBNull.Value),
                    new SqlParameter("HealthFacilityId", vm.HealthFacilityId ?? (object)DBNull.Value),
                    new SqlParameter("SubCountyId", vm.SubCountyId ?? (object)DBNull.Value),
                    new SqlParameter("WardId", vm.WardId ?? (object)DBNull.Value),
                    new SqlParameter("StatusId", vm.StatusId ?? (object)DBNull.Value),
                };
                var result =
                    SQLExtensions.GetModelFromQuery<ChangeSummaryViewModel>(_context,
                        "EXEC [ReportsComplaintsSummary] @ComplaintTypeId,@HealthFacilityId,@SubCountyId,@WardId,@StatusId", parms);
                vm.Summaries = result;
                return View("Summary", vm);
            }

            if (vm.ReportTypeId == 2)
            {
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
                    .Include(c => c.Village.Ward.SubCounty)
                    .OrderByDescending(i => i.DateCreated).AsQueryable();

                if (vm.HealthFacilityId != null)
                {
                    complaints = complaints.Where(i => i.HealthFacilityId == vm.HealthFacilityId);
                }
                if (vm.ComplaintTypeId != null)
                {
                    complaints = complaints.Where(i => i.ComplaintTypeId == vm.ComplaintTypeId);
                }
                if (vm.StatusId != null)
                {
                    complaints = complaints.Where(h => h.StatusId == vm.StatusId);
                }

                if (vm.WardId != null)
                {
                    complaints = complaints.Where(h => h.Village.WardId == vm.WardId);
                }

                if (vm.SubCountyId != null)
                {
                    complaints = complaints.Where(h => h.Village.Ward.SubCountyId == vm.SubCountyId);
                }

                var page = vm.Page ?? 1;
                var pageSize = vm.PageSize ?? 20;
                vm.Total = complaints.Count();
                vm.Complaints = complaints.ToPagedList(page, pageSize);
            }

            return View(vm);
        }
    }
}