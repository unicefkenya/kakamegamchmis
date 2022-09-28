using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MCHMIS.Data;
using MCHMIS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace MCHMIS.Services
{
    public class MyUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public MyUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {
            _userManager = userManager;
            _context = context;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            var userRoles = await _userManager.GetRolesAsync(user);
            var role = userRoles.First();
            var userRoleId = _context.Roles.Single(i => i.Name == role).Id;
            var userprofiles = _context.RoleProfiles.Where(p => p.RoleId == userRoleId)
                .Select(p => p.SystemTask.Parent.Name + ":" + p.SystemTask.Name).ToList();
            var userProfiles = "";
            foreach (var task in userprofiles)
            {
                userProfiles += task;
            }

            identity.AddClaim(new Claim("UserRoles", userProfiles));
            return identity;
        }
    }
}
