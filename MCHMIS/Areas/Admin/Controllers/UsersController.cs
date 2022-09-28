using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MCHMIS.Data;
using MCHMIS.Interfaces;
using MCHMIS.Models;
using MCHMIS.Services;
using MCHMIS.Services.Email;
using MCHMIS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace MCHMIS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Permission("Security:Manage Security")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _uow;
        private readonly IEmailSenderEnhance _emailSender;
        private readonly IDBService _dbService;

        public UsersController(ApplicationDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IUnitOfWork uow,
            IEmailSenderEnhance emailSender,
            IDBService dbService
            )
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _uow = uow;
            _emailSender = emailSender;
            _dbService = dbService;
        }

        public async Task<ActionResult> Index(ListUserViewModel vm)
        {
            // var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var users = _context.Users
                .Include(i => i.HealthFacility)
                .Include(i => i.SubCounty)
                .AsQueryable();

            if (vm.HealthFacilityId != null)
            {
                users = users.Where(i => i.HealthFacilityId == vm.HealthFacilityId);
            }
            if (vm.HealthFacilityId != null)
            {
                users = users.Where(i => i.HealthFacilityId == vm.HealthFacilityId);
            }
            if (!string.IsNullOrEmpty(vm.Email))
            {
                users = users.Where(i => i.UserName.Contains(vm.Email));
            }
            if (!string.IsNullOrEmpty(vm.Name))
            {
                users = users.Where(h =>
                    h.FirstName.Contains(vm.Name) || h.MiddleName.Contains(vm.Name) || h.Surname.Contains(vm.Name)
                );
            }
            if (vm.Role != null)
            {
                var usersInRoles = await _userManager.GetUsersInRoleAsync(vm.Role);
                var userIds = usersInRoles.Select(i => i.Id);
                users = users.Where(i => userIds.Contains(i.Id));
                // users = users.Where(i => i. == vm.HealthFacilityId);
            }
            var page = vm.Page ?? 1;
            var pageSize = vm.PageSize ?? 20;

            var roles = new List<UserRoleViewModel>();

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                foreach (var role in userRoles)
                {
                    roles.Add(new UserRoleViewModel
                    {
                        UserId = user.Id,
                        Role = role
                    });
                }
            }
            var model = new ListUserViewModel()
            {
                Users = users.ToPagedList(page, pageSize),
                Roles = roles.ToList()
            };
            var rolesArray = _roleManager.Roles.OrderBy(i => i.Name).ToList();
            ViewData["Role"] = new SelectList(rolesArray, "Name", "Name");
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities, "Id", "Name", vm.HealthFacilityId);
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Create()
        {
            var rolesArray = _roleManager.Roles.OrderBy(i => i.Name).ToList();
            ViewData["Role"] = new SelectList(rolesArray, "Name", "Name", "Nurse");
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities, "Id", "Name");
            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name");
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser();
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, user);
                user.UserName = vm.Email;
                user.LastPasswordChangedDate = DateTime.UtcNow.AddHours(3);

                if (string.IsNullOrEmpty(vm.Password))
                    vm.Password = _dbService.GeneratePassword(6);

                var result = await _userManager.CreateAsync(user, vm.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, vm.Role);
                    user.LastPasswordChangedDate = null;
                    _context.SaveChanges();
                    var requestPath = new Postal.RequestPath();
                    requestPath.PathBase = Request.PathBase.ToString();
                    requestPath.Host = Request.Host.ToString();
                    requestPath.IsHttps = Request.IsHttps;
                    requestPath.Scheme = Request.Scheme;
                    requestPath.Method = Request.Method;

                    var emailData = new Postal.Email("AccountPassword");
                    emailData.RequestPath = requestPath;
                    emailData.ViewData["to"] = vm.Email;
                    emailData.ViewData["Name"] = vm.FirstName;
                    emailData.ViewData["Password"] = vm.Password;
                    emailData.ViewData["Url"] = _context.SystemSettings.Single(i => i.key == "ROOT.URL").Value;
                    await _emailSender.SendEmailAsync(emailData);
                    TempData["Message"] = "User information saved and password sent to the email.";
                    return RedirectToAction("Create");
                }
                AddErrors(result);
            }
            var rolesArray = _roleManager.Roles.OrderBy(i => i.Name).ToList();
            ViewData["Role"] = new SelectList(rolesArray, "Name", "Name", vm.Role);
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities, "Id", "Name", vm.HealthFacilityId);
            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name", vm.SubCountyId);
            return View(vm);
        }

        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ApplicationUser user = _context.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }
            var model = new UpdateProfileViewModel();

            new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(user, model);
            var rolesArray = _context.Roles.OrderBy(i => i.Name);

            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Any())
            {
                var userRole = userRoles[0];
                ViewBag.Role = new SelectList(rolesArray, "Name", "Name", userRole);
            }
            else
            {
                ViewBag.Role = new SelectList(rolesArray, "Name", "Name");
            }
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities, "Id", "Name", user.HealthFacilityId);
            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name", user.SubCountyId);
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(UpdateProfileViewModel vm)
        {
            ApplicationUser user = _context.Users.Find(vm.Id);
            var userRoles = await _userManager.GetRolesAsync(user);
            if (ModelState.IsValid)
            {
                new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, user);
                if (string.IsNullOrEmpty(vm.IsActive))
                {
                    user.LockoutEnd = DateTime.Now.AddYears(50);
                }
                else
                {
                    user.LockoutEnd = null;
                    user.AccessFailedCount = 0;
                }

                user.UserName = vm.Email;
                // user.LastActivityDate = DateTime.UtcNow.AddHours(3);

                TempData["Message"] = "User information saved.";
                // If reset Password
                if (vm.ResetPassword)
                {
                    // Save old credentials just in case change password does not happen.
                    var oldHarsh = user.PasswordHash;
                    var oldSecurityStamp = user.SecurityStamp;
                    if (string.IsNullOrEmpty(vm.Password))
                        vm.Password = _dbService.GeneratePassword(6);

                    //user.AccessFailedCount = 0;
                    await _userManager.RemovePasswordAsync(user);
                    var result = await _userManager.AddPasswordAsync(user, vm.Password);
                    user.LastPasswordChangedDate = null;
                    if (!result.Succeeded)
                    {
                        // Return user old credentials
                        user.PasswordHash = oldHarsh;
                        user.SecurityStamp = oldSecurityStamp;
                        _context.SaveChanges();
                        AddErrors(result);
                        var rolesArray2 = _context.Roles.OrderBy(i => i.Name);
                        if (userRoles.Any())
                        {
                            var userRole = userRoles[0];
                            ViewBag.Role = new SelectList(rolesArray2, "Name", "Name", userRole);
                        }
                        else
                        {
                            ViewBag.Role = new SelectList(rolesArray2, "Name", "Name");
                        }
                        ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities, "Id", "Name", user.HealthFacilityId);
                        return View(vm);
                    }

                    var requestPath = new Postal.RequestPath();
                    requestPath.PathBase = Request.PathBase.ToString();
                    requestPath.Host = Request.Host.ToString();
                    requestPath.IsHttps = Request.IsHttps;
                    requestPath.Scheme = Request.Scheme;
                    requestPath.Method = Request.Method;

                    var emailData = new Postal.Email("AccountPassword");
                    emailData.RequestPath = requestPath;
                    emailData.ViewData["to"] = vm.Email;
                    emailData.ViewData["Name"] = vm.FirstName;
                    emailData.ViewData["Password"] = vm.Password;
                    emailData.ViewData["Url"] = _context.SystemSettings.Single(i => i.key == "ROOT.URL").Value;

                    await _emailSender.SendEmailAsync(emailData);
                    TempData["Message"] = "User information saved and password sent to the email.";
                }

                _uow.GetRepository<ApplicationUser>().Update(user);
                _uow.Save();
                await _userManager.RemoveFromRolesAsync(user, userRoles);
                await _userManager.AddToRoleAsync(user, vm.Role);
                return RedirectToAction("Index");
            }

            var rolesArray = _context.Roles.OrderBy(i => i.Name);
            if (userRoles.Any())
            {
                var userRole = userRoles[0];
                ViewBag.Role = new SelectList(rolesArray, "Name", "Name", userRole);
            }
            else
            {
                ViewBag.Role = new SelectList(rolesArray, "Name", "Name");
            }
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities, "Id", "Name", user.HealthFacilityId);
            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name", user.SubCountyId);
            return View(vm);
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