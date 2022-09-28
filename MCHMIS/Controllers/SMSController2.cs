using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MCHMIS.Data;
using MCHMIS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MCHMIS.Controllers
{
 
  
    public class SMSController2 : Controller
    {
       
      
        [Route("sms/send/{id}", Name = "mchmissms")]
        public JsonResult Send(string id="mchmissms")
        {
            //// Get nearing visit
            if (!string.IsNullOrEmpty(id) && id.Equals("mchmissms"))
            {
                var offset = int.Parse(_context.SystemSettings.Single(i => i.key == "CLINIC.VISIT.REMINDER.OFFSET").Value);
                var now = DateTime.UtcNow;
                var cases = _context.CaseManagement.Include(i => i.Household).Where(i => i.NextVisit != null &&
                                                                                         i.NextVisit > now && now.Subtract((DateTime)i.NextVisit).Days == offset);
                foreach (var item in cases)
                {
                    var sms = _context.SMS.SingleOrDefault(i =>
                        i.ClinicVisitId == item.NextVisitClinicId && i.TriggerEvent == "REMINDER");
                    _smsService.Send(item.Household.Phone,
                        sms.Message.Replace("###NAME#", item.Household.CommonName ?? item.Household.Mother.FullName));
                }


                //// Get Missed Visits
                cases = _context.CaseManagement.Include(i => i.Household).Where(i => i.NextVisit != null &&
                                                                                     i.NextVisit < now);
                foreach (var item in cases)
                {
                    var sms = _context.SMS.SingleOrDefault(i =>
                        i.ClinicVisitId == item.NextVisitClinicId && i.TriggerEvent == "ANC.REMINDER");
                    _smsService.Send(item.Household.Phone,
                        sms.Message.Replace("###NAME#", item.Household.CommonName ?? item.Household.Mother.FullName));
                }
                return Json("OK");
            }
            
            return Json("invalid");
        }

       
    }
}