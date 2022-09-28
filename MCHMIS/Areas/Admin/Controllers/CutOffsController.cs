using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MCHMIS.Data;
using MCHMIS.Models;
using MCHMIS.ViewModels;
using System.Data.SqlClient;
using MCHMIS.Interfaces;

namespace MCHMIS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CutOffsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _uow;
        public CutOffsController(ApplicationDbContext context, IUnitOfWork uow)
        {
            _context = context;
            _uow = uow;
        }

        // GET: Admin/CutOffs
        public async Task<IActionResult> Index()
        {

            SqlParameter[] parms = new SqlParameter[]
            {
                new SqlParameter("@Id", 1)
            };
            var locationCutoffs = SQLExtensions.GetModelFromQuery<LocationCutoff>
                (_context, "EXEC [GetCutOffs] @Id", parms).ToList();

            var vm = new CutoffsViewModel
            {
                LocationCutoffs = locationCutoffs
            };
            return View(vm);
        }
        public async Task<IActionResult> Create(CutoffsViewModel vm)
        {

            SqlParameter[] parms = new SqlParameter[]
            {
                new SqlParameter("@Id", 1)
            };
            var locationCutoffs = SQLExtensions.GetModelFromQuery<LocationCutoff>
                (_context, "EXEC [GetCutOffs] @Id", parms).ToList();

            vm.LocationCutoffs = locationCutoffs;
            return View(vm);
        }
        public async Task<IActionResult> Save(CutoffsViewModel vm)
        {
            var affected = vm.IdValues.Where(i => i.Value != i.OldValue).ToList();
            foreach (var item in affected)
            {
                var cutOff=new CutOff
                {
                    SubLocationId = item.Id,
                    Value = (decimal)item.Value
                };
                if (CutOffExists(item.Id))
                {
                    _uow.GetRepository<CutOff>().Update(cutOff);
                }
                else
                {
                    _uow.GetRepository<CutOff>().Add(cutOff);
                }
            }
                
            _uow.Save();
            TempData["Info"] = "Cut off points saved.";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(int id)
        {

            var vm = new CutOffEditViewModel();
            var cutoff = await _context.CutOffs.Include(i=>i.SubLocation).SingleOrDefaultAsync(i=>i.SubLocationId==id);
            if (cutoff == null)
                vm.IsNew = true;
            else
            {
                vm.Value = cutoff.Value;
            }

            vm.SubLocationId = id;
            vm.SubLocationName = _context.SubLocations.Find(id).Name;
            return View(vm);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CutOffEditViewModel vm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var cutOff = _context.CutOffs.SingleOrDefault(i => i.SubLocationId == vm.SubLocationId);
                   
                    if (vm.IsNew)
                    {
                        cutOff = new CutOff
                        {
                            SubLocationId = vm.SubLocationId,
                            Value = (decimal)vm.Value
                        };
                        _uow.GetRepository<CutOff>().Add(cutOff);
                    }
                    else
                    {
                        cutOff.Value = (decimal)vm.Value;
                        _uow.GetRepository<CutOff>().Update(cutOff);
                    }
                   
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData["Error"] = "Cut off not found.";
                    return RedirectToAction(nameof(Index));
                }
                TempData["Info"] = "Cut off points Saved.";
                return RedirectToAction(nameof(Index));
            }
            return View(vm);
        }
        private bool CutOffExists(int id)
        {
            return _context.CutOffs.Any(e => e.SubLocationId == id);
        }

    }
}
