using System;
using System.Data.SqlClient;
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
using System.Globalization;
using ExcelDataReader;
using System.Reflection;
using System.Collections.Generic;
using System.Data;
using ClosedXML.Excel;
using ClosedXML.Extensions;

namespace MCHMIS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ChangesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IUnitOfWork _uow;
        private readonly IDBService _dbService;
        private ISingleRegistryService _singleRegistryService;

        public ChangesController(ApplicationDbContext context,
            IHostingEnvironment hostingEnvironment,
            IUnitOfWork uow
            , ISingleRegistryService singleRegistryService
            , IDBService dbService)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _uow = uow;
            _dbService = dbService;
            _singleRegistryService = singleRegistryService;
        }

        public async Task<IActionResult> Index(ChangesListViewModel vm)
        {
            var changes = _context.Changes
                .OrderByDescending(c => c.DateCreated)
                .Include(c => c.ActionedBy)
                .Include(c => c.ChangeType)
                .Include(c => c.CreatedBy)
                .Include(c => c.Household.Mother)
                .Include(c => c.Household.Village.Ward.SubCounty)
                .Include(c => c.MPESACheckStatus)
                .Include(c => c.Status).AsQueryable();

            vm.AwaitingIPRS = _context.Changes.Count(i => i.RequiresIPRSCheck && i.IPRSVerified == false);
            vm.AwaitingMPesaVerification = _context.Changes.Count(i => i.RequiresMPESACheck && i.MPESACheckStatusId == null && i.StatusId == 3);

            var healthFacilityId = _dbService.GetHealthFacilityId();
            if (healthFacilityId != 0)
            {
                bool isGlobal = await _dbService.IsGlobal();
                changes = changes.Where(i => i.Household.HealthFacilityId == healthFacilityId || isGlobal);
            }
            if (vm.HealthFacilityId != null)
            {
                changes = changes.Where(i => i.Household.HealthFacilityId == vm.HealthFacilityId);
            }
            if (!string.IsNullOrEmpty(vm.UniqueId))
            {
                changes = changes.Where(h => h.Household.UniqueId == vm.UniqueId);
            }
            if (!string.IsNullOrEmpty(vm.IdNumber))
            {
                changes = changes.Where(h => h.Household.Mother.IdNumber.Contains(vm.IdNumber));
            }

            if (!string.IsNullOrEmpty(vm.Name))
            {
                var names = vm.Name.Split(' ');
                foreach (var name in names)
                {
                    changes = changes.Where(h =>
                        h.Household.Mother.FirstName.Contains(name)
                        || h.Household.Mother.MiddleName.Contains(name)
                        || h.Household.Mother.Surname.Contains(name)

                    );
                }
            }
            if (vm.StatusId != null)
            {
                changes = changes.Where(h => h.StatusId == vm.StatusId);
            }
            if (vm.WardId != null)
            {
                changes = changes.Where(h => h.Household.Village.WardId == vm.WardId);
            }
            if (vm.SubCountyId != null)
            {
                changes = changes.Where(h => h.Household.Village.Ward.SubCountyId == vm.SubCountyId);
            }
            if (vm.HealthFacilityId != null)
            {
                changes = changes.Where(h => h.Household.HealthFacilityId == vm.HealthFacilityId);
            }
            if (!string.IsNullOrEmpty(vm.Option) && vm.Option.Equals("mpesa"))
            {
                changes = changes.Where(i => i.RequiresMPESACheck
                                             && i.MPESACheckStatusId == null
                                             // && i.StatusId == 3
                                             );
            }
            if (!string.IsNullOrEmpty(vm.Option) && vm.Option.Equals("iprs"))
            {
                changes = changes.Where(i => i.RequiresIPRSCheck && i.IPRSVerified == false);
            }
            var page = vm.Page ?? 1;
            var pageSize = vm.PageSize ?? 20;

            vm.Changes = changes.ToPagedList(page, pageSize);
            ViewData["StatusId"] = new SelectList(_context.ApprovalStatus, "Id", "Name", vm.StatusId);
            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name", vm.SubCountyId);
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities, "Id", "Name", vm.HealthFacilityId);
            vm.Wards = await _context.Wards.ToListAsync();

            return View(vm);
        }

        public async Task<IActionResult> Search(RegistrationListViewModel vm, string postBack)
        {
            if (!string.IsNullOrEmpty(postBack))
            {
                var households = _context.HouseholdRegs
                    .Include(r => r.Village.Ward.SubCounty)
                    .Include(r => r.Mother)
                    .Include(r => r.Status)
                    .Include(r => r.HealthFacility)
                    .Where(r => r.TypeId == 1).OrderByDescending(r => r.DateCreated)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(vm.UniqueId))
                {
                    households = households.Where(h => h.UniqueId == vm.UniqueId);
                }
                if (!string.IsNullOrEmpty(vm.IdNumber))
                {
                    households = households.Where(h => h.Mother.IdNumber.Contains(vm.IdNumber));
                }
                if (!string.IsNullOrEmpty(vm.Name))
                {
                    households = households.Where(h =>
                        h.Mother.FirstName.Contains(vm.Name)
                        || h.Mother.MiddleName.Contains(vm.Name)
                        || h.Mother.Surname.Contains(vm.Name)
                        || h.Mother.Surname.Contains(vm.Name)
                    );
                }
                if (vm.StatusId != null)
                {
                    households = households.Where(h => h.StatusId == vm.StatusId);
                }
                if (vm.WardId != null)
                {
                    households = households.Where(h => h.Village.WardId == vm.WardId);
                }
                if (vm.SubCountyId != null)
                {
                    households = households.Where(h => h.Village.Ward.SubCountyId == vm.SubCountyId);
                }
                var healthFacilityId = _dbService.GetHealthFacilityId();
                if (healthFacilityId != 0)
                {
                    bool isGlobal = await _dbService.IsGlobal();
                    households = households.Where(i => i.HealthFacilityId == healthFacilityId || isGlobal);
                }
                vm.HouseholdRegs = await households.ToListAsync();
                if (vm.HouseholdRegs.Count == 1)
                {
                    return RedirectToAction("Create", new { id = vm.HouseholdRegs.First().Id });
                }
            }

            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Name", vm.StatusId);

            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name", vm.SubCountyId);
            vm.Wards = await _context.Wards.ToListAsync();
            return View(vm);
        }

        [Route("admin/changes/create/{id}/{changeTypeId?}")]
        public IActionResult Create(string id, int? changeTypeId)
        {
            var vm = new CreateChangeViewModel();
            var hh = _context.HouseholdRegs
                .Include(r => r.Village.Ward.SubCounty)
                .Include(r => r.Mother)
                .Include(r => r.Status)
                .Include(r => r.HealthFacility)
                .Single(i => i.Id == id);
            vm.HouseholdId = id;
            vm.ChangeTypeId = changeTypeId;
            vm.OwnsPhoneId = hh.HasProxy == false ? 1 : 2;
            ViewData["ChangeTypeId"] =
                new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Change Types"), "Id", "Code",
                    changeTypeId);
            ViewData["OwnsPhoneId"] = new SelectList(_context.SystemCodeDetails
                    .Where(i => i.SystemCode.Code == "Boolean Options" && i.OrderNo < 3), "OrderNoNumber", "Code",
                vm.OwnsPhoneId);
            new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(hh, vm);
            vm.Household = hh;
            if (vm.ChangeTypeId == 304) // New or Change of Nominee
            {
                vm.OwnsPhoneId = 2;
            }
            // Check if the mother has similar change pending approval (Approved / Rejected)
            if (_context.Changes.Any(i => i.HouseholdId == id && i.ChangeTypeId == changeTypeId && i.StatusId != 3 && i.StatusId != 4))
            {
                TempData["Info"] = "The selected mother has a similar change request pending approval.";
                return RedirectToAction("Create", new { id, changeTypeId = 0 });
            }

            if (vm.ChangeTypeId == 307) // Death of a Child
            {
                try
                {
                    var children = _context.Children.Where(i => i.Delivery.Pregnancy.StatusId == 1 && i.Delivery.Pregnancy.CaseManagement.HouseholdId == id)
                        .ToList();
                    if (!children.Any())
                    {
                        TempData["Info"] = "Mother delivery information missing.<br />No children found.";
                        return RedirectToAction("Create", new { id, changeTypeId = 0 });
                    }
                    ViewData["ChildId"] = new SelectList(children, "Id", "DisplayName");
                }
                catch (Exception ex)
                {
                    TempData["Info"] = "Mother delivery information missing.<br />No children found.";
                    return RedirectToAction("Create", new { id, changeTypeId = 0 });
                }
            }

            if (vm.ChangeTypeId == 358) // Change of health Facility
            {

                ViewData["DestinationHealthFacilityId"] =
                    new SelectList(_context.HealthFacilities.OrderBy(i=>i.Name).Where(i => i.Id != _context.HouseholdRegs.Single(x=>x.Id==id).HealthFacilityId), "Id", "Name", vm.DestinationHealthFacilityId);
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("admin/changes/create/{id}/{changeTypeId?}")]
        public async Task<IActionResult> Create(string id, int? changeTypeId, CreateChangeViewModel vm, IFormFile file)
        {
            var proceed = true;
            if (vm.ChangeTypeId == 303 || vm.ChangeTypeId == 304) // Change of  Recipient Phone Number or New or Change of Nominee
            {
                if (_context.HouseholdRegs.Any(i => i.Phone == vm.Phone && i.Id != vm.HouseholdId))
                {
                    TempData["Info"] = "Phone number already registered.";
                    proceed = false;
                }
                if (!vm.Phone.StartsWith("07") && !vm.Phone.StartsWith("011"))
                {
                    TempData["Info"] = "Phone number for mother / nominee should start with 07... or 011...";
                    proceed = false;
                }
            }
            if ((vm.ChangeTypeId == 306 || vm.ChangeTypeId == 307) && (vm.DeathDate > vm.NotificationDate))
            {
                TempData["Info"] = "Death Date cannot be earlier than Notification Date";
                proceed = false;
            }
            if (vm.ChangeTypeId == 360)
            {
                var beneficiary = _context.HouseholdRegs.Include(i => i.Status).Single(i => i.Id == vm.HouseholdId);
                if (beneficiary.StatusId != 27)
                {
                    TempData["Info"] = "Beneficiary status is <strong>" + beneficiary.Status.Name + "</strong> and thus cannot be reactivated.<br />Only <strong>Exited</strong> beneficiaries can be reactivated.";
                    proceed = false;
                }
            }
            if (ModelState.IsValid && proceed)
            {
                var change = new Change();
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, change);
                change.Id = Guid.NewGuid().ToString().ToLower();

                if (file != null && file.Length > 0)
                {
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    string path = "";
                    ViewBag.Id = id;

                    var fileName = DateTime.UtcNow.AddHours(3).ToString("yyyymmddhhmmss") + "-";
                    fileName = fileName + file.FileName;
                    path = webRootPath + "/uploads/changes/" + Path.GetFileName(fileName);
                    var stream = new FileStream(path, FileMode.Create);
                    await file.CopyToAsync(stream);
                   
                    change.SupportingDocument = fileName;
                }

                change.CreatedById = User.GetUserId();
                change.DateCreated = DateTime.UtcNow.AddHours(3);
                change.StatusId = 1;

                if (vm.ChangeTypeId == 304) //New or Change of Nominee
                {
                    change.OwnsPhoneId = 2;
                }
                // Change of  Recipient Phone Number or New or Change of Nominee

                if (vm.ChangeTypeId == 303 || vm.ChangeTypeId == 304)
                {
                    change.StatusId = 5;
                }
                if (vm.ChangeTypeId == 328 || vm.ChangeTypeId == 306) //Change of Finger Prints / Death of a Mother
                {
                    change.StatusId = 6; //Awaiting Finger Print Taking
                    change.TakingFingerPrint = true;
                }
                if (vm.ChangeTypeId == 358) // Change of health Facility
                {
                    change.CurrentHealthFacilityId = _context.HouseholdRegs.Single(x => x.Id == id).HealthFacilityId;
                }
                var household = _context.HouseholdRegs.Find(vm.HouseholdId);
                // Change of  Recipient Phone Number
                // New or Change of Nominee
                // Death of a Mother
                if (vm.ChangeTypeId == 303 || vm.ChangeTypeId == 304 || vm.ChangeTypeId == 306)
                {
                    change.RequiresMPESACheck = true;
                    // Suspend beneficiary payments until phone number is approved and verfied
                    var beneficiary = _context.Beneficiaries.SingleOrDefault(i => i.Id == change.HouseholdId);
                    if (beneficiary != null)
                    {
                        beneficiary.StatusId = 25; //Awaiting Validation
                    }

                    if (vm.OwnsPhoneId == 1)
                    {
                        household.RecipientNames = household.RecipientNames;
                    }
                    else
                    {
                        change.NomineeFirstName = change.NomineeFirstName?.ToUpper();
                        if (!string.IsNullOrEmpty(change.NomineeMiddleName))
                        {
                            change.NomineeMiddleName = change.NomineeMiddleName.ToUpper();
                        }
                        change.NomineeSurname = change.NomineeSurname?.ToUpper();

                        household.RecipientNames = vm.NomineeFirstName + " " + vm.NomineeMiddleName + " " + vm.NomineeSurname;
                    }
                }

                // New or Change of Nominee
                // Death of a Mother
                // Change of  Recipient Phone Number and phone does not belong to the mother
                if (vm.ChangeTypeId == 304 || vm.ChangeTypeId == 306 || (vm.ChangeTypeId == 303 && vm.OwnsPhoneId == 2))
                {
                    change.RequiresIPRSCheck = true;
                    var iprsVm = await IPRS(vm);
                    change.IPRSVerified = iprsVm.IPRSED;
                    change.IPRSPassed = iprsVm.IPRSMatched;
                    change.IPRSExceptionId = iprsVm.IPRSExceptionId;
                }

                _uow.GetRepository<Change>().Add(change);
                _uow.Save();
                TempData["Message"] = "Update saved successfully.";
                // Change of  Recipient Phone Number or New or Change of Nominee
                //Awaiting Fingerprint Verification
                if (change.StatusId == 5 || change.StatusId == 6)
                {
                    var sql = @"UPDATE FingerPrintVerifications SET Verified=0,IsVerifying=1 where HouseholdId= @householdId and Id=@id";
                    _context.Database.ExecuteSqlCommand(sql,
                        new SqlParameter("@householdId", change.HouseholdId),
                        new SqlParameter("@id", id));

                    TempData["Message"] = "Update saved successfully. Verify fingerprints now.";
                    return RedirectToAction(nameof(FingerPrint), new { change.Id });
                }
                return RedirectToAction(nameof(Index));
            }
            var hh = _context.HouseholdRegs
                .Include(r => r.Village.Ward.SubCounty)
                .Include(r => r.Mother)
                .Include(r => r.Status)
                .Include(r => r.HealthFacility)
                .Single(i => i.Id == id);
            vm.HouseholdId = id;
            vm.ChangeTypeId = changeTypeId;

            ViewData["ChangeTypeId"] =
                new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Change Types"), "Id", "Code",
                    changeTypeId);
            ViewData["OwnsPhoneId"] = new SelectList(_context.SystemCodeDetails
                    .Where(i => i.SystemCode.Code == "Boolean Options" && i.OrderNo < 3), "OrderNoNumber", "Code",
                vm.OwnsPhoneId);
            new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(hh, vm);
            vm.Household = hh;
            if (vm.ChangeTypeId == 304)
            {
                vm.OwnsPhoneId = 2;
            }
            if (vm.ChangeTypeId == 307)
            {
                var activeDelivery =
                    _context.Deliveries.SingleOrDefault(i => i.Pregnancy.CaseManagement.HouseholdId == id && i.Pregnancy.StatusId == 1);
                ViewData["ChildId"] = new SelectList(_context.Children.Where(i => i.DeliveryId == activeDelivery.Id),
                    "Id", "DisplayName");
            }

            if (vm.ChangeTypeId == 358) // Change of health Facility
            {

                ViewData["DestinationHealthFacilityId"] =
                    new SelectList(_context.HealthFacilities.OrderBy(i => i.Name).Where(i => i.Id != _context.HouseholdRegs.Single(x => x.Id == id).HealthFacilityId), "Id", "Name", vm.DestinationHealthFacilityId);
            }

            return View(vm);
        }

        [Route("admin/changes/edit/{id}/{changeTypeId?}")]
        public async Task<IActionResult> Edit(string id, int? changeTypeId)
        {
            CreateChangeViewModel vm = new CreateChangeViewModel();
            var change = _context.Changes.Find(id);

            var hh = _context.HouseholdRegs
                .Include(r => r.Village.Ward.SubCounty)
                .Include(r => r.Mother)
                .Include(r => r.Status)
                .Include(r => r.HealthFacility)
                .Single(i => i.Id == change.HouseholdId);
            vm.HouseholdId = change.HouseholdId;
            vm.OwnsPhoneId = hh.HasProxy == false ? 1 : 2;
            ViewData["ChangeTypeId"] =
                new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Change Types"), "Id", "Code",
                    change.ChangeTypeId);
            ViewData["OwnsPhoneId"] = new SelectList(_context.SystemCodeDetails
                    .Where(i => i.SystemCode.Code == "Boolean Options" && i.OrderNo < 3), "OrderNoNumber", "Code",
                vm.OwnsPhoneId);
            new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(hh, vm);
            new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(change, vm);

            vm.Household = hh;
            vm.HouseholdId = hh.Id;
            vm.ChangeTypeId = changeTypeId ?? vm.ChangeTypeId;
            // vm.NomineeFirstName = vm.NomineeMiddleName = vm.NomineeSurname = vm.NomineeIdNumber = "";
            if (vm.ChangeTypeId == 304)
            {
                vm.OwnsPhoneId = 2;
            }
            if (vm.ChangeTypeId == 307)
            {
                try
                {
                    var children = _context.Children.Where(i => i.Delivery.Pregnancy.CaseManagement.HouseholdId == change.HouseholdId && i.Delivery.Pregnancy.StatusId == 1).ToList();
                    ViewData["ChildId"] = new SelectList(children, "Id", "DisplayName");
                }
                catch (Exception ex)
                {
                    TempData["Info"] = "Mother delivery information missing.<br />No child found.";
                    return RedirectToAction("Edit", new { id = vm.Id, changeTypeId = 0 });
                }
            }
            if (change.ChangeTypeId == 358) // Change of health Facility
            {

                ViewData["DestinationHealthFacilityId"] =
                    new SelectList(_context.HealthFacilities.OrderBy(i => i.Name).Where(i => i.Id != change.CurrentHealthFacilityId), "Id", "Name", change.DestinationHealthFacilityId);
            }
            return View(vm);
        }

        [HttpPost]
        [Route("admin/changes/edit/{id}/{changeTypeId?}")]
        public async Task<IActionResult> Edit(CreateChangeViewModel vm, IFormFile file)
        {
            var proceed = true;
            var change = _context.Changes.Find(vm.Id);
            if (vm.ChangeTypeId == 303 || vm.ChangeTypeId == 304)
            {
                if (_context.HouseholdRegs.Any(i => i.Phone == vm.Phone && i.Id != vm.HouseholdId))
                {
                    TempData["Info"] = "Phone number already registered.";
                    proceed = false;
                }
                if (!vm.Phone.StartsWith("07") && !vm.Phone.StartsWith("011"))
                {
                    TempData["Info"] = "Phone number for mother / nominee should start with 07... or 011...";
                    proceed = false;
                }
            }
            if ((vm.ChangeTypeId == 306 || vm.ChangeTypeId == 307) && (vm.DeathDate > vm.NotificationDate))
            {
                TempData["Info"] = "Death Date cannot be earlier than Notification Date";
                proceed = false;
            }
            if (vm.ChangeTypeId == 360)
            {
                var beneficiary = _context.HouseholdRegs.Include(i => i.Status).Single(i => i.Id == vm.HouseholdId);
                if (beneficiary.StatusId != 27)
                {
                    TempData["Info"] = "Beneficiary status is <strong>" + beneficiary.Status.Name + "</strong> and thus cannot be reactivated.<br />Only <strong>Exited</strong> beneficiaries can be reactivated.";
                    proceed = false;
                }
            }
            if (ModelState.IsValid && proceed)
            {
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, change);
                if (file != null && file.Length > 0)
                {
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    string path = "";

                    var fileName = DateTime.UtcNow.AddHours(3).ToString("yyyymmddhhmmss") + "-";
                    fileName = fileName + file.FileName;
                    path = webRootPath + "/uploads/changes/" + Path.GetFileName(fileName);
                    var stream = new FileStream(path, FileMode.Create);
                    await file.CopyToAsync(stream);
                   
                    change.SupportingDocument = fileName;
                }

                if (vm.ChangeTypeId == 304) //New or Change of Nominee
                {
                    change.OwnsPhoneId = 2;
                }
                if (vm.ChangeTypeId == 303 || vm.ChangeTypeId == 304)
                {
                    change.StatusId = 5;
                    change.FingerPrintVerified = false;
                }
                if (vm.ChangeTypeId == 328) //Change of Finger Prints
                {
                    change.StatusId = 6; //Awaiting Finger Print Taking
                    change.TakingFingerPrint = true;
                }

                var household = _context.HouseholdRegs.Find(vm.HouseholdId);
                // Change of  Recipient Phone Number
                // New or Change of Nominee
                // Death of a Mother
                if (vm.ChangeTypeId == 303 || vm.ChangeTypeId == 304 || vm.ChangeTypeId == 306)
                {
                    change.RequiresMPESACheck = true;
                    // Suspend beneficiary payments until phone number is approved and verfied
                    var beneficiary = _context.Beneficiaries.SingleOrDefault(i => i.Id == change.HouseholdId);
                    if (beneficiary != null)
                    {
                        beneficiary.StatusId = 25; //Awaiting Validation
                    }
                }

                // New or Change of Nominee
                // Death of a Mother
                // Change of  Recipient Phone Number and phone does not belong to the mother
                if (vm.ChangeTypeId == 304 || vm.ChangeTypeId == 306 || (vm.ChangeTypeId == 303 && vm.OwnsPhoneId == 2))
                {
                    change.RequiresIPRSCheck = true;

                    var iprsVm = await IPRS(vm);
                    change.IPRSVerified = iprsVm.IPRSED;
                    change.IPRSPassed = iprsVm.IPRSMatched;
                }
                _uow.GetRepository<Change>().Update(change);
                _uow.Save();
                TempData["Message"] = "Update saved successfully.";
                if (change.StatusId == 5 || change.StatusId == 6)
                {
                    var sql = @"UPDATE FingerPrintVerifications SET Verified=0,IsVerifying=1 where HouseholdId= @householdId and Id=@id";
                    _context.Database.ExecuteSqlCommand(sql,
                        new SqlParameter("@householdId", change.HouseholdId),
                        new SqlParameter("@id", change.Id));

                    TempData["Message"] = "Update saved successfully. " + (change.StatusId == 5 ? "Verify fingerprints now." : "Take Mother's <strong>Left Index Finger</strong> fingerprints now.");
                    return RedirectToAction(nameof(FingerPrint), new { change.Id });
                }
                if (vm.ChangeTypeId == 358) // Change of health Facility
                {

                    ViewData["DestinationHealthFacilityId"] =
                        new SelectList(_context.HealthFacilities.OrderBy(i => i.Name).Where(i => i.Id != change.CurrentHealthFacilityId), "Id", "Name", change.DestinationHealthFacilityId);
                }
                return RedirectToAction(nameof(Index));
            }

            var hh = _context.HouseholdRegs
                .Include(r => r.Village.Ward.SubCounty)
                .Include(r => r.Mother)
                .Include(r => r.Status)
                .Include(r => r.HealthFacility)
                .Single(i => i.Id == change.HouseholdId);

            vm.OwnsPhoneId = hh.HasProxy == false ? 1 : 2;
            ViewData["ChangeTypeId"] =
                new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Change Types"), "Id", "Code",
                    vm.ChangeTypeId);
            ViewData["OwnsPhoneId"] = new SelectList(_context.SystemCodeDetails
                    .Where(i => i.SystemCode.Code == "Boolean Options" && i.OrderNo < 3), "OrderNoNumber", "Code",
                vm.OwnsPhoneId);
            new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(hh, vm);
            vm.Household = hh;
            if (vm.ChangeTypeId == 304)
            {
                vm.OwnsPhoneId = 2;
            }
            if (vm.ChangeTypeId == 307)
            {
                var activeDelivery =
                    _context.Deliveries.SingleOrDefault(i => i.Pregnancy.CaseManagement.HouseholdId == vm.Id && i.Pregnancy.StatusId == 1);
                ViewData["ChildId"] = new SelectList(_context.Children.Where(i => i.DeliveryId == activeDelivery.Id),
                    "Id", "DisplayName");
            }
            return View(vm);
        }

        public IActionResult Details(string id)
        {
            var vm = new ChangeDetailsViewModel();

            var change = _context.Changes
                .OrderByDescending(c => c.DateCreated)
                .Include(c => c.ActionedBy)
                .Include(c => c.ChangeType)
                .Include(c => c.CreatedBy)
                .Include(c => c.CurrentHealthFacility)
                .Include(c => c.DestinationHealthFacility)
                .Include(c => c.Household)
                .Include(c => c.Status)
                .Include(c => c.Child)
                .Single(i => i.Id == id);
            var hh = _context.HouseholdRegs
                .Include(r => r.Village.Ward.SubCounty)
                .Include(r => r.Mother)
                .Include(r => r.Status)
                .Include(r => r.HealthFacility)
                .Single(i => i.Id == change.HouseholdId);
            vm.Household = hh;
            vm.Change = change;
            return View(vm);
        }

        public IActionResult Approvals(string id)
        {
            var vm = new ChangeDetailsViewModel();

            var change = _context.Changes
                .OrderByDescending(c => c.DateCreated)
                .Include(c => c.ActionedBy)
                .Include(c => c.ChangeType)
                .Include(c => c.CreatedBy)
                .Include(c => c.Household)
                .Include(c => c.Status)
                .Include(c => c.DestinationHealthFacility)
                .Include(c => c.CurrentHealthFacility)
                .Include(c => c.Child)
                .Single(i => i.Id == id);
            var hh = _context.HouseholdRegs
                .Include(r => r.Village.Ward.SubCounty)
                .Include(r => r.Mother)
                .Include(r => r.Status)
                .Include(r => r.HealthFacility)
                .Single(i => i.Id == change.HouseholdId);
            vm.Household = hh;
            vm.Change = change;
            return View(vm);
        }

        [HttpPost]
        public IActionResult Approvals(ApprovalViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var change = _context.Changes.Include(i => i.ChangeType).Single(i => i.Id == vm.Id);
                change.StatusId = vm.StatusId;
                change.ApprovalNotes = vm.Notes;
                change.ActionedById = User.GetUserId();
                change.DateActioned = DateTime.UtcNow.AddHours(3);
                TempData["Message"] = vm.StatusId == 3 ? "Update approved successfully" : "Update rejected successfully";

                _uow.GetRepository<Change>().Update(change);
                var household = _context.HouseholdRegs.Include(i => i.Mother).Single(i => i.Id == change.HouseholdId);
                if (vm.StatusId == 3) // Effect changes
                {
                    if (change.ChangeType.Code.Equals("Change of Recipient Phone Number"))
                    {
                        if (household.StatusId == 15) //Enrolment in Progress - The phone number had an issue
                        {
                            household.RequiresMPESACheck = true;
                        }

                        _uow.GetRepository<HouseholdReg>().Update(household);
                    }
                    else if (change.ChangeType.Code.Equals("New or Change of Nominee") || change.ChangeType.Code.Equals("Death of a Mother"))
                    {
                        // Numbers should be update after Mpesa Validation
                        //household.Phone = change.Phone;
                        //household.RecipientNames = change.NomineeFirstName + " " + change.NomineeMiddleName + " " + change.NomineeSurname;
                        //household.HasProxy = true;
                        //household.NomineeFirstName = change.NomineeFirstName;
                        //household.NomineeMiddleName = change.NomineeMiddleName;
                        //household.NomineeSurname = change.NomineeSurname;
                        //household.NomineeIdNumber = change.NomineeIdNumber;
                        household.IPRSVerified = change.IPRSVerified != null && (bool)change.IPRSVerified;
                        household.IPRSPassed = change.IPRSPassed;

                        if (household.StatusId == 15 && change.ChangeType.Code.Equals("New or Change of Nominee")) //Enrolment in Progress - The phone number had an issue
                        {
                            household.RequiresMPESACheck = true;
                        }
                        _uow.GetRepository<HouseholdReg>().Update(household);
                    }
                    else if (change.ChangeType.Code.Equals("Change of Next of Kin"))
                    {
                        household.NOKFirstName = change.NOKFirstName;
                        household.NOKMiddleName = change.NOKMiddleName;
                        household.NOKSurname = change.NOKSurname;
                        household.NOKIdNumber = change.NOKIdNumber;
                        _uow.GetRepository<HouseholdReg>().Update(household);
                    }
                    else if (change.ChangeType.Code.Equals("Death of a Mother"))
                    {
                        household.StatusId = 21;
                    }
                    else if (change.ChangeType.Code.Equals("Change of Health Facility"))
                    {
                        household.HealthFacilityId = (int)change.DestinationHealthFacilityId;
                        var beneficiary = _context.Beneficiaries.SingleOrDefault(i => i.HouseholdId == household.Id);
                        if (beneficiary != null)
                            beneficiary.HealthFacilityId = household.HealthFacilityId;
                    }
                    else if (change.ChangeTypeId.Equals(360)) //Reactivate Beneficiary
                    {
                        household.StatusId = 19; // Enrolled
                    }
                    else if (change.ChangeType.Code.Equals("Death of a Child"))
                    {
                        var child = _context.Children.Find(change.ChildId);
                        child.StatusId = 311; //Deceased
                        _uow.GetRepository<Child>().Update(child);

                        // If only child, add benevolent payments
                        //if (_context.Children.Count(i => i.DeliveryId == child.DeliveryId) == 1)
                        //{
                        //    // get active case
                        //    var activeCase = _context.CaseManagement.First(i =>
                        //        i.HouseholdId == change.HouseholdId && i.StatusId == 1);
                        //    // Check if the household has ever been paid
                        //    if (!_context.PaymentTransactions.Any(
                        //            i => i.Beneficiary.HouseholdId == household.Id && i.PaymentPointId == 7) && !_context.BeneficiaryPaymentPoints.Any(i => i.HouseholdId == household.Id && i.PaymentPointId == 7))
                        //    {
                        //        var beneficiaryPaymentPoint = new BeneficiaryPaymentPoint
                        //        {
                        //            HouseholdId = household.Id,
                        //            PaymentPointId = 7,
                        //            CreatedById = User.GetUserId(),
                        //            DateCreated =DateTime.UtcNow.AddHours(3),
                        //            StatusId = 1,
                        //            CaseManagementId = activeCase.Id
                        //        };
                        //        _uow.GetRepository<BeneficiaryPaymentPoint>().Add(beneficiaryPaymentPoint);

                        //        beneficiaryPaymentPoint = new BeneficiaryPaymentPoint
                        //        {
                        //            HouseholdId = household.Id,
                        //            PaymentPointId = 8,
                        //            CreatedById = User.GetUserId(),
                        //            DateCreated =DateTime.UtcNow.AddHours(3),
                        //            StatusId = 1,
                        //            CaseManagementId = activeCase.Id
                        //        };
                        //        _uow.GetRepository<BeneficiaryPaymentPoint>().Add(beneficiaryPaymentPoint);

                        //        // Exit the Mother from the Program
                        //        activeCase.StatusId = 2;
                        //        activeCase.ReasonId = 8;//Death of the Child
                        //        activeCase.DateExited =DateTime.UtcNow.AddHours(3);
                        //        _uow.GetRepository<CaseManagement>().Update(activeCase);
                        //    }
                        //}
                    }
                    else if (change.ChangeType.Code.Equals("Change of Finger Prints"))
                    {
                        household.FingerPrint = change.FingerPrint;
                        household.RawFingerPrint = change.RawFingerPrint;
                    }
                }

                _uow.Save();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Approvals), new { vm.Id });
        }

        public ActionResult SendForApproval(string[] Ids)
        {
            if (Ids == null || Ids.Length == 0)
            {
                TempData["Info"] = "No Updates selected.";
                return RedirectToAction("Index");
            }
            var vm = new SendForApprovalViewModel();
            vm.Count = Ids.Count();

            vm.Ids = Ids;

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendForApprovalSave(string[] Ids)
        {
            foreach (var id in Ids)
            {
                var obj = _context.Changes.Find(id);
                obj.StatusId = 2;
            }
            await _context.SaveChangesAsync();
            var count = Ids.Count();

            TempData["Success"] = count + " update" + (count > 1 ? "s" : "") + " sent for approval";

            return RedirectToAction("Index");
        }

        public IActionResult FingerPrint(string id)
        {
            var userId = User.GetUserId();
            ViewBag.Id = id;
            var change = _context.Changes.Find(id);
            var householdId = change.HouseholdId;
            var household = _context.HouseholdRegs.Find(householdId);
            var healthFacilityId = household.HealthFacilityId;
            if (change.StatusId == 5)
            {
                var date = DateTime.UtcNow.AddHours(3);

                var sql = @"UPDATE FingerPrintVerifications SET IsVerifying=0 where HealthFacilityId= @healthFacilityId and Id<>@id";
                var rowsAffected = _context.Database.ExecuteSqlCommand(sql,
                    new SqlParameter("@healthFacilityId", healthFacilityId),
                    new SqlParameter("@id", id)
                );
                var verification = _context.FingerPrintVerifications
                    .SingleOrDefault(i => i.Id == id && i.HouseholdId == change.HouseholdId);

                if (verification == null)
                {
                    verification = new FingerPrintVerification
                    {
                        Id = id,
                        HealthFacilityId = healthFacilityId,
                        CreatedById = userId,
                        DateCreated = date,
                        HouseholdId = change.HouseholdId,
                        IsVerifying = true
                    };
                    if (_context.FingerPrintVerifications.Any(i => i.Id == id && i.HouseholdId == change.HouseholdId))
                    {
                        _context.FingerPrintVerifications.Update(verification);
                    }
                    else
                    {
                        _context.FingerPrintVerifications.Add(verification);
                    }

                    _context.SaveChanges();
                    ViewBag.Title = "Verify fingerprint for " + _context.HouseholdRegs
                                        .Include(i => i.Mother).Single(i => i.Id == householdId).Mother.FullName;
                    ViewBag.Url = "FingerPrint/" + id;
                }
                else if (verification.Verified == false)
                {
                    verification.HealthFacilityId = healthFacilityId;
                    verification.CreatedById = userId;
                    if (!verification.IsVerifying)
                    {
                        verification.IsVerifying = true;
                        verification.Verified = false;
                    }
                    _context.SaveChanges();
                    ViewBag.Title = "Verify fingerprint for " + _context.HouseholdRegs
                                        .Include(i => i.Mother).Single(i => i.Id == householdId).Mother.FullName;
                    ViewBag.Url = "FingerPrint/" + id;
                }
                else
                {
                    change.FingerPrintVerified = true;
                    change.StatusId = 1;
                    TempData["Info"] = "Fingerprint Verified.";
                    _uow.Save();
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                var sql = @"UPDATE T1 SET TakingFingerPrint=0 from Changes T1 INNER JOIN HouseholdRegs T2 ON T2.Id=T1.HouseholdId where T2.HealthFacilityId= @healthFacilityId and T1.Id<>@id";
                _context.Database.ExecuteSqlCommand(sql,
                    new SqlParameter("@healthFacilityId", healthFacilityId),
                    new SqlParameter("@id", id));

                if (change.FingerPrintVerified == false)
                {
                    ViewBag.Title = "Taking fingerprint for " + _context.HouseholdRegs
                                        .Include(i => i.Mother).Single(i => i.Id == householdId).Mother.FullName;
                    ViewBag.Url = "FingerPrint/" + id;
                }
                else
                {
                    change.StatusId = 1;
                    TempData["Info"] = "Fingerprint Taken.";
                    _uow.Save();
                    return RedirectToAction(nameof(Index));
                }
            }

            _uow.Save();

            return View();
        }

        public async Task<IPRSViewModel> IPRS(CreateChangeViewModel vm)
        {
            var model = new IPRSViewModel();
            try
            {
                // Do IPRS Check
                var login = new LoginVm
                {
                    Password = _context.SystemSettings.Single(i => i.key == "SR.PASSWORD").Value,
                    UserName = _context.SystemSettings.Single(i => i.key == "SR.USERNAME").Value
                };
                var auth = await _singleRegistryService.Login(login);
                var change = _context.Changes.Single(i => i.Id == vm.Id);
                if (auth.TokenAuth != null)
                {
                    string IdNumber = "", firstName = "", middleName = "", surname = "";
                    IdNumber = vm.NomineeIdNumber;
                    firstName = vm.NOKFirstName;
                    middleName = vm.NOKMiddleName;
                    surname = vm.NOKSurname;

                    var hhd = new VerificationSrPostVm
                    {
                        TokenCode = auth.TokenAuth,
                        IDNumber = IdNumber,
                        Names = ""
                    };
                    DateTime date1;
                    var hhdIprs = await _singleRegistryService.IprsVerification(hhd);
                    if (!string.IsNullOrEmpty(hhdIprs.ID_Number))
                    {
                        model.IPRSED = true;

                        if (hhdIprs.First_Name.Equals(firstName,
                                    StringComparison.CurrentCultureIgnoreCase)
                                && hhdIprs.Middle_Name.Equals(middleName,
                                    StringComparison.CurrentCultureIgnoreCase) &&
                                hhdIprs.Surname.Equals(surname,
                                    StringComparison.CurrentCultureIgnoreCase))
                        {
                            model.IPRSMatched = true;
                        }
                        else // Names Not Matched
                        {
                            model.IPRSExceptionId = 333;
                        }
                    }
                    else
                    {
                        model.IPRSExceptionId = 335; // Invalid National Id
                    }
                }
            }
            catch (Exception ex)
            {
                model.IPRSED = false;
            }

            return model;
        }

        public IActionResult IPRSCheck()
        {
            ViewBag.Count = _context.Changes.Count(i => i.RequiresIPRSCheck && i.IPRSVerified == false);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IPRSCheck(int Id)
        {
            int matched = 0;
            int failed = 0;
            var changes = _context.Changes.Where(i => i.RequiresIPRSCheck && i.IPRSVerified == false);

            // Do IPRS Check
            try
            {
                var login = new LoginVm
                {
                    Password = _context.SystemSettings.Single(i => i.key == "SR.PASSWORD").Value,
                    UserName = _context.SystemSettings.Single(i => i.key == "SR.USERNAME").Value
                };
                var auth = await _singleRegistryService.Login(login);

                if (auth.TokenAuth != null)
                {
                    foreach (var change in changes)
                    {
                        string IdNumber = "", firstName = "", middleName = "", surname = "";

                        IdNumber = change.NomineeIdNumber;
                        firstName = change.NOKFirstName;
                        middleName = change.NOKMiddleName;
                        surname = change.NOKSurname;

                        var hhd = new VerificationSrPostVm
                        {
                            TokenCode = auth.TokenAuth,
                            IDNumber = IdNumber,
                            Names = ""
                        };
                        DateTime date1;
                        var hhdIprs = await _singleRegistryService.IprsVerification(hhd);
                        if (!string.IsNullOrEmpty(hhdIprs.ID_Number))
                        {
                            change.IPRSVerified = true;

                            var firstnamesMatched =
                                hhdIprs.First_Name.Trim().ToLower() == firstName.Trim().ToLower();

                            var middleNamesMatched =
                                hhdIprs.Middle_Name.ToLower().Trim() == middleName.ToLower().Trim();

                            var surnameMatched = hhdIprs.Surname.Trim().ToLower() == surname.Trim().ToLower();
                            if (firstnamesMatched &&
                                middleNamesMatched && surnameMatched)
                            {
                                change.IPRSPassed = true;
                                matched++;
                            }
                            else // Names Not Matched
                            {
                                change.IPRSExceptionId = 333;
                                failed++;
                            }
                        }
                        else
                        {
                            failed++;
                            change.IPRSExceptionId = 335; // Invalid National Id
                        }
                    }

                    _context.SaveChanges();
                }

                TempData["Info"] = "Service completed successfully.<br />Matched: " + matched + "<br />Failed: " +
                                   failed;
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Sorry, seems IPRS service is offline. Try again later.";
            }

            return RedirectToAction("Index");
        }

        public class IPRSViewModel
        {
            public bool IPRSED { get; set; }
            public bool IPRSMatched { get; set; }
            public int IPRSExceptionId { get; set; }
        }

        public IActionResult Import(int id)
        {
            var vm = new EnrolmentListImportViewModel();
            vm.Id = id;
            return View(vm);
        }

        public async Task<IActionResult> Export()
        {
            var details = _context.Changes.Where(i => i.RequiresMPESACheck && i.MPESACheckStatusId == null && i.StatusId == 3);
            var data = details.Select(b => new EnrolmentListExportViewModel
            {
                CreditIdentityStringType = "MSISDN",
                CreditIdentityString = "254" + b.Phone.Substring(b.Phone.Length - 9, 9),
                Comment = "Updates"
            }).ToList();
            // exportService.ExportToExcel(data, reportTitle);
            var wb = new XLWorkbook();

            // Add all DataTables in the DataSet as a worksheets
            var ds = new DataSet();
            var ws = wb.Worksheets.Add("Enrolment Details");
            var reportTitle = "Updates List _" + DateTime.UtcNow.AddHours(3).ToString("yyyymmddhhmmss") + ".xlsx";
            ws.Cell(1, 1).InsertTable(data.AsEnumerable());
            ws.Columns().AdjustToContents();
            var xlTable = ws.Tables.FirstOrDefault();
            if (xlTable != null) xlTable.ShowAutoFilter = false;
            return wb.Deliver(reportTitle);
        }

        [HttpPost]
        public async Task<IActionResult> Import(int id, IFormFile uploadfile)
        {
            var model = new EnrolmentListImportViewModel();
            model.Id = id;
            if (uploadfile != null && uploadfile.Length > 0)
            {
                string webRootPath = _hostingEnvironment.WebRootPath;
                string path = "";
                ViewBag.Id = id;

                var fileName = DateTime.UtcNow.AddHours(3).ToString("yyyymmddhhmmss") + "-";
                fileName = fileName + uploadfile.FileName;
                path = webRootPath + "/uploads/enrolment/" + Path.GetFileName(fileName);

                //using (var stream = new FileStream(path, FileMode.Create))
                //{
                //    await file.CopyToAsync(stream);
                //}

                var stream = new FileStream(path, FileMode.Create);
                 await uploadfile.CopyToAsync(stream); 

                //ExcelDataReader works on binary excel file
                // Stream stream = uploadfile.InputStream;
                //We need to written the Interface.
                IExcelDataReader reader = null;
                if (uploadfile.FileName.EndsWith(".xls"))
                {
                    //reads the excel file with .xls extension
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (uploadfile.FileName.EndsWith(".xlsx"))
                {
                    //reads excel file with .xlsx extension
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                else if (uploadfile.FileName.EndsWith(".csv"))
                {
                    //reads excel file with .xlsx extension
                    reader = ExcelReaderFactory.CreateCsvReader(stream);
                }
                else
                {
                    //Shows error if uploaded file is not Excel file
                    // ModelState.AddModelError("File", "This file format is not supported");
                    TempData["Info"] = "Please upload an Excel or CSV document";

                    return View(model);
                }

                DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });

                reader.Close(); stream.Close();
               
                ViewBag.UploadedFilePath = path;
                ViewBag.FileName = fileName;

                EnrolmentListImportViewModel vm = new EnrolmentListImportViewModel();
                var tableData = result.Tables[0];

                var recordNo = tableData.Rows[11][0].ToString().ToLower();
                var amount = tableData.Rows[11][1].ToString().ToLower();
                if (!recordNo.Equals("record no") || !amount.Equals("amount"))
                {
                    TempData["Info"] = "Please upload the Excel document as downloaded from Safaricom.";
                    return View(model);
                }
                var firstPhoneNumber = tableData.Rows[12][3].ToString().ToLower();
                if (firstPhoneNumber.Length != 12)
                {
                    TempData["Info"] = "Please remove formatting of the Credit Identity String column.";
                    return View(model);
                }
                DataTable dt = tableData.Clone();
                var count = 0;
                foreach (DataRow dr in tableData.Rows)
                {
                    dt.Rows.Add(dr.ItemArray);
                    count++;
                    if (count == 20)
                        break;
                }

                // dt = ToDataTable(tableData.Select().AsEnumerable().Take(1).ToList());
                vm.Table = dt;
                vm.Rows = tableData.Rows.Count - 12;
                vm.Id = id;
                ViewBag.Id = id;
                return View(vm);
            }

            ModelState.AddModelError("File", "Please upload your file");
            return View(model);
        }

        public async Task<IActionResult> ImportSave(int id, string filepath, string filename)
        {
            FileStream stream = System.IO.File.Open(filepath, FileMode.Open, FileAccess.Read);
            IExcelDataReader reader = null;
            if (filepath.EndsWith(".xls"))
            {
                //reads the excel file with .xls extension
                reader = ExcelReaderFactory.CreateBinaryReader(stream);
            }
            else if (filepath.EndsWith(".xlsx"))
            {
                //reads excel file with .xlsx extension
                reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            }
            else if (filepath.EndsWith(".csv"))
            {
                //reads excel file with .xlsx extension
                reader = ExcelReaderFactory.CreateCsvReader(stream);
            }
            DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true
                }
            });
            var table = result.Tables[0];
            for (int i = table.Rows.Count - 1; i >= 0; i--)
            {
                DataRow dr = table.Rows[i];
                if (string.IsNullOrEmpty(dr["Column3"].ToString()))
                    dr.Delete();
            }
            table.AcceptChanges();
            await _context.Database.ExecuteSqlCommandAsync("Delete from TempTable;DBCC CHECKIDENT ('TempTable',Reseed,0);  ");
            using (var bulkCopy = new SqlBulkCopy(_context.Database.GetDbConnection().ConnectionString, SqlBulkCopyOptions.KeepIdentity))
            {
                bulkCopy.ColumnMappings.Add(3, "Col1");
                bulkCopy.ColumnMappings.Add(8, "Col2");
                bulkCopy.ColumnMappings.Add(9, "Col3");

                bulkCopy.BulkCopyTimeout = 600;
                bulkCopy.DestinationTableName = "TempTable";
                bulkCopy.WriteToServer(table);
            }
            // Update the records
            var rowsAffected = _context.Database.ExecuteSqlCommand("exec EnrolmentProcessPhoneValidation  @EnrolmentId",
                new SqlParameter("EnrolmentId", "0")
            );

            // Attempt to resolve recipient names in different order
            var option = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var changes = _context.Changes
                .Include(c => c.Household.Mother)
                .Include(c => c.Status).AsQueryable();

            changes = changes.Where(i => i.DateMPESAVerified.ToString().Contains(option) && i.MPESACheckStatusId == 18); // Names Mismatch

            if (changes.Any())
            {
                foreach (var change in changes)
                {
                    var original = _context.HouseholdRegMembers.First(i => i.HouseholdId == change.HouseholdId);

                    var firstName = change.OwnsPhoneId == 1 ? change.Household.Mother.FirstName ?? "" : change.NOKFirstName ?? "";
                    var middleName = change.OwnsPhoneId == 1 ? change.Household.Mother.MiddleName ?? "" : change.NOKMiddleName ?? "";
                    var surname = change.OwnsPhoneId == 1 ? change.Household.Mother.Surname ?? "" : change.NOKSurname ?? "";

                    var count = 0;

                    if (change.CustomerName.ToLower().Contains(firstName.ToLower()))
                    {
                        count++;
                    }
                    if (change.CustomerName.ToLower().Contains(middleName.ToLower()))
                    {
                        count++;
                    }
                    if (change.CustomerName.ToLower().Contains(surname.ToLower()))
                    {
                        count++;
                    }

                    if (count > 1) // If the returned name contains at least two names
                    {
                        change.MPESACheckStatusId = 16;
                        _context.Update(change);
                    }

                    if (change.CustomerName.Contains(firstName) &&
                        change.CustomerName.Contains(middleName) &&
                        change.CustomerName.Contains(surname))
                    {
                        change.MPESACheckStatusId = 16;
                        _context.Update(change);
                    }
                }
                _context.SaveChanges();
            }

            // Effect Changes

            var verified = _context.Changes.Where(i => i.DateMPESAVerified.ToString().Contains(option) && i.MPESACheckStatusId == 16);
            foreach (var change in verified)
            {
                var household = _context.HouseholdRegs.Find(change.HouseholdId);
                var beneficiary = _context.Beneficiaries.SingleOrDefault(i => i.Id == change.HouseholdId);
                if (change.OwnsPhoneId == 1)
                {
                    household.Phone = change.Phone;
                    household.HasProxy = false;

                    if (beneficiary != null)
                    {
                        beneficiary.RecipientPhone = change.Phone;
                        beneficiary.StatusId = 19; // Enrolled
                    }
                }
                else
                {
                    household.RecipientNames = change.NomineeFirstName + " " + change.NomineeMiddleName + " " + change.NomineeSurname;
                    household.Phone = change.Phone;
                    household.HasProxy = true;
                    if (beneficiary != null)
                    {
                        beneficiary.RecipientPhone = change.Phone;
                        beneficiary.RecipientName = change.RecipientNames;
                        beneficiary.StatusId = 19; // Enrolled
                    }
                }
                household.IPRSVerified = (bool)change.IPRSVerified;
                household.IPRSPassed = change.IPRSPassed;
            }
            // Update failed records
            var failed = _context.Changes.Where(i => i.DateMPESAVerified.ToString().Contains(option) && i.MPESACheckStatusId != 16).ToList();
            failed.ForEach(i => i.StatusId = 7); //Failed Verification

            _context.SaveChanges();
            return RedirectToAction("MPesaDetails", new { option = option });
        }

        public async Task<IActionResult> MPesaDetails(ChangesListViewModel vm)
        {
            var changes = _context.Changes
                .OrderByDescending(c => c.DateCreated)
                .Include(c => c.ActionedBy)
                .Include(c => c.ChangeType)
                .Include(c => c.CreatedBy)
                .Include(c => c.MPESACheckStatus)
                .Include(c => c.Household.Mother)
                .Include(c => c.Household.Village.Ward.SubCounty)
                .Include(c => c.Status).AsQueryable();
            var date = DateTime.Parse(vm.Option);

            changes = changes.Where(i => i.DateMPESAVerified.ToString().Contains(vm.Option));

            vm.AwaitingIPRS = _context.Changes.Count(i => i.RequiresIPRSCheck && i.IPRSVerified == false);
            vm.AwaitingMPesaVerification = _context.Changes.Count(i => i.RequiresMPESACheck && i.MPESACheckStatusId == null && i.StatusId == 3);

            var healthFacilityId = _dbService.GetHealthFacilityId();
            if (healthFacilityId != 0)
            {
                bool isGlobal = await _dbService.IsGlobal();
                changes = changes.Where(i => i.Household.HealthFacilityId == healthFacilityId || isGlobal);
            }
            if (vm.HealthFacilityId != null)
            {
                changes = changes.Where(i => i.Household.HealthFacilityId == vm.HealthFacilityId);
            }
            if (!string.IsNullOrEmpty(vm.UniqueId))
            {
                changes = changes.Where(h => h.Household.UniqueId == vm.UniqueId);
            }
            if (!string.IsNullOrEmpty(vm.IdNumber))
            {
                changes = changes.Where(h => h.Household.Mother.IdNumber.Contains(vm.IdNumber));
            }
            if (!string.IsNullOrEmpty(vm.Name))
            {
                changes = changes.Where(h =>
                    h.Household.Mother.FirstName.Contains(vm.Name) ||
                    h.Household.Mother.MiddleName.Contains(vm.Name) ||
                    h.Household.Mother.Surname.Contains(vm.Name)

                );
            }
            if (vm.StatusId != null)
            {
                changes = changes.Where(h => h.StatusId == vm.StatusId);
            }
            if (vm.WardId != null)
            {
                changes = changes.Where(h => h.Household.Village.WardId == vm.WardId);
            }
            if (vm.SubCountyId != null)
            {
                changes = changes.Where(h => h.Household.Village.Ward.SubCountyId == vm.SubCountyId);
            }
            if (vm.HealthFacilityId != null)
            {
                changes = changes.Where(h => h.Household.HealthFacilityId == vm.HealthFacilityId);
            }

            var page = vm.Page ?? 1;
            var pageSize = vm.PageSize ?? 20;

            vm.Changes = changes.ToPagedList(page, pageSize);
            ViewData["StatusId"] = new SelectList(_context.ApprovalStatus, "Id", "Name", vm.StatusId);
            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name", vm.SubCountyId);
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities, "Id", "Name", vm.HealthFacilityId);
            vm.Wards = await _context.Wards.ToListAsync();

            return View(vm);
        }

        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
    }
}