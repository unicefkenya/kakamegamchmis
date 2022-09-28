using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MCHMIS.Data;
using MCHMIS.Interfaces;
using MCHMIS.Models;

namespace MCHMIS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SystemSettingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _uow;

        public SystemSettingsController(ApplicationDbContext context, IUnitOfWork uow)
        {
            _context = context;
            _context = context;
        }

        // GET: Admin/SystemSettings
        public async Task<IActionResult> Index()
        {
            return View(await _context.SystemSettings.ToListAsync());
        }

        // GET: Admin/SystemSettings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemSetting = await _context.SystemSettings
                .SingleOrDefaultAsync(m => m.Id == id);
            if (systemSetting == null)
            {
                return NotFound();
            }

            return View(systemSetting);
        }

        // GET: Admin/SystemSettings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/SystemSettings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,key,Value")] SystemSetting systemSetting)
        {
            if (ModelState.IsValid)
            {
                _uow.GetRepository<SystemSetting>().Add(systemSetting);
                _uow.Save();

                // _context.Add(systemSetting);
                // await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(systemSetting);
        }

        // GET: Admin/SystemSettings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemSetting = await _context.SystemSettings.SingleOrDefaultAsync(m => m.Id == id);
            if (systemSetting == null)
            {
                return NotFound();
            }
            return View(systemSetting);
        }

        // POST: Admin/SystemSettings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,key,Value")] SystemSetting systemSetting)
        {
            if (id != systemSetting.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(systemSetting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SystemSettingExists(systemSetting.Id))
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
            return View(systemSetting);
        }

        // GET: Admin/SystemSettings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemSetting = await _context.SystemSettings
                .SingleOrDefaultAsync(m => m.Id == id);
            if (systemSetting == null)
            {
                return NotFound();
            }

            return View(systemSetting);
        }

        // POST: Admin/SystemSettings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var systemSetting = await _context.SystemSettings.SingleOrDefaultAsync(m => m.Id == id);
            _context.SystemSettings.Remove(systemSetting);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SystemSettingExists(int id)
        {
            return _context.SystemSettings.Any(e => e.Id == id);
        }
    }
}