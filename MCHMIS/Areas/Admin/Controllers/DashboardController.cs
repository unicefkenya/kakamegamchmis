using System;
using System.Linq;
using System.Threading.Tasks;
using ElmahCore;
using MCHMIS.Data;
using MCHMIS.Services;
using MCHMIS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace MCHMIS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IDBService _dbService;
        public DashboardController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment, IDBService dbService)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _dbService = dbService;
        }

        public async Task<IActionResult> Index()
        {
            var healthFacilityId = _dbService.GetHealthFacilityId();
            bool isGlobal = await _dbService.IsGlobal();
          
            var vm = new DashboardViewModel
            {
                OpenComplaints = _context.Complaints.Count(i=>i.StatusId==1 && (i.HealthFacilityId == healthFacilityId || isGlobal)),
                ClosedComplaints = _context.Complaints.Count(i => i.StatusId == 4 && (i.HealthFacilityId == healthFacilityId || isGlobal)),
                PendingUpdates = _context.Changes.Count(i=>i.StatusId==1 && (i.Household.HealthFacilityId == healthFacilityId || isGlobal)),
                ApprovedUpdates = _context.Changes.Count(i => i.StatusId == 3 && (i.Household.HealthFacilityId == healthFacilityId || isGlobal)),
            };
          
            return View(vm);
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
        //}
    }
}