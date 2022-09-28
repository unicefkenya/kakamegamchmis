using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MCHMIS.Data;
using MCHMIS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Http;

namespace MCHMIS.Static
{
    public  class UserProfile
    {
        private  Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        private  ApplicationUser _applicationUser;
        private readonly IHttpContextAccessor _http;
        private readonly ApplicationDbContext _context;
        public UserProfile(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager,
            ApplicationDbContext context, IHttpContextAccessor http)
        {
            _context = context;
            _http = http;
            _applicationUser = userManager.GetUserAsync(_http.HttpContext.User).Result;
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
    }
}
