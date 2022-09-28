using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MCHMIS.Data;
using MCHMIS.Extensions;
using MCHMIS.Models;

namespace MCHMIS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SystemCodeDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SystemCodeDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/SystemCodeDetails
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SystemCodeDetails.OrderBy(i => i.SystemCodeId).ThenBy(i => i.OrderNo).Include(s => s.SystemCode);
            return View(await applicationDbContext.ToListAsync());
        }

        
        // GET: Admin/SystemCodeDetails/Create
        public IActionResult Create()
        {
            var systemCodeId = TempData["SystemCodeId"];
            ViewData["SystemCodeId"] = new SelectList(_context.SystemCodes, "Id", "Code", systemCodeId);
            var model=new SystemCodeDetail();
            if(systemCodeId!=null)
              model.SystemCodeId = (int) systemCodeId;
            //try
            //{
            //    model.OrderNo = decimal.Parse(TempData["NextId"].ToString());
            //}
            //catch (Exception ex)
            //{

            //}

            return View(model);
        }

        // POST: Admin/SystemCodeDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SystemCodeDetail systemCodeDetail)
        {
            systemCodeDetail.CreatedOn = DateTime.UtcNow.AddHours(3);
            systemCodeDetail.CreatedBy = User.GetUserId();
            if (ModelState.IsValid)
            {
                _context.Add(systemCodeDetail);
                await _context.SaveChangesAsync();
                //TempData["NextId"] =
                //    _context.SystemCodeDetails.Count(i => i.SystemCodeId == systemCodeDetail.SystemCodeId) + 1;
                TempData["SystemCodeId"] = systemCodeDetail.SystemCodeId;
                
                return RedirectToAction(nameof(Create));
            }
            ViewData["SystemCodeId"] = new SelectList(_context.SystemCodes, "Id", "Code", systemCodeDetail.SystemCodeId);
            return View(systemCodeDetail);
        }

        // GET: Admin/SystemCodeDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemCodeDetail = await _context.SystemCodeDetails.SingleOrDefaultAsync(m => m.Id == id);
            if (systemCodeDetail == null)
            {
                return NotFound();
            }
            ViewData["SystemCodeId"] = new SelectList(_context.SystemCodes, "Id", "Code", systemCodeDetail.SystemCodeId);
            return View(systemCodeDetail);
        }

        // POST: Admin/SystemCodeDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SystemCodeId,Code,Description,OrderNo,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn")] SystemCodeDetail systemCodeDetail)
        {
            if (id != systemCodeDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    systemCodeDetail.ModifiedOn = DateTime.UtcNow.AddHours(3);
                    systemCodeDetail.ModifiedBy = User.GetUserId();
                    _context.Update(systemCodeDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SystemCodeDetailExists(systemCodeDetail.Id))
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
            ViewData["SystemCodeId"] = new SelectList(_context.SystemCodes, "Id", "Code", systemCodeDetail.SystemCodeId);
            return View(systemCodeDetail);
        }

        // GET: Admin/SystemCodeDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemCodeDetail = await _context.SystemCodeDetails
                .Include(s => s.SystemCode)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (systemCodeDetail == null)
            {
                return NotFound();
            }

            return View(systemCodeDetail);
        }

        // POST: Admin/SystemCodeDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var systemCodeDetail = await _context.SystemCodeDetails.SingleOrDefaultAsync(m => m.Id == id);
            _context.SystemCodeDetails.Remove(systemCodeDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SystemCodeDetailExists(int id)
        {
            return _context.SystemCodeDetails.Any(e => e.Id == id);
        }
    }
}