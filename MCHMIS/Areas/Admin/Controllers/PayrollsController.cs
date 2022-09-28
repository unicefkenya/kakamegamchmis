using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using ClosedXML.Extensions;
using ExcelDataReader;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MCHMIS.Data;
using MCHMIS.Extensions;
using MCHMIS.Interfaces;
using MCHMIS.Models;
using MCHMIS.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using X.PagedList;
using RestSharp;
using RestSharp.Authenticators;
using MCHMIS.Services;
using Microsoft.AspNetCore.Authorization;

namespace MCHMIS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    [Permission("Payroll:View Payroll")]
    public class PayrollsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IUnitOfWork _uow;

        public PayrollsController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment, IUnitOfWork uow)
        {
            _context = context;
            _uow = uow;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Admin/FundRequests
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Payrolls.OrderByDescending(i => i.Id)
                .Include(f => f.ApprovedBy).Include(f => f.CreatedBy).Include(f => f.Cycle).Include(f => f.Status)
                .OrderByDescending(i => i.Id);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/FundRequests/Details/5
        public async Task<IActionResult> Details(PaymentsViewModel vm)
        {
            IQueryable<Payment> details = _context.Payments.AsNoTracking().OrderBy(i => i.BeneficiaryName)
                .Include(i => i.HealthFacility)
                .Include(i => i.Beneficiary)
                .Include(r => r.Village.Ward.SubCounty)
                .Include(r => r.Status)
                .Where(i => i.PayrollId == vm.Id).AsQueryable();

            if (!string.IsNullOrEmpty(vm.UniqueId))
            {
                details = details.Where(h => h.Beneficiary.UniqueId == vm.UniqueId);
            }
            if (!string.IsNullOrEmpty(vm.PhoneNumber))
            {
                details = details.Where(h => h.Beneficiary.RecipientPhone == vm.PhoneNumber);
            }
            if (!string.IsNullOrEmpty(vm.IdNumber))
            {
                details = details.Where(h => h.Beneficiary.IdNumber.Contains(vm.IdNumber));
            }
            if (vm.StatusId != null)
            {
                details = details.Where(h => h.StatusId == vm.StatusId);
            }
            if (!string.IsNullOrEmpty(vm.Name))
            {
                details = details.Where(h =>
                    h.Beneficiary.BeneficiaryName.Contains(vm.Name)
                );
            }
            if (vm.WardId != null)
            {
                details = details.Where(h => h.Beneficiary.Village.WardId == vm.WardId);
            }
            if (vm.SubCountyId != null)
            {
                details = details.Where(h => h.Village.Ward.SubCountyId == vm.SubCountyId);
            }
            if (vm.HealthFacilityId != null)
            {
                details = details.Where(h => h.HealthFacilityId == vm.HealthFacilityId);
            }

            if (vm.Id != 3)
            {
                vm.Total = details.Sum(i => i.Amount);
            }

            var page = vm.Page ?? 1;
            var pageSize = vm.PageSize ?? 20;

            vm.Details = new PagedList<Payment>(details, page, pageSize); //details.ToPagedList(page, pageSize);
            vm.Payroll = _context.Payrolls.Find(vm.Id);
            vm.Wards = _context.Wards.ToList();
            ViewBag.HealthFacilityId = new SelectList(_context.HealthFacilities.OrderBy(i => i.Name), "Id", "Name", vm.HealthFacilityId);
            ViewBag.SubCountyId = new SelectList(_context.SubCounties.OrderBy(i => i.Name), "Id", "Name", vm.SubCountyId);
            ViewBag.StatusId = new SelectList(_context.PaymentStatus.Where(i => i.Id < 5).OrderBy(i => i.Name), "Id", "Name", vm.StatusId);

            return View(vm);
        }

        // GET: Admin/FundRequests/Create
        public IActionResult Create()
        {
            var takenIds = _context.Payrolls.Select(i => i.FundRequestId);
            ViewBag.FundRequestId =
       new SelectList(_context.FundRequests.Include(i => i.Cycle)
       .Where(c => c.StatusId == 3 && !takenIds.Contains(c.Id))
       .OrderByDescending(c => c.Id).ToList(), "Id",
           "Name");

            return View();
        }

        // POST: Admin/FundRequests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int fundRequestId)
        {
            var userId = User.GetUserId();
            SqlParameter[] @params =
            {
                    new SqlParameter("fundRequestId", fundRequestId),
                    new SqlParameter("createdById", userId),
                    new SqlParameter("OutPayrollId", SqlDbType.Int) {Direction = ParameterDirection.Output},
                };
            int data =
                _context.Database.ExecuteSqlCommand(";Exec PayrollGenerate @fundRequestId,@createdById,@OutPayrollId OUT",
                    @params
                );
            // _context.SaveChanges();
            var payrollId = @params[2].Value;
            if (payrollId.ToString().Equals("0"))
            {
                TempData["Info"] = "Payroll for the cycle already generated.";
                return RedirectToAction("Index");
            }
            TempData["success"] = "Payroll generated.";
            return RedirectToAction("Summary", new { id = payrollId });
        }

        public IActionResult Summary(int id)
        {
            SqlParameter[] parms = new SqlParameter[]
            {
                new SqlParameter("PayrollId", id ),
                new SqlParameter("PaymentPointId",  (object)DBNull.Value),
                new SqlParameter("HealthFacilityId",  (object)DBNull.Value),
                new SqlParameter("SubCountyId", (object)DBNull.Value),
                new SqlParameter("WardId", (object)DBNull.Value),
            };
            var result =
                SQLExtensions.GetModelFromQuery<FRSummaryViewModel>(_context,
                    "EXEC [PayrollSummary] @PayrollId,@PaymentPointId,@HealthFacilityId,@SubCountyId,@WardId", parms);

            ViewBag.Id = id;
            return View(result);
        }

        public IActionResult SendForApproval(FRApprovalSummaryViewModel vm)
        {
            var query = _context.Payments.Where(i => i.PayrollId == vm.Id)
                .GroupBy(p => p.HealthFacilityId)
                .Select(g => new FRApprovalSummaryViewModel
                {
                    Beneficiaries = g.Select(i => i.BeneficiaryId).Distinct().Count(),
                    Amount = g.Sum(i => i.Amount)
                }).ToList();

            vm = new FRApprovalSummaryViewModel
            {
                Id = vm.Id,
                Beneficiaries = query.Sum(i => i.Beneficiaries),
                Amount = query.Sum(i => i.Amount),
                Facilities = query.Count(),
                Option = vm.Option
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<ActionResult> Send(FRApprovalSummaryViewModel vm)
        {
            var payroll = _context.Payrolls
                .Include(l => l.CreatedBy)
                .SingleOrDefault(l => l.Id == vm.Id);
            if (vm.Option.Equals("reconciliation"))
            {
                payroll.ReconciliationStatusId = 2;
                TempData["Message"] = "Payroll reconciliation sent for approval";
            }
            else
            {
                payroll.StatusId = 2;
                TempData["Message"] = "Payroll sent for approval";
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public IActionResult Action(FRApprovalSummaryViewModel vm)
        {
            var query = _context.Payments.Where(i => i.PayrollId == vm.Id)
                .GroupBy(p => p.HealthFacilityId)
                .Select(g => new FRApprovalSummaryViewModel
                {
                    Beneficiaries = g.Select(i => i.BeneficiaryId).Distinct().Count(),
                    Amount = g.Sum(i => i.Amount)
                }).ToList();

            vm = new FRApprovalSummaryViewModel
            {
                Id = vm.Id,
                Option = vm.Option,
                Beneficiaries = query.Sum(i => i.Beneficiaries),
                Amount = query.Sum(i => i.Amount),
                Facilities = query.Count,
                Payroll = _context.Payrolls
                   .Include(l => l.CreatedBy)
                   .SingleOrDefault(l => l.Id == vm.Id)
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<ActionResult> ActionSave(FRApprovalSummaryViewModel vm)
        {
            var payroll = _context.Payrolls
                .SingleOrDefault(l => l.Id == vm.Id);
            if (vm.Option.Equals("reconciliation"))
            {
                payroll.ReconciliationStatusId = 3;
                payroll.ReconciliationNotes = vm.Notes;
                payroll.ReconciliationDateApproved = DateTime.UtcNow.AddHours(3);
                payroll.ReconciliationApprovedById = User.GetUserId();
                TempData["Message"] = "Payroll Reconciliation approved.";
                // Close and create a new Payment Cycle
                var paymentCycle = _context.PaymentCycles.Find(payroll.CycleId);
                paymentCycle.Closed = true;
                var monthName = DateTime.UtcNow.AddHours(3).ToString("MMM yyyy");
                // Open a new One
                PaymentCycle newCycle;
                if (paymentCycle.StartDate.Day > 1)
                {
                    var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    var mid = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 15);
                    newCycle = new PaymentCycle
                    {
                        CreatedById = User.GetUserId(),
                        DateCreated = DateTime.UtcNow.AddHours(3),
                        StartDate = startDate,
                        EndDate = mid,
                        Name = monthName + " - First Half"
                    };
                }
                else
                {
                    var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    var mid = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 16);
                    var endDate = startDate.AddMonths(1).AddDays(-1);
                    newCycle = new PaymentCycle
                    {
                        CreatedById = User.GetUserId(),
                        DateCreated = DateTime.UtcNow.AddHours(3),
                        StartDate = mid,
                        EndDate = endDate,
                        Name = monthName + " - Second Half"
                    };
                }
                _context.Add(newCycle);
                await _context.SaveChangesAsync();
            }
            else
            {
                var jobId = BackgroundJob.Enqueue(
                  () => MakeMpesaPayment(vm.Id));


                payroll.StatusId = 3;
                payroll.DateApproved = DateTime.UtcNow.AddHours(3);
                payroll.ApprovedById = User.GetUserId();
                await _context.SaveChangesAsync();
                TempData["Message"] = "Payroll approved and payments dispatched. Wait for some few minutes and check the payment status.";
                return RedirectToAction("Details", new { vm.Id });

                /*
                                payroll.Notes = vm.Notes;
                                await MakeMpesaPayment(payroll.Id);
                                await _context.SaveChangesAsync();
                                var pending = _context.Payments.Count(i => i.PayrollId == vm.Id && i.StatusId == 1);
                                TempData["Pending"] = pending;
                                return RedirectToAction("Processing", new { vm.Id });
                */
            }

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Export(int id)
        {
            var details = _context.Payments.Where(i => i.PayrollId == id);
            var payroll = _context.Payrolls.Include(i => i.Cycle).Single(i => i.Id == id);
            var data = details.Select(i => new
            {
                UniqueId = i.Beneficiary.UniqueId,
                IdNumber = i.IdNumber,
                i.BeneficiaryName
                ,
                i.RecipientName
                ,
                i.RecipientPhone
                ,
                HealthFacility = i.HealthFacility.Name
                ,
                SubCounty = i.Village.Ward.SubCounty.Name
                ,
                Ward = i.Village.Ward.Name,
                Village = i.Village.Ward.Name,
                Reconciled = i.Reconciled ? "Yes" : "No",
                PaymentStatus = i.Status.Name,
                MpesaCode = i.TransactionReceipt,
                i.Amount

            }).ToList();
            // exportService.ExportToExcel(data, reportTitle);

            var wb = new XLWorkbook();

            // Add all DataTables in the DataSet as a worksheets
            var ds = new DataSet();
            var ws = wb.Worksheets.Add("Enrolment Details");
            var reportTitle = payroll.Cycle.Name + " Payroll Batch#" + id + ".xlsx";
            ws.Cell(1, 1).InsertTable(data.AsEnumerable());
            ws.Columns().AdjustToContents();
            var xlTable = ws.Tables.FirstOrDefault();
            if (xlTable != null) xlTable.ShowAutoFilter = false;
            return wb.Deliver(reportTitle);
        }

        public IActionResult Reconcile(int id)
        {
            var vm = new EnrolmentListImportViewModel();
            vm.Id = id;
            return View(vm);
        }



        private string initiatorName,
            shortcode,
            baseUrl,
            requestUrl,
            callbackUrl,
            securityCredential,
            commandID,
            timeStamp,
            passKey, password;
        string accessCode = "";



        public async Task<int> MakeMpesaPayment(int id)
        {
            var consumerKey = _context.SystemSettings.Single(s => s.key == "CONSUMER.KEY").Value;
            var consumerSecret = _context.SystemSettings.Single(s => s.key == "CONSUMER.SECRET").Value;
            initiatorName = _context.SystemSettings.Single(s => s.key == "PAYMENT.INITIATOR.NAME").Value;
            shortcode = _context.SystemSettings.Single(s => s.key == "PAYMENT.SHORTCODE").Value;

            baseUrl = _context.SystemSettings.Single(s => s.key == "PAYMENT.BASE.URL").Value;
            requestUrl = _context.SystemSettings.Single(s => s.key == "PAYMENT.REQUEST.URL").Value;
            callbackUrl = _context.SystemSettings.Single(s => s.key == "PAYMENT.CALLBACK.URL").Value;
            securityCredential = _context.SystemSettings.Single(s => s.key == "SecurityCredential").Value;
            commandID = _context.SystemSettings.Single(s => s.key == "CommandID").Value;
            timeStamp = DateTime.UtcNow.AddHours(3).ToString("yyyyMMddHHmmss");
            passKey = _context.SystemSettings.Single(s => s.key == "PAYMENT.PASSKEY").Value; ;

            string webRootPath = _hostingEnvironment.WebRootPath;
            string path = "";

            path = webRootPath + "/cert/public.cer";

            var cert = new X509Certificate2(path);
            byte[] bytes = Encoding.ASCII.GetBytes(passKey);

            var enrcypted = EncryptDataOaepSha1(cert, bytes);

            password = Convert.ToBase64String(enrcypted);
            var token = _context.MpesaAuthorizationCode.FirstOrDefault();

            // token = null; //Remove before going live, Payments are done once in a while so we can assume the previous code will always be expired
            if (token != null && token.UtcDateTime.AddHours(1).AddMinutes(-15) > DateTime.UtcNow)
            {
                var expiryTime = token.UtcDateTime.AddHours(1).AddMinutes(-15);
                var now = DateTime.UtcNow.AddHours(3);

                accessCode = token.Value;
            }
            else
            {
                var responseToken = await GenerateSafaricomTokenAsync(consumerKey, consumerSecret, "json");
                TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseToken);
                accessCode = tokenResponse.AccessCode;
                if (token == null)
                {
                    token = new MpesaAuthorizationCode();
                }

                token.Value = tokenResponse.AccessCode;
                token.UtcDateTime = DateTime.UtcNow.AddMinutes(-5);
                _context.MpesaAuthorizationCode.Update(token);
            }
            var payments = _context.Payments
                .Include(i => i.Payroll.Cycle)
                .Where(i => i.PayrollId == id && i.StatusId == 1
                ).Take(10).ToList();
            int success = 0;
            while (payments.Any())
            {
                foreach (var payment in payments)
                {
                    int amount = int.Parse(Math.Ceiling(payment.Amount).ToString());
                    var model = new MPESAViewModel
                    {
                        InitiatorName = initiatorName,
                        SecurityCredential = password,
                        CommandID = commandID,
                        Amount = amount.ToString(),
                        PartyA = shortcode,
                        PartyB = "254" + payment.RecipientPhone.Substring(payment.RecipientPhone.Length - 9, 9),
                        Remarks = payment.Payroll.Cycle.Name,
                        ResultURL = callbackUrl + "?paymentId=" + payment.Id,
                        QueueTimeOutURL = callbackUrl + "?paymentId=" + payment.Id,
                        Occasion = "123",
                    };

                    var client = new RestClient(baseUrl);
                    var json = JsonConvert.SerializeObject(model);
                    RestRequest request = new RestRequest(requestUrl, Method.POST);

                    // request.AddParameter("grant_type", "client_credentials", ParameterType.QueryString);
                    request.AddHeader("Host", "sandbox.safaricom.co.ke");
                    request.AddHeader("Authorization", "Bearer " + accessCode);
                    request.AddHeader("Content-Type", "application/json");
                    request.RequestFormat = DataFormat.Json;
                    request.AddBody(model);

                    IRestResponse restResponse = await client.ExecuteAsync(request);
                    // Log Feedback
                    TestData Init = new TestData
                    {
                        Value = "PaymentId=" + payment.Id + "MakeMpesaPayment Time: " + DateTime.UtcNow.AddHours(3)
                                + " " + restResponse.Content
                    };
                    _context.TestData.Add(Init);
                    payment.StatusId = 4;
                    _context.SaveChanges();

                    success++;

                    Thread.Sleep(5000);
                }
               
                payments = _context.Payments
                .Include(i => i.Payroll.Cycle)
                .Where(i => i.PayrollId == id && i.StatusId == 1
                ).Take(10).ToList();
            }

            return success;
        }

        public IActionResult Processing(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        public async Task<IActionResult> CallBack(int id)
        {

            if (_context.Payments.Any(i => i.PayrollId == id && i.StatusId == 1))
            {
                await MakeMpesaPayment(id);


            }
            else
            {
                var payroll = _context.Payrolls
                    .Single(l => l.Id == id);
                payroll.StatusId = 3;
                payroll.DateApproved = DateTime.UtcNow.AddHours(3);
                payroll.ApprovedById = User.GetUserId();
                await _context.SaveChangesAsync();
                TempData["Message"] = "Payroll approved and payments dispatched. Wait for some few minutes and check the payment status.";
                return RedirectToAction("Details", new { id });

            }

            var pending = _context.Payments.Count(i => i.PayrollId == id && i.StatusId == 1);
            TempData["Pending"] = pending;
            return RedirectToAction("Processing", new { id });
        }

        public async Task<IActionResult> Reconcile(int id, IFormFile uploadfile)
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
        [HttpPost]
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
                bulkCopy.ColumnMappings.Add(7, "Col2");
                bulkCopy.ColumnMappings.Add(9, "Col3");

                bulkCopy.BulkCopyTimeout = 600;
                bulkCopy.DestinationTableName = "TempTable";
                bulkCopy.WriteToServer(table);
            }

            // Update the records
            var rowsAffected = _context.Database.ExecuteSqlCommand("exec PayrollReconciliation  @PayrollId",
                new SqlParameter("PayrollId", id)
            );
            var payroll = _context.Payrolls.Find(id);
            payroll.Reconciled = true;
            payroll.ReconciledById = User.GetUserId();
            payroll.ReconciliationStatusId = 1;
            payroll.DateReconciled = DateTime.UtcNow.AddHours(3);
            _uow.GetRepository<Payroll>().Update(payroll);
            _uow.Save();
            TempData["Success"] = "Reconciliation successful";
            return RedirectToAction("Details", new { Id = id });
        }


        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static byte[] EncryptDataOaepSha1(X509Certificate2 cert, byte[] data)
        {
            // GetRSAPublicKey returns an object with an independent lifetime, so it should be
            // handled via a using statement.
            using (RSA rsa = cert.GetRSAPublicKey())
            {
                // OAEP allows for multiple hashing algorithms, what was formermly just "OAEP" is
                // now OAEP-SHA1.
                return rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1);
            }
        }

        private static async Task<string> GenerateSafaricomTokenAsync(string consumerKey, string consumerSecret, string grantType)

        {
            RestClient restClient = new RestClient
            {
                Authenticator = new HttpBasicAuthenticator(consumerKey, consumerSecret),
                BaseUrl = new Uri("https://sandbox.safaricom.co.ke"),
            };

            RestRequest request = new RestRequest("/oauth/v1/generate", Method.GET);

            request.AddParameter("grant_type", "client_credentials", ParameterType.QueryString);

            IRestResponse restResponse = await restClient.ExecuteAsync(request);

            if (restResponse != null && !string.IsNullOrEmpty(restResponse.Content))
            {
                return restResponse.Content;
            }

            return string.Empty;
        }

        private bool FundRequestExists(int id)
        {
            return _context.FundRequests.Any(e => e.Id == id);
        }
    }

    public static class RestClientExtensions
    {
        public static async Task<RestResponse> ExecuteAsync(this RestClient client, RestRequest request)
        {
            TaskCompletionSource<IRestResponse> taskCompletion = new TaskCompletionSource<IRestResponse>();
            RestRequestAsyncHandle handle = client.ExecuteAsync(request, r => taskCompletion.SetResult(r));
            return (RestResponse)(await taskCompletion.Task);
        }
    }
}