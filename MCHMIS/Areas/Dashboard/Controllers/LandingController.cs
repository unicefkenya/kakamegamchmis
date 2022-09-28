using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using ElmahCore;
using MCHMIS.Data;
using MCHMIS.Models;
using MCHMIS.Services;
using MCHMIS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MCHMIS.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    [Authorize]
    public class LandingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IDBService _dbService;

        public LandingController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment, IDBService dbService)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _dbService = dbService;
        }

        public async Task<IActionResult> Index()
        {
            // var healthFacilityId = _dbService.GetHealthFacilityId();
            //bool isGlobal = await _dbService.IsGlobal();

            //var vm = new DashboardViewModel
            //{
            //    OpenComplaints = _context.Complaints.Count(i => i.StatusId == 1 && (i.HealthFacilityId == healthFacilityId || isGlobal)),
            //    ClosedComplaints = _context.Complaints.Count(i => i.StatusId == 4 && (i.HealthFacilityId == healthFacilityId || isGlobal)),
            //    PendingUpdates = _context.Changes.Count(i => i.StatusId == 1 && (i.Household.HealthFacilityId == healthFacilityId || isGlobal)),
            //    ApprovedUpdates = _context.Changes.Count(i => i.StatusId == 3 && (i.Household.HealthFacilityId == healthFacilityId || isGlobal)),
            //};
            var vm = new DashboardViewModel();
            var periods = _context.ReportingPeriods.OrderByDescending(i => i.Id).ToList();
            ViewData["PeriodId"] = new SelectList(periods, "Id", "Name");
            ViewData["CurrentPeriodId"] = periods.First().Id;
            ViewData["PeriodName"] = periods.First().Name;
            return View(vm);
        }

        public JsonResult FirstChart(int id, int? healthFacilityId, int? wardId)
        {
            // var latestPeriod = dbService.GetLatestPeriod();

            SqlParameter[] parms = new SqlParameter[]
            {
                new SqlParameter("@PeriodId", id),
                new SqlParameter("@Module", "Registration"),
                new SqlParameter("@HealthFacilityId", healthFacilityId ?? (object)DBNull.Value),
                new SqlParameter("@WardId", wardId ?? (object)DBNull.Value)
            };
            var registration =
                SQLExtensions.GetModelFromQuery<RegistrationSummary>(_context,
                    "EXEC [Dashboards] @PeriodId,@Module,@HealthFacilityId,@WardId", parms).FirstOrDefault();

            if (registration == null)
                registration = new RegistrationSummary();

            parms = new SqlParameter[]
              {
                new SqlParameter("@PeriodId", id),
                new SqlParameter("@Module", "CommunityValidation"),
                new SqlParameter("@HealthFacilityId", healthFacilityId ?? (object)DBNull.Value),
                new SqlParameter("@WardId", wardId ?? (object)DBNull.Value)
              };
            var cv =
                SQLExtensions.GetModelFromQuery<CommunityValidationSummary>(_context,
                    "EXEC [Dashboards] @PeriodId,@Module,@HealthFacilityId,@WardId", parms).FirstOrDefault() ?? new CommunityValidationSummary();
            parms = new SqlParameter[]
            {
                new SqlParameter("@PeriodId", id),
                new SqlParameter("@Module", "Enrolment"),
                new SqlParameter("@HealthFacilityId", healthFacilityId ?? (object)DBNull.Value),
                new SqlParameter("@WardId", wardId ?? (object)DBNull.Value)
            };
            var enrolment =
                SQLExtensions.GetModelFromQuery<EnrolmentSummary>(_context,
                    "EXEC [Dashboards] @PeriodId,@Module,@HealthFacilityId,@WardId", parms).FirstOrDefault() ?? new EnrolmentSummary();

            parms = new SqlParameter[]
            {
                new SqlParameter("@PeriodId", id),
                new SqlParameter("@Module", "Payments"),
                new SqlParameter("@HealthFacilityId", healthFacilityId ?? (object)DBNull.Value),
                new SqlParameter("@WardId", wardId ?? (object)DBNull.Value),
                new SqlParameter("@IndicatorId", 349),
            };
            var payments =
                SQLExtensions.GetModelFromQuery<PaymentsSummary>(_context,
                    "EXEC [Dashboards] @PeriodId,@Module,@HealthFacilityId,@WardId,@IndicatorId", parms).FirstOrDefault() ??
                new PaymentsSummary();

            parms = new SqlParameter[]
              {
                new SqlParameter("@PeriodId", id),
                new SqlParameter("@Module", "CaseManagement"),
                new SqlParameter("@HealthFacilityId", healthFacilityId ?? (object)DBNull.Value),
                new SqlParameter("@WardId", wardId ?? (object)DBNull.Value)
              };
            var cases =
                SQLExtensions.GetModelFromQuery<CaseManagementSummary>(_context,
                    "EXEC [Dashboards] @PeriodId,@Module,@HealthFacilityId,@WardId", parms).FirstOrDefault() ?? new CaseManagementSummary();
            parms = new SqlParameter[]
            {
                new SqlParameter("@PeriodId", id),
                new SqlParameter("@Module", "Complaints"),
                new SqlParameter("@HealthFacilityId", healthFacilityId ?? (object)DBNull.Value),
                new SqlParameter("@WardId", wardId ?? (object)DBNull.Value)
            };
            var complaints =
                SQLExtensions.GetModelFromQuery<ComplaintsSummary>(_context,
                    "EXEC [Dashboards] @PeriodId,@Module,@HealthFacilityId,@WardId", parms).FirstOrDefault() ?? new ComplaintsSummary();

            var result = new
            {
                Registration = registration,
                CV = cv,
                Enrolment = enrolment,
                Payments = payments,
                Cases = cases,
                Complaints = complaints
            };

            return Json(result);
        }
    }

    //string webRootPath = _hostingEnvironment.WebRootPath;
    //string contentRootPath = _hostingEnvironment.ContentRootPath;
    //var filepath = webRootPath + "/uploads/data.xlsx";

    //FileStream stream = System.IO.File.Open(filepath, FileMode.Open, FileAccess.Read);

    //IExcelDataReader reader = null;
    //if (filepath.EndsWith(".xls"))
    //{
    //    //reads the excel file with .xls extension
    //    reader = ExcelReaderFactory.CreateBinaryReader(stream);
    //}
    //else if (filepath.EndsWith(".xlsx"))
    //{
    //    //reads excel file with .xlsx extension
    //    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
    //}

    //DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
    //{
    //    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
    //    {
    //        UseHeaderRow = true
    //    }
    //});
    //var table = result.Tables[0];
    //using (var bulkCopy = new SqlBulkCopy(_context.Database.GetDbConnection().ConnectionString, SqlBulkCopyOptions.KeepIdentity))
    //{
    //    bulkCopy.ColumnMappings.Add(0, "Col1");

    //    bulkCopy.ColumnMappings.Add(1, "Col2");

    //    bulkCopy.ColumnMappings.Add(2, "Col3");

    //    bulkCopy.BulkCopyTimeout = 600;
    //    bulkCopy.DestinationTableName = "TempTable";
    //    bulkCopy.WriteToServer(table);
}