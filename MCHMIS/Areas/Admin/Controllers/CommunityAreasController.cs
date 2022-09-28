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
    public class CommunityAreasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CommunityAreasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/CommunityAreas
        public async Task<IActionResult> Index(int? id)
        {
            IQueryable<CommunityArea> applicationDbContext = _context.CommunityAreas.Include(c => c.Village);
            if (id != null)
                applicationDbContext = applicationDbContext.Where(i => i.VillageId == id);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/CommunityAreas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var communityArea = await _context.CommunityAreas
                .Include(c => c.Village)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (communityArea == null)
            {
                return NotFound();
            }

            return View(communityArea);
        }

        // GET: Admin/CommunityAreas/Create
        public IActionResult Create()
        {
            ViewData["VillageId"] = new SelectList(_context.Villages, "Id", "Name");
            return View();
        }

        // POST: Admin/CommunityAreas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,VillageId,Name,Code")] CommunityArea communityArea)
        {
            if (ModelState.IsValid)
            {
                _context.Add(communityArea);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["VillageId"] = new SelectList(_context.Villages, "Id", "Name", communityArea.VillageId);
            return View(communityArea);
        }

        // GET: Admin/CommunityAreas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var communityArea = await _context.CommunityAreas.FindAsync(id);
            if (communityArea == null)
            {
                return NotFound();
            }
            ViewData["VillageId"] = new SelectList(_context.Villages, "Id", "Name", communityArea.VillageId);
            return View(communityArea);
        }

        // POST: Admin/CommunityAreas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,VillageId,Name,Code")] CommunityArea communityArea)
        {
            if (id != communityArea.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(communityArea);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommunityAreaExists(communityArea.Id))
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
            ViewData["VillageId"] = new SelectList(_context.Villages, "Id", "Name", communityArea.VillageId);
            return View(communityArea);
        }

        // GET: Admin/CommunityAreas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var communityArea = await _context.CommunityAreas
                .Include(c => c.Village)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (communityArea == null)
            {
                return NotFound();
            }

            return View(communityArea);
        }

        // POST: Admin/CommunityAreas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var communityArea = await _context.CommunityAreas.FindAsync(id);
            _context.CommunityAreas.Remove(communityArea);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommunityAreaExists(int id)
        {
            return _context.CommunityAreas.Any(e => e.Id == id);
        }
    }
}
