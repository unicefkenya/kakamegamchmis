using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ClosedXML.Excel;
using ClosedXML.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MCHMIS.Data;
using MCHMIS.Extensions;
using MCHMIS.ViewModels;
using ExcelDataReader;
using MCHMIS.Interfaces;
using MCHMIS.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using X.PagedList;

namespace MCHMIS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EnrolmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IUnitOfWork _uow;
        private readonly IDBService _dbService;
        private readonly ISMSService _smsService;

        public EnrolmentController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment,
            IUnitOfWork uow, IDBService dbService, ISMSService smsService)
        {
            _context = context;
            _uow = uow;
            _hostingEnvironment = hostingEnvironment;
            _dbService = dbService;
            _smsService = smsService;
        }

        public async Task<IActionResult> Index(BeneficiaryListViewModel vm)
        {
            var beneficiaries = _context.Beneficiaries
                .Include(r => r.Household.Village.Ward.SubCounty)
                .Include(r => r.Status)
                .Include(r => r.HealthFacility)
                .OrderByDescending(r => r.DateEnrolled)
                .AsQueryable();
            var healthFacilityId = _dbService.GetHealthFacilityId();
            if (healthFacilityId != 0)
            {
                bool isGlobal = await _dbService.IsGlobal();
                beneficiaries = beneficiaries.Where(i => i.Household.HealthFacilityId == healthFacilityId || isGlobal);
            }
            if (vm.HealthFacilityId != null)
            {
                beneficiaries = beneficiaries.Where(i => i.Household.HealthFacilityId == vm.HealthFacilityId);
            }
            if (!string.IsNullOrEmpty(vm.UniqueId))
            {
                beneficiaries = beneficiaries.Where(h => h.UniqueId == vm.UniqueId);
            }
            if (!string.IsNullOrEmpty(vm.IdNumber))
            {
                beneficiaries = beneficiaries.Where(h => h.IdNumber.Contains(vm.IdNumber));
            }
            if (!string.IsNullOrEmpty(vm.Name))
            {
                beneficiaries = beneficiaries.Where(h =>
                    h.BeneficiaryName.Contains(vm.Name)
                );
            }
            if (vm.StatusId != null)
            {
                beneficiaries = beneficiaries.Where(h => h.StatusId == vm.StatusId);
            }
            if (vm.WardId != null)
            {
                beneficiaries = beneficiaries.Where(h => h.Household.Village.WardId == vm.WardId);
            }
            if (vm.SubCountyId != null)
            {
                beneficiaries = beneficiaries.Where(h => h.Household.Village.Ward.SubCountyId == vm.SubCountyId);
            }
            if (vm.HealthFacilityId != null)
            {
                beneficiaries = beneficiaries.Where(h => h.HealthFacilityId == vm.HealthFacilityId);
            }
            var page = vm.Page ?? 1;
            var pageSize = vm.PageSize ?? 20;

            vm.Beneficiaries = beneficiaries.ToPagedList(page, pageSize);
            List<int> benStatusIds = new List<int> { 19, 27, 28 };

            ViewData["StatusId"] = new SelectList(_context.Status.Where(i => benStatusIds.Contains(i.Id)), "Id", "Name", vm.StatusId);
            if (vm.StatusId != null)
            {
                vm.Status = _context.Status.Find(vm.StatusId).Name;
            }
            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name", vm.SubCountyId);
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities, "Id", "Name", vm.HealthFacilityId);

            vm.Wards = await _context.Wards.ToListAsync();
            return View(vm);
        }

        public async Task<IActionResult> Lists()
        {
            var applicationDbContext = _context.Enrolments.OrderByDescending(i => i.Id)
                .Include(e => e.CreatedBy)
                .Include(e => e.ApprovedBy)
                .Include(e => e.Status)
                .Include(e => e.ImportStatus)
                .Include(e => e.ImportedBy);
            return View(await applicationDbContext.ToListAsync());
        }

        public ActionResult GenerateList()
        {
            ViewBag.Count = _context.HouseholdRegs.Count(i => i.StatusId == 10); // Ready for Enrolment;
          
            ViewBag.Updated = _context.HouseholdRegs.Count(i => (i.StatusId == 17 || i.StatusId == 18) && i.RequiresMPESACheck == true); // Ready for Enrolment;
            return View();
        }

        public async Task<IActionResult> GenerateListConfirmed(EnrolmentGenerateListViewModel vm)
        {
            if (vm.StatusIds == null || vm.StatusIds.Length == 0)
            {
                TempData["Info"] = "No mother's category selected for enrolment.";
                return RedirectToAction("Index");
            }

            var enrolment = new Enrolment
            {
                CreatedById = User.GetUserId(),
                DateCreated = DateTime.UtcNow.AddHours(3),
                StatusId = 1,
                Notes = vm.Notes
            };
            _context.Add(enrolment);
            await _context.SaveChangesAsync();
            var rowsAffected = _context.Database.ExecuteSqlCommand("EnrolmentDetailsInsert  @EnrolmentId,@Ready,@Updated,@NoToGenerate",
                new SqlParameter("EnrolmentId", enrolment.Id),
                new SqlParameter("Ready", vm.StatusIds.Contains(1)),
                new SqlParameter("Updated", vm.StatusIds.Contains(2)),
                new SqlParameter("NoToGenerate", vm.NoToGenerate)
            );
            TempData["Message"] = "Enrolment list generated successfully";
            return RedirectToAction("Lists");
        }

        public async Task<IActionResult> Details(int id)
        {
            var details = _context.EnrolmentDetails.Where(i => i.EnrolmentId == id)
                .Include(e => e.Enrolment).Include(e => e.Household).Include(e => e.Status);
            var vm = new EnrolmentListDetailsViewModel
            {
                Enrolment = _context.Enrolments.Find(id),
                EnrolmentDetails = details,
                Id = id
            };
            ViewBag.ActionId = new SelectList(_context.SystemCodeDetails.Where(i=>i.SystemCode.Code== "Exception Actions"), "Id", "Code");
            return View(vm);
        }

        public async Task<IActionResult> Export(int id)
        {
            var details = _context.EnrolmentDetails.Where(i => i.EnrolmentId == id);
            var data = details.Select(b => new EnrolmentListExportViewModel
            {
                CreditIdentityStringType = "MSISDN",
                CreditIdentityString = "254" + b.RecipientPhone.Substring(b.RecipientPhone.Length - 9, 9),
                Comment = "Enrolment"
            }).ToList();
            // exportService.ExportToExcel(data, reportTitle);
            var wb = new XLWorkbook();

            // Add all DataTables in the DataSet as a worksheets
            var ds = new DataSet();
            var ws = wb.Worksheets.Add("Enrolment Details");
            var reportTitle = "Enrolment List Batch#" + id + ".xlsx";
            ws.Cell(1, 1).InsertTable(data.AsEnumerable());
            ws.Columns().AdjustToContents();
            var xlTable = ws.Tables.FirstOrDefault();
            if (xlTable != null) xlTable.ShowAutoFilter = false;
            return wb.Deliver(reportTitle);
        }

        public IActionResult ExportMismatch(int id)
        {
            var details = _context.EnrolmentDetails
                .Include(i => i.Status)
                .Include(i => i.Household.Village.Ward.SubCounty)
                .Include(i => i.Household.HealthFacility)
                .Include(i => i.Household.CommunityArea)
                .Where(i => i.EnrolmentId == id && i.StatusId != 16);
            var data = details.Select(b => new EnrolmentListMismatchExportViewModel
            {
                MotherUniqueId = b.Household.UniqueId,
                RecipientPhone = b.RecipientPhone,
                RecipientNames = b.RecipientNames,
                CustomerName = b.CustomerName,
                CustomerType = b.CustomerType,
                HealthFacility = b.Household.HealthFacility.Name,
                SubCounty = b.Household.Village.Ward.SubCounty.Name,
                Ward = b.Household.Village.Ward.Name,
                VillageUnit = b.Household.Village.Name,
                CommunityArea = b.Household.CommunityArea.Name,
                Issue = b.Status.Name
            }).ToList();
            // exportService.ExportToExcel(data, reportTitle);
            var wb = new XLWorkbook();

            // Add all DataTables in the DataSet as a worksheets
            var ds = new DataSet();
            var ws = wb.Worksheets.Add("Mismatched Beneficiaries");
            var reportTitle = "Mismatched Beneficiaries for List Batch#" + id + ".xlsx";
            ws.Cell(1, 1).InsertTable(data.AsEnumerable());
            ws.Columns().AdjustToContents();
            var xlTable = ws.Tables.FirstOrDefault();
            if (xlTable != null) xlTable.ShowAutoFilter = false;
            return wb.Deliver(reportTitle);
        }

        public IActionResult Import(int id)
        {
            var vm = new EnrolmentListImportViewModel();
            vm.Id = id;
            return View(vm);
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
                new SqlParameter("EnrolmentId", id)
            );
            var enrolment = _context.Enrolments.Find(id);
            enrolment.ImportStatusId = 1;
            enrolment.ImportedById = User.GetUserId();
            enrolment.DateImported = DateTime.UtcNow.AddHours(3);
            _uow.GetRepository<Enrolment>().Update(enrolment);
            _uow.Save();

            // Attempt to resolve recipient names in different order
            var mothers = _context.EnrolmentDetails.Where(i => i.EnrolmentId == id && i.StatusId == 18).ToList();
            if (mothers.Any())
            {
                foreach (var mother in mothers)
                {
                    var original = mother.RecipientNames.Split(' ');

                    var count = 0;
                    foreach (var item in original)
                    {
                        if (mother.CustomerName.ToLower().Contains(item.ToLower()))
                        {
                            count++;
                        }
                    }

                    if (count > 1)
                    {
                        mother.StatusId = 16;
                        _context.Update(mother);
                    }
                }
                _context.SaveChanges();
            }
            enrolment.HouseholdsValidated = _context.EnrolmentDetails.Count(i => i.StatusId == 16 && i.EnrolmentId == id);
            _context.Update(enrolment);
            _context.SaveChanges();
            return RedirectToAction("Details", new { Id = id });
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

        public async Task<IActionResult> Summary(int id)
        {
            SqlParameter[] parms = new SqlParameter[] { new SqlParameter("@EnrolmentId", id) };
            var result = SQLExtensions.GetModelFromQuery<EnrolmentListSummaryViewModel>(_context, "EXEC [EnrolmentListSummary] @EnrolmentId", parms);
            ViewBag.Id = id;
            return View(result);
        }

        public IActionResult SendForApproval(EnrolmentApprovalsViewModel vm)
        {
            vm.Enrolment = _context.Enrolments
                .Include(l => l.CreatedBy)
                .SingleOrDefault(l => l.Id == vm.Id);
            var details = _context.EnrolmentDetails.Where(i => i.EnrolmentId == vm.Id);
            if (vm.Option.Equals("import"))
            {
                vm.Validated = details.Count(i => i.StatusId == 16);
                vm.NotRegistered = details.Count(i => i.StatusId == 17);
                vm.NamesMismatch = details.Count(i => i.StatusId == 18);
            }

            return View(vm);
        }

        [HttpPost]
        public async Task<ActionResult> Send(EnrolmentApprovalsViewModel vm)
        {
            var list = _context.Enrolments
                .Include(l => l.CreatedBy)
                .Single(l => l.Id == vm.Id);
            if (!vm.Option.Equals("import"))
            {
                list.StatusId = 2;
            }
            else
            {
                list.ImportStatusId = 2;
            }
            await _context.SaveChangesAsync();
            TempData["Message"] = "Enrolment List sent for approval";
            return RedirectToAction("Details", new { vm.Id });
        }

        public IActionResult Action(EnrolmentApprovalsViewModel vm)
        {
            vm.Enrolment = _context.Enrolments
                .Include(l => l.CreatedBy)
                .SingleOrDefault(l => l.Id == vm.Id);
            var details = _context.EnrolmentDetails.Where(i => i.EnrolmentId == vm.Id);
            if (vm.Option.Equals("import"))
            {
                vm.Validated = details.Count(i => i.StatusId == 16);
                vm.NotRegistered = details.Count(i => i.StatusId == 17);
                vm.NamesMismatch = details.Count(i => i.StatusId == 18);
            }
            return View(vm);
        }

        [HttpPost]
        public async Task<ActionResult> ActionSave(EnrolmentApprovalsViewModel vm)
        {
            var list = _context.Enrolments
                .Include(l => l.CreatedBy)
                .Single(l => l.Id == vm.Id);
            if (!vm.Option.Equals("import"))
            {
                list.StatusId = vm.StatusId;
                list.ApprovalNotes = vm.Notes;
                list.DateApproved = DateTime.UtcNow.AddHours(3);
                list.ApprovedById = User.GetUserId();
                TempData["Message"] = "Approvals saved successfully.";
            }
            else
            {
                list.ImportStatusId = vm.StatusId;
                list.ImportApprovalNotes = vm.Notes;
                list.ImportDateApproved = DateTime.UtcNow.AddHours(3);
                list.ImportApprovedById = User.GetUserId();

                // Add Validated beneficiaries to the Beneficiary Table
                var rowsAffected = _context.Database.ExecuteSqlCommand("EnrolmentInsertValidatedHHs  @EnrolmentId",
                    new SqlParameter("EnrolmentId", vm.Id));
                // Notify Mothers
                var mothers = _context.EnrolmentDetails.Include(i => i.Household.Mother)
                    .Where(i => i.EnrolmentId == vm.Id && i.StatusId == 19);

                var smses = _context.SMS.OrderBy(i => i.Order).Where(i => i.TriggerEvent == "ENROLMENT-SUCCESSFUL").ToList();
                foreach (var mother in mothers)
                {
                    foreach (var sms in smses)
                    {
                        _smsService.Send(mother.Household.Phone,
                            sms.Message.Replace("##NAME##", mother.Household.CommonName ?? mother.Household.Mother.FirstName));
                    }
                }

                TempData["Message"] = "Approvals saved  and mothers enrolled successfully.";
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { vm.Id });
        }
        public async Task<IActionResult> AllowToEnroll(EnrolmentListDetailsViewModel vm)
        {
            if (vm.Ids == null || !vm.Ids.Any())
            {
                TempData["Error"] = "No record selected";
                return RedirectToAction("Details", new { vm.Id });
            }

            var records = _context.EnrolmentDetails.Where(i => vm.Ids.Contains(i.Id));
            if(vm.ActionId== 363) // Allow
                await records.ForEachAsync(i => i.StatusId = 16);// Validated
            else
                await records.ForEachAsync(i => i.StatusId = 18);// Names Mismatch
            _context.SaveChanges();
            TempData["Message"] = "Record Saved successfully";
            return RedirectToAction("Details",new{vm.Id});
        }
        private bool EnrolmentDetailExists(int id)
        {
            return _context.EnrolmentDetails.Any(e => e.Id == id);
        }
    }
}