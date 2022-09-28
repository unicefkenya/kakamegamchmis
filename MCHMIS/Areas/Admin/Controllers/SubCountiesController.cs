using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MCHMIS.Data;
using MCHMIS.Models;

namespace MCHMIS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCountiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubCountiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/SubCounties
        public async Task<IActionResult> Index()
        {
            return View(await _context.SubCounties.ToListAsync());
        }

        // GET: Admin/SubCounties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCounty = await _context.SubCounties
                .SingleOrDefaultAsync(m => m.Id == id);
            if (subCounty == null)
            {
                return NotFound();
            }

            return View(subCounty);
        }

        // GET: Admin/SubCounties/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/SubCounties/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] SubCounty subCounty)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subCounty);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(subCounty);
        }

        // GET: Admin/SubCounties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCounty = await _context.SubCounties.SingleOrDefaultAsync(m => m.Id == id);
            if (subCounty == null)
            {
                return NotFound();
            }
            return View(subCounty);
        }

        // POST: Admin/SubCounties/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] SubCounty subCounty)
        {
            if (id != subCounty.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subCounty);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubCountyExists(subCounty.Id))
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
            return View(subCounty);
        }

        // GET: Admin/SubCounties/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCounty = await _context.SubCounties
                .SingleOrDefaultAsync(m => m.Id == id);
            if (subCounty == null)
            {
                return NotFound();
            }

            return View(subCounty);
        }

        // POST: Admin/SubCounties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subCounty = await _context.SubCounties.SingleOrDefaultAsync(m => m.Id == id);
            _context.SubCounties.Remove(subCounty);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubCountyExists(int id)
        {
            return _context.SubCounties.Any(e => e.Id == id);
        }
    }
}
