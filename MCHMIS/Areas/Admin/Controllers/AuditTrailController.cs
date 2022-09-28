using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using ClosedXML.Extensions;
using MCHMIS.Data;
using MCHMIS.Models;
using MCHMIS.Services;
using MCHMIS.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace MCHMIS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin")]
    [Permission("Security:Manage Security")]
    public class AuditTrailController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IExportService _exportService;

        public AuditTrailController(ApplicationDbContext context, IExportService exportService)
        {
            _context = context;
            _exportService = exportService;
        }

        [Route("audit-trail", Name = "AuditTrail")]
        public async Task<IActionResult> Index(AuditTrailListViewModel vm)
        {
            IQueryable<AuditTrail> data = _context.AuditTrail
                .OrderByDescending(i => i.Date).Include(a => a.User);
            if (!string.IsNullOrEmpty(vm.ChangeType))
            {
                data = data.Where(i => i.ChangeType == vm.ChangeType);
            }
            if (!string.IsNullOrEmpty(vm.TableName))
            {
                data = data.Where(i => i.ChangeType == vm.ChangeType);
            }
            if (!string.IsNullOrEmpty(vm.FieldName))
            {
                data = data.Where(i => i.Description.Contains(vm.FieldName));
            }
            if (!string.IsNullOrEmpty(vm.UserId))
            {
                data = data.Where(i => i.UserId == vm.UserId);
            }
            if (vm.StartDate!=null)
            {
                data = data.Where(i => i.Date >=vm.StartDate);
            }
            var DbF = EF.Functions;
            DbFunctions dfunc = null;
            if (vm.EndDate != null)
            {
                data = data.Where(i => dfunc.DateDiffDay(i.Date,vm.EndDate)>=0);
            }
            if (vm.Option != null && vm.Option.Equals("export"))
            {

                _context.Database.SetCommandTimeout(1000);
                var conString = DBService.DBConnection();
                var connection = new SqlConnectionStringBuilder(conString);
                var fileName = "";
                SqlParameter[] @params =
                {
                    new SqlParameter("DBServer", connection.DataSource),
                    new SqlParameter("DBName", connection.InitialCatalog),
                    new SqlParameter("DBUser", connection.UserID),
                    new SqlParameter("DBPassword", connection.Password),
                    new SqlParameter("UserId", vm.UserId?? (object)DBNull.Value),
                    new SqlParameter("ChangeType", vm.ChangeType?? (object)DBNull.Value),
                    new SqlParameter("TableName", vm.TableName?? (object)DBNull.Value),
                    new SqlParameter("FieldName", vm.FieldName?? (object)DBNull.Value),
                    new SqlParameter("StartDate", vm.StartDate?? (object)DBNull.Value),
                    new SqlParameter("EndDate", vm.EndDate?? (object)DBNull.Value),
                    new SqlParameter("returnVal", SqlDbType.VarChar) { Direction = ParameterDirection.Output,DbType = DbType.String,Size = 100}
                };
                _context.Database.ExecuteSqlCommand(";Exec ExportAuditTrail @DBServer,@DBName,@DBUser,@DBPassword,@UserId,@ChangeType,@TableName,@FieldName,@StartDate,@EndDate,@returnVal OUT",
                    @params
                );
                fileName = (string)@params[10].Value;
                var path = _context.SystemSettings.Single(i => i.key == "DOWNLOAD.PATH").Value;
                string filePath = path + fileName;

                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

                return File(fileBytes, "application/force-download", fileName);
            }
            var page = vm.Page ?? 1;
            var pageSize = vm.PageSize ?? 20;
            vm.AuditTrails = data.ToPagedList(page, pageSize);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "DisplayName", vm.UserId);
            var changeTypes = _context.AuditTrail.Select(i => i.ChangeType).Distinct()
                .Select(i => new ChangeTypeViewModel { ChangeType = i }).ToList();
            ViewData["ChangeType"] = new SelectList(changeTypes, "ChangeType", "ChangeType", vm.ChangeType);
            var tables = _context.AuditTrail.Select(i => i.TableName).Distinct()
                .Select(i => new TableViewModel { TableName = i }).ToList();
            ViewData["Table"] = new SelectList(tables, "TableName", "TableName", vm.TableName);
            return View(vm);
        }

        [Route("audit-trail/details/{id}", Name = "AuditTrailDetails")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auditTrail = await _context.AuditTrail
                .Include(a => a.User)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (auditTrail == null)
            {
                return NotFound();
            }

            return View(auditTrail);
        }
    }
}