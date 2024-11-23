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


        //søkefunskjon for saksbehandlere
        public IActionResult SearchGeoChanges(DateTime? fromDate, DateTime? toDate, GeoChangeCategory? category)
        {
            var geoChanges = _geoChangeService.SearchGeoChanges(fromDate, toDate, category);
            return View("Saksbehandler");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var geoChange = await _context.GeoChanges.FindAsync(id);
            if (geoChange == null)
            {

                return NotFound();
            }
            return View(geoChange);
        }

        [HttpPost]
        public async Task<IActionResult> Update(GeoChange model)
        {
            ModelState.Remove("UserId");
            if (ModelState.IsValid)
            {
                var geoChange = await _context.GeoChanges.FirstOrDefaultAsync(g => g.Id == model.Id);

                if (geoChange == null)
                {
                    _logger.LogWarning("GeoChange not found");
                    return NotFound();
                }

                _logger.LogInformation("Updating geochanges with ID: " + model.Id);

                geoChange.Category = model.Category;
                geoChange.Status = model.Status;
                geoChange.Description = model.Description;
                geoChange.GeoJson = model.GeoJson ?? geoChange.GeoJson;
                geoChange.Id = model.Id;

                // Kall UpdateGeoChange i tjenesten
                _geoChangeService.UpdateGeoChangeAdmin(

                    geoChange.Id,
                    geoChange.Status,
                    geoChange.Category
                );

                _logger.LogInformation("GeoChange updated successfully");
                return RedirectToAction("Saksbehandler");
            }

            else
            {
                _logger.LogWarning("ModelState is invalid");
                foreach (var state in ModelState)
                {
                    _logger.LogWarning($"{state.Key}: {string.Join(", ",state.Value.Errors.Select(e => e.ErrorMessage))}");
                }
            }
            
            return View(model);
        }

    }

}
