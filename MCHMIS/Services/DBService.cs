using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MCHMIS.Data;
using MCHMIS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace MCHMIS.Services
{
    public class DBService : IDBService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _http;
        private readonly ApplicationUser _applicationUser;
        private readonly UserManager<ApplicationUser> _userManager;

        public DBService(ApplicationDbContext context, IHttpContextAccessor http,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _http = http;
            if(_http.HttpContext!=null)
             _applicationUser = userManager.GetUserAsync(_http.HttpContext.User).Result;
            _userManager = userManager;
        }

        public int GetHealthFacilityId()
        {
            int id = 0;
            //var setup = _context.InstallationSetup.FirstOrDefault();
            //if (setup != null)
            //    id = setup.HealthFacilityId;
            if (_applicationUser.HealthFacilityId != null)
            {
                id = (int)_applicationUser.HealthFacilityId;
            }
            return id;
        }

        public string GetHealthFacilityName()
        {
            string name = "";
            //var setup = _context.InstallationSetup.Include(i=>i.HealthFacility).FirstOrDefault();
            //if (setup != null)
            //    name = setup.HealthFacility.Name;
            var facilityId = GetHealthFacilityId();
            var facility = _context.HealthFacilities.Find(facilityId);
            if (facility != null)
                name = facility.Name;
            return name;
        }

        public string GetUserDisplayName()
        {
            return _applicationUser.DisplayName;
        }

        public int GetHealthFacilitySubCountyId()
        {
            int id = GetHealthFacilityId();
            var healthFacility = _context.HealthFacilities.Find(id);
            if (healthFacility != null)
                return healthFacility.SubCountyId;
            return 0;
        }

        public async Task<bool> IsGlobal()
        {
            var global = !await _userManager.IsInRoleAsync(_applicationUser, "Data Clerk")
                && !await _userManager.IsInRoleAsync(_applicationUser, "Nurse")
                && !await _userManager.IsInRoleAsync(_applicationUser, "CHEW");
            return global;
        }

        public void AuditTrail(AuditTrail auditTrail)
        {
            if (_applicationUser != null)
                auditTrail.UserId = _applicationUser.Id;
            auditTrail.Date = DateTime.UtcNow.AddHours(3);
            auditTrail.UserAgent = _http.HttpContext.Request.Headers["User-Agent"];
            // auditTrail.RequestIpAddress = GetExternalIP();
            auditTrail.RequestIpAddress = _http.HttpContext.Connection.RemoteIpAddress.ToString();
            _context.AuditTrail.Add(auditTrail);
            // _context.SaveChanges();
        }

        private string GetComputerName(string clientIP)
        {
            try
            {
                var hostEntry = Dns.GetHostEntry(clientIP);
                return hostEntry.HostName;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        private static string GetExternalIP()
        {
            try
            {
                string externalIP;
                externalIP = (new WebClient()).DownloadString("http://checkip.dyndns.org/");
                externalIP = (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}"))
                    .Matches(externalIP)[0].ToString();
                return externalIP;
            }
            catch { return null; }
        }

        public async Task<List<string>> UserProfileByName()
        {
            var userRoles = await _userManager.GetRolesAsync(_applicationUser);
            var role = userRoles.First();
            var userRoleId = _context.Roles.Single(i => i.Name == role).Id;
            var userprofiles = _context.RoleProfiles.Where(p => p.RoleId == userRoleId)
                .Select(p => p.SystemTask.Parent.Name + ":" + p.SystemTask.Name).ToList();
            return userprofiles;
        }
        public static string DBConnection()
        {

            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
                builder.AddEnvironmentVariables();
               var configuration = builder.Build();
                string connectionString = configuration.GetConnectionString("DefaultConnection");
                return connectionString;
            }
            catch
            {
                return "";
            }
        }
        public string GeneratePassword(int maxSize)
        {
            var passwords = string.Empty;
            var chArray1 = new char[52];
            var chArray2 = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^*()_+".ToCharArray();
            var data1 = new byte[1];
            using (var cryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                cryptoServiceProvider.GetNonZeroBytes(data1);
                var data2 = new byte[maxSize];
                cryptoServiceProvider.GetNonZeroBytes(data2);
                var stringBuilder = new StringBuilder(maxSize);
                foreach (var num in data2)
                    stringBuilder.Append(chArray2[(int)num % chArray2.Length]);
                passwords = stringBuilder.ToString();
                var number = Random("N");
                var upper = Random("S");
                var lower = Random("l");
                passwords += number + upper;
                return passwords;
            }
        }

        public string Random(string type)
        {
            var data2 = new byte[1];
            var passwords = string.Empty;
            switch (type)
            {
                case "N":
                    {
                        var charArray = "0123456789";
                        var stringBuilder = new StringBuilder(2);
                        foreach (var num in data2)
                            stringBuilder.Append(charArray[(int)num % charArray.Length]);
                        passwords = stringBuilder.ToString();
                        return passwords;
                    }

                case "l":
                    {
                        var charArray = "abcdefghijklmnopqrstuvwxyz";

                        var stringBuilder = new StringBuilder(2);
                        foreach (var num in data2)
                            stringBuilder.Append(charArray[(int)num % charArray.Length]);
                        passwords = stringBuilder.ToString();
                        return passwords;
                    }

                case "C":
                    {
                        var charArray = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

                        var stringBuilder = new StringBuilder(2);
                        foreach (var num in data2)
                            stringBuilder.Append(charArray[(int)num % charArray.Length]);
                        passwords = stringBuilder.ToString();
                        return passwords;
                    }

                case "S":
                    {
                        var charArray = "!@#$%^&*()_+-={}|[]:;<>?,./";
                        var stringBuilder = new StringBuilder(2);
                        foreach (var num in data2)
                            stringBuilder.Append(charArray[(int)num % charArray.Length]);
                        passwords = stringBuilder.ToString();
                        return passwords;
                    }
            }

            return string.Empty;
        }
    }
}