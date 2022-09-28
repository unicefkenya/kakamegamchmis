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
using RestSharp.Extensions;
using DinkToPdf;
using Microsoft.AspNetCore.Authorization;

namespace MCHMIS.Areas.Export.Controllers
{
    [Area("Export")]
    [AllowAnonymous]
    public class ChangesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IUnitOfWork _uow;

        private readonly IDBService _dbService;
        private readonly IExportService _exportService;

        public ChangesController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment,
            IUnitOfWork uow, IDBService dbService, IExportService exportService)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _uow = uow;
            _dbService = dbService;
            _exportService = exportService;
        }

        public async Task<IActionResult> Index(ChangesReportListViewModel vm)
        {
            var reportTitle = (vm.ReportTypeId == 1 ? "Summary of" : "List of") + " Updates";
            if (vm.ChangeTypeId != null)
            {
                reportTitle = reportTitle + " - " + _context.SystemCodeDetails.Find(vm.ChangeTypeId).Code;
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
                    new SqlParameter("ChangeTypeId", vm.ChangeTypeId ?? (object)DBNull.Value),
                    new SqlParameter("HealthFacilityId", vm.HealthFacilityId ?? (object)DBNull.Value),
                    new SqlParameter("SubCountyId", vm.SubCountyId ?? (object)DBNull.Value),
                    new SqlParameter("WardId", vm.WardId ?? (object)DBNull.Value),
                    new SqlParameter("StatusId", vm.StatusId ?? (object)DBNull.Value),
                    new SqlParameter("@Option","Summary"),
                    new SqlParameter("@StartDate",vm.StartDate ?? (object)DBNull.Value),
                    new SqlParameter("@EndDate",vm.EndDate ?? (object)DBNull.Value),
                };
                var result =
                    SQLExtensions.GetModelFromQuery<ChangeSummaryViewModel>(_context,
                        "EXEC [ReportsChangesSummary] @ChangeTypeId,@HealthFacilityId,@SubCountyId,@WardId,@StatusId,@Option,@StartDate,@EndDate", parms);
                vm.Summaries = result;
                return View("Summary", vm);
            }
            if (vm.ReportTypeId == 2)
            {
                var changes = _context.Changes
               .OrderByDescending(c => c.DateCreated)
               .Include(c => c.ActionedBy)
               .Include(c => c.ChangeType)
               .Include(c => c.CreatedBy)
               .Include(c => c.Household.Mother)
               .Include(c => c.Household.Village.Ward.SubCounty)
               .Include(c => c.MPESACheckStatus)
               .Include(c => c.Status).AsQueryable();

                vm.AwaitingIPRS = _context.Changes.Count(i => i.RequiresIPRSCheck && i.IPRSVerified == false);
                vm.AwaitingMPesaVerification = _context.Changes.Count(i => i.RequiresMPESACheck && i.MPESACheckStatusId == null && i.StatusId == 3);

                if (vm.HealthFacilityId != null)
                {
                    changes = changes.Where(i => i.Household.HealthFacilityId == vm.HealthFacilityId);
                }
                if (vm.ChangeTypeId != null)
                {
                    changes = changes.Where(i => i.ChangeTypeId == vm.ChangeTypeId);
                }
                if (!string.IsNullOrEmpty(vm.UniqueId))
                {
                    changes = changes.Where(h => h.Household.UniqueId == vm.UniqueId);
                }
                if (!string.IsNullOrEmpty(vm.IdNumber))
                {
                    changes = changes.Where(h => h.Household.Mother.IdNumber.Contains(vm.IdNumber));
                }
                if (!string.IsNullOrEmpty(vm.Name))
                {
                    changes = changes.Where(h =>
                        h.Household.Mother.FirstName.Contains(vm.Name) ||
                        h.Household.Mother.MiddleName.Contains(vm.Name) ||
                        h.Household.Mother.Surname.Contains(vm.Name)

                    );
                }
                if (vm.StatusId != null)
                {
                    changes = changes.Where(h => h.StatusId == vm.StatusId);
                }
                if (vm.WardId != null)
                {
                    changes = changes.Where(h => h.Household.Village.WardId == vm.WardId);
                }
                if (vm.SubCountyId != null)
                {
                    changes = changes.Where(h => h.Household.Village.Ward.SubCountyId == vm.SubCountyId);
                }
                if (vm.HealthFacilityId != null)
                {
                    changes = changes.Where(h => h.Household.HealthFacilityId == vm.HealthFacilityId);
                }
                if (!string.IsNullOrEmpty(vm.Option) && vm.Option.Equals("mpesa"))
                {
                    changes = changes.Where(i => i.RequiresMPESACheck
                                                 && i.MPESACheckStatusId == null
                                                 // && i.StatusId == 3
                                                 );
                }
                if (!string.IsNullOrEmpty(vm.Option) && vm.Option.Equals("iprs"))
                {
                    changes = changes.Where(i => i.RequiresIPRSCheck && i.IPRSVerified == false);
                }
                if (vm.StartDate != null)
                {
                    changes = changes.Where(h => h.DateCreated >= DateTime.Parse(vm.StartDate));
                }
                if (vm.EndDate != null)
                {
                    changes = changes.Where(h => h.DateCreated <= DateTime.Parse(vm.EndDate));
                }
                var page = vm.Page ?? 1;
                var pageSize = vm.PageSize ?? changes.Count();
                vm.Total = changes.Count();
                vm.Changes = changes.ToPagedList(page, pageSize);
            }

            return View(vm);
        }

        public async Task<IActionResult> UpdatesTypes(ChangesReportListViewModel vm)
        {
            var reportTitle = (vm.ReportTypeId == 1 ? "Summary of" : "List of") + " Updates";
            if (vm.ChangeTypeId != null)
            {
                reportTitle = reportTitle + " - " + _context.SystemCodeDetails.Find(vm.ChangeTypeId).Code;
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
            if (vm.StartDate != null)
            {
                reportTitle = reportTitle + " - " + vm.StartDate;
            }
            if (vm.EndDate != null)
            {
                reportTitle = reportTitle + " - " + vm.EndDate;
            }
            ViewBag.Title = reportTitle;

            if (vm.ReportTypeId == 1)
            {
                SqlParameter[] parms = new SqlParameter[]
                {
                    new SqlParameter("ChangeTypeId", vm.ChangeTypeId ?? (object)DBNull.Value),
                    new SqlParameter("HealthFacilityId", vm.HealthFacilityId ?? (object)DBNull.Value),
                    new SqlParameter("SubCountyId", vm.SubCountyId ?? (object)DBNull.Value),
                    new SqlParameter("WardId", vm.WardId ?? (object)DBNull.Value),
                    new SqlParameter("StatusId", vm.StatusId ?? (object)DBNull.Value),
                    new SqlParameter("@Option","ChangeType"),
                    new SqlParameter("@StartDate",vm.StartDate ?? (object)DBNull.Value),
                    new SqlParameter("@EndDate",vm.EndDate ?? (object)DBNull.Value),
                };
                var result =
                    SQLExtensions.GetModelFromQuery<ChangeSummaryViewModel>(_context,
                        "EXEC [ReportsChangesSummary] @ChangeTypeId,@HealthFacilityId,@SubCountyId,@WardId,@StatusId,@Option,@StartDate,@EndDate", parms);
                vm.Summaries = result;
                return View("UpdatesTypes", vm);
            }

            return View(vm);
        }
    }
}