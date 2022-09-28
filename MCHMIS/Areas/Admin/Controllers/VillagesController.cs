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
    public class VillagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VillagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Villages
        public async Task<IActionResult> Index(int? id)
        {
            //var kvillages = _context.KakamegaVillages.ToList();
            //string wardName = "";
            //foreach (var village in kvillages)
            //{
            //    if (!string.IsNullOrEmpty(village.Ward))
            //        wardName = village.Ward;
            //    village.Ward = wardName;
            //}
            //_context.SaveChanges();

            //var list=new List<CommunityArea>();
            //var arreas = _context.KakamegaVillages.ToList();
            //string wardName = "";
            //foreach (var village in arreas)
            //{
            //    if (!string.IsNullOrEmpty(village.Ward))
            //        wardName = village.Ward;
            //    village.Ward = wardName;
            //}
            //_context.SaveChanges();

            IQueryable<Village> applicationDbContext = _context.Villages.Include(v => v.SubLocation).Include(v => v.Ward);
            if (id != null)
                applicationDbContext = applicationDbContext.Where(i => i.WardId == id);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/Villages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var village = await _context.Villages
                .Include(v => v.SubLocation)
                .Include(v => v.Ward)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (village == null)
            {
                return NotFound();
            }

            return View(village);
        }

        // GET: Admin/Villages/Create
        public IActionResult Create()
        {
            ViewData["SubLocationId"] = new SelectList(_context.SubLocations, "Id", "Name");
            ViewData["WardId"] = new SelectList(_context.Wards, "Id", "Id");
            return View();
        }

        // POST: Admin/Villages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,SubLocationId,WardId")] Village village)
        {
            if (ModelState.IsValid)
            {
                _context.Add(village);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SubLocationId"] = new SelectList(_context.SubLocations, "Id", "Name", village.SubLocationId);
            ViewData["WardId"] = new SelectList(_context.Wards, "Id", "Name", village.WardId);
            return View(village);
        }

        // GET: Admin/Villages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var village = await _context.Villages.FindAsync(id);
            if (village == null)
            {
                return NotFound();
            }
            ViewData["SubLocationId"] = new SelectList(_context.SubLocations, "Id", "Name", village.SubLocationId);
            ViewData["WardId"] = new SelectList(_context.Wards, "Id", "Name", village.WardId);
            return View(village);
        }

        // POST: Admin/Villages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,SubLocationId,WardId")] Village village)
        {
            if (id != village.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(village);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VillageExists(village.Id))
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
            ViewData["SubLocationId"] = new SelectList(_context.SubLocations, "Id", "Name", village.SubLocationId);
            ViewData["WardId"] = new SelectList(_context.Wards, "Id", "Name", village.WardId);
            return View(village);
        }

        // GET: Admin/Villages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var village = await _context.Villages
                .Include(v => v.SubLocation)
                .Include(v => v.Ward)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (village == null)
            {
                return NotFound();
            }

            return View(village);
        }

        // POST: Admin/Villages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var village = await _context.Villages.FindAsync(id);
            _context.Villages.Remove(village);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VillageExists(int id)
        {
            return _context.Villages.Any(e => e.Id == id);
        }
    }
}