using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MCHMIS.Data;
using MCHMIS.Models;
using MCHMIS.Services;
using MCHMIS.ViewModels;

namespace MCHMIS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Permission("Security:Manage Security")]
    public class ProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("admin/profiles/{id?}")]
        [Permission("Security:Manage Security")]
        public ActionResult Index(string id)
        {
            var rolesArray = _context.Roles.OrderBy(r => r.Name).ToList();

            var vm = new ProfilesViewModel();
            id = string.IsNullOrEmpty(id) ? rolesArray.First().Id : id;
            ViewBag.RoleId = new SelectList(rolesArray, "Id", "Name", id);
            vm.SystemTasks = _context.SystemTasks
                .Include(i => i.Children)
                .OrderBy(i => i.Id)
                .Where(t => t.ParentId == null).ToList();
            vm.RoleProfileIds = _context.RoleProfiles.Where(r => r.RoleId == id).Select(r => r.TaskId).ToList();

            return View(vm);
        }

        [Route("admin/profiles/save", Name = "ProfilesSave")]
        [Permission("Security:Manage Security")]
        public ActionResult Save(ProfilesViewModel model)
        {
            _context.RoleProfiles.RemoveRange(
                _context.RoleProfiles.Where(d => d.RoleId == model.RoleId));
            _context.SaveChanges();
            foreach (var taskId in model.Ids)
            {
                var roleProfile = new RoleProfile
                {
                    TaskId = taskId,
                    RoleId = model.RoleId
                };
                _context.RoleProfiles.Add(roleProfile);
            }

            _context.SaveChanges();

            TempData["Message"] = "Profile Updated";

            return RedirectToAction("Index", new { id = model.RoleId });
        }
    }
}