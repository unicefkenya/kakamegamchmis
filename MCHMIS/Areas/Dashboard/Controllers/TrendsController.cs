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
using Newtonsoft.Json;

namespace MCHMIS.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    [Authorize]
    public class TrendsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrendsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(TrendsViewModel vm)
        {
            var periods = _context.ReportingPeriods.ToList();
            ViewData["PeriodId"] = new SelectList(periods, "Id", "Name");
            ViewData["CurrentPeriodId"] = periods.First().Id;
            var indicators = _context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Dashboard Trends");
            DateTime PreviousYearDate = System.DateTime.Now.AddYears(-1);//Previous Year Date
            vm.StartPeriodId = periods.First(i => i.StartDate >= PreviousYearDate).Id;
            vm.EndPeriodId = periods.OrderByDescending(i => i.Id).First().Id;
            ViewData["IndicatorId"] = new SelectList(indicators, "Id", "Code", vm.StartPeriodId);

            ViewData["CurrentIndicatorId"] = indicators.OrderByDescending(i => i.Id).First();

            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities.OrderBy(i => i.Name), "Id", "Name");
            ViewData["WardId"] = new SelectList(_context.Wards.OrderBy(i => i.Name), "Id", "Name");

            return View(vm);
        }

        //[HttpPost]
        public JsonResult FirstChart(TrendsViewModel vm)
        {
            var module = "";
            if (vm.IndicatorId == 369)
            {
                module = "Registration";
            }
            else if (vm.IndicatorId == 370)
            {
                module = "CaseManagement";
            }
            else if (vm.IndicatorId == 371)
            {
                module = "Payments";
            }
            else if (vm.IndicatorId == 372)
            {
                module = "Complaints";
            }
            SqlParameter[] parms = new SqlParameter[]
            {
                new SqlParameter("@Module", module),
                new SqlParameter("@HealthFacilityId", vm.HealthFacilityId ??  (object)DBNull.Value),
                new SqlParameter("@StartPeriodId", vm.StartPeriodId ??  (object)DBNull.Value),
                new SqlParameter("@EndPeriodId", vm.EndPeriodId ??  (object)DBNull.Value),
            };
            var periods = _context.ReportingPeriods.AsQueryable();
            if (vm.StartPeriodId != null)
                periods = periods.Where(i => i.Id >= vm.StartPeriodId);
            if (vm.EndPeriodId != null)
                periods = periods.Where(i => i.Id <= vm.EndPeriodId);

            vm.ReportingPeriods = periods.ToList();
            List<string> categories = new List<string>();
            List<MonthViewModel> monthsSummary = new List<MonthViewModel>();
            if (vm.IndicatorId == 369) // Beneficiaries
            {
                var result =
                SQLExtensions.GetModelFromQuery<RegistrationTrend>(_context,
                    "EXEC [DashboardsTrends] @Module,@HealthFacilityId,@StartPeriodId,@EndPeriodId", parms).ToList();

                var monthViewModel = new MonthViewModel
                {
                    Category = "Registered",
                };
                List<int> values = new List<int>();
                foreach (var month in vm.ReportingPeriods)
                {
                    var value = result.SingleOrDefault(y => y.PeriodId == month.Id);
                    if (value != null)
                    {
                        values.Add(value.Registered);
                    }
                    else
                    {
                        values.Add(0);
                    }
                }
                monthViewModel.Values = values;

                monthsSummary.Add(monthViewModel);
                // Validated
                monthViewModel = new MonthViewModel
                {
                    Category = "Validated",
                };
                values = new List<int>();
                foreach (var month in vm.ReportingPeriods)
                {
                    var value = result.SingleOrDefault(y => y.PeriodId == month.Id);
                    if (value != null)
                    {
                        values.Add(value.Validated);
                    }
                    else
                    {
                        values.Add(0);
                    }
                }
                monthViewModel.Values = values;

                monthsSummary.Add(monthViewModel);

                // Enrolled
                monthViewModel = new MonthViewModel
                {
                    Category = "Enrolled",
                };
                values = new List<int>();
                foreach (var month in vm.ReportingPeriods)
                {
                    var value = result.SingleOrDefault(y => y.PeriodId == month.Id);
                    if (value != null)
                    {
                        values.Add(value.Enrolled);
                    }
                    else
                    {
                        values.Add(0);
                    }
                }
                monthViewModel.Values = values;

                monthsSummary.Add(monthViewModel);

                vm.TrendsSummary = monthsSummary;

                foreach (var item in vm.ReportingPeriods)
                {
                    categories.Add(item.Name);
                }
            }
            else if (vm.IndicatorId == 370) // Case Management
            {
                var result =
               SQLExtensions.GetModelFromQuery<CaseManagementSummary>(_context,
                   "EXEC [DashboardsTrends] @Module,@HealthFacilityId,@StartPeriodId,@EndPeriodId", parms).ToList();

                var monthViewModel = new MonthViewModel
                {
                    Category = "Updated",
                };
                List<int> values = new List<int>();
                foreach (var month in vm.ReportingPeriods)
                {
                    var value = result.SingleOrDefault(y => y.PeriodId == month.Id);
                    if (value != null)
                    {
                        values.Add(value.Updated);
                    }
                    else
                    {
                        values.Add(0);
                    }
                }
                monthViewModel.Values = values;

                monthsSummary.Add(monthViewModel);
                // Validated
                monthViewModel = new MonthViewModel
                {
                    Category = "Missed",
                };
                values = new List<int>();
                foreach (var month in vm.ReportingPeriods)
                {
                    var value = result.SingleOrDefault(y => y.PeriodId == month.Id);
                    if (value != null)
                    {
                        values.Add(value.Missed);
                    }
                    else
                    {
                        values.Add(0);
                    }
                }
                monthViewModel.Values = values;

                monthsSummary.Add(monthViewModel);

                vm.TrendsSummary = monthsSummary;

                foreach (var item in vm.ReportingPeriods)
                {
                    categories.Add(item.Name);
                }
            }
            else if (vm.IndicatorId == 371) // Payments
            {
                var result =
               SQLExtensions.GetModelFromQuery<PaymentsSummary>(_context,
                   "EXEC [DashboardsTrends] @Module,@HealthFacilityId,@StartPeriodId,@EndPeriodId", parms).ToList();

                var paymentPoints = _context.PaymentPoints.ToList();
                foreach (var point in paymentPoints)
                {
                    var monthViewModel = new MonthViewModel
                    {
                        Category = point.Name
                    };
                    List<int> values = new List<int>();
                    foreach (var month in vm.ReportingPeriods)
                    {
                        var value = result.SingleOrDefault(y => y.PeriodId == month.Id);
                        if (value != null)
                        {
                            switch (point.Id)
                            {
                                case 1:
                                    values.Add(value.Stage1);
                                    break;

                                case 2:
                                    values.Add(value.Stage2);
                                    break;

                                case 3:
                                    values.Add(value.Stage3);
                                    break;

                                case 4:
                                    values.Add(value.Stage4);
                                    break;

                                case 5:
                                    values.Add(value.Stage5);
                                    break;
                            }
                        }
                        else
                        {
                            values.Add(0);
                        }
                    }
                    monthViewModel.Values = values;

                    monthsSummary.Add(monthViewModel);
                }

                vm.TrendsSummary = monthsSummary;

                foreach (var item in vm.ReportingPeriods)
                {
                    categories.Add(item.Name);
                }
            }
            else if (vm.IndicatorId == 372) // Complaints
            {
                var result =
                    SQLExtensions.GetModelFromQuery<ComplaintsSummary>(_context,
                        "EXEC [DashboardsTrends] @Module,@HealthFacilityId,@StartPeriodId,@EndPeriodId", parms).ToList();

                var monthViewModel = new MonthViewModel
                {
                    Category = "Open Within SLA",
                };
                List<int> values = new List<int>();
                foreach (var month in vm.ReportingPeriods)
                {
                    var value = result.SingleOrDefault(y => y.PeriodId == month.Id);
                    if (value != null)
                    {
                        values.Add(value.OpenWithinSLA);
                    }
                    else
                    {
                        values.Add(0);
                    }
                }
                monthViewModel.Values = values;

                monthsSummary.Add(monthViewModel);
                // Start
                monthViewModel = new MonthViewModel
                {
                    Category = "Open Outside SLA",
                };
                values = new List<int>();
                foreach (var month in vm.ReportingPeriods)
                {
                    var value = result.SingleOrDefault(y => y.PeriodId == month.Id);
                    if (value != null)
                    {
                        values.Add(value.OpenOutsideSLA);
                    }
                    else
                    {
                        values.Add(0);
                    }
                }
                monthViewModel.Values = values;
                monthsSummary.Add(monthViewModel);
                // End
                // Start
                monthViewModel = new MonthViewModel
                {
                    Category = "Resolved Within SLA",
                };
                values = new List<int>();
                foreach (var month in vm.ReportingPeriods)
                {
                    var value = result.SingleOrDefault(y => y.PeriodId == month.Id);
                    if (value != null)
                    {
                        values.Add(value.ResolvedWithinSLA);
                    }
                    else
                    {
                        values.Add(0);
                    }
                }
                monthViewModel.Values = values;
                monthsSummary.Add(monthViewModel);
                // End
                // Start
                monthViewModel = new MonthViewModel
                {
                    Category = "Resolved Outside SLA",
                };
                values = new List<int>();
                foreach (var month in vm.ReportingPeriods)
                {
                    var value = result.SingleOrDefault(y => y.PeriodId == month.Id);
                    if (value != null)
                    {
                        values.Add(value.ResolvedOutsideSLA);
                    }
                    else
                    {
                        values.Add(0);
                    }
                }
                monthViewModel.Values = values;
                monthsSummary.Add(monthViewModel);
                // End

                vm.TrendsSummary = monthsSummary;

                foreach (var item in vm.ReportingPeriods)
                {
                    categories.Add(item.Name);
                }
            }

            var returnData = new
            {
                Categories = categories,
                Data = monthsSummary,
            };

            return Json(returnData);
        }
    }
}