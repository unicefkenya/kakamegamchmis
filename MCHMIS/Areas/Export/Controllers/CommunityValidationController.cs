using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MCHMIS.Areas.Reports.ViewModels;
using MCHMIS.Data;
using MCHMIS.Interfaces;
using MCHMIS.Models;
using MCHMIS.Services;
using MCHMIS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using CommunityValidationListViewModel = MCHMIS.Areas.Reports.ViewModels.CommunityValidationListViewModel;

namespace MCHMIS.Areas.Export.Controllers
{
    [Area("Export")]
    [AllowAnonymous]
    public class CommunityValidationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IUnitOfWork _uow;
        private readonly IExportService _exportService;
        private readonly IDBService _dbService;

        public CommunityValidationController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment
            , IUnitOfWork uow
            , IExportService exportService
            , IDBService dbService)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _uow = uow;
            _dbService = dbService;
            _exportService = exportService;
        }

        public async Task<IActionResult> Compare(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var detail = _context.CvListDetails
                .Include(i => i.InterviewResult)
                .Include(i => i.InterviewStatus)
                .Single(i => i.Id == id);

            var houseHoldId = detail.CVHouseHoldId;

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
    }
}