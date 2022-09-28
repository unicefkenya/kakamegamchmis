using MCHMIS.Api.Data;
using MCHMIS.Api.Helpers;
using MCHMIS.Api.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using System.Web.ModelBinding;
using System.Xml;
using System.Xml.Serialization;
using AutoMapper;

namespace MCHMIS.Api.Controllers
{
    public class TargetingController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        ///  Get Community Validation Data
        /// </summary>
        /// <param name="model">Login View Model</param>
        /// <returns> This is Endpoint that accepts the  Community Validation Households
        /// </returns>
        [Route("api/Targeting/GetCommunityValidation/")]
        [HttpPost]
        public IHttpActionResult GetCommunityValidation(LoginVM model)
        {
          //  FileLog("Pulling Data National Id:"+model.NationalId);
           // FileLog(JsonConvert.SerializeObject(model));
            var GenericService = new GenericService(new GenericRepository<ApplicationDbContext>(new ApplicationDbContext()));
            var returnModel = new CommValidationVm();
            var spName = "";
            var parameterNames = "@Id";
            var parameterList = new List<ParameterEntity>
            {
                new ParameterEntity { ParameterTuple =new Tuple<string, object>("Id",model.Id)},
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
            //FileLog(JsonConvert.SerializeObject(returnModel));
            return Ok(returnModel);
        }

        [ResponseType(typeof(ApiStatus))]
        [Route("api/Targeting/PushCommunityValidation/")]
        [HttpPost]
        public IHttpActionResult PushCommunityValidation(TabletHouseholdViewModel model)
        {
            FileLog("Pushing data at "+DateTime.UtcNow.AddHours(3));

            var genericService = new GenericService(new GenericRepository<ApplicationDbContext>(new ApplicationDbContext()));

           FileLog(JsonConvert.SerializeObject(model));
            ApiStatus apiFeedback;
            // var userId = db.Users.Single(i => i.Email == "admin@test.com").Id;
            var userId = "02e97990-17d2-41b5-a15f-6df20e1c502d";
            //  FileLog("UserId:" + userId);
            if (!ModelState.IsValid)
            {
                var description = $" {GetErrorListFromModelState(ModelState)} - The Posted Data has been rejected";
                FileLog(description);
                apiFeedback = new ApiStatus
                {
                    StatusId = -1,
                    Description = description
                };
                return Ok(apiFeedback);
            }
            try
            {
                var vm = JsonConvert.DeserializeObject<RegistrationHHVm>(model.HouseholdInfo);

                var deviceInfo = JsonConvert.DeserializeObject<TabEnvironment>(model.DeviceInfo);

                // Update CV CVListDetails
                var cvListDetails = db.CvListDetails.First(i => i.HouseholdId == vm.ParentId && i.StatusId == 9); // Pending 

                var parent = db.HouseholdRegs.Find(cvListDetails.HouseholdId);
                var household = new HouseholdReg
                {
                    Id = Guid.NewGuid().ToString().ToLower(),
                    ParentId = parent.Id,
                    VillageId = parent.VillageId,
                    SubLocationId = parent.SubLocationId,
                    HealthFacilityId = parent.HealthFacilityId,
                    CreatedById = userId,
                    DateCreated = DateTime.Now,
                    TypeId = 2,
                    StatusId = 3,// Awaiting Approval
                    Latitude = vm.Latitude,
                    Longitude = vm.Longitude,
                    CaptureStartDate = vm.StartTime,
                    CaptureEndDate = vm.EndTime,
                    RequiresIPRSECheck = false
                    //  EnumeratorId=vm.
                };
              
                db.HouseholdRegs.Add(household);

                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        FileLog("Error Saving Saving#################################HouseholdReg");
                        FileLog("Entity of type " + eve.Entry.Entity.GetType().Name +
                                " in state " + eve.Entry.State + " has the following validation errors:");

                        foreach (var ve in eve.ValidationErrors)
                        {
                            FileLog("- Property: " + ve.PropertyName + ", Error: " + ve.ErrorMessage);
                        }
                    }
                }
                catch (Exception ex)
                {
                    FileLog("Save HouseholdReg Error: " + ex.Message + "\n\n" + ex.InnerException);
                }

                if (vm.InterviewResultId != 185 && vm.InterviewResultId != 184)
                {
                    var xtics = new HouseholdRegCharacteristic
                    {
                        HouseholdId = household.Id,
                        HabitableRoomsNo = vm.HabitableRooms,
                        TenureStatusId = vm.TenureStatusId,
                        TenureOwnerOccupiedId = vm.IsOwnedId == 100 ? 269 : 270,
                        RoofMaterialId = vm.RoofConstructionMaterialId,
                        WallMaterialId = vm.WallConstructionMaterialId,
                        FloorMaterialId = vm.FloorConstructionMaterialId,
                        UnitRiskId = vm.DwellingUnitRiskId,
                        WaterSourceId = vm.WaterSourceId,
                        ToiletTypeId = vm.WasteDisposalModeId,
                        CookingFuelSourceId = vm.CookingFuelTypeId,
                        LightingSourceId = vm.LightingFuelTypeId,
                        LiveBirths = vm.LiveBirths,
                        Deaths = vm.Deaths,
                        HouseholdConditionId = vm.HouseHoldConditionId,
                        HasSkippedMealId = vm.IsSkippedMealId,
                        IsRecievingNSNPBenefit = vm.NsnpProgrammesId,
                        IsReceivingOtherBenefitId = vm.OtherProgrammesId,
                        OtherProgrammes = vm.OtherProgrammeNames,
                        // OtherProgrammesBenefitTypeId= vm.BenefitTypeId==0 ? DBNull.Value : vm.BenefitTypeId, //  Check
                        OtherProgrammesBenefitAmount = vm.LastReceiptAmount,
                        OtherProgrammesInKindBenefit = vm.InKindBenefitId,
                        BenefitFromFriendsRelativeId = vm.BenefitFromFriendsRelativeId ?? 95, // Null means No
                    };
                    if (vm.BenefitTypeId != 0)
                        xtics.OtherProgrammesBenefitTypeId = vm.BenefitTypeId;

                    // new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(householdInfo, xtics);

                    xtics.HouseholdId = household.Id;
                    db.HouseholdRegCharacteristics.Add(xtics);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            FileLog("Entity of type " + eve.Entry.Entity.GetType().Name +
                                    " in state " + eve.Entry.State + " has the following validation errors:");

                            foreach (var ve in eve.ValidationErrors)
                            {
                                FileLog("- Property: " + ve.PropertyName + ", Error: " + ve.ErrorMessage);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        FileLog("SaveChanges Error: " + ex.Message + "\n\n" + ex.InnerException);
                    }

                    FileLog(
                        "::::::::::::::::::::::::::::HouseholdRegCharacteristics Saved::::::::::::::::::::::::::::::::::");
                    // Get mothers details from registration Table

                    //var motherId = db.HouseholdRegs.Find(cvListDetails.HouseholdId).MotherId;
                    //var mother = db.HouseholdRegMembers.Find(motherId);
                    //var cvMother = new HouseholdRegMember
                    //{
                    //    Id =  Guid.NewGuid().ToString().ToLower(),
                    //    HouseholdId = household.Id,
                    //    FirstName = mother.FirstName,
                    //    MiddleName = mother.MiddleName,
                    //    Surname = mother.Surname,
                    //    IdNumber = mother.IdNumber,
                    //    IdentificationFormId = mother.IdentificationFormId,
                    //    DOB = mother.DOB,
                    //    MaritalStatusId = mother.MaritalStatusId,
                    //    GenderId = 192
                    //};

                    //db.HouseholdRegMembers.Add(cvMother);

                    // Add Members

                    ICollection<HouseholdRegMember> members = new List<HouseholdRegMember>();
                    foreach (var item in vm.RegistrationMembers)
                    {
                        var member = new HouseholdRegMember
                        {
                            Id = item.MemberId,
                            HouseholdId = household.Id,
                            FirstName = item.FirstName,
                            MiddleName = item.MiddleName,
                            Surname = item.Surname,
                            IdentificationFormId = item.IdentificationDocumentTypeId,
                            IdNumber = item.IdentificationNumber,
                            RelationshipId = item.RelationshipId,
                            GenderId = item.SexId,
                            DOB = DateTime.Parse(item.DateOfBirth),
                            MaritalStatusId = item.MaritalStatusId,
                            SpouseInHouseholdId = item.SpouseInHouseholdId,
                            FatherAliveId = item.FatherAliveStatusId,
                            MotherAliveId = item.MotherAliveStatusId,
                            ChronicIllnessId = item.ChronicIllnessStatusId,
                            DisabilityCaregiverId = item.CareGiverId,
                            DisabilityRequires24HrCareId = item.DisabilityCareStatusId,
                            EducationAttendanceId = item.LearningStatusId,
                            EducationLevelId = item.EducationLevelId,
                            SchoolTypeId = item.SchoolTypeId,
                            OccupationTypeId = item.WorkTypeId,
                            FormalJobTypeId = item.FormalJobNgoId,
                        };
                        members.Add(member);
                    }

                    db.HouseholdRegMembers.AddRange(members);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            FileLog("Entity of type " + eve.Entry.Entity.GetType().Name +
                                    " in state " + eve.Entry.State + " has the following validation errors:");

                            foreach (var ve in eve.ValidationErrors)
                            {
                                FileLog("- Property: " + ve.PropertyName + ", Error: " + ve.ErrorMessage);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        FileLog("SaveChanges Error: " + ex.Message + "\n\n" + ex.InnerException);
                    }

                   
                    var motherIdNumber = db.HouseholdRegMembers.Single(i => i.Id == parent.MotherId).IdNumber;
                    household.MotherId = db.HouseholdRegMembers.First(i => i.IdNumber == motherIdNumber).Id;

                    // Add disabilities of the members
                    ICollection<HouseholdRegMemberDisability> disabilities = new List<HouseholdRegMemberDisability>();
                    foreach (var item in vm.RegistrationMemberDisabilities)
                    {
                        var disability = new HouseholdRegMemberDisability
                        {
                            DisabilityId = item.DisabilityId,
                            HouseholdRegMemberId = item.RegistrationMemberId
                        };
                        disabilities.Add(disability);
                    }

                    // clear any previously saved assests, e.g. from failed
                    var memberIds = vm.RegistrationMemberDisabilities.Select(i=>i.RegistrationMemberId);
                    var existingDisabilities = db.HouseholdRegMemberDisabilities.Where(i => memberIds.Contains(i.HouseholdRegMemberId));
                    if (existingDisabilities.Any())
                    {
                        db.HouseholdRegMemberDisabilities.RemoveRange(existingDisabilities);
                        db.SaveChanges();
                    }
                    db.HouseholdRegMemberDisabilities.AddRange(disabilities);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            FileLog("Entity of type " + eve.Entry.Entity.GetType().Name +
                                    " in state " + eve.Entry.State + " has the following validation errors:");

                            foreach (var ve in eve.ValidationErrors)
                            {
                                FileLog("- Property: " + ve.PropertyName + ", Error: " + ve.ErrorMessage);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        FileLog("SaveChanges Error: " + ex.Message + "\n\n" + ex.InnerException);
                    }

                   
                    try
                    {
                    }
                    catch (Exception ex)
                    {
                        FileLog("SaveChanges Error: " + ex.Message + "\n\n" + ex.InnerException);
                    }

                    ICollection<HouseholdRegAsset> assets = new List<HouseholdRegAsset>();

                    var possibleAssets = db.SystemCodeDetails.Where(s =>
                            s.SystemCode.Code == "Household Assets" || s.SystemCode.Code == "Household Livestock")
                        .OrderBy(i => i.OrderNo);

                    // Items TV

                    var asset = new HouseholdRegAsset
                    {
                        HouseholdId = household.Id,
                        AssetId = possibleAssets.Single(i => i.Code == "Television").Id,
                        AssetTypeId = 203,
                        HasItem = false
                    };

                    if (vm.IsTelevisionId != 100)
                    {
                        asset.HasItem = true;
                    }

                    assets.Add(asset);

                    // Motorcycle
                    asset = new HouseholdRegAsset
                    {
                        HouseholdId = household.Id,
                        AssetTypeId = 203,
                    };
                    asset.AssetId = possibleAssets.Single(i => i.Code == "Motorcycle").Id;
                    asset.HasItem = vm.IsMotorcycleId == 100;
                    assets.Add(asset);

                    // IsTukTukId
                    asset = new HouseholdRegAsset
                    {
                        HouseholdId = household.Id,
                        AssetTypeId = 203,
                    };
                    asset.AssetId = possibleAssets.Single(i => i.Code == "Tuk-Tuk").Id;
                    asset.HasItem = vm.IsTukTukId == 100;
                    assets.Add(asset);

                    // IsCarId
                    asset = new HouseholdRegAsset
                    {
                        HouseholdId = household.Id,
                        AssetTypeId = 203,
                    };
                    asset.AssetId = possibleAssets.Single(i => i.Code == "Car").Id;
                    asset.HasItem = vm.IsCarId == 100;
                    assets.Add(asset);
                    // IsMobilePhoneId
                    asset = new HouseholdRegAsset
                    {
                        HouseholdId = household.Id,
                        AssetTypeId = 203,
                    };
                    asset.AssetId = possibleAssets.Single(i => i.Code == "Mobile phone").Id;
                    asset.HasItem = vm.IsMobilePhoneId == 100;
                    assets.Add(asset);

                    // IsBicycleId
                    asset = new HouseholdRegAsset
                    {
                        HouseholdId = household.Id,
                        AssetTypeId = 203,
                    };
                    asset.AssetId = possibleAssets.Single(i => i.Code == "Bicycle").Id;
                    asset.HasItem = vm.IsBicycleId == 100;
                    assets.Add(asset);

                    // ExoticCattle
                    asset = new HouseholdRegAsset
                    {
                        HouseholdId = household.Id,
                        AssetTypeId = 204,
                    };
                    asset.AssetId = possibleAssets.Single(i => i.Code == "Exotic cattle").Id;
                    asset.ItemCount = vm.ExoticCattle;
                    assets.Add(asset);
                    // IndigenousCattle
                    asset = new HouseholdRegAsset
                    {
                        HouseholdId = household.Id,
                        AssetTypeId = 204,
                    };
                    asset.AssetId = possibleAssets.Single(i => i.Code == "Indigenous cattle").Id;
                    asset.ItemCount = vm.IndigenousCattle;
                    assets.Add(asset);

                    // Sheep
                    asset = new HouseholdRegAsset
                    {
                        HouseholdId = household.Id,
                        AssetTypeId = 204,
                    };
                    asset.AssetId = possibleAssets.Single(i => i.Code == "Sheep").Id;
                    asset.ItemCount = vm.Sheep;
                    assets.Add(asset);

                    // Goats
                    asset = new HouseholdRegAsset
                    {
                        HouseholdId = household.Id,
                        AssetTypeId = 204,
                    };
                    asset.AssetId = possibleAssets.Single(i => i.Code == "Goats").Id;
                    asset.ItemCount = vm.Goats;
                    assets.Add(asset);

                    // Camels
                    asset = new HouseholdRegAsset
                    {
                        HouseholdId = household.Id,
                        AssetTypeId = 204,
                    };
                    asset.AssetId = possibleAssets.Single(i => i.Code == "Camels").Id;
                    asset.ItemCount = vm.Camels;
                    assets.Add(asset);

                    // Donkeys
                    asset = new HouseholdRegAsset
                    {
                        HouseholdId = household.Id,
                        AssetTypeId = 204,
                    };
                    asset.AssetId = possibleAssets.Single(i => i.Code == "Donkeys").Id;
                    asset.ItemCount = vm.Donkeys;
                    assets.Add(asset);

                    // Pigs
                    asset = new HouseholdRegAsset
                    {
                        HouseholdId = household.Id,
                        AssetTypeId = 204,
                    };
                    asset.AssetId = possibleAssets.Single(i => i.Code == "Pigs").Id;
                    asset.ItemCount = vm.Pigs;
                    assets.Add(asset);

                    // Chicken
                    asset = new HouseholdRegAsset
                    {
                        HouseholdId = household.Id,
                        AssetTypeId = 204,
                    };
                    asset.AssetId = possibleAssets.Single(i => i.Code == "Chicken").Id;
                    asset.ItemCount = vm.Chicken;
                    assets.Add(asset);
                    // clear any previously saved assests, e.g. from failed
                    var existing = db.HouseholdRegAssets.Where(i => i.HouseholdId == household.Id);
                    if (existing.Any())
                    {
                        db.HouseholdRegAssets.RemoveRange(existing);
                        db.SaveChanges();
                    }
                    db.HouseholdRegAssets.AddRange(assets);

                   

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            FileLog("Entity of type " + eve.Entry.Entity.GetType().Name +
                                    " in state " + eve.Entry.State + " has the following validation errors:");

                            foreach (var ve in eve.ValidationErrors)
                            {
                                FileLog("- Property: " + ve.PropertyName + ", Error: " + ve.ErrorMessage);
                            }
                        }
                    }
                }
                cvListDetails.StatusId = 3; // Awaiting approval
                cvListDetails.CVHouseHoldId = household.Id;
                cvListDetails.InterviewResultId = vm.InterviewResultId;
                cvListDetails.InterviewStatusId = vm.InterviewStatusId;
                cvListDetails.CannotFindHouseholdReasonId = vm.CannotFindHouseholdReasonId;
                cvListDetails.DateSubmitedByCHV = DateTime.UtcNow;
                db.SaveChanges();

                var serializer = new XmlSerializer(typeof(List<RegistrationHHVm>), new XmlRootAttribute("Registrations"));
                var settings = new XmlWriterSettings { Indent = false, OmitXmlDeclaration = true, };
                var xml = string.Empty;
                var listViewModel = new List<RegistrationHHVm> { vm };
                using (var sw = new StringWriter())
                {
                    var xw = XmlWriter.Create(sw, settings);
                    serializer.Serialize(xw, listViewModel);
                    xml += sw.ToString();
                }

                //  FileLog(xml);

                var serializerx = new XmlSerializer(typeof(List<TabEnvironment>), new XmlRootAttribute("Registrations"));
                var settingsx = new XmlWriterSettings { Indent = false, OmitXmlDeclaration = true, };
                var xmlx = string.Empty;
                var listViewModelx = new List<TabEnvironment> { deviceInfo };
                using (var sw = new StringWriter())
                {
                    var xw = XmlWriter.Create(sw, settingsx);
                    serializerx.Serialize(xw, listViewModelx);
                    xmlx += sw.ToString();
                }
                // FileLog(xmlx);
                var spName = "AddEditRegistrationHH";
                var parameterNames = "@HouseHoldInfoXml,@DeviceInfoXml,@NewHouseholdId";
                var parameterList = new List<ParameterEntity>
                {
                    new ParameterEntity { ParameterTuple =new Tuple<string, object>("HouseHoldInfoXml",xml)},
                    new ParameterEntity { ParameterTuple =new Tuple<string, object>("DeviceInfoXml",xmlx)},
                    new ParameterEntity { ParameterTuple =new Tuple<string, object>("NewHouseholdId",household.Id)},
                };

                var apiDbFeedback = genericService.GetOneBySp<ApiStatus>(spName, parameterNames, parameterList);

                apiFeedback = apiDbFeedback;

                apiFeedback = new ApiStatus
                {
                    StatusId = 0,
                    Description = "The HouseHold Registration details were Successfully Submitted"
                };
            }
            catch (Exception e)
            {
                FileLog("\n" + e.ToString());
                apiFeedback = new ApiStatus
                {
                    StatusId = -1,
                    Description = "\n" + e.Message + " \n" + e.InnerException?.StackTrace + " \n" + e.InnerException?.Message + " \n"
                };
            }
            return this.Ok(apiFeedback);
        }

        //[Route("api/Targeting/PushMeVal/")]
        //[HttpPost]
        //public IHttpActionResult PushMeVal(LoginVM model)
        //{
        //    return Ok(model);
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

        protected string GetErrorListFromModelState(System.Web.Http.ModelBinding.ModelStateDictionary modelState)
        {
            var message = string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
            return message;

            var query = from state in modelState.Values
                        from error in state.Errors
                        select error.ErrorMessage;
            var delimiter = " ";
            var errorList = query.ToList();
            return errorList.Aggregate((i, j) => i + delimiter + j);
        }
    }
}