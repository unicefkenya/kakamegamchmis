using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MCHMIS.Data;
using MCHMIS.Models;

namespace MCHMIS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SMSController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SMSController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/SMS
        public async Task<IActionResult> Index()
        {
            var smses = _context.SMS
                 .OrderBy(i => i.Order)
                .Include(s => s.ClinicVisit);
            //int count = 1;
            //foreach (var sms in smses)
            //{
            //    sms.Order = count;
            //    count++;
            //}

            //_context.SaveChanges();
            return View(await smses.ToListAsync());
        }

        // GET: Admin/SMS/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sMS = await _context.SMS
                .Include(s => s.ClinicVisit)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sMS == null)
            {
                return NotFound();
            }

            return View(sMS);
        }

        // GET: Admin/SMS/Create
        public IActionResult Create()
        {
            ViewData["ClinicVisitId"] = new SelectList(_context.ClinicVisits.OrderBy(i => i.Order), "Id", "Name");
            var sms = new SMS();
            sms.Order = _context.SMS.Count() + 1;
            if (TempData["ClinicVisitId"] != null)
                sms.ClinicVisitId = int.Parse(TempData["ClinicVisitId"].ToString());
            if (TempData["TriggerEvent"] != null)
                sms.TriggerEvent = TempData["TriggerEvent"].ToString();

            return View(sms);
        }

        // POST: Admin/SMS/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SMS sMS)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sMS);
                await _context.SaveChangesAsync();
                TempData["ClinicVisitId"] = sMS.ClinicVisitId;
                TempData["TriggerEvent"] = sMS.TriggerEvent;
                return RedirectToAction(nameof(Create));
            }
            ViewData["ClinicVisitId"] = new SelectList(_context.ClinicVisits, "Id", "Name", sMS.ClinicVisitId);
            return View(sMS);
        }

        // GET: Admin/SMS/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sMS = await _context.SMS.FindAsync(id);
            if (sMS == null)
            {
                return NotFound();
            }
            ViewData["ClinicVisitId"] = new SelectList(_context.ClinicVisits, "Id", "Name", sMS.ClinicVisitId);
            return View(sMS);
        }

        // POST: Admin/SMS/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SMS sMS)
        {
            if (id != sMS.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sMS);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SMSExists(sMS.Id))
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
            ViewData["ClinicVisitId"] = new SelectList(_context.ClinicVisits, "Id", "Name", sMS.ClinicVisitId);
            return View(sMS);
        }

        // GET: Admin/SMS/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sMS = await _context.SMS
                .Include(s => s.ClinicVisit)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sMS == null)
            {
                return NotFound();
            }

            return View(sMS);
        }

        // POST: Admin/SMS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sMS = await _context.SMS.FindAsync(id);
            _context.SMS.Remove(sMS);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SMSExists(int id)
        {
            return _context.SMS.Any(e => e.Id == id);
        }
    }
}