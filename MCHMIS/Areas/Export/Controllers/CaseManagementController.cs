using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using MCHMIS.Areas.Reports.ViewModels;
using MCHMIS.Data;
using MCHMIS.Interfaces;
using MCHMIS.Models;
using MCHMIS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace MCHMIS.Areas.Export.Controllers
{
    [AllowAnonymous]
    [Area("Export")]
    public class CaseManagementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IDBService _dbService;

        public CaseManagementController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment,
            IUnitOfWork uow, IDBService dbService, IExportService exportService
            )
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _uow = uow;
            _dbService = dbService;
            _exportService = exportService;
        }

        public IActionResult Index(CaseManagementListViewModel vm)
        {
            var reportTitle = (vm.ReportTypeId == 1 ? "Summary of" : "List of") + " Case Management";

            if (vm.ClinicVisitId != null)
            {
                reportTitle = reportTitle + " - " + _context.ClinicVisits.Find(vm.ClinicVisitId).Name;
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

            ViewData["ReportTypeId"] = new SelectList(_context.ReportTypes, "Id", "Name", vm.ReportTypeId);
            ViewData["ClinicVisitId"] = new SelectList(_context.ClinicVisits, "Id", "Name", vm.ClinicVisitId);
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Name", vm.StatusId);
            ViewData["CaseStatusId"] = new SelectList(_context.CaseManagementStatus, "Id", "Name", vm.StatusId);
            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name", vm.SubCountyId);
            ViewData["ReasonId"] = new SelectList(_context.Reasons.Where(i => i.CategoryId == 2), "Id", "Name", vm.ReasonId);
            ViewData["HealthFacilityId"] =
                new SelectList(_context.HealthFacilities, "Id", "Name", vm.HealthFacilityId);

            if (vm.ReportTypeId != null)
            {
                if (vm.ReportTypeId == 1)
                {
                    SqlParameter[] parms = new SqlParameter[]
                    {
                        new SqlParameter("@StatusId", vm.StatusId ?? (object) DBNull.Value),
                        new SqlParameter("@HealthFacilityId", vm.HealthFacilityId ?? (object) DBNull.Value),
                        new SqlParameter("@SubCountyId", vm.SubCountyId ?? (object) DBNull.Value),
                        new SqlParameter("@WardId", vm.WardId ?? (object) DBNull.Value),
                        new SqlParameter("@ClinicVisitId", vm.ClinicVisitId ?? (object) DBNull.Value),
                        new SqlParameter("@Option", "Cases"),
                        new SqlParameter("@StartDate", vm.StartDate ?? (object) DBNull.Value),
                        new SqlParameter("@EndDate", vm.EndDate ?? (object) DBNull.Value),
                        new SqlParameter("@OptionId", vm.ReasonId ?? (object) DBNull.Value),
                        new SqlParameter("@CaseStatusId", vm.CaseStatusId ?? 0),
                    };
                    var result =
                        SQLExtensions.GetModelFromQuery<ExitedViewModel>(_context,
                            "EXEC [ReportCaseManagement] @StatusId,@HealthFacilityId,@SubCountyId,@WardId,@ClinicVisitId,@Option,@StartDate,@EndDate,@OptionId,@CaseStatusId",
                            parms);
                    vm.ExitedSummaries = result;
                    return View("CasesSummary", vm);
                }

                var cases = _context.Pregnancies
                  .OrderBy(c => c.CaseManagement.Household.Mother.FirstName)
                  .Include(c => c.CaseManagement.Household.Village.Ward.SubCounty)
                  .Include(c => c.CaseManagement.Household.Mother)
                  .Include(c => c.CaseManagement.Household.Status)
                  .Include(c => c.Status)
                  .Include(c => c.CaseManagement.Household.HealthFacility)
                  .AsQueryable();

                if (!string.IsNullOrEmpty(vm.UniqueId))
                {
                    cases = cases.Where(h => h.CaseManagement.Household.UniqueId == vm.UniqueId);
                }

                if (!string.IsNullOrEmpty(vm.IdNumber))
                {
                    cases = cases.Where(h => h.CaseManagement.Household.Mother.IdNumber.Contains(vm.IdNumber));
                }

                if (!string.IsNullOrEmpty(vm.Name))
                {
                    cases = cases.Where(h =>
                        h.CaseManagement.Household.Mother.FirstName.Contains(vm.Name)
                        || h.CaseManagement.Household.Mother.MiddleName.Contains(vm.Name)
                        || h.CaseManagement.Household.Mother.Surname.Contains(vm.Name)
                        || h.CaseManagement.Household.Mother.Surname.Contains(vm.Name)
                    );
                }

                if (vm.WardId != null)
                {
                    cases = cases.Where(h => h.CaseManagement.Household.Village.WardId == vm.WardId);
                }

                if (vm.SubCountyId != null)
                {
                    cases = cases.Where(h => h.CaseManagement.Household.Village.Ward.SubCountyId == vm.SubCountyId);
                }
                if (vm.HealthFacilityId != null)
                {
                    cases = cases.Where(h => h.CaseManagement.Household.HealthFacilityId == vm.HealthFacilityId);
                }
                if (vm.CaseStatusId != null)
                {
                    cases = cases.Where(i => i.StatusId == vm.CaseStatusId);
                }
                if (vm.StatusId != null)
                {
                    cases = cases.Where(i => i.CaseManagement.Household.StatusId == vm.StatusId);
                }

                //var test = _context.CaseManagement.ToList();
                //visits.ToList().Join(test, l => l.CaseManagementId, r => r.Id, (lft, rgt) => new ClinicVisit { });

                var page = vm.Page ?? 1;
                var pageSize = vm.PageSize ?? 20;
                vm.Total = cases.Count();
                vm.Cases = cases.ToPagedList(page, vm.Total);
            }

            return View(vm);
        }

        public IActionResult ClinicVisits(CaseManagementListViewModel vm)
        {
            var reportTitle = (vm.ReportTypeId == 1 ? "Summary of" : "List of") + " Clinic Visits";

            if (vm.ClinicVisitId != null)
            {
                reportTitle = reportTitle + " - " + _context.ClinicVisits.Find(vm.ClinicVisitId).Name;
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
                        new SqlParameter("@StatusId", vm.StatusId ?? (object) DBNull.Value),
                        new SqlParameter("@HealthFacilityId", vm.HealthFacilityId ?? (object) DBNull.Value),
                        new SqlParameter("@SubCountyId", vm.SubCountyId ?? (object) DBNull.Value),
                        new SqlParameter("@WardId", vm.WardId ?? (object) DBNull.Value),
                        new SqlParameter("@ClinicVisitId", vm.ClinicVisitId ?? (object) DBNull.Value),
                        new SqlParameter("@Option", "Statistics"),
                        new SqlParameter("@StartDate", vm.StartDate ?? (object) DBNull.Value),
                        new SqlParameter("@EndDate", vm.EndDate ?? (object) DBNull.Value),
                    };
                    var result =
                        SQLExtensions.GetModelFromQuery<FacilitySummaryViewModel>(_context,
                            "EXEC [ReportCaseManagement] @StatusId,@HealthFacilityId,@SubCountyId,@WardId,@ClinicVisitId,@Option,@StartDate,@EndDate",
                            parms);
                    vm.Summaries = result;
                    return View("VisitsSummary", vm);
                }

                var visits = _context.MotherClinicVisits
                    .OrderBy(c => c.Pregnancy.CaseManagement.Household.Mother.FirstName)
                    .Include(c => c.Pregnancy.CaseManagement.Household.Village.Ward.SubCounty)
                    .Include(c => c.Pregnancy.CaseManagement.Household.Mother)
                    .Include(c => c.ClinicVisit)
                    .Include(c => c.Pregnancy.CaseManagement.Household.HealthFacility).AsQueryable();

                if (!string.IsNullOrEmpty(vm.UniqueId))
                {
                    visits = visits.Where(h => h.Pregnancy.CaseManagement.Household.UniqueId == vm.UniqueId);
                }

                if (!string.IsNullOrEmpty(vm.IdNumber))
                {
                    visits = visits.Where(h => h.Pregnancy.CaseManagement.Household.Mother.IdNumber.Contains(vm.IdNumber));
                }

                if (vm.ClinicVisitId != null)
                {
                    visits = visits.Where(h => h.ClinicVisitId == vm.ClinicVisitId);
                }

                if (!string.IsNullOrEmpty(vm.Name))
                {
                    visits = visits.Where(h =>
                        h.Pregnancy.CaseManagement.Household.Mother.FirstName.Contains(vm.Name)
                        || h.Pregnancy.CaseManagement.Household.Mother.MiddleName.Contains(vm.Name)
                        || h.Pregnancy.CaseManagement.Household.Mother.Surname.Contains(vm.Name)
                        || h.Pregnancy.CaseManagement.Household.Mother.Surname.Contains(vm.Name)
                    );
                }

                if (vm.WardId != null)
                {
                    visits = visits.Where(h => h.Pregnancy.CaseManagement.Household.Village.WardId == vm.WardId);
                }

                if (vm.SubCountyId != null)
                {
                    visits = visits.Where(h => h.Pregnancy.CaseManagement.Household.Village.Ward.SubCountyId == vm.SubCountyId);
                }
                if (vm.HealthFacilityId != null)
                {
                    visits = visits.Where(h => h.Pregnancy.CaseManagement.Household.HealthFacilityId == vm.HealthFacilityId);
                }
                if (vm.StartDate != null)
                {
                    visits = visits.Where(h => h.VisitDate >= DateTime.Parse(vm.StartDate));
                }
                if (vm.EndDate != null)
                {
                    visits = visits.Where(h => h.VisitDate <= DateTime.Parse(vm.EndDate));
                }
                //var test = _context.CaseManagement.ToList();
                //visits.ToList().Join(test, l => l.CaseManagementId, r => r.Id, (lft, rgt) => new ClinicVisit { });

                var page = vm.Page ?? 1;
                vm.Total = visits.Count();
                vm.MotherClinicVisits = visits.ToPagedList(page, vm.Total == 0 ? 1 : vm.Total);
            }

            return View(vm);
        }

        public IActionResult Delivery(CaseManagementListViewModel vm)
        {
            var reportTitle = (vm.ReportTypeId == 1 ? "Summary of" : "List of") + " Delivery Statistics";

            if (vm.PregnancyOutcomeId != null)
            {
                reportTitle = reportTitle + " - " + _context.SystemCodeDetails.Find(vm.PregnancyOutcomeId).Code;
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
                        new SqlParameter("@StatusId", vm.StatusId ?? (object) DBNull.Value),
                        new SqlParameter("@HealthFacilityId", vm.HealthFacilityId ?? (object) DBNull.Value),
                        new SqlParameter("@SubCountyId", vm.SubCountyId ?? (object) DBNull.Value),
                        new SqlParameter("@WardId", vm.WardId ?? (object) DBNull.Value),
                        new SqlParameter("@ClinicVisitId", vm.ClinicVisitId ?? (object) DBNull.Value),
                        new SqlParameter("@Option", "Delivery"),
                        new SqlParameter("@StartDate", vm.StartDate ?? (object) DBNull.Value),
                        new SqlParameter("@EndDate", vm.EndDate ?? (object) DBNull.Value),
                        new SqlParameter("@OptionId", vm.PregnancyOutcomeId ?? (object) DBNull.Value)
                    };
                    var result =
                        SQLExtensions.GetModelFromQuery<FacilitySummaryViewModel>(_context,
                            "EXEC [ReportCaseManagement] @StatusId,@HealthFacilityId,@SubCountyId,@WardId,@ClinicVisitId,@Option,@StartDate,@EndDate,@OptionId",
                            parms);
                    vm.Summaries = result;
                    return View("DeliverySummary", vm);
                }

                var deliveries = _context.Deliveries
                    .OrderBy(c => c.Pregnancy.CaseManagement.Household.Mother.FirstName)
                    .Include(c => c.DeliveryMode)
                    .Include(c => c.PregnancyOutcome)
                    .Include(c => c.Pregnancy.CaseManagement.Household.Village.Ward.SubCounty)
                    .Include(c => c.Pregnancy.CaseManagement.Household.Mother)
                    .Include(c => c.Pregnancy.CaseManagement.Household.HealthFacility).AsQueryable();

                if (!string.IsNullOrEmpty(vm.UniqueId))
                {
                    deliveries = deliveries.Where(h => h.Pregnancy.CaseManagement.Household.UniqueId == vm.UniqueId);
                }

                if (!string.IsNullOrEmpty(vm.IdNumber))
                {
                    deliveries = deliveries.Where(h => h.Pregnancy.CaseManagement.Household.Mother.IdNumber.Contains(vm.IdNumber));
                }

                if (!string.IsNullOrEmpty(vm.Name))
                {
                    deliveries = deliveries.Where(h =>
                        h.Pregnancy.CaseManagement.Household.Mother.FirstName.Contains(vm.Name)
                        || h.Pregnancy.CaseManagement.Household.Mother.MiddleName.Contains(vm.Name)
                        || h.Pregnancy.CaseManagement.Household.Mother.Surname.Contains(vm.Name)
                        || h.Pregnancy.CaseManagement.Household.Mother.Surname.Contains(vm.Name)
                    );
                }

                if (vm.WardId != null)
                {
                    deliveries = deliveries.Where(h => h.Pregnancy.CaseManagement.Household.Village.WardId == vm.WardId);
                }

                if (vm.SubCountyId != null)
                {
                    deliveries = deliveries.Where(h => h.Pregnancy.CaseManagement.Household.Village.Ward.SubCountyId == vm.SubCountyId);
                }
                if (vm.HealthFacilityId != null)
                {
                    deliveries = deliveries.Where(h => h.Pregnancy.CaseManagement.Household.HealthFacilityId == vm.HealthFacilityId);
                }
                if (vm.PregnancyOutcomeId != null)
                {
                    deliveries = deliveries.Where(h => h.PregnancyOutcomeId == vm.PregnancyOutcomeId);
                }
                if (vm.StartDate != null)
                {
                    deliveries = deliveries.Where(h => h.DeliveryDate >= DateTime.Parse(vm.StartDate));
                }
                if (vm.EndDate != null)
                {
                    deliveries = deliveries.Where(h => h.DeliveryDate <= DateTime.Parse(vm.EndDate));
                }
                //var test = _context.CaseManagement.ToList();
                //visits.ToList().Join(test, l => l.CaseManagementId, r => r.Id, (lft, rgt) => new ClinicVisit { });

                var page = vm.Page ?? 1;
                var pageSize = vm.PageSize ?? 20;
                vm.Total = deliveries.Count();
                vm.Deliveries = deliveries.ToPagedList(page, vm.Total == 0 ? 1 : vm.Total);
            }

            return View(vm);
        }

        public IActionResult HomeDelivery(CaseManagementListViewModel vm)
        {
            var reportTitle = (vm.ReportTypeId == 1 ? "Summary of" : "List of") + " Home Deliveries";

            if (vm.PregnancyOutcomeId != null)
            {
                reportTitle = reportTitle + " - " + _context.SystemCodeDetails.Find(vm.PregnancyOutcomeId).Code;
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
                    SqlParameter[] parms = new[]
                    {
                        new SqlParameter("@StatusId", vm.StatusId ?? (object) DBNull.Value),
                        new SqlParameter("@HealthFacilityId", vm.HealthFacilityId ?? (object) DBNull.Value),
                        new SqlParameter("@SubCountyId", vm.SubCountyId ?? (object) DBNull.Value),
                        new SqlParameter("@WardId", vm.WardId ?? (object) DBNull.Value),
                        new SqlParameter("@ClinicVisitId", vm.ClinicVisitId ?? (object) DBNull.Value),
                        new SqlParameter("@Option", "HomeDelivery"),
                        new SqlParameter("@StartDate", vm.StartDate ?? (object) DBNull.Value),
                        new SqlParameter("@EndDate", vm.EndDate ?? (object) DBNull.Value),
                        new SqlParameter("@OptionId", vm.PregnancyOutcomeId ?? (object) DBNull.Value)
                    };
                    var result =
                        SQLExtensions.GetModelFromQuery<HHSummaryViewModel>(_context,
                            "EXEC [ReportCaseManagement] @StatusId,@HealthFacilityId,@SubCountyId,@WardId,@ClinicVisitId,@Option,@StartDate,@EndDate,@OptionId",
                            parms);
                    vm.WardSummaries = result;
                    return View("HomeDeliverySummary", vm);
                }

                var deliveries = _context.Children
                    .OrderBy(c => c.Delivery.Pregnancy.CaseManagement.Household.Mother.FirstName)
                    .Include(c => c.Delivery.DeliveryMode)
                    .Include(c => c.Delivery.PregnancyOutcome)
                    .Include(c => c.Delivery.Pregnancy.CaseManagement.Household.Village.Ward.SubCounty)
                    .Include(c => c.Delivery.Pregnancy.CaseManagement.Household.Mother)
                    .Include(c => c.Delivery.Pregnancy.CaseManagement.Household.HealthFacility)
                    .Where(i => i.DeliveryPlaceId == 259).AsQueryable();

                if (!string.IsNullOrEmpty(vm.UniqueId))
                {
                    deliveries = deliveries.Where(h => h.Delivery.Pregnancy.CaseManagement.Household.UniqueId == vm.UniqueId);
                }

                if (!string.IsNullOrEmpty(vm.IdNumber))
                {
                    deliveries = deliveries.Where(h => h.Delivery.Pregnancy.CaseManagement.Household.Mother.IdNumber.Contains(vm.IdNumber));
                }

                if (!string.IsNullOrEmpty(vm.Name))
                {
                    deliveries = deliveries.Where(h =>
                        h.Delivery.Pregnancy.CaseManagement.Household.Mother.FirstName.Contains(vm.Name)
                        || h.Delivery.Pregnancy.CaseManagement.Household.Mother.MiddleName.Contains(vm.Name)
                        || h.Delivery.Pregnancy.CaseManagement.Household.Mother.Surname.Contains(vm.Name)
                        || h.Delivery.Pregnancy.CaseManagement.Household.Mother.Surname.Contains(vm.Name)
                    );
                }

                if (vm.WardId != null)
                {
                    deliveries = deliveries.Where(h => h.Delivery.Pregnancy.CaseManagement.Household.Village.WardId == vm.WardId);
                }

                if (vm.SubCountyId != null)
                {
                    deliveries = deliveries.Where(h => h.Delivery.Pregnancy.CaseManagement.Household.Village.Ward.SubCountyId == vm.SubCountyId);
                }
                if (vm.HealthFacilityId != null)
                {
                    deliveries = deliveries.Where(h => h.Delivery.Pregnancy.CaseManagement.Household.HealthFacilityId == vm.HealthFacilityId);
                }
                if (vm.PregnancyOutcomeId != null)
                {
                    deliveries = deliveries.Where(h => h.Delivery.PregnancyOutcomeId == vm.PregnancyOutcomeId);
                }
                if (vm.StartDate != null)
                {
                    deliveries = deliveries.Where(h => h.Delivery.DeliveryDate >= DateTime.Parse(vm.StartDate));
                }
                if (vm.EndDate != null)
                {
                    deliveries = deliveries.Where(h => h.Delivery.DeliveryDate <= DateTime.Parse(vm.EndDate));
                }
                //var test = _context.CaseManagement.ToList();
                //visits.ToList().Join(test, l => l.CaseManagementId, r => r.Id, (lft, rgt) => new ClinicVisit { });

                var page = vm.Page ?? 1;
                var pageSize = vm.PageSize ?? 20;
                vm.Total = deliveries.Count();
                vm.Children = deliveries.ToPagedList(page, vm.Total == 0 ? 1 : vm.Total);
            }

            return View(vm);
        }

        public IActionResult DeliveryOtherFacilities(CaseManagementListViewModel vm)
        {
            var reportTitle = (vm.ReportTypeId == 1 ? "Summary of" : "List of") + " deliveries in other facilities";

            if (vm.PregnancyOutcomeId != null)
            {
                reportTitle = reportTitle + " - " + _context.SystemCodeDetails.Find(vm.PregnancyOutcomeId).Code;
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
                    SqlParameter[] parms = new[]
                    {
                        new SqlParameter("@StatusId", vm.StatusId ?? (object) DBNull.Value),
                        new SqlParameter("@HealthFacilityId", vm.HealthFacilityId ?? (object) DBNull.Value),
                        new SqlParameter("@SubCountyId", vm.SubCountyId ?? (object) DBNull.Value),
                        new SqlParameter("@WardId", vm.WardId ?? (object) DBNull.Value),
                        new SqlParameter("@ClinicVisitId", vm.ClinicVisitId ?? (object) DBNull.Value),
                        new SqlParameter("@Option", "DeliveryOtherFacilities"),
                        new SqlParameter("@StartDate", vm.StartDate ?? (object) DBNull.Value),
                        new SqlParameter("@EndDate", vm.EndDate ?? (object) DBNull.Value),
                        new SqlParameter("@OptionId", vm.PregnancyOutcomeId ?? (object) DBNull.Value)
                    };
                    var result =
                        SQLExtensions.GetModelFromQuery<FacilitySummaryViewModel>(_context,
                            "EXEC [ReportCaseManagement] @StatusId,@HealthFacilityId,@SubCountyId,@WardId,@ClinicVisitId,@Option,@StartDate,@EndDate,@OptionId",
                            parms);
                    vm.Summaries = result;
                    return View("DeliveryOtherFacilitiesSummary", vm);
                }

                var children = _context.Children
                    .OrderBy(c => c.Delivery.Pregnancy.CaseManagement.Household.Mother.FirstName)
                    .Include(c => c.Delivery.DeliveryMode)
                    .Include(c => c.DeliveryHealthFacility)
                    .Include(c => c.Delivery.PregnancyOutcome)
                    .Include(c => c.Delivery.Pregnancy.CaseManagement.Household.Village.Ward.SubCounty)
                    .Include(c => c.Delivery.Pregnancy.CaseManagement.Household.Mother)
                    .Include(c => c.Delivery.Pregnancy.CaseManagement.Household.HealthFacility)
                    .Where(i => i.DeliveryPlaceId == 258 && i.DeliveryHealthFacilityId != i.Delivery.Pregnancy.CaseManagement.Household.HealthFacilityId).AsQueryable();

                if (!string.IsNullOrEmpty(vm.UniqueId))
                {
                    children = children.Where(h => h.Delivery.Pregnancy.CaseManagement.Household.UniqueId == vm.UniqueId);
                }

                if (!string.IsNullOrEmpty(vm.IdNumber))
                {
                    children = children.Where(h => h.Delivery.Pregnancy.CaseManagement.Household.Mother.IdNumber.Contains(vm.IdNumber));
                }

                if (!string.IsNullOrEmpty(vm.Name))
                {
                    children = children.Where(h =>
                        h.Delivery.Pregnancy.CaseManagement.Household.Mother.FirstName.Contains(vm.Name)
                        || h.Delivery.Pregnancy.CaseManagement.Household.Mother.MiddleName.Contains(vm.Name)
                        || h.Delivery.Pregnancy.CaseManagement.Household.Mother.Surname.Contains(vm.Name)
                        || h.Delivery.Pregnancy.CaseManagement.Household.Mother.Surname.Contains(vm.Name)
                    );
                }

                if (vm.WardId != null)
                {
                    children = children.Where(h => h.Delivery.Pregnancy.CaseManagement.Household.Village.WardId == vm.WardId);
                }

                if (vm.SubCountyId != null)
                {
                    children = children.Where(h => h.Delivery.Pregnancy.CaseManagement.Household.Village.Ward.SubCountyId == vm.SubCountyId);
                }
                if (vm.HealthFacilityId != null)
                {
                    children = children.Where(h => h.Delivery.Pregnancy.CaseManagement.Household.HealthFacilityId == vm.HealthFacilityId);
                }
                if (vm.DeliveryHealthFacilityId != null)
                {
                    children = children.Where(h => h.DeliveryHealthFacilityId == vm.DeliveryHealthFacilityId);
                }
                if (vm.PregnancyOutcomeId != null)
                {
                    children = children.Where(h => h.Delivery.PregnancyOutcomeId == vm.PregnancyOutcomeId);
                }
                if (vm.StartDate != null)
                {
                    children = children.Where(h => h.Delivery.DeliveryDate >= DateTime.Parse(vm.StartDate));
                }
                if (vm.EndDate != null)
                {
                    children = children.Where(h => h.Delivery.DeliveryDate <= DateTime.Parse(vm.EndDate));
                }
                //var test = _context.CaseManagement.ToList();
                //visits.ToList().Join(test, l => l.CaseManagementId, r => r.Id, (lft, rgt) => new ClinicVisit { });

                var page = vm.Page ?? 1;
                var pageSize = vm.PageSize ?? 20;
                vm.Total = children.Count();
                vm.Children = children.ToPagedList(page, pageSize);
            }

            return View(vm);
        }

        public IActionResult NoBirthNotification(CaseManagementListViewModel vm)
        {
            var reportTitle = (vm.ReportTypeId == 1 ? "Summary of" : "List of") + " children without birth notifications";

            if (vm.PregnancyOutcomeId != null)
            {
                reportTitle = reportTitle + " - " + _context.SystemCodeDetails.Find(vm.PregnancyOutcomeId).Code;
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
                    SqlParameter[] parms = new[]
                    {
                        new SqlParameter("@StatusId", vm.StatusId ?? (object) DBNull.Value),
                        new SqlParameter("@HealthFacilityId", vm.HealthFacilityId ?? (object) DBNull.Value),
                        new SqlParameter("@SubCountyId", vm.SubCountyId ?? (object) DBNull.Value),
                        new SqlParameter("@WardId", vm.WardId ?? (object) DBNull.Value),
                        new SqlParameter("@ClinicVisitId", vm.ClinicVisitId ?? (object) DBNull.Value),
                        new SqlParameter("@Option", "NoBirthNotification"),
                        new SqlParameter("@StartDate", vm.StartDate ?? (object) DBNull.Value),
                        new SqlParameter("@EndDate", vm.EndDate ?? (object) DBNull.Value),
                        new SqlParameter("@OptionId", vm.PregnancyOutcomeId ?? (object) DBNull.Value)
                    };
                    var result =
                        SQLExtensions.GetModelFromQuery<FacilitySummaryViewModel>(_context,
                            "EXEC [ReportCaseManagement] @StatusId,@HealthFacilityId,@SubCountyId,@WardId,@ClinicVisitId,@Option,@StartDate,@EndDate,@OptionId",
                            parms);
                    vm.Summaries = result;
                    return View("NoBirthNotificationSummary", vm);
                }

                var deliveries = _context.Children
                    .OrderBy(c => c.Delivery.Pregnancy.CaseManagement.Household.Mother.FirstName)
                    .Include(c => c.Delivery.DeliveryMode)
                    .Include(c => c.Delivery.PregnancyOutcome)
                    .Include(c => c.Delivery.Pregnancy.CaseManagement.Household.Village.Ward.SubCounty)
                    .Include(c => c.Delivery.Pregnancy.CaseManagement.Household.Mother)
                    .Include(c => c.Delivery.Pregnancy.CaseManagement.Household.HealthFacility)
                    .Where(i => (i.ChildHealthRecord.BirthNotificationNumber == null || i.ChildHealthRecord.BirthNotificationNumber == "")).AsQueryable();

                if (!string.IsNullOrEmpty(vm.UniqueId))
                {
                    deliveries = deliveries.Where(h => h.Delivery.Pregnancy.CaseManagement.Household.UniqueId == vm.UniqueId);
                }

                if (!string.IsNullOrEmpty(vm.IdNumber))
                {
                    deliveries = deliveries.Where(h => h.Delivery.Pregnancy.CaseManagement.Household.Mother.IdNumber.Contains(vm.IdNumber));
                }

                if (!string.IsNullOrEmpty(vm.Name))
                {
                    deliveries = deliveries.Where(h =>
                        h.Delivery.Pregnancy.CaseManagement.Household.Mother.FirstName.Contains(vm.Name)
                        || h.Delivery.Pregnancy.CaseManagement.Household.Mother.MiddleName.Contains(vm.Name)
                        || h.Delivery.Pregnancy.CaseManagement.Household.Mother.Surname.Contains(vm.Name)
                        || h.Delivery.Pregnancy.CaseManagement.Household.Mother.Surname.Contains(vm.Name)
                    );
                }

                if (vm.WardId != null)
                {
                    deliveries = deliveries.Where(h => h.Delivery.Pregnancy.CaseManagement.Household.Village.WardId == vm.WardId);
                }

                if (vm.SubCountyId != null)
                {
                    deliveries = deliveries.Where(h => h.Delivery.Pregnancy.CaseManagement.Household.Village.Ward.SubCountyId == vm.SubCountyId);
                }
                if (vm.HealthFacilityId != null)
                {
                    deliveries = deliveries.Where(h => h.Delivery.Pregnancy.CaseManagement.Household.HealthFacilityId == vm.HealthFacilityId);
                }
                if (vm.PregnancyOutcomeId != null)
                {
                    deliveries = deliveries.Where(h => h.Delivery.PregnancyOutcomeId == vm.PregnancyOutcomeId);
                }
                if (vm.StartDate != null)
                {
                    deliveries = deliveries.Where(h => h.Delivery.DeliveryDate >= DateTime.Parse(vm.StartDate));
                }
                if (vm.EndDate != null)
                {
                    deliveries = deliveries.Where(h => h.Delivery.DeliveryDate <= DateTime.Parse(vm.EndDate));
                }
                //var test = _context.CaseManagement.ToList();
                //visits.ToList().Join(test, l => l.CaseManagementId, r => r.Id, (lft, rgt) => new ClinicVisit { });

                var page = vm.Page ?? 1;
                var pageSize = vm.PageSize ?? 20;
                vm.Total = deliveries.Count();
                vm.Children = deliveries.ToPagedList(page, vm.Total == 0 ? 1 : vm.Total);
            }

            return View(vm);
        }

        public IActionResult FamilyPlanning(CaseManagementListViewModel vm)
        {
            var reportTitle = (vm.ReportTypeId == 1 ? "Summary of" : "List of") + " Family Planning";

            if (vm.FPMethodId != null)
            {
                reportTitle = reportTitle + " - " + _context.SystemCodeDetails.Find(vm.FPMethodId).Code;
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
                    SqlParameter[] parms = new[]
                    {
                        new SqlParameter("@StatusId", vm.StatusId ?? (object) DBNull.Value),
                        new SqlParameter("@HealthFacilityId", vm.HealthFacilityId ?? (object) DBNull.Value),
                        new SqlParameter("@SubCountyId", vm.SubCountyId ?? (object) DBNull.Value),
                        new SqlParameter("@WardId", vm.WardId ?? (object) DBNull.Value),
                        new SqlParameter("@ClinicVisitId", vm.ClinicVisitId ?? (object) DBNull.Value),
                        new SqlParameter("@Option", "FamilyPlanning"),
                        new SqlParameter("@StartDate", vm.StartDate ?? (object) DBNull.Value),
                        new SqlParameter("@EndDate", vm.EndDate ?? (object) DBNull.Value),
                        new SqlParameter("@OptionId", vm.FPMethodId ?? (object) DBNull.Value)
                    };
                    var result =
                        SQLExtensions.GetModelFromQuery<FacilitySummaryViewModel>(_context,
                            "EXEC [ReportCaseManagement] @StatusId,@HealthFacilityId,@SubCountyId,@WardId,@ClinicVisitId,@Option,@StartDate,@EndDate,@OptionId",
                            parms);
                    vm.Summaries = result;
                    return View("FamilyPlanningSummary", vm);
                }

                var details = _context.PostNatalExaminations
                    .OrderBy(c => c.Pregnancy.CaseManagement.Household.Mother.FirstName)
                    .Include(c => c.Pregnancy.CaseManagement.Household.Mother)
                    .Include(c => c.Pregnancy.CaseManagement.Household.Village.Ward.SubCounty)
                    .Include(c => c.ClinicVisit)
                    .Include(c => c.FPCounseling)
                    .Include(c => c.FPMethod)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(vm.UniqueId))
                {
                    details = details.Where(h => h.Pregnancy.CaseManagement.Household.UniqueId == vm.UniqueId);
                }

                if (!string.IsNullOrEmpty(vm.IdNumber))
                {
                    details = details.Where(h => h.Pregnancy.CaseManagement.Household.Mother.IdNumber.Contains(vm.IdNumber));
                }

                if (!string.IsNullOrEmpty(vm.Name))
                {
                    details = details.Where(h =>
                        h.Pregnancy.CaseManagement.Household.Mother.FirstName.Contains(vm.Name)
                        || h.Pregnancy.CaseManagement.Household.Mother.MiddleName.Contains(vm.Name)
                        || h.Pregnancy.CaseManagement.Household.Mother.Surname.Contains(vm.Name)
                        || h.Pregnancy.CaseManagement.Household.Mother.Surname.Contains(vm.Name)
                    );
                }

                if (vm.FPMethodId != null)
                {
                    details = details.Where(h => h.FPMethodId == vm.FPMethodId);
                }

                if (vm.WardId != null)
                {
                    details = details.Where(h => h.Pregnancy.CaseManagement.Household.Village.WardId == vm.WardId);
                }

                if (vm.SubCountyId != null)
                {
                    details = details.Where(h => h.Pregnancy.CaseManagement.Household.Village.Ward.SubCountyId == vm.SubCountyId);
                }
                if (vm.HealthFacilityId != null)
                {
                    details = details.Where(h => h.Pregnancy.CaseManagement.Household.HealthFacilityId == vm.HealthFacilityId);
                }

                if (vm.StartDate != null)
                {
                    details = details.Where(h => h.VisitDate >= DateTime.Parse(vm.StartDate));
                }
                if (vm.EndDate != null)
                {
                    details = details.Where(h => h.VisitDate <= DateTime.Parse(vm.EndDate));
                }
                //var test = _context.CaseManagement.ToList();
                //visits.ToList().Join(test, l => l.CaseManagementId, r => r.Id, (lft, rgt) => new ClinicVisit { });

                var page = vm.Page ?? 1;
                var pageSize = vm.PageSize ?? 20;
                vm.Total = details.Count();
                vm.PostNatalExaminations = details.ToPagedList(page, vm.Total == 0 ? 1 : vm.Total);
            }

            return View(vm);
        }

        public IActionResult Repeat(CaseManagementListViewModel vm)
        {
            var reportTitle = (vm.ReportTypeId == 1 ? "Summary of" : "List of") + " Mothers coming for the 2nd round of registration";

            if (vm.ClinicVisitId != null)
            {
                reportTitle = reportTitle + " - " + _context.ClinicVisits.Find(vm.ClinicVisitId).Name;
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
                        new SqlParameter("@StatusId", vm.StatusId ?? (object) DBNull.Value),
                        new SqlParameter("@HealthFacilityId", vm.HealthFacilityId ?? (object) DBNull.Value),
                        new SqlParameter("@SubCountyId", vm.SubCountyId ?? (object) DBNull.Value),
                        new SqlParameter("@WardId", vm.WardId ?? (object) DBNull.Value),
                        new SqlParameter("@ClinicVisitId", vm.ClinicVisitId ?? (object) DBNull.Value),
                        new SqlParameter("@Option", "RepeatSummary"),
                        new SqlParameter("@StartDate", vm.StartDate ?? (object) DBNull.Value),
                        new SqlParameter("@EndDate", vm.EndDate ?? (object) DBNull.Value),
                    };
                    var result =
                        SQLExtensions.GetModelFromQuery<FacilitySummaryViewModel>(_context,
                            "EXEC [ReportCaseManagement] @StatusId,@HealthFacilityId,@SubCountyId,@WardId,@ClinicVisitId,@Option,@StartDate,@EndDate",
                            parms);
                    vm.Summaries = result;
                    return View("RepeatSummary", vm);
                }

                SqlParameter[] parmsDetailed = new SqlParameter[]
                {
                    new SqlParameter("@StatusId", vm.StatusId ?? (object) DBNull.Value),
                    new SqlParameter("@HealthFacilityId", vm.HealthFacilityId ?? (object) DBNull.Value),
                    new SqlParameter("@SubCountyId", vm.SubCountyId ?? (object) DBNull.Value),
                    new SqlParameter("@WardId", vm.WardId ?? (object) DBNull.Value),
                    new SqlParameter("@ClinicVisitId", vm.ClinicVisitId ?? (object) DBNull.Value),
                    new SqlParameter("@Option", "RepeatDetailed"),
                    new SqlParameter("@StartDate", vm.StartDate ?? (object) DBNull.Value),
                    new SqlParameter("@EndDate", vm.EndDate ?? (object) DBNull.Value),
                };
                var details =
                    SQLExtensions.GetModelFromQuery<RepeatCasesViewModel>(_context,
                        "EXEC [ReportCaseManagement] @StatusId,@HealthFacilityId,@SubCountyId,@WardId,@ClinicVisitId,@Option,@StartDate,@EndDate",
                        parmsDetailed);
                var page = vm.Page ?? 1;
                var pageSize = vm.PageSize ?? 20;
                vm.Total = details.Count();
                vm.RepeatCases = details.AsQueryable().ToPagedList(page, vm.Total == 0 ? 1 : vm.Total);
            }

            return View(vm);
        }

        public IActionResult Exited(CaseManagementListViewModel vm)
        {
            var reportTitle = (vm.ReportTypeId == 1 ? "Summary of" : "List of") + " Mothers who have exited the program";

            if (vm.ClinicVisitId != null)
            {
                reportTitle = reportTitle + " - " + _context.ClinicVisits.Find(vm.ClinicVisitId).Name;
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
                        new SqlParameter("@StatusId", vm.StatusId ?? (object) DBNull.Value),
                        new SqlParameter("@HealthFacilityId", vm.HealthFacilityId ?? (object) DBNull.Value),
                        new SqlParameter("@SubCountyId", vm.SubCountyId ?? (object) DBNull.Value),
                        new SqlParameter("@WardId", vm.WardId ?? (object) DBNull.Value),
                        new SqlParameter("@ClinicVisitId", vm.ClinicVisitId ?? (object) DBNull.Value),
                        new SqlParameter("@Option", "Exited"),
                        new SqlParameter("@StartDate", vm.StartDate ?? (object) DBNull.Value),
                        new SqlParameter("@EndDate", vm.EndDate ?? (object) DBNull.Value),
                        new SqlParameter("@OptionId", vm.ReasonId ?? (object) DBNull.Value),
                    };
                    var result =
                        SQLExtensions.GetModelFromQuery<ExitedViewModel>(_context,
                            "EXEC [ReportCaseManagement] @StatusId,@HealthFacilityId,@SubCountyId,@WardId,@ClinicVisitId,@Option,@StartDate,@EndDate,@OptionId",
                            parms);
                    vm.ExitedSummaries = result;
                    return View("ExitedSummary", vm);
                }

                var cases = _context.Pregnancies
                    .OrderBy(c => c.CaseManagement.Household.Mother.FirstName)
                    .Include(c => c.CaseManagement.Household.Village.Ward.SubCounty)
                    .Include(c => c.CaseManagement.Household.Mother)
                    .Include(c => c.Reason)
                    .Include(c => c.CaseManagement.Household.HealthFacility)
                    .Where(i => i.StatusId == 2)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(vm.UniqueId))
                {
                    cases = cases.Where(h => h.CaseManagement.Household.UniqueId == vm.UniqueId);
                }

                if (!string.IsNullOrEmpty(vm.IdNumber))
                {
                    cases = cases.Where(h => h.CaseManagement.Household.Mother.IdNumber.Contains(vm.IdNumber));
                }

                if (!string.IsNullOrEmpty(vm.Name))
                {
                    cases = cases.Where(h =>
                        h.CaseManagement.Household.Mother.FirstName.Contains(vm.Name)
                        || h.CaseManagement.Household.Mother.MiddleName.Contains(vm.Name)
                        || h.CaseManagement.Household.Mother.Surname.Contains(vm.Name)
                        || h.CaseManagement.Household.Mother.Surname.Contains(vm.Name)
                    );
                }

                if (vm.WardId != null)
                {
                    cases = cases.Where(h => h.CaseManagement.Household.Village.WardId == vm.WardId);
                }

                if (vm.SubCountyId != null)
                {
                    cases = cases.Where(h => h.CaseManagement.Household.Village.Ward.SubCountyId == vm.SubCountyId);
                }
                if (vm.HealthFacilityId != null)
                {
                    cases = cases.Where(h => h.CaseManagement.Household.HealthFacilityId == vm.HealthFacilityId);
                }
                if (vm.ReasonId != null)
                {
                    cases = cases.Where(h => h.ReasonId == vm.ReasonId);
                }
                if (vm.StartDate != null)
                {
                    cases = cases.Where(h => h.DateExited >= DateTime.Parse(vm.StartDate));
                }
                if (vm.EndDate != null)
                {
                    cases = cases.Where(h => h.DateExited <= DateTime.Parse(vm.EndDate));
                }
                //var test = _context.CaseManagement.ToList();
                //visits.ToList().Join(test, l => l.CaseManagementId, r => r.Id, (lft, rgt) => new ClinicVisit { });

                var page = vm.Page ?? 1;
                var pageSize = vm.PageSize ?? 20;
                vm.Total = cases.Count();
                vm.Cases = cases.ToPagedList(page, vm.Total == 0 ? 1 : vm.Total);
            }

            return View(vm);
        }

        public IActionResult MaternalDeath(CaseManagementListViewModel vm)
        {
            var reportTitle = (vm.ReportTypeId == 1 ? "Summary of" : "List of") + " Maternal Deaths";

            if (vm.ClinicVisitId != null)
            {
                reportTitle = reportTitle + " - " + _context.ClinicVisits.Find(vm.ClinicVisitId).Name;
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
                        new SqlParameter("@StatusId", vm.StatusId ?? (object) DBNull.Value),
                        new SqlParameter("@HealthFacilityId", vm.HealthFacilityId ?? (object) DBNull.Value),
                        new SqlParameter("@SubCountyId", vm.SubCountyId ?? (object) DBNull.Value),
                        new SqlParameter("@WardId", vm.WardId ?? (object) DBNull.Value),
                        new SqlParameter("@ClinicVisitId", vm.ClinicVisitId ?? (object) DBNull.Value),
                        new SqlParameter("@Option", "MaternalDeath"),
                        new SqlParameter("@StartDate", vm.StartDate ?? (object) DBNull.Value),
                        new SqlParameter("@EndDate", vm.EndDate ?? (object) DBNull.Value),
                        new SqlParameter("@OptionId", vm.ReasonId ?? (object) DBNull.Value),
                    };
                    var result =
                        SQLExtensions.GetModelFromQuery<ExitedViewModel>(_context,
                            "EXEC [ReportCaseManagement] @StatusId,@HealthFacilityId,@SubCountyId,@WardId,@ClinicVisitId,@Option,@StartDate,@EndDate,@OptionId",
                            parms);
                    vm.ExitedSummaries = result;
                    return View("MaternalDeathSummary", vm);
                }

                var changes = _context.Changes
                    .OrderBy(c => c.Household.Mother.FirstName)
                    .Include(c => c.Household.Village.Ward.SubCounty)
                    .Include(c => c.Household.Mother)
                    .Include(c => c.Household.HealthFacility)
                    .Where(i => i.ChangeTypeId == 306)
                    .AsQueryable();

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
                        h.Household.Mother.FirstName.Contains(vm.Name)
                        || h.Household.Mother.MiddleName.Contains(vm.Name)
                        || h.Household.Mother.Surname.Contains(vm.Name)
                        || h.Household.Mother.Surname.Contains(vm.Name)
                    );
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

                if (vm.StartDate != null)
                {
                    changes = changes.Where(h => h.DeathDate >= DateTime.Parse(vm.StartDate));
                }
                if (vm.EndDate != null)
                {
                    changes = changes.Where(h => h.DeathDate <= DateTime.Parse(vm.EndDate));
                }
                //var test = _context.CaseManagement.ToList();
                //visits.ToList().Join(test, l => l.CaseManagementId, r => r.Id, (lft, rgt) => new ClinicVisit { });

                var page = vm.Page ?? 1;
                var pageSize = vm.PageSize ?? 20;
                vm.Total = changes.Count();
                vm.Changes = changes.ToPagedList(page, vm.Total == 0 ? 1 : vm.Total);
            }

            return View(vm);
        }

        public IActionResult ChildrenDeath(CaseManagementListViewModel vm)
        {
            var reportTitle = (vm.ReportTypeId == 1 ? "Summary of" : "List of") + " Children Deaths";

            if (vm.ClinicVisitId != null)
            {
                reportTitle = reportTitle + " - " + _context.ClinicVisits.Find(vm.ClinicVisitId).Name;
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
                        new SqlParameter("@StatusId", vm.StatusId ?? (object) DBNull.Value),
                        new SqlParameter("@HealthFacilityId", vm.HealthFacilityId ?? (object) DBNull.Value),
                        new SqlParameter("@SubCountyId", vm.SubCountyId ?? (object) DBNull.Value),
                        new SqlParameter("@WardId", vm.WardId ?? (object) DBNull.Value),
                        new SqlParameter("@ClinicVisitId", vm.ClinicVisitId ?? (object) DBNull.Value),
                        new SqlParameter("@Option", "ChildrenDeath"),
                        new SqlParameter("@StartDate", vm.StartDate ?? (object) DBNull.Value),
                        new SqlParameter("@EndDate", vm.EndDate ?? (object) DBNull.Value),
                        new SqlParameter("@OptionId", vm.ReasonId ?? (object) DBNull.Value),
                    };
                    var result =
                        SQLExtensions.GetModelFromQuery<ExitedViewModel>(_context,
                            "EXEC [ReportCaseManagement] @StatusId,@HealthFacilityId,@SubCountyId,@WardId,@ClinicVisitId,@Option,@StartDate,@EndDate,@OptionId",
                            parms);
                    vm.ExitedSummaries = result;
                    return View("ChildrenDeathSummary", vm);
                }

                var changes = _context.Changes
                    .OrderBy(c => c.Household.Mother.FirstName)
                    .Include(c => c.Household.Village.Ward.SubCounty)
                    .Include(c => c.Household.Mother)
                    .Include(c => c.Child)
                    .Include(c => c.Household.HealthFacility)
                    .Where(i => i.ChangeTypeId == 307)
                    .AsQueryable();

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
                        h.Household.Mother.FirstName.Contains(vm.Name)
                        || h.Household.Mother.MiddleName.Contains(vm.Name)
                        || h.Household.Mother.Surname.Contains(vm.Name)
                        || h.Household.Mother.Surname.Contains(vm.Name)
                    );
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

                if (vm.StartDate != null)
                {
                    changes = changes.Where(h => h.DeathDate >= DateTime.Parse(vm.StartDate));
                }
                if (vm.EndDate != null)
                {
                    changes = changes.Where(h => h.DeathDate <= DateTime.Parse(vm.EndDate));
                }
                //var test = _context.CaseManagement.ToList();
                //visits.ToList().Join(test, l => l.CaseManagementId, r => r.Id, (lft, rgt) => new ClinicVisit { });

                var page = vm.Page ?? 1;
                var pageSize = vm.PageSize ?? 20;
                vm.Total = changes.Count();
                vm.Changes = changes.ToPagedList(page, vm.Total == 0 ? 1 : vm.Total);
            }

            return View(vm);
        }

        public IActionResult BreastFeeding(CaseManagementListViewModel vm)
        {
            var reportTitle = (vm.ReportTypeId == 1 ? "Summary of" : "List of") + " children on exclusive breastfeeding";

            if (vm.ClinicVisitId != null)
            {
                reportTitle = reportTitle + " - " + _context.ClinicVisits.Find(vm.ClinicVisitId).Name;
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
                        new SqlParameter("@StatusId", vm.StatusId ?? (object) DBNull.Value),
                        new SqlParameter("@HealthFacilityId", vm.HealthFacilityId ?? (object) DBNull.Value),
                        new SqlParameter("@SubCountyId", vm.SubCountyId ?? (object) DBNull.Value),
                        new SqlParameter("@WardId", vm.WardId ?? (object) DBNull.Value),
                        new SqlParameter("@ClinicVisitId", vm.ClinicVisitId ?? (object) DBNull.Value),
                        new SqlParameter("@Option", "BreastFeedingSummary"),
                        new SqlParameter("@StartDate", vm.StartDate ?? (object) DBNull.Value),
                        new SqlParameter("@EndDate", vm.EndDate ?? (object) DBNull.Value),
                        new SqlParameter("@OptionId", vm.ReasonId ?? (object) DBNull.Value),
                  };
                    var result =
                        SQLExtensions.GetModelFromQuery<HHSummaryViewModel>(_context,
                            "EXEC [ReportCaseManagement] @StatusId,@HealthFacilityId,@SubCountyId,@WardId,@ClinicVisitId,@Option,@StartDate,@EndDate,@OptionId",
                            parms);
                    vm.WardSummaries = result;
                    return View("BreastFeedingSummary", vm);
                }
                else
                {
                    SqlParameter[] parms = new SqlParameter[]
                  {
                        new SqlParameter("@StatusId", vm.StatusId ?? (object) DBNull.Value),
                        new SqlParameter("@HealthFacilityId", vm.HealthFacilityId ?? (object) DBNull.Value),
                        new SqlParameter("@SubCountyId", vm.SubCountyId ?? (object) DBNull.Value),
                        new SqlParameter("@WardId", vm.WardId ?? (object) DBNull.Value),
                        new SqlParameter("@ClinicVisitId", vm.ClinicVisitId ?? (object) DBNull.Value),
                        new SqlParameter("@Option", "BreastFeeding"),
                        new SqlParameter("@StartDate", vm.StartDate ?? (object) DBNull.Value),
                        new SqlParameter("@EndDate", vm.EndDate ?? (object) DBNull.Value),
                        new SqlParameter("@OptionId", vm.ReasonId ?? (object) DBNull.Value),
                  };
                    var result =
                    SQLExtensions.GetModelFromQuery<ChildrenBreastFeedingViewModel>(_context,
                   "EXEC [ReportCaseManagement] @StatusId,@HealthFacilityId,@SubCountyId,@WardId,@ClinicVisitId,@Option,@StartDate,@EndDate,@OptionId",
                            parms);
                    var page = vm.Page ?? 1;

                    var queryable = result.AsQueryable();
                    vm.ChildrenBreastFeeding = queryable.ToPagedList(page, result.Count());
                }
            }

            return View(vm);
        }
    }
}