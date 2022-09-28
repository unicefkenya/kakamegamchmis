using MCHMIS.Mobile.Database;
using MCHMIS.Mobile.Helpers;
using MCHMIS.Mobile.Interface;
using MCHMIS.Mobile.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using FormsToolkit;

namespace MCHMIS.Mobile.Services
{
    public class AppClient : IAppClient
    {
        private const string CctpSSOApiKey = "0c833t3w37jq58dj249dt675a465k6b0rz090zl3jpoa9jw8vz7y6awpj5ox0qmb";

        private readonly HttpClient client;

        public AppClient()
            : this(CctpSSOApiKey)
        {
        }

        public AppClient(string apiKey)
        {
            this.client = new HttpClient { BaseAddress = new Uri(Constants.BaseApiAddress) };
            this.client.DefaultRequestHeaders.Clear();
            this.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        #region User and Enumerator Account Mgt

        public async Task<AccountResponse> CreateToken(string email, string password)
        {
            var keyValues = new List<KeyValuePair<string, string>>
                                {
                                    new KeyValuePair<string, string>(
                                        "username",
                                        email),
                                    new KeyValuePair<string, string>(
                                        "password",
                                        password),
                                    new KeyValuePair<string, string>(
                                        "grant_type",
                                        "password"),
                                };

            var json = await this.PostForm("Token", keyValues);
            return JsonConvert.DeserializeObject<AccountResponse>(json);
        }

        public async Task<AuthKeyResponse> CreateToken(string authKey)
        {
            var keyValues = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(
                    "authkey",
                    authKey),

            };

            var json = await this.PostForm("api/Account/Token", keyValues);
            return JsonConvert.DeserializeObject<AuthKeyResponse>(json);
        }

        public async Task<AccountResponse> ForgotPassAsync(string username)
        {
            var form = new List<KeyValuePair<string, string>>
                           {
                               new KeyValuePair<string, string>(
                                   "username",
                                   username),
                           };

            var json = await this.PostForm("api/account/forgotpassword/", form).ConfigureAwait(true);
            return json == "" ? null : JsonConvert.DeserializeObject<AccountResponse>(json);
        }

        public async Task<AccountResponse> ForgotPasswordAsync(string username)
        {
            return await ForgotPassAsync(username);
        }
        public async Task<AuthKeyResponse> LoginAsync(string authKey) => await this.CreateToken(authKey);

        public async Task<AccountResponse> LoginAsync(string username, string password) =>
            await this.CreateToken(username, password);

        public async Task<EnumeratorLoginResponse> LoginEnumerator(string nationalId, string pin, string id)
        {
            var form = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("nationalId",nationalId),
                new KeyValuePair<string, string>("pin",pin),
                new KeyValuePair<string, string>("id",id),
            };

            var json = await this.PostForm("api/Enumerators/Login/", form).ConfigureAwait(true);

            return json == "" ? new EnumeratorLoginResponse() : JsonConvert.DeserializeObject<EnumeratorLoginResponse>(json);
        }

        #endregion User and Enumerator Account Mgt

        #region Targeting Listing



        public async Task<ListingOptionsResponse> GetListingSettings(string nationalIdNo, string pin, string id)
        {
            var form = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("nationalId",nationalIdNo),
                new KeyValuePair<string, string>("pin",pin),
                new KeyValuePair<string, string>("id",id),
            };

            var json = await this.PostForm("api/options/listing/", form).ConfigureAwait(true);

            return json == "" ? new ListingOptionsResponse() : JsonConvert.DeserializeObject<ListingOptionsResponse>(json);
        }


        public async Task<EnumeratorCVResponse> RecertificationListByEnumerator(string nationalIdNo, string pin, string id)
        {
            var form = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("nationalId",nationalIdNo),
                new KeyValuePair<string, string>("pin",pin),
                new KeyValuePair<string, string>("id",id),
            };

            var json = await this.PostForm("api/Options/Recertification", form).ConfigureAwait(true);
            return json == "" ? new EnumeratorCVResponse() : JsonConvert.DeserializeObject<EnumeratorCVResponse>(json);

        }

        //public async Task<ApiStatus> PostListing(Listing reg, LocalDeviceInfo deviceInfo)
        //{
        //    var regdict = ObjectToDictionary(reg).ToList();
        //    var model = ObjectToDictionary(deviceInfo).ToList();
        //    regdict.AddRange(model);
        //    var json = await this.PostForm("api/Targeting/Listing", regdict).ConfigureAwait(true);
        //    return json == "" ? new ApiStatus() : JsonConvert.DeserializeObject<ApiStatus>(json);
        //}

        #endregion Targeting Listing

        public async Task<EnumeratorCVResponse> ValidationListByEnumerator(string nationalIdNo, string pin, string id)
        {
            var form = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("nationalId",nationalIdNo),
                new KeyValuePair<string, string>("pin",pin),
                new KeyValuePair<string, string>("id",id),
            };
            var json = await this.PostForm("api/Options/CommunityVal", form).ConfigureAwait(true);
            return json == "" ? new EnumeratorCVResponse() : JsonConvert.DeserializeObject<EnumeratorCVResponse>(json);
        }


        public async Task<EnumeratorCVResponse> RegistrationListByEnumerator(string nationalIdNo, string pin, string id)
        {
            var form = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("nationalId",nationalIdNo),
                new KeyValuePair<string, string>("pin",pin),
                new KeyValuePair<string, string>("id",id),
            };
          
            var json = await this.PostForm("api/Targeting/GetCommunityValidation", form).ConfigureAwait(true);
          
            return json == "" ? new EnumeratorCVResponse() : JsonConvert.DeserializeObject<EnumeratorCVResponse>(json);

        }

        public async Task<EnumeratorCVResponse> GetTargetingData(string nationalIdNo, string pin, string id)
        {
            var form = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("nationalId",nationalIdNo),
                new KeyValuePair<string, string>("pin",pin),
                new KeyValuePair<string, string>("id",id),
            };
            var json = await this.PostForm("api/Options/TargetingData", form).ConfigureAwait(true);
            return json == "" ? new EnumeratorCVResponse() : JsonConvert.DeserializeObject<EnumeratorCVResponse>(json);
        }

        public async Task<ApiStatus> ForgotPin(string nationalIdNo, string emailAddress, string id)
        {
            var form = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("nationalId",nationalIdNo),
                new KeyValuePair<string, string>("emailAddress",emailAddress),
                new KeyValuePair<string, string>("id",id),
            };
            var json = await this.PostForm("api/Account/ForgotPin", form).ConfigureAwait(true);
            return json == "" ? new ApiStatus() : JsonConvert.DeserializeObject<ApiStatus>(json);
        }

        public async Task<ApiStatus> ChangePin(string currentPin, string newPin, string id)
        {
            var form = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("nationalId",currentPin),
                new KeyValuePair<string, string>("emailAddress",newPin),
                new KeyValuePair<string, string>("id",id),
            };
            var json = await this.PostForm("api/Account/ChangePin", form).ConfigureAwait(true);
            return json == "" ? new ApiStatus() : JsonConvert.DeserializeObject<ApiStatus>(json);
        }
            
        //PostTargeting
        public async Task<ApiStatus> PostRegistration(Registration reg, LocalDeviceInfo deviceInfo)
        {
            var regdict = ObjectToDictionary(reg).ToList();
            var model = ObjectToDictionary(deviceInfo).ToList();
            regdict.AddRange(model);


            var form = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("HouseholdInfo",JsonConvert.SerializeObject(reg,Formatting.None)),
                new KeyValuePair<string, string>("DeviceInfo",JsonConvert.SerializeObject(deviceInfo,Formatting.None)),

            };
            var json = await this.PostForm("api/Targeting/PushCommunityValidation", form).ConfigureAwait(true);

            return json == "" ? new ApiStatus() : JsonConvert.DeserializeObject<ApiStatus>(json);
        }

        //public async Task<ApiStatus> PostListingCommVal(ComValListingPlanHH reg, LocalDeviceInfo deviceInfo)
        //{
        //    var regdict = ObjectToDictionary(reg).ToList();
        //    var model = ObjectToDictionary(deviceInfo).ToList();
        //    regdict.AddRange(model);
        //    var json = await this.PostForm("api/Targeting/CommVal/", regdict).ConfigureAwait(true);
        //    return json == "" ? new ApiStatus() : JsonConvert.DeserializeObject<ApiStatus>(json);
        //}

        public async Task LogoutAsync()
        {
            await this.PostForm("api/Account/Logout/", null).ConfigureAwait(true);

            await Task.FromResult(0);
        }

        private async Task<string> PostForm(string endpoint, List<KeyValuePair<string, string>> keyValues)
        {
            if (!string.IsNullOrEmpty(Settings.Current.AccessToken))
            {
                if (endpoint != "Token")
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.Current.AccessToken);
                }
            }

            var response = await this.client.PostAsync(
                               this.client.BaseAddress + endpoint,
                               new FormUrlEncodedContent(keyValues));

           
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var data= await response.Content.ReadAsStringAsync();
                return data;
            }
            else
            {
                return "";
            }
        }

        public static Dictionary<string, string> ObjectToDictionary(object obj)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();

            foreach (PropertyInfo prop in obj.GetType().GetProperties())
            {
                string propName = prop.Name;
                var val = obj.GetType().GetProperty(propName).GetValue(obj, null);
                if (val != null)
                {
                    ret.Add(propName, val.ToString());
                }
                else
                {
                    ret.Add(propName, null);
                }
            }

            return ret;
        }

        public static void Merge<TKey, TValue>(IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second)
        {
            if (second == null || first == null)
            {
                return;
            }

            foreach (var item in second)
            {
                if (!first.ContainsKey(item.Key))
                {
                    first.Add(item.Key, item.Value);
                }
            }
        }
    }

}
