using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Threading.Tasks;
using MCHMIS.Data;
using MCHMIS.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MCHMIS.Models;
using MCHMIS.Models.AccountViewModels;
using MCHMIS.Services;
using MCHMIS.Services.Email;
using MCHMIS.ViewModels;
using ForgotPasswordViewModel = MCHMIS.Models.AccountViewModels.ForgotPasswordViewModel;
using LoginViewModel = MCHMIS.Models.AccountViewModels.LoginViewModel;
using ResetPasswordViewModel = MCHMIS.Models.AccountViewModels.ResetPasswordViewModel;

namespace MCHMIS.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSenderEnhance _emailSender;
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;
        private readonly IDBService _dbService;

        public AccountController(
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

        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            var model = new LoginViewModel();
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            model.RecaptchaKey = _context.SystemSettings.Single(s => s.key == "RECAPTCHA.KEY").Value;
            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            //string response = model.RecaptchaResponse;
            ////var response = Request["g-recaptcha-response"];
            //if (string.IsNullOrEmpty(response))
            //{
            //    model.RecaptchaKey = _context.SystemSettings.Single(s => s.key == "RECAPTCHA.KEY").Value;
            //    ViewBag.CaptchaErrorMessage = "Please validate that you are not a robot.";
            //    return View(model);
            //}

            //string secretKey = _context.SystemSettings.Single(s => s.key == "RECAPTCHA.SECRET").Value;
            //var client = new WebClient();
            //var googleResult = client.DownloadString(
            //    $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={response}");
            //var obj = JObject.Parse(googleResult);
            //var status = (bool)obj.SelectToken("success");
            //if (!status)
            //{
            //    model.RecaptchaKey = _context.SystemSettings.Single(s => s.key == "RECAPTCHA.KEY").Value;
            //    ViewBag.CaptchaErrorMessage = "Error validating that you are not a robot.";
            //    return View(model);
            //}
            int passwordAttempts = int.Parse(_context.SystemSettings.Single(s => s.key == "PASSWORD.ATTEMPTS").Value);
            ViewData["ReturnUrl"] = returnUrl;
            model.CaptchaCode = model.CaptchaCode.ToUpper();
            if (!Captcha.ValidateCaptchaCode(model.CaptchaCode, HttpContext))
            {
                ModelState.AddModelError(string.Empty, "The code for confirming that you are not a Robot did not match.");
            }
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = _context.Users.Single(u => u.UserName == model.Email);
                    var userId = user.Id;
                    user.LastActivityDate = DateTime.Now;
                    if (user.LastPasswordChangedDate == null)
                    {
                        await _signInManager.SignOutAsync();
                        return RedirectToAction("ChangePassword", new { user.Id });
                    }

                    var passwordResetDays = int.Parse(_context.SystemSettings.Single(s => s.key == "PASSWORD.RESET.DAYS").Value);
                    if (user.LastPasswordChangedDate == null || ((DateTime)user.LastPasswordChangedDate).AddDays(passwordResetDays) < DateTime.Now)
                    {
                        await _signInManager.SignOutAsync();
                        return RedirectToAction("ChangePassword", new { user.Id });
                    }

                    // Check Last Activity Date
                    int accountIdleDays = int.Parse(_context.SystemSettings.Single(s => s.key == "ACCOUNT.IDLE.DAYS").Value);
                    if (user.LastActivityDate != null && ((DateTime)user.LastActivityDate).AddDays(accountIdleDays)
                        < DateTime.Now)
                    {
                        user.LockoutEnd = DateTime.Now.AddYears(50);
                        _context.Users.Update(user);
                        _context.SaveChanges();
                    }

                    string firstMacAddress = NetworkInterface
                        .GetAllNetworkInterfaces()
                        .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                        .Select(nic => nic.GetPhysicalAddress().ToString())
                        .FirstOrDefault();

                    user.MacAddress = firstMacAddress;
                    user.IsLoggedIn = true;

                    _logger.LogInformation("User logged in.");
                    var auditTrail = new AuditTrail
                    {
                        ChangeType = "Login",
                        TableName = "User",
                        Description = "Login Success",
                        UserId = userId
                    };

                    _dbService.AuditTrail(auditTrail);
                    _context.SaveChanges();

                    if (string.IsNullOrEmpty(returnUrl))
                        return Redirect("~/");
                    return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    var user = _context.Users.SingleOrDefault(u => u.UserName == model.Email);

                    if (user != null)
                    {
                        user.AccessFailedCount = user.AccessFailedCount + 1;

                        _context.SaveChanges();
                        var userId = user.Id;
                        var auditTrail = new AuditTrail
                        {
                            ChangeType = "Login",
                            TableName = "User",
                            Description = "Login Failure. Username:" + model.Email + "<br />" + result,
                            UserId = userId
                        };

                        _dbService.AuditTrail(auditTrail);
                        if (user.AccessFailedCount >= passwordAttempts)
                        {
                            user.LockoutEnd = DateTime.UtcNow.AddYears(99);
                            _logger.LogWarning("User account locked out.");
                            return RedirectToAction(nameof(Lockout));
                        }

                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        int remaining = passwordAttempts - user.AccessFailedCount;
                        ModelState.AddModelError(string.Empty, remaining + " attempt" + (remaining > 1 ? "s" : "") + " remaining.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    }

                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            model.CaptchaCode = "";
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var model = new LoginWith2faViewModel { RememberMe = rememberMe };
            ViewData["ReturnUrl"] = returnUrl;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with a recovery code.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid recovery code entered for user with ID {UserId}", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ChangePassword(string id)
        {
            var vm = new ChangePasswordForcedViewModel
            {
                Id = id
            };
            return View(vm);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordForcedViewModel vm, string returnUrl = null)
        {
            if (vm.OldPassword == vm.Password)
            {
                TempData["Error"] = "New password cannot be the same as the Current Password";
                return View(nameof(ChangePassword), vm);
            }
            if (ModelState.IsValid)
            {
                var user = _context.Users.Find(vm.Id);
                user.LastPasswordChangedDate = DateTime.UtcNow.AddHours(3);
                var result = await _userManager.ChangePasswordAsync(user, vm.OldPassword, vm.Password);
                if (result.Succeeded)
                {
                    ////  _logger.LogInformation("User created a new account with password.");

                    // var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    // var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    // await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    //   _logger.LogInformation("User created a new account with password.");
                    _context.SaveChanges();
                    return Redirect("~/admin");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(vm);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var userId = User.GetUserId();
            var user = _context.Users.Find(userId);

            user.IsLoggedIn = false;
            user.LastActivityDate = DateTime.UtcNow.AddHours(3);

            _logger.LogInformation("User logged in.");
            var auditTrail = new AuditTrail
            {
                ChangeType = "LogOff",
                TableName = "User",
                Description = "Log off success",
                UserId = userId
            };

            _dbService.AuditTrail(auditTrail);

            _context.SaveChanges();
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Login");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToAction(nameof(Login));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLogin", new ExternalLoginViewModel { Email = email });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    throw new ApplicationException("Error loading external login information during confirmation.");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(nameof(ExternalLogin), model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Login");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                // if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                var password = _dbService.GeneratePassword(6);
                await _userManager.RemovePasswordAsync(user);
                var result = await _userManager.AddPasswordAsync(user, password);
                if (result.Succeeded)
                {
                    var requestPath = new Postal.RequestPath();
                    requestPath.PathBase = Request.PathBase.ToString();
                    requestPath.Host = Request.Host.ToString();
                    requestPath.IsHttps = Request.IsHttps;
                    requestPath.Scheme = Request.Scheme;
                    requestPath.Method = Request.Method;

                    var emailData = new Postal.Email("AccountPassword");
                    emailData.RequestPath = requestPath;
                    emailData.ViewData["to"] = model.Email;
                    emailData.ViewData["Name"] = user.FirstName;
                    emailData.ViewData["Password"] = password;

                    await _emailSender.SendEmailAsync(emailData);
                }
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                user.LastPasswordChangedDate = null;
                _context.SaveChanges();
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        #endregion Helpers
    }
}