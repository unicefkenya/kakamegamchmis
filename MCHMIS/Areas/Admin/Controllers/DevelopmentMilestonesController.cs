using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MCHMIS.Data;
using MCHMIS.Models;

namespace MCHMIS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DevelopmentMilestonesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DevelopmentMilestonesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/DevelopmentMilestones
        public async Task<IActionResult> Index()
        {
            return View(await _context.DevelopmentMilestones.ToListAsync());
        }

        // GET: Admin/DevelopmentMilestones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var developmentMilestone = await _context.DevelopmentMilestones
                .FirstOrDefaultAsync(m => m.Id == id);
            if (developmentMilestone == null)
            {
                return NotFound();
            }

            return View(developmentMilestone);
        }

        // GET: Admin/DevelopmentMilestones/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/DevelopmentMilestones/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,NormalLimits,UpperLimit,LimitTag")] DevelopmentMilestone developmentMilestone)
        {
            if (ModelState.IsValid)
            {
                _context.Add(developmentMilestone);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(developmentMilestone);
        }

        // GET: Admin/DevelopmentMilestones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var developmentMilestone = await _context.DevelopmentMilestones.FindAsync(id);
            if (developmentMilestone == null)
            {
                return NotFound();
            }
            return View(developmentMilestone);
        }

        // POST: Admin/DevelopmentMilestones/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,NormalLimits,UpperLimit,LimitTag")] DevelopmentMilestone developmentMilestone)
        {
            if (id != developmentMilestone.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(developmentMilestone);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DevelopmentMilestoneExists(developmentMilestone.Id))
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
            return View(developmentMilestone);
        }

        // GET: Admin/DevelopmentMilestones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var developmentMilestone = await _context.DevelopmentMilestones
                .FirstOrDefaultAsync(m => m.Id == id);
            if (developmentMilestone == null)
            {
                return NotFound();
            }

            return View(developmentMilestone);
        }

        // POST: Admin/DevelopmentMilestones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var developmentMilestone = await _context.DevelopmentMilestones.FindAsync(id);
            _context.DevelopmentMilestones.Remove(developmentMilestone);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DevelopmentMilestoneExists(int id)
        {
            return _context.DevelopmentMilestones.Any(e => e.Id == id);
        }
    }
}
