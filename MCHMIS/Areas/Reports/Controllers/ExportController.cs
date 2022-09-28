using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using MCHMIS.Data;
using MCHMIS.Interfaces;
using MCHMIS.Services;
using MCHMIS.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MCHMIS.Areas.Reports.Controllers
{
    [Area("Reports")]
    public class ExportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IUnitOfWork _uow;

        private readonly IDBService _dbService;

        public ExportController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment, IUnitOfWork uow, IDBService dbService)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _uow = uow;
            _dbService = dbService;
        }

        public IActionResult Index(ExportViewModel vm)
        {
            ViewData["ModuleId"] = new SelectList(_context.SystemCodeDetails.Where(i=>i.SystemCode.Code=="Data Export Modules"), "Id", "Code", vm.ModuleId);
            ViewData["ReportId"] = new SelectList(_context.SystemCodeDetails.Where(i=>i.SystemCode.Code=="CMR"), "Id", "Code", vm.ReportId);
            return View();
        }
        public FileContentResult Download(ExportViewModel vm)
        {
            _context.Database.SetCommandTimeout(1000);
            var conString = DBService.DBConnection();
            var connection = new SqlConnectionStringBuilder(conString);
            var module = _context.SystemCodeDetails.Single(i => i.Id == vm.ModuleId).Code;
            var fileName = "";
            SqlParameter[] @params =
            {
                new SqlParameter("DBServer", connection.DataSource),
                new SqlParameter("DBName", connection.InitialCatalog),
                new SqlParameter("DBUser", connection.UserID),
                new SqlParameter("DBPassword", connection.Password),
                new SqlParameter("returnVal", SqlDbType.VarChar) { Direction = ParameterDirection.Output,DbType = DbType.String,Size = 100}
            };
            if (module.Equals("Registration & Targeting"))
            {
                _context.Database.ExecuteSqlCommand(";Exec ExportRegistrationData @DBServer,@DBName,@DBUser,@DBPassword,@returnVal OUT",
                    @params
                    );
                fileName = (string)@params[4].Value;
            }
            else if (module.Equals("Community Validation"))
            {
                _context.Database.ExecuteSqlCommand(";Exec ExportCommunityValidationData @DBServer,@DBName,@DBUser,@DBPassword,@returnVal OUT",
                    @params);
                fileName = (string)@params[4].Value;
            }
            else if (module.Equals("Case Management"))
            {
                SqlParameter[] @params2 =
                {
                    new SqlParameter("DBServer", connection.DataSource),
                    new SqlParameter("DBName", connection.InitialCatalog),
                    new SqlParameter("DBUser", connection.UserID),
                    new SqlParameter("DBPassword", connection.Password),
                    new SqlParameter("Option", vm.ReportId),
                    new SqlParameter("returnVal", SqlDbType.VarChar) { Direction = ParameterDirection.Output,DbType = DbType.String,Size = 100}
                };
                _context.Database.ExecuteSqlCommand(";Exec ExportCaseManagementData @DBServer,@DBName,@DBUser,@DBPassword,@Option,@returnVal OUT",
                    @params2);
                fileName = (string)@params2[5].Value;
            }
            else if (module.Equals("Payments"))
            {
                _context.Database.ExecuteSqlCommand(";Exec ExportPaymentsData @DBServer,@DBName,@DBUser,@DBPassword,@returnVal OUT",
                    @params);
                fileName = (string)@params[4].Value;
            }
            var path = _context.SystemSettings.Single(i => i.key == "DOWNLOAD.PATH").Value;
            string filePath = path + fileName;

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/force-download", fileName);
        }
    }
}