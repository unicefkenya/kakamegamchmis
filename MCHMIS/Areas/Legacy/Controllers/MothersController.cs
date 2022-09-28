using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using ClosedXML.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MCHMIS.Areas.Legacy.Data;
using MCHMIS.Areas.Legacy.Models;
using MCHMIS.Areas.Legacy.ViewModels;
using MCHMIS.Extensions;
using MCHMIS.Interfaces;
using MCHMIS.Services;
using MCHMIS.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using X.PagedList;
using System.Data.SqlClient;
using MCHMIS.Data;

namespace MCHMIS.Areas.Legacy.Controllers
{
    [Area("Legacy")]
    public class MothersController : Controller
    {
        private readonly LegacyDbContext _context;
        private readonly ApplicationDbContext _context2;
        private readonly IDBService _dbService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public MothersController(LegacyDbContext context, ApplicationDbContext context2, IDBService dbService, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _context2 = context2;
            _dbService = dbService;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Legacy/Mothers
        public async Task<IActionResult> Index(ListViewModel vm)
        {
            var data = _context.Dirty
                .Include(i => i.Mother.Facility)
                .Include(i => i.Status)
                .AsQueryable();
            //// Migrate once
            //var approved = data.Where(i => i.ApprovalStatusId == 5).ToList();
            //var count = 0;
            //foreach (var item in approved)
            //{
            //    Migrate(item.Id);
            //    count++;
            //}
            var healthFacilityId = _dbService.GetHealthFacilityId();
            if (healthFacilityId != 0)
            {
                bool isGlobal = await _dbService.IsGlobal();
                data = data.Where(i => i.Mother.Facility.newfacilityid == healthFacilityId || isGlobal);
            }
            if (!string.IsNullOrEmpty(vm.ReasonValue))
            {
                data = data.Where(i => i.Field == vm.ReasonValue);
            }
            if (vm.StatusId != null)
            {
                data = data.Where(h => h.StatusId == vm.StatusId);
            }
            if (vm.ApprovalStatusId != null)
            {
                data = data.Where(h => h.ApprovalStatusId == vm.ApprovalStatusId);
            }
            if (!string.IsNullOrEmpty(vm.IdNumber))
            {
                data = data.Where(h => h.Mother.M_IDNo == vm.IdNumber);
            }
            if (!string.IsNullOrEmpty(vm.PhoneNumber))
            {
                data = data.Where(h => h.Mother.M_Phoneno == vm.PhoneNumber || h.Mother.F_Phoneno == vm.PhoneNumber);
            }
            if (!string.IsNullOrEmpty(vm.Name))
            {
                data = data.Where(h => h.Mother.M_Names.Contains(vm.Name));
            }
            if (!string.IsNullOrEmpty(vm.Reason))
            {
                data = data.Where(i => i.Reason.Contains(vm.Reason));
                //if(vm.Reason.Contains("Duplicate Phone"))
                data = data.OrderByDescending(i => i.Field);
            }

            vm.Pending = data.Count(i => i.StatusId == 1);
            vm.AwaitingApproval = data.Count(i => i.ApprovalStatusId == 4);
            vm.Approved = data.Count(i => i.ApprovalStatusId == 5);
            vm.Rejected = data.Count(i => i.ApprovalStatusId == 6);
            vm.Migrated = data.Count(i => i.ApprovalStatusId == 7);

            if (vm.Option != null && vm.Option.Equals("export"))
            {
                var exportData = data.Select(i => new LegacyExportViewModel
                {
                    Name = i.Mother.M_Names,
                    IDNumber = i.Mother.M_IDNo,
                    DOB = i.Mother.M_DOB,
                    Ward = i.Mother.M_Ward,
                    PhoneNumber = i.Mother.M_Phoneno,
                    Facility = i.Mother.Facility.names,
                    Reason = i.Reason,
                    ReasonValue = i.Field,
                    Status = i.Status.Name
                });
                var wb = new XLWorkbook();
                // Add all DataTables in the DataSet as a worksheets
                var ds = new DataSet();
                var ws = wb.Worksheets.Add("Enrolment Details");

                var reportTitle = "Legacy Data Export_" + (string.IsNullOrEmpty(vm.Reason) ? "" : "_" + vm.Reason + "_") + DateTime.UtcNow.ToString("yyyy_MM_dd") + ".xlsx";

                ws.Cell(1, 1).InsertTable(exportData.AsEnumerable());
                ws.Columns().AdjustToContents();
                var xlTable = ws.Tables.FirstOrDefault();
                if (xlTable != null) xlTable.ShowAutoFilter = false;
                return wb.Deliver(reportTitle);
            }
            var page = vm.Page ?? 1;
            var pageSize = vm.PageSize ?? 20;
            var statuses = _context.Status;
            vm.DirtyList = data.ToPagedList(page, pageSize);
            var reasons = _context.Reasons.ToList();
            ViewData["Reason"] = new SelectList(reasons, "Name", "Name", vm.Reason);
            ViewData["StatusId"] = new SelectList(statuses.Where(i => i.Id < 4), "Id", "Name", vm.StatusId);
            ViewData["ApprovalStatusId"] = new SelectList(statuses.Where(i => i.Id > 3), "Id", "Name", vm.ApprovalStatusId);
            return View(vm);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var record = await _context.Dirty.Include(i => i.Status).SingleOrDefaultAsync(i => i.Id == id);

            if (record == null)
            {
                return NotFound();
            }
            var mother = await _context.Mother
                .FirstOrDefaultAsync(m => m.Motherid == record.MotherId && m.db == record.db);

            var vm = new LegacyDetailsViewModel();

            var data = _context.Dirty
                .Include(i => i.Mother.Facility)
                .Include(i => i.Status)
                .Where(i => i.Id != id && i.MotherUniqueId == record.MotherUniqueId);
            vm.DirtyList = data;
            vm.Mother = mother;
            vm.NationalId = mother.M_IDNo;
            vm.Exception = record.Reason;
            vm.DOB = mother.M_DOB;
            vm.PhoneNumber = mother.M_Phoneno;
            vm.Notes = record.Notes;
            vm.ApprovalNotes = record.ApprovalNotes;
            vm.Status = record.Status.Name;
            vm.Id = id;
            vm.StatusId = record.StatusId;
            vm.ApprovalStatusId = record.ApprovalStatusId;
            ViewData["StatusId"] = new SelectList(_context.Status.Where(i => i.Id < 4), "Id", "Name", record.StatusId);
            return View(vm);
        }

        public async Task<IActionResult> Update(LegacyDetailsViewModel vm, IFormFile file)
        {
            var record = _context.Dirty.Find(vm.Id);
            if (file != null && file.Length > 0)
            {
                string webRootPath = _hostingEnvironment.WebRootPath;
                string path = "";

                var fileName = DateTime.UtcNow.AddHours(3).ToString("yyyymmddhhmmss") + "-";
                fileName = fileName + file.FileName;
                path = webRootPath + "/uploads/legacy/" + Path.GetFileName(fileName);
                var stream = new FileStream(path, FileMode.Create);
                await file.CopyToAsync(stream);
               
                record.SupportingDocument = fileName;
            }

            record.StatusId = vm.StatusId;
            record.ApprovalStatusId = null;
            record.Notes = vm.Notes;
            record.DateEdited = DateTime.UtcNow.AddHours(3);
            record.EditedById = User.GetUserId();
            var mother = _context.Mother
                .Single(m => m.Motherid == record.MotherId && m.db == record.db);
            mother.M_IDNo = vm.NationalId;
            mother.M_Phoneno = vm.PhoneNumber;
            mother.M_DOB = vm.DOB;
            _context.SaveChanges();
            TempData["Success"] = "Record updated successfully";
            var exceptions = _context.Dirty.Where(i =>
                i.MotherUniqueId == record.MotherUniqueId && i.StatusId == 1).ToList();
            if (exceptions.Any())
            {
                var message = "Record updated but the mother still has the following exception(s)";
                foreach (var item in exceptions)
                {
                    message += "<br />-" + item.Reason;
                }

                TempData["Message"] = message;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var record = await _context.Dirty.Include(i => i.Status).SingleOrDefaultAsync(i => i.Id == id);

            if (record == null)
            {
                return NotFound();
            }
            var mother = await _context.Mother
                .FirstOrDefaultAsync(m => m.Motherid == record.MotherId && m.db == record.db);

            var vm = new LegacyDetailsViewModel();
            vm.Mother = mother;
            vm.NationalId = mother.M_IDNo;
            vm.Exception = record.Reason;
            vm.DOB = mother.M_DOB;
            vm.PhoneNumber = mother.M_Phoneno;
            vm.Notes = record.Notes;
            vm.Status = record.Status.Name;
            vm.Id = id;
            vm.SupportingDocument = record.SupportingDocument;
            vm.StatusId = record.StatusId;
            ViewData["StatusId"] = new SelectList(_context.Status.Where(i => i.Id < 4), "Id", "Name", record.StatusId);
            return View(vm);
        }

        public ActionResult SendForApproval(int[] Ids)
        {
            if (Ids == null || Ids.Length == 0)
            {
                TempData["Info"] = "No records selected.";
                return RedirectToAction("Index");
            }

            foreach (var id in Ids)
            {
                var data = _context.Dirty.Find(id);
                data.ApprovalStatusId = 4; //
            }

            _context.SaveChanges();
            TempData["success"] = Ids.Length + " records sent for approval";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Approvals(int id)
        {
            var record = _context.Dirty.Find(id);

            var data = _context.Dirty
                .Include(i => i.Mother.Facility)
                .Include(i => i.Status)
                .Where(i=>i.Id!=id && i.MotherUniqueId==record.MotherUniqueId);
            if (record == null)
            {
                return NotFound();
            }
            var mother = await _context.Mother
                .FirstOrDefaultAsync(m => m.Motherid == record.MotherId && m.db == record.db);

            var vm = new LegacyDetailsViewModel();
            vm.DirtyList = data;
            vm.Mother = mother;
            vm.NationalId = mother.M_IDNo;
            vm.Exception = record.Reason;
            vm.DOB = mother.M_DOB;
            vm.PhoneNumber = mother.M_Phoneno;
            vm.Notes = record.Notes;
            vm.Id = id;
            vm.StatusId = record.StatusId;

            vm.SupportingDocument = record.SupportingDocument;
            ViewData["StatusId"] = new SelectList(_context.Status.Where(i => i.Id < 4), "Id", "Name", record.StatusId);
            return View(vm);
        }

        [HttpPost]
        public IActionResult Approvals(LegacyApprovalViewModel vm)
        {
            if (ModelState.IsValid)
            {
                vm.StatusId = vm.StatusId == 3 ? 5 : 6;

                var updatingRecord = _context.Dirty.Single(i => i.Id == vm.Id);
                updatingRecord.ApprovalStatusId = vm.StatusId;
                updatingRecord.ApprovalNotes = vm.ApprovalNotes;
                updatingRecord.ApprovedById = User.GetUserId();
                updatingRecord.DateApproved = DateTime.UtcNow.AddHours(3);
                TempData["Message"] =
                    vm.StatusId == 5 ? "Record update approved successfully" : "Record update rejected successfully";
                _context.SaveChanges();
                if (vm.StatusId == 5) // Migrate the record
                {
                    // Check if the mother has another exception
                    var exceptions = _context.Dirty.Where(i =>
                        i.MotherUniqueId == updatingRecord.MotherUniqueId && i.ApprovalStatusId != 5).ToList();
                    if (exceptions.Any(i => i.ApprovalStatusId == 7))
                    {
                        TempData["Info"] = "Record update approved but the mother had been previously migrated";
                        updatingRecord.ApprovalStatusId = 7; // Migrated
                        _context.SaveChanges();
                    }
                    else if (exceptions.Any())
                    {
                        var message = "Record update approved but the mother could not be migrated because she still has the following exception(s)";
                        foreach (var item in exceptions)
                        {
                            message += "<br />-"+item.Reason;
                        }

                        TempData["Message"] = message;
                    }
                    else
                    {
                        _context2.Database.ExecuteSqlCommand(";Exec MigrateBeneficiary @Id,@dirtyId",
                            new SqlParameter("Id", updatingRecord.MotherUniqueId),
                            new SqlParameter("dirtyId", updatingRecord.Id));

                        // Mark all mother records as migrated
                        var records = _context.Dirty.Where(i => i.MotherUniqueId == updatingRecord.MotherUniqueId);
                        foreach (var item in records)
                        {
                            item.ApprovalStatusId = 7; // Migrated
                        }
                        _context.SaveChanges();
                        TempData["Message"] =
                            "Record update approved and data migrated successfully";
                    }
                  
                }
                return RedirectToAction(nameof(Index));
            }

            var id = vm.Id;

            var record = _context.Dirty.Find(id);

            if (record == null)
            {
                return NotFound();
            }
            var mother = _context.Mother
                .First(m => m.Motherid == record.MotherId && m.db == record.db);
            var model = new LegacyDetailsViewModel();
            model.Mother = mother;
            model.NationalId = mother.M_IDNo;
            model.Exception = record.Reason;
            model.DOB = mother.M_DOB;
            model.PhoneNumber = mother.M_Phoneno;
            model.Notes = record.Notes;
            model.Id = id;
            model.StatusId = record.StatusId;
            model.SupportingDocument = record.SupportingDocument;
            ViewData["StatusId"] = new SelectList(_context.Status.Where(i => i.Id < 4), "Id", "Name", record.StatusId);
            return View(model);
        }
        public IActionResult Migrate(int id)
        {
            var updatingRecord = _context.Dirty.Single(i => i.Id == id);
            // Check if the mother has another exception
            var exceptions = _context.Dirty.Where(i =>
                        i.MotherUniqueId == updatingRecord.MotherUniqueId && i.ApprovalStatusId != 5).ToList();
            if (exceptions.Any(i=>i.ApprovalStatusId==7))
            {
                TempData["Info"] = "Record update approved but the mother had been previously migrated";
                updatingRecord.ApprovalStatusId = 7; // Migrated
                _context.SaveChanges();

            }
            else if (exceptions.Any())
            {
                var message = "Record update approved but the mother could not be migrated because she still has the following exception(s)";
                foreach (var item in exceptions)
                {
                    message += "<br />-" + item.Reason;
                }

                TempData["Message"] = message;
            }
            else
            {
                try
                {
                    _context2.Database.ExecuteSqlCommand(";Exec MigrateBeneficiary @Id,@dirtyId",
                        new SqlParameter("Id", updatingRecord.MotherUniqueId),
                        new SqlParameter("dirtyId", updatingRecord.Id)
                        );
                    updatingRecord.ApprovalStatusId = 7; // Migrated
                    _context.SaveChanges();
                    TempData["Message"] =
                        "Record update approved and data migrated successfully";
                }
                catch (Exception ex)
                {
                    TempData["Error"] += updatingRecord.MotherUniqueId+"::::-"+ updatingRecord.Id + "-::::::"+ex.Message+"<br />";
                }
               
            }

            return RedirectToAction(nameof(Approvals), new {id});
        }

        // POST: Legacy/Mothers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var mother = await _context.Mother.FindAsync(id);
            _context.Mother.Remove(mother);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MotherExists(string id)
        {
            return _context.Mother.Any(e => e.Motherid == id);
        }
    }
}