using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

using MCHMIS.Data;
using MCHMIS.Extensions;
using MCHMIS.Interfaces;
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
    // [Route("admin/[controller]/[action]")]
    public class CommunityValidationController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IUnitOfWork _uow;
        private readonly IDBService _dbService;
        private readonly ISMSService _smsService;
        private readonly IExportService _exportService;

        public CommunityValidationController(ApplicationDbContext context, IDBService dbService, IUnitOfWork uow
            , ISMSService smsService, IExportService exportService)
        {
            _uow = uow;
            _context = context;
            _dbService = dbService;
            _smsService = smsService;
            _exportService = exportService;
        }

        [Route("admin/community-validation")]
        public async Task<IActionResult> Index(CommunityValidationListViewModel vm)
        {
            //// Calculate Variance

            //var data = _context.CvListDetails.Where(i=>i.InterviewResultId==181).ToList();
            //foreach(var item in data)
            //{
            //    CalculateVariance(item.CVHouseHoldId);
            //}
            //

            if (TempData["VarianceCategoryId"] != null)
                vm.VarianceCategoryId = int.Parse(TempData["VarianceCategoryId"].ToString());

            bool isGlobal = await _dbService.IsGlobal();
            IQueryable<CVListDetail> details = _context.CvListDetails
                .Include(r => r.Household.Village.Ward.SubCounty)
                .Include(r => r.Household.CommunityArea)
                .Include(r => r.Household.Status)
                .Include(r => r.Status)
                .Include(r => r.ActionedBy)
                .Include(r => r.CVHouseHold)
                .Include(r => r.Household.HealthFacility)
                .Include(r => r.Household.Mother)
                .Include(r => r.VarianceCategory)
                .Include(r => r.Enumerator)
                .Include(r => r.InterviewResult)
                .OrderByDescending(i => i.DateSubmitedByCHV)
                .ThenByDescending(i => i.List.DateCreated)
                .ThenBy(i => i.Household.PMTScore)
                .Where(i => i.List.ListTypeId == 211 || i.List.ListTypeId == 357); // CHV List or Top-up List

            var userId = User.GetUserId();
            var user = _context.Users.Find(userId);

            details = user.HealthFacilityId != null ?
                details.Where(i => i.Household.HealthFacilityId == user.HealthFacilityId || isGlobal) : details.Where(i => isGlobal);

            if (vm.StatusId != null)
            {
                details = details.Where(h => h.StatusId == vm.StatusId);
            }
            if (vm.WardId != null)
            {
                details = details.Where(h => h.Household.Village.WardId == vm.WardId);
            }
            if (vm.SubCountyId != null)
            {
                details = details.Where(h => h.Household.Village.Ward.SubCountyId == vm.SubCountyId);
            }
            if (vm.HealthFacilityId != null)
            {
                details = details.Where(h => h.Household.HealthFacilityId == vm.HealthFacilityId);
            }
            if (vm.SelectedEnumeratorId != null)
            {
                details = details.Where(h => h.EnumeratorId == vm.SelectedEnumeratorId);
            }
            if (vm.VarianceCategoryId != null)
            {
                details = details.Where(h => h.VarianceCategoryId == vm.VarianceCategoryId);
            }
            if (!string.IsNullOrEmpty(vm.UniqueId))
            {
                details = details.Where(h => h.Household.UniqueId == vm.UniqueId);
            }
            if (vm.StartDate != null)
            {
                details = details.Where(i => i.DateSubmitedByCHV >= vm.StartDate);
            }
            if (vm.EndDate != null)
            {
                details = details.Where(i => i.DateSubmitedByCHV <= vm.EndDate);
            }
            if (!string.IsNullOrEmpty(vm.Name))
            {
                details = details.Where(h =>
                    h.Household.Mother.FirstName.Contains(vm.Name)
                    || h.Household.Mother.MiddleName.Contains(vm.Name)
                    || h.Household.Mother.Surname.Contains(vm.Name)
                    || h.Household.Mother.Surname.Contains(vm.Name)
                );
            }
            var page = vm.Page ?? 1;
            var pageSize = vm.PageSize ?? 20;
            vm.ListDetails = details.ToPagedList(page, pageSize);
            var statusIds = new[] { 9, 3, 4, 10, 5 };
            ViewData["StatusId"] = new SelectList(_context.Status
                .Where(i => statusIds.Contains(i.Id)), "Id", "Name", vm.StatusId);

            ViewData["SubCountyId"] = new SelectList(_context.Constituencies, "Id", "Name", vm.SubCountyId);
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities, "Id", "Name", vm.HealthFacilityId);
           
            ViewData["VarianceCategoryId"] = new SelectList(_context.SystemCodeDetails.Where(i=>i.SystemCode.Code== "Variance Category"), "Id", "Code", vm.VarianceCategoryId);

            ViewData["EnumeratorId"] = new SelectList(_context.Enumerators.Where(i => i.EnumeratorGroupId == 313), "Id", "FullName", vm.EnumeratorId);  ViewData["EnumeratorId"] = new SelectList(_context.Enumerators.Where(i => i.EnumeratorGroupId == 313), "Id", "FullName", vm.EnumeratorId);
            vm.Wards = await _context.Wards.ToListAsync();
            vm.CVList = _context.CVLists.Find(vm.Id);
            return View(vm);
        }
        [Route("admin/community-validation/monitoring-evaluation")]
        public async Task<IActionResult> MonitoringEvaluation(CommunityValidationListViewModel vm)
        {
           

            if (TempData["VarianceCategoryId"] != null)
                vm.VarianceCategoryId = int.Parse(TempData["VarianceCategoryId"].ToString());

            bool isGlobal = await _dbService.IsGlobal();
            IQueryable<CVListDetail> details = _context.CvListDetails
                .Include(r => r.Household.Village.Ward.SubCounty)
                .Include(r => r.Household.CommunityArea)
                .Include(r => r.Household.Status)
                .Include(r => r.Status)
                .Include(r => r.ActionedBy)
                .Include(r => r.CVHouseHold)
                .Include(r => r.Household.HealthFacility)
                .Include(r => r.Household.Mother)
                .Include(r => r.VarianceCategory)
                .Include(r => r.Enumerator)
                .Include(r => r.InterviewResult)
                .OrderByDescending(i => i.DateSubmitedByCHV)
                .ThenByDescending(i => i.List.DateCreated)
                .ThenBy(i => i.Household.PMTScore)
                .Where(i => i.List.ListTypeId == 212);

            var userId = User.GetUserId();
            var user = _context.Users.Find(userId);

            details = user.HealthFacilityId != null ?
                details.Where(i => i.Household.HealthFacilityId == user.HealthFacilityId || isGlobal) : details.Where(i => isGlobal);

            if (vm.StatusId != null)
            {
                details = details.Where(h => h.StatusId == vm.StatusId);
            }
            if (vm.WardId != null)
            {
                details = details.Where(h => h.Household.Village.WardId == vm.WardId);
            }
            if (vm.SubCountyId != null)
            {
                details = details.Where(h => h.Household.Village.Ward.SubCountyId == vm.SubCountyId);
            }
            if (vm.HealthFacilityId != null)
            {
                details = details.Where(h => h.Household.HealthFacilityId == vm.HealthFacilityId);
            }
            if (vm.SelectedEnumeratorId != null)
            {
                details = details.Where(h => h.EnumeratorId == vm.SelectedEnumeratorId);
            }
            if (vm.VarianceCategoryId != null)
            {
                details = details.Where(h => h.VarianceCategoryId == vm.VarianceCategoryId);
            }
            if (!string.IsNullOrEmpty(vm.UniqueId))
            {
                details = details.Where(h => h.Household.UniqueId == vm.UniqueId);
            }
            if (vm.StartDate != null)
            {
                details = details.Where(i => i.DateSubmitedByCHV >= vm.StartDate);
            }
            if (vm.EndDate != null)
            {
                details = details.Where(i => i.DateSubmitedByCHV <= vm.EndDate);
            }
            if (!string.IsNullOrEmpty(vm.Name))
            {
                details = details.Where(h =>
                    h.Household.Mother.FirstName.Contains(vm.Name)
                    || h.Household.Mother.MiddleName.Contains(vm.Name)
                    || h.Household.Mother.Surname.Contains(vm.Name)
                    || h.Household.Mother.Surname.Contains(vm.Name)
                );
            }
            var page = vm.Page ?? 1;
            var pageSize = vm.PageSize ?? 20;
            vm.ListDetails = details.ToPagedList(page, pageSize);
            var statusIds = new[] { 9, 3, 4, 10, 5 };
            ViewData["StatusId"] = new SelectList(_context.Status
                .Where(i => statusIds.Contains(i.Id)), "Id", "Name", vm.StatusId);

            ViewData["SubCountyId"] = new SelectList(_context.Constituencies, "Id", "Name", vm.SubCountyId);
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities, "Id", "Name", vm.HealthFacilityId);
           
            ViewData["VarianceCategoryId"] = new SelectList(_context.SystemCodeDetails.Where(i=>i.SystemCode.Code== "Variance Category"), "Id", "Code", vm.VarianceCategoryId);

            ViewData["EnumeratorId"] = new SelectList(_context.Enumerators.Where(i => i.EnumeratorGroupId == 313), "Id", "FullName", vm.EnumeratorId);  ViewData["EnumeratorId"] = new SelectList(_context.Enumerators.Where(i => i.EnumeratorGroupId == 313), "Id", "FullName", vm.EnumeratorId);
            vm.Wards = await _context.Wards.ToListAsync();
            vm.CVList = _context.CVLists.Find(vm.Id);
            return View(vm);
        }

        [Route("admin/community-validation/lists")]
        public async Task<IActionResult> Lists(CVListsViewModel vm)
        {
            bool isGlobal = await _dbService.IsGlobal();
            var healthFacilityId = _dbService.GetHealthFacilityId();
            IQueryable<CVList> lists = _context.CVLists
                .Include(r => r.CreatedBy)
                .Include(r => r.ApprovedBy)
                .Include(r => r.HealthFacility.SubCounty)
                .Include(r => r.Status)
                .OrderByDescending(r => r.DateCreated)
                .Where(i => (i.ListTypeId == 211 || i.ListTypeId== 357)
                            && (i.HealthFacilityId == healthFacilityId || isGlobal));
            var healthFacilities = _context.HealthFacilities.Where(i => i.Id == healthFacilityId || isGlobal).AsQueryable();
            if (vm.StatusId != null)
            {
                lists = lists.Where(h => h.StatusId == vm.StatusId);
              
            }
            if (vm.SubCountyId != null)
            {
                lists = lists.Where(h => h.HealthFacility.SubCountyId == vm.SubCountyId);
                healthFacilities= healthFacilities.Where(h => h.SubCountyId == vm.SubCountyId);
            }
            if (vm.HealthFacilityId != null)
            {
                lists = lists.Where(h => h.HealthFacilityId == vm.HealthFacilityId);
            }
            var page = vm.Page ?? 1;
            var pageSize = vm.PageSize ?? 20;
            vm.Lists = lists.ToPagedList(page, pageSize);
            
            ViewData["StatusId"] = new SelectList(_context.ApprovalStatus.ToList(), "Id", "Name", vm.StatusId);
            ViewData["SubCountyId"] = new SelectList(_context.Constituencies, "Id", "Name", vm.SubCountyId);
            
            ViewData["HealthFacilityId"] = new SelectList(healthFacilities, "Id", "Name", vm.HealthFacilityId);
            vm.Wards = await _context.Wards.ToListAsync();

            return View(vm);
        }

        [Route("admin/community-validation/monitoring-evaluation/lists")]
        public async Task<IActionResult> MonitoringEvaluationLists(CVListsViewModel vm)
        {
            //var details = _context.CvListDetails.Where(i => i.List.ListTypeId == 212 && i.StatusId == 4).ToList();
            //foreach (var cvListDetail in details)
            //{
                
            //    // Calculate Variance
            //    var houseHold = _context.HouseholdRegs.Single(i => i.Id == cvListDetail.CVHouseHoldId);
            //    var parent = _context.HouseholdRegs.Find(houseHold.ParentId);
            //    var originalRecord = _context.HouseholdRegs.Find(parent.Id);
            //    var pmtScore = houseHold.PMTScore;
            //    parent.PMTScoreFinal = pmtScore;

              
            //        // check for eligibility
            //    var pmtCutoff = decimal.Parse(_context.SystemSettings.Single(i => i.key == "PMT.CUTOFF").Value);
            //    parent.StatusId = originalRecord.StatusId = pmtScore <= pmtCutoff ? 10 : 29; // Ready for Enrolment, Ineligible
            //    parent.PMTCutOffUsed = originalRecord.PMTCutOffUsed = pmtCutoff;
            //    _context.SaveChanges();
                

            //}

            IQueryable<CVList> lists = _context.CVLists
                .Include(r => r.CreatedBy)
                .Include(r => r.ApprovedBy)
                .Include(r => r.HealthFacility.SubCounty)
                .Include(r => r.Status).OrderByDescending(r => r.DateCreated).Where(i => i.ListTypeId == 212);


            var page = vm.Page ?? 1;
            var pageSize = vm.PageSize ?? 20;
            vm.Lists = lists.ToPagedList(page, pageSize);
            ViewData["StatusId"] = new SelectList(_context.ApprovalStatus.ToList(), "Id", "Name", vm.StatusId);
            ViewData["SubCountyId"] = new SelectList(_context.Constituencies, "Id", "Name", vm.SubCountyId);
            vm.Wards = await _context.Wards.ToListAsync();
            return View(vm);
        }

        [Route("admin/community-validation/create")]
        public async Task<IActionResult> Create()
        {
            var healthFacilityId = _dbService.GetHealthFacilityId();
            var bens = _context.HouseholdRegs.Where(i =>
                i.StatusId == 30 && // Eligible
                i.TypeId == 1 && (i.HealthFacilityId == healthFacilityId)).ToList();
            var vm = new CVListGenerateViewModel
            {
                Total = bens.Count(),
                Ready = bens.Count(i => i.IPRSVerified && i.IPRSPassed == true || !i.RequiresIPRSECheck),
                NotMatched = bens.Count(i => i.IPRSVerified && i.IPRSPassed == false),
                NotIprsed = bens.Count(i => i.IPRSVerified == false && i.RequiresIPRSECheck)
            };
            return View(vm);
        }

        [HttpPost]
        [Route("admin/community-validation/save")]
        public async Task<IActionResult> Save(CVListGenerateViewModel vm)
        {
            var completeHouseholds = _context.HouseholdRegs.Where(i => i.StatusId == 4 && i.TypeId == 1);
            var healthFacilityId = _dbService.GetHealthFacilityId();
            if (vm.StatusIds == null || !vm.StatusIds.Any())
            {
                TempData["Info"] = "No households selected.";
                return RedirectToAction("Index");
            }

            var userId = User.GetUserId();
            var user = _context.Users.Find(userId);
            var list = new CVList
            {
                DateCreated = DateTime.UtcNow.AddHours(3),
                CreatedById = user.Id,
                StatusId = 1,
                ListTypeId = 211,
                Notes = vm.Notes,
                HealthFacilityId = healthFacilityId
            };
            _context.Add(list);
            await _context.SaveChangesAsync();

            bool ready = false, notIPRSed = false, notMatched = false;
            if (vm.StatusIds != null && vm.StatusIds.Any())
            {
                ready = vm.StatusIds.Contains(1);
                notMatched = vm.StatusIds.Contains(2);
                notIPRSed = vm.StatusIds.Contains(3);
            }

            _context.Database.ExecuteSqlCommand(";Exec GenerateCHVList @listId,@HealthFacilityId,@ready,@notMatched,@notIPRSed,@NoToGenerate",
                new SqlParameter("listId", list.Id),
                new SqlParameter("HealthFacilityId", healthFacilityId),
                new SqlParameter("ready", ready),
                new SqlParameter("notMatched", notMatched),
                new SqlParameter("notIPRSed", notIPRSed),
                new SqlParameter("NoToGenerate", vm.NoToGenerate)

            );

            TempData["Message"] = "List generated successfully.";

            var test = _context.CvListDetails.Where(i => i.Household.UniqueId.Contains("MTH")).ToList();
            foreach (var item in test)
            {
                var hh = _context.HouseholdRegs.Single(i => i.TypeId == 1 && i.Id == item.HouseholdId);
                hh.UniqueId = GetUniqueId(hh.HealthFacilityId);
                _context.HouseholdRegs.Update(hh);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Details", new { id = list.Id });
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

        [Route("admin/community-validation/topups")]
        public IActionResult Topups()
        {

            var cutOff =Decimal.Parse(_context.SystemSettings.Single(i => i.key == "PMT.CUTOFF").Value);
            var vm = new TopupsViewModel
            {
                Eligible = _context.HouseholdRegs.Count(i=>i.PMTScore<= cutOff),
                InCommunityValidation = _context.HouseholdRegs.Count(i => i.StatusId == 6),
                RecommendedForME = _context.HouseholdRegs.Count(i => i.StatusId == 11),
                ReadyForEnrolment = _context.HouseholdRegs.Count(i => i.StatusId == 10),
                Ineligible= _context.HouseholdRegs.Count(i => i.StatusId == 29)

            };
            vm.Total = vm.InCommunityValidation + vm.Eligible + vm.RecommendedForME + vm.Eligible;
            return View(vm);
        }
        [HttpPost]
        [Route("admin/community-validation/topupsave")]
        public async Task<IActionResult> TopupSave(CVListGenerateViewModel vm)
        {
            var completeHouseholds = _context.HouseholdRegs.Where(i => i.StatusId == 4 && i.TypeId == 1);
            var userId = User.GetUserId();
            var user = _context.Users.Find(userId);
            var list = new CVList
            {
                DateCreated = DateTime.UtcNow.AddHours(3),
                CreatedById = user.Id,
                StatusId = 1,
                ListTypeId = 357, // Top-list
                Notes = vm.Notes,
            };
            _context.Add(list);
            await _context.SaveChangesAsync();

            bool ready = false, notIPRSed = false, notMatched = false;
            if (vm.StatusIds != null && vm.StatusIds.Any())
            {
                ready = vm.StatusIds.Contains(1);
                notMatched = vm.StatusIds.Contains(2);
                notIPRSed = vm.StatusIds.Contains(3);
            }

            _context.Database.ExecuteSqlCommand(";Exec GenerateCHVTopUpList @listId,@NoToGenerate",
                new SqlParameter("listId", list.Id),
                new SqlParameter("NoToGenerate", vm.NoToGenerate)

            );

            TempData["Message"] = "List generated successfully.";
            return RedirectToAction("Details", new { id = list.Id });
        }

        [Route("admin/community-validation/details/{id}")]
        public async Task<IActionResult> Details(CommunityValidationListViewModel vm)
        {
            var userId = User.GetUserId();
            var user = _context.Users.Find(userId);
            bool isGlobal = await _dbService.IsGlobal();
            IQueryable<CVListDetail> details = _context.CvListDetails

                .Include(r => r.Household.Village.Ward.SubCounty)
                .Include(r => r.Household.CommunityArea)
                .Include(r => r.Household.Status)
                .Include(r => r.Status)
                .Include(r => r.ActionedBy)
                .Include(r => r.CVHouseHold)
                .Include(r => r.Household.HealthFacility)
                .Include(r => r.Household.Mother)
                .Include(r => r.VarianceCategory)
                .Include(r => r.Enumerator)
                .Include(r => r.InterviewResult)
                .OrderBy(i => i.Household.PMTScore)
                .Where(i => i.ListId == vm.Id && (i.Household.HealthFacilityId == user.HealthFacilityId || isGlobal));

            //if (vm.StatusId != null)
            //{
            //    households = households.Where(h => h.StatusId == vm.StatusId);
            //}
            //if (vm.WardId != null)
            //{
            //    households = households.Where(h => h.Village.WardId == vm.WardId);
            //}
            //if (vm.SubCountyId != null)
            //{
            //    households = households.Where(h => h.Village.Ward.SubCountyId == vm.SubCountyId);
            //}
           
            if (vm.StatusId != null)
            {
                details = details.Where(h => h.StatusId == vm.StatusId);
            }
            if (vm.WardId != null)
            {
                details = details.Where(h => h.Household.Village.WardId == vm.WardId);
            }
            if (vm.SubCountyId != null)
            {
                details = details.Where(h => h.Household.Village.Ward.SubCountyId == vm.SubCountyId);
            }
            if (vm.HealthFacilityId != null)
            {
                details = details.Where(h => h.Household.HealthFacilityId == vm.HealthFacilityId);
            }
            if (vm.EnumeratorId != null)
            {
                details = details.Where(h => h.EnumeratorId == vm.EnumeratorId);
            }
            if (!string.IsNullOrEmpty(vm.UniqueId))
            {
                details = details.Where(h => h.Household.UniqueId == vm.UniqueId);
            }
            if (vm.StartDate != null)
            {
                details = details.Where(i => i.DateSubmitedByCHV >= vm.StartDate);
            }
            if (vm.EndDate != null)
            {
                details = details.Where(i => i.DateSubmitedByCHV <= vm.EndDate);
            }
            if (!string.IsNullOrEmpty(vm.Name))
            {
                details = details.Where(h =>
                    h.Household.Mother.FirstName.Contains(vm.Name)
                    || h.Household.Mother.MiddleName.Contains(vm.Name)
                    || h.Household.Mother.Surname.Contains(vm.Name)
                    || h.Household.Mother.Surname.Contains(vm.Name)
                );
            }
            var page = vm.Page ?? 1;
            var pageSize = vm.PageSize ?? 20;
            vm.Details = details.ToPagedList(page, pageSize);
            var statusIds = new[] { 9, 3, 4, 10, 5 };
            ViewData["StatusId"] = new SelectList(_context.Status
                .Where(i => statusIds.Contains(i.Id)), "Id", "Name", vm.StatusId);

            ViewData["SubCountyId"] = new SelectList(_context.Constituencies, "Id", "Name", vm.SubCountyId);
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities, "Id", "Name", vm.HealthFacilityId);

            ViewData["EnumeratorId"] = new SelectList(_context.Enumerators.Where(i => i.EnumeratorGroupId == 313), "Id", "FullName", vm.EnumeratorId);

            vm.Wards = await _context.Wards.ToListAsync();
            vm.CVList = _context.CVLists.Find(vm.Id);
            ViewBag.PageSize = this.GetPager(vm.PageSize);
            return View(vm);
        }

        public async Task<IActionResult> Assign(CommunityValidationListViewModel vm)
        {
            IQueryable<CVListDetail> details = _context.CvListDetails

                .Include(r => r.Household.Status)
                .Include(r => r.Household.CommunityArea)
                .Include(r => r.Household.Village.Ward.SubCounty)
                .Include(r => r.Status)
                .Include(r => r.CVHouseHold)
                .Include(r => r.Enumerator)
                .Include(r => r.Household.HealthFacility)
                .Include(r => r.Household.Mother)
                .Include(r => r.VarianceCategory)
                .Where(i => i.ListId == vm.Id 
                        //    && i.Status.Name=="Pending"
                            );
            var enumerators = _context.Enumerators.Where(i => i.IsActive);

            if (vm.SubCountyId != null)
            {
                details = details.Where(h => h.Household.Village.Ward.SubCountyId == vm.SubCountyId);
                enumerators = enumerators.Where(h => h.Village.Ward.SubCountyId == vm.SubCountyId);
            }
            if (vm.WardId != null)
            {
                details = details.Where(h => h.Household.Village.WardId == vm.WardId);
                enumerators = enumerators.Where(h => h.Village.WardId == vm.WardId);
            }
            if (vm.VillageId != null)
            {
                details = details.Where(h => h.Household.VillageId == vm.VillageId);
                enumerators = enumerators.Where(h => h.VillageId == vm.VillageId);
            }
            if (vm.CommunityAreaId != null)
            {
                details = details.Where(h => h.Household.CommunityAreaId == vm.CommunityAreaId);
            }

            var count = details.Count();
       
            vm.Details = details.ToPagedList(1, count);
            ViewData["StatusId"] = new SelectList(_context.SystemCodeDetails
                .Where(i => i.SystemCode.Code == "Beneficiary Status"), "Id", "Code", vm.StatusId);
            ViewData["SubCountyId"] = new SelectList(_context.Constituencies, "Id", "Name", vm.SubCountyId);

            vm.Wards = await _context.Wards.ToListAsync();
            vm.Villages = await _context.Villages.ToListAsync();
            vm.CommunityAreas = await _context.CommunityAreas.ToListAsync();

            vm.CVList = _context.CVLists.Find(vm.Id);
            if (vm.CVList.ListTypeId == 211 || vm.CVList.ListTypeId == 357)
                ViewData["EnumeratorId"] = new SelectList(enumerators.Where(i => i.EnumeratorGroupId == 313), "Id", "FullName", vm.EnumeratorId);
            else
                ViewData["EnumeratorId"] = new SelectList(enumerators.Where(i => i.EnumeratorGroupId == 314), "Id", "FullName", vm.EnumeratorId);

            ModelState.Clear();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AssignSave(AssignViewModel vm)
        {
            if (vm.Ids == null)
            {
                TempData["Info"] = "No households selected";
            }
            else
            {
                foreach (var id in vm.Ids)
                {
                    var household = _context.CvListDetails.Find(id);
                    household.EnumeratorId = vm.EnumeratorId;
                }
                await _context.SaveChangesAsync();
                var count = vm.Ids.Length;
                TempData["Info"] = count + " household" + (count > 1 ? "s" : "") + " assigned";
            }

            return RedirectToAction(nameof(Assign), new { vm.Id });
        }

        // https://stackoverflow.com/questions/39364687/export-html-to-pdf-in-asp-net-core
        // https://jsreport.net/learn/dotnet-aspnetcore
        //  [MiddlewareFilter(typeof(JsReportPipeline))]
        [Route("admin/community-validation/export/{id}")]
        public async Task<FileStreamResult> Export(CommunityValidationListViewModel vm)
        {
            bool isGlobal = await _dbService.IsGlobal();
            IQueryable<CVListDetail> details = _context.CvListDetails

                .Include(r => r.Household.Village.Ward.SubCounty)
                .Include(r => r.Household.CommunityArea)
                .Include(r => r.Household.Status)
                .Include(r => r.Status)
                .Include(r => r.CVHouseHold)
                .Include(r => r.Household.HealthFacility)
                .Include(r => r.Household.Mother)
                .Include(r => r.VarianceCategory)
                .Include(r => r.Enumerator)
                .OrderBy(i => i.Household.PMTScore)
                .Where(i => i.ListId == vm.Id);
            var userId = User.GetUserId();
            var user = _context.Users.Find(userId);
            if (user.SubCountyId != null)
            {
                details = details.Where(i => i.Household.Village.Ward.SubCountyId == user.SubCountyId || isGlobal);
            }
            else
            {
                details = details.Where(i => isGlobal);
            }

            var data = details.Select(b => new CVListExportViewModel
            {
                MotherUniqueId = b.Household.UniqueId,
                FirstName = b.Household.Mother.FirstName,
                MiddleName = b.Household.Mother.MiddleName,
                Surname = b.Household.Mother.Surname,
                CommonName = b.Household.CommonName,
                IdNumber = b.Household.Mother.IdNumber,
                HealthFacility = b.Household.HealthFacility.Name,
                Phone = b.Household.Phone,
                SubCounty = b.Household.Village.Ward.SubCounty.Name,
                Ward = b.Household.Village.Ward.Name,
                VillageUnit = b.Household.Village.Name,
                CommunityArea = b.Household.CommunityArea.Name,
                CHV = b.Enumerator.FullName
            }).ToList();

            var file = _exportService.ExportToExcel(data, "Community Validation List Batch#" + vm.Id);

            return file;
        }

        //public async Task<IActionResult> Export2(HouseholdDwellingViewModel vm)
        //{
        //    vm.SystemCodes = await _context.SystemCodes.Include(c => c.SystemCodeDetails).ToListAsync();

        //    var rs = new LocalReporting().UseBinary(JsReportBinary.GetBinary()).AsUtility().Create();

        //    var report = await rs.RenderAsync(new RenderRequest()
        //    {
        //        Template = new Template()
        //        {
        //            Recipe = Recipe.PhantomPdf,
        //            Engine = Engine.None,
        //            Content = "<style>body{background:red}</style><strong>Hello from pdf</strong>",
        //        }
        //    });

        //    var memoryStream = new MemoryStream();
        //    await report.Content.CopyToAsync(memoryStream);
        //    memoryStream.Seek(0, SeekOrigin.Begin);

        //    return new FileStreamResult(memoryStream, "application/pdf") { FileDownloadName = "out1.pdf" };
        //    // ModelState.Clear();
        //    return View(vm);
        //}
        [Route("admin/community-validation/dwelling/{id?}", Name = "CVDwelling")]
        public async Task<IActionResult> Dwelling(string id)
        {
            var vm = new HouseholdDwellingViewModel
            {
                SystemCodes = await _context.SystemCodes.Include(c => c.SystemCodeDetails).ToListAsync()
            };

            vm.CVListDetailsId = id;
            // check for Existing Details
            var cvListDetails = _context.CvListDetails.Find(vm.CVListDetailsId);
            if (cvListDetails != null)
            {
                var cvHouseholdId = cvListDetails.CVHouseHoldId;
                ViewBag.HouseHoldId = cvListDetails.HouseholdId;
                var household = _context.HouseholdRegs.SingleOrDefault(l => l.Id == cvHouseholdId);
                if (household != null)
                {
                    ViewBag.HouseHoldCaptured = true;
                    var xx = _context.HouseholdRegCharacteristics.SingleOrDefault(x => x.HouseholdId == cvHouseholdId);
                    new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(xx, vm);
                }
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("admin/community-validation/dwelling/{id?}", Name = "CVDwellingSave")]
        public async Task<IActionResult> Dwelling(HouseholdDwellingViewModel vm)
        {
            // Update CV CVListDetails
            var cvListDetails = _context.CvListDetails.Find(vm.CVListDetailsId);
            cvListDetails.StatusId = 1; // Incomplete
            var userId = User.GetUserId();
            var parent = _context.HouseholdRegs.Find(cvListDetails.HouseholdId);
            var household = new HouseholdReg
            {
                Id = Guid.NewGuid().ToString().ToLower(),
                ParentId = parent.Id,
                VillageId = parent.VillageId,
                HealthFacilityId = parent.HealthFacilityId,
                CreatedById = userId,
                DateCreated = DateTime.UtcNow.AddHours(3),
                TypeId = 2,
                StatusId = 1 // Incomplete
            };

            _context.Add(household);
            cvListDetails.CVHouseHoldId = household.Id;
            await _context.SaveChangesAsync();
            var xtics = new HouseholdRegCharacteristic();
            new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, xtics);
            xtics.HouseholdId = household.Id;
            _context.Add(xtics);
            await _context.SaveChangesAsync();

            // Get mothers details from registration Table

            var motherId = _context.HouseholdRegs.Find(cvListDetails.HouseholdId).MotherId;
            var mother = _context.HouseholdRegMembers.Find(motherId);
            var cvMother = new HouseholdRegMember
            {
                Id = Guid.NewGuid().ToString().ToLower(),
                HouseholdId = household.Id,
                FirstName = mother.FirstName,
                MiddleName = mother.MiddleName,
                Surname = mother.Surname,
                IdNumber = mother.IdNumber,
                IdentificationFormId = mother.IdentificationFormId,
                DOB = mother.DOB,
                MaritalStatusId = mother.MaritalStatusId,
                GenderId = 192
            };

            _context.Add(cvMother);

            if (vm.OtherSPProgrammes != null)
            {
                ICollection<HouseholdRegOtherProgramme> programmes = new List<HouseholdRegOtherProgramme>();
                foreach (var id in vm.OtherSPProgrammes)
                {
                    var program = new HouseholdRegOtherProgramme
                    {
                        HouseholdId = household.Id,
                        OtherProgrammeId = id
                    };
                    programmes.Add(program);
                }
                _context.HouseholdRegOtherProgrammes.AddRange(programmes);
            }
            await _context.SaveChangesAsync();
            if (vm.HouseholdAssets != null)
            {
                ICollection<HouseholdRegAsset> assets = new List<HouseholdRegAsset>();
                var possibleAssets = _context.SystemCodeDetails.Where(s => s.SystemCode.Code == "Household Assets")
                    .OrderBy(i => i.OrderNo);
                foreach (var item in possibleAssets)
                {
                    var asset = new HouseholdRegAsset
                    {
                        HouseholdId = household.Id,
                        AssetId = item.Id,
                        AssetTypeId = 203,
                        HasItem = vm.HouseholdAssets.Contains(item.Id)
                    };
                    assets.Add(asset);
                }
                _context.HouseholdRegAssets.AddRange(assets);
            }
            await _context.SaveChangesAsync();
            vm.HouseholdLivestock.ForEach(i => i.HasItem = true);
            vm.HouseholdLivestock.ForEach(i => i.HouseholdId = household.Id);
            vm.HouseholdLivestock.ForEach(i => i.AssetTypeId = 204);
            household.MotherId = cvMother.Id;
            _context.HouseholdRegAssets.AddRange(vm.HouseholdLivestock);

            await _context.SaveChangesAsync();
            TempData["success"] = "Dwelling and household information saved.";
            return RedirectToAction("Demographics", new { id = vm.CVListDetailsId });
        }

        [Route("admin/community-validation/demographics/{id?}", Name = "CVDemographics")]
        public async Task<IActionResult> Demographics(string id)
        {
            var cvHouseHoldId = _context.CvListDetails.Single(i => i.Id == id).CVHouseHoldId;
            var members = await _context.HouseholdRegMembers
                .Include(i => i.MaritalStatus)
                .Include(i => i.Relationship)
                .Include(i => i.Gender)
                .Include(i => i.IdentificationForm)
                .OrderBy(c => c.CreateOn)
                .Where(c => c.HouseholdId == cvHouseHoldId).ToListAsync();

            ViewBag.CVListDetailsId = id;
            ViewBag.HouseHoldId = cvHouseHoldId;
            var houseHold = _context.HouseholdRegs.Find(cvHouseHoldId);
            if (houseHold != null)
                ViewBag.StatusId = houseHold.StatusId;
            return View(members);
        }

        [Route("admin/community-validation/add-member/{householdId}/{id?}", Name = "CVAddMemberRoute")]
        public async Task<IActionResult> AddMember(string householdId, string id)
        {
            ViewBag.HouseHoldId = householdId;

            var member = _context.HouseholdRegMembers.FirstOrDefault(m => m.HouseholdId == householdId);
            var vm = new HouseholdRegMemberViewModel();
            vm.HouseholdId = householdId;

            var systemCodeDetails = _context.SystemCodeDetails.AsNoTracking().OrderBy(i => i.OrderNo)
                .Include(s => s.SystemCode).ToList();

            ViewData["IdentificationFormId"] =
                new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Identification Documents"), "Id",
                    "DisplayName");

            if (string.IsNullOrEmpty(id) && member != null && member.RelationshipId == null)
            {
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(member, vm);
                vm.IsMother = true;
                vm.Id = member.Id;
            }
            if (!string.IsNullOrEmpty(id))
            {
                member = _context.HouseholdRegMembers.FirstOrDefault(m => m.Id == id);
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(member, vm);
                var motherId = _context.HouseholdRegs.Find(member.HouseholdId).MotherId;
                if (member.Id == motherId)
                {
                    vm.IsMother = true;
                }
                vm.Id = member.Id;
            }
            ViewData["IdentificationFormId"] =
                new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Identification Documents"), "Id",
                    "DisplayName", vm.IdNumber);

            ViewData["GenderId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Gender"), "Id",
                "DisplayName", vm.GenderId);
            ViewData["MaritalStatusId"] =
                new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Marital Status"), "Id", "DisplayName",
                    vm.MaritalStatusId);
            ViewData["RelationshipId"] =
                new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Relationship"), "Id", "DisplayName",
                    vm.RelationshipId);

            var booleanOptions = systemCodeDetails.Where(i => i.SystemCode.Code == "Boolean Options");
            ViewData["SpouseInHouseholdId"] = new SelectList(booleanOptions, "Id", "DisplayName", vm.SpouseInHousehold);
            ViewData["FatherAliveId"] = new SelectList(booleanOptions, "Id", "DisplayName", vm.FatherAliveId);
            ViewData["MotherAliveId"] = new SelectList(booleanOptions, "Id", "DisplayName", vm.MotherAliveId);
            ViewData["ChronicIllnessId"] = new SelectList(booleanOptions, "Id", "DisplayName", vm.ChronicIllnessId);
            ViewData["DisabilityTypeId"] =
                new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Disability"), "Id", "DisplayName");
            ViewData["DisabilityRequires24HrCareId"] =
                new SelectList(booleanOptions, "Id", "DisplayName", vm.DisabilityRequires24HrCareId);
            ViewData["SupportStatusId"] =
                new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "HIV Status"), "Id", "DisplayName",
                    vm.RelationshipId);

            ViewData["DisabilityCaregiverId"] =
                new SelectList(_context.HouseholdRegMembers.Where(i => i.HouseholdId == householdId), "Id", "FullName",
                    vm.DisabilityCaregiverId);
            ViewData["EducationAttendanceId"] =
                new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Education Attendance"), "Id",
                    "DisplayName", vm.EducationAttendanceId);
            ViewData["EducationLevelId"] =
                new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Education Level"), "Id",
                    "DisplayName", vm?.EducationAttendanceId);
            ViewData["OccupationTypeId"] =
                new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Work Type"), "Id", "DisplayName",
                    vm.OccupationTypeId);
            ViewData["FormalJobTypeId"] = new SelectList(booleanOptions, "Id", "DisplayName", vm.FormalJobTypeId);
            ViewData["SchoolTypeId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "School Type"),
                "Id", "DisplayName", vm.SchoolTypeId);

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
                member.CreateOn = DateTime.UtcNow.AddHours(3);
                member.HouseholdId = vm.HouseholdId;
                if (!string.IsNullOrEmpty(vm.Id))
                {
                    _context.Update(member);
                    vm.IsMother = true;
                }
                else
                {
                    _context.Add(member);
                }
                _context.SaveChanges();
                // Add Disability Types
                // Remove previous
                _context.HouseholdRegMemberDisabilities
                    .RemoveRange(
                        _context.HouseholdRegMemberDisabilities.Where(i => i.HouseholdRegMemberId == member.Id));
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

                var parentId = _context.HouseholdRegs.Find(id).ParentId;
                var cvListDetails = _context.CvListDetails.First(l => l.HouseholdId == parentId);
                return RedirectToAction(nameof(Demographics), new { id = cvListDetails.Id });
            }
            var systemCodeDetails = _context.SystemCodeDetails.OrderBy(i => i.OrderNo).Include(s => s.SystemCode)
                .ToList();
            ViewData["GenderId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Gender"), "Id",
                "DisplayName", vm.GenderId);
            ViewData["MaritalStatusId"] =
                new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Marital Status"), "Id", "DisplayName",
                    vm.MaritalStatusId);
            ViewData["RelationshipId"] =
                new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Relationship"), "Id", "DisplayName",
                    vm.RelationshipId);
            ViewData["IdentificationFormId"] =
                new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Identification Documents"), "Id",
                    "DisplayName", vm.IdentificationFormId);
            var booleanOptions = systemCodeDetails.Where(i => i.SystemCode.Code == "Boolean Options");
            ViewData["SpouseInHouseholdId"] = new SelectList(booleanOptions, "Id", "DisplayName");
            ViewData["FatherAliveId"] = new SelectList(booleanOptions, "Id", "DisplayName");
            ViewData["MotherAliveId"] = new SelectList(booleanOptions, "Id", "DisplayName");
            ViewData["ChronicIllnessId"] = new SelectList(booleanOptions, "Id", "DisplayName");
            ViewData["DisabilityTypeId"] =
                new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Disability"), "Id", "DisplayName");
            ViewData["DisabilityRequires24HrCareId"] = new SelectList(booleanOptions, "Id", "DisplayName");

            ViewData["DisabilityCaregiverId"] =
                new SelectList(_context.HouseholdRegMembers.Where(i => i.HouseholdId == id), "Id", "FullName");
            ViewData["EducationAttendanceId"] =
                new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Education Attendance"), "Id",
                    "DisplayName");
            ViewData["EducationLevelId"] =
                new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Education Level"), "Id",
                    "DisplayName");
            ViewData["OccupationTypeId"] =
                new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Work Type"), "Id", "DisplayName");
            ViewData["FormalJobTypeId"] = new SelectList(booleanOptions, "Id", "DisplayName");
            ViewData["SchoolTypeId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "School Type"),
                "Id", "DisplayName");
            return View("AddMember", vm);
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
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // Obsolete for now as this is now handled via mobile app
        public async Task<ActionResult> SaveHousehold(string id)
        {
            var cvListDetail = _context.CvListDetails.FirstOrDefault(l => l.CVHouseHoldId == id);
            // Check if household head is already saved
            var head = _context.HouseholdRegMembers.FirstOrDefault(i => i.HouseholdId == id && i.RelationshipId == 112);
            if (head == null)
            {
                TempData["Info"] = "Household head information missing. Add the Household head details before saving.";
                var parentId = _context.HouseholdRegs.Find(id).ParentId;
                return RedirectToAction(nameof(Demographics), new { id = cvListDetail.Id });
            }
            var houseHold = _context.HouseholdRegs.Find(id);
            houseHold.StatusId = 2;
            // Update CVListDetails

            if (cvListDetail != null)
            {
                cvListDetail.StatusId = 2; // Complete
                // Update Captured
                var list = _context.CVLists.Single(c => c.Id == cvListDetail.ListId);
                list.Captured = list.Captured + 1;

                // Calculate PMT Score

                // Calculate PMT Score
                SqlParameter[] @params =
                {
                    new SqlParameter("HouseholdId", id),
                    new SqlParameter("@PMTValue", SqlDbType.Int) {Direction = ParameterDirection.Output},
                };
                await _context.Database.ExecuteSqlCommandAsync("exec PMT_Generate  @HouseholdId"
                    , @params);
                var result = @params[1].Value;

                //var householdId = new SqlParameter("HouseholdId", id);

                //var houseHoldReturned = _context.HouseholdRegs.FromSql("EXECUTE PMT_Generate @HouseholdId", householdId).First();
                //var houseHoldReturned1 = _context.HouseholdRegs.FromSql("select * from HouseholdRegs where id='77af5cac-2c56-402c-8e99-e68358f587f4'").First();
                //var rowsAffected = _context.Database.ExecuteSqlCommand("PMT_Generate  @HouseholdId",
                //    new SqlParameter("HouseholdId", id)
                //);

                //await _context.SaveChangesAsync();
                // Calculate Variance
                //houseHold = _context.HouseholdRegs.Single(i => i.Id == id);
                //var parent = _context.HouseholdRegs.Find(houseHold.ParentId);
                //var originalRecord = _context.HouseholdRegs.Find(parent.Id);
                //var initialPmtScore = parent.PMTScore;
                //var pmtScore = houseHold.PMTScore;
                //float? variance = Math.Abs((float)((initialPmtScore - pmtScore) / initialPmtScore) * 100);
                //cvListDetail.Variance = (float)variance;
                //if (variance <= 50)
                //{
                //    parent.StatusId = 10;
                //    originalRecord.StatusId = 10;
                //    cvListDetail.VarianceCategoryId = 210;
                //}
                //else if (variance > 50 && variance < 80)
                //{
                //    parent.StatusId = 11;
                //    originalRecord.StatusId = 11;
                //    cvListDetail.VarianceCategoryId = 209;
                //}
                //else
                //{
                //    parent.StatusId = 12;
                //    originalRecord.StatusId = 12;
                //    cvListDetail.VarianceCategoryId = 208;
                //}
                // TempData["Success"] = "Household information saved.";
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("CalculatePMT", new { id = id });
        }

        [Route("admin/community-validation/send-for-approval/{id}")]
        public IActionResult SendForApproval(CommunityValidationApprovalsViewModel vm)
        {
            vm.List = _context.CVLists
                .Include(l => l.CreatedBy)
                .SingleOrDefault(l => l.Id == vm.Id);
            return View(vm);
        }

        [Route("admin/community-validation/send/{id}")]
        [HttpPost]
        public async Task<ActionResult> Send(CommunityValidationApprovalsViewModel vm)
        {
            var list = _context.CVLists
                .Include(l => l.CreatedBy)
                .SingleOrDefault(l => l.Id == vm.Id);
            list.StatusId = 2;
            await _context.SaveChangesAsync();

            if (list.ListTypeId == 211 || list.ListTypeId == 357)
            {
                TempData["Message"] = "Community Validation List sent for approval";
                return RedirectToAction("Index");
            }
            TempData["Message"] = "M&E Validation List sent for approval";
            return RedirectToAction("MonitoringEvaluation");
        }

        [Route("admin/community-validation/action/{id}")]
        public IActionResult Action(CommunityValidationApprovalsViewModel vm)
        {
            vm.List = _context.CVLists
                .Include(l => l.CreatedBy)
                .SingleOrDefault(l => l.Id == vm.Id);
            return View(vm);
        }

        [Route("admin/community-validation/action-save/{id}")]
        [HttpPost]
        public async Task<ActionResult> ActionSave(CommunityValidationApprovalsViewModel vm)
        {
            var list = _context.CVLists
                .Include(l => l.CreatedBy)
                .SingleOrDefault(l => l.Id == vm.Id);
            list.StatusId = vm.Action;
            list.ApprovalNotes = vm.Notes;
            list.DateApproved = DateTime.UtcNow.AddHours(3);
            list.ApprovedById = User.GetUserId();
            await _context.SaveChangesAsync();
            if (list.ListTypeId == 211 || list.ListTypeId == 357)
            {
                TempData["Message"] = "Community Validation List Approved";
                // Send SMS to the Mothers
                var listDetails = _context.CvListDetails.Include(i => i.Household.Mother).Where(i => i.ListId == vm.Id);
                var smses = _context.SMS.OrderBy(i => i.Order).Where(i => i.TriggerEvent == "COMMUNITY-VALIDATION").ToList();
                foreach (var item in listDetails)
                {
                    foreach (var sms in smses)
                    {
                        _smsService.Send(item.Household.Phone, sms.Message.Replace("##NAME##", item.Household.CommonName ?? item.Household.Mother.FirstName));
                    }
                }

                return RedirectToAction("Index");
            }
            TempData["Message"] = "M&E Validation List Approved";
            return RedirectToAction("MonitoringEvaluation");
        }

        public ActionResult GenerateMEForms()
        {
            var householdsCount = _context.HouseholdRegs.Count(i => i.StatusId == 11 && i.TypeId == 1);

            ViewBag.Count = householdsCount;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GenerateForms()
        {
            var households = _context.HouseholdRegs.Where(i => i.StatusId == 11 && i.TypeId == 1);

            if (!households.Any())
            {
                TempData["Info"] = "No households found for Monitoring and Evaluation.";
                return RedirectToAction("MonitoringEvaluation");
            }
            var userId = User.GetUserId();
            var user = _context.Users.Find(userId);
            var list = new CVList
            {
                DateCreated = DateTime.UtcNow.AddHours(3),
                CreatedById = user.Id,
                StatusId = 1,
                ListTypeId = 212
            };
            _context.Add(list);
            await _context.SaveChangesAsync();

            _context.Database.ExecuteSqlCommand(";Exec GenerateMEList @listId",
                new SqlParameter("listId", list.Id)
            );

            //var details = new List<CVListDetail>();
            //foreach (var household in households)
            //{
            //    var detail = new CVListDetail
            //    {
            //        Id = Guid.NewGuid().ToString().ToLower(),
            //        ListId = list.Id,
            //        HouseholdId = household.Id,
            //        StatusId = 9 // Pending
            //    };
            //    details.Add(detail);
            //}
            //_context.CvListDetails.AddRange(details);
            //await households.ForEachAsync(i => i.StatusId = 11); //Recommended for M&E Validation
            //list.Households = details.Count;
            //await _context.SaveChangesAsync();

            TempData["Message"] = "List generated successfully.";
            return RedirectToAction("Details", new { id = list.Id });
        }

        public async Task<IActionResult> Captured(string id, string view)
        {
            if (id == null)
            {
                return NotFound();
            }
            var detail = _context.CvListDetails
                .Include(i => i.InterviewResult)
                .Include(i => i.InterviewStatus)
                .Include(i => i.CannotFindHouseholdReason)
                .Single(i => i.Id == id);

            var houseHoldId = detail.CVHouseHoldId;
            if (houseHoldId == null) // Special category mothers
            {
                return RedirectToAction(nameof(Details), nameof(Registration), new { id = detail.HouseholdId });
            }

            var houseHold = _context.HouseholdRegs
                .Include(i => i.HealthFacility)
                .Include(i => i.Status)
                .Include(i => i.Mother)
                .Include(i => i.Village.Ward.SubCounty.County)
                .Include(i => i.SubLocation.Location)
                .SingleOrDefault(h => h.Id == houseHoldId);

            var parent = _context.HouseholdRegs
                .Include(i => i.HealthFacility)
                .Include(i => i.Status)
                .Include(i => i.Mother)
                .Include(i => i.Village.Ward.SubCounty.County)
                .Include(i => i.SubLocation.Location)
                .SingleOrDefault(h => h.Id == detail.HouseholdId);

            var vm = new HouseholdDetailsViewModel();
            vm.ParentHousehold = parent;
            vm.View = view;
            vm.Id = id;
            vm.Detail = detail;
            vm.StatusId = detail.StatusId;
            var members = await _context.HouseholdRegMembers
                .Include(i => i.MaritalStatus)
                .Include(i => i.Relationship)
                .Include(i => i.Relationship)
                .Include(i => i.Gender)
                .Include(i => i.IdentificationForm)
                .OrderBy(c => c.CreateOn)
                .Where(c => c.HouseholdId == houseHoldId).ToListAsync();

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
                .SingleOrDefault(i => i.HouseholdId == houseHoldId);
            vm.Assets = await _context.HouseholdRegAssets
                .Include(i => i.Asset)
                .OrderBy(i => i.Asset.OrderNo)
                .Where(i => i.HouseholdId == houseHoldId).ToListAsync();
            vm.OtherProgrammes = await _context.HouseholdRegOtherProgrammes
                .Include(i => i.OtherProgramme)
                .Where(i => i.HouseholdId == houseHoldId).ToListAsync();

            // vm.SupportStatus = _context.HouseholdRegMembers.Include(m => m.SupportStatus).Single(i => i.Id == houseHold.MotherId).SupportStatus;
            vm.Disabilities = _context.HouseholdRegMemberDisabilities.Include(c => c.Disability)
                .Where(i => i.HouseholdRegMemberId == houseHold.Id)
                .ToList();

            return View(vm);
        }

        public async Task<IActionResult> Compare(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.Id = id;
            var detail = _context.CvListDetails
                .Include(i => i.InterviewResult)
                .Include(i => i.InterviewStatus)
                .Include(i => i.VarianceCategory)
                .Include(i => i.List)
                .Single(i => i.Id == id);

            var houseHoldId = detail.CVHouseHoldId;
            if (houseHoldId == null)
            {
                return RedirectToAction(nameof(Details), nameof(Registration), new { id = detail.HouseholdId });
            }

            var houseHold = _context.HouseholdRegs
                .Include(i => i.HealthFacility)
                .Include(i => i.Status)
                .Include(i => i.Mother)
                .Include(i => i.Village.Ward.SubCounty.County)
                .Include(i => i.SubLocation.Location)
                .SingleOrDefault(h => h.Id == houseHoldId);

            var parent = _context.HouseholdRegs
                .Include(i => i.HealthFacility)
                .Include(i => i.Status)
                .Include(i => i.Mother)
                .Include(i => i.Village.Ward.SubCounty.County)
                .Include(i => i.SubLocation.Location)
                .SingleOrDefault(h => h.Id == detail.HouseholdId);

            var vm = new HouseholdDetailsViewModel();
            vm.ParentHousehold = parent;

            vm.Id = id;
            vm.Detail = detail;
            vm.StatusId = detail.StatusId;
            var members = await _context.HouseholdRegMembers
                .Include(i => i.MaritalStatus)
                .Include(i => i.Relationship)
                .Include(i => i.Relationship)
                .Include(i => i.Gender)
                .Include(i => i.IdentificationForm)
                .OrderBy(c => c.CreateOn)
                .Where(c => c.HouseholdId == houseHoldId).ToListAsync();
            vm.Members = members;
            var parentMembers = await _context.HouseholdRegMembers
               .Include(i => i.MaritalStatus)
               .Include(i => i.Relationship)
               .Include(i => i.Relationship)
               .Include(i => i.Gender)
               .Include(i => i.IdentificationForm)
               .OrderBy(c => c.CreateOn)
               .Where(c => c.HouseholdId == parent.Id).ToListAsync();
            vm.ParentMembers = parentMembers;
            vm.Household = houseHold;

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
                .SingleOrDefault(i => i.HouseholdId == houseHoldId);

            vm.ParentCharacteristic = _context.HouseholdRegCharacteristics
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
               .SingleOrDefault(i => i.HouseholdId == parent.Id);

            vm.Assets = await _context.HouseholdRegAssets
                .Include(i => i.Asset)
                .OrderBy(i => i.Asset.OrderNo)
                .Where(i => i.HouseholdId == houseHoldId).ToListAsync();

            vm.ParentAssets = await _context.HouseholdRegAssets
              .Include(i => i.Asset)
              .OrderBy(i => i.Asset.OrderNo)
              .Where(i => i.HouseholdId == parent.Id).ToListAsync();

            vm.OtherProgrammes = await _context.HouseholdRegOtherProgrammes
                .Include(i => i.OtherProgramme)
                .Where(i => i.HouseholdId == houseHoldId).ToListAsync();

            vm.ParentOtherProgrammes = await _context.HouseholdRegOtherProgrammes
              .Include(i => i.OtherProgramme)
              .Where(i => i.HouseholdId == parent.Id).ToListAsync();

            // vm.SupportStatus = _context.HouseholdRegMembers.Include(m => m.SupportStatus).Single(i => i.Id == houseHold.MotherId).SupportStatus;
            vm.Disabilities = _context.HouseholdRegMemberDisabilities.Include(c => c.Disability)
                .Where(i => i.HouseholdRegMemberId == houseHold.Id)
                .ToList();

            vm.ParentDisabilities = _context.HouseholdRegMemberDisabilities.Include(c => c.Disability)
              .Where(i => i.HouseholdRegMemberId == parent.Id)
              .ToList();

            return View(vm);
        }

        public IActionResult CompareExport(string id)
        {
            var parentId = _context.CvListDetails.Find(id).HouseholdId;
            var motherName = _context.HouseholdRegMembers.Single(i => i.Id == _context.HouseholdRegs.Single(x => x.Id == parentId).MotherId).FullName;
            var reportTitle = "Community Validation Data Comparison - " + motherName;
            var url = "CommunityValidation/Compare?id=" + id;
            var file = _exportService.ExportToPDF(url);
            return File(file, "application/pdf", reportTitle.Replace(" ", "-") + ".pdf");
        }


        [HttpPost]
        public async Task<IActionResult> AllowToEnroll(AllowToEnrollViewModel vm)
        {
            var detail = _context.CvListDetails.Find(vm.Id);
            detail.AllowedToEnroll = true;
            detail.AllowedReason = vm.AllowedReason;
            var household = _context.HouseholdRegs.Find(detail.HouseholdId);
            household.StatusId = 10;// Ready for enrollment
            _uow.GetRepository<HouseholdReg>().Update(household);
            _uow.GetRepository<CVListDetail>().Update(detail);
            _uow.Save();
            TempData["Message"] = "Record Saved successfully";
            if (vm.ListTypeId == 212)
                return RedirectToAction("MonitoringEvaluation");
            TempData["VarianceCategoryId"] = 208;
            return RedirectToAction("Index");
        }
      
        [HttpPost]
        public async Task<IActionResult> Approve(ApprovalViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var detail = _context.CvListDetails.Include(i => i.Household.Mother).Single(i => i.Id == vm.Id);
                var id = detail.CVHouseHoldId;
                detail.StatusId = vm.StatusId;
                detail.ActionedById = User.GetUserId();
                detail.DateActioned = DateTime.UtcNow.AddHours(3);
                detail.Notes = vm.Notes;
                _uow.GetRepository<CVListDetail>().Update(detail);
                _uow.Save();
                TempData["Info"] = "Household data " + (vm.StatusId == 4 ? "approved" : "rejected");

                if (vm.StatusId == 4) // Approved
                {
                    // Check if Household characteristics are available
                    var xtics = _context.HouseholdRegCharacteristics.SingleOrDefault(i =>
                        i.HouseholdId == detail.CVHouseHoldId);
                    var parent = _context.HouseholdRegs.Find(detail.HouseholdId);
                    var household = _context.HouseholdRegs.Find(detail.CVHouseHoldId);
                    if (xtics == null)
                    {
                        
                        household.PMTScore = null;
                        detail.Variance = null;
                        detail.VarianceCategoryId = null;
                        household.StatusId = 26;// Household Cannot be Found

                        //Update Parent
                      
                        parent.StatusId = 26;// Household Cannot be Found
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Details", new { id = detail.ListId });
                    }
                    else
                    {
                        // Calculate PMT Score

                        SqlParameter[] @params =
                        {
                            new SqlParameter("HouseholdId", id),
                            new SqlParameter("@PMTValue", SqlDbType.Int) {Direction = ParameterDirection.Output},
                        };
                        await _context.Database.ExecuteSqlCommandAsync("exec PMT_Generate  @HouseholdId"
                            , @params);
                        var result = @params[1].Value;

                        // Send SMS
                        var smses = _context.SMS.OrderBy(i => i.Order).Where(i => i.TriggerEvent == "CV-ACCEPTED").ToList();
                        foreach (var sms in smses)
                        {
                            _smsService.Send(detail.Household.Phone, sms.Message.Replace("##NAME##", detail.Household.CommonName ?? detail.Household.Mother.FirstName));
                        }
                    }
                }
                await _context.SaveChangesAsync();

                return RedirectToAction("CalculatePMT", new { id = id });
                // return RedirectToAction(nameof(Details), new {id= detail.ListId });
            }

            return RedirectToAction(nameof(Captured), new { vm.Id });
        }

        public async Task<ActionResult> CalculatePMT(string id)
        {
            var cvListDetail = _context.CvListDetails.Include(i => i.List)
                .Include(i => i.Household.Mother).First(l => l.CVHouseHoldId == id);
            // Calculate Variance
            var houseHold = _context.HouseholdRegs.Single(i => i.Id == id);
            var parent = _context.HouseholdRegs.Find(houseHold.ParentId);
            var originalRecord = _context.HouseholdRegs.Find(parent.Id);
            var initialPmtScore = parent.PMTScore;
            var pmtScore = houseHold.PMTScore;
            parent.PMTScoreFinal = pmtScore;

            if (cvListDetail.List.ListTypeId==212) // M&E
            {
                // check for eligibility
                var pmtCutoff = decimal.Parse(_context.SystemSettings.Single(i => i.key == "PMT.CUTOFF").Value);
                parent.StatusId= originalRecord.StatusId = pmtScore <= pmtCutoff ? 10 : 29; // Ready for Enrolment, Ineligible
                parent.PMTCutOffUsed = originalRecord.PMTCutOffUsed = pmtCutoff;
             
            }

            if (cvListDetail.List.ListTypeId == 211 || cvListDetail.List.ListTypeId == 357) // CHV; For M&E, no need to calculate variance.
            {
                float? variance = Math.Abs((float)((initialPmtScore - pmtScore) / initialPmtScore) * 100);

                cvListDetail.Variance = (float)variance;
                if (variance <= 50) // Low variance, there accept mothers
                {
                    parent.StatusId = 10; // Ready for Enrolment;
                    originalRecord.StatusId = 10;
                    cvListDetail.VarianceCategoryId = 210;
                }
                else if (variance > 50 && variance < 80) // Recommend for M&E Evaluation
                {
                    parent.StatusId = 11;
                    originalRecord.StatusId = 11;
                    cvListDetail.VarianceCategoryId = 209;

                    var smses = _context.SMS.OrderBy(i => i.Order).Where(i => i.TriggerEvent == "RECOMMENDED.FOR.M&E").ToList();
                    foreach (var sms in smses)
                    {
                        _smsService.Send(cvListDetail.Household.Phone,
                            sms.Message.Replace("##NAME##", cvListDetail.Household.CommonName ?? cvListDetail.Household.Mother.FullName));
                    }
                }
                else // Rejected
                {
                    parent.StatusId = 12;
                    originalRecord.StatusId = 12;
                    cvListDetail.VarianceCategoryId = 208;

                    var smses = _context.SMS.OrderBy(i => i.Order).Where(i => i.TriggerEvent == "ENROLMENT-UNSUCCESSFUL").ToList();
                    foreach (var sms in smses)
                    {
                        _smsService.Send(cvListDetail.Household.Phone,
                            sms.Message.Replace("##NAME##", cvListDetail.Household.CommonName ?? cvListDetail.Household.Mother.FirstName));
                    }
                }
            }

            TempData["Success"] = "Household information saved.";

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = cvListDetail.ListId });
        }

        public void CalculateVariance(string id)
        {
            var cvListDetail = _context.CvListDetails.Include(i => i.List)
                .Include(i => i.Household.Mother).FirstOrDefault(l => l.CVHouseHoldId == id);

            // Calculate Variance
            var houseHold = _context.HouseholdRegs.Single(i => i.Id == id);
            var parent = _context.HouseholdRegs.Find(houseHold.ParentId);
            var originalRecord = _context.HouseholdRegs.Find(parent.Id);
            var initialPmtScore = parent.PMTScore;
            var pmtScore = houseHold.PMTScore;
            parent.PMTScoreFinal = pmtScore;

            if (cvListDetail.List.ListTypeId == 211 || cvListDetail.List.ListTypeId == 357) // CHV; For M&E, no need to calculate variance.
            {
                float? variance = Math.Abs((float)((initialPmtScore - pmtScore) / initialPmtScore) * 100);

                cvListDetail.Variance = (float)variance;
                if (variance <= 50) // Low variance, there accept mothers
                {
                    parent.StatusId = 10; //Ready for Enrolment
                    originalRecord.StatusId = 10;
                    cvListDetail.VarianceCategoryId = 210;
                }
                else if (variance > 50 && variance < 80) // Recommend for M&E Evaluation
                {
                    parent.StatusId = 11; // Recommended for M&E Validation
                    originalRecord.StatusId = 11;
                    cvListDetail.VarianceCategoryId = 209;

                    var smses = _context.SMS.OrderBy(i => i.Order).Where(i => i.TriggerEvent == "RECOMMENDED.FOR.M&E").ToList();
                    foreach (var sms in smses)
                    {
                        _smsService.Send(cvListDetail.Household.Phone,
                            sms.Message.Replace("##NAME##", cvListDetail.Household.CommonName ?? cvListDetail.Household.Mother.FullName));
                    }
                }
                else // Rejected
                {
                    parent.StatusId = 12;
                    originalRecord.StatusId = 12;
                    cvListDetail.VarianceCategoryId = 208;

                    var smses = _context.SMS.OrderBy(i => i.Order).Where(i => i.TriggerEvent == "ENROLMENT-UNSUCCESSFUL").ToList();
                    foreach (var sms in smses)
                    {
                        _smsService.Send(cvListDetail.Household.Phone,
                            sms.Message.Replace("##NAME##", cvListDetail.Household.CommonName ?? cvListDetail.Household.Mother.FirstName));
                    }
                }
            }

            TempData["Success"] = "Household information saved.";

            _context.SaveChanges();
        }
    }
}