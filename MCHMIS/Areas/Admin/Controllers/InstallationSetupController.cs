using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MCHMIS.Data;
using MCHMIS.Interfaces;
using MCHMIS.Models;

namespace MCHMIS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class InstallationSetupController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _uow;

        public InstallationSetupController(ApplicationDbContext context, IUnitOfWork uow)
        {
            _context = context;
            _uow = uow;
        }

        // GET: Admin/InstallationSetup
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.InstallationSetup.Include(i => i.HealthFacility);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/InstallationSetup/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var installationSetup = await _context.InstallationSetup
                .Include(i => i.HealthFacility)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (installationSetup == null)
            {
                return NotFound();
            }

            return View(installationSetup);
        }

        // GET: Admin/InstallationSetup/Create
        public IActionResult Create()
        {
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities, "Id", "Name");
            return View();
        }

        // POST: Admin/InstallationSetup/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,HealthFacilityId")] InstallationSetup installationSetup)
        {
            if (ModelState.IsValid)
            {
                var existing = _context.InstallationSetup.FirstOrDefault();
                if (existing == null)
                    _uow.GetRepository<InstallationSetup>().Add(installationSetup);
                else
                {
                    existing.HealthFacilityId = installationSetup.HealthFacilityId;
                    _uow.GetRepository<InstallationSetup>().Update(existing);
                }

                _uow.Save();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities, "Id", "Name", installationSetup.HealthFacilityId);
            return View(installationSetup);
        }

        // GET: Admin/InstallationSetup/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var installationSetup = await _context.InstallationSetup.SingleOrDefaultAsync(m => m.Id == id);
            if (installationSetup == null)
            {
                return NotFound();
            }
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities, "Id", "Name", installationSetup.HealthFacilityId);
            return View(installationSetup);
        }

        // POST: Admin/InstallationSetup/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,HealthFacilityId")] InstallationSetup installationSetup)
        {
            if (id != installationSetup.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _uow.GetRepository<InstallationSetup>().Update(installationSetup);
                    _uow.Save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstallationSetupExists(installationSetup.Id))
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
            ViewData["HealthFacilityId"] = new SelectList(_context.HealthFacilities, "Id", "Name", installationSetup.HealthFacilityId);
            return View(installationSetup);
        }

        // GET: Admin/InstallationSetup/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var installationSetup = await _context.InstallationSetup
                .Include(i => i.HealthFacility)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (installationSetup == null)
            {
                return NotFound();
            }

            return View(installationSetup);
        }

        // POST: Admin/InstallationSetup/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var installationSetup = await _context.InstallationSetup.SingleOrDefaultAsync(m => m.Id == id);
            _context.InstallationSetup.Remove(installationSetup);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InstallationSetupExists(int id)
        {
            return _context.InstallationSetup.Any(e => e.Id == id);
        }
    }
}