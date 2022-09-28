using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using MCHMIS.Data;
using MCHMIS.Models;
using MCHMIS.Services;
using MCHMIS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MCHMIS.Controllers
{
    // [Route("[controller]/[action]")]
    [AllowAnonymous]
    public class SMSController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISMSService _smsService;
        private ISingleRegistryService _singleRegistryService;

        public SMSController(ApplicationDbContext context, ISMSService smsService, ISingleRegistryService singleRegistryService)
        {
            _context = context;
            _smsService = smsService;
            _singleRegistryService = singleRegistryService;
        }

        [Route("sms/test/{id}", Name = "TestSMS")]
        public async Task<JsonResult> TestAsync(string id)
        {
            var settings = _context.SystemSettings.ToList();
            var accessKey = settings.Single(i => i.key == "SMS.CLIENT.AccessKey").Value;
            var senderId = settings.Single(i => i.key == "SMS.SENDER.ID").Value;
            var apiKey = settings.Single(i => i.key == "SMS.ACCESS.ApiKey").Value;
            var clientId = settings.Single(i => i.key == "SMS.CLIENT.ID").Value;
            var request = new SendSMSViewModel
            {
                SenderId = senderId,
                ApiKey = apiKey,
                ClientId = clientId
            };
            // var auth = await _smsService.LoginSMS(login);
            _smsService.Send(request, accessKey, "0725282737", "Test Message 101");
            // _smsService.Send("0725282737", "Test Message 102");
            return Json("OK");
        }

        //sms/send?id=mchmissms
        [Route("sms/send/{id}", Name = "SendSMS")]
        public JsonResult Send(string id)
        {
            //// Get nearing visit
            // mchmissms
            var settings = _context.SystemSettings.ToList();
            var accessKey = settings.Single(i => i.key == "SMS.CLIENT.AccessKey").Value;
            var senderId = settings.Single(i => i.key == "SMS.SENDER.ID").Value;
            var apiKey = settings.Single(i => i.key == "SMS.ACCESS.ApiKey").Value;
            var clientId = settings.Single(i => i.key == "SMS.CLIENT.ID").Value;
            var request = new SendSMSViewModel
            {
                SenderId = senderId,
                ApiKey = apiKey,
                ClientId = clientId
            };
            if (!string.IsNullOrEmpty(id) && id.Equals("mchmissms"))
            {
                var offset = int.Parse(_context.SystemSettings.Single(i => i.key == "CLINIC.VISIT.REMINDER.OFFSET").Value);
                var gracePeriod = int.Parse(_context.SystemSettings.Single(i => i.key == "RECORDS.UPDATE.GRACE.PERIOD").Value);
                var exitNumber = int.Parse(_context.SystemSettings.Single(i => i.key == "RECORDS.UPDATE.EXIT.NUMBER").Value);
                var now = DateTime.UtcNow.AddHours(3);

                SqlParameter[] parms = new SqlParameter[]
                {
                    new SqlParameter("@period", offset),
                    new SqlParameter("@option", "upcoming"),
                };
                var cases = SQLExtensions.GetModelFromQuery<SchedularViewModel>(_context,
                    "EXEC [ClinicVisitsNotification] @period,@option", parms).ToList();

                foreach (var item in cases)
                {
                    var sms = _context.SMS.SingleOrDefault(i =>
                        i.ClinicVisitId == item.NextVisitClinicId && i.TriggerEvent == "REMINDER");
                    if (sms != null)
                    {
                        var message = sms.Message
                            .Replace("##NAME##", item.CommonName ?? item.FirstName)
                            .Replace("##DATE##", ((DateTime)item.NextVisit).ToString("dd/MM/yyyy"));
                        if (message.Contains("##BABYNAME##"))
                        {
                            var firstChild =
                                _context.Children.FirstOrDefault(i => i.Delivery.Pregnancy.CaseManagementId == item.Id);
                            if (firstChild != null)
                            {
                                message = message.Replace("##BABYNAME##", firstChild.Name.Split(' ')[0]);
                            }
                        }
                        _smsService.Send(request, accessKey, item.Phone, message);
                    }
                }

                // Get Due Visits (Remind the day after)
                parms = new SqlParameter[]
                {
                    new SqlParameter("@period", 1),
                    new SqlParameter("@option", "missed"),
                };
                cases = SQLExtensions.GetModelFromQuery<SchedularViewModel>(_context,
                    "EXEC [ClinicVisitsNotification] @period,@option", parms).ToList();
                foreach (var item in cases)
                {
                    var smses = _context.SMS.Where(i => i.TriggerEvent == "ANC.REMINDER").ToList();
                    foreach (var sms in smses)
                    {
                        var message = sms.Message
                            .Replace("##NAME##", item.CommonName ?? item.FirstName)
                            .Replace("##DATE##", ((DateTime)item.NextVisit).ToString("dd/MM/yyyy"));
                        _smsService.Send(request, accessKey, item.Phone, message);
                    }
                }

                // Get Forfeited Cases
                parms = new SqlParameter[]
                {
                    new SqlParameter("@period", gracePeriod),
                    new SqlParameter("@option", "forfeited"),
                };
                var missedCases =
                    SQLExtensions.GetModelFromQuery<SchedularViewModel>(_context,
                        "EXEC [ClinicVisitsNotification] @period,@option", parms).ToList();

                var paymentPoints = new List<BeneficiaryPaymentPoint>();
                foreach (var item in missedCases)
                {
                    var hhCase = _context.Pregnancies.Find(item.Id);
                    hhCase.MissedVisits = hhCase.MissedVisits + 1;
                    // Exit mothers who have reached the maximum allowed times
                    if (hhCase.MissedVisits == exitNumber)
                    {
                        var hh = _context.HouseholdRegs.Find(item.HouseholdId);
                        hh.StatusId = 27;

                        hhCase.ReasonId=hh.ReasonId = 7; // Missed Visits
                        hhCase.StatusId = 2;
                        hhCase.DateExited = DateTime.UtcNow.AddHours(3);
                    }
                    // check if the missed visit has payment triggers
                    var visit = _context.ClinicVisits.Find(item.NextVisitClinicId);
                    if (visit.PaymentPointId != null)
                    {
                        var beneficiaryPaymentPoint = new BeneficiaryPaymentPoint
                        {
                            HouseholdId = item.HouseholdId,
                            CaseManagementId = item.Id,
                            PaymentPointId = (int)visit.PaymentPointId,
                            DateCreated = DateTime.UtcNow.AddHours(3),
                            StatusId = 5 //Forfeited
                        };
                        paymentPoints.Add(beneficiaryPaymentPoint);
                    }
                }
                _context.BeneficiaryPaymentPoints.AddRange(paymentPoints);
                _context.SaveChanges();

                return Json("OK");
            }
            return Json("invalid");
        }
    }
}