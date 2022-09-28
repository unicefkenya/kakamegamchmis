using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MCHMIS.Data;
using MCHMIS.Interfaces;
using MCHMIS.Models;
using MCHMIS.Services;
using MCHMIS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace MCHMIS.Controllers
{
    [Route("[controller]/[action]")]
    [AllowAnonymous]
    public class PaymentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IUnitOfWork _uow;
        private readonly IHttpContextAccessor _http;
        private readonly ILogService _logService;
        private readonly ISMSService _smsService;

        public PaymentsController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment,
            IUnitOfWork uow, ILogService logService, ISMSService smsService)
        {
            _context = context;
            _uow = uow;
            _hostingEnvironment = hostingEnvironment;
            _logService = logService;
            _smsService = smsService;
        }

        public JsonResult CallBack(int paymentId)
        {
            string json = "called";

            Stream req = Request.Body;
            try
            {
                json = new System.IO.StreamReader(Request.Body).ReadToEnd();

                var feedback = new MPesaFeedBack
                {
                    PaymentId = paymentId,
                    Result = json
                };
                _context.MPesaFeedBack.Add(feedback);

                var vm = JsonConvert.DeserializeObject<RootObject>(json);
                //vm.Result.Id = paymentId;
                var payment = _context.Payments.Include(i => i.Beneficiary).SingleOrDefault(p => p.Id == paymentId);
                // _logService.FileLog("Payment", JsonConvert.SerializeObject(payment));
                if (payment != null)
                {
                    payment.TransactionId = vm.Result.TransactionID;
                    // If Successful
                    var dateTime = vm.Result.ResultParameters.ResultParameter
                        .Single(i => i.Key == "TransactionCompletedDateTime").Value.Replace('.', '-');
                    if (vm.Result.ResultCode == 0)
                    {
                        payment.StatusId = 2; // Paid
                        payment.Reconciled = true;
                        payment.TransactionReceipt = vm.Result.ResultParameters.ResultParameter.Single(i => i.Key == "TransactionReceipt").Value;
                        payment.PaymentDate = DateTime.ParseExact(dateTime, "dd-MM-yyyy HH:mm:ss", null); //DateTime.UtcNow.AddHours(3);
                        payment.ReceiverPartyPublicName = vm.Result.ResultParameters.ResultParameter.Single(i => i.Key == "ReceiverPartyPublicName").Value;
                        payment.Reconciled = true;
                    }
                    else
                    {
                        payment.StatusId = 3; // Failed
                        payment.PaymentErrorMessage = vm.Result.ErrorMessage;
                    }

                    // Update Payment transactions
                    var paymentTransaction = _context.PaymentTransactions
                        .Where(i => i.BeneficiaryId == payment.BeneficiaryId &&
                                    i.FundRequestId == payment.FundRequestId);
                    foreach (var item in paymentTransaction)
                    {
                        item.StatusId = payment.StatusId;
                    }

                    //Update beneficiary Payment Points
                    var beneficiaryPaymentPointIds = paymentTransaction.Select(i => i.BeneficiaryPaymentPointId).ToList();
                    var points =
                        _context.BeneficiaryPaymentPoints.Where(i => beneficiaryPaymentPointIds.Contains(i.Id));

                    var paymentPoints = "";
                    foreach (var item in points)
                    {
                        item.StatusId = payment.StatusId;
                        switch (item.PaymentPointId)
                        {
                            case 1:
                                paymentPoints += ",Kwanza";
                                break;

                            case 2:
                                paymentPoints += ",Pili,";
                                break;

                            case 3:
                                paymentPoints += ",Tatu,";
                                break;

                            case 4:
                                paymentPoints += ",Nne,";
                                break;

                            case 5:
                                paymentPoints += ",Tano,";
                                break;

                            case 6:
                                paymentPoints += ",Sita,";
                                break;
                        }
                    }
                    _context.Payments.Update(payment);

                    if (vm.Result.ResultCode == 0)
                    {
                        if (string.IsNullOrEmpty(paymentPoints))
                            paymentPoints = paymentPoints.Substring(1, paymentPoints.Length - 1);
                        // Send SMS
                        var sms = _context.SMS.SingleOrDefault(i =>
                            i.TriggerEvent == "PAYMENT.NOTIFICATION");
                        if (sms != null)
                        {
                            var message = sms.Message
                                .Replace("##NAME##", payment.BeneficiaryName)
                                .Replace("##AMOUNT##", payment.Amount.ToString())
                                .Replace(" ##PAYMENTS##", paymentPoints);

                            _smsService.Send(payment.RecipientPhone, message);
                        }
                    }


                    if (!_context.Payments.Any(i => i.PayrollId == payment.PayrollId && !i.Reconciled))
                    {
                        var payroll = _context.Payrolls.Find(payment.PayrollId);
                        payroll.Reconciled = true;
                        payroll.DateReconciled = DateTime.UtcNow.AddHours(3);

                        // Close Payment Cycle
                        var paymentCycle = _context.PaymentCycles.Find(payroll.CycleId);
                        paymentCycle.Closed = true;
                        _uow.GetRepository<PaymentCycle>().Update(paymentCycle);
                        var newPaymentCycle = new PaymentCycle
                        {
                            StartDate = paymentCycle.StartDate.AddMonths(1),
                            EndDate = paymentCycle.EndDate.AddMonths(1),
                            DateCreated = DateTime.UtcNow.AddHours(3),
                            Name = paymentCycle.StartDate.AddMonths(1).ToString("MMM yyyy")
                        };
                        _uow.GetRepository<PaymentCycle>().Add(newPaymentCycle);
                        _uow.Save();
                    }

                }
                _context.SaveChanges();
                // Check if all reconciliations have been done
                json = "executed-" + paymentId;
            }
            catch (Exception ex)
            {
                json = "MPESA Error: " + DateTime.UtcNow.AddHours(3) + " " + ex.ToString();
                var init = new TestData
                {
                    Value = "MPESA Error: " + DateTime.UtcNow.AddHours(3) + " " + ex.ToString()
                };
                _context.TestData.Add(init);
                _context.SaveChanges();
            }

            return Json(json);
        }

        public JsonResult CallBackManual(int id)
        {
            //Col1: Receipt No.	
            //Col2: Completion Time	
            //Col3: Other Party Info
            var pData = _context.TempTable.ToList();
            foreach (var paymentData in pData)
            {
                var split = paymentData.Col3.Split('-');
                string phone = split[0].Replace("Salary Payment to ","");
                string transactionReceipt = paymentData.Col1;
                DateTime paymentDate;
                string receiverPartyPublicName = split[1];

                var payment = _context.Payments.Include(i => i.Beneficiary)
                    .SingleOrDefault(p =>p.PayrollId== id && p.RecipientPhone == phone.Replace("254", "0"));
                // _logService.FileLog("Payment", JsonConvert.SerializeObject(payment));
                if (payment != null)
                {
                    payment.TransactionId = transactionReceipt;
                    // If Successful
                    try
                    {
                        paymentDate = DateTime.ParseExact(paymentData.Col2, "dd-MM-yyyy HH:mm:ss", null);
                    }
                    catch (Exception e)
                    {
                        var dateSplit = paymentData.Col2.Split(' ');
                        paymentDate =DateTime.Parse(dateSplit[0]);
                    }
                    
                    payment.StatusId = 2; // Paid
                    payment.Reconciled = true;
                    payment.TransactionReceipt = transactionReceipt;
                    payment.PaymentDate = paymentDate;
                    payment.ReceiverPartyPublicName = receiverPartyPublicName;

                    // Update Payment transactions
                    var paymentTransaction = _context.PaymentTransactions
                        .Where(i => i.BeneficiaryId == payment.BeneficiaryId &&
                                    i.FundRequestId == payment.FundRequestId);
                    foreach (var item in paymentTransaction)
                    {
                        item.StatusId = payment.StatusId;
                    }

                    //Update beneficiary Payment Points
                    var beneficiaryPaymentPointIds = paymentTransaction.Select(i => i.BeneficiaryPaymentPointId).ToList();
                    var points =
                        _context.BeneficiaryPaymentPoints.Where(i => beneficiaryPaymentPointIds.Contains(i.Id));

                    var paymentPoints = "";
                    foreach (var item in points)
                    {
                        item.StatusId = payment.StatusId;
                        switch (item.PaymentPointId)
                        {
                            case 1:
                                paymentPoints += ",Kwanza";
                                break;

                            case 2:
                                paymentPoints += ",Pili,";
                                break;

                            case 3:
                                paymentPoints += ",Tatu,";
                                break;

                            case 4:
                                paymentPoints += ",Nne,";
                                break;

                            case 5:
                                paymentPoints += ",Tano,";
                                break;

                            case 6:
                                paymentPoints += ",Sita,";
                                break;
                        }
                    }
                    _context.Payments.Update(payment);
                    try
                    {
                        if (string.IsNullOrEmpty(paymentPoints))
                            paymentPoints = paymentPoints.Substring(1, paymentPoints.Length - 1);
                        // Send SMS
                        var sms = _context.SMS.SingleOrDefault(i =>
                            i.TriggerEvent == "PAYMENT.NOTIFICATION");
                        if (sms != null)
                        {
                            var message = sms.Message
                                .Replace("##NAME##", payment.BeneficiaryName)
                                .Replace("##AMOUNT##", payment.Amount.ToString())
                                .Replace(" ##PAYMENTS##", paymentPoints);

                            //  _smsService.Send(payment.RecipientPhone, message);
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                }
                if (!_context.Payments.Any(i => i.PayrollId == id && !i.Reconciled))
                {
                    var payroll = _context.Payrolls.Find(payment.PayrollId);
                    payroll.Reconciled = true;
                    payroll.DateReconciled = DateTime.UtcNow.AddHours(3);

                    // Close Payment Cycle
                    var paymentCycle = _context.PaymentCycles.Find(payroll.CycleId);
                    paymentCycle.Closed = true;
                    _uow.GetRepository<PaymentCycle>().Update(paymentCycle);
                    var newPaymentCycle = new PaymentCycle
                    {
                        StartDate = paymentCycle.StartDate.AddMonths(1),
                        EndDate = paymentCycle.EndDate.AddMonths(1),
                        DateCreated = DateTime.UtcNow.AddHours(3),
                        Name = paymentCycle.StartDate.AddMonths(1).ToString("MMM yyyy")
                    };
                    _uow.GetRepository<PaymentCycle>().Add(newPaymentCycle);
                    _uow.Save();
                }

                _context.SaveChanges();
                // Check if all reconciliations have been done
            }

            string json = "called";
            //  DateTime PaymentDate=DateTime.Parse("")

            return Json(json);
        }
    }
}