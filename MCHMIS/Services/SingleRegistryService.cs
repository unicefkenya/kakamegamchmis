using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using MCHMIS.Data;
using MCHMIS.ViewModels;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace MCHMIS.Services
{
    public interface ISingleRegistryService
    {
        Task<Authentication> Login(LoginVm model);

        Task<SrVerificationVm> SrVerification(VerificationSrPostVm model);

        Task<IprsVerificationVm> IprsVerification(VerificationSrPostVm model);

        Task<SRAPI> ChangePassword(ChangeSrPasswordPostVm model);
    }

    public class SingleRegistryService : ISingleRegistryService
    {
        #region Properties:

        public string BaseUrl { get; set; }
        protected internal string Username { get; set; }
        protected internal string Password { get; set; }
        private Authentication Authentication { get; set; }
        public string AccessToken { get; set; }

        private static HttpClient _client = new HttpClient();
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _http;

        #endregion Properties:

        public SingleRegistryService(ApplicationDbContext context, IHttpContextAccessor http)
        {
            _context = context;
            _http = http;
            if (_client.BaseAddress == null)
            {
                _client.BaseAddress = new Uri(_context.SystemSettings.Single(i => i.key == "SR.SERVER").Value);
                _client.DefaultRequestHeaders.Accept.Clear();
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
        }

        private async Task<string> Post(string path, Dictionary<string, string> values)
        {
            // Add Access Token to the Header:
            if (this.Authentication != null)
                if (this.Authentication.TokenAuth != string.Empty)
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("TokenAuth", this.Authentication.TokenAuth);
            // Encode Values:
            var content = new FormUrlEncodedContent(values);
            // Post and get Response:
            var response = await _client.PostAsync(this.BaseUrl + path, content);
            // Return Response:
            return await response.Content.ReadAsStringAsync();
        }

        private static Dictionary<string, string> ObjectToDictionary(object obj)
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

        public async Task<Authentication> Login(LoginVm model)
        {
            var dic = new Dictionary<string, string>()
            {
                { "grant_type", "password" },
                { "username", model.UserName },
                { "password", model.Password }
            };
            var auth = await this.Post("Api/SR/ApiLogin", dic);
            Debug.WriteLine(auth);
            var authentication = JsonConvert.DeserializeObject<Authentication>(auth);
            return authentication;
        }

        public async Task<SrVerificationVm> SrVerification(VerificationSrPostVm model)
        {
            var dict = ObjectToDictionary(model);
            var content = await this.Post("Api/SR/SRVerification", dict);
            var srVerification = new SrVerificationVm();
            Debug.WriteLine(content);
            if (content == null) return srVerification;
            try
            {
                srVerification = JsonConvert.DeserializeObject<SrVerificationVm>(content);
            }
            catch (Exception e)
            {
                srVerification = JsonConvert.DeserializeObject<List<SrVerificationVm>>(content).First();
            }
            return srVerification;
        }

        public async Task<IprsVerificationVm> IprsVerification(VerificationSrPostVm model)
        {
            var dict = ObjectToDictionary(model);
            var content = await this.Post("Api/SR/IPRSVerification", dict);
            var srVerification = new IprsVerificationVm();
            Debug.WriteLine(content);
            if (content == null) return srVerification;
            try
            {
                srVerification = JsonConvert.DeserializeObject<IprsVerificationVm>(content);
            }
            catch (Exception e)
            {
                srVerification = JsonConvert.DeserializeObject<List<IprsVerificationVm>>(content).First();
            }
            return srVerification;
        }

        public async Task<SRAPI> ChangePassword(ChangeSrPasswordPostVm model)
        {
            var dict = ObjectToDictionary(model);
            var content = await this.Post("Api/SR/PasswordChange", dict);
            var srVerification = JsonConvert.DeserializeObject<SRAPI>(content);
            return srVerification;
        }
    }
}