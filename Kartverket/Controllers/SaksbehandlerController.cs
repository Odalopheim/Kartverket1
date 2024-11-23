using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Kartverket.Services;
using Kartverket.Models;
using Microsoft.AspNetCore.Identity;
using Kartverket.Data;
using System.Linq;
using Kartverket.Data;
using Kartverket.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Kartverket.Controllers
{
    [Authorize(Roles = "Saksbehandler")]
    public class SaksbehandlerController : Controller
    {
        private readonly ILogger<SaksbehandlerController> _logger;
        private readonly GeoChangeService _geoChangeService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SaksbehandlerController(ILogger<SaksbehandlerController> logger, GeoChangeService geoChangeService, ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _geoChangeService = geoChangeService;
            _context = context;
            _userManager = userManager;
        }

        // View list of geo changes for admin
        public async Task<IActionResult> Saksbehandler()
        {
            var geoChanges = await _context.GeoChanges.ToListAsync();
            return View(geoChanges);
        }

        // View detailed geo change for admin
        [HttpGet]
        public async Task<IActionResult> GeoChangeDetails(int id)
        {
            var geoChange = await _context.GeoChanges.FirstOrDefaultAsync(g => g.Id == id);

            if (geoChange == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(geoChange.UserId);
            var model = new GeoChangeDetailsViewModel
            {
                GeoChange = geoChange,
                UserName = user?.UserName ?? "Ukjent Bruker",
                Status = geoChange.Status.ToString()
            };

            return View(model);
        }

        // Update geo change for admin
        [HttpPost]
        public async Task<IActionResult> SaveChanges(GeoChangeDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var geoChange = await _context.GeoChanges.FirstOrDefaultAsync(g => g.Id == model.GeoChange.Id);

                if (geoChange != null)
                {
                    geoChange.Status = model.GeoChange.Status;
                    geoChange.Category = model.GeoChange.Category;

                    _context.Update(geoChange);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Saksbehandler");
                }
            }

            var geoChangeData = await _context.GeoChanges.FirstOrDefaultAsync(g => g.Id == model.GeoChange.Id);
            if (geoChangeData != null)
            {
                model.GeoChange.GeoJson = geoChangeData.GeoJson;
                model.GeoChange.Description = geoChangeData.Description;
                model.UserName = await _context.Users
                    .Where(u => u.Id == geoChangeData.UserId)
                    .Select(u => u.UserName)
                    .FirstOrDefaultAsync();
            }

            return View("GeoChangeDetails", model);
        }

        // Search for geo changes for admin
        public IActionResult SearchGeoChanges(DateTime? fromDate, DateTime? toDate, GeoChangeCategory? category)
        {
            var geoChanges = _geoChangeService.SearchGeoChanges(fromDate, toDate, category);
            return View("Saksbehandler", Saksbehandler);
        }
    }

}
