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
    public class MapsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MapsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var periods = _context.ReportingPeriods.OrderByDescending(i => i.Id).ToList();
            ViewData["PeriodId"] = new SelectList(periods, "Id", "Name");
            ViewData["CurrentPeriodId"] = periods.First().Id;

            var indicators = _context.SystemCodeDetails.Where(i => i.SystemCode.SystemModuleId == 7).ToList();
            ViewData["IndicatorId"] = new SelectList(indicators, "Id", "Code");
            ViewData["CurrentIndicatorId"] = indicators.First().Id;

            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities.OrderBy(i => i.Name), "Id", "Name");

            return View();
        }

        public JsonResult FirstChart(int id, int indicatorId, int? healthFacilityId)
        {
            List<int[]> mapData = new List<int[]>();
            if (indicatorId == 336)
            {
                SqlParameter[] parms = new SqlParameter[]
                {
                    new SqlParameter("@PeriodId", id),
                    new SqlParameter("@Module", "Registration"),
                    new SqlParameter("@IndicatorId", indicatorId),
                    new SqlParameter("@HealthFacilityId", healthFacilityId ?? (object)DBNull.Value)
                };
                var result =
                    SQLExtensions.GetModelFromQuery<RegistrationSummary>(_context,
                        "EXEC [DashboardsMaps] @PeriodId,@Module,@IndicatorId,@HealthFacilityId", parms);

                mapData = result.Where(i => i.Registered > 0 && i.WardId!=null).Select(i => new int[]
                {
                    (int)i.WardId,
                    int.Parse(i.Registered.ToString())
                }).ToList();
            }
            else if (indicatorId == 337)
            {
                SqlParameter[] parms = new SqlParameter[]
                {
                    new SqlParameter("@PeriodId", id),
                    new SqlParameter("@Module", "Registration"),
                    new SqlParameter("@IndicatorId", indicatorId),
                    new SqlParameter("@HealthFacilityId", healthFacilityId ?? (object)DBNull.Value)
                };
                var result =
                    SQLExtensions.GetModelFromQuery<RegistrationSummary>(_context,
                        "EXEC [DashboardsMaps] @PeriodId,@Module,@IndicatorId,@HealthFacilityId", parms);

                mapData = result.Where(i => i.Eligible > 0 && i.WardId != null).Select(i => new int[]
                  {
                    (int)i.WardId,
                    int.Parse(i.Eligible.ToString())
                  }).ToList();
            }
            else if (indicatorId == 338)
            {
                SqlParameter[] parms = new SqlParameter[]
                {
                    new SqlParameter("@PeriodId", id),
                    new SqlParameter("@Module", "Registration"),
                    new SqlParameter("@IndicatorId", indicatorId),
                    new SqlParameter("@HealthFacilityId", healthFacilityId ?? (object)DBNull.Value)
                };
                var result =
                    SQLExtensions.GetModelFromQuery<RegistrationSummary>(_context,
                        "EXEC [DashboardsMaps] @PeriodId,@Module,@IndicatorId,@HealthFacilityId", parms);

                mapData = result.Where(i => i.Ineligible > 0 && i.WardId != null).Select(i => new int[]
                {
                    (int)i.WardId,
                    int.Parse(i.Ineligible.ToString())
                }).ToList();
            }
            else if (indicatorId == 339)
            {
                SqlParameter[] parms = new SqlParameter[]
                {
                    new SqlParameter("@PeriodId", id),
                    new SqlParameter("@Module", "CommunityValidation"),
                    new SqlParameter("@IndicatorId", indicatorId),
                    new SqlParameter("@HealthFacilityId", healthFacilityId ?? (object)DBNull.Value)
                };
                var result =
                    SQLExtensions.GetModelFromQuery<CommunityValidationSummary>(_context,
                        "EXEC [DashboardsMaps] @PeriodId,@Module,@IndicatorId,@HealthFacilityId", parms);

                mapData = result.Where(i => i.Low > 0 && i.WardId != null).Select(i => new int[]
                {
                    (int)i.WardId,
                    int.Parse(i.Low.ToString())
                }).ToList();
            }
            else if (indicatorId == 343)
            {
                SqlParameter[] parms = new SqlParameter[]
                {
                    new SqlParameter("@PeriodId", id),
                    new SqlParameter("@Module", "CommunityValidation"),
                    new SqlParameter("@IndicatorId", indicatorId),
                    new SqlParameter("@HealthFacilityId", healthFacilityId ?? (object)DBNull.Value)
                };
                var result =
                    SQLExtensions.GetModelFromQuery<CommunityValidationSummary>(_context,
                        "EXEC [DashboardsMaps] @PeriodId,@Module,@IndicatorId,@HealthFacilityId", parms);

                mapData = result.Where(i => i.Medium > 0 && i.WardId != null).Select(i => new int[]
                {
                    (int)i.WardId,
                    int.Parse(i.Medium.ToString())
                }).ToList();
            }
            else if (indicatorId == 344)
            {
                SqlParameter[] parms = new SqlParameter[]
                {
                    new SqlParameter("@PeriodId", id),
                    new SqlParameter("@Module", "CommunityValidation"),
                    new SqlParameter("@IndicatorId", indicatorId),
                    new SqlParameter("@HealthFacilityId", healthFacilityId ?? (object)DBNull.Value)
                };
                var result =
                    SQLExtensions.GetModelFromQuery<CommunityValidationSummary>(_context,
                        "EXEC [DashboardsMaps] @PeriodId,@Module,@IndicatorId,@HealthFacilityId", parms);

                mapData = result.Where(i => i.High > 0 && i.WardId != null).Select(i => new int[]
                {
                    (int)i.WardId,
                    int.Parse(i.High.ToString())
                }).ToList();
            }
            else if (indicatorId == 345) // Number of enrolled mothers within the reporting period
            {
                SqlParameter[] parms = new SqlParameter[]
                {
                    new SqlParameter("@PeriodId", id),
                    new SqlParameter("@Module", "Enrolment"),
                    new SqlParameter("@IndicatorId", indicatorId),
                    new SqlParameter("@HealthFacilityId", healthFacilityId ?? (object)DBNull.Value)
                };
                var result =
                    SQLExtensions.GetModelFromQuery<EnrolmentSummary>(_context,
                        "EXEC [DashboardsMaps] @PeriodId,@Module,@IndicatorId,@HealthFacilityId", parms);

                mapData = result.Where(i => i.Enrolled > 0).Select(i => new int[]
                {
                    (int)i.WardId,
                    int.Parse(i.Enrolled.ToString())
                }).ToList();
            }
            else if (indicatorId == 346) // Number of eligible mothers on 'waiting list'
            {
                SqlParameter[] parms = new SqlParameter[]
                {
                    new SqlParameter("@PeriodId", id),
                    new SqlParameter("@Module", "Enrolment"),
                    new SqlParameter("@IndicatorId", indicatorId),
                    new SqlParameter("@HealthFacilityId", healthFacilityId ?? (object)DBNull.Value)
                };
                var result =
                    SQLExtensions.GetModelFromQuery<EnrolmentSummary>(_context,
                        "EXEC [DashboardsMaps] @PeriodId,@Module,@IndicatorId,@HealthFacilityId", parms);

                mapData = result.Where(i => i.Waiting > 0).Select(i => new int[]
                {
                    (int)i.WardId,
                    int.Parse(i.Waiting.ToString())
                }).ToList();
            }
            else if (indicatorId == 347) // Total number of ACTIVE mothers enrolled
            {
                SqlParameter[] parms = new SqlParameter[]
                {
                    new SqlParameter("@PeriodId", id),
                    new SqlParameter("@Module", "Enrolment"),
                    new SqlParameter("@IndicatorId", indicatorId),
                    new SqlParameter("@HealthFacilityId", healthFacilityId ?? (object)DBNull.Value)
                };
                var result =
                    SQLExtensions.GetModelFromQuery<EnrolmentSummary>(_context,
                        "EXEC [DashboardsMaps] @PeriodId,@Module,@IndicatorId,@HealthFacilityId", parms);

                mapData = result.Where(i => i.Active > 0 && i.WardId!=null).Select(i => new int[]
                {
                    (int)i.WardId,
                    int.Parse(i.Active.ToString())
                }).ToList();
            }
            else if (indicatorId == 348) // Total number of PEAK mothers enrolled
            {
                SqlParameter[] parms = new SqlParameter[]
                {
                    new SqlParameter("@PeriodId", id),
                    new SqlParameter("@Module", "Enrolment"),
                    new SqlParameter("@IndicatorId", indicatorId),
                    new SqlParameter("@HealthFacilityId", healthFacilityId ?? (object)DBNull.Value)
                };
                var result =
                    SQLExtensions.GetModelFromQuery<EnrolmentSummary>(_context,
                        "EXEC [DashboardsMaps] @PeriodId,@Module,@IndicatorId,@HealthFacilityId", parms);

                mapData = result.Where(i => i.Peak > 0 && i.WardId != null).Select(i => new int[]
                {
                    (int)i.WardId,
                    int.Parse(i.Peak.ToString())
                }).ToList();
            }
            else if (indicatorId == 349) // Number of mothers paid within the reporting period
            {
                SqlParameter[] parms = new SqlParameter[]
                {
                    new SqlParameter("@PeriodId", id),
                    new SqlParameter("@Module", "Payments"),
                    new SqlParameter("@IndicatorId", indicatorId),
                    new SqlParameter("@HealthFacilityId", healthFacilityId ?? (object)DBNull.Value)
                };
                var result =
                    SQLExtensions.GetModelFromQuery<PaymentsSummary>(_context,
                        "EXEC [Dashboards] @PeriodId,@Module,@IndicatorId,@HealthFacilityId,@IndicatorId", parms);

                mapData = result.Where(i=>i.WardId != null).Select(i => new int[]
                {
                    (int)i.WardId,
                   i.Stage1+i.Stage2+i.Stage3+i.Stage4+i.Stage5+i.Stage6+i.Benevolent
                }).ToList();
            }
            else if (indicatorId == 349 || indicatorId == 350 || indicatorId == 351 || indicatorId == 352) // Payments
            {
                SqlParameter[] parms = new SqlParameter[]
                {
                    new SqlParameter("@PeriodId", id),
                    new SqlParameter("@Module", "Payments"),
                    new SqlParameter("@IndicatorId", indicatorId),
                    new SqlParameter("@HealthFacilityId", healthFacilityId ?? (object)DBNull.Value)
                };
                var result =
                    SQLExtensions.GetModelFromQuery<PaymentsSummary>(_context,
                        "EXEC [Dashboards] @PeriodId,@Module,@IndicatorId,@HealthFacilityId,@IndicatorId", parms);

                mapData = result.Where(i => i.WardId != null).Select(i => new int[]
                    {
                    (int)i.WardId,
                    i.Stage1+i.Stage2+i.Stage3+i.Stage4+i.Stage5+i.Stage6+i.Benevolent
                    }).ToList();
            }
            return Json(mapData);
        }
    }
}