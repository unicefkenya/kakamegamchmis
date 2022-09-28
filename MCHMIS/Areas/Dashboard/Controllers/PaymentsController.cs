using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using MCHMIS.Areas.Dashboard.ViewModels;
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
    public class PaymentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var periods = _context.ReportingPeriods.OrderByDescending(i => i.Id).ToList();
            ViewData["PeriodId"] = new SelectList(periods, "Id", "Name");
            ViewData["CurrentPeriodId"] = periods.First().Id;
            var indicators = _context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Payment Indicators");

            ViewData["IndicatorId"] = new SelectList(indicators, "Id", "Code");

            ViewData["CurrentIndicatorId"] = indicators.First().Id;

            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities.OrderBy(i => i.Name), "Id", "Name");
            ViewData["WardId"] = new SelectList(_context.Wards.OrderBy(i => i.Name), "Id", "Name");

            return View();
        }

        //[HttpPost]
        public JsonResult FirstChart(string id, string indicatorId, int? healthFacilityId, int? wardId)
        {
            SqlParameter[] parms = new SqlParameter[]
            {
                new SqlParameter("@PeriodId", id),
                new SqlParameter("@Module", "Payments"),
                new SqlParameter("@HealthFacilityId", healthFacilityId ?? (object)DBNull.Value),
                new SqlParameter("@WardId", wardId ?? (object)DBNull.Value),
                new SqlParameter("@IndicatorId", indicatorId),
            };
            var result =
                SQLExtensions.GetModelFromQuery<PaymentsSummary>(_context,
                    "EXEC [Dashboards] @PeriodId,@Module,@HealthFacilityId,@WardId,@IndicatorId", parms).FirstOrDefault();
            if (result == null)
                result = new PaymentsSummary();
            return Json(result);
        }
    }
}