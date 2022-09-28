using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MCHMIS.Data;
using MCHMIS.Extensions;
using MCHMIS.Interfaces;
using MCHMIS.Models;
using MCHMIS.Services;
using MCHMIS.ViewModels;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using X.PagedList;

namespace MCHMIS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ComplaintsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IUnitOfWork _uow;
        private readonly IDBService _dbService;

        public ComplaintsController(ApplicationDbContext context,
            IHostingEnvironment hostingEnvironment,
            IUnitOfWork uow, IDBService dbService)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _uow = uow;
            _dbService = dbService;
        }

        // GET: Admin/Complaints
        public async Task<IActionResult> Index(ComplaintListViewModel vm)
        {
            var complaints = _context.Complaints
                .Include(c => c.ApprovedBy)
                .Include(c => c.ClosedBy)
                .Include(c => c.Category)
                .Include(c => c.ComplaintType)
                .Include(c => c.CreatedBy)
                .Include(c => c.EscalatedBy)
                .Include(c => c.HealthFacility)
                .Include(c => c.ResolvedBy)
                .Include(c => c.Status)
                .Include(c => c.Village.Ward)
                .OrderByDescending(i => i.DateCreated).AsQueryable();

            var healthFacilityId = _dbService.GetHealthFacilityId();
            if (healthFacilityId != 0)
            {
                bool isGlobal = await _dbService.IsGlobal();
                complaints = complaints.Where(i => i.HealthFacilityId == healthFacilityId || isGlobal);
            }
            if (vm.HealthFacilityId != null)
            {
                complaints = complaints.Where(i => i.HealthFacilityId == vm.HealthFacilityId);
            }
            //if (!string.IsNullOrEmpty(vm.UniqueId))
            //{
            //   complaints = complaints.Where(h => h.Household.UniqueId == vm.UniqueId);
            //}
            //if (!string.IsNullOrEmpty(vm.IdNumber))
            //{
            //   complaints = complaints.Where(h => h.Household.Mother.IdNumber.Contains(vm.IdNumber));
            //}
            if (!string.IsNullOrEmpty(vm.Name))
            {
                complaints = complaints.Where(h =>
                     h.Name.Contains(vm.Name) ||
                     h.Name.Contains(vm.Name) ||
                     h.Name.Contains(vm.Name)

                 );
            }
            if (vm.StatusId != null)
            {
                complaints = complaints.Where(h => h.StatusId == vm.StatusId);
            }
            if (vm.WardId != null)
            {
                complaints = complaints.Where(h => h.Village.WardId == vm.WardId);
            }
            if (vm.SubCountyId != null)
            {
                complaints = complaints.Where(h => h.Village.Ward.SubCountyId == vm.SubCountyId);
            }

            var page = vm.Page ?? 1;
            var pageSize = vm.PageSize ?? 20;

            vm.Complaints = complaints.ToPagedList(page, pageSize);

            ViewData["StatusId"] = new SelectList(_context.ComplaintStatus, "Id", "Name", vm.StatusId);
            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name", vm.SubCountyId);
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities, "Id", "Name", vm.HealthFacilityId);
            vm.Wards = await _context.Wards.ToListAsync();
            return View(vm);
        }

        public IActionResult Create()
        {
            var vm = new ComplaintViewModels();
            vm.ComplaintTypes = _context.ComplaintTypes.ToList();

            ViewData["CategoryId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Complaint Categories"), "Id", "Code");
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities, "Id", "Name");
            ViewData["StatusId"] = new SelectList(_context.ComplaintStatus, "Id", "Name");
            ViewData["WardId"] = new SelectList(_context.Wards, "Id", "Name");
            ViewData["IsComplainantAnonymousId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");
            ViewData["IsComplainantBeneficiaryId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");
            vm.Villages = _context.Villages.ToList();
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ComplaintViewModels vm, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                var complaint = new Complaint();
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, complaint);
                complaint.Name = complaint.Name.ToUpper();
                complaint.Id = Guid.NewGuid().ToString().ToLower();

                if (file != null && file.Length > 0)
                {
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    string path = "";

                    var fileName = DateTime.UtcNow.AddHours(3).ToString("yyyymmddhhmmss") + "-";
                    fileName = fileName + file.FileName;
                    path = webRootPath + "/uploads/complaints/" + Path.GetFileName(fileName);
                    var stream = new FileStream(path, FileMode.Create);
                    await file.CopyToAsync(stream);
                   
                    complaint.Form = fileName;
                }
                complaint.CreatedById = User.GetUserId();
                complaint.DateCreated = DateTime.UtcNow.AddHours(3);
                complaint.StatusId = 1;
                _uow.GetRepository<Complaint>().Add(complaint);
                _uow.Save();
                TempData["Message"] = "Complaint saved successfully";
                return RedirectToAction(nameof(Index));
            }
            vm.ComplaintTypes = _context.ComplaintTypes.ToList();

            ViewData["ComplaintTypeCategoryId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Complaint Categories"), "Id", "Code");
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities, "Id", "Name");
            ViewData["StatusId"] = new SelectList(_context.ComplaintStatus, "Id", "Name");
            ViewData["WardId"] = new SelectList(_context.Wards, "Id", "Name");
            ViewData["IsComplainantAnonymousId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");
            ViewData["IsComplainantBeneficiaryId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");
            vm.Villages = _context.Villages.ToList();
            var errors = ModelState.Select(x => x.Value.Errors)
                .Where(y => y.Count > 0)
                .ToList();
            return View(vm);
        }

        public IActionResult Edit(string id)
        {
            var vm = new ComplaintViewModels();
            var complaint = _context.Complaints.Include(i => i.Village).Single(i => i.Id == id);
            new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(complaint, vm);

            vm.ComplaintTypes = _context.ComplaintTypes.ToList();

            ViewData["CategoryId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Complaint Categories"), "Id", "Code", complaint.CategoryId);
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities, "Id", "Name");
            ViewData["WardId"] = new SelectList(_context.Wards, "Id", "Name");
            ViewData["IsComplainantAnonymousId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code", complaint.IsComplainantAnonymousId);
            ViewData["IsComplainantBeneficiaryId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code", complaint.IsComplainantBeneficiaryId);
            vm.Villages = _context.Villages.ToList();
            vm.WardId = complaint.Village?.WardId;
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ComplaintViewModels vm, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                var complaint = new Complaint();
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, complaint);

                if (file != null && file.Length > 0)
                {
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    string path = "";
                    var fileName = DateTime.UtcNow.AddHours(3).ToString("yyyymmddhhmmss") + "-";
                    fileName = fileName + file.FileName;
                    path = webRootPath + "/uploads/complaints/" + Path.GetFileName(fileName);
                    var stream = new FileStream(path, FileMode.Create);
                    await file.CopyToAsync(stream);
                   
                    complaint.Form = fileName;
                }

                _uow.GetRepository<Complaint>().Update(complaint);
                _uow.Save();
                TempData["Message"] = "Complaint updated successfully";
                return RedirectToAction(nameof(Index));
            }

            vm.ComplaintTypes = _context.ComplaintTypes.ToList();

            ViewData["CategoryId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Complaint Categories"), "Id", "Code", vm.CategoryId);
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities, "Id", "Name");
            ViewData["WardId"] = new SelectList(_context.Wards, "Id", "Name");
            ViewData["IsComplainantAnonymousId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code", vm.IsComplainantAnonymousId);
            ViewData["IsComplainantBeneficiaryId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code", vm.IsComplainantBeneficiaryId);
            vm.Villages = _context.Villages.ToList();
            return View(vm);
        }

        public ActionResult Details(string id)
        {
            var complaint = _context.Complaints
                .Include(c => c.ClosedBy)
                .Include(c => c.Category)
                .Include(c => c.ComplaintType)
                .Include(c => c.CreatedBy)
                .Include(c => c.HealthFacility)
                .Include(c => c.ResolvedBy)
                .Include(c => c.Status)
                .Include(c => c.Village.Ward)
                .Include(c => c.IsComplainantAnonymous)
                .Include(c => c.IsComplainantBeneficiary)
                .Single(i => i.Id == id);
            var vm = new ComplaintDetailsViewModel
            {
                Complaint = complaint,
                ComplaintNotes = _context.ComplaintNotes.Include(i => i.Category).Where(i => i.ComplaintId == id).ToList()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Action(ComplaintDetailsViewModel vm)
        {
            var complaint = _uow.GetRepository<Complaint>()
                .SingleOrDefault(c => c.Id == vm.Id);
            string userId = User.GetUserId();
            //if (ModelState.IsValid)
            //{
            var complaintNote = new ComplaintNote
            {
                DateCreated = DateTime.UtcNow.AddHours(3),
                CreatedById = User.GetUserId(),
                ComplaintId = complaint.Id,
            };
            if (vm.Action.Equals("FollowUp"))
            {
                complaint.StatusId = 2;
                complaintNote.Description = vm.Notes;
                complaintNote.CategoryId = _context.SystemCodeDetails.Single(i => i.Code == "Follow Up").Id;
            }
            else if (vm.Action.Equals("Resolve"))
            {
                complaint.StatusId = 3;
                complaint.ResolvedById = userId;
                complaint.DateResolved = DateTime.UtcNow.AddHours(3);
                complaintNote.Description = vm.Notes;
                complaintNote.CategoryId = _context.SystemCodeDetails.Single(i => i.Code == "Resolution").Id;
            }
            else if (vm.Action.Equals("Close"))
            {
                complaint.StatusId = 4;
                complaint.ClosedById = userId;
                complaint.DateClosed = DateTime.UtcNow.AddHours(3);
                complaintNote.Description = vm.Notes;
                complaintNote.CategoryId = _context.SystemCodeDetails.Single(i => i.Code == "Final").Id;
            }
            if (!string.IsNullOrEmpty(vm.Notes))
            {
                _uow.GetRepository<ComplaintNote>().Add(complaintNote);
            }
            _uow.Save();

            TempData["Message"] = "Complaint updated successfully";
            return RedirectToAction(nameof(Details), new { vm.Id });
        }
    }
}