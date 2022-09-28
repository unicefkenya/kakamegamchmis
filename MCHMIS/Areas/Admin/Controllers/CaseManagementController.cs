using System;
using System.Collections.Generic;
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

namespace MCHMIS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CaseManagementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _uow;
        private readonly IDBService _dbService;
        private readonly ISMSService _smsService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public CaseManagementController(ApplicationDbContext context, IUnitOfWork uow, IDBService dbService,
            IHostingEnvironment hostingEnvironment,
            ISMSService smsService)
        {
            _context = context;
            _uow = uow;
            _dbService = dbService;
            _smsService = smsService;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Admin/CaseManagements
        public async Task<IActionResult> Index(CaseManagementListViewModel vm)
        {
            var pregnancies = _context.Pregnancies
                .OrderByDescending(c => c.LastVisit)
               // .ThenBy(c => c.NextVisit)
                .Include(c => c.CaseManagement.Household.Village.Ward.SubCounty)
                .Include(c => c.CaseManagement.Household.Mother)
                .Include(c => c.CreatedBy)
                .Include(c => c.Status)
                .Include(c => c.CaseManagement.Household.HealthFacility).AsQueryable();
          
            var healthFacilityId = _dbService.GetHealthFacilityId();
            bool isGlobal = await _dbService.IsGlobal();
            if (healthFacilityId != 0)
            {
                pregnancies = pregnancies.Where(i => i.CaseManagement.Household.HealthFacilityId == healthFacilityId || isGlobal);
            }

            if (vm.StatusId != null && vm.StatusId != 0)
            {
                pregnancies = pregnancies.Where(i => i.StatusId == vm.StatusId);
            }
            if (!string.IsNullOrEmpty(vm.UniqueId))
            {
                pregnancies = pregnancies.Where(h => h.CaseManagement.Household.UniqueId == vm.UniqueId);
            }
            if (!string.IsNullOrEmpty(vm.Phone))
            {
                pregnancies = pregnancies.Where(h => h.CaseManagement.Household.Phone == vm.Phone);
            }
            if (!string.IsNullOrEmpty(vm.IdNumber))
            {
                pregnancies = pregnancies.Where(h => h.CaseManagement.Household.Mother.IdNumber.Contains(vm.IdNumber));
            }

            if (!string.IsNullOrEmpty(vm.Name))
            {
                var names = vm.Name.Split(' ');
                foreach (var name in names)
                {
                    pregnancies = pregnancies.Where(h =>
                        h.CaseManagement.Household.Mother.FirstName.Contains(name)
                        || h.CaseManagement.Household.Mother.MiddleName.Contains(name)
                        || h.CaseManagement.Household.Mother.Surname.Contains(name)

                    );
                }
            }

            if (vm.WardId != null)
            {
                pregnancies = pregnancies.Where(h => h.CaseManagement.Household.Village.WardId == vm.WardId);
            }
            if (vm.SubCountyId != null)
            {
                pregnancies = pregnancies.Where(h => h.CaseManagement.Household.Village.Ward.SubCountyId == vm.SubCountyId);
            }

            var page = vm.Page ?? 1;
            var pageSize = vm.PageSize ?? 20;
            vm.Pregnancies = pregnancies.ToPagedList(page, pageSize);
            vm.ReminderOffset = int.Parse(_context.SystemSettings.Single(i => i.key == "CLINIC.VISIT.REMINDER.OFFSET").Value);
            ViewData["StatusId"] = new SelectList(_context.CaseManagementStatus, "Id", "Name", vm.StatusId);
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

                var healthFacilityId = _dbService.GetHealthFacilityId();
                bool isGlobal = await _dbService.IsGlobal();
                if (healthFacilityId != 0)
                {
                    households = households.Where(i => i.HealthFacilityId == healthFacilityId || isGlobal);
                }
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
                vm.HouseholdRegs = await households.ToListAsync();
                if (vm.HouseholdRegs.Count == 1)
                {
                    return RedirectToAction(nameof(Create), new { id = vm.HouseholdRegs.First().Id });
                }
            }

            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Name", vm.StatusId);

            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name", vm.SubCountyId);
            vm.Wards = await _context.Wards.ToListAsync();
            return View(vm);
        }

        public IActionResult Create(string id, string caseManagementId)
        {
            // Check if the mother has an Open case
            if (_context.Pregnancies.Any(i => i.CaseManagement.HouseholdId == id && i.StatusId == 1))
            {
                TempData["Info"] = "Sorry, the mother has an active pregnancy. Use the update button to open the case details.";
                return RedirectToAction(nameof(Index));
            }
            var systemCodeDetails = _context.SystemCodeDetails.Include(i => i.SystemCode)
                .Where(i => i.SystemCode.SystemModule.Name == "Case Management" || i.SystemCode.Code == "Yes No Choices").ToList();
            var vm = new CaseManagementViewModel
            {
                HouseholdId = id,
                Household = _context.HouseholdRegs.Include(i => i.Mother).Single(i => i.Id == id),
                CaseManagementId = caseManagementId,
                Id = caseManagementId
            };
            ViewBag.motherSupportStatusId = vm.Household.Mother.SupportStatusId;
            ViewData["ClinicVisitId"] = new SelectList(_context.ClinicVisits.Where(i => i.VisitType.Code == "ANC visit").ToList(), "Id", "Name");
            ViewData["BloodGroupId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Blood Group"), "Id", "Code");
            ViewData["RhesusId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Rhesus"), "Id", "Code");
            ViewData["SupportStatusId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "HIV Status 2"), "Id", "Code");
            ViewData["InfantFeedingCounselingDoneId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");
            ViewData["BreastfeedingCounselingDoneId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");
            var services = _context.MotherPreventiveServices
                .OrderBy(i => i.PreventiveServiceId)
                .Include(i => i.PreventiveService)
                .Where(i => i.PregnancyData.Pregnancy.CaseManagement.HouseholdId == id);

            ViewData["PreventiveServiceId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Preventive Services"), "Id", "Code");

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CaseManagementViewModel vm)
        {
            vm.Id = Guid.NewGuid().ToString().ToLower();

            if (ModelState.IsValid)
            {
                var household = _context.HouseholdRegs.Include(i => i.Mother).Single(i => i.Id == vm.HouseholdId);

                // Check if the beneficiary has ever had a case
                var testDate = DateTime.UtcNow.AddDays(-7);

                var caseManagement = _context.CaseManagement.SingleOrDefault(i => i.HouseholdId == vm.HouseholdId && i.DateCreated< testDate);

                if (caseManagement == null)
                {
                    caseManagement = new CaseManagement();
                    new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, caseManagement);
                    caseManagement.CreatedById = User.GetUserId();
                    caseManagement.DateCreated = DateTime.UtcNow.AddHours(3);
                    caseManagement.Id = Guid.NewGuid().ToString().ToLower();
                    _uow.GetRepository<CaseManagement>().Add(caseManagement);
                }
                else
                {
                    household.StatusId = 22; //Repeat Case
                }

                // Create Pregnancy Entry
                var pregnancy = new Pregnancy();
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, pregnancy);
                pregnancy.StatusId = 1;
                pregnancy.Id = Guid.NewGuid().ToString().ToLower();
                pregnancy.CaseManagementId = caseManagement.Id;
                pregnancy.CreatedById = User.GetUserId();
                pregnancy.DateCreated = DateTime.UtcNow.AddHours(3);
                pregnancy.LastVisit = vm.VisitDate;
                pregnancy.EDD = household.EDD;
                _context.Add(pregnancy);

                // Create Visit
                var visit = new MotherClinicVisit();
                visit.HouseholdId = vm.HouseholdId;
                visit.HealthFacilityId = _dbService.GetHealthFacilityId();
                visit.Id = Guid.NewGuid().ToString().ToLower();
                visit.DateCreated = DateTime.UtcNow.AddHours(3);
                visit.CreatedById = User.GetUserId();
                visit.VisitDate = vm.VisitDate;
                visit.PregnancyId = pregnancy.Id;
                visit.TypeId = 1;
                visit.ClinicVisitId = vm.ClinicVisitId;
                _uow.GetRepository<MotherClinicVisit>().Add(visit);

                // Record Pregnancy Data
                var pregnancyData = new PregnancyData();
                pregnancyData.Id = Guid.NewGuid().ToString().ToLower();
                pregnancyData.HouseholdId = vm.HouseholdId;
                pregnancyData.PregnancyId = pregnancy.Id;
                pregnancyData.ClinicVisitId = vm.ClinicVisitId;
                pregnancyData.BloodPressure = vm.BloodPressure;
                pregnancyData.Weight = vm.Weight;
                pregnancyData.VisitDate = vm.VisitDate;
                pregnancyData.NextVisit = vm.NextVisit;
                pregnancyData.MotherClinicVisitId = visit.Id;
                pregnancyData.DateCreated = DateTime.UtcNow.AddHours(3);
                pregnancyData.CreatedById = User.GetUserId();
                _uow.GetRepository<PregnancyData>().Add(pregnancyData);

                // Record Preventive services Given
                if (vm.PreventiveServices != null && vm.PreventiveServices.Any())
                {
                    List<MotherPreventiveService> preventiveServices = new List<MotherPreventiveService>();
                    foreach (var id in vm.PreventiveServices)
                    {
                        var service = new MotherPreventiveService
                        {
                            PregnancyDataId = pregnancyData.Id,
                            PreventiveServiceId = id
                        };
                        preventiveServices.Add(service);
                    }
                    _context.AddRange(preventiveServices);
                }

                await _context.SaveChangesAsync();
                var smses = _context.SMS.OrderBy(i => i.Order).Where(i => i.ClinicVisitId == vm.ClinicVisitId && i.TriggerEvent == null).ToList();
                foreach (var sms in smses)
                {
                    _smsService.Send(household.Phone,
                        sms.Message.Replace("##NAME##", household.CommonName ?? household.Mother.FirstName)
                            .Replace("##DATE##", ((DateTime)vm.NextVisit).ToString("dd/MM/yyyy")));
                }

                var clinicVisit = _context.ClinicVisits.Find(vm.ClinicVisitId);
                if (clinicVisit.PaymentPointId != null) //Payment Trigger
                {
                    visit.RequiresFingerPrint = true;
                    _context.SaveChanges();
                    TempData["Message"] = "Pregnancy Updated Saved. Verify fingerprint now.";
                    return RedirectToAction(nameof(FingerPrint), new { id = visit.Id });
                }

                TempData["Message"] = "Pregnancy Data Saved.";
                return RedirectToAction(nameof(Update), new { id = pregnancy.Id });
            }
            var systemCodeDetails = _context.SystemCodeDetails.Include(i => i.SystemCode)
                .Where(i => i.SystemCode.SystemModule.Name == "Case Management" || i.SystemCode.Code == "Yes No Choices").ToList();

            ViewData["ClinicVisitId"] = new SelectList(_context.ClinicVisits.Where(i => i.VisitType.Code == "ANC visit").ToList(), "Id", "Name", vm.ClinicVisitId);
            ViewData["BloodGroupId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Blood Group"), "Id", "Code", vm.BloodGroupId);
            ViewData["RhesusId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Rhesus"), "Id", "Code", vm.RhesusId);
            ViewData["SupportStatusId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "HIV Status 2"), "Id", "Code", vm.SupportStatusId);
            ViewData["InfantFeedingCounselingDoneId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code", vm.InfantFeedingCounselingDoneId);
            ViewData["BreastfeedingCounselingDoneId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code", vm.BreastfeedingCounselingDoneId);
            var services = _context.MotherPreventiveServices
                .OrderBy(i => i.PreventiveServiceId)
                .Include(i => i.PreventiveService)
                .Where(i => i.PregnancyData.Pregnancy.CaseManagement.HouseholdId == vm.HouseholdId);

            var servicesAlreadyGiven = services
                .Select(i => i.PreventiveServiceId).ToList();
            ViewData["PreventiveServiceId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Preventive Services" && !servicesAlreadyGiven.Contains(i.Id)), "Id", "Code");
            ViewBag.motherSupportStatusId = vm.Household.Mother.SupportStatusId;
            return View(vm);
        }

        public IActionResult FingerPrint(string id)
        {
            var userId = User.GetUserId();
            ViewBag.Id = id;
            var motherVisit = _context.MotherClinicVisits.Include(i => i.Pregnancy).SingleOrDefault(i => i.Id == id);
            var householdId = motherVisit.HouseholdId;

            var date = DateTime.UtcNow.AddHours(3);

            var household = _context.HouseholdRegs.Find(householdId);
            var healthFacilityId = household.HealthFacilityId;
            var sql = @"UPDATE FingerPrintVerifications SET IsVerifying=0 where HealthFacilityId= @healthFacilityId and Id<>@id";
            var rowsAffected = _context.Database.ExecuteSqlCommand(sql,
                new SqlParameter("@healthFacilityId", healthFacilityId),
                new SqlParameter("@id", id)
                );
            var verification = _context.FingerPrintVerifications
                .SingleOrDefault(i => i.HealthFacilityId == healthFacilityId && i.Id == id && i.CreatedById == userId);
            if (verification == null)
            {
                verification = new FingerPrintVerification
                {
                    Id = id,
                    HealthFacilityId = healthFacilityId,
                    CreatedById = userId,
                    DateCreated = date,
                    HouseholdId = motherVisit.HouseholdId,
                    IsVerifying = true
                };
                if (_context.FingerPrintVerifications.Any(i => i.Id == id && i.HouseholdId == motherVisit.HouseholdId))
                {
                    _uow.GetRepository<FingerPrintVerification>().Update(verification);
                }
                else
                {
                    _uow.GetRepository<FingerPrintVerification>().Add(verification);
                }

                motherVisit.IsVerifying = true;
                _context.SaveChanges();
                ViewBag.Title = "Verify fingerprint for " + _context.HouseholdRegs
                                    .Include(i => i.Mother).Single(i => i.Id == householdId).Mother.FullName;
                ViewBag.Url = "FingerPrint/" + id;
            }
            else if (verification.Verified == false)
            {
                if (!verification.IsVerifying)
                {
                    verification.IsVerifying = true;
                    verification.Verified = false;
                }
                motherVisit.IsVerifying = true;
                _context.SaveChanges();
                ViewBag.Title = "Verify fingerprint for " + _context.HouseholdRegs
                                    .Include(i => i.Mother).Single(i => i.Id == householdId).Mother.FullName;
                ViewBag.Url = "FingerPrint/" + id;
            }
            else
            {
                motherVisit.IsVerifying = false;
                motherVisit.Verified = true;
                TempData["Info"] = "Fingerprint Verified.";

                // Update Payments if Any

                if (motherVisit.TypeId == 1 || motherVisit.TypeId == 2)
                {
                    var visit = _context.ClinicVisits.Find(motherVisit.ClinicVisitId);

                    if (visit.PaymentPointId != null)
                    {
                        if (!_context.PaymentTransactions.Any(
                                i => i.Beneficiary.HouseholdId == householdId &&
                                     i.PaymentPointId == visit.PaymentPointId)
                            && !_context.BeneficiaryPaymentPoints
                                .Any(i => i.HouseholdId == householdId && i.PaymentPointId == visit.PaymentPointId))
                        {
                            var beneficiaryPaymentPoint = new BeneficiaryPaymentPoint
                            {
                                HouseholdId = householdId,
                                PaymentPointId = (int)visit.PaymentPointId,
                                CreatedById = User.GetUserId(),
                                DateCreated = DateTime.UtcNow.AddHours(3),
                                StatusId = 1
                            };
                            _uow.GetRepository<BeneficiaryPaymentPoint>().Add(beneficiaryPaymentPoint);
                        }
                    }
                }
                else // Delivery
                {
                    // Check Pregnancy Outcome
                    var delivery =
                        _context.Deliveries.Single(i => i.PregnancyId == motherVisit.PregnancyId);
                    if (delivery.PregnancyOutcomeId == 321) // Live birth
                    {
                        if (!_context.PaymentTransactions.Any(
                                i => i.Beneficiary.HouseholdId == householdId &&
                                     i.PaymentPointId == 2)
                            && !_context.BeneficiaryPaymentPoints
                                .Any(i => i.HouseholdId == householdId && i.PaymentPointId == 2))
                        {
                            var beneficiaryPaymentPoint = new BeneficiaryPaymentPoint
                            {
                                HouseholdId = householdId,
                                PaymentPointId = 2,
                                StatusId = 1,
                                CreatedById = User.GetUserId(),
                                DateCreated = DateTime.UtcNow.AddHours(3),
                            };
                            _uow.GetRepository<BeneficiaryPaymentPoint>().Add(beneficiaryPaymentPoint);
                        }
                    }
                    else if (delivery.PregnancyOutcomeId == 322 || delivery.PregnancyOutcomeId == 324) //Stillbirth or Miscarriage
                    {
                        // Check if the household has ever been paid
                        if (!_context.PaymentTransactions.Any(
                                i => i.Beneficiary.HouseholdId == household.Id && i.PaymentPointId == 7) && !_context.BeneficiaryPaymentPoints.Any(i => i.HouseholdId == household.Id && i.PaymentPointId == 7))
                        {
                            var beneficiaryPaymentPoint = new BeneficiaryPaymentPoint
                            {
                                HouseholdId = household.Id,
                                PaymentPointId = 7,
                                CreatedById = User.GetUserId(),
                                DateCreated = DateTime.UtcNow.AddHours(3),
                                StatusId = 1
                            };
                            _uow.GetRepository<BeneficiaryPaymentPoint>().Add(beneficiaryPaymentPoint);

                            beneficiaryPaymentPoint = new BeneficiaryPaymentPoint
                            {
                                HouseholdId = household.Id,
                                PaymentPointId = 8,
                                CreatedById = User.GetUserId(),
                                DateCreated = DateTime.UtcNow.AddHours(3),
                                StatusId = 1
                            };
                            _uow.GetRepository<BeneficiaryPaymentPoint>().Add(beneficiaryPaymentPoint);
                        }

                        // Exit Mother

                        var activePregnancy = _context.Pregnancies.Find(motherVisit.PregnancyId);
                        activePregnancy.StatusId = 2;
                        activePregnancy.ReasonId = 6;
                        activePregnancy.DateExited = DateTime.UtcNow.AddHours(3);
                        _uow.GetRepository<Pregnancy>().Update(activePregnancy);
                    }
                }

                if (motherVisit.ClinicVisitId == 17) // 18th Month; exit the mother
                {
                    var activePregnancy = _context.Pregnancies.Find(motherVisit.PregnancyId);
                    activePregnancy.StatusId = 2;
                    activePregnancy.ReasonId = 5; // Completed Program
                    activePregnancy.DateExited = DateTime.UtcNow.AddHours(3);

                    _uow.GetRepository<Pregnancy>().Update(activePregnancy);
                }
                _uow.Save();

                if (motherVisit.TypeId == 1)//Post Natal
                {
                    return RedirectToAction(nameof(Update), new { id = motherVisit.PregnancyId });
                    // ViewBag.Url = "update/" + motherVisit.CaseManagementId;
                }
                if (motherVisit.TypeId == 2) //Post Natal
                {
                    return RedirectToAction(nameof(PostNatal), new { id = motherVisit.PregnancyId });
                    // ViewBag.Url = "postanatal/" + motherVisit.CaseManagementId;
                }
                return RedirectToAction(nameof(Delivery), new { id = motherVisit.PregnancyId });
                // ViewBag.Url = "delivery/" + motherVisit.CaseManagementId;
            }

            if (motherVisit.TypeId == 1)//Post Natal
            {
                ViewBag.Back = "Update";
            }
            else if (motherVisit.TypeId == 2) //Post Natal
            {
                ViewBag.Back = "PostNatal";
            }
            else
            {
                ViewBag.Back = "Delivery";
            }

            ViewBag.Id = motherVisit.PregnancyId;
            return View();
        }

        public IActionResult Update(string id, int? ClinicVisitId)
        {
            var systemCodeDetails = _context.SystemCodeDetails.Include(i => i.SystemCode)
                .Where(i => i.SystemCode.SystemModule.Name == "Case Management" || i.SystemCode.Code == "Yes No Choices").ToList();
            var vm = new PregnancyDataUpdateViewModel();
            vm.UpdatingClinicVisitId = ClinicVisitId;
            var pregnancy = _context.Pregnancies.Find(id);
            var caseManagement = _context.CaseManagement.Find(pregnancy.CaseManagementId);
            new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(caseManagement, vm);
            vm.PregnancyId = id;

            vm.PregnancyNo = pregnancy.PregnancyNo;
            vm.StatusId = pregnancy.StatusId;
            vm.BloodGroupId = pregnancy.BloodGroupId;
            vm.RhesusId = pregnancy.RhesusId;
            vm.SupportStatusId = pregnancy.SupportStatusId;
            vm.InfantFeedingCounselingDoneId = pregnancy.InfantFeedingCounselingDoneId;
            vm.BreastfeedingCounselingDoneId = pregnancy.BreastfeedingCounselingDoneId;

            var previousVisitsIds = _context.PregnancyData
                .Where(i => i.PregnancyId == id).Select(i => i.ClinicVisitId).ToList();

            ViewData["ClinicVisitId"] = new SelectList(_context.ClinicVisits.Where(i => !previousVisitsIds.Contains(i.Id) && i.VisitType.Code == "ANC visit").ToList(), "Id", "Name");
            ViewData["BloodGroupId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Blood Group"), "Id", "Code", pregnancy.BloodGroupId);
            ViewData["RhesusId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Rhesus"), "Id", "Code", pregnancy.RhesusId);
            ViewData["SupportStatusId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "HIV Status 2"), "Id", "Code", pregnancy.SupportStatusId);
            ViewData["InfantFeedingCounselingDoneId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code", pregnancy.InfantFeedingCounselingDoneId);
            ViewData["BreastfeedingCounselingDoneId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code", pregnancy.BreastfeedingCounselingDone);
            vm.PregnancyDataList = _context.PregnancyData
                .OrderBy(i => i.ClinicVisitId)
                .Include(i => i.MotherClinicVisit)
                .Include(i => i.MotherPreventiveServices)
                .Include(i => i.ClinicVisit)
                .Where(i => i.PregnancyId == id).ToList();

            var services = _context.MotherPreventiveServices
                .OrderBy(i => i.PreventiveServiceId)
                .Include(i => i.PreventiveService)
                .Where(i => i.PregnancyData.PregnancyId == id);
            if (ClinicVisitId != null)
            {
                vm.ClinicVisitId = (int)ClinicVisitId;
                var data = vm.PregnancyDataList.Single(i => i.ClinicVisitId == ClinicVisitId);
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(data, vm);
                ViewData["ClinicVisitId"] = new SelectList(_context.ClinicVisits.Where(i => i.VisitType.Code == "ANC visit").ToList(), "Id", "Name", data.ClinicVisitId);
                vm.IsUpdating = true;

                var visitServicesIds = services.Where(i => i.PregnancyDataId == data.Id)
                    .Select(i => i.PreventiveServiceId).ToList();
                vm.PreventiveServices = visitServicesIds;

                var servicesAlreadyGiven = services.Where(i => i.PregnancyDataId != data.Id && i.PreventiveServiceId != 236)
                    .Select(i => i.PreventiveServiceId).ToList();

                ViewData["PreventiveServiceId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Preventive Services" && !servicesAlreadyGiven.Contains(i.Id)), "Id", "Code");
            }
            else
            {
                var servicesAlreadyGiven = services.Where(i => i.PreventiveServiceId != 236)
                    .Select(i => i.PreventiveServiceId).ToList();

                ViewData["PreventiveServiceId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Preventive Services" && !servicesAlreadyGiven.Contains(i.Id)), "Id", "Code");

                vm.NextVisit = null;
            }

            ViewBag.motherSupportStatusId = pregnancy.SupportStatusId;

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(PregnancyDataUpdateViewModel vm)
        {
            var pregnancy = _context.Pregnancies.Include(i => i.CaseManagement)
                .SingleOrDefault(i => i.Id == vm.PregnancyId);

            pregnancy.LastVisit = vm.VisitDate;
            pregnancy.NextVisit = vm.NextVisit;
            if (vm.ClinicVisitId == 8) // Last ANC
            {
                pregnancy.NextVisitClinicId = 19; // Delivery
            }
            else
            {
                pregnancy.NextVisitClinicId = _context.ClinicVisits.Find(vm.ClinicVisitId).Id + 1;
            }

            if (ModelState.IsValid)
            {
                // Enforce Blood Group and Rhesus on 4th ANC
                bool proceed = true;
                if (vm.ClinicVisitId >= 4 && (pregnancy.BloodGroupId == null || pregnancy.RhesusId == null))
                {
                    var info = "The following are required on 4th ANC";
                    if (vm.BloodGroupId == null)
                    {
                        info += "<br> - Blood Group";
                        proceed = false;
                    }
                    else
                    {
                        pregnancy.BloodGroupId = vm.BloodGroupId;
                    }

                    if (vm.RhesusId == null)
                    {
                        info += "<br> - Rhesus";
                        proceed = false;
                    }
                    else
                    {
                        pregnancy.RhesusId = vm.RhesusId;
                    }

                    if (proceed == false)
                    {
                        TempData["Info"] = info;
                        ViewBag.ShowDisbabled = "1";
                    }
                }
                if (proceed)
                {
                    pregnancy.MissedVisits = 0;
                    var pregnancyData = new PregnancyData();
                    var visit = new ClinicVisit();
                    var statusId = 0;
                    var motherVisit = new MotherClinicVisit();
                    var smses = _context.SMS.OrderBy(i => i.Order)
                        .Where(i => i.ClinicVisitId == vm.ClinicVisitId && i.TriggerEvent == null).ToList();
                    var household = _context.HouseholdRegs.Include(i => i.Mother).Single(i => i.Id == vm.HouseholdId);
                    if (vm.IsUpdating)

                    {
                        var pregnancyDataUpdating = _context.PregnancyData
                            .Single(i => i.ClinicVisitId == vm.UpdatingClinicVisitId
                                         && i.PregnancyId == vm.PregnancyId);

                        var pregnancyDataId = pregnancyDataUpdating.Id;
                        if (vm.ClinicVisitId != vm.UpdatingClinicVisitId)
                        {
                        }
                        // Delete Previous ones

                        var sql = @"DELETE FROM MotherPreventiveServices where PregnancyDataId= @pregnancyDataId";
                        _context.Database.ExecuteSqlCommand(sql,
                            new SqlParameter("@pregnancyDataId", pregnancyDataUpdating.Id));

                        //var sql2 = @"DELETE FROM PregnancyData where Id= @id";
                        //_context.Database.ExecuteSqlCommand(sql2,
                        //    new SqlParameter("@id", pregnancyDataUpdating.Id));

                        new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper()
                            .Map(vm, pregnancyDataUpdating);
                        pregnancyDataUpdating.Id = pregnancyDataId;
                        _uow.GetRepository<PregnancyData>().Update(pregnancyDataUpdating);

                        motherVisit.ClinicVisitId = vm.ClinicVisitId;
                        visit = _context.ClinicVisits.Find(vm.ClinicVisitId);
                        if (visit.PaymentPointId != null) //Payment Trigger
                        {
                            motherVisit = _context.MotherClinicVisits.Find(pregnancyDataUpdating.MotherClinicVisitId);
                            motherVisit.RequiresFingerPrint = true;
                        }

                        if (vm.PreventiveServices != null && vm.PreventiveServices.Any())
                        {
                            List<MotherPreventiveService> preventiveServices = new List<MotherPreventiveService>();
                            foreach (var id in vm.PreventiveServices)
                            {
                                var service = new MotherPreventiveService
                                {
                                    PregnancyDataId = pregnancyDataUpdating.Id,
                                    PreventiveServiceId = id
                                };
                                preventiveServices.Add(service);
                            }

                            _context.AddRange(preventiveServices);
                        }

                        await _context.SaveChangesAsync();
                        _uow.Save();
                        visit = _context.ClinicVisits.Find(vm.ClinicVisitId);

                        foreach (var sms in smses)
                        {
                            _smsService.Send(household.Phone, sms.Message
                                .Replace("##NAME##", household.CommonName ?? household.Mother.FirstName)
                                .Replace("##DATE##", ((DateTime)vm.NextVisit).ToString("dd/MM/yyyy")));
                        }

                        TempData["Message"] = "Pregnancy Updated Saved.";
                        if (motherVisit.RequiresFingerPrint)
                        {
                            TempData["Message"] = "Pregnancy Updated Saved. Verify fingerprint now.";
                            return RedirectToAction(nameof(FingerPrint), new { id = motherVisit.Id });
                        }

                        return RedirectToAction(nameof(Update), new { id = vm.Id });
                    }

                    // Create Visit
                    motherVisit = new MotherClinicVisit();
                    motherVisit.HouseholdId = vm.HouseholdId;
                    motherVisit.HealthFacilityId = _dbService.GetHealthFacilityId();
                    motherVisit.Id = Guid.NewGuid().ToString().ToLower();
                    motherVisit.DateCreated = DateTime.UtcNow.AddHours(3);
                    motherVisit.CreatedById = User.GetUserId();
                    motherVisit.DueDate = vm.NextVisit;
                    motherVisit.VisitDate = (DateTime)vm.VisitDate;
                    motherVisit.PregnancyId = vm.PregnancyId;
                    motherVisit.ClinicVisitId = vm.ClinicVisitId;
                    motherVisit.TypeId = 1;
                    _uow.GetRepository<MotherClinicVisit>().Add(motherVisit);
                    _uow.Save();

                    // Record Pregnancy Data

                    pregnancyData.HouseholdId = vm.HouseholdId;
                    pregnancyData.PregnancyId = pregnancy.Id;
                    pregnancyData.ClinicVisitId = vm.ClinicVisitId;
                    pregnancyData.BloodPressure = vm.BloodPressure;
                    pregnancyData.Weight = vm.Weight;
                    pregnancyData.VisitDate = vm.VisitDate;
                    pregnancyData.NextVisit = vm.NextVisit;
                    pregnancyData.MotherClinicVisitId = motherVisit.Id;
                    pregnancyData.DateCreated = DateTime.UtcNow.AddHours(3);
                    pregnancyData.CreatedById = User.GetUserId();
                    _uow.GetRepository<PregnancyData>().Add(pregnancyData);

                    // Check if visit triggers payment
                    if (household.IsBeneficiary)
                    {
                        visit = _context.ClinicVisits.Find(vm.ClinicVisitId);
                        if (visit.PaymentPointId != null)
                        {
                            motherVisit.RequiresFingerPrint = true;
                        }
                    }

                    // Record Preventive services Given
                    if (vm.PreventiveServices != null && vm.PreventiveServices.Any())
                    {
                        List<MotherPreventiveService> preventiveServices = new List<MotherPreventiveService>();
                        foreach (var id in vm.PreventiveServices)
                        {
                            var service = new MotherPreventiveService
                            {
                                PregnancyDataId = pregnancyData.Id,
                                PreventiveServiceId = id
                            };
                            preventiveServices.Add(service);
                        }

                        _context.AddRange(preventiveServices);
                    }

                    _uow.Save();
                    TempData["Message"] = "Pregnancy Data Saved.";
                    foreach (var sms in smses)
                    {
                        _smsService.Send(household.Phone,
                            sms.Message.Replace("##NAME##", household.CommonName ?? household.Mother.FirstName)
                                .Replace("##DATE##", ((DateTime)vm.NextVisit).ToString("dd/MM/yyyy")));
                    }

                    if (motherVisit.RequiresFingerPrint)
                        return RedirectToAction(nameof(FingerPrint), new { id = motherVisit.Id });
                    return RedirectToAction(nameof(Update), new { id = vm.Id });
                }
            }

            var systemCodeDetails = _context.SystemCodeDetails.Include(i => i.SystemCode)
                    .Where(i => i.SystemCode.SystemModule.Name == "Case Management" || i.SystemCode.Code == "Yes No Choices").ToList();

            new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(pregnancy, vm);
            vm.PregnancyId = vm.PregnancyId;

            vm.PregnancyNo = pregnancy.PregnancyNo;
            vm.BloodGroupId = pregnancy.BloodGroupId;
            vm.RhesusId = pregnancy.RhesusId;
            vm.SupportStatusId = pregnancy.SupportStatusId;
            vm.InfantFeedingCounselingDoneId = pregnancy.InfantFeedingCounselingDoneId;
            vm.BreastfeedingCounselingDoneId = pregnancy.BreastfeedingCounselingDoneId;
            var previousVisitsIds = _context.PregnancyData
                .Where(i => i.PregnancyId == vm.PregnancyId).Select(i => i.ClinicVisitId).ToList();
            ViewData["ClinicVisitId"] = new SelectList(_context.ClinicVisits.Where(i => !previousVisitsIds.Contains(i.Id) && i.VisitType.Code == "ANC visit").ToList(), "Id", "Name");
            ViewData["BloodGroupId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Blood Group"), "Id", "Code", pregnancy.BloodGroupId);
            ViewData["RhesusId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Rhesus"), "Id", "Code", pregnancy.RhesusId);
            ViewData["SupportStatusId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "HIV Status 2"), "Id", "Code", pregnancy.SupportStatusId);
            ViewData["InfantFeedingCounselingDoneId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code", pregnancy.InfantFeedingCounselingDoneId);
            ViewData["BreastfeedingCounselingDoneId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code", pregnancy.BreastfeedingCounselingDone);

            vm.PregnancyDataList = _context.PregnancyData
                .OrderBy(i => i.ClinicVisitId)
                .Include(i => i.MotherClinicVisit)
                .Include(i => i.MotherPreventiveServices)
                .Include(i => i.ClinicVisit)
                .Where(i => i.PregnancyId == pregnancy.Id).ToList();
            var services = _context.MotherPreventiveServices
                .OrderBy(i => i.PreventiveServiceId)
                .Include(i => i.PreventiveService)
                .Where(i => i.PregnancyData.PregnancyId == pregnancy.Id);

            var servicesAlreadyGiven = services.Where(i => i.PreventiveServiceId != 236)
                .Select(i => i.PreventiveServiceId).ToList();

            ViewData["PreventiveServiceId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Preventive Services" && !servicesAlreadyGiven.Contains(i.Id)), "Id", "Code");

            return View(vm);
        }

        public IActionResult UpdateAntenatal(string id)
        {
            var systemCodeDetails = _context.SystemCodeDetails.Include(i => i.SystemCode)
                .Where(i => i.SystemCode.SystemModule.Name == "Case Management" || i.SystemCode.Code == "Yes No Choices").ToList();
            var vm = new CaseManagementViewModel();

            var caseManagement = _context.CaseManagement.Find(id);
            new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(caseManagement, vm);
            vm.CaseManagementId = id;

            var pregnancy = _context.Pregnancies.SingleOrDefault(i => i.Id == id);
            vm.PregnancyNo = pregnancy.PregnancyNo;
            vm.BloodGroupId = pregnancy.BloodGroupId;
            vm.CaseManagementId = pregnancy.CaseManagementId;
            vm.Id = id;
            vm.RhesusId = pregnancy.RhesusId;
            vm.SupportStatusId = pregnancy.SupportStatusId;
            vm.InfantFeedingCounselingDoneId = pregnancy.InfantFeedingCounselingDoneId;
            vm.BreastfeedingCounselingDoneId = pregnancy.BreastfeedingCounselingDoneId;

            ViewData["ANCId"] = new SelectList(_context.ClinicVisits.ToList(), "Id", "Name");
            ViewData["BloodGroupId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Blood Group"), "Id", "Code", pregnancy.BloodGroupId);
            ViewData["RhesusId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Rhesus"), "Id", "Code", pregnancy.RhesusId);
            ViewData["SupportStatusId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "HIV Status 2"), "Id", "Code", pregnancy.SupportStatusId);
            ViewData["InfantFeedingCounselingDoneId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code", pregnancy.InfantFeedingCounselingDoneId);
            ViewData["BreastfeedingCounselingDoneId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code", pregnancy.BreastfeedingCounselingDone);

            return View(vm);
        }

        [HttpPost]
        public IActionResult UpdateAntenatal(CaseManagementViewModel vm)
        {
            var pregnancy = _context.Pregnancies.SingleOrDefault(i => i.Id == vm.Id);
            var pregnancyId = pregnancy.Id;
            new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, pregnancy);
            pregnancy.Id = pregnancyId;
            pregnancy.ModifiedById = User.GetUserId();
            pregnancy.DateModified = DateTime.UtcNow.AddHours(3);
            _uow.GetRepository<Pregnancy>().Update(pregnancy);
            _uow.Save();
            TempData["Message"] = "Antenatal Profile Updated";
            return RedirectToAction("Update", new { id = vm.Id });
        }

        public IActionResult Delivery(string id, string option = "", string optionId = "")
        {
            var systemCodeDetails = _context.SystemCodeDetails.Include(i => i.SystemCode)
                .Where(i => i.SystemCode.SystemModule.Name == "Case Management"
                || i.SystemCode.Code == "Yes No Choices").ToList();
            var vm = new DeliveryViewModel
            {
                Option = option,
                OptionId = optionId
            };

            var delivery = _context.Deliveries
                .Include(i => i.DeliveryMode)
                .Include(i => i.Pregnancy)
                .Include(i => i.Pregnancy.CaseManagement)
                .Include(i => i.ObstructedLabour)

                .Include(i => i.MeconiumStainedLiquor)
                .SingleOrDefault(i => i.PregnancyId == id);
            if (delivery != null)
            {
                vm.DeliveryId = delivery.Id;
                vm.Children = _context.Children.OrderBy(i => i.BirthOrder).Where(i => i.DeliveryId == delivery.Id);
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(delivery, vm);
                vm.MotherDrugsAdministeredIds = _context.DrugsAdministered
                    .Where(i => i.RecipientTypeId == 1 && i.DeliveryId == delivery.Id).Select(i => i.DrugId).ToList();

                var firstChild = _context.Children.Include(i => i.Immunized).OrderBy(i => i.BirthOrder).FirstOrDefault(i => i.DeliveryId == delivery.Id);
                if (!string.IsNullOrEmpty(optionId))
                {
                    vm.IsUpdating = true;
                    vm.Option = option;
                    vm.OptionId = optionId;
                    if (vm.Option.Equals("baby"))
                    {
                        firstChild = _context.Children.Find(vm.OptionId);
                    }
                }
                if (firstChild != null)
                {
                    vm.ChildId = firstChild.Id;
                    new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(firstChild, vm);
                    vm.DeliveryAssistantId = firstChild.DeliveryAssistantId;
                    vm.BabyDrugsAdministeredIds = _context.DrugsAdministered
                        .Where(i => i.RecipientTypeId == 2 && i.DeliveryId == delivery.Id && i.RecipientId == firstChild.Id).Select(i => i.DrugId).ToList();
                }
                var motherVisit = _context.MotherClinicVisits.SingleOrDefault(i => i.TypeId == 3 && i.PregnancyId == delivery.PregnancyId);
                if (motherVisit != null)
                {
                    vm.IsVerified = motherVisit.Verified;
                    ViewBag.MotherVisitId = motherVisit.Id;
                }
                vm.IsBeneficiary = _context.HouseholdRegs.Find(delivery.Pregnancy.CaseManagement.HouseholdId).IsBeneficiary;
            }
            var prevSupportStatusId = _context.Pregnancies.Single(i => i.Id == id).SupportStatusId;
            vm.PrevSupportStatusId = vm.SupportStatusId = prevSupportStatusId;
            vm.PregnancyId = id;
            ViewData["PregnancyOutcomeId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Pregnancy Outcome"), "Id", "Code");
            ViewData["DeliveryModeId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Delivery Modes"), "Id", "Code");
            ViewData["BloodGroupId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Blood Group"), "Id", "Code");
            ViewData["BloodLossId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Blood Loss"), "Id", "Code");
            ViewData["RhesusId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Rhesus"), "Id", "Code");
            ViewData["SupportStatusId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "HIV Status 2"), "Id", "Code");
            ViewData["InfantFeedingCounselingDoneId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");
            ViewData["BreastfeedingCounselingDoneId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");

            ViewData["PreEclampsiaId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");
            ViewData["EclampsiaId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");
            ViewData["ObstructedLabourId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");
            ViewData["RescuscitationDoneId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");

            ViewData["MotherDrugsAdministeredId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Mother Delivery Drugs"), "Id", "Code");
            ViewData["BabyDrugsAdministeredId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Baby Delivery Drugs"), "Id", "Code");

            ViewData["MeconiumStainedLiquorId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Meconium Stained Liquor"), "Id", "Code");

            ViewData["DeliveryPlaceId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Place of Delivery"), "Id", "Code");
            ViewData["DeliveryHealthFacilityId"] = new SelectList(_context.HealthFacilities.OrderBy(i => i.Name), "Id", "Name", vm.DeliveryHealthFacilityId);
            ViewData["DeliveryAssistantId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Delivery Assistant"), "Id", "Code", vm.DeliveryAssistantId);
            ViewData["ChildGenderId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Gender"), "Id", "Code");
            ViewBag.motherSupportStatusId = _context.Pregnancies.OrderByDescending(i => i.DateCreated)
                .First(i => i.Id == vm.PregnancyId).SupportStatusId;
            vm.StatusId = _context.Pregnancies.Find(id).StatusId;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delivery(DeliveryViewModel vm, string id, string option = "", string optionId = "")
        {
            var pregnancy = _context.Pregnancies.Find(vm.PregnancyId);
            var caseManagent = _context.CaseManagement.Find(pregnancy.CaseManagementId);

            if (vm.IsUpdating)
            {
                if (vm.Option.Equals("mother"))
                {
                    var delivery = _context.Deliveries.Include(i => i.Pregnancy.CaseManagement).Single(i => i.Id == vm.DeliveryId);
                    new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, delivery);
                    delivery.Id = vm.DeliveryId;
                    _uow.GetRepository<Delivery>().Update(delivery);

                    var householdId = delivery.Pregnancy.CaseManagement.HouseholdId;
                    // Create Visit
                    var motherVisit = _context.MotherClinicVisits.SingleOrDefault(i => i.TypeId == 3 && i.PregnancyId == delivery.PregnancyId);
                    if (motherVisit == null)
                    {
                        motherVisit = new MotherClinicVisit();
                        motherVisit.HouseholdId = householdId;
                        motherVisit.HealthFacilityId = _dbService.GetHealthFacilityId();
                        motherVisit.Id = Guid.NewGuid().ToString().ToLower();
                        motherVisit.DateCreated = DateTime.UtcNow.AddHours(3);
                        motherVisit.CreatedById = User.GetUserId();
                        motherVisit.VisitDate = (DateTime)vm.DeliveryDate;
                        motherVisit.PregnancyId = vm.PregnancyId;
                        motherVisit.ClinicVisitId = 19; // Delivery
                        motherVisit.TypeId = 3;
                        _uow.GetRepository<MotherClinicVisit>().Add(motherVisit);
                    }
                    else
                    {
                        motherVisit.Verified = false;
                        motherVisit.IsVerifying = true;
                        _uow.GetRepository<MotherClinicVisit>().Update(motherVisit);
                    }

                    pregnancy.LastVisit = vm.DeliveryDate;
                    pregnancy.NextVisit = vm.NextVisit;
                    pregnancy.NextVisitClinicId = 9; //48 Hour Visit;
                    _uow.Save();

                    var statusId = _context.HouseholdRegs.Find(householdId).StatusId;
                    // Home Delivery doesn't require fingerprint verification
                    if (statusId != 20 && statusId != 23 && statusId != 28 && vm.DeliveryPlaceId != 259)
                    {
                        TempData["Message"] = "Delivery information updated. Verify fingerprints now.";
                        return RedirectToAction(nameof(FingerPrint), new { id = motherVisit.Id });
                    }
                    TempData["Message"] = "Delivery information updated.";
                    return RedirectToAction(nameof(Delivery), new { id = vm.PregnancyId });
                }
                else
                {
                    var delivery = _context.Deliveries.Find(vm.DeliveryId);
                    if (!vm.OptionId.Equals("0"))
                    {
                        var child = _context.Children.Find(vm.ChildId);
                        new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, child);
                        if (delivery.PregnancyOutcomeId == 321)
                        {
                            child.StatusId = _context.SystemCodeDetails.Single(i => i.Code == "Alive").Id;
                        }
                        _uow.GetRepository<Child>().Update(child);
                        TempData["Message"] = "Child's information updated.";
                    }
                    else
                    {
                        var child = new Child();
                        new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, child);
                        if (child.Name != null)
                            child.Name = child.Name.ToUpper();

                        child.Id = Guid.NewGuid().ToString().ToLower();
                        child.DeliveryId = vm.DeliveryId;
                        child.StatusId = _context.SystemCodeDetails.Single(i => i.Code == "Alive").Id;
                        child.Id = Guid.NewGuid().ToString().ToLower();
                        child.DOB = vm.DeliveryDate;
                        if (vm.PregnancyOutcomeId == 321)
                        {
                            child.StatusId = _context.SystemCodeDetails.Single(i => i.Code == "Alive").Id;
                        }
                        _uow.GetRepository<Child>().Add(child);
                        ;
                        if (vm.BabyDrugsAdministeredIds != null)
                        {
                            foreach (var drugId in vm.BabyDrugsAdministeredIds)
                            {
                                var drugAdministered = new DrugsAdministered
                                {
                                    DrugId = drugId,
                                    RecipientTypeId = 2,
                                    DeliveryId = vm.DeliveryId,
                                    RecipientId = child.Id
                                };
                                _context.DrugsAdministered.Add(drugAdministered);
                            }
                        }

                        TempData["Message"] = "Child's information saved.";
                    }
                }
                _uow.Save();
                ViewBag.motherSupportStatusId = _context.Pregnancies.OrderByDescending(i => i.DateCreated)
                    .First(i => i.Id == vm.PregnancyId).SupportStatusId;
                return RedirectToAction(nameof(Delivery), new { id = vm.PregnancyId });
            }

            if (ModelState.IsValid)
            {
                pregnancy = _context.Pregnancies
                   .Include(i => i.CaseManagement)
                   .Single(i => i.Id == vm.PregnancyId);
                var pregnancyId = pregnancy.Id;
                var householdId = pregnancy.CaseManagement.HouseholdId;
                // Create Visit
                var motherVisit = new MotherClinicVisit();
                motherVisit.HouseholdId = householdId;
                motherVisit.HealthFacilityId = _dbService.GetHealthFacilityId();
                motherVisit.Id = Guid.NewGuid().ToString().ToLower();
                motherVisit.DateCreated = DateTime.UtcNow.AddHours(3);
                motherVisit.CreatedById = User.GetUserId();
                motherVisit.VisitDate = (DateTime)vm.DeliveryDate;
                motherVisit.PregnancyId = vm.PregnancyId;
                motherVisit.ClinicVisitId = 19; // Delivery
                motherVisit.TypeId = 3;
                _uow.GetRepository<MotherClinicVisit>().Add(motherVisit);
                _uow.Save();

                var delivery = new Delivery();
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, delivery);
                delivery.Id = Guid.NewGuid().ToString().ToLower();
                delivery.PregnancyId = pregnancyId;
                delivery.DateCreated = DateTime.UtcNow.AddHours(3);
                delivery.CreatedById = User.GetUserId();
                delivery.MotherClinicVisitId = motherVisit.Id;

                _context.Deliveries.Add(delivery);
                if (vm.BirthWeight != null)
                {
                    // Add a child
                    var child = new Child();
                    new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, child);
                    child.Id = Guid.NewGuid().ToString().ToLower();
                    child.DeliveryId = delivery.Id;
                    child.DOB = vm.DeliveryDate;
                    child.StatusId = _context.SystemCodeDetails.Single(i => i.Code == "Alive").Id;
                    _context.Children.Add(child);
                    if (vm.BabyDrugsAdministeredIds != null)
                    {
                        foreach (var drugId in vm.BabyDrugsAdministeredIds)
                        {
                            var drugAdministered = new DrugsAdministered
                            {
                                DrugId = drugId,
                                RecipientTypeId = 2,
                                DeliveryId = delivery.Id,
                                RecipientId = child.Id
                            };
                            _context.DrugsAdministered.Add(drugAdministered);
                        }
                    }
                }

                if (vm.MotherDrugsAdministeredIds != null)
                {
                    foreach (var drugId in vm.MotherDrugsAdministeredIds)
                    {
                        var drugAdministered = new DrugsAdministered
                        {
                            DrugId = drugId,
                            RecipientTypeId = 1,
                            DeliveryId = delivery.Id
                        };
                        _context.DrugsAdministered.Add(drugAdministered);
                    }
                }

                var household = _context.HouseholdRegs.Include(i => i.Mother).Single(i => i.Id == pregnancy.CaseManagement.HouseholdId);
                var smses = _context.SMS.OrderBy(i => i.Order).Where(i => i.TriggerEvent == "DELIVERY").ToList();
                foreach (var sms in smses)
                {
                    _smsService.Send(household.Phone, sms.Message.Replace("##NAME##",
                        household.CommonName ?? household.Mother.FirstName)
                        .Replace("##DATE##", ((DateTime)vm.NextVisit).ToString("dd/MM/yyyy")));
                }
                var statusId = household.StatusId;
                // Home Delivery doesn't require fingerprint verification
                if (statusId != 20 && statusId != 23 && statusId != 28 && vm.DeliveryPlaceId != 259)
                {
                    // Add Payment
                    motherVisit.RequiresFingerPrint = true;
                    pregnancy.LastVisit = vm.DeliveryDate;
                    pregnancy.NextVisit = vm.NextVisit;
                    pregnancy.NextVisitClinicId = 9; //48 Hour Visit;
                    _context.SaveChanges();
                    TempData["Message"] = "Delivery Data Saved. Verify fingerprint now.";
                    return RedirectToAction(nameof(FingerPrint), new { id = motherVisit.Id });
                }
                TempData["Message"] = "Delivery Data Saved.";
                _context.SaveChanges();
                return RedirectToAction(nameof(Delivery), new { id = vm.PregnancyId });
            }
            var systemCodeDetails = _context.SystemCodeDetails.Include(i => i.SystemCode)
                .Where(i => i.SystemCode.SystemModule.Name == "Case Management"
                            || i.SystemCode.Code == "Yes No Choices").ToList();

            ViewData["DeliveryModeId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Delivery Modes"), "Id", "Code", vm.DeliveryModeId);
            ViewData["BloodLossId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Blood Loss"), "Id", "Code", vm.BloodLossId);
            ViewData["SupportStatusId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "HIV Status 2"), "Id", "Code", vm.SupportStatusId);

            ViewData["PreEclampsiaId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code", vm.PreEclampsiaId);
            ViewData["EclampsiaId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code", vm.EclampsiaId);
            ViewData["ObstructedLabourId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code", vm.ObstructedLabourId);
            ViewData["RescuscitationDoneId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code", vm.RescuscitationDoneId);

            ViewData["MotherDrugsAdministeredId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Mother Delivery Drugs"), "Id", "Code");
            ViewData["BabyDrugsAdministeredId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Baby Delivery Drugs"), "Id", "Code");

            ViewData["MeconiumStainedLiquorId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Meconium Stained Liquor"), "Id", "Code", vm.MeconiumStainedLiquorId);

            ViewData["DeliveryPlaceId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Place of Delivery"), "Id", "Code", vm.DeliveryPlaceId);
            ViewData["DeliveryHealthFacilityId"] = new SelectList(_context.HealthFacilities.OrderBy(i => i.Name), "Id", "Name", vm.DeliveryHealthFacilityId); ViewData["DeliveryAssistantId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Delivery Assistant"), "Id", "Code", vm.DeliveryHealthFacilityId);
            ViewData["ChildGenderId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Gender"), "Id", "Code", vm.ChildGenderId);
            return View(vm);
        }

        //public IActionResult PreventiveServices(string id, int? preventiveServiceId)
        //{
        //    var systemCodeDetails = _context.SystemCodeDetails.Include(i => i.SystemCode)
        //        .Where(i => i.SystemCode.SystemModule.Name == "Case Management"
        //                    || i.SystemCode.Code == "Yes No Choices").ToList();
        //    var services = _context.MotherPreventiveServices
        //        .OrderBy(i => i.PreventiveServiceId)
        //        .Include(i => i.PreventiveService)
        //        .OrderByDescending(i => i.DateGiven)
        //        .Where(i => i.CaseManagementId == id);

        //    var servicesAlreadyGiven = services
        //        .Select(i => i.PreventiveServiceId).ToList();

        //    var vm = new PreventiveServicesViewModel
        //    {
        //        CaseManagementId = id,
        //        MotherPreventiveServices = services
        //    };
        //    ViewData["WasServiceGivenId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");
        //    ViewData["PreventiveServiceId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Preventive Services" && !servicesAlreadyGiven.Contains(i.Id)), "Id", "Code");
        //    if (preventiveServiceId != null)
        //    {
        //        var service =
        //            _context.MotherPreventiveServices.Single(
        //                i => i.CaseManagementId == id && i.PreventiveServiceId == preventiveServiceId);
        //        vm.NextVisit = service.NextVisit;
        //        vm.DateGiven = service.DateGiven;
        //        vm.PreventiveServiceId = service.PreventiveServiceId;
        //        vm.IsUpdating = true;
        //        ViewData["PreventiveServiceId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Preventive Services"), "Id", "Code", service.PreventiveServiceId);

        //    }
        //    return View(vm);
        //}
        //[HttpPost]
        //public IActionResult PreventiveServices(PreventiveServicesViewModel vm)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (vm.IsUpdating)
        //        {
        //            var service =
        //                _context.MotherPreventiveServices.Single(
        //                    i => i.CaseManagementId == vm.CaseManagementId && i.PreventiveServiceId == vm.UpdatingPreventiveServiceId);
        //            service.DateGiven = vm.DateGiven;
        //            service.NextVisit = vm.NextVisit;

        //            if (vm.UpdatingPreventiveServiceId != vm.PreventiveServiceId) //If service Id has changed, delete the previous one and add new
        //            {
        //                // Check for conflict
        //                if (_context.MotherPreventiveServices.Any(i => i.CaseManagementId == vm.CaseManagementId &&
        //                                                               i.PreventiveServiceId == vm.PreventiveServiceId))
        //                {
        //                    TempData["Info"] = "Preventive service already exists.";
        //                    return RedirectToAction("PreventiveServices", new { id = vm.CaseManagementId });
        //                }
        //                _uow.GetRepository<MotherPreventiveService>().Remove(service);
        //                _uow.Save();
        //                service.PreventiveServiceId = vm.PreventiveServiceId;
        //                _uow.GetRepository<MotherPreventiveService>().Add(service);
        //            }
        //            else
        //            {
        //                _uow.GetRepository<MotherPreventiveService>().Update(service);
        //            }
        //            _uow.Save();
        //            TempData["Message"] = "Preventive service updated.";
        //            return RedirectToAction("PreventiveServices", new { id = vm.CaseManagementId });
        //        }

        //        MotherPreventiveService model = new MotherPreventiveService();
        //        new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, model);
        //        model.DateCreated =DateTime.UtcNow.AddHours(3);
        //        model.CreatedById = User.GetUserId();
        //        _uow.GetRepository<MotherPreventiveService>().Add(model);
        //        _uow.Save();
        //        TempData["Message"] = "Preventive service saved.";
        //        return RedirectToAction("PreventiveServices", new { id = model.CaseManagementId });
        //    }
        //    var systemCodeDetails = _context.SystemCodeDetails.Include(i => i.SystemCode)
        //        .Where(i => i.SystemCode.SystemModule.Name == "Case Management"
        //                    || i.SystemCode.Code == "Yes No Choices").ToList();
        //    var services = _context.MotherPreventiveServices
        //        .OrderBy(i => i.PreventiveServiceId)
        //        .Include(i => i.PreventiveService)
        //        .Where(i => i.CaseManagementId == vm.CaseManagementId);

        //    var servicesAlreadyGiven = services
        //        .Select(i => i.PreventiveServiceId).ToList();

        //    vm = new PreventiveServicesViewModel
        //    {
        //        CaseManagementId = vm.CaseManagementId,
        //        MotherPreventiveServices = services

        //    };
        //    ViewData["WasServiceGivenId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");
        //    ViewData["PreventiveServiceId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Preventive Services" && !servicesAlreadyGiven.Contains(i.Id)), "Id", "Code");
        //    return View(vm);
        //}

        public IActionResult PMTCTInterventions(string id, int? interventionId)
        {
            var systemCodeDetails = _context.SystemCodeDetails.Include(i => i.SystemCode)
                .Where(i => i.SystemCode.SystemModule.Name == "Case Management"
                            || i.SystemCode.Code == "Yes No Choices").ToList();

            var vm = new PMTCTInterventionViewModel
            {
                PregnancyId = id,
                PMTCTServices = _context.PMTCTServices.Include(i => i.PMTCTInterventions).Where(i => i.PregnancyId == id)
            };
            //ViewData["PMTCTMotherId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "PMTCT-MOTHER" && !alreadyGivenIds.Contains(i.Id)), "Id", "Code");
            //ViewData["PMTCTInfantId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "PMTCT-INFANT" && !alreadyGivenIds.Contains(i.Id)), "Id", "Code");

            ViewData["PMTCTMotherId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "PMTCT-MOTHER"), "Id", "Code");
            ViewData["PMTCTInfantId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "PMTCT-INFANT"), "Id", "Code");

            ViewData["RecipientTypeId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "PMTCT Recipient Types"), "Id", "Code");

            //if (interventionId != null)
            //{
            //    var service =
            //        _context.PMTCTInterventions.Single(
            //            i => i.CaseManagementId == id && i.InterventionId == interventionId);
            //    vm.RecipientTypeId = service.RecipientTypeId;

            //    vm.DateGiven = service.DateGiven;
            //    vm.InterventionId = service.InterventionId;
            //    vm.IsUpdating = true;

            //    ViewData["RecipientTypeId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "PMTCT Recipient Types"), "Id", "Code", service.RecipientTypeId);
            //    ViewData["PMTCTMotherId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "PMTCT-MOTHER"), "Id", "Code", service.InterventionId);
            //    ViewData["PMTCTInfantId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "PMTCT-INFANT"), "Id", "Code", service.InterventionId);

            //}
            vm.StatusId = _context.Pregnancies.Find(id).StatusId;
            ViewBag.motherSupportStatusId = 266;
            return View(vm);
        }

        [HttpPost]
        public IActionResult PMTCTInterventions(PMTCTInterventionViewModel vm)
        {
            vm.InterventionId = vm.RecipientTypeId == 295 ? vm.PMTCTMotherId : vm.PMTCTInfantId;
            if (ModelState.IsValid)
            {
                //if (vm.IsUpdating)
                //{
                //    var service =
                //        _context.PMTCTInterventions.Single(
                //            i => i.CaseManagementId == vm.CaseManagementId && i.InterventionId == vm.UpdatingInterventionId);
                //    service.DateGiven = (DateTime)vm.DateGiven;

                //    if (vm.UpdatingInterventionId != vm.InterventionId) //If service Id has changed, delete the previous one and add new
                //    {
                //        // Check for conflict
                //        if (_context.PMTCTInterventions.Any(i => i.CaseManagementId == vm.CaseManagementId &&
                //                                                       i.InterventionId == vm.InterventionId))
                //        {
                //            TempData["Info"] = "PMTCT Intervention already exists.";
                //            return RedirectToAction("PMTCTInterventions", new { id = vm.CaseManagementId });
                //        }
                //        _uow.GetRepository<PMTCTIntervention>().Remove(service);
                //        _uow.Save();
                //        if (vm.InterventionId != null) service.InterventionId = (int)vm.InterventionId;
                //        _uow.GetRepository<PMTCTIntervention>().Add(service);
                //    }
                //    else
                //    {
                //        _uow.GetRepository<PMTCTIntervention>().Update(service);
                //    }

                //    _uow.Save();
                //    TempData["Message"] = "PMTCT Intervention updated.";
                //    return RedirectToAction("PMTCTInterventions", new { id = vm.CaseManagementId });
                //}

                var PMTCTService = new PMTCTService();
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, PMTCTService);
                PMTCTService.DateCreated = DateTime.UtcNow.AddHours(3);
                PMTCTService.CreatedById = User.GetUserId();
                PMTCTService.Id = Guid.NewGuid().ToString().ToLower();

                _uow.GetRepository<PMTCTService>().Add(PMTCTService);

                var addedInterventions = new List<PMTCTIntervention>();
                foreach (var id in vm.InterventionIds)
                {
                    var intervention = new PMTCTIntervention
                    {
                        InterventionId = id,
                        PMTCTServiceId = PMTCTService.Id
                    };
                    addedInterventions.Add(intervention);
                }
                _uow.GetRepository<PMTCTIntervention>().AddRange(addedInterventions);
                _uow.Save();
                TempData["Message"] = "PMTCT Intervention saved.";
                return RedirectToAction("PMTCTInterventions", new { id = vm.PregnancyId });
            }
            var systemCodeDetails = _context.SystemCodeDetails.Include(i => i.SystemCode)
                .Where(i => i.SystemCode.SystemModule.Name == "Case Management"
                            || i.SystemCode.Code == "Yes No Choices").ToList();

            var interventions = _context.PMTCTInterventions
                .Where(i => i.PMTCTService.PregnancyId == vm.PregnancyId);

            vm = new PMTCTInterventionViewModel
            {
                PregnancyId = vm.PregnancyId,
                PMTCTServices = _context.PMTCTServices.Include(i => i.PMTCTInterventions)
                    .Where(i => i.PregnancyId == vm.PregnancyId)
            };
            // List<int> alreadyGivenIds = interventions.Select(i => i.InterventionId).ToList();

            ViewData["PMTCTMotherId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "PMTCT-MOTHER"), "Id", "Code", vm.PMTCTMotherId);
            ViewData["PMTCTInfantId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "PMTCT-INFANT"), "Id", "Code", vm.PMTCTInfantId);
            ViewData["RecipientTypeId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "PMTCT Recipient Types"), "Id", "Code", vm.RecipientTypeId);
            var errors = ModelState.Select(x => x.Value.Errors)
                .Where(y => y.Count > 0)
                .ToList();
            var message = string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
            ViewBag.motherSupportStatusId = 266;
            return View(vm);
        }

        public IActionResult PostNatal(string id, string option = "", string optionId = "", string postNatalExaminationId = "")
        {
            var children = _context.Children.OrderBy(i => i.BirthOrder).Where(i => i.Delivery.PregnancyId == id);

            if (!children.Any())
            {
                TempData["Info"] = "Sorry, you cannot enter Post Natal data before Delivery data.";
                string referer = Request.Headers["Referer"].ToString();
                return Redirect(referer);
            }
            var vm = new PostNatalViewModel()
            {
                PregnancyId = id,
                Option = option,
                KeyMilestones = _context.KeyMilestones.ToList()
            };
            var registrations = _context.CivilRegistrations.Where(i => i.PregnancyId == id);
            if (registrations.Any())
            {
                if (registrations.Count() == registrations.Count(i => i.BirthCertificate != null))
                {
                    vm.BirthCertificateObatined = true;
                    // Check if the clinic visit had been marked to require Fingerprint
                    var visit = _context.MotherClinicVisits.FirstOrDefault(i => i.ClinicVisitId == 17 && i.PregnancyId == id && !i.RequiresFingerPrint);
                    if (visit != null)
                    {
                        visit.RequiresFingerPrint = true;
                        _context.SaveChanges();
                    }
                }
            }
            ViewBag.ChildId = new SelectList(children, "Id", "DisplayName");

            var systemCodeDetails = _context.SystemCodeDetails.Include(i => i.SystemCode)
                .Where(i => i.SystemCode.SystemModule.Name == "Case Management"
                            || i.SystemCode.Code == "Yes No Choices").ToList();

            if (vm.Option.Equals("edit"))
            {
                var details =
                    _context.PostNatalExaminationDetails
                        .SingleOrDefault(
                            i => i.ChildId == optionId && i.PostNatalExaminationId == postNatalExaminationId);

                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(details, vm);

                var postnatalExam = _context.PostNatalExaminations.Find(postNatalExaminationId);
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(postnatalExam, vm);

                vm.KeyMilestoneIds =
                    _context.PostNatalMilestones.Where(i => i.PostNatalExaminationDetailId == details.Id).Select(i => i.KeyMilestoneId).ToList();
                vm.ClinicVisitId = postnatalExam.ClinicVisitId;
                vm.NextVisit = postnatalExam.NextDate;
                vm.ClinicVisitId = postnatalExam.ClinicVisitId;

                var previousVisitsIds = _context.PostNatalExaminations
                    .Where(i => i.PregnancyId == vm.PregnancyId && i.Id != postNatalExaminationId).Select(i => i.ClinicVisitId).ToList();
                ViewData["ClinicVisitId"] =
                    new SelectList(
                        _context.ClinicVisits
                            .Where(i => !previousVisitsIds.Contains(i.Id) && i.VisitType.Code == "PNC visit").ToList(),
                        "Id", "Name");
            }
            vm.PostNatalExaminations = _context.PostNatalExaminations
                .Include(i => i.FPMethod)
                .Include(i => i.MotherClinicVisit)
                .Include(i => i.ClinicVisit)
                .Include(i => i.FPCounseling)
                .Include(i => i.Details)
                .OrderByDescending(i => i.ClinicVisitId)
                .Where(i => i.PregnancyId == id).ToList();

            var detailIds = _context.PostNatalExaminationDetails.Where(i => i.PostNatalExamination.PregnancyId == id)
                .Select(i => i.Id);
            var milestones =
                _context.PostNatalMilestones.Include(i => i.KeyMilestone).Where(i => detailIds.Contains(i.PostNatalExaminationDetailId));
            vm.PostNatalMilestones = milestones;
            ViewData["FPMethodId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Birth Control Methods"), "Id", "Code");
            ViewData["FeedingMethodId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Baby Feeding Methods"), "Id", "Code");

            ViewData["FPCounselingId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");

            int? prevSupportStatusId = _context.Pregnancies.Single(i => i.Id == id).SupportStatusId;
            vm.PrevSupportStatusId = vm.SupportStatusId = prevSupportStatusId;
            ViewData["SupportStatusId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "HIV Status 2"), "Id", "Code");
            ViewBag.ImmunizationStartedId = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");
            ViewBag.ARTProphylaxisGivenId = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices" || i.SystemCode.Code == "N/A"), "Id", "Code");
            ViewBag.CPInitiatedId = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices" || i.SystemCode.Code == "N/A"), "Id", "Code");
            if (!vm.Option.Equals("edit"))
            {
                var previousVisitsIds = _context.PostNatalExaminations
                    .Where(i => i.PregnancyId == vm.PregnancyId).Select(i => i.ClinicVisitId).ToList();
                ViewData["ClinicVisitId"] =
                    new SelectList(
                        _context.ClinicVisits
                            .Where(i => !previousVisitsIds.Contains(i.Id) && i.VisitType.Code == "PNC visit").ToList(),
                        "Id", "Name");
            }

            if (option.Equals("newbaby"))
            {
                ViewData["PostNatalExaminationId"] = new SelectList(vm.PostNatalExaminations, "Id", "ClinicVisit.Name");
            }
            ViewBag.motherSupportStatusId = _context.Pregnancies.OrderByDescending(i => i.DateCreated)
                .First(i => i.Id == vm.PregnancyId).SupportStatusId;
            vm.StatusId = _context.Pregnancies.Find(id).StatusId;
            return View(vm);
        }

        [HttpPost]
        public IActionResult PostNatal(PostNatalViewModel vm, string option = "", string optionId = "", string postNatalExaminationId = "")
        {
            if (!string.IsNullOrEmpty(vm.Option) && vm.Option.Equals("newbaby"))
            {
                if (_context.PostNatalExaminationDetails.Any(i => i.ChildId == vm.ChildId &&
                                                                  i.PostNatalExaminationId ==
                                                                  vm.PostNatalExaminationId))
                {
                    TempData["Info"] = "Baby data already present.";
                    return RedirectToAction("PostNatal", new { id = vm.PregnancyId });
                }
                // Record visit

                // Create Details
                var detail = new PostNatalExaminationDetail();
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, detail);
                detail.PostNatalExaminationId = vm.PostNatalExaminationId;
                _uow.GetRepository<PostNatalExaminationDetail>().Add(detail);
                _uow.Save();
                TempData["Message"] = "Baby data saved.";
                return RedirectToAction("PostNatal", new { id = vm.PregnancyId });
            }

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(vm.Option) && vm.Option.Equals("edit"))
                {
                    var postnatalExam = _context.PostNatalExaminations.Find(vm.PostNatalExaminationId);

                    var details =
                        _context.PostNatalExaminationDetails.SingleOrDefault(
                            i => i.ChildId == vm.ChildId && i.PostNatalExaminationId == vm.PostNatalExaminationId);
                    var prevKeyMilestones =
                        _context.PostNatalMilestones.Where(i => i.PostNatalExaminationDetailId == details.Id).ToList();
                    if (prevKeyMilestones.Any())
                    {
                        // Remove Data
                        _context.PostNatalMilestones.RemoveRange(prevKeyMilestones);
                        _uow.Save();
                    }

                    _context.PostNatalExaminationDetails.Remove(details);

                    _uow.Save();
                    _context.PostNatalExaminations.Remove(postnatalExam);

                    _uow.Save();

                    // Remove Prev MotherClinicVisit
                    var clinicVisit =
                        _context.MotherClinicVisits.First(i => i.PregnancyId == postnatalExam.PregnancyId && i.ClinicVisitId == vm.UpdatingClinicVisitId);

                    _context.MotherClinicVisits.Remove(clinicVisit);
                    _uow.Save();
                    //TempData["Message"] = "Post natal examination data updated.";

                    //return RedirectToAction("PostNatal", new { id = vm.CaseManagementId });
                }
                var pregnancy = _context.Pregnancies.Include(i => i.CaseManagement).Single(i => i.Id == vm.PregnancyId);

                pregnancy.MissedVisits = 0;
                // Mother Clinic Visit
                var motherVisit = new MotherClinicVisit();
                motherVisit.HouseholdId = pregnancy.CaseManagement.HouseholdId;
                motherVisit.HealthFacilityId = _dbService.GetHealthFacilityId();
                motherVisit.Id = Guid.NewGuid().ToString().ToLower();
                motherVisit.DateCreated = DateTime.UtcNow.AddHours(3);
                motherVisit.CreatedById = User.GetUserId();
                motherVisit.VisitDate = (DateTime)vm.VisitDate;
                motherVisit.TypeId = 2;
                motherVisit.PregnancyId = vm.PregnancyId;
                motherVisit.DueDate = pregnancy.NextVisit;
                motherVisit.VisitDate = DateTime.UtcNow.AddHours(3);
                motherVisit.ClinicVisitId = vm.ClinicVisitId;

                // Check if visit triggers payment
                var statusId = _context.HouseholdRegs.Find(pregnancy.CaseManagement.HouseholdId).StatusId;
                bool birthCertAvailable = false;
                if (statusId != 20 && statusId != 23 && statusId != 28)
                {
                    // If 18th Month, pay only if birth certificate is available
                    if (vm.ClinicVisitId == 17 && vm.KeyMilestoneIds != null && vm.KeyMilestoneIds.Contains(19))
                    {
                        // Check if birth certificate has been uploaded
                        var registration = _context.CivilRegistrations.SingleOrDefault(i => i.ChildId == vm.ChildId);
                        if (registration != null && !string.IsNullOrEmpty(registration.BirthCertificate))
                        {
                            motherVisit.RequiresFingerPrint = true;
                            birthCertAvailable = true;
                        }
                    }
                    else if (vm.ClinicVisitId == 17)
                    {
                        motherVisit.RequiresFingerPrint = false;
                    }
                    else
                    {
                        var visit = _context.ClinicVisits.Find(motherVisit.ClinicVisitId);
                        if (visit.PaymentPointId != null)
                        {
                            motherVisit.RequiresFingerPrint = true;
                        }
                    }
                }

                _uow.GetRepository<MotherClinicVisit>().Add(motherVisit);
                _uow.Save();

                // Postnatal Exam
                var model = new PostNatalExamination();
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, model);
                model.Id = Guid.NewGuid().ToString().ToLower();
                model.CreatedById = User.GetUserId();
                model.DateCreated = DateTime.UtcNow.AddHours(3);
                model.NextDate = vm.NextVisit;
                model.MotherClinicVisitId = motherVisit.Id;
                _uow.GetRepository<PostNatalExamination>().Add(model);

                // Postnatal Exam Details
                var detail = new PostNatalExaminationDetail();
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, detail);
                detail.PostNatalExaminationId = model.Id;
                detail.Id = Guid.NewGuid().ToString();
                _uow.GetRepository<PostNatalExaminationDetail>().Add(detail);

                // Milestones
                if (vm.KeyMilestoneIds != null)
                {
                    List<PostNatalMilestone> keyMilestones = new List<PostNatalMilestone>();
                    PostNatalMilestone milestone = new PostNatalMilestone();

                    foreach (var id in vm.KeyMilestoneIds)
                    {
                        milestone = new PostNatalMilestone
                        {
                            KeyMilestoneId = id,
                            PostNatalExaminationDetailId = detail.Id
                        };
                        keyMilestones.Add(milestone);
                    }
                    _uow.GetRepository<PostNatalMilestone>().AddRange(keyMilestones);
                }

                var smses = _context.SMS.OrderBy(i => i.Order).Where(i => i.ClinicVisitId == vm.ClinicVisitId && i.TriggerEvent == null).ToList();
                var household = _context.HouseholdRegs.Include(i => i.Mother).Single(i => i.Id == pregnancy.CaseManagement.HouseholdId);
                foreach (var sms in smses)
                {
                    var message = sms.Message.Replace("##NAME##",
                            household.CommonName ?? household.Mother.FirstName);

                    if (vm.NextVisit != null)
                    {
                        message = message.Replace("##DATE##", ((DateTime)vm.NextVisit).ToString("dd/MM/yyyy"));
                    }
                    _smsService.Send(household.Phone, message
                        );
                }

                pregnancy.NextVisit = vm.NextVisit;
                pregnancy.LastVisit = vm.VisitDate;
                pregnancy.NextVisitClinicId = _context.ClinicVisits.Find(vm.ClinicVisitId).Id + 1;
                _uow.Save();
                if (statusId != 20 && statusId != 23 && statusId != 28)
                {
                    if (vm.ClinicVisitId == 17 && (vm.KeyMilestoneIds == null || (vm.KeyMilestoneIds != null && !vm.KeyMilestoneIds.Contains(19))))
                    {
                        TempData["Info"] = "Post natal examination data saved but payments cannot be done because birth certificate is not available";
                    }
                    else
                    {
                        if (vm.ClinicVisitId == 17 && !birthCertAvailable)
                        {
                            var info = "Post natal examination data saved but payments cannot be done because birth certificate has been marked as obtained but it has not yet been uploaded";
                            info += "<br />Go to <strong>Child Data</strong> Tab and upload the certificate under section  <strong> C. Civil Registration</strong>";
                            TempData["Info"] = info;
                        }
                        else
                        {
                            TempData["Message"] = "Post natal examination data saved.";
                        }
                    }
                }
                else
                {
                    TempData["Message"] = "Post natal examination data saved.";
                }
                if (vm.ClinicVisitId == 17) // 18th Month; exit the mother
                {
                    var activePregnancy = _context.Pregnancies.Find(motherVisit.PregnancyId);
                    activePregnancy.StatusId = 2;
                    activePregnancy.ReasonId = 5; // Completed Program
                    activePregnancy.DateExited = DateTime.UtcNow.AddHours(3);

                    _uow.GetRepository<Pregnancy>().Update(activePregnancy);
                    _uow.Save();
                }
                if (motherVisit.RequiresFingerPrint)
                    return RedirectToAction(nameof(FingerPrint), new { id = motherVisit.Id });
                return RedirectToAction("PostNatal", new { id = vm.PregnancyId });
            }

            var children = _context.Children.OrderBy(i => i.BirthOrder)
                .Where(i => i.Delivery.PregnancyId == vm.PregnancyId);
            ViewBag.ChildId = new SelectList(children, "Id", "DisplayName");

            var systemCodeDetails = _context.SystemCodeDetails.Include(i => i.SystemCode)
                .Where(i => i.SystemCode.SystemModule.Name == "Case Management"
                            || i.SystemCode.Code == "Yes No Choices").ToList();

            if (vm.Option.Equals("edit"))
            {
                var details =
                    _context.PostNatalExaminationDetails
                        .SingleOrDefault(
                            i => i.ChildId == optionId && i.PostNatalExaminationId == postNatalExaminationId);

                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(details, vm);

                var postnatalExam = _context.PostNatalExaminations.Find(postNatalExaminationId);
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(postnatalExam, vm);

                vm.KeyMilestoneIds =
                    _context.PostNatalMilestones.Include(i => i.KeyMilestone).Where(i => i.PostNatalExaminationDetailId == details.Id).Select(i => i.KeyMilestoneId).ToList();
                vm.ClinicVisitId = postnatalExam.ClinicVisitId;
                ViewData["ClinicVisitId"] = new SelectList(_context.ClinicVisits.Where(i => i.VisitType.Code == "PNC visit").ToList(), "Id", "Name", postnatalExam.ClinicVisitId);
                vm.NextVisit = postnatalExam.NextDate;
                vm.ClinicVisitId = postnatalExam.ClinicVisitId;
                var previousVisitsIds1 = _context.PostNatalExaminations
                    .Where(i => i.PregnancyId == vm.PregnancyId && i.Id != postNatalExaminationId).Select(i => i.ClinicVisitId).ToList();
                ViewData["ClinicVisitId"] =
                    new SelectList(
                        _context.ClinicVisits
                            .Where(i => !previousVisitsIds1.Contains(i.Id) && i.VisitType.Code == "PNC visit").ToList(),
                        "Id", "Name");
            }
            vm.PostNatalExaminations = _context.PostNatalExaminations
                .Include(i => i.FPMethod)
                .Include(i => i.MotherClinicVisit)
                .Include(i => i.ClinicVisit)
                .Include(i => i.FPCounseling)
                .Include(i => i.Details)
                .OrderByDescending(i => i.Id)
                .Where(i => i.PregnancyId == vm.PregnancyId).ToList();

            var detailIds = _context.PostNatalExaminationDetails.Where(i => i.PostNatalExamination.PregnancyId == vm.PregnancyId)
                .Select(i => i.Id);
            var milestones =
                _context.PostNatalMilestones.Where(i => detailIds.Contains(i.PostNatalExaminationDetailId));
            vm.PostNatalMilestones = milestones;
            ViewData["FPMethodId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Birth Control Methods"), "Id", "Code");
            ViewData["FeedingMethodId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Baby Feeding Methods"), "Id", "Code");

            ViewData["FPCounselingId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");
            ViewData["SupportStatusId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "HIV Status 2"), "Id", "Code");
            ViewBag.ImmunizationStartedId = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");
            ViewBag.ARTProphylaxisGivenId = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices" || i.SystemCode.Code == "N/A"), "Id", "Code");
            ViewBag.CPInitiatedId = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices" || i.SystemCode.Code == "N/A"), "Id", "Code");

            if (!vm.Option.Equals("edit"))
            {
                var previousVisitsIds = _context.PostNatalExaminations
                    .Where(i => i.PregnancyId == vm.PregnancyId).Select(i => i.ClinicVisitId).ToList();
                ViewData["ClinicVisitId"] =
                    new SelectList(
                        _context.ClinicVisits
                            .Where(i => !previousVisitsIds.Contains(i.Id) && i.VisitType.Code == "PNC visit").ToList(),
                        "Id", "Name");
            }

            if (option.Equals("newbaby"))
            {
                ViewData["PostNatalExaminationId"] = new SelectList(vm.PostNatalExaminations, "Id", "ClinicVisit.Name");
            }
            ViewBag.motherSupportStatusId = _context.Pregnancies.OrderByDescending(i => i.DateCreated)
                .First(i => i.Id == vm.PregnancyId).SupportStatusId;
            return View(vm);
        }

        public IActionResult FamilyPlanning(string id, string optionId = "")
        {
            var systemCodeDetails = _context.SystemCodeDetails.Include(i => i.SystemCode)
                .Where(i => i.SystemCode.SystemModule.Name == "Case Management"
                            || i.SystemCode.Code == "Yes No Choices" || i.SystemCode.Code == "N/A").ToList();
            var vm = new FamilyPlanningViewModel
            {
                PregnancyId = id,
                OptionId = optionId,
                FamilyPlannings = _context.FamilyPlannings.OrderByDescending(i => i.Id).Where(i => i.PregnancyId == id)
            };
            if (!string.IsNullOrEmpty(optionId))
            {
                var obj = _context.FamilyPlannings.Find(optionId);
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(obj, vm);
            }
            ViewData["FPCounselingId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");
            ViewData["SupportStatusId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "HIV Status 2"), "Id", "Code");
            ViewBag.ImmunizationStartedId = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");
            ViewBag.ARTProphylaxisGivenId = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices" || i.SystemCode.Code == "N/A"), "Id", "Code");
            ViewBag.CPInitiatedId = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices" || i.SystemCode.Code == "N/A"), "Id", "Code");
            ViewBag.motherSupportStatusId = _context.Pregnancies.OrderByDescending(i => i.DateCreated)
                .First(i => i.Id == vm.PregnancyId).SupportStatusId;
            vm.StatusId = _context.Pregnancies.Find(id).StatusId;
            return View(vm);
        }

        [HttpPost]
        public IActionResult FamilyPlanning(FamilyPlanningViewModel vm)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(vm.OptionId))
                {
                    var obj = new FamilyPlanning();
                    new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, obj);
                    obj.CreatedById = User.GetUserId();
                    obj.DateCreated = DateTime.UtcNow.AddHours(3);
                    obj.Id = Guid.NewGuid().ToString().ToLower();
                    _uow.GetRepository<FamilyPlanning>().Add(obj);
                    _uow.Save();
                    TempData["Message"] = "Family Planning record saved.";
                }
                else
                {
                    var obj = _context.FamilyPlannings.Find(vm.OptionId);
                    new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, obj);
                    _uow.GetRepository<FamilyPlanning>().Update(obj);
                    _uow.Save();
                    TempData["Message"] = "Family Planning record updated.";
                }

                return RedirectToAction("FamilyPlanning", new { id = vm.PregnancyId });
            }
            var systemCodeDetails = _context.SystemCodeDetails.Include(i => i.SystemCode)
                .Where(i => i.SystemCode.SystemModule.Name == "Case Management"
                            || i.SystemCode.Code == "Yes No Choices" || i.SystemCode.Code == "N/A").ToList();

            vm.FamilyPlannings = _context.FamilyPlannings.OrderByDescending(i => i.Id).Where(i => i.PregnancyId == vm.PregnancyId);

            ViewData["FPCounselingId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");
            ViewData["SupportStatusId"] = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "HIV Status 2"), "Id", "Code");
            ViewBag.ImmunizationStartedId = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");
            ViewBag.ARTProphylaxisGivenId = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices" || i.SystemCode.Code == "N/A"), "Id", "Code");
            ViewBag.CPInitiatedId = new SelectList(systemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices" || i.SystemCode.Code == "N/A"), "Id", "Code");
            ViewBag.motherSupportStatusId = _context.Pregnancies.OrderByDescending(i => i.DateCreated)
                .First(i => i.Id == vm.PregnancyId).SupportStatusId;
            return View(vm);
        }

        public IActionResult ChildHealthMonitoring(string id, string childId = "")
        {
            var vm = new ChildHealthMonitoringViewModel
            {
                PregnancyId = id,
                DevelopmentMilestones = _context.DevelopmentMilestones,
            };
            if (!string.IsNullOrEmpty(childId))
                vm.ChildId = childId;

            var delivery = _context.Deliveries.FirstOrDefault(i => i.PregnancyId == id);
            if (delivery != null)
            {
                vm.Children = _context.Children
                    .Include(i => i.Gender)
                    .Include(i => i.Status)
                    .Include(i => i.Delivery.Pregnancy)
                    .OrderBy(i => i.BirthOrder)
                    .Where(i => i.DeliveryId == delivery.Id).ToList();
                ViewData["ChildId"] = new SelectList(vm.Children, "Id", "DisplayName");

                var child = vm.Children
                    .FirstOrDefault(
                        i => i.Id == childId || childId == "" && i.Delivery.PregnancyId == id);
                if (child != null)
                {
                    vm.ChildId = child.Id;
                    new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(child, vm);
                }

                var healthRecord =
                    _context.ChildHealthRecords.SingleOrDefault(i => i.PregnancyId == id && i.ChildId == child.Id);
                if (healthRecord != null)
                {
                    new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(healthRecord, vm);
                    vm.HealthFacilityId = healthRecord.HealthFacilityId;
                }
                else
                {
                    vm.HealthFacilityId = vm.Children.First().DeliveryHealthFacilityId;
                }

                var civilRegistration =
                    _context.CivilRegistrations.FirstOrDefault(i => i.PregnancyId == id && i.ChildId == child.Id);
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper()
                    .Map(civilRegistration, vm);

                var childFeedingInformation =
                    _context.ChildFeedingInformation.SingleOrDefault(
                        i => i.PregnancyId == id && i.ChildId == child.Id);
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper()
                    .Map(childFeedingInformation, vm);
                vm.ChildDevelopmentMilestones = _context.ChildDevelopmentMilestones.Where(i => i.ChildId == child.Id).ToList();
                vm.DOB = delivery.DeliveryDate;
                vm.GestationAtBirth = delivery.PregnancyDuration;

                vm.DeliveryPlaceId = vm.Children.First().DeliveryPlaceId;
            }
            else
            {
                TempData["Info"] = "Sorry, you cannot enter Child Monitoring data before Delivery Data.";
                string referer = Request.Headers["Referer"].ToString();
                return Redirect(referer);
            }

            ViewData["CountyId"] = new SelectList(_context.Counties.Where(i => i.Id == 45 || i.Id == 9001), "Id", "Name");
            ViewData["DistrictId"] = new SelectList(_context.Districts, "Id", "Name");
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities.OrderBy(i => i.Name), "Id", "Name", vm.HealthFacilityId);
            ViewData["ChildGenderId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Gender"), "Id", "Code");
            ViewData["ComplimentaryFoodIntroducedId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");
            ViewData["OtherFoodIntroducedId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");
            ViewData["BreastfeedingId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Breastfeeding"), "Id", "Code");
            ViewData["DeliveryPlaceId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Place of Delivery"), "Id", "Code");
            ViewBag.motherSupportStatusId = _context.Pregnancies.OrderByDescending(i => i.DateCreated)
                .First(i => i.Id == vm.PregnancyId).SupportStatusId;
            vm.StatusId = _context.Pregnancies.Find(id).StatusId;
            return View(vm);
        }

        [HttpPost]
        public IActionResult ChildParticulars(ChildParticularsViewModel vm)
        {
            var id = vm.PregnancyId;
            var child = _context.Children
                .Include(i => i.Gender)
                .FirstOrDefault(i => i.Id == vm.ChildId || vm.ChildId == "" && i.Delivery.PregnancyId == id && i.Id == vm.ChildId);

            if (ModelState.IsValid)
            {
                child.Name = vm.Name;
                child.GenderId = vm.GenderId;
                child.GestationAtBirth = vm.GestationAtBirth;
                child.DOB = vm.DOB;

                _uow.Save();
                TempData["Message"] = "Child particulars updated.";
                ViewBag.motherSupportStatusId = _context.Pregnancies.OrderByDescending(i => i.DateCreated)
                    .First(i => i.Id == vm.PregnancyId).SupportStatusId;
                return RedirectToAction("ChildHealthMonitoring", new { id = vm.PregnancyId });
            }

            if (!string.IsNullOrEmpty(vm.ChildId))
                vm.ChildId = vm.ChildId;

            new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(child, vm);
            var delivery = _context.Deliveries.FirstOrDefault(i => i.PregnancyId == id);

            if (delivery != null)
            {
                vm.Children = _context.Children
                    .Include(i => i.Gender)
                    .Include(i => i.Status)
                    .Include(i => i.Delivery.Pregnancy)
                    .OrderBy(i => i.BirthOrder)
                    .Where(i => i.DeliveryId == delivery.Id).ToList();
                ViewData["ChildId"] = new SelectList(vm.Children, "Id", "DisplayName");

                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(child, vm);

                var healthRecord =
                    _context.ChildHealthRecords.SingleOrDefault(i => i.PregnancyId == id && i.ChildId == child.Id);
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(healthRecord, vm);

                var civilRegistration =
                    _context.CivilRegistrations.FirstOrDefault(i => i.PregnancyId == id && i.ChildId == child.Id);
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper()
                    .Map(civilRegistration, vm);

                var childFeedingInformation =
                    _context.ChildFeedingInformation.SingleOrDefault(
                        i => i.PregnancyId == id && i.ChildId == child.Id);
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper()
                    .Map(childFeedingInformation, vm);
                vm.ChildDevelopmentMilestones = _context.ChildDevelopmentMilestones.Where(i => i.ChildId == child.Id).ToList();
                vm.DOB = delivery.DeliveryDate;
                vm.GestationAtBirth = delivery.PregnancyDuration;
                vm.HealthFacilityId = vm.Children.First().DeliveryHealthFacilityId;
                vm.DeliveryPlaceId = vm.Children.First().DeliveryPlaceId;
            }

            vm.DevelopmentMilestones = _context.DevelopmentMilestones;
            ViewData["CountyId"] = new SelectList(_context.Counties.Where(i => i.Id == 45 || i.Id == 9001), "Id", "Name");
            ViewData["DistrictId"] = new SelectList(_context.Districts, "Id", "Name");
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities.OrderBy(i => i.Name), "Id", "Name", vm.HealthFacilityId);
            ViewData["ChildGenderId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Gender"), "Id", "Code");
            ViewData["ComplimentaryFoodIntroducedId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");
            ViewData["OtherFoodIntroducedId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");
            ViewData["BreastfeedingId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Breastfeeding"), "Id", "Code");
            ViewData["DeliveryPlaceId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Place of Delivery"), "Id", "Code");
            ViewBag.motherSupportStatusId = _context.Pregnancies.OrderByDescending(i => i.DateCreated)
                .First(i => i.Id == vm.PregnancyId).SupportStatusId;
            var model = new ChildHealthMonitoringViewModel();
            new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper()
                .Map(vm, model);
            return View("ChildHealthMonitoring", model);
        }

        [HttpPost]
        public IActionResult HealthRecord(ChildHealthRecordViewModel vm)
        {
            var id = vm.PregnancyId;
            var child = _context.Children
                .Include(i => i.Gender)
                .FirstOrDefault(i => i.Id == vm.ChildId || vm.ChildId == "" && i.Delivery.PregnancyId == id && i.Id == vm.ChildId);

            if (ModelState.IsValid)
            {
                // Health record of child
                var healthRecord = new ChildHealthRecord();
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, healthRecord);
                healthRecord.ChildId = child.Id;
                healthRecord.CreatedById = User.GetUserId();
                healthRecord.DateCreated = DateTime.UtcNow.AddHours(3);

                if (_context.ChildHealthRecords.Any(i => i.PregnancyId == vm.PregnancyId && i.ChildId == vm.ChildId))
                    _uow.GetRepository<ChildHealthRecord>().Update(healthRecord);
                else
                    _uow.GetRepository<ChildHealthRecord>().Add(healthRecord);
                _uow.Save();
                TempData["Message"] = "Health Record updated.";
                ViewBag.motherSupportStatusId = _context.Pregnancies.OrderByDescending(i => i.DateCreated)
                    .First(i => i.Id == vm.PregnancyId).SupportStatusId;
                return RedirectToAction("ChildHealthMonitoring", new { id = vm.PregnancyId });
            }

            if (!string.IsNullOrEmpty(vm.ChildId))
                vm.ChildId = vm.ChildId;

            new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(child, vm);
            var delivery = _context.Deliveries.FirstOrDefault(i => i.PregnancyId == id);

            if (delivery != null)
            {
                vm.Children = _context.Children
                    .Include(i => i.Gender)
                    .Include(i => i.Status)
                    .Include(i => i.Delivery.Pregnancy)
                    .OrderBy(i => i.BirthOrder)
                    .Where(i => i.DeliveryId == delivery.Id).ToList();
                ViewData["ChildId"] = new SelectList(vm.Children, "Id", "DisplayName");

                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(child, vm);

                var healthRecord =
                    _context.ChildHealthRecords.SingleOrDefault(i => i.PregnancyId == id && i.ChildId == child.Id);
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(healthRecord, vm);

                var civilRegistration =
                    _context.CivilRegistrations.FirstOrDefault(i => i.PregnancyId == id && i.ChildId == child.Id);
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper()
                    .Map(civilRegistration, vm);

                var childFeedingInformation =
                    _context.ChildFeedingInformation.SingleOrDefault(
                        i => i.PregnancyId == id && i.ChildId == child.Id);
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper()
                    .Map(childFeedingInformation, vm);
                vm.ChildDevelopmentMilestones = _context.ChildDevelopmentMilestones.Where(i => i.ChildId == child.Id).ToList();
                vm.DOB = delivery.DeliveryDate;
                vm.GestationAtBirth = delivery.PregnancyDuration;
                vm.HealthFacilityId = vm.Children.First().DeliveryHealthFacilityId;
                vm.DeliveryPlaceId = vm.Children.First().DeliveryPlaceId;
            }

            vm.DevelopmentMilestones = _context.DevelopmentMilestones;
            ViewData["CountyId"] = new SelectList(_context.Counties.Where(i => i.Id == 45 || i.Id == 9001), "Id", "Name");
            ViewData["DistrictId"] = new SelectList(_context.Districts, "Id", "Name");
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities.OrderBy(i => i.Name), "Id", "Name", vm.HealthFacilityId);
            ViewData["ChildGenderId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Gender"), "Id", "Code");
            ViewData["ComplimentaryFoodIntroducedId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");
            ViewData["OtherFoodIntroducedId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");
            ViewData["BreastfeedingId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Breastfeeding"), "Id", "Code");
            ViewData["DeliveryPlaceId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Place of Delivery"), "Id", "Code");
            ViewBag.motherSupportStatusId = _context.Pregnancies.OrderByDescending(i => i.DateCreated)
                .First(i => i.Id == vm.PregnancyId).SupportStatusId;
            var model = new ChildHealthMonitoringViewModel();
            new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper()
                .Map(vm, model);
            return View("ChildHealthMonitoring", model);
        }

        [HttpPost]
        public async Task<IActionResult> CivilRegistration(CivilRegistrationViewModel vm, IFormFile birthCertificateFile)
        {
            var id = vm.PregnancyId;
            var child = _context.Children
                .Include(i => i.Gender)
                .FirstOrDefault(i => i.Id == vm.ChildId || vm.ChildId == "" && i.Delivery.PregnancyId == id && i.Id == vm.ChildId);

            if (ModelState.IsValid)
            {
                //Civil Registration - Mother and child
                var civilRegistration = _context.CivilRegistrations
                    .SingleOrDefault(i => i.PregnancyId == vm.PregnancyId && i.ChildId == vm.ChildId);

                if (civilRegistration == null)
                    civilRegistration = new CivilRegistration();

                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, civilRegistration);
                civilRegistration.ChildId = child.Id;

                if (birthCertificateFile != null && birthCertificateFile.Length > 0)
                {
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    string path = "";
                    ViewBag.Id = id;

                    var fileName = DateTime.UtcNow.AddHours(3).ToString("yyyymmddhhmmss") + "-";
                    fileName = fileName + birthCertificateFile.FileName;
                    path = webRootPath + "/uploads/birthcertificates/" + Path.GetFileName(fileName);
                    var stream = new FileStream(path, FileMode.Create);
                    await birthCertificateFile.CopyToAsync(stream);
                   
                    civilRegistration.BirthCertificate = fileName;
                }

                TempData["Message"] = "Civil Registration Details Updated.";
                if (_context.CivilRegistrations.Any(i => i.PregnancyId == vm.PregnancyId && i.ChildId == vm.ChildId))
                    _uow.GetRepository<CivilRegistration>().Update(civilRegistration);
                else
                    _uow.GetRepository<CivilRegistration>().Add(civilRegistration);
                _uow.Save();
                return RedirectToAction("ChildHealthMonitoring", new { id = vm.PregnancyId });
            }

            if (!string.IsNullOrEmpty(vm.ChildId))
                vm.ChildId = vm.ChildId;

            new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(child, vm);
            var delivery = _context.Deliveries.FirstOrDefault(i => i.PregnancyId == id);

            if (delivery != null)
            {
                vm.Children = _context.Children
                    .Include(i => i.Gender)
                    .Include(i => i.Status)
                    .Include(i => i.Delivery.Pregnancy)
                    .OrderBy(i => i.BirthOrder)
                    .Where(i => i.DeliveryId == delivery.Id).ToList();
                ViewData["ChildId"] = new SelectList(vm.Children, "Id", "DisplayName");

                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(child, vm);

                var healthRecord =
                    _context.ChildHealthRecords.SingleOrDefault(i => i.PregnancyId == id && i.ChildId == child.Id);
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(healthRecord, vm);

                var civilRegistration =
                    _context.CivilRegistrations.FirstOrDefault(i => i.PregnancyId == id && i.ChildId == child.Id);
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper()
                    .Map(civilRegistration, vm);

                var childFeedingInformation =
                    _context.ChildFeedingInformation.SingleOrDefault(
                        i => i.PregnancyId == id && i.ChildId == child.Id);
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper()
                    .Map(childFeedingInformation, vm);
                vm.ChildDevelopmentMilestones = _context.ChildDevelopmentMilestones.Where(i => i.ChildId == child.Id).ToList();
                vm.DOB = delivery.DeliveryDate;
                vm.GestationAtBirth = delivery.PregnancyDuration;
                vm.HealthFacilityId = vm.Children.First().DeliveryHealthFacilityId;
                vm.DeliveryPlaceId = vm.Children.First().DeliveryPlaceId;
            }

            vm.DevelopmentMilestones = _context.DevelopmentMilestones;
            ViewData["CountyId"] = new SelectList(_context.Counties.Where(i => i.Id == 45 || i.Id == 9001), "Id", "Name");
            ViewData["DistrictId"] = new SelectList(_context.Districts, "Id", "Name");
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities.OrderBy(i => i.Name), "Id", "Name", vm.HealthFacilityId);
            ViewData["ChildGenderId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Gender"), "Id", "Code");
            ViewData["ComplimentaryFoodIntroducedId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");
            ViewData["OtherFoodIntroducedId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");
            ViewData["BreastfeedingId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Breastfeeding"), "Id", "Code");
            ViewData["DeliveryPlaceId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Place of Delivery"), "Id", "Code");
            ViewBag.motherSupportStatusId = _context.Pregnancies.OrderByDescending(i => i.DateCreated)
                .First(i => i.Id == vm.PregnancyId).SupportStatusId;
            var model = new ChildHealthMonitoringViewModel();
            new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper()
                .Map(vm, model);
            return View("ChildHealthMonitoring", model);
        }

        [HttpPost]
        public IActionResult FeedingInformation(ChildFeedingInformationViewModel vm)
        {
            var id = vm.PregnancyId;
            var child = _context.Children
                .Include(i => i.Gender)
                .FirstOrDefault(i => i.Id == vm.ChildId || vm.ChildId == "" && i.Delivery.PregnancyId == id && i.Id == vm.ChildId);

            if (ModelState.IsValid)
            {
                //  Feeding information from parent/guardian
                var childFeedingInformation = new ChildFeedingInformation();
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, childFeedingInformation);
                childFeedingInformation.ChildId = child.Id;
                if (_context.ChildFeedingInformation.Any(i => i.PregnancyId == vm.PregnancyId && i.ChildId == vm.ChildId))
                    _uow.GetRepository<ChildFeedingInformation>().Update(childFeedingInformation);
                else
                    _uow.GetRepository<ChildFeedingInformation>().Add(childFeedingInformation);
                _uow.Save();
                TempData["Message"] = "Feeding information updated.";
                ViewBag.motherSupportStatusId = _context.Pregnancies.OrderByDescending(i => i.DateCreated)
                    .First(i => i.Id == vm.PregnancyId).SupportStatusId;
                return RedirectToAction("ChildHealthMonitoring", new { id = vm.PregnancyId });
            }

            if (!string.IsNullOrEmpty(vm.ChildId))
                vm.ChildId = vm.ChildId;

            new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(child, vm);
            var delivery = _context.Deliveries.FirstOrDefault(i => i.PregnancyId == id);

            if (delivery != null)
            {
                vm.Children = _context.Children
                    .Include(i => i.Gender)
                    .Include(i => i.Status)
                    .Include(i => i.Delivery.Pregnancy)
                    .OrderBy(i => i.BirthOrder)
                    .Where(i => i.DeliveryId == delivery.Id).ToList();
                ViewData["ChildId"] = new SelectList(vm.Children, "Id", "DisplayName");

                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(child, vm);

                var healthRecord =
                    _context.ChildHealthRecords.SingleOrDefault(i => i.PregnancyId == id && i.ChildId == child.Id);
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(healthRecord, vm);

                var civilRegistration =
                    _context.CivilRegistrations.FirstOrDefault(i => i.PregnancyId == id && i.ChildId == child.Id);
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper()
                    .Map(civilRegistration, vm);

                var childFeedingInformation =
                    _context.ChildFeedingInformation.SingleOrDefault(
                        i => i.PregnancyId == id && i.ChildId == child.Id);
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper()
                    .Map(childFeedingInformation, vm);
                vm.ChildDevelopmentMilestones = _context.ChildDevelopmentMilestones.Where(i => i.ChildId == child.Id).ToList();
                vm.DOB = delivery.DeliveryDate;
                vm.GestationAtBirth = delivery.PregnancyDuration;
                vm.HealthFacilityId = vm.Children.First().DeliveryHealthFacilityId;
                vm.DeliveryPlaceId = vm.Children.First().DeliveryPlaceId;
            }

            vm.DevelopmentMilestones = _context.DevelopmentMilestones;
            ViewData["CountyId"] = new SelectList(_context.Counties.Where(i => i.Id == 45 || i.Id == 9001), "Id", "Name");
            ViewData["DistrictId"] = new SelectList(_context.Districts, "Id", "Name");
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities.OrderBy(i => i.Name), "Id", "Name", vm.HealthFacilityId);
            ViewData["ChildGenderId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Gender"), "Id", "Code");
            ViewData["ComplimentaryFoodIntroducedId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");
            ViewData["OtherFoodIntroducedId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");
            ViewData["BreastfeedingId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Breastfeeding"), "Id", "Code");
            ViewData["DeliveryPlaceId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Place of Delivery"), "Id", "Code");
            ViewBag.motherSupportStatusId = _context.Pregnancies.OrderByDescending(i => i.DateCreated)
                .First(i => i.Id == vm.PregnancyId).SupportStatusId;
            var model = new ChildHealthMonitoringViewModel();
            new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper()
                .Map(vm, model);
            return View("ChildHealthMonitoring", model);
        }

        [HttpPost]
        public IActionResult DevelopmentMilestones(DevelopmentalMilestonesViewModel vm)
        {
            var id = vm.PregnancyId;
            var child = _context.Children
                .Include(i => i.Gender)
                .FirstOrDefault(i => i.Id == vm.ChildId || vm.ChildId == "" && i.Delivery.PregnancyId == id && i.Id == vm.ChildId);

            if (ModelState.IsValid)
            {
                // Add Developmental Milestones
                bool proceed = true;
                if (vm.Ids != null && vm.Ids.Any())
                {
                    // Remove Previous
                    var previous = _context.ChildDevelopmentMilestones.Where(i => i.ChildId == vm.ChildId);
                    if (previous.Any())
                    {
                        _context.ChildDevelopmentMilestones.RemoveRange(previous);
                    }

                    try
                    {
                        int count = 0;
                        List<ChildDevelopmentMilestone> milestones = new List<ChildDevelopmentMilestone>();
                        foreach (var milestoneId in vm.Ids)
                        {
                            var milestone = new ChildDevelopmentMilestone
                            {
                                DevelopmentMilestoneId = milestoneId,
                                ChildId = vm.ChildId,
                                AgeAchieved = int.Parse(vm.ChildAges[count])
                            };
                            milestones.Add(milestone);
                            _uow.GetRepository<ChildDevelopmentMilestone>().AddRange(milestones);
                            count++;
                        }
                    }
                    catch (Exception e)
                    {
                        proceed = false;
                        TempData["Error"] = "<strong>Age Achieved</strong> is required for all the developmental milestones achieved.";
                    }
                }

                if (proceed)
                {
                    _uow.Save();
                    TempData["Message"] = "Developmental Milestone (s) saved.";
                    ViewBag.motherSupportStatusId = _context.Pregnancies.OrderByDescending(i => i.DateCreated)
                        .First(i => i.Id == vm.PregnancyId).SupportStatusId;
                    return RedirectToAction("ChildHealthMonitoring", new { id = vm.PregnancyId });
                }
            }

            if (!string.IsNullOrEmpty(vm.ChildId))
                vm.ChildId = vm.ChildId;

            new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(child, vm);
            var delivery = _context.Deliveries.FirstOrDefault(i => i.PregnancyId == id);

            if (delivery != null)
            {
                vm.Children = _context.Children
                    .Include(i => i.Gender)
                    .Include(i => i.Status)
                    .Include(i => i.Delivery.Pregnancy)
                    .OrderBy(i => i.BirthOrder)
                    .Where(i => i.DeliveryId == delivery.Id).ToList();
                ViewData["ChildId"] = new SelectList(vm.Children, "Id", "DisplayName");

                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(child, vm);

                var healthRecord =
                    _context.ChildHealthRecords.SingleOrDefault(i => i.PregnancyId == id && i.ChildId == child.Id);
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(healthRecord, vm);

                var civilRegistration =
                    _context.CivilRegistrations.FirstOrDefault(i => i.PregnancyId == id && i.ChildId == child.Id);
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper()
                    .Map(civilRegistration, vm);

                var childFeedingInformation =
                    _context.ChildFeedingInformation.SingleOrDefault(
                        i => i.PregnancyId == id && i.ChildId == child.Id);
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper()
                    .Map(childFeedingInformation, vm);
                vm.ChildDevelopmentMilestones = _context.ChildDevelopmentMilestones.Where(i => i.ChildId == child.Id).ToList();
                vm.DOB = delivery.DeliveryDate;
                vm.GestationAtBirth = delivery.PregnancyDuration;
                vm.HealthFacilityId = vm.Children.First().DeliveryHealthFacilityId;
                vm.DeliveryPlaceId = vm.Children.First().DeliveryPlaceId;
            }

            vm.DevelopmentMilestones = _context.DevelopmentMilestones;
            ViewData["CountyId"] = new SelectList(_context.Counties.Where(i => i.Id == 45 || i.Id == 9001), "Id", "Name");
            ViewData["DistrictId"] = new SelectList(_context.Districts, "Id", "Name");
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities.OrderBy(i => i.Name), "Id", "Name", vm.HealthFacilityId);
            ViewData["ChildGenderId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Gender"), "Id", "Code");
            ViewData["ComplimentaryFoodIntroducedId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");
            ViewData["OtherFoodIntroducedId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");
            ViewData["BreastfeedingId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Breastfeeding"), "Id", "Code");
            ViewData["DeliveryPlaceId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Place of Delivery"), "Id", "Code");
            ViewBag.motherSupportStatusId = _context.Pregnancies.OrderByDescending(i => i.DateCreated)
                .First(i => i.Id == vm.PregnancyId).SupportStatusId;
            var model = new ChildHealthMonitoringViewModel();
            new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper()
                .Map(vm, model);
            return View("ChildHealthMonitoring", model);
        }

        [HttpPost]
        public async Task<IActionResult> ChildHealthMonitoring(ChildHealthMonitoringViewModel vm, IFormFile birthCertificateFile)
        {
            var id = vm.PregnancyId;
            var child = _context.Children
                .Include(i => i.Gender)
                .FirstOrDefault(i => i.Id == vm.ChildId || vm.ChildId == "" && i.Delivery.PregnancyId == id && i.Id == vm.ChildId);

            if (ModelState.IsValid)
            {
                child.Name = vm.Name;
                child.GenderId = vm.GenderId;
                child.GestationAtBirth = vm.GestationAtBirth;
                child.DOB = vm.DOB;

                // Health record of child
                var healthRecord = new ChildHealthRecord();
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, healthRecord);
                healthRecord.ChildId = child.Id;
                healthRecord.CreatedById = User.GetUserId();
                healthRecord.DateCreated = DateTime.UtcNow.AddHours(3);

                if (_context.ChildHealthRecords.Any(i => i.PregnancyId == vm.PregnancyId && i.ChildId == vm.ChildId))
                    _uow.GetRepository<ChildHealthRecord>().Update(healthRecord);
                else
                    _uow.GetRepository<ChildHealthRecord>().Add(healthRecord);

                //Civil Registration - Mother and child
                var civilRegistration = _context.CivilRegistrations
                    .SingleOrDefault(i => i.PregnancyId == vm.PregnancyId && i.ChildId == vm.ChildId);

                if (civilRegistration == null)
                    civilRegistration = new CivilRegistration();

                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, civilRegistration);
                civilRegistration.ChildId = child.Id;

                if (birthCertificateFile != null && birthCertificateFile.Length > 0)
                {
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    string path = "";
                    ViewBag.Id = id;

                    var fileName = DateTime.UtcNow.AddHours(3).ToString("yyyymmddhhmmss") + "-";
                    fileName = fileName + birthCertificateFile.FileName;
                    path = webRootPath + "/uploads/birthcertificates/" + Path.GetFileName(fileName);
                    var stream = new FileStream(path, FileMode.Create);
                    await birthCertificateFile.CopyToAsync(stream);
                   
                    civilRegistration.BirthCertificate = fileName;
                }

                if (_context.CivilRegistrations.Any(i => i.PregnancyId == vm.PregnancyId && i.ChildId == vm.ChildId))
                    _uow.GetRepository<CivilRegistration>().Update(civilRegistration);
                else
                    _uow.GetRepository<CivilRegistration>().Add(civilRegistration);

                //  Feeding information from parent/guardian
                var childFeedingInformation = new ChildFeedingInformation();
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, childFeedingInformation);
                childFeedingInformation.ChildId = child.Id;
                if (_context.ChildFeedingInformation.Any(i => i.PregnancyId == vm.PregnancyId && i.ChildId == vm.ChildId))
                    _uow.GetRepository<ChildFeedingInformation>().Update(childFeedingInformation);
                else
                    _uow.GetRepository<ChildFeedingInformation>().Add(childFeedingInformation);
                // Add Developmental Milestones
                bool proceed = true;
                if (vm.Ids != null && vm.Ids.Any())
                {
                    // Remove Previous
                    var previous = _context.ChildDevelopmentMilestones.Where(i => i.ChildId == vm.ChildId);
                    if (previous.Any())
                    {
                        _context.ChildDevelopmentMilestones.RemoveRange(previous);
                    }

                    try
                    {
                        int count = 0;
                        List<ChildDevelopmentMilestone> milestones = new List<ChildDevelopmentMilestone>();
                        foreach (var milestoneId in vm.Ids)
                        {
                            var milestone = new ChildDevelopmentMilestone
                            {
                                DevelopmentMilestoneId = milestoneId,
                                ChildId = vm.ChildId,
                                AgeAchieved = int.Parse(vm.ChildAges[count])
                            };
                            milestones.Add(milestone);
                            _uow.GetRepository<ChildDevelopmentMilestone>().AddRange(milestones);
                            count++;
                        }
                    }
                    catch (Exception e)
                    {
                        proceed = false;
                        TempData["Error"] = "<strong>Age Achieved</strong> is required for all the developmental milestones achieved.";
                    }
                }

                if (proceed)
                {
                    _uow.Save();
                    TempData["Message"] = "Child data saved.";
                    ViewBag.motherSupportStatusId = _context.Pregnancies.OrderByDescending(i => i.DateCreated)
                        .First(i => i.Id == vm.PregnancyId).SupportStatusId;
                    return RedirectToAction("ChildHealthMonitoring", new { id = vm.PregnancyId });
                }
            }

            if (!string.IsNullOrEmpty(vm.ChildId))
                vm.ChildId = vm.ChildId;

            new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(child, vm);
            var delivery = _context.Deliveries.FirstOrDefault(i => i.PregnancyId == id);

            if (delivery != null)
            {
                vm.Children = _context.Children
                    .Include(i => i.Gender)
                    .Include(i => i.Status)
                    .Include(i => i.Delivery.Pregnancy)
                    .OrderBy(i => i.BirthOrder)
                    .Where(i => i.DeliveryId == delivery.Id).ToList();
                ViewData["ChildId"] = new SelectList(vm.Children, "Id", "DisplayName");

                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(child, vm);

                var healthRecord =
                    _context.ChildHealthRecords.SingleOrDefault(i => i.PregnancyId == id && i.ChildId == child.Id);
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(healthRecord, vm);

                var civilRegistration =
                    _context.CivilRegistrations.FirstOrDefault(i => i.PregnancyId == id && i.ChildId == child.Id);
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper()
                    .Map(civilRegistration, vm);

                var childFeedingInformation =
                    _context.ChildFeedingInformation.SingleOrDefault(
                        i => i.PregnancyId == id && i.ChildId == child.Id);
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper()
                    .Map(childFeedingInformation, vm);
                vm.ChildDevelopmentMilestones = _context.ChildDevelopmentMilestones.Where(i => i.ChildId == child.Id).ToList();
                vm.DOB = delivery.DeliveryDate;
                vm.GestationAtBirth = delivery.PregnancyDuration;
                vm.HealthFacilityId = vm.Children.First().DeliveryHealthFacilityId;
                vm.DeliveryPlaceId = vm.Children.First().DeliveryPlaceId;
            }

            vm.DevelopmentMilestones = _context.DevelopmentMilestones;
            ViewData["CountyId"] = new SelectList(_context.Counties.Where(i => i.Id == 45 || i.Id == 9001), "Id", "Name");
            ViewData["DistrictId"] = new SelectList(_context.Districts, "Id", "Name");
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities.OrderBy(i => i.Name), "Id", "Name", vm.HealthFacilityId);
            ViewData["ChildGenderId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Gender"), "Id", "Code");
            ViewData["ComplimentaryFoodIntroducedId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");
            ViewData["OtherFoodIntroducedId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Yes No Choices"), "Id", "Code");
            ViewData["BreastfeedingId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Breastfeeding"), "Id", "Code");
            ViewData["DeliveryPlaceId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Place of Delivery"), "Id", "Code");
            ViewBag.motherSupportStatusId = _context.Pregnancies.OrderByDescending(i => i.DateCreated)
                .First(i => i.Id == vm.PregnancyId).SupportStatusId;
            return View(vm);
        }

        public IActionResult Details(string id)
        {
            var vm = new CaseManagementDetailsViewModel();
            var pregnancy = _context.Pregnancies.Find(id);
            var caseManagement = _context.CaseManagement.Find(pregnancy.CaseManagementId);
            vm.Household = _context.HouseholdRegs
                .Include(i => i.Mother)
                .SingleOrDefault(i => i.Id == caseManagement.HouseholdId);
            vm.Pregnancy = _context.Pregnancies
                .Include(i => i.BloodGroup)
                .Include(i => i.BreastfeedingCounselingDone)
                .Include(i => i.InfantFeedingCounselingDone)
                .Include(i => i.Rhesus)
                .SingleOrDefault(i => i.CaseManagementId == id);

            vm.PregnancyDataList = _context.PregnancyData
                .OrderBy(i => i.ClinicVisitId)
                .Include(i => i.MotherClinicVisit)
                .Include(i => i.ClinicVisit)
                .Where(i => i.PregnancyId == id).ToList();

            vm.Delivery = _context.Deliveries
                .Include(i => i.DeliveryMode)
                .Include(i => i.BloodLoss)
                .Include(i => i.PregnancyOutcome)
                //.Include(i => i.PreEclampsia)
                //.Include(i => i.Eclampsia)
                .Include(i => i.ObstructedLabour)

                .Include(i => i.MeconiumStainedLiquor)
                .SingleOrDefault(i => i.PregnancyId == id);
            if (vm.Delivery != null)
            {
                vm.MotherDrugsAdministered = _context.DrugsAdministered
                    .Include(i => i.Drug)
                    .Where(i => i.RecipientTypeId == 1 && i.DeliveryId == vm.Delivery.Id);

                vm.BabyDrugsAdministered = _context.DrugsAdministered
                    .Include(i => i.Drug)
                    .Where(i => i.RecipientTypeId == 2 && i.DeliveryId == vm.Delivery.Id);

                vm.Children = _context.Children
                    .OrderBy(i => i.BirthOrder)
                    .Include(i => i.DeliveryAssistant)
                    .Include(i => i.DeliveryPlace)
                    .Include(i => i.Gender)
                    .Where(i => i.DeliveryId == vm.Delivery.Id);
            }

            //var services = _context.MotherPreventiveServices
            //    .Include(i => i.PreventiveService)
            //    .OrderBy(i => i.PreventiveServiceId)
            //    .Where(i => i.CaseManagementId == id);

            //  vm.MotherPreventiveServices = services;

            vm.FamilyPlannings = _context.FamilyPlannings
                .OrderByDescending(i => i.DateCreated)
                .Where(i => i.PregnancyId == id);
            vm.PostNatalExaminations = _context.PostNatalExaminations
                .Include(i => i.FPMethod)
                .Include(i => i.FPCounseling)
                .Include(i => i.Details)
                .OrderByDescending(i => i.Id)
                .Where(i => i.PregnancyId == id);
            vm.HealthRecords = _context.ChildHealthRecords.Where(i => i.PregnancyId == id).ToList();
            vm.CivilRegistrations = _context.CivilRegistrations.Where(i => i.PregnancyId == id).ToList();
            vm.FeedingInformation = _context.ChildFeedingInformation.Where(i => i.PregnancyId == id).ToList();

            vm.PostNatalExaminationDetails = _context.PostNatalExaminationDetails
                .Include(i => i.FeedingMethod)
                .Where(i => i.PostNatalExamination.PregnancyId == id);

            vm.HealthRecord = _context.ChildHealthRecords
                .Include(i => i.DeliveryPlace)
                 .Include(i => i.HealthFacility)
                .FirstOrDefault(i => i.PregnancyId == id);

            vm.CivilRegistration = _context.CivilRegistrations
                .FirstOrDefault(i => i.PregnancyId == id);

            vm.ChildFeedingInformation = _context.ChildFeedingInformation
                .Include(i => i.Breastfeeding)
                .Include(i => i.ComplimentaryFoodIntroduced)
                .Include(i => i.OtherFoodIntroduced)
                .FirstOrDefault(i => i.PregnancyId == id);

            vm.PMTCTServices = _context.PMTCTServices.Include(i => i.PMTCTInterventions)
                .Where(i => i.PregnancyId == id).ToList();

            vm.PMTCTInterventions = _context.PMTCTInterventions
                .Include(i => i.Intervention)
                .Include(i => i.PMTCTService)
                .Where(i => i.PMTCTService.PregnancyId == id).ToList();

            return View(vm);
        }
    }
}