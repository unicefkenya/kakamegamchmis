using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MCHMIS.Areas.Legacy.Data;
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
using System.Data;

namespace MCHMIS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RegistrationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDBService _dbService;
        private readonly IUnitOfWork _uow;
        private ISingleRegistryService _singleRegistryService;
        private readonly ISMSService _smsService;
        private readonly IEncryptDecrypt _encryptDecrypt;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly LegacyDbContext _context2;

        public RegistrationController(ApplicationDbContext context,
            IDBService dbService, IUnitOfWork uow,
            ISingleRegistryService singleRegistryService
             , IHostingEnvironment hostingEnvironment
            , ISMSService smsService, IEncryptDecrypt encryptDecrypt
            , LegacyDbContext context2
            )
        {
            _uow = uow;
            _context = context;
            _context2 = context2;
            _dbService = dbService;
            _singleRegistryService = singleRegistryService;
            _smsService = smsService;
            _encryptDecrypt = encryptDecrypt;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Admin/Registration
        public async Task<IActionResult> Index(HouseholdsListViewModel vm)
        {

            var healthFacilityId = _dbService.GetHealthFacilityId();
            bool isGlobal = await _dbService.IsGlobal();
          
                var households = _context.HouseholdRegs.AsNoTracking()
                .Include(r => r.Village.Ward.SubCounty)
                .Include(r => r.CommunityArea)
                .Include(r => r.Ward.SubCounty)
                .Include(r => r.Mother)
                .Include(r => r.IPRSException)
                .Include(r => r.Status)
                .Include(r => r.HealthFacility)
                .Where(r => r.TypeId == 1)
                .OrderByDescending(r => r.DateCreated)
              .AsQueryable();
              
              
                if (vm.HealthFacilityId != null)
                {
                    households = households.Where(i => i.HealthFacilityId == vm.HealthFacilityId);
                }
                else if (healthFacilityId != 0)
                {
                    households = households.Where(i => i.HealthFacilityId == healthFacilityId || isGlobal);
                }

                if (!string.IsNullOrEmpty(vm.UniqueId))
                {
                    households = households.Where(h => h.UniqueId == vm.UniqueId || h.OldUniqueId == vm.UniqueId);
                }
                if (!string.IsNullOrEmpty(vm.Phone))
                {
                    households = households.Where(h => h.Phone == vm.Phone);
                }
                if (!string.IsNullOrEmpty(vm.IdNumber))
                {
                    households = households.Where(h => h.Mother.IdNumber.Contains(vm.IdNumber));
                }
                if (!string.IsNullOrEmpty(vm.Name))
                {
                    var names = vm.Name.Split(' ');
                    foreach (var name in names)
                    {
                        households = households.Where(h =>
                            h.Mother.FirstName.Contains(name)
                            || h.Mother.MiddleName.Contains(name)
                            || h.Mother.Surname.Contains(name)
                        );
                    }
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
                var page = vm.Page ?? 1;
                var pageSize = vm.PageSize ?? 20;
                //vm.AwaitingIPRS = households.Count(i => i.RequiresIPRSECheck && i.IPRSVerified == false);
                vm.HouseholdRegs = households.ToPagedList(page, pageSize);
            

            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Name", vm.StatusId);
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities.Where(i => i.Id == healthFacilityId || isGlobal), "Id", "Name", vm.HealthFacilityId);
            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name", vm.SubCountyId);
            // vm.Wards = await _context.Wards.ToListAsync();
            return View(vm);
        }

        public IActionResult FingerPrint(string id)
        {
            ViewBag.HouseHoldId = id;
            var household = _context.HouseholdRegs.Find(id);

            if (!household.VerifyingFingerPrint && (household.FingerPrint.Length < 10 || household.FingerPrint == null))
            {
                var sql = @"UPDATE HouseholdRegs SET VerifyingFingerPrint=0  where HealthFacilityId= @healthFacilityId and Id<>@id";
                var rowsAffected = _context.Database.ExecuteSqlCommand(sql,
                    new SqlParameter("@healthFacilityId", _dbService.GetHealthFacilityId()),
                    new SqlParameter("@id", id)
                    );
                household.VerifyingFingerPrint = true;
                _context.SaveChanges();
            }

            if (household.FingerPrint.Length > 10 && household.VerifyingFingerPrint == false)
            {
                ViewBag.Url = "Dwelling/" + id;
                // Check if there is need to collect more information
                if (household.StatusId != 1) // Non Resident
                {
                    return RedirectToAction("Index");
                }
                return RedirectToAction(nameof(Dwelling), new { id });
            }

            ViewBag.Url = "FingerPrint/" + id;
            return View();
        }

        public async Task<IActionResult> Dwelling(string id)
        {
            var vm = new HouseholdDwellingViewModel();
            vm.SystemCodes = await _context.SystemCodes
                .Where(i => i.SystemModuleId == null || i.Code == "Yes No Choices")
                .Include(c => c.SystemCodeDetails).ToListAsync();
            ViewBag.HouseHoldId = id;
            vm.HouseholdId = id;
            if (!string.IsNullOrEmpty(id))
            {
                var xtics = _context.HouseholdRegCharacteristics.SingleOrDefault(i => i.HouseholdId == id);
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(xtics, vm);
                vm.Assets = _context.HouseholdRegAssets.Where(i => i.HouseholdId == id).ToList();

                vm.HouseholdAssets = _context.HouseholdRegAssets
                    .Where(i => i.HouseholdId == id && i.HasItem == true).Select(i => i.AssetId).ToList();

                vm.OtherSPProgrammes = _context.HouseholdRegOtherProgrammes
                    .Where(i => i.HouseholdId == id)
                    .Select(i => i.OtherProgrammeId).ToList();
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dwelling(HouseholdDwellingViewModel vm)
        {
            var xtics = _context.HouseholdRegCharacteristics.SingleOrDefault(i => i.HouseholdId == vm.HouseholdId);
            if (xtics == null)
            {
                xtics = new HouseholdRegCharacteristic();
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, xtics);
                _context.Add(xtics);
            }
            else
            {
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, xtics);
                _context.Update(xtics);
                // Delete Previous Other Programme
                var programmes = _context.HouseholdRegOtherProgrammes.Where(i => i.HouseholdId == vm.HouseholdId);
                _context.HouseholdRegOtherProgrammes.RemoveRange(programmes);
                // Remove Previous Assets
                var assetsOld = _context.HouseholdRegAssets.Where(i => i.HouseholdId == vm.HouseholdId);
                _context.HouseholdRegAssets.RemoveRange(assetsOld);
                await _context.SaveChangesAsync();
            }

            if (vm.OtherSPProgrammes != null)
            {
                ICollection<HouseholdRegOtherProgramme> programmes = new List<HouseholdRegOtherProgramme>();
                foreach (var id in vm.OtherSPProgrammes)
                {
                    var program = new HouseholdRegOtherProgramme
                    {
                        HouseholdId = vm.HouseholdId,
                        OtherProgrammeId = id
                    };
                    programmes.Add(program);
                }
                _context.HouseholdRegOtherProgrammes.AddRange(programmes);
            }
            await _context.SaveChangesAsync();

            ICollection<HouseholdRegAsset> assets = new List<HouseholdRegAsset>();
            var possibleAssets = _context.SystemCodeDetails.Where(s => s.SystemCode.Code == "Household Assets")
                .OrderBy(i => i.OrderNo);
            foreach (var item in possibleAssets)
            {
                var asset = new HouseholdRegAsset
                {
                    HouseholdId = vm.HouseholdId,
                    AssetId = item.Id,
                    AssetTypeId = 203,
                    HasItem = vm.HouseholdAssets != null && vm.HouseholdAssets.Contains(item.Id)
                };
                assets.Add(asset);
            }
            _context.HouseholdRegAssets.AddRange(assets);

            await _context.SaveChangesAsync();
            vm.HouseholdLivestock.ForEach(i => i.HasItem = true);
            vm.HouseholdLivestock.ForEach(i => i.HouseholdId = vm.HouseholdId);
            vm.HouseholdLivestock.ForEach(i => i.AssetTypeId = 204);

            _context.HouseholdRegAssets.AddRange(vm.HouseholdLivestock);

            await _context.SaveChangesAsync();
            TempData["success"] = "Dwelling and household information saved.";
            return RedirectToAction("Demographics", new { id = vm.HouseholdId });
        }

        public async Task<IActionResult> Demographics(string id)
        {
            var members = await _context.HouseholdRegMembers
                .Include(i => i.MaritalStatus)
                .Include(i => i.Relationship)
                .Include(i => i.Gender)
                .Include(i => i.IdentificationForm)
                .OrderBy(c => c.CreateOn)
                .Where(c => c.HouseholdId == id).ToListAsync();
            ViewBag.HouseHoldId = id;
            ViewBag.StatusId = _context.HouseholdRegs.Find(id).StatusId;
            return View(members);
        }

        [Route("admin/registration/add-member/{householdId}/{id?}", Name = "AddMemberRoute")]
        public async Task<IActionResult> AddMember(string householdId, string id)
        {
            ViewBag.HouseHoldId = householdId;

            var member = _context.HouseholdRegMembers.FirstOrDefault(m => m.HouseholdId == householdId);
            var vm = new HouseholdRegMemberViewModel();
            vm.HouseholdId = householdId;

            var systemCodeDetails = _context.SystemCodeDetails.AsNoTracking().OrderBy(i => i.OrderNo).Include(s => s.SystemCode).ToList();

            ViewData["IdentificationFormId"] = new SelectList(systemCodeDetails.OrderBy(i => i.OrderNo)
                .Where(i => i.SystemCode.Code == "Identification Documents"), "Id", "DisplayName");

            if (string.IsNullOrEmpty(id) && member != null && member.RelationshipId == null)
            {
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(member, vm);
                vm.IsMother = true;
                vm.Id = member.Id;
                vm.DisabilityTypes = _context.HouseholdRegMemberDisabilities.Where(i => i.HouseholdRegMemberId == id)
                    .Select(i => i.DisabilityId).ToList();
            }
            if (!string.IsNullOrEmpty(id))
            {
                member = _context.HouseholdRegMembers.FirstOrDefault(m => m.Id == id);
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(member, vm);
                if (member.Id == _context.HouseholdRegs.Find(member.HouseholdId).MotherId)
                {
                    vm.IsMother = true;
                }
                vm.DisabilityTypes = _context.HouseholdRegMemberDisabilities.Where(i => i.HouseholdRegMemberId == id)
                    .Select(i => i.DisabilityId).ToList(); vm.Id = member.Id;
            }
            var supportStatusId = "";
            if (member != null && !string.IsNullOrEmpty(member.SupportStatusId))
            {
                supportStatusId = _encryptDecrypt.DecryptString(member.SupportStatusId);
                vm.SupportStatusId = supportStatusId;
            }

            ViewData["IdentificationFormId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Identification Documents"), "Id", "DisplayName", vm.IdNumber);

            ViewData["GenderId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Gender"), "Id", "DisplayName", vm.GenderId);
            ViewData["MaritalStatusId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Marital Status"), "Id", "DisplayName", vm.MaritalStatusId);
            ViewData["RelationshipId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Relationship"), "Id", "DisplayName", vm.RelationshipId);
            ViewData["SupportStatusId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "HIV Status 2"), "Id", "DisplayName", supportStatusId);

            var booleanOptions = systemCodeDetails.Where(i => i.SystemCode.Code == "Boolean Options");
            ViewData["SpouseInHouseholdId"] = new SelectList(booleanOptions, "Id", "DisplayName", vm.SpouseInHousehold);
            ViewData["FatherAliveId"] = new SelectList(booleanOptions, "Id", "DisplayName", vm.FatherAliveId);
            ViewData["MotherAliveId"] = new SelectList(booleanOptions, "Id", "DisplayName", vm.MotherAliveId);
            ViewData["ChronicIllnessId"] = new SelectList(booleanOptions, "Id", "DisplayName", vm.ChronicIllnessId);
            ViewData["DisabilityTypeId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Disability"), "Id", "DisplayName");
            ViewData["DisabilityRequires24HrCareId"] = new SelectList(booleanOptions, "Id", "DisplayName", vm.DisabilityRequires24HrCareId);

            ViewData["DisabilityCaregiverId"] = new SelectList(_context.HouseholdRegMembers.Where(i => i.HouseholdId == householdId), "Id", "FullName", vm.DisabilityCaregiverId);
            ViewData["EducationAttendanceId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Education Attendance"), "Id", "DisplayName", vm.EducationAttendanceId);
            ViewData["EducationLevelId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Education Level"), "Id", "DisplayName", vm?.EducationAttendanceId);
            ViewData["OccupationTypeId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Work Type"), "Id", "DisplayName", vm.OccupationTypeId);
            ViewData["FormalJobTypeId"] = new SelectList(booleanOptions, "Id", "DisplayName", vm.FormalJobTypeId);
            ViewData["SchoolTypeId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "School Type"), "Id", "DisplayName", vm.SchoolTypeId);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveMember(HouseholdRegMemberViewModel vm)
        {
            string id = ViewBag.HouseHoldId = vm.HouseholdId;

           

            if (ModelState.IsValid)
            {
                var member = new HouseholdRegMember();
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, member);
                if (!string.IsNullOrEmpty(vm.SupportStatusId))
                {
                    member.SupportStatusId = _encryptDecrypt.EncryptString(vm.SupportStatusId.ToString());
                }

                if (!string.IsNullOrEmpty(vm.Id))
                {
                    _context.Update(member);
                    vm.IsMother = true;
                }
                else
                {
                    member.CreateOn = DateTime.UtcNow.AddHours(3);
                    _context.Add(member);
                }

                // Add Disability Types
                _context.HouseholdRegMemberDisabilities.RemoveRange(_context.HouseholdRegMemberDisabilities.Where(i => i.HouseholdRegMemberId == member.Id));
                _context.SaveChanges();
                if (vm.DisabilityTypes != null)
                {
                    var disabilityTypes = new List<HouseholdRegMemberDisability>();
                    foreach (var item in vm.DisabilityTypes)
                    {
                        var disability = new HouseholdRegMemberDisability
                        {
                            DisabilityId = item,
                            HouseholdRegMemberId = member.Id
                        };
                        disabilityTypes.Add(disability);
                    }
                    _context.AddRange(disabilityTypes);
                    _context.SaveChanges();
                }
                TempData["Message"] = "Details saved successfully";
                return RedirectToAction(nameof(Demographics), new { id = vm.HouseholdId });
            }
            var systemCodeDetails = _context.SystemCodeDetails.OrderBy(i => i.OrderNo).Include(s => s.SystemCode).ToList();
            ViewData["GenderId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Gender"), "Id", "DisplayName", vm.GenderId);
            ViewData["MaritalStatusId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Marital Status"), "Id", "DisplayName", vm.MaritalStatusId);
            ViewData["RelationshipId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Relationship"), "Id", "DisplayName", vm.RelationshipId);
            ViewData["IdentificationFormId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Identification Documents"), "Id", "DisplayName", vm.IdentificationFormId);
            var booleanOptions = systemCodeDetails.Where(i => i.SystemCode.Code == "Boolean Options");
            ViewData["SpouseInHouseholdId"] = new SelectList(booleanOptions, "Id", "DisplayName");
            ViewData["FatherAliveId"] = new SelectList(booleanOptions, "Id", "DisplayName");
            ViewData["MotherAliveId"] = new SelectList(booleanOptions, "Id", "DisplayName");
            ViewData["ChronicIllnessId"] = new SelectList(booleanOptions, "Id", "DisplayName");
            ViewData["DisabilityTypeId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Disability"), "Id", "DisplayName");
            ViewData["DisabilityRequires24HrCareId"] = new SelectList(booleanOptions, "Id", "DisplayName");

            ViewData["DisabilityCaregiverId"] = new SelectList(_context.HouseholdRegMembers.Where(i => i.HouseholdId == id), "Id", "FullName");
            ViewData["EducationAttendanceId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Education Attendance"), "Id", "DisplayName");
            ViewData["EducationLevelId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Education Level"), "Id", "DisplayName");
            ViewData["OccupationTypeId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Work Type"), "Id", "DisplayName");
            ViewData["FormalJobTypeId"] = new SelectList(booleanOptions, "Id", "DisplayName");
            ViewData["SchoolTypeId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "School Type"), "Id", "DisplayName");
            return View("AddMember", vm);
        }

        // [Route("admin/registration/{id?}/{view?}")]
        public async Task<IActionResult> Details(string id, string view)
        {
            if (id == null)
            {
                return NotFound();
            }
            var houseHold = _context.HouseholdRegs
                .Include(i => i.HealthFacility)
                .Include(i => i.Status)
                .Include(i => i.Notes)
                .Include(i => i.Village.Ward.SubCounty.County)
                .Include(i => i.SubLocation.Location)
                .SingleOrDefault(h => h.Id == id);
            if (houseHold == null)
            {
                return NotFound();
            }
            var vm = new HouseholdDetailsViewModel();
            vm.View = view;
            var members = await _context.HouseholdRegMembers
                .Include(i => i.MaritalStatus)
                .Include(i => i.Relationship)
                .Include(i => i.Relationship)
                .Include(i => i.Gender)
                .Include(i => i.IdentificationForm)
                .OrderBy(c => c.CreateOn)
                .Where(c => c.HouseholdId == id).ToListAsync();
            vm.Household = houseHold;
            vm.Members = members;
            vm.Characteristic = _context.HouseholdRegCharacteristics
                .Include(i => i.TenureStatus)
                .Include(i => i.RoofMaterial)
                .Include(i => i.WallMaterial)
                .Include(i => i.FloorMaterial)
                .Include(i => i.HouseholdCondition)
                .Include(i => i.HasSkippedMeal)
                .Include(i => i.IsReceivingOtherBenefit)
                .Include(i => i.OtherProgrammesBenefitType)
                .Include(i => i.WaterSource)
                .Include(i => i.ToiletType)
                .Include(i => i.CookingFuelSource)
                .Include(i => i.LightingSource)
                .Include(i => i.UnitRisk)
                .Include(i => i.BenefitFromFriendsRelative)
                .SingleOrDefault(i => i.HouseholdId == id);
            vm.Assets = await _context.HouseholdRegAssets
                .Include(i => i.Asset)
                .OrderBy(i => i.Asset.OrderNo)
                .Where(i => i.HouseholdId == id).ToListAsync();
            vm.OtherProgrammes = await _context.HouseholdRegOtherProgrammes
                .Include(i => i.OtherProgramme)
                .Where(i => i.HouseholdId == id).ToListAsync();

            var mother = _context.HouseholdRegMembers.Single(i => i.Id == houseHold.MotherId);
            if (!string.IsNullOrEmpty(mother.SupportStatusId))
            {
                var supportStatusId = int.Parse(_encryptDecrypt.DecryptString(mother.SupportStatusId));
                vm.SupportStatus = _context.SystemCodeDetails.Find(supportStatusId);
            }

            vm.Disabilities = _context.HouseholdRegMemberDisabilities
                .Include(c => c.Disability)
                .Where(i => i.HouseholdRegMemberId == houseHold.MotherId)
                .ToList();

            return View(vm);
        }

        public async Task<IActionResult> MemberDetails(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.HouseholdRegMembers
                .Include(i => i.MaritalStatus)
                .Include(i => i.Relationship)
                .Include(i => i.Gender)
                .Include(i => i.IdentificationForm)
                .Include(i => i.SpouseInHousehold)
                .Include(i => i.FatherAlive)
                .Include(i => i.MotherAlive)
                .Include(i => i.DisabilityRequires24HrCare)
                .Include(i => i.DisabilityCaregiver)
                .Include(i => i.EducationLevel)
                .Include(i => i.EducationAttendance)
                .Include(i => i.SchoolType)
                .Include(i => i.OccupationType)
                .Include(i => i.FormalJobType)
                .SingleOrDefaultAsync(m => m.Id == id);
            ViewBag.Disabilities = _context.HouseholdRegMemberDisabilities
                .Include(c => c.Disability)
                .Where(i => i.HouseholdRegMemberId == id)
                .ToList();
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // GET: Admin/Registration/Create
        public IActionResult Create(string id)
        {
            var vm = new RegistrationViewModel();
            ViewData["CountyId"] = new SelectList(_context.Counties.Where(i => i.Id == 801 || i.Id == 9001), "Id", "Name");
            ViewData["OtherCountyId"] = new SelectList(_context.Counties.Where(i => i.Id != 801 && i.Id != 9001), "Id", "Name");
            // ViewData["SubCountyId"] = new SelectList(_context.SubCounties.Where(i => i.Id == _dbService.GetHealthFacilitySubCountyId()), "Id", "Name");
            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name");

            // ViewData["ConstituencyId"] = new SelectList(_context.Constituencies, "Id", "Name");
            ViewData["OwnsPhoneId"] = new SelectList(_context.SystemCodeDetails
                .Where(i => i.SystemCode.Code == "Boolean Options" && i.OrderNo < 3), "OrderNoNumber", "Code");

            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities, "Id", "Name");
            ViewData["MaritalStatusId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Marital Status"), "Id", "DisplayName");
            ViewData["IdentificationFormId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Identification Documents"), "Id", "DisplayName");

            var options = _context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices").OrderBy(i => i.OrderNo)
                .ToList();
            ViewData["HasBeenInMCHProgramId"] = new SelectList(options, "Id", "Code");

            //ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name");
            //ViewData["SubLocationId"] = new SelectList(_context.SubLocations, "Id", "Name");
            // ViewData["VillageId"] = new SelectList(_context.Villages, "Id", "Name");
            // vm.Locations = _context.Locations.Include(l => l.Division.District).ToList();
            vm.Wards = _context.Wards.ToList();
            vm.SubLocations = _context.SubLocations.Include(i => i.Location).ToList();
            vm.Villages = _context.Villages.ToList();
            vm.CommunityAreas = _context.CommunityAreas.ToList();

            vm.MinimumAge = _context.SystemSettings.Single(i => i.key == "MINIMUM.MOTHER.AGE").Value;
            ViewBag.HouseHoldId = vm.Id = id;
            if (!string.IsNullOrEmpty(id))
            {
                var household = _context.HouseholdRegs.Find(id);
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(household, vm);
                var mother = _context.HouseholdRegMembers.Find(household.MotherId);
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(mother, vm);
                ViewData["CountyId"] = new SelectList(_context.Counties.Where(i => i.Id == 801 || i.Id == 9001), "Id", "Name", household.CountyId);
                ViewData["OtherCountyId"] = new SelectList(_context.Counties.Where(i => i.Id != 801 && i.Id != 9001), "Id", "Name", household.OtherCountyId);

                var village = _context.Villages.Include(i => i.Ward.SubCounty).SingleOrDefault(c => c.Id == household.VillageId);
                if (village != null)
                {
                    vm.VillageId = village.Id;
                    vm.WardId = village.WardId;
                    vm.SubCountyId = village.Ward.SubCountyId;
                }
                vm.OwnsPhoneId = household.HasProxy == false ? 1 : 2;
            }
            else // Creating a new registration
            {
                var healthFacilityId = _dbService.GetHealthFacilityId();
                if (healthFacilityId == 0)
                {
                    TempData["Info"] = "You are not assigned to any health facility so you cannot register mothers.";
                    return RedirectToAction(nameof(Index));
                }
            }
            vm.VerifyingFingerPrint = true;
            return View(vm);
        }

        // POST: Admin/Registration/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegistrationViewModel vm, IFormFile file)
        {
            var proceed = true;
            // Check if phone number is already registred

            if (_context.HouseholdRegs.Any(i => i.Phone == vm.Phone))
            {
                TempData["Info"] = "Phone number already registered.";
                proceed = false;
            }
            if (_context2.Mother.Any(i => i.M_Phoneno == vm.Phone))
            {
                TempData["Info"] = "Phone number already registered in the legacy system.";
                proceed = false;
            }
            if (!vm.Phone.StartsWith("07") && !vm.Phone.StartsWith("011"))
            {
                TempData["Info"] = "Phone number for mother / nominee should start with 07... or 011...";
                proceed = false;
            }
            if (vm.EDD<DateTime.UtcNow)
            {
                TempData["Info"] = "EDD is less than registration date. Only pregnant mothers can be registered.";
                proceed = false;
            }
            if (vm.IdentificationFormId == 186)
            {
                if (_context.HouseholdRegMembers.Any(i => i.IdNumber == vm.IdNumber))
                {
                    // Check if the ID number belongs to a registered after
                    var householdIds = _context.HouseholdRegMembers.Where(i => i.IdNumber == vm.IdNumber)
                        .Select(i => i.Id).ToList();
                    if (_context.HouseholdRegs.Any(i => householdIds.Contains(i.MotherId)))
                    {
                        TempData["Info"] = "ID Number number already registered.";
                        proceed = false;
                    }
                }

                if (_context2.Mother.Any(i => i.M_IDNo == vm.IdNumber))
                {
                    // Check if the ID number belongs to a registered after
                    var householdIds = _context.HouseholdRegMembers.Where(i => i.IdNumber == vm.IdNumber)
                        .Select(i => i.Id).ToList();
                    if (_context.HouseholdRegs.Any(i => householdIds.Contains(i.MotherId)))
                    {
                        TempData["Info"] = "ID Number number already registered in the legacy system.";
                        proceed = false;
                    }
                }

                var exception = _context2.Dirty.FirstOrDefault(i => i.Mother.M_IDNo == vm.IdNumber);
                if (exception != null)
                {
                    TempData["Info"] = "ID Number number already registered in the legacy system but could not be migrated due to the following reason: <strong>" + exception.Reason + "<strong/>";
                    proceed = false;
                }
            }

            if (vm.IdentificationFormId == 186 || vm.OwnsPhoneId == 2)
                vm.RequiresIPRSECheck = true;
            try
            {
                if (proceed && vm.RequiresIPRSECheck) // National Id
                {
                    try
                    {
                        // Do IPRS Check
                        var login = new LoginVm
                        {
                            Password = _context.SystemSettings.Single(i => i.key == "SR.PASSWORD").Value,
                            UserName = _context.SystemSettings.Single(i => i.key == "SR.USERNAME").Value
                        };
                        var auth = await _singleRegistryService.Login(login);
                        if (auth.TokenAuth != null)
                        {
                            string IdNumber = "", firstName = "", middleName = "", surname = "";
                            if (vm.IdentificationFormId == 186) //National ID Card
                            {
                                IdNumber = vm.IdNumber;
                                firstName = vm.FirstName;
                                middleName = vm.MiddleName;
                                surname = vm.Surname;
                            }
                            else
                            {
                                IdNumber = vm.NomineeIdNumber;
                                firstName = vm.NomineeFirstName;
                                middleName = vm.NomineeMiddleName;
                                surname = vm.NomineeSurname;
                            }
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
                                vm.IPRSVerified = true;

                                if (vm.IdentificationFormId == 186) // If mother is the primary recepient, cross check DOB
                                {
                                    try
                                    {
                                        hhdIprs.Date_of_Birth = hhdIprs.Date_of_Birth.Trim();
                                        if (hhdIprs.Date_of_Birth.Length == 9)
                                            hhdIprs.Date_of_Birth = "0" + hhdIprs.Date_of_Birth;
                                        DateTime dt = DateTime.ParseExact(hhdIprs.Date_of_Birth, "MM/dd/yyyy",
                                            CultureInfo.InvariantCulture);
                                        var dob = ((DateTime)vm.DOB);
                                        if (dt.ToString("yyyy").Equals(dob.ToString("yyyy")))
                                        {
                                            vm.IPRSPassed = true;
                                        }
                                        //else if (!dt.ToString("MM/dd/yyyy").Equals(dob.ToString("MM/dd/yyyy")))
                                        //{
                                        //    vm.IPRSMatch = true;
                                        //}
                                        else
                                        {
                                            TempData["Info"] =
                                                "The Beneficiary <strong>Date of birth </strong> does not match the ones on the Integrated Population Registration System. " +
                                                "<br />Correct date of birth and try again.";
                                            proceed = false;
                                        }
                                    }
                                    catch (Exception ex) // IPRS returned wrong date format, so just proceed
                                    {
                                    }
                                }

                                var firstnamesMatched =
                                    hhdIprs.First_Name.Trim().ToLower() == firstName.Trim().ToLower();

                                var middleNamesMatched = false;
                                if (!string.IsNullOrEmpty(hhdIprs.Middle_Name))
                                {
                                    if (middleName != null)
                                    {
                                        middleNamesMatched =
                                            hhdIprs.Middle_Name.ToLower().Trim() == middleName.ToLower().Trim();
                                    }
                                }
                                else
                                {
                                    middleNamesMatched = true;
                                }

                                var surnameMatched = hhdIprs.Surname.Trim().ToLower() == surname.Trim().ToLower();
                                if (firstnamesMatched &&
                                middleNamesMatched && surnameMatched)
                                {
                                    vm.IPRSPassed = true;
                                }
                                else
                                {
                                    TempData["Info"] =
                                        "The " + (vm.IdentificationFormId == 186 ? "Beneficiary" : "Nominee") + " ID Number and the Names given do not match the ones on the Integrated Population Registration System. " +
                                        "<br />Correct the ID Number / Names and try again.";
                                    proceed = false;
                                }
                            }
                            //else
                            //{
                            //    TempData["Info"] =
                            //        "The Beneficiary ID Number is invalid. " +
                            //        "<br /><br />Correct the ID Number and try again.";
                            //    proceed = false;
                            //}
                        }
                        else
                        {
                            proceed = true;
                            vm.IPRSVerified = false;
                            vm.IPRSPassed = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        proceed = true;
                    }
                }
            }
            catch (Exception ex)
            {
                proceed = true;
            }

            if (ModelState.IsValid && proceed)
            {
                vm.HealthFacilityId = _dbService.GetHealthFacilityId();
                var sql = @"UPDATE HouseholdRegs SET VerifyingFingerPrint=0  where HealthFacilityId= @healthFacilityId";
                var rowsAffected = _context.Database.ExecuteSqlCommand(sql, new SqlParameter("@healthFacilityId", vm.HealthFacilityId));
                var userID = User.GetUserId();
                var household = new HouseholdReg();
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, household);

                household.StatusId = household.CountyId == 801 ? 1 : 20;
                household.Id = Guid.NewGuid().ToString().ToLower();
                household.CreatedById = userID;
                household.TypeId = 1;
                household.VerifyingFingerPrint = true;
                household.DateCreated = DateTime.UtcNow.AddHours(3);
                household.HealthFacilityId = _dbService.GetHealthFacilityId();
                // Get the next UniqueId for the facility
                var uniqueId = GetUniqueId(household.HealthFacilityId);
                while (_context.HouseholdRegs.Any(i => i.UniqueId == uniqueId))
                {
                    uniqueId = (int.Parse(uniqueId) + 1).ToString();
                }
                household.UniqueId = uniqueId; 

                if (file != null && file.Length > 0)
                {
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    string path = "";
                    var fileName = DateTime.UtcNow.AddHours(3).ToString("yyyymmddhhmmss") + "-";
                    fileName = fileName + file.FileName;
                    path = webRootPath + "/uploads/registration/" + Path.GetFileName(fileName);
                    var stream = new FileStream(path, FileMode.Create);
                    await file.CopyToAsync(stream);
                   
                    household.SupportingDocument = fileName;
                }
                if (vm.OwnsPhoneId == 1)
                {
                    household.RecipientNames = vm.FirstName + " " + vm.MiddleName + " " + vm.Surname;
                    household.HasProxy = false;
                }
                else
                {
                    household.RecipientNames = vm.NomineeFirstName + " " + vm.NomineeMiddleName + " " + vm.NomineeSurname;
                    household.HasProxy = true;
                }

                household.RecipientNames = household.RecipientNames.Trim().Replace("  ", " "); // Replace double spacing with one. If middle name is null, there will be double spacing

                household.CommonName = household.CommonName?.ToUpper();

                
               
                _uow.GetRepository<HouseholdReg>().Add(household);

                _uow.Save();
                // Partially Register Mother
                var member = new HouseholdRegMember();
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, member);
                member.HouseholdId = household.Id;
                member.GenderId = 192;

                member.Id = Guid.NewGuid().ToString().ToLower();
                member.CreateOn = DateTime.UtcNow.AddHours(3);
                member.FirstName = member.FirstName.Replace("`", "'").ToUpper();
                member.MiddleName = member.MiddleName?.Replace("`", "'").ToUpper();
                member.Surname = member.Surname.Replace("`", "'").ToUpper();

                _uow.GetRepository<HouseholdRegMember>().Add(member);
                _uow.Save();

                household.MotherId = member.Id;
                _context.HouseholdRegs.Update(household);

                TempData["success"] = "Mother's information saved.";

                if (vm.HasBeenInMCHProgramId == 269) // Has been a member
                {
                    household.StatusId = 22;
                }
                if (vm.ResidenceDurationYears == 0 && vm.ResidenceDurationMonths < 6) // Residence Less than 6 Months
                {
                    household.StatusId = 23;
                }
                await _context.SaveChangesAsync();
                if (household.StatusId != 1) // Non Resident
                {
                    TempData["success"] = "You are not a resident of Kakamega county thus not eligible to be enrolled in the program.<br />However you are encourage to keep updating your clinics visits. This will make you be receiving reminders on your clinics.";
                    return RedirectToAction("Index");
                }
                return RedirectToAction(nameof(FingerPrint), new { id = household.Id });
            }

            var errors = ModelState.Select(x => x.Value.Errors)
                .Where(y => y.Count > 0)
                .ToList();
            var message = string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));

            ViewData["CountyId"] = new SelectList(_context.Counties.Where(i => i.Id == 801 || i.Id == 9001), "Id", "Name");
            ViewData["OtherCountyId"] = new SelectList(_context.Counties.Where(i => i.Id != 801 && i.Id != 9001), "Id", "Name");

            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name");

            // ViewData["ConstituencyId"] = new SelectList(_context.Constituencies, "Id", "Name");
            ViewData["OwnsPhoneId"] = new SelectList(_context.SystemCodeDetails
                .Where(i => i.SystemCode.Code == "Boolean Options" && i.OrderNo < 3), "OrderNoNumber", "Code");

            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities, "Id", "Name");
            ViewData["MaritalStatusId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Marital Status"), "Id", "DisplayName");
            ViewData["IdentificationFormId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Identification Documents"), "Id", "DisplayName");

            var options = _context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices").OrderBy(i => i.OrderNo)
                .ToList();
            ViewData["HasBeenInMCHProgramId"] = new SelectList(options, "Id", "Code");

            // vm.Locations = _context.Locations.Include(l => l.Division.District).ToList();
            vm.Wards = _context.Wards.ToList();
            vm.SubLocations = _context.SubLocations.Include(i => i.Location).ToList();
            vm.Villages = _context.Villages.ToList();
            vm.CommunityAreas = _context.CommunityAreas.ToList();
            vm.MinimumAge = _context.SystemSettings.Single(i => i.key == "MINIMUM.MOTHER.AGE").Value;
            ViewBag.HouseHoldId = vm.Id;
            return View(vm);
        }

        public async Task<IActionResult> Edit(RegistrationViewModel vm, IFormFile file)
        {
            var proceed = true;

            // Check if phone number is already registred
            if (_context.HouseholdRegs.Any(i => i.Phone == vm.Phone && i.Id != vm.Id))
            {
                TempData["Info"] = "Phone number already registered.";
                proceed = false;
            }
            if (!vm.Phone.StartsWith("07") & !vm.Phone.StartsWith("011"))
            {
                TempData["Info"] = "Phone number for mother / nominee should start with 07... or 011...";
                proceed = false;
            }

            if (vm.IdentificationFormId == 186)
            {
                if (_context.HouseholdRegMembers.Any(i => i.IdNumber == vm.IdNumber))
                {
                    // Check if the ID number belongs to a registered mother
                    var householdIds = _context.HouseholdRegMembers.Where(i => i.IdNumber == vm.IdNumber && i.HouseholdId != vm.Id)
                        .Select(i => i.Id).ToList();
                    if (_context.HouseholdRegs.Any(i => householdIds.Contains(i.MotherId)))
                    {
                        TempData["Info"] = "ID Number number already registered.";
                        proceed = false;
                    }
                }
            }

            if (vm.IdentificationFormId == 186 || vm.OwnsPhoneId == 2)
                vm.RequiresIPRSECheck = true;
            if (proceed && vm.RequiresIPRSECheck) // National Id
            {
                try
                {
                    // Do IPRS Check
                    var login = new LoginVm
                    {
                        Password = _context.SystemSettings.Single(i => i.key == "SR.PASSWORD").Value,
                        UserName = _context.SystemSettings.Single(i => i.key == "SR.USERNAME").Value
                    };
                    var auth = await _singleRegistryService.Login(login);
                    if (auth.TokenAuth != null)
                    {
                        string IdNumber = "", firstName = "", middleName = "", surname = "";
                        if (vm.IdentificationFormId == 186) //National ID Card
                        {
                            IdNumber = vm.IdNumber;
                            firstName = vm.FirstName;
                            middleName = vm.MiddleName;
                            surname = vm.Surname;
                        }
                        else
                        {
                            IdNumber = vm.NomineeIdNumber;
                            firstName = vm.NomineeFirstName;
                            middleName = vm.NomineeMiddleName;
                            surname = vm.NomineeSurname;
                        }
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
                            vm.IPRSVerified = true;

                            if (vm.IdentificationFormId == 186) // If mother is the primary recepient, cross check DOB
                            {
                                try
                                {
                                    hhdIprs.Date_of_Birth = hhdIprs.Date_of_Birth.Trim();
                                    if (hhdIprs.Date_of_Birth.Length == 9)
                                        hhdIprs.Date_of_Birth = "0" + hhdIprs.Date_of_Birth;
                                    DateTime dt = DateTime.ParseExact(hhdIprs.Date_of_Birth, "MM/dd/yyyy",
                                        CultureInfo.InvariantCulture);
                                    var dob = ((DateTime)vm.DOB);
                                    if (dt.ToString("yyyy").Equals(dob.ToString("yyyy")))
                                    {
                                        vm.IPRSPassed = true;
                                    }
                                    //else if (!dt.ToString("MM/dd/yyyy").Equals(dob.ToString("MM/dd/yyyy")))
                                    //{
                                    //    vm.IPRSMatch = true;
                                    //}
                                    else
                                    {
                                        TempData["Info"] =
                                            "The Beneficiary <strong>Date of birth </strong> does not match the ones on the Integrated Population Registration System. " +
                                            "<br />Correct date of birth and try again.";
                                        proceed = false;
                                    }
                                }
                                catch (Exception ex) // IPRS returned wrong date format, so just proceed
                                {
                                }
                            }

                            var firstnamesMatched =
                                hhdIprs.First_Name.Trim().ToLower() == firstName.Trim().ToLower();
                            var middleNamesMatched = false;
                            if (!string.IsNullOrEmpty(hhdIprs.Middle_Name))
                            {
                                if (middleName != null)
                                {
                                    middleNamesMatched =
                                        hhdIprs.Middle_Name.ToLower().Trim() == middleName.ToLower().Trim();
                                }
                            }
                            else
                            {
                                middleNamesMatched = true;
                            }

                            var surnameMatched = hhdIprs.Surname.Trim().ToLower() == surname.Trim().ToLower();
                            if (firstnamesMatched &&
                            middleNamesMatched && surnameMatched)
                            {
                                vm.IPRSPassed = true;
                            }
                            else
                            {
                                TempData["Info"] =
                                    "The " + (vm.IdentificationFormId == 186 ? "Beneficiary" : "Nominee") + " ID Number and the Names given do not match the ones on the Integrated Population Registration System. " +
                                    "<br />Correct the ID Number / Names and try again.";
                                proceed = false;
                            }
                        }
                        //else
                        //{
                        //    TempData["Info"] =
                        //        "The Beneficiary ID Number is invalid. " +
                        //        "<br /><br />Correct the ID Number and try again.";
                        //    proceed = false;
                        //}
                    }
                    else
                    {
                        proceed = true;
                        vm.IPRSVerified = false;
                        vm.IPRSPassed = null;
                    }
                }
                catch (Exception ex)
                {
                    proceed = true;
                }
            }

            if (ModelState.IsValid && proceed)
            {
                // var rowsAffected = _context.Database.ExecuteSqlCommand("UPDATE HouseholdRegs SET VerifyingFingerPrint=0");

                var household = _context.HouseholdRegs.Find(vm.Id);
                string x = household.UniqueId;
                int value;
                if (!int.TryParse(x, out value))
                {
                 
                    household.OldUniqueId = household.UniqueId;
                    household.UniqueId = GetUniqueId(household.HealthFacilityId);
                }

                var fingerPrint = household.FingerPrint;

                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, household);
                household.CreatedById = User.GetUserId(); // Edit Created by to current user to enable taking of finger prints
                household.FingerPrint = fingerPrint;
                household.StatusId = household.CountyId == 801 ? 1 : 20;
                household.CommonName = household.CommonName?.ToUpper();
                //if (string.IsNullOrEmpty(household.CreatedById))

                if (file != null && file.Length > 0)
                {
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    string path = "";
                    var fileName = DateTime.UtcNow.AddHours(3).ToString("yyyymmddhhmmss") + "-";
                    fileName = fileName + file.FileName;
                    path = webRootPath + "/uploads/registration/" + Path.GetFileName(fileName);
                    var stream = new FileStream(path, FileMode.Create);
                    await file.CopyToAsync(stream);
                   
                    household.SupportingDocument = fileName;
                }
                // household.VerifyingFingerPrint = true;
                if (vm.OwnsPhoneId == 1)
                {
                    household.RecipientNames = vm.FirstName + " " + vm.MiddleName + " " + vm.Surname;
                    household.HasProxy = false;
                }
                else
                {
                    household.RecipientNames = vm.NomineeFirstName + " " + vm.NomineeMiddleName + " " + vm.NomineeSurname;
                    household.HasProxy = true;
                }

                household.RecipientNames = household.RecipientNames.Trim().Replace("  ", " "); // Replace double spacing with one. If middle name is null, there will be double spacing

                // Edit Partially Register Mother
                var member = _context.HouseholdRegMembers.SingleOrDefault(i => i.Id == household.MotherId);
                if (member != null)
                {
                    member.FirstName = vm.FirstName.ToUpper();
                    member.MiddleName = vm.MiddleName?.ToUpper();
                    member.Surname = vm.Surname.ToUpper();
                    member.IdentificationFormId = vm.IdentificationFormId;
                    member.IdNumber = vm.IdNumber;
                    member.MaritalStatusId = vm.MaritalStatusId;
                    member.DOB = (DateTime)vm.DOB;
                }

                if (member == null) // A check for migrated data with issues
                {
                    new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, member);
                    member.HouseholdId = household.Id;
                    member.GenderId = 192;

                    member.Id = Guid.NewGuid().ToString().ToLower();
                    member.CreateOn = DateTime.UtcNow.AddHours(3);

                    member.FirstName = member.FirstName.Replace("`", "'").ToUpper();
                    member.MiddleName = member.MiddleName?.Replace("`", "'").ToUpper();
                    member.Surname = member.Surname.Replace("`", "'").ToUpper();

                    _uow.GetRepository<HouseholdRegMember>().Add(member);
                    _uow.Save();

                    household.MotherId = member.Id;
                }
                else
                {
                    _uow.GetRepository<HouseholdRegMember>().Update(member);
                }
                _uow.GetRepository<HouseholdReg>().Update(household);

                _uow.Save();

                TempData["success"] = "Mother's information updated.";
                if (household.StatusId != 1) // Non Resident
                {
                    TempData["success"] = "You are not a resident of Kakamega county thus not eligible to be enrolled in the program.<br />However you are encourage to keep updating your clinics visits. This will make you be receiving reminders on your clinics.";
                    return RedirectToAction("Index");
                }
                if (fingerPrint == null || fingerPrint.Length == 0)
                    return RedirectToAction(nameof(FingerPrint), new { id = household.Id });
                return RedirectToAction(nameof(Create), new { id = household.Id });
            }
            ViewData["CountyId"] = new SelectList(_context.Counties.Where(i => i.Id == 801 || i.Id == 9001), "Id", "Name");
            ViewData["OtherCountyId"] = new SelectList(_context.Counties.Where(i => i.Id != 801 && i.Id != 9001), "Id", "Name");

            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name");

            // ViewData["ConstituencyId"] = new SelectList(_context.Constituencies, "Id", "Name");
            ViewData["OwnsPhoneId"] = new SelectList(_context.SystemCodeDetails
                .Where(i => i.SystemCode.Code == "Boolean Options" && i.OrderNo < 3), "OrderNoNumber", "Code");

            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities, "Id", "Name");
            ViewData["MaritalStatusId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Marital Status"), "Id", "DisplayName");
            ViewData["IdentificationFormId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Identification Documents"), "Id", "DisplayName");

            var options = _context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices").OrderBy(i => i.OrderNo)
                .ToList();
            ViewData["HasBeenInMCHProgramId"] = new SelectList(options, "Id", "Code");

            // vm.Locations = _context.Locations.Include(l => l.Division.District).ToList();
            vm.Wards = _context.Wards.ToList();
            vm.SubLocations = _context.SubLocations.Include(i => i.Location).ToList();
            vm.Villages = _context.Villages.ToList();
            vm.CommunityAreas = _context.CommunityAreas.ToList();
            vm.MinimumAge = _context.SystemSettings.Single(i => i.key == "MINIMUM.MOTHER.AGE").Value;
            ViewBag.HouseHoldId = vm.Id;

            return View("Create", vm);
        }
        public string GetUniqueId(int healthFacilityId)
        {
            SqlParameter[] @params =
            {
                new SqlParameter("@FacilityId", healthFacilityId),
                new SqlParameter("@Option", "ONE"),
                new SqlParameter("@returnVal", SqlDbType.Int) {Direction = ParameterDirection.Output}
            };
            _context.Database.ExecuteSqlCommand("exec @returnVal=GetMotherUniqueId @FacilityId,@Option", @params);
            var facilityNextId = @params[2].Value;
            var uniqueId = facilityNextId.ToString();
            return uniqueId;
        }
        public ActionResult SendForApproval(string[] Ids)
        {
            if (Ids == null || Ids.Length == 0)
            {
                TempData["Info"] = "No households selected.";
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
                var household = _context.HouseholdRegs.Find(id);
                household.StatusId = 3;
            }
            await _context.SaveChangesAsync();
            var count = Ids.Count();

            //// Email Notifications
            //var em = new EmailService();
            //// Get Role with Approval rights, from the RTSU or Global

            //var changes = db.Changes
            //    .Include(c => c.Beneficiary.Registration.Village.Parish.SubCounty.County)
            //    .Include(c => c.ChangeType).Where(c => Ids.Contains(c.Id)).ToList();
            //var userId = User.GetUserId();
            //var roles = new List<string>();
            //foreach (var id in Ids)
            //{
            //    var changeType = "Approves " + changes.Single(c => c.Id == id).ChangeType.Name;
            //    var prof = db.RoleProfiles.Where(r => r.SystemTask.TaskName == changeType).Select(r => r.RoleId)
            //        .ToList();
            //    roles.AddRange(prof);
            //}
            //var districtIds = userPermissions.Districts();

            //// Current RTSU
            //var districtId = changes.First().Beneficiary.Registration.Village.Parish.SubCounty.County.DistrictId;
            //var rtsuId = db.Districts.Find(districtId).RTSUId;

            //var rtsuUsersIds = db.RTSUUsers.Where(r => r.RTSUId == rtsuId).Select(u => u.UserId).ToList();

            //var user = db.Users.Find(userId);
            //roles = roles.Distinct().ToList();
            //foreach (var role in roles)
            //{
            //    var recipients = _uow.GetRepository<ApplicationUser>()
            //        .GetAll(u => u.Roles.Select(r => r.RoleId).Contains(role)).Distinct().ToList();

            //    var rtsuUsersWithRoles = recipients.Where(r => rtsuUsersIds.Contains(r.Id)).ToList();

            //    foreach (var recipient in rtsuUsersWithRoles)
            //    {
            //        var tEmail = new Thread(() =>
            //            em.SendAsync(new CaseManagementEmail
            //            {
            //                To = recipient.Email,
            //                Subject = "Case Management Awaiting Approval ",
            //                Name = recipient.FirstName,
            //                // Item = count+" case"+(count>1?"s":""),
            //                Item = "Cases",
            //                Action = "for approval",
            //                Narration = "has sent",
            //                User = user,
            //            })
            //        );
            //        tEmail.Start();
            //    }
            //}

            TempData["Success"] = count + " household" + (count > 1 ? "s" : "") + " sent for approval";

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Approve(ApprovalViewModel vm)
        {
            var userId = User.GetUserId();
            var household = _context.HouseholdRegs.Find(vm.Id);
            // Check if mother is HIV Postive or has disability
            var mother = _context.HouseholdRegMembers.Find(household.MotherId);
            var motherSupportStatusId = mother.SupportStatusId;
            var motherMUAC = mother.MUAC;
            household.StatusId = vm.StatusId;

            if (motherSupportStatusId.Equals("LDi63M+Rqvg=") && vm.StatusId == 4)
            {
                household.StatusId = 10;
                household.ReasonId = 1;
            }
            // Disability Check
            if (_context.HouseholdRegMemberDisabilities.Any(i => i.HouseholdRegMemberId == household.MotherId) && vm.StatusId == 4)
            {
                household.StatusId = 10;
                household.ReasonId = 2;
            }
            // Check Under Age Mothers
            if (mother.Age < 18)
            {
                household.StatusId = 10;
                household.ReasonId = 3;
            }
            if (motherMUAC > 0 && motherMUAC < 21)
            {
                household.StatusId = 10;
                household.ReasonId = 4;
            }

            household.ApprovedById = userId;
            household.DateApproved = DateTime.UtcNow.AddHours(3);

            if (!string.IsNullOrEmpty(vm.Notes))
            {
                var note = new Notes
                {
                    HouseholdId = vm.Id,
                    CategoryId = 1,
                    CreatedById = userId,
                    DateCreated = DateTime.UtcNow.AddHours(3),
                    Description = vm.Notes
                };
                _context.Add(note);
            }
            // If status is approved. check for eligibility
            if (vm.StatusId == 4)
            {
                var pmtCutoff = decimal.Parse(_context.SystemSettings.Single(i => i.key == "PMT.CUTOFF").Value);
                var pmtScore = household.PMTScore;
                household.StatusId = pmtScore <= pmtCutoff ? 30 : 29; // Eligible, Ineligible
               household.PMTCutOffUsed = pmtCutoff;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Household Status Updated.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<ActionResult> SaveHousehold(string id)
        {
            // Check if household head is already saved
            var head = _context.HouseholdRegMembers.FirstOrDefault(i => i.HouseholdId == id && i.RelationshipId == 112);
            if (head == null)
            {
                TempData["Info"] = "Household head information missing. Add the Household head details before saving.";
                return RedirectToAction("Demographics", new { id });
            }
            var houseHold = _context.HouseholdRegs.Find(id);
            if (houseHold.FingerPrint.Length < 5)
            {
                TempData["Info"] = "Fingerprint needs to be taken.";
                return RedirectToAction("Demographics", new { id });
            }
            if (string.IsNullOrEmpty(houseHold.SupportingDocument))
            {
                TempData["Info"] = "Supporting document(s) missing.";
                return RedirectToAction("Demographics", new { id });
            }
            // Check is household characteristics are set
            if (!_context.HouseholdRegCharacteristics.Any(i => i.HouseholdId == id))
            {
                TempData["Info"] = "Dwelling and household characteristics missing.";
                return RedirectToAction("Demographics", new { id });
            }

            // Check if mother is HIV Postive or has disability
            var mother = _context.HouseholdRegMembers.Find(houseHold.MotherId);
            var motherSupportStatusId = mother.SupportStatusId;
            var motherMUAC = mother.MUAC;
            houseHold.RequiresHealthVerification = false;
            if (motherSupportStatusId.Equals("LDi63M+Rqvg=")) //Positive
            {
                houseHold.RequiresHealthVerification = true;
            }
            // Disability Check
            if (_context.HouseholdRegMemberDisabilities.Any(i => i.HouseholdRegMemberId == houseHold.MotherId))
            {
                houseHold.RequiresHealthVerification = true;
            }

            if (motherMUAC > 0 && motherMUAC < 21)
            {
                houseHold.RequiresHealthVerification = true;
            }
            //houseHold.StatusId = 14;
            houseHold.StatusId = 2;

            // Send SMS
            var smses = _context.SMS.OrderBy(i => i.Order).Where(i => i.TriggerEvent == "REGISTRATION").ToList();
            foreach (var sms in smses)
            {
                _smsService.Send(houseHold.Phone, sms.Message);
            }

            await _context.SaveChangesAsync();
            // Calculate PMT Score
            var rowsAffected = _context.Database.ExecuteSqlCommand("PMT_Generate  @HouseholdId",
                new SqlParameter("HouseholdId", id)
                );

            TempData["Success"] = "Household information saved.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> IPRS()
        {
            ViewBag.Count = _context.HouseholdRegs
                .Count(i => //i.StatusId == 4 &&// Approved
                             i.RequiresIPRSECheck &&
                             i.TypeId == 1 && i.IPRSVerified == false);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IPRS(int Id)
        {
            int matched = 0;
            int failed = 0;
            var households = _context.HouseholdRegs
                .Include(i => i.Mother)
                .Where(i => //i.StatusId == 4 &&// Approved
                    i.RequiresIPRSECheck &&
                    i.TypeId == 1 && i.IPRSVerified == false
                    );

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
                    foreach (var household in households)
                    {
                        string IdNumber = "", firstName = "", middleName = "", surname = "";

                        if (household.Mother.IdentificationFormId == 186) //National ID Card
                        {
                            IdNumber = household.Mother.IdNumber;
                            firstName = household.Mother.FirstName;
                            middleName = household.Mother.MiddleName;
                            surname = household.Mother.Surname;
                        }
                        else
                        {
                            IdNumber = household.NomineeIdNumber;
                            firstName = household.NOKFirstName;
                            middleName = household.NOKMiddleName;
                            surname = household.NOKSurname;
                        }
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
                            household.IPRSVerified = true;

                            var firstnamesMatched =
                                hhdIprs.First_Name.Trim().ToLower() == firstName.Trim().ToLower();

                            var middleNamesMatched = false;
                            if (!string.IsNullOrEmpty(hhdIprs.Middle_Name))
                            {
                                if (middleName != null)
                                {
                                    middleNamesMatched =
                                        hhdIprs.Middle_Name.ToLower().Trim() == middleName.ToLower().Trim();
                                }
                            }
                            else
                            {
                                middleNamesMatched = true;
                            }

                            var surnameMatched = hhdIprs.Surname.Trim().ToLower() == surname.Trim().ToLower();
                            if (firstnamesMatched &&
                                middleNamesMatched && surnameMatched)
                            {
                                household.IPRSPassed = true;
                                matched++;
                                if (household.HasProxy == false) // If mother is the primary recepient, cross check DOB
                                {
                                    try
                                    {
                                        hhdIprs.Date_of_Birth = hhdIprs.Date_of_Birth.Trim();
                                        if (hhdIprs.Date_of_Birth.Length == 9)
                                            hhdIprs.Date_of_Birth = "0" + hhdIprs.Date_of_Birth;
                                        DateTime dt = DateTime.ParseExact(hhdIprs.Date_of_Birth, "MM/dd/yyyy",
                                            CultureInfo.InvariantCulture);
                                        var dob = ((DateTime)household.Mother.DOB);

                                        if (dt.ToString("yyyy").Equals(dob.ToString("yyyy")))
                                        {
                                            household.IPRSPassed = true;
                                        }
                                        //else if (!dt.ToString("MM/dd/yyyy").Equals(dob.ToString("MM/dd/yyyy")))
                                        //{
                                        //    household.IPRSPassed = true;
                                        //}
                                        //else if (!dt.ToString("MM/dd/yyyy").Equals(dob.ToString("MM/dd/yyyy")))
                                        //{
                                        //    household.IPRSPassed = true;
                                        //}
                                        else
                                        {
                                            failed++;
                                            matched--;
                                            household.IPRSExceptionId = 334;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }
                            }
                            else // Names Not Matched
                            {
                                household.IPRSExceptionId = 333;
                                failed++;
                            }
                        }
                        else
                        {
                            failed++;
                            household.IPRSExceptionId = 335; // Invalid National Id
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

        [HttpPost]
        public IActionResult Blacklist(BlacklistViewModel vm)
        {
            var houseHold = _context.HouseholdRegs
                .SingleOrDefault(h => h.Id == vm.Id);
            if (houseHold == null)
            {
                return NotFound();
            }

            houseHold.StatusId = 28; //Blacklisted]
            var note = new Notes
            {
                HouseholdId = vm.Id,
                Description = vm.Notes,
                DateCreated = DateTime.UtcNow,
                CreatedById = User.GetUserId(),
                CategoryId = 1
            };
            _uow.GetRepository<Notes>().Add(note);
            // Deactivate beneficiary if any
            var beneficiary = _context.Beneficiaries
                .SingleOrDefault(h => h.HouseholdId == vm.Id);
            if (beneficiary != null)
            {
                beneficiary.StatusId = 28; //Blacklisted
                _uow.GetRepository<Beneficiary>().Update(beneficiary);
            }

            _uow.GetRepository<HouseholdReg>().Update(houseHold);

            _uow.Save();

            TempData["success"] = "Household blacklisted successfully";
            return RedirectToAction(nameof(Index));
        }

        


    }
}