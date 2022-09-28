using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using ClosedXML.Extensions;
using ExcelDataReader;
using MCHMIS.Data;
using MCHMIS.Extensions;
using MCHMIS.Interfaces;
using MCHMIS.Models;
using MCHMIS.Services;
using MCHMIS.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace MCHMIS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Permission("Pre-Payroll Checks:View Pre-Payroll Checks")]
    public class PrePayrollChecksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _uow;
        private readonly IHostingEnvironment _hostingEnvironment;

        public PrePayrollChecksController(ApplicationDbContext context, IUnitOfWork uow, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _uow = uow;
        }

        [Route("admin/pre-payroll-checks")]
        public IActionResult Index(PrePayrollListViewModel vm)
        {
            var data = _context.PrePayrollChecks.OrderByDescending(p => p.Id)
                .Include(p => p.PaymentCycle)
                .Include(p => p.Status)
                .Include(p => p.ApprovedBy)
                // .Include(p => p.PrePayrollChecksDetails)
                .Where(p => p.PaymentCycleId == vm.PaymentCycleId || vm.PaymentCycleId == null)
                .OrderByDescending(i=>i.Id)
                .ToList();

            ViewBag.PaymentCycleId = new SelectList(_context.PaymentCycles.OrderByDescending(c => c.Id).ToList(), "Id",
                "Name", vm.PaymentCycleId);
            vm.PrePayrollChecks = data;
            return View(vm);
        }

        public ActionResult Generate()
        {
            ViewBag.PaymentCycleId =
                new SelectList(_context.PaymentCycles.Where(c => c.Closed == false).OrderByDescending(c => c.Id).ToList(), "Id",
                    "Name");
            var standardAmount = _context.SystemSettings.Single(i => i.key == "PAYMENT.STANDARD.AMOUNT").Value;
            var vm = new GeneratePrePayrollViewModel
            {
                ExpectedAmount = Convert.ToDecimal(standardAmount)
            };
            return View(vm);
        }

        [HttpPost]
        public ActionResult Generate(GeneratePrePayrollViewModel vm)
        {
            var userId = User.GetUserId();
            int data =
                _context.Database.ExecuteSqlCommand(";Exec PrePayrollChecksGenerate @PaymentCycleId,@ExpectedAmount,@CreatedById",
                    new SqlParameter("PaymentCycleId", vm.PaymentCycleId),
                    new SqlParameter("ExpectedAmount", vm.ExpectedAmount),
                    new SqlParameter("createdById", userId)
                );

            if (data < 0)
            {
                var paymentName = _context.PaymentCycles.Find(vm.PaymentCycleId).Name;
                TempData["info"] = "Pre-Payroll checks for " + paymentName + " already generated.";
                return RedirectToAction("Index");
            }
            TempData["success"] = "Pre-payroll checks generated.";
            return RedirectToAction("Index");
        }

        public ActionResult DuplicatesPhones(PrePayrollFilterViewModel vm)
        {
            var details = _context.PrePayrollChecksDetails
                .OrderBy(e => e.Beneficiary.RecipientPhone)
                .Include(e => e.Action)
                .Include(e => e.ApprovedBy)
                .Include(e => e.Status)
                .Include(e => e.Beneficiary.HealthFacility)
                .Include(e => e.Beneficiary.Village.Ward.SubCounty.County)
                .Where(e => e.PrePayrollCheckId == vm.Id
                            && e.ExceptionId == 1
                ).AsNoTracking();
            if (!string.IsNullOrEmpty(vm.UniqueId))
            {
                details = details.Where(h => h.Beneficiary.Household.UniqueId == vm.UniqueId);
            }
            if (!string.IsNullOrEmpty(vm.IdNumber))
            {
                details = details.Where(h => h.Beneficiary.Household.Mother.IdNumber.Contains(vm.IdNumber));
            }
            if (vm.WardId != null)
            {
                details = details.Where(h => h.Beneficiary.Household.Village.WardId == vm.WardId);
            }
            if (vm.SubCountyId != null)
            {
                details = details.Where(h => h.Beneficiary.Household.Village.Ward.SubCountyId == vm.SubCountyId);
            }
            ViewBag.HealthFacilityId = new SelectList(_context.HealthFacilities.OrderBy(i => i.Name), "Id", "Name", vm.HealthFacilityId);

            ViewBag.PrePayrollCheckId = vm.Id;
            ViewBag.ActionId = new SelectList(_context.PrePayrollActions, "Id", "Name");
            if (vm.HealthFacilityId != null)
            {
                details = details.Where(e => e.Beneficiary.HealthFacilityId == vm.HealthFacilityId);
            }

            var page = vm.Page ?? 1;
            var pageSize = vm.PageSize ?? 20;
            vm.Details = details.ToPagedList(page, pageSize);
            vm.StatusId = _context.PrePayrollChecks.Single(i => i.Id == vm.Id).StatusId;
            ModelState.Clear();
            return View(vm);
        }

        public ActionResult DuplicatesIds(PrePayrollFilterViewModel vm)
        {
            var details = _context.PrePayrollChecksDetails
                .OrderBy(e => e.Beneficiary.IdNumber)
                .Include(e => e.Action)
                .Include(e => e.ApprovedBy)
                .Include(e => e.Status)
                .Include(e => e.Beneficiary.HealthFacility)
                .Include(e => e.Beneficiary.Village.Ward.SubCounty.County)
                .Where(e => e.PrePayrollCheckId == vm.Id
                            && e.ExceptionId == 2
                ).AsNoTracking();
            if (!string.IsNullOrEmpty(vm.UniqueId))
            {
                details = details.Where(h => h.Beneficiary.Household.UniqueId == vm.UniqueId);
            }
            if (!string.IsNullOrEmpty(vm.IdNumber))
            {
                details = details.Where(h => h.Beneficiary.Household.Mother.IdNumber.Contains(vm.IdNumber));
            }
            if (vm.WardId != null)
            {
                details = details.Where(h => h.Beneficiary.Household.Village.WardId == vm.WardId);
            }
            if (vm.SubCountyId != null)
            {
                details = details.Where(h => h.Beneficiary.Household.Village.Ward.SubCountyId == vm.SubCountyId);
            }
            ViewBag.HealthFacilityId = new SelectList(_context.HealthFacilities.OrderBy(i => i.Name), "Id", "Name", vm.HealthFacilityId);

            ViewBag.PrePayrollCheckId = vm.Id;
            ViewBag.ActionId = new SelectList(_context.PrePayrollActions, "Id", "Name");
            if (vm.HealthFacilityId != null)
            {
                details = details.Where(e => e.Beneficiary.HealthFacilityId == vm.HealthFacilityId);
            }

            var page = vm.Page ?? 1;
            var pageSize = vm.PageSize ?? 20;
            vm.Details = details.ToPagedList(page, pageSize);
            vm.StatusId = _context.PrePayrollChecks.Single(i => i.Id == vm.Id).StatusId;
            ModelState.Clear();
            return View(vm);
        }

        public async Task<ActionResult> UnusualAmounts(PrePayrollFilterViewModel vm)
        {
            var details = _context.PrePayrollChecksDetails
                .OrderBy(e => e.Beneficiary.RecipientPhone)
                .Include(e => e.Action)
                .Include(e => e.ApprovedBy)
                .Include(e => e.Status)
                .Include(e => e.Beneficiary.HealthFacility)
                .Include(e => e.Beneficiary.Village.Ward.SubCounty.County)
                .Where(e => e.PrePayrollCheckId == vm.Id
                            && e.ExceptionId == 3
                ).AsNoTracking();
            if (!string.IsNullOrEmpty(vm.UniqueId))
            {
                details = details.Where(h => h.Beneficiary.Household.UniqueId == vm.UniqueId);
            }
            if (!string.IsNullOrEmpty(vm.PhoneNumber))
            {
                details = details.Where(h => h.Beneficiary.RecipientPhone.Contains(vm.PhoneNumber));
            }
            if (!string.IsNullOrEmpty(vm.IdNumber))
            {
                details = details.Where(h => h.Beneficiary.Household.Mother.IdNumber.Contains(vm.IdNumber));
            }
            if (vm.WardId != null)
            {
                details = details.Where(h => h.Beneficiary.Household.Village.WardId == vm.WardId);
            }
            if (vm.SubCountyId != null)
            {
                details = details.Where(h => h.Beneficiary.Household.Village.Ward.SubCountyId == vm.SubCountyId);
            }

            ViewBag.HealthFacilityId = new SelectList(_context.HealthFacilities.OrderBy(i => i.Name), "Id", "Name", vm.HealthFacilityId);

            ViewBag.PrePayrollCheckId = vm.Id;
            ViewBag.ActionId = new SelectList(_context.PrePayrollActions, "Id", "Name");
            if (vm.HealthFacilityId != null)
            {
                details = details.Where(e => e.Beneficiary.HealthFacilityId == vm.HealthFacilityId);
            }

            if (vm.Page == null)
            {
                var page = HttpContext.Session.GetInt32("UnusualAmountsPage");
                var pageSize = HttpContext.Session.GetInt32("UnusualAmountsPageSize");
                if (page != null)
                    vm.Page = page;
                if (pageSize != null)
                    vm.PageSize = pageSize;
                vm.Page = vm.Page ?? 1;
                vm.PageSize = vm.PageSize ?? 20;
            }
            else
            {
                HttpContext.Session.SetInt32("UnusualAmountsPage", (int)vm.Page);
                HttpContext.Session.SetInt32("UnusualAmountsPageSize", (int)vm.PageSize);
            }

            vm.Details = details.ToPagedList((int)vm.Page, (int)vm.PageSize);
            vm.StatusId = _context.PrePayrollChecks.Single(i => i.Id == vm.Id).StatusId;
            ViewBag.PageSize = this.GetPager(vm.PageSize);
            ModelState.Clear();
            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name", vm.SubCountyId);
            vm.Wards = await _context.Wards.ToListAsync();
            vm.PrePayrollTransactions = _context.PrePayrollTransactions
                .Include(i => i.PaymentPoint)
                .Where(i => i.PrePayrollCheckId == vm.Id).ToList();
            return View(vm);
        }

        public async Task<ActionResult> RecipientChange(PrePayrollFilterViewModel vm)
        {
            var details = _context.PrePayrollChecksDetails
                .OrderBy(e => e.Beneficiary.RecipientPhone)
                .Include(e => e.Action)
                .Include(e => e.ApprovedBy)
                .Include(e => e.Status)
                .Include(e => e.Beneficiary.HealthFacility)
                .Include(e => e.Beneficiary.Village.Ward.SubCounty.County)
                .Where(e => e.PrePayrollCheckId == vm.Id
                            && e.ExceptionId == 4
                ).AsNoTracking();
            if (!string.IsNullOrEmpty(vm.UniqueId))
            {
                details = details.Where(h => h.Beneficiary.Household.UniqueId == vm.UniqueId);
            }
            if (!string.IsNullOrEmpty(vm.IdNumber))
            {
                details = details.Where(h => h.Beneficiary.Household.Mother.IdNumber.Contains(vm.IdNumber));
            }
            if (vm.WardId != null)
            {
                details = details.Where(h => h.Beneficiary.Household.Village.WardId == vm.WardId);
            }
            if (vm.SubCountyId != null)
            {
                details = details.Where(h => h.Beneficiary.Household.Village.Ward.SubCountyId == vm.SubCountyId);
            }

            ViewBag.HealthFacilityId = new SelectList(_context.HealthFacilities.OrderBy(i => i.Name), "Id", "Name", vm.HealthFacilityId);

            ViewBag.PrePayrollCheckId = vm.Id;
            ViewBag.ActionId = new SelectList(_context.PrePayrollActions, "Id", "Name");
            if (vm.HealthFacilityId != null)
            {
                details = details.Where(e => e.Beneficiary.HealthFacilityId == vm.HealthFacilityId);
            }

            var page = vm.Page ?? 1;
            var pageSize = vm.PageSize ?? 20;
            vm.Details = details.ToPagedList(page, pageSize);
            vm.StatusId = _context.PrePayrollChecks.Single(i => i.Id == vm.Id).StatusId;
            ModelState.Clear();
            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name", vm.SubCountyId);
            vm.Wards = await _context.Wards.ToListAsync();
            vm.PrePayrollTransactions = _context.PrePayrollTransactions
                .Include(i => i.PaymentPoint)
                .Where(i => i.PrePayrollCheckId == vm.Id);
            return View(vm);
        }

        public async Task<ActionResult> PhoneNumberValidation(PrePayrollFilterViewModel vm)
        {
            var details = _context.PrePayrollChecksDetails
                .OrderBy(e => e.Beneficiary.RecipientPhone)
                .Include(e => e.Action)
                .Include(e => e.Exception)
                .Include(e => e.ApprovedBy)
                .Include(e => e.Status)
                .Include(e => e.Beneficiary.HealthFacility)
                .Include(e => e.Beneficiary.Village.Ward.SubCounty.County)
                .Where(e => e.PrePayrollCheckId == vm.Id
                            && (e.ExceptionId == 5 || e.ExceptionId == 6)).AsNoTracking();
            if (!string.IsNullOrEmpty(vm.UniqueId))
            {
                details = details.Where(h => h.Beneficiary.Household.UniqueId == vm.UniqueId);
            }
            if (!string.IsNullOrEmpty(vm.IdNumber))
            {
                details = details.Where(h => h.Beneficiary.Household.Mother.IdNumber.Contains(vm.IdNumber));
            }
            if (vm.WardId != null)
            {
                details = details.Where(h => h.Beneficiary.Household.Village.WardId == vm.WardId);
            }
            if (!string.IsNullOrEmpty(vm.PhoneNumber))
            {
                details = details.Where(h => h.Beneficiary.RecipientPhone.Contains(vm.PhoneNumber));
            }
            if (vm.SubCountyId != null)
            {
                details = details.Where(h => h.Beneficiary.Household.Village.Ward.SubCountyId == vm.SubCountyId);
            }

            ViewBag.HealthFacilityId = new SelectList(_context.HealthFacilities.OrderBy(i => i.Name), "Id", "Name", vm.HealthFacilityId);

            ViewBag.PrePayrollCheckId = vm.Id;
            ViewBag.ActionId = new SelectList(_context.PrePayrollActions, "Id", "Name");
            if (vm.HealthFacilityId != null)
            {
                details = details.Where(e => e.Beneficiary.HealthFacilityId == vm.HealthFacilityId);
            }


            if (vm.Page == null)
            {
                var page = HttpContext.Session.GetInt32("Page");
                var pageSize = HttpContext.Session.GetInt32("PageSize");
                if (page != null)
                    vm.Page = page;
                if (pageSize != null)
                    vm.PageSize = pageSize;
                vm.Page = vm.Page ?? 1;
                vm.PageSize = vm.PageSize ?? 20;
            }
            else
            {
                HttpContext.Session.SetInt32("Page", (int)vm.Page);
                HttpContext.Session.SetInt32("PageSize", (int)vm.PageSize);
            }
            
            vm.Details = details.ToPagedList((int)vm.Page, (int)vm.PageSize);
            var prePayroll = _context.PrePayrollChecks.Single(i => i.Id == vm.Id);
            vm.StatusId = prePayroll.StatusId;
            vm.Imported = prePayroll.Imported;
            ModelState.Clear();
            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name", vm.SubCountyId);
            vm.Wards = await _context.Wards.ToListAsync();
            ViewBag.PageSize = this.GetPager(vm.PageSize);
            vm.PrePayrollTransactions = _context.PrePayrollTransactions
                .Include(i => i.PaymentPoint)
                .Where(i => i.PrePayrollCheckId == vm.Id);
          
            return View(vm);
        }

        public async Task<IActionResult> Export(int id)
        {
            var pendingPaymentsHHIds = _context.BeneficiaryPaymentPoints.Where(i => i.StatusId == 1)
                .Select(i => i.HouseholdId);
            var details = _context.Beneficiaries
                .Where(i => (i.StatusId == 19 || i.StatusId== 27) && 
            pendingPaymentsHHIds.Contains(i.HouseholdId));



            var data = details.Select(b => new EnrolmentListExportViewModel
            {
                CreditIdentityStringType = "MSISDN",
                CreditIdentityString = "254" + b.RecipientPhone.Substring(b.RecipientPhone.Length - 9, 9),
                Comment = "Payment Validation"
            }).ToList();

            // exportService.ExportToExcel(data, reportTitle);
            var wb = new XLWorkbook();
            // Add all DataTables in the DataSet as a worksheets
            var ds = new DataSet();
            var ws = wb.Worksheets.Add("Enrolment Details");
            var reportTitle = "Pre-Payroll-Checks-Phone-Number-Validation Batch#" + id + ".xlsx";
            ws.Cell(1, 1).InsertTable(data.AsEnumerable());
            ws.Columns().AdjustToContents();
            var xlTable = ws.Tables.FirstOrDefault();
            if (xlTable != null) xlTable.ShowAutoFilter = false;
            return wb.Deliver(reportTitle);
        }

        public IActionResult ExportMismatch(int id)
        {
            var details = _context.PrePayrollChecksDetails
                .Include(i => i.Exception)
                .Include(i => i.Beneficiary.Village.Ward.SubCounty)
                .Include(i => i.Beneficiary.HealthFacility)
                .Include(i => i.Beneficiary.CommunityArea)
                .Where(i => i.PrePayrollCheckId == id && (i.ExceptionId == 5 || i.ExceptionId == 6));

            var data = details.Select(b => new EnrolmentListMismatchExportViewModel
            {
                MotherUniqueId = b.Beneficiary.UniqueId,
                RecipientPhone = b.Beneficiary.RecipientPhone,
                RecipientNames = b.Beneficiary.RecipientName,
                CustomerName = b.CustomerName,
                CustomerType = b.CustomerType,
                HealthFacility = b.Beneficiary.HealthFacility.Name,
                SubCounty = b.Beneficiary.Village.Ward.SubCounty.Name,
                Ward = b.Beneficiary.Village.Ward.Name,
                VillageUnit = b.Beneficiary.Village.Name,
                CommunityArea = b.Beneficiary.CommunityArea.Name,
                Issue = b.Exception.Name
            }).ToList();
            // exportService.ExportToExcel(data, reportTitle);
            var wb = new XLWorkbook();

            // Add all DataTables in the DataSet as a worksheets
            var ds = new DataSet();
            var ws = wb.Worksheets.Add("Mismatched Beneficiaries");
            var reportTitle = "Mismatched Beneficiaries for Pre-Payroll checks Batch#" + id + ".xlsx";
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
                var firstPhoneNumber = tableData.Rows[12][3].ToString().ToLower();
                if (!recordNo.Equals("record no") || !amount.Equals("amount"))
                {
                    TempData["Info"] = "Please upload the Excel document as downloaded from Safaricom.";
                    return View(model);
                }

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
            //for (int i = table.Rows.Count - 1; i >= 0; i--)
            //{
            //    DataRow dr = table.Rows[i];
            //    if (string.IsNullOrEmpty(dr["Column3"].ToString()))
            //        dr.Delete();
            //}
            //table.AcceptChanges();
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
            var rowsAffected = _context.Database.ExecuteSqlCommand("exec PrepayrollProcessPhoneValidation  @PrePayrollCheckId",
                new SqlParameter("PrePayrollCheckId", id)
            );

            //// Attempt to resolve recipient names in different order
            //var mothers = _context.EnrolmentDetails.Where(i => i.EnrolmentId == id && i.StatusId == 18).ToList();
            //if (mothers.Any())
            //{
            //    foreach (var mother in mothers)
            //    {
            //        var original = mother.RecipientNames.Split(' ');

            //        var count = 0;
            //        foreach (var item in original)
            //        {
            //            if (mother.CustomerName.ToLower().Contains(item.ToLower()))
            //            {
            //                count++;
            //            }
            //        }

            //        if (count > 1)
            //        {
            //            mother.StatusId = 16;
            //            _context.Update(mother);
            //        }
            //    }
            //    _context.SaveChanges();
            //}
            //enrolment.HouseholdsValidated = _context.EnrolmentDetails.Count(i => i.StatusId == 16 && i.EnrolmentId == id);
            //_context.Update(enrolment);
            //_context.SaveChanges();
            TempData["Message"] = "Import processed successfully.";
            return RedirectToAction("PhoneNumberValidation", new { Id = id });
        }

        [Route("prepayrollchecks/action/{id}/{page?}")]
        public ActionResult Action(int id,int? page)
        {
            var vm = new PrePayrollActionViewModel();
            vm.ExceptionDetail = _context.PrePayrollChecksDetails
                .Include(e => e.Exception)
                .Include(e => e.Payment)
                .Include(e => e.Beneficiary.HealthFacility)
                .Include(e => e.Beneficiary.Status)
                .Single(e => e.Id == id);
            vm.Beneficiary = _context.Beneficiaries
                .Include(b => b.Village.Ward.SubCounty.County)
                .Single(b => b.Id == vm.ExceptionDetail.BeneficiaryId);
            ViewBag.PayrollActionId = new SelectList(_context.PrePayrollActions, "Id", "Name");
            vm.Id = id;
            vm.Page = page;
            if (vm.ExceptionDetail.Exception.Id == 6)
                vm.Notes = "Misspelling";
            return View(vm);
        }

        [HttpPost]
        public ActionResult ActionSave(PrePayrollActionViewModel vm)
        {
            var check = _context.PrePayrollChecksDetails
                .Include(c => c.Exception)
                .Single(c => c.Id == vm.Id);
            check.ActionId = vm.PayrollActionId;
            check.DateApproved = DateTime.UtcNow.AddHours(3);
            check.ApprovedById = User.GetUserId();
            check.Notes = vm.Notes;
            check.StatusId = 1;
            _uow.GetRepository<PrePayrollChecksDetail>().Update(check);
            _uow.Save();

            var returnData = "";
            vm.Id = check.PrePayrollCheckId;
            if (check.ExceptionId == 1)
            {
                returnData = "DuplicatesPhones";
            }
            else if (check.ExceptionId == 2)
            {
                returnData = "DuplicatesIds";
            }
            else if (check.ExceptionId == 6)
            {
                returnData = "phonenumbervalidation";
            }
            else
            {
                returnData = "UnusualAmounts";
            }

            if (vm.FacilityId != null)
            {
                return RedirectToAction(returnData, new { vm.Id, vm.FacilityId });
            }
            TempData["Message"] = "Saved successfully";
            return RedirectToAction(returnData, new { id = vm.Id});
        }

        [HttpPost]
        public ActionResult BatchAction(PrePayrollBatchActionViewModel vm)
        {
            var ids = string.Join(",", vm.Ids);
            var rowsAffected = _context.Database.ExecuteSqlCommand(";Exec PrePayrollChecksBatchApprove @Ids,@ActionId,@Notes,@UserId,@ApproveAll,@ExceptionId,@PrePayrollCheckId",
                new SqlParameter("Ids", ids),
                new SqlParameter("ActionId", vm.ActionId.ToString()),
                new SqlParameter("Notes", vm.Notes),
                new SqlParameter("UserId", User.GetUserId()),
                new SqlParameter("ApproveAll", vm.ApproveAll),
                new SqlParameter("ExceptionId", vm.ExceptionId),
                new SqlParameter("PrePayrollCheckId", vm.Id)
            );
            if (rowsAffected > 0)
            {
                TempData["success"] = "Records saved successfully";
            }
            else
            {
                TempData["success"] = "Error saving the records ";
            }

            var returnData = "";

            if (vm.ExceptionId == 1)
            {
                returnData = "DuplicatesPhones";
            }
            else if (vm.ExceptionId == 2)
            {
                returnData = "DuplicatesIds";
            }
            else
            {
                returnData = "UnusualAmounts";
            }

            if (vm.HealthFacilityId != null)
            {
                return RedirectToAction(returnData, new { vm.Id, vm.HealthFacilityId });
            }
            return RedirectToAction(returnData, new { id = vm.Id });
        }

        public ActionResult Summary(int id)
        {
            var details = _context.PrePayrollChecksDetails.Include(e => e.Action)
                .Where(e => e.PrePayrollCheckId == id);

            ViewBag.PrePayrollCheckId = id;
            ViewBag.ActionId = new SelectList(_context.PrePayrollActions, "Id", "Name");
            var vm = new PrePayrollSummaryViewModel
            {
                PrePayrollCheck = _context.PrePayrollChecks.Include(e => e.PaymentCycle).Single(e => e.Id == id),
                PrePayrollChecksDetails = details,
                Exceptions = _context.PayrollExceptions.OrderBy(i => i.Order).Where(e => e.ExceptionTypeId == 1).ToList()
            };
            vm.Id = id;
            return View(vm);
        }

        public ActionResult SendForApproval(int id)
        {
            var model = new PrePayrollApprovalActionViewModel();
            model.PrePayrollCheck = _context.PrePayrollChecks
                .Include(t => t.CreatedBy)
                .Include(t => t.PaymentCycle).Single(t => t.Id == id);
            model.Id = id;

            return View(model);
        }

        public async Task<ActionResult> Send(int id)
        {
            var obj = _context.PrePayrollChecks.Find(id);
            obj.StatusId = 2;
            _uow.GetRepository<PrePayrollCheck>().Update(obj);
            _uow.Save();

            //// Email Notifications
            //var em = new EmailService();
            //// Get Role with Approve Import Registration Data
            //var userId = User.GetUserId();
            //var roles = _context.RoleProfiles.Where(r => r.TaskId == 47)//Approves Pre-Payroll Checks
            //    .Select(r => r.RoleId).ToList();

            //var checks = _context.PrePayrollCheck
            //    .Include(t => t.CreatedBy)
            //    .Include(t => t.PaymentCycle)
            //    .Single(t => t.Id == id);
            //var user = _context.Users.Find(userId);
            //foreach (var role in roles)
            //{
            //    var recipients = _uow.GetRepository<ApplicationUser>()
            //        .GetAll(u => u.Roles.Select(r => r.RoleId).Contains(role)).ToList();
            //    foreach (var recipient in recipients)
            //    {
            //        var tEmail = new Thread(() =>
            //            em.SendAsync(new ApprovalEmail
            //            {
            //                To = recipient.Email,
            //                Subject = "Pre-Payroll Checks Awaiting Approval ",
            //                Name = recipient.FirstName,
            //                Item = "Pre-Payroll Checks",
            //                Action = "for approval",
            //                Narration = "has sent",
            //                StatusId = 2,
            //                PrePayrollCheck = checks,
            //                User = user,
            //            })
            //        );
            //        tEmail.Start();
            //    }
            //}

            TempData["success"] = "Pre-Payroll checks sent for approval.";
            return RedirectToAction("Index");
        }

        public ActionResult Approvals(int id,int? page)
        {
            var model = new PrePayrollApprovalActionViewModel();
            model.PrePayrollCheck = _context.PrePayrollChecks
                .Include(t => t.CreatedBy)
                .Include(t => t.PaymentCycle).Single(t => t.Id == id);
            model.Id = id;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Approvals(PrePayrollApprovalActionViewModel model)
        {
            var checks = _context.PrePayrollChecks.Find(model.Id);
            checks.StatusId = 3;
            checks.DateApproved = DateTime.UtcNow.AddHours(3);
            checks.ApprovedById = User.GetUserId();
            checks.Notes = model.Notes;
            _uow.GetRepository<PrePayrollCheck>().Update(checks);
            _uow.Save();

            TempData["success"] = "Pre-Payroll checks approved successfully.";

            // Notify User
            //checks = _context.PrePayrollCheck
            //    .Include(t => t.CreatedBy)
            //    .Include(t => t.PaymentCycle)
            //    .Single(t => t.Id == model.Id);
            //var em = new EmailService();
            //var recipient = _context.Users.Find(checks.CreatedById);
            //var userId = User.GetUserId();
            //var user = _context.Users.Find(userId);
            //var tEmail = new Thread(() =>
            //    em.SendAsync(new ApprovalEmail
            //    {
            //        To = recipient.Email,
            //        Subject = "Pre-Payroll Checks Approval",
            //        Name = recipient.FirstName,
            //        Narration = "has",
            //        Item = "Pre-Payroll Checks",
            //        Action = action,
            //        PrePayrollCheck = checks,
            //        User = user,
            //        StatusId = 3
            //    })
            //);
            //tEmail.Start();

            return RedirectToAction("Index");
        }

        public void ExportPDF(PrePayrollFilterViewModel vm)
        {
        }
    }
}