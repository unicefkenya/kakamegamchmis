using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MCHMIS.Data;
using MCHMIS.Interfaces;
using MCHMIS.Models;
using MCHMIS.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MCHMIS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RolesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _uow;
        public RolesController(ApplicationDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IUnitOfWork uow
        )
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _uow = uow;
        }

       
        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(RolesViewModel vm)
        {
           await _roleManager.CreateAsync(new IdentityRole(vm.Name));
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(string id)
        {
            var role = _roleManager.Roles.Single(i=>i.Id== id);
            var vm = new RolesViewModel
            {
                Name = role.Name,
                Id = role.Id
            };
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(RolesViewModel vm)
        {
            var role = _roleManager.Roles.Single(i => i.Id == vm.Id);
            role.Name = vm.Name;
            await _roleManager.UpdateAsync(role);
            TempData["Info"] = "Role updated.";
            return RedirectToAction(nameof(Index));
        }
    }
}