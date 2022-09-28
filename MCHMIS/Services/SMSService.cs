using MCHMIS.Data;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MCHMIS.Areas.Admin.Controllers;
using MCHMIS.ViewModels;
using Microsoft.Rest;
using RestSharp;

namespace MCHMIS.Services
{
    public interface ISMSService
    {
        Task<Authentication> LoginSMS(LoginVm model);

        void Send(string PhoneNumber, string Message);

        void Send(SendSMSViewModel request, string accessKey, string PhoneNumber, string Message);

        void Send(SendSMSViewModel request, string accessKey);
    }

    public class SMSService : ISMSService
    {
        public string baseUrl { get; set; }
        private static HttpClient _client = new HttpClient();
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _http;
        public string accessToken { get; set; }
        public string destination { get; set; }
        private readonly ILogService _logService;

        public SMSService(ApplicationDbContext context, IHttpContextAccessor http, ILogService logService)
        {
            _context = context;
            _logService = logService;
            _http = http;
            baseUrl = _context.SystemSettings.Single(i => i.key == "SMS.BASE.URL").Value;

            if (_client.BaseAddress == null)
            {
                _client.BaseAddress = new Uri(baseUrl);
                _client.DefaultRequestHeaders.Accept.Clear();
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
        }

        public async void Send(string PhoneNumber, string Message)
        {
            var settings = _context.SystemSettings.ToList();
            var accessKey = settings.Single(i => i.key == "SMS.CLIENT.AccessKey").Value;
            var senderId = settings.Single(i => i.key == "SMS.SENDER.ID").Value;
            var apiKey = settings.Single(i => i.key == "SMS.ACCESS.ApiKey").Value;
            var clientId = settings.Single(i => i.key == "SMS.CLIENT.ID").Value;
            var requestParms = new SendSMSViewModel
            {
                SenderId = senderId,
                ApiKey = apiKey,
                ClientId = clientId
            };

            _logService.FileLog("SMS", PhoneNumber + ":" + Message);
            PhoneNumber = "254" + PhoneNumber.Substring(PhoneNumber.Length - 9, 9);
            List<SendSMSViewModel> items = new List<SendSMSViewModel>();

            var parm = new Messageparameter
            {
                Number = PhoneNumber,
                Text = Message
            };
            Messageparameter[] messageParameters = new[] { parm };
            requestParms.MessageParameters = messageParameters;
            var jsonData = JsonConvert.SerializeObject(requestParms);
            var stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var postUrl = _context.SystemSettings.Single(i => i.key == "SMS.SEND.URL").Value;
            _client.DefaultRequestHeaders.AcceptCharset.ParseAdd("utf-8");
            _client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            _client.DefaultRequestHeaders.Add("AccessKey", accessKey);

            var result = await _client.PostAsync(postUrl, stringContent).ConfigureAwait(false);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                string content = result.Content.AsString();
            }
        }

        public async void Send(SendSMSViewModel requestParms, string accessKey, string PhoneNumber, string Message)
        {
            _logService.FileLog("SMS", PhoneNumber + ":" + Message);
            PhoneNumber = "254" + PhoneNumber.Substring(PhoneNumber.Length - 9, 9);
            List<SendSMSViewModel> items = new List<SendSMSViewModel>();

            var parm = new Messageparameter
            {
                Number = PhoneNumber,
                Text = Message
            };
            Messageparameter[] messageParameters = new[] { parm };
            requestParms.MessageParameters = messageParameters;
            var jsonData = JsonConvert.SerializeObject(requestParms);
            var stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var postUrl = _context.SystemSettings.Single(i => i.key == "SMS.SEND.URL").Value;
            _client.DefaultRequestHeaders.AcceptCharset.ParseAdd("utf-8");
            _client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            _client.DefaultRequestHeaders.Add("AccessKey", accessKey);
            var result = await _client.PostAsync(postUrl, stringContent).ConfigureAwait(false);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                string content = result.Content.AsString();
            }
        }

        public async void Send(SendSMSViewModel requestParms, string accessKey)
        {
            foreach (var sms in requestParms.MessageParameters)
            {
                _logService.FileLog("SMS", sms.Number + ":" + sms.Text);
            }

            List<SendSMSViewModel> items = new List<SendSMSViewModel>();
            var jsonData = JsonConvert.SerializeObject(requestParms);
            var stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var postUrl = _context.SystemSettings.Single(i => i.key == "SMS.SEND.URL").Value;
            _client.DefaultRequestHeaders.AcceptCharset.ParseAdd("utf-8");
            _client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            _client.DefaultRequestHeaders.Add("AccessKey", accessKey);
            var result = await _client.PostAsync(postUrl, stringContent).ConfigureAwait(false);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                string content = result.Content.AsString();
            }
        }

        private async Task<string> Post(string path, Dictionary<string, string> values, string accessKey = "")
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("AccessKey", accessKey);
            //// Encode Values:

            var content2 = new FormUrlEncodedContent(values);
            // Post and get Response:
            var str = "";
            try
            {
                var response2 = await _client.PostAsync(path, content2);
                // Return Response:
                str = await response2.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
            }

            return str;
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

        public async Task<Authentication> LoginSMS(LoginVm model)
        {
            var login = new SendSMSView
            {
                grant_type = "password",
                username = model.UserName,
                password = model.Password,
                client_id = model.ClientId,
                client_secret = model.ClientSecret
            };

            var postUrl = _context.SystemSettings.Single(i => i.key == "SMS.TOKEN.URL").Value;
            var dict = ObjectToDictionary(login);

            var ssss = login.ToString();
            var auth = await this.Post(postUrl, dict);

            Debug.WriteLine(auth);
            var authentication = JsonConvert.DeserializeObject<Authentication>(auth);
            if (!string.IsNullOrEmpty(authentication.access_token))
            {
                var token = _context.SystemSettings.Single(i => i.key == "SMS.ACCESS.TOKEN");
                token.Value = authentication.access_token;
                _context.SaveChanges();
            }
            return authentication;
        }
    }

    public class SendSMSViewModel
    {
        public string SenderId { get; set; }
        public Messageparameter[] MessageParameters { get; set; }
        public string ApiKey { get; set; }
        public string ClientId { get; set; }
    }

    public class Messageparameter
    {
        public string Number { get; set; }
        public string Text { get; set; }
    }
}