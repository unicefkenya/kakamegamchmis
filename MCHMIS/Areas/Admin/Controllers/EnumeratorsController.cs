using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MCHMIS.Data;
using MCHMIS.Models;
using MCHMIS.Services.Email;
using MCHMIS.ViewModels;
using MCHMIS.Services;
using X.PagedList;

namespace MCHMIS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EnumeratorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSenderEnhance _emailSender;
        private readonly ISMSService _smsService;

        public EnumeratorsController(ApplicationDbContext context, IEmailSenderEnhance emailSender, ISMSService smsService)
        {
            _context = context;
            _emailSender = emailSender;
            _smsService = smsService;
        }

        // GET: Admin/Enumerators
        public async Task<IActionResult> Index(EnumeratorListViewModel vm)
        {
            var data = _context.Enumerators
                .Include(i => i.Village.Ward.SubCounty)
                .Include(e => e.EnumeratorGroup).AsQueryable();
            if (!string.IsNullOrEmpty(vm.Email))
            {
                data = data.Where(i => i.Email.Contains(vm.Email));
            }
            if (!string.IsNullOrEmpty(vm.MobileNo))
            {
                data = data.Where(i => i.MobileNo==vm.MobileNo);
            }
            if (!string.IsNullOrEmpty(vm.NationalIdNo))
            {
                data = data.Where(i => i.NationalIdNo == vm.NationalIdNo);
            }
            if (!string.IsNullOrEmpty(vm.Name))
            {
                data = data.Where(h =>
                    h.FirstName.Contains(vm.Name) || h.MiddleName.Contains(vm.Name) || h.Surname.Contains(vm.Name)
                );
            }

            var page = vm.Page ?? 1;
            var pageSize = vm.PageSize ?? 20;
            vm.Enumerators = data.ToPagedList(page, pageSize);

            return View(vm);
        }

        // GET: Admin/Enumerators/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enumerator = await _context.Enumerators
                .Include(e => e.EnumeratorGroup)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (enumerator == null)
            {
                return NotFound();
            }

            return View(enumerator);
        }

        [Permission("Security:Manage Security")]
        public IActionResult Create()
        {
            ViewData["EnumeratorGroupId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Enumerator Group"), "Id", "Code");
            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name");
            var vm = new EnumeratorViewModel();
            vm.Wards = _context.Wards.ToList();
            vm.Villages = _context.Villages.ToList();
            return View(vm);
        }

        [Permission("Security:Manage Security")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EnumeratorViewModel vm)
        {
            Enumerator enumerator = new Enumerator();

            if (string.IsNullOrEmpty(vm.Password))
            {
                vm.Password = RandomDigits(4);
            }
            string hashed = Hash(vm.Password);
            new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, enumerator);
            enumerator.PasswordHash = hashed;

            if (ModelState.IsValid)
            {
                // check if ID Number is already registred
                if (_context.Enumerators.Any(i => i.NationalIdNo == vm.NationalIdNo))
                {
                    TempData["Info"] = "An Enumerator with ID Number <strong>" + vm.NationalIdNo + "</strong> is already registered";
                }
                else if (_context.Enumerators.Any(i => i.MobileNo == vm.MobileNo))
                {
                    TempData["Info"] = "An Enumerator with phone number <strong>" + vm.MobileNo + "</strong> is already registered";
                }
                else
                {
                    _context.Add(enumerator);
                    await _context.SaveChangesAsync();

                    var requestPath = new Postal.RequestPath();
                    requestPath.PathBase = Request.PathBase.ToString();
                    requestPath.Host = Request.Host.ToString();
                    requestPath.IsHttps = Request.IsHttps;
                    requestPath.Scheme = Request.Scheme;
                    requestPath.Method = Request.Method;

                    TempData["Message"] = "Enumerator information saved and PIN sent to the phone and email.";
                    _smsService.Send(vm.MobileNo, "Hello " + vm.FirstName + ". Your pin is " + vm.Password + "...Imarisha Afya ya Mama na Mtoto...");

                    if (!string.IsNullOrEmpty(vm.Email))
                    {
                        var emailData = new Postal.Email("AccountPin")
                        {
                            RequestPath = requestPath,
                            ViewData =
                            {
                                ["to"] = vm.Email,
                                ["Name"] = vm.FirstName,
                                ["Password"] = vm.Password
                            }
                        };
                        await _emailSender.SendEmailAsync(emailData);
                    }

                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["EnumeratorGroupId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Enumerator Group"), "Id", "Code", vm.EnumeratorGroupId);
            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name", vm.SubCountyId);
            vm.Wards = _context.Wards.ToList();
            vm.Villages = _context.Villages.ToList(); return View(vm);
        }

        public string RandomDigits(int length)
        {
            var random = new Random();
            string s = string.Empty;
            for (int i = 0; i < length; i++)
                s = String.Concat(s, random.Next(1, 9).ToString());
            return s;
        }

        private static string GetMd5Hash(byte[] data)
        {
            var sBuilder = new StringBuilder();
            for (var i = 0; i < data.Length; i++)
                sBuilder.Append(data[i].ToString("x2"));
            return sBuilder.ToString();
        }

        public static string Hash(string data)
        {
            using (var md5 = MD5.Create())
                return GetMd5Hash(md5.ComputeHash(Encoding.UTF8.GetBytes(data)));
        }

        [Permission("Security:Manage Security")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enumerator = _context.Enumerators.Include(i => i.Village.Ward.SubCounty)
                .SingleOrDefault(i => i.Id == id);

            if (enumerator == null)
            {
                return NotFound();
            }

            var vm = new EnumeratorViewModel();
            new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(enumerator, vm);
            ViewData["EnumeratorGroupId"] =
                new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Enumerator Group"), "Id", "Code", enumerator.EnumeratorGroupId);
            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name", enumerator.Village?.Ward.SubCountyId);
            vm.SubCountyId = enumerator.Village?.Ward.SubCountyId;
            vm.WardId = enumerator.Village?.WardId;
            vm.Wards = _context.Wards.ToList();
            vm.Villages = _context.Villages.ToList();

            return View(vm);
        }

        [Permission("Security:Manage Security")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EnumeratorViewModel vm)
        {
            if (id != vm.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var enumerator = _context.Enumerators.Find(vm.Id);

                    new MapperConfiguration(cfg => cfg.ValidateInlineMaps = false).CreateMapper().Map(vm, enumerator);

                    TempData["Message"] = "Enumerator information saved.";
                    if (vm.ResetPin)
                    {
                        TempData["Message"] = "Enumerator information saved and PIN sent to the phone and email.";
                        if (string.IsNullOrEmpty(vm.Password))
                            vm.Password = RandomDigits(4);
                        string hashed = Hash(vm.Password);
                        enumerator.PasswordHash = hashed;

                        var requestPath = new Postal.RequestPath();
                        requestPath.PathBase = Request.PathBase.ToString();
                        requestPath.Host = Request.Host.ToString();
                        requestPath.IsHttps = Request.IsHttps;
                        requestPath.Scheme = Request.Scheme;
                        requestPath.Method = Request.Method;

                        _smsService.Send(vm.MobileNo, "Hello " + vm.FirstName + ". Your pin is " + vm.Password + "...Imarisha Afya ya Mama na Mtoto...");
                        if (!string.IsNullOrEmpty(vm.Email))
                        {
                            var emailData = new Postal.Email("AccountPin")
                            {
                                RequestPath = requestPath,
                                ViewData =
                                {
                                    ["to"] = vm.Email,
                                    ["Name"] = vm.FirstName,
                                    ["Password"] = vm.Password
                                }
                            };
                            await _emailSender.SendEmailAsync(emailData);
                        }
                    }
                    _context.Update(enumerator);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnumeratorExists(vm.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EnumeratorGroupId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Enumerator Group"), "Id", "Code", vm.EnumeratorGroupId);
            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name", vm.SubCountyId);

            vm.Wards = _context.Wards.ToList();
            vm.Villages = _context.Villages.ToList(); return View(vm);
        }

        // GET: Admin/Enumerators/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enumerator = await _context.Enumerators
                .Include(e => e.EnumeratorGroup)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (enumerator == null)
            {
                return NotFound();
            }

            return View(enumerator);
        }

        // POST: Admin/Enumerators/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enumerator = await _context.Enumerators.FindAsync(id);
            _context.Enumerators.Remove(enumerator);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EnumeratorExists(int id)
        {
            return _context.Enumerators.Any(e => e.Id == id);
        }
    }
}