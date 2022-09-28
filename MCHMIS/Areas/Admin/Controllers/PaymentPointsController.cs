using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MCHMIS.Data;
using MCHMIS.Models;

namespace MCHMIS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PaymentPointsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentPointsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/PaymentPoints
        public async Task<IActionResult> Index()
        {
            return View(await _context.PaymentPoints.ToListAsync());
        }

        // GET: Admin/PaymentPoints/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentPoint = await _context.PaymentPoints
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paymentPoint == null)
            {
                return NotFound();
            }

            return View(paymentPoint);
        }

        // GET: Admin/PaymentPoints/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/PaymentPoints/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Amount")] PaymentPoint paymentPoint)
        {
            if (ModelState.IsValid)
            {
                _context.Add(paymentPoint);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(paymentPoint);
        }

        // GET: Admin/PaymentPoints/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentPoint = await _context.PaymentPoints.FindAsync(id);
            if (paymentPoint == null)
            {
                return NotFound();
            }
            return View(paymentPoint);
        }

        // POST: Admin/PaymentPoints/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Amount")] PaymentPoint paymentPoint)
        {
            if (id != paymentPoint.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paymentPoint);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentPointExists(paymentPoint.Id))
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
            return View(paymentPoint);
        }

        // GET: Admin/PaymentPoints/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentPoint = await _context.PaymentPoints
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paymentPoint == null)
            {
                return NotFound();
            }

            return View(paymentPoint);
        }

        // POST: Admin/PaymentPoints/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var paymentPoint = await _context.PaymentPoints.FindAsync(id);
            _context.PaymentPoints.Remove(paymentPoint);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentPointExists(int id)
        {
            return _context.PaymentPoints.Any(e => e.Id == id);
        }
    }
}
