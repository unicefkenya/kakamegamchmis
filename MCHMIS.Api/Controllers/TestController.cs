using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using MCHMIS.Api.Data;
using MCHMIS.Api.Helpers;
using MCHMIS.Api.Models;

namespace MCHMIS.Api.Controllers
{
    public class TestController : ApiController
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        [Route("api/Test/Codes/")]
        [HttpGet]
        public IHttpActionResult PullCodes()
        {
            //var model =  new CvSetupVm
            //{
            //    SystemCodes = db.SystemCodes.ToList(),
            //    SystemCodeDetails = db.SystemCodeDetails.ToList()
            //};

            return Ok(EasyMD5.Hash("123456"));
        }

        [Route("api/Test/Login/")]
        [HttpGet]

        public IHttpActionResult Login(string id, string nationalId,  string pin)
        {
            var model = new LoginVM
            {
                Id = id, EmailAddress = "", Pin = pin, NationalId = nationalId

            };
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
           // returnModel = GetLoginData(enumerator.Id);

            return Ok(returnModel);



        }

        [Route("api/Test/GetCommunityValidation/{id}/")]
        [HttpGet]
        public IHttpActionResult GetCommunityValidation(int id)
        {

            var GenericService = new GenericService(new GenericRepository<ApplicationDbContext>(new ApplicationDbContext()));
            var returnModel = new CommValidationVm();
            var spName = "";
            var parameterNames = "@Id";
            var parameterList = new List<ParameterEntity>
            {
                new ParameterEntity { ParameterTuple =new Tuple<string, object>("Id",id)},
            };

            spName = "GetEnumeratorById";

            returnModel.Enumerator = GenericService.GetOneBySp<Enumerator>(spName, parameterNames, parameterList);

            spName = "GetHouseholdsByEnumerator";
            try
            {
                returnModel.Registrations = GenericService.GetManyBySp<HouseholdRegVm>(spName, parameterNames, parameterList).ToList();
            }
            catch (Exception e)
            {
                returnModel.Error = e.Message;
            }
            return Ok(returnModel);
        }



    }
}
