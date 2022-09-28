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
    public class HealthFacilitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HealthFacilitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/HealthFacilities
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.HealthFacilities.Include(h => h.SubCounty);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/HealthFacilities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var healthFacility = await _context.HealthFacilities
                .Include(h => h.SubCounty)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (healthFacility == null)
            {
                return NotFound();
            }

            return View(healthFacility);
        }

        // GET: Admin/HealthFacilities/Create
        public IActionResult Create()
        {
            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name");
            return View();
        }

        // POST: Admin/HealthFacilities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,SubCountyId")] HealthFacility healthFacility)
        {
         
            if (ModelState.IsValid)
            {
                _context.Add(healthFacility);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name", healthFacility.SubCountyId);
            return View(healthFacility);
        }

        // GET: Admin/HealthFacilities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var healthFacility = await _context.HealthFacilities.SingleOrDefaultAsync(m => m.Id == id);
            if (healthFacility == null)
            {
                return NotFound();
            }
            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name", healthFacility.SubCountyId);
            return View(healthFacility);
        }

        // POST: Admin/HealthFacilities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,SubCountyId")] HealthFacility healthFacility)
        {
            if (id != healthFacility.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(healthFacility);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HealthFacilityExists(healthFacility.Id))
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
            ViewData["SubCountyId"] = new SelectList(_context.SubCounties, "Id", "Name", healthFacility.SubCountyId);
            return View(healthFacility);
        }

        // GET: Admin/HealthFacilities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var healthFacility = await _context.HealthFacilities
                .Include(h => h.SubCounty)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (healthFacility == null)
            {
                return NotFound();
            }

            return View(healthFacility);
        }

        // POST: Admin/HealthFacilities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var healthFacility = await _context.HealthFacilities.SingleOrDefaultAsync(m => m.Id == id);
            _context.HealthFacilities.Remove(healthFacility);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HealthFacilityExists(int id)
        {
            return _context.HealthFacilities.Any(e => e.Id == id);
        }
    }
}