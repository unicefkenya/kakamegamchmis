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
    public class WardsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WardsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Wards
        public async Task<IActionResult> Index(int? id)
        {
            IQueryable<Ward> applicationDbContext = _context.Wards.Include(w => w.Constituency);
            if (id != null)
                applicationDbContext = applicationDbContext.Where(i => i.SubCountyId == id);

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/Wards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ward = await _context.Wards
                .Include(w => w.Constituency)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (ward == null)
            {
                return NotFound();
            }

            return View(ward);
        }

        // GET: Admin/Wards/Create
        public IActionResult Create()
        {
            ViewData["ConstituencyId"] = new SelectList(_context.Constituencies, "Id", "Id");
            return View();
        }

        // POST: Admin/Wards/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ConstituencyId")] Ward ward)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ward);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ConstituencyId"] = new SelectList(_context.Constituencies, "Id", "Id", ward.ConstituencyId);
            return View(ward);
        }

        // GET: Admin/Wards/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ward = await _context.Wards.SingleOrDefaultAsync(m => m.Id == id);
            if (ward == null)
            {
                return NotFound();
            }
            ViewData["ConstituencyId"] = new SelectList(_context.Constituencies, "Id", "Id", ward.ConstituencyId);
            return View(ward);
        }

        // POST: Admin/Wards/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ConstituencyId")] Ward ward)
        {
            if (id != ward.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ward);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WardExists(ward.Id))
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
            ViewData["ConstituencyId"] = new SelectList(_context.Constituencies, "Id", "Id", ward.ConstituencyId);
            return View(ward);
        }

        // GET: Admin/Wards/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ward = await _context.Wards
                .Include(w => w.Constituency)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (ward == null)
            {
                return NotFound();
            }

            return View(ward);
        }

        // POST: Admin/Wards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ward = await _context.Wards.SingleOrDefaultAsync(m => m.Id == id);
            _context.Wards.Remove(ward);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WardExists(int id)
        {
            return _context.Wards.Any(e => e.Id == id);
        }
    }
}
