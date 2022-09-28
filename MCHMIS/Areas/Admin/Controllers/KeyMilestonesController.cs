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
    public class KeyMilestonesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KeyMilestonesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/KeyMilestones
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.KeyMilestones.Include(k => k.ClinicVisit);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/KeyMilestones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var keyMilestone = await _context.KeyMilestones
                .Include(k => k.ClinicVisit)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (keyMilestone == null)
            {
                return NotFound();
            }

            return View(keyMilestone);
        }

        // GET: Admin/KeyMilestones/Create
        public IActionResult Create()
        {
            ViewData["ClinicVisitId"] = new SelectList(_context.ClinicVisits.Where(i=>i.VisitTypeId== 309), "Id", "Name", TempData["ClinicVisitId"]);
            return View();
        }

        // POST: Admin/KeyMilestones/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Order,ClinicVisitId")] KeyMilestone keyMilestone)
        {
            if (ModelState.IsValid)
            {
                _context.Add(keyMilestone);
                await _context.SaveChangesAsync();
                TempData["ClinicVisitId"] = keyMilestone.ClinicVisitId;
                return RedirectToAction(nameof(Create));
            }
            ViewData["ClinicVisitId"] = new SelectList(_context.ClinicVisits.Where(i=>i.VisitTypeId== 309), "Id", "Name", keyMilestone.ClinicVisitId);
            return View(keyMilestone);
        }

        // GET: Admin/KeyMilestones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var keyMilestone = await _context.KeyMilestones.FindAsync(id);
            if (keyMilestone == null)
            {
                return NotFound();
            }
            ViewData["ClinicVisitId"] = new SelectList(_context.ClinicVisits.Where(i=>i.VisitTypeId== 309), "Id", "Name", keyMilestone.ClinicVisitId);
            return View(keyMilestone);
        }

        // POST: Admin/KeyMilestones/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Order,ClinicVisitId")] KeyMilestone keyMilestone)
        {
            if (id != keyMilestone.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(keyMilestone);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KeyMilestoneExists(keyMilestone.Id))
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
            ViewData["ClinicVisitId"] = new SelectList(_context.ClinicVisits.Where(i=>i.VisitTypeId== 309), "Id", "Name", keyMilestone.ClinicVisitId);
            return View(keyMilestone);
        }

        // GET: Admin/KeyMilestones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var keyMilestone = await _context.KeyMilestones
                .Include(k => k.ClinicVisit)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (keyMilestone == null)
            {
                return NotFound();
            }

            return View(keyMilestone);
        }

        // POST: Admin/KeyMilestones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var keyMilestone = await _context.KeyMilestones.FindAsync(id);
            _context.KeyMilestones.Remove(keyMilestone);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KeyMilestoneExists(int id)
        {
            return _context.KeyMilestones.Any(e => e.Id == id);
        }
    }
}
