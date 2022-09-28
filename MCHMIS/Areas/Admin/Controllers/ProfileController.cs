using System.Threading.Tasks;
using MCHMIS.Controllers;
using MCHMIS.Data;
using MCHMIS.Extensions;
using MCHMIS.Models;
using MCHMIS.Services;
using MCHMIS.Services.Email;
using MCHMIS.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MCHMIS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSenderEnhance _emailSender;
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;
        private readonly IDBService _dbService;

        public ProfileController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSenderEnhance emailSender,
            ApplicationDbContext context,
            IDBService dbService,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _context = context;
            _dbService = dbService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel vm)
        {
            if (vm.OldPassword == vm.Password)
            {
                TempData["Error"] = "New password cannot be the same as the Current Password";
                return View(nameof(Index), vm);
            }
            var userId = User.GetUserId();
            var user = _context.Users.Find(userId);
            var result =
                await _signInManager.PasswordSignInAsync(user.Email, vm.OldPassword, false, lockoutOnFailure: false);

            // Save old credentials just in case change password does not happen.
            var oldHarsh = user.PasswordHash;
            var oldSecurityStamp = user.SecurityStamp;

            if (result.Succeeded)
            {
                await _userManager.RemovePasswordAsync(user);
                var result2 = await _userManager.AddPasswordAsync(user, vm.Password);
                if (result2.Succeeded)
                {
                    TempData["Message"] = "Password changed successfully.";

                }
                // ReSharper disable once RedundantIfElseBlock
                else
                {
                    // Return user old credentials
                    user.PasswordHash = oldHarsh;
                    user.SecurityStamp = oldSecurityStamp;
                    _context.SaveChanges();
                    AddErrors(result2);
                    //  TempData["Info"] = ;

                }
            }
            else
            {
                TempData["Error"] = "Current password is wrong. Input the correct current password and try again.";
            }
           // TempData["Info"] = result.ToString();
            return View(nameof(Index),vm);
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }

   
}