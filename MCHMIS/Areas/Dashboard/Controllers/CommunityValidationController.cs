using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using MCHMIS.Data;
using MCHMIS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MCHMIS.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    [Authorize]
    public class CommunityValidationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CommunityValidationController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var periods = _context.ReportingPeriods.OrderByDescending(i => i.Id).ToList();
            ViewData["PeriodId"] = new SelectList(periods, "Id", "Name");
            ViewData["CurrentPeriodId"] = periods.First().Id;
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities.OrderBy(i => i.Name), "Id", "Name");
            ViewData["WardId"] = new SelectList(_context.Wards.OrderBy(i => i.Name), "Id", "Name");
            return View();
        }

        //[HttpPost]
        public JsonResult FirstChart(int id, int? healthFacilityId, int? wardId)
        {
            SqlParameter[] parms = new SqlParameter[]
            {
                new SqlParameter("@PeriodId", id),
                new SqlParameter("@Module", "CommunityValidation"),
                new SqlParameter("@HealthFacilityId", healthFacilityId ?? (object)DBNull.Value),
                new SqlParameter("@WardId", wardId ?? (object)DBNull.Value)
            };
            var result =
                SQLExtensions.GetModelFromQuery<CommunityValidationSummary>(_context,
                    "EXEC [Dashboards] @PeriodId,@Module,@HealthFacilityId,@WardId", parms).FirstOrDefault();
            if (result == null)
                result = new CommunityValidationSummary();
            return Json(result);
        }
    }
}