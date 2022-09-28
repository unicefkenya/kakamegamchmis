using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MCHMIS.Data;
using MCHMIS.Extensions;
using MCHMIS.Models;

namespace MCHMIS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PaymentCyclesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentCyclesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/PaymentCycles
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PaymentCycles.OrderByDescending(p => p.Id).Include(p => p.CreatedBy);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/PaymentCycles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentCycle = await _context.PaymentCycles
                .Include(p => p.CreatedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paymentCycle == null)
            {
                return NotFound();
            }

            return View(paymentCycle);
        }

        // GET: Admin/PaymentCycles/Create
        public IActionResult Create()
        {
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Admin/PaymentCycles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,StartDate,EndDate,Reconciled,DateCreated,CreatedById")] PaymentCycle paymentCycle)
        {
            paymentCycle.DateCreated = DateTime.UtcNow.AddHours(3);
            paymentCycle.CreatedById = User.GetUserId();
            if (ModelState.IsValid)
            {
                _context.Add(paymentCycle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id", paymentCycle.CreatedById);
            return View(paymentCycle);
        }

        // GET: Admin/PaymentCycles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentCycle = await _context.PaymentCycles.FindAsync(id);
            if (paymentCycle == null)
            {
                return NotFound();
            }
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id", paymentCycle.CreatedById);
            return View(paymentCycle);
        }

        // POST: Admin/PaymentCycles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartDate,EndDate,Reconciled,DateCreated,CreatedById")] PaymentCycle paymentCycle)
        {
            if (id != paymentCycle.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paymentCycle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentCycleExists(paymentCycle.Id))
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
            ViewData["CreatedById"] = new SelectList(_context.Users, "Id", "Id", paymentCycle.CreatedById);
            return View(paymentCycle);
        }

        // GET: Admin/PaymentCycles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentCycle = await _context.PaymentCycles
                .Include(p => p.CreatedBy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paymentCycle == null)
            {
                return NotFound();
            }

            return View(paymentCycle);
        }

        // POST: Admin/PaymentCycles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var paymentCycle = await _context.PaymentCycles.FindAsync(id);
            _context.PaymentCycles.Remove(paymentCycle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentCycleExists(int id)
        {
            return _context.PaymentCycles.Any(e => e.Id == id);
        }
    }
}