using System;
using MCHMIS.Api.Data;
using MCHMIS.Api.Helpers;
using MCHMIS.Api.Models;
using MCHMIS.Api.ViewModels;
using System.Linq;
using System.Web.Http;
using System.IO;
using System.Web;
using Newtonsoft.Json;

namespace MCHMIS.Api.Controllers
{
    /// <summary>
    /// Enumerators End Point
    /// </summary>
    [AllowAnonymous]
    public class EnumeratorsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Login Enumerator
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("api/Enumerators/Login/")]
        [HttpPost]
        public IHttpActionResult Login(LoginVM model)
        {
            //FileLog("Login Called");
            //FileLog(JsonConvert.SerializeObject(model));
            try
            {
                string hashPin;
                var returnModel = new CvSetupVm();
                Enumerator enumerator;
                if (string.IsNullOrEmpty(model.Id) && (string.IsNullOrEmpty(model.NationalId) || string.IsNullOrEmpty(model.Pin)))
                {
                    returnModel.Error = "Your National ID Number and Pin is required.";
                    return Ok(returnModel);
                }
                if (!string.IsNullOrEmpty(model.Id))
                {
                    var enumeratorId = int.Parse(model.Id);
                    enumerator = db.Enumerators.FirstOrDefault(x => x.Id == enumeratorId);
                }
                else
                {
                    hashPin = EasyMD5.Hash(model.Pin);
                    enumerator = db.Enumerators.FirstOrDefault(x => x.NationalIdNo == model.NationalId && x.PasswordHash == hashPin);
                }
                if (enumerator == null)
                {
                    returnModel.Error = "Your Account Does not exist or is Deactivated. \n Check National Id and PIN and Try Again";
                    return Ok(returnModel);
                }
                returnModel = GetLoginData(enumerator.Id);

                return Ok(returnModel);
            }
            catch (Exception e)
            {
                FileLog(e.Message);
                FileLog(e.StackTrace);
                FileLog(e.ToString());
                return Ok(e.Message);
            }
        }

        public CvSetupVm GetLoginData(int id)
        {
            var returnModel = new CvSetupVm
            {
                Enumerator = db.Enumerators.FirstOrDefault(x => x.Id == id),

                Wards = (db.Wards.Select(x => new WardVm()
                {
                    Id = x.Id,
                    Name = x.Name
                }).Distinct().ToList()),

                Villages = (db.Villages.Select(x => new VillageVm()
                {
                    Id = x.Id,
                    Name = x.Name
                }).Distinct().ToList()),

                CommunityAreas = (db.CommunityAreas.Select(x => new CommunityAreaVm()
                {
                    Id = x.Id,
                    Name = x.Name
                }).Distinct().ToList()),

                SystemCodes = (from c in this.db.SystemCodes
                               where (c.SystemModuleId == null)
                               select new SystemCodeVm()
                               {
                                   Id = c.Id,
                                   Code = c.Code,
                                   Description = c.Description
                               }).Distinct().ToList(),

                SystemCodeDetails =
                (from c in this.db.SystemCodeDetails
                 where (c.SystemCode.SystemModuleId == null)
                 select new SystemCodeDetailVm()
                 {
                     Id = c.Id,
                     Code = c.Code,
                     OrderNo = c.OrderNo,
                     SystemCodeId = c.SystemCodeId
                 }).Distinct().ToList()
            };

            return returnModel;
        }

        /// <summary>
        /// Logout Enumerator
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("api/Enumerators/Logout/")]
        [HttpPost]
        public IHttpActionResult Logout(LoginVM model)
        {
            return Ok(model);
        }

        /// <summary>
        /// Enumerator Change Pin EndPoint
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("api/Enumerators/ChangePin/")]
        [HttpPost]
        public IHttpActionResult ChangePin(LoginVM model)
        {
            return Ok(model);
        }

        /// <summary>
        /// Enumerator Reset Pin EndPoint
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("api/Enumerators/ResetPin/")]
        [HttpPost]
        public IHttpActionResult ResetPin(LoginVM model)
        {
            return Ok(model);
        }

        //[Route("api/Enumerators/PullSettings/")]
        //[HttpPost]
        //public IHttpActionResult PullSettings(LoginVM model)
        //{
        //    var returnModel = new CvSetupVm
        //    {
        //        SystemCodes = db.SystemCodes.ToList(),
        //        SystemCodeDetails = db.SystemCodeDetails.ToList()
        //    };
        //    return Ok(returnModel);

        //}
        private readonly string path = HttpContext.Current.Server.MapPath("~/logs/");

        public void FileLog(string data)
        {
            string t;
            int seconds;
            string todaydate, hour;
            var dt = DateTime.Now;
            seconds = dt.Second;
            todaydate = dt.Date.ToString("yyyy-MM-dd");
            var minute = dt.Date.ToString("mm");
            hour = dt.TimeOfDay.Hours.ToString();
            if (!Equals(seconds, dt.Second))
            {
                seconds = dt.Second;
            }

            t = dt.ToString("T");
            var fs = new FileStream(
                $"{this.path} log{todaydate}.txt",
                FileMode.OpenOrCreate,
                FileAccess.Write);
            using (var mStreamWriter = new StreamWriter(fs))
            {
                mStreamWriter.BaseStream.Seek(0, SeekOrigin.End);
                mStreamWriter.WriteLine("||            Date: {0}   Time : {1}", todaydate, t);

                mStreamWriter.WriteLine(" ****************************************************************************************************************************");
                mStreamWriter.WriteLine(data);
                mStreamWriter.WriteLine(" ****************************************************************************************************************************");
                mStreamWriter.Flush();
                mStreamWriter.Close();
            }
        }
    }
}