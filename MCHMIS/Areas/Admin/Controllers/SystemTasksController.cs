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
    public class SystemTasksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SystemTasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/SystemTasks
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SystemTasks.Include(s => s.Children).Where(i=>i.ParentId==null);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/SystemTasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemTask = await _context.SystemTasks
                .Include(s => s.Parent)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (systemTask == null)
            {
                return NotFound();
            }

            return View(systemTask);
        }

        // GET: Admin/SystemTasks/Create
        public IActionResult Create()
        {
            ViewData["ParentId"] = new SelectList(_context.SystemTasks.Where(i=>i.ParentId==null), "Id", "Name", TempData["ParentId"]);
            return View();
        }

        // POST: Admin/SystemTasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ParentId,Name,Order")] SystemTask systemTask)
        {
            if (ModelState.IsValid)
            {
                _context.Add(systemTask);
                await _context.SaveChangesAsync();
                TempData["ParentId"] = systemTask.ParentId;
                return RedirectToAction(nameof(Create));
               
                // return RedirectToAction(nameof(Index));
            }
            ViewData["ParentId"] = new SelectList(_context.SystemTasks, "Id", "Name", systemTask.ParentId);
            return View(systemTask);
        }

        // GET: Admin/SystemTasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemTask = await _context.SystemTasks.SingleOrDefaultAsync(m => m.Id == id);
            if (systemTask == null)
            {
                return NotFound();
            }
            ViewData["ParentId"] = new SelectList(_context.SystemTasks, "Id", "Name", systemTask.ParentId);
            return View(systemTask);
        }

        // POST: Admin/SystemTasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ParentId,Name,Order")] SystemTask systemTask)
        {
            if (id != systemTask.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(systemTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SystemTaskExists(systemTask.Id))
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
            ViewData["ParentId"] = new SelectList(_context.SystemTasks, "Id", "Name", systemTask.ParentId);
            return View(systemTask);
        }

        // GET: Admin/SystemTasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemTask = await _context.SystemTasks
                .Include(s => s.Parent)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (systemTask == null)
            {
                return NotFound();
            }

            return View(systemTask);
        }

        // POST: Admin/SystemTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var systemTask = await _context.SystemTasks.SingleOrDefaultAsync(m => m.Id == id);
            _context.SystemTasks.Remove(systemTask);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SystemTaskExists(int id)
        {
            return _context.SystemTasks.Any(e => e.Id == id);
        }
    }
}
