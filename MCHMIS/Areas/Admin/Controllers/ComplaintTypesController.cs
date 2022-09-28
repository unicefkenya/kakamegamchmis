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
    public class ComplaintTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ComplaintTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/ComplaintTypes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ComplaintTypes.Include(c => c.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/ComplaintTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var complaintType = await _context.ComplaintTypes
                .Include(c => c.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (complaintType == null)
            {
                return NotFound();
            }

            return View(complaintType);
        }

        // GET: Admin/ComplaintTypes/Create
        public IActionResult Create()
        {
            ViewData["ComplaintTypeCategoryId"] = new SelectList(_context.SystemCodeDetails.Where(i=>i.SystemCode.Code=="Complaint Categories"), "Id", "Code");
            return View();
        }

        // POST: Admin/ComplaintTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ComplaintTypeCategoryId")] ComplaintType complaintType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(complaintType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ComplaintTypeCategoryId"] = new SelectList(_context.SystemCodeDetails.Where(i=>i.SystemCode.Code=="Complaint Categories"), "Id", "Code", complaintType.CategoryId);
            return View(complaintType);
        }

        // GET: Admin/ComplaintTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var complaintType = await _context.ComplaintTypes.FindAsync(id);
            if (complaintType == null)
            {
                return NotFound();
            }
            ViewData["ComplaintTypeCategoryId"] = new SelectList(_context.SystemCodeDetails.Where(i=>i.SystemCode.Code=="Complaint Categories"), "Id", "Code", complaintType.CategoryId);
            return View(complaintType);
        }

        // POST: Admin/ComplaintTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ComplaintTypeCategoryId")] ComplaintType complaintType)
        {
            if (id != complaintType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(complaintType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComplaintTypeExists(complaintType.Id))
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
            ViewData["ComplaintTypeCategoryId"] = new SelectList(_context.SystemCodeDetails.Where(i=>i.SystemCode.Code=="Complaint Categories"), "Id", "Code", complaintType.CategoryId);
            return View(complaintType);
        }

        // GET: Admin/ComplaintTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var complaintType = await _context.ComplaintTypes
                .Include(c => c.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (complaintType == null)
            {
                return NotFound();
            }

            return View(complaintType);
        }

        // POST: Admin/ComplaintTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var complaintType = await _context.ComplaintTypes.FindAsync(id);
            _context.ComplaintTypes.Remove(complaintType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ComplaintTypeExists(int id)
        {
            return _context.ComplaintTypes.Any(e => e.Id == id);
        }
    }
}
