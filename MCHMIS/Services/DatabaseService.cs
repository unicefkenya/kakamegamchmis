using System;
using System.Net;
using System.Text.RegularExpressions;
using MCHMIS.Data;
using MCHMIS.Interfaces;
using MCHMIS.Models;
using Microsoft.AspNetCore.Http;
using MCHMIS.Extensions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Http.Features;

namespace MCHMIS.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly ApplicationDbContext db;
        private readonly IUnitOfWork _uow;
        private readonly IHttpContextAccessor _http;

        public DatabaseService(ApplicationDbContext context, IUnitOfWork uow, IHttpContextAccessor http)
        {
            _http = http;
            db = context;
        }

        public void AuditTrail(AuditTrail change)
        {
            var userId = _http.HttpContext.User.Identity.GetUserId();

            string ip = _http.HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress.ToString();

            string computerName = GetComputerName(ip);
            change.PCName = computerName;
            if (string.IsNullOrEmpty(change.UserId))
            {
                change.UserId = userId;
            }

            change.Date = DateTime.Now;
            var objBrwInfo = _http.HttpContext.Request.Headers["User-Agent"].ToString();
            change.UserAgent = objBrwInfo;// objBrwInfo.Browser + " " + objBrwInfo.Version + " " + objBrwInfo.Platform;
            change.RequestIpAddress = GetExternalIP();

            db.AuditTrail.Add(change);
            db.SaveChanges();
        }

        string IDatabaseService.GetComputerName(string clientIP)
        {
            return GetComputerName(clientIP);
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

        public string GetExternalIP()
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

        public void Dispose()
        {
            db.Dispose();
        }
    }
}