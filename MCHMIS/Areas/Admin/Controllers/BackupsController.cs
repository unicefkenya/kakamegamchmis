using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MCHMIS.Data;
using MCHMIS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using X.PagedList;

namespace MCHMIS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BackupsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BackupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(BackupListViewModel vm)
        {
            var page = vm.Page ?? 1;
            var pageSize = vm.PageSize ?? 10;
            var backupPath = _context.SystemSettings.Single(i => i.key == "DATABASE.BACKUP.PATH").Value;
            var provider = new PhysicalFileProvider(backupPath);
            var contents = provider.GetDirectoryContents(string.Empty);
            vm.Files =
                contents.Select(i => new DatabaseBackupsList
                {
                    Name = i.Name,
                    LastModified = i.LastModified,
                    PhysicalPath = i.PhysicalPath,
                    Length = i.Length
                }).ToList()
                    .OrderByDescending(f => f.LastModified).ToPagedList(page, pageSize); 
          
            return View(vm);
        }

        public ActionResult Download(string id)
        {
            var backupPath = _context.SystemSettings.Single(i => i.key == "DATABASE.BACKUP.PATH").Value;
            string filePath = backupPath + id;

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/force-download", id);
        }

        [AllowAnonymous]
        public JsonResult Backup()
        {
            _context.Database.SetCommandTimeout(30000);
            _context.Database.ExecuteSqlCommand("exec BackupDatabase");
            return Json("OK");
        }

        [AllowAnonymous]
        public JsonResult Reports()
        {
            _context.Database.SetCommandTimeout(30000);
            _context.Database.ExecuteSqlCommand("exec SPD_Scheduler_Manual");
            return Json("OK");
        }
    }

}