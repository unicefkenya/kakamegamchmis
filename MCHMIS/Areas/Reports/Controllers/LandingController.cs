using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MCHMIS.Data;
using MCHMIS.Interfaces;
using MCHMIS.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace MCHMIS.Areas.Reports.Controllers
{
    [Area("Reports")]
    public class LandingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IUnitOfWork _uow;

        private readonly IDBService _dbService;

        public LandingController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment, IUnitOfWork uow, IDBService dbService)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _uow = uow;
            _dbService = dbService;
        }

        public IActionResult Index()
        {
            var applicationDbContext = _context.Reports
                .OrderBy(c => c.Order);

            return View(applicationDbContext.ToList());
        }
    }
}