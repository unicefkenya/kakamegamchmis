﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MCHMIS.Data;
using MCHMIS.Models;

namespace MCHMIS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ReportTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/ReportTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.ReportTypes.ToListAsync());
        }

        // GET: Admin/ReportTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reportType = await _context.ReportTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reportType == null)
            {
                return NotFound();
            }

            return View(reportType);
        }

        // GET: Admin/ReportTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/ReportTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] ReportType reportType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reportType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(reportType);
        }

        // GET: Admin/ReportTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reportType = await _context.ReportTypes.FindAsync(id);
            if (reportType == null)
            {
                return NotFound();
            }
            return View(reportType);
        }

        // POST: Admin/ReportTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] ReportType reportType)
        {
            if (id != reportType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reportType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReportTypeExists(reportType.Id))
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
            return View(reportType);
        }

        // GET: Admin/ReportTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reportType = await _context.ReportTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reportType == null)
            {
                return NotFound();
            }

            return View(reportType);
        }

        // POST: Admin/ReportTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reportType = await _context.ReportTypes.FindAsync(id);
            _context.ReportTypes.Remove(reportType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReportTypeExists(int id)
        {
            return _context.ReportTypes.Any(e => e.Id == id);
        }
    }
}
