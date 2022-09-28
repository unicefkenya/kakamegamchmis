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
    public class ClinicVisitsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClinicVisitsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/ClinicVisits
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ClinicVisits.OrderBy(i=>i.VisitTypeId).ThenBy(i=>i.Order).Include(c => c.VisitType).Include(c => c.PaymentPoint);
            return View(await applicationDbContext.ToListAsync());
        }


        // GET: Admin/ClinicVisits/Create
        public IActionResult Create()
        {
            ViewData["VisitTypeId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Clinic Visit Types"), "Id", "Code");
            ViewData["PaymentPointId"] = new SelectList(_context.PaymentPoints, "Id", "Name");

            return View();
        }

        // POST: Admin/ClinicVisits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClinicVisit clinicVisit)
        {
            if (ModelState.IsValid)
            {
                _context.Add(clinicVisit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PaymentPointId"] = new SelectList(_context.PaymentPoints, "Id", "Name",clinicVisit.PaymentPointId);
            ViewData["VisitTypeId"] = new SelectList(_context.SystemCodeDetails.Where(i=>i.SystemCode.Code== "Clinic Visit Types"), "Id", "Code", clinicVisit.VisitTypeId);
            return View(clinicVisit);
        }

        // GET: Admin/ClinicVisits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clinicVisit = await _context.ClinicVisits.FindAsync(id);
            if (clinicVisit == null)
            {
                return NotFound();
            }
            ViewData["PaymentPointId"] = new SelectList(_context.PaymentPoints, "Id", "Name", clinicVisit.PaymentPointId);

            ViewData["VisitTypeId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Clinic Visit Types"), "Id", "Code", clinicVisit.VisitTypeId);
            return View(clinicVisit);
        }

        // POST: Admin/ClinicVisits/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,ClinicVisit clinicVisit)
        {
            if (id != clinicVisit.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clinicVisit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClinicVisitExists(clinicVisit.Id))
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
            ViewData["PaymentPointId"] = new SelectList(_context.PaymentPoints, "Id", "Name", clinicVisit.PaymentPointId);

            ViewData["VisitTypeId"] = new SelectList(_context.SystemCodeDetails.Where(i => i.SystemCode.Code == "Clinic Visit Types"), "Id", "Code", clinicVisit.VisitTypeId);
            return View(clinicVisit);
        }

      

        private bool ClinicVisitExists(int id)
        {
            return _context.ClinicVisits.Any(e => e.Id == id);
        }
    }
}
