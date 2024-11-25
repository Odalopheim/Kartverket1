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
    [Authorize]
    public class GeoChangeController : Controller
    {
        private readonly ILogger<GeoChangeController> _logger;
        private readonly GeoChangeService _geoChangeService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public GeoChangeController(ILogger<GeoChangeController> logger, GeoChangeService geoChangeService, UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _logger = logger;
            _geoChangeService = geoChangeService;
            _userManager = userManager;
            _context = context;
        }

        // User functions
        [HttpGet]
        public IActionResult RegisterAreaChange() => View();

        [HttpPost]
        public async Task<IActionResult> RegisterAreaChange(string geoJson, string description, GeoChangeCategory category)
        {
            try
            {
                if (string.IsNullOrEmpty(geoJson) || string.IsNullOrEmpty(description))
                    return BadRequest("Invalid data");

                var user = await _userManager.GetUserAsync(User);
                var userId = user.Id;

                var status = GeoChangeStatus.SendtInn;
                _geoChangeService.AddGeoChange(description, geoJson, userId, status, category);

                return RedirectToAction("MinSide", "Account");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}, Inner Exception: {ex.InnerException?.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // Edit geo change
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            var geoChange = _geoChangeService.GetGeoChangeById(id, userId);
            if (geoChange == null) return NotFound();

            return View(geoChange);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(GeoChange model)
        {
            if (ModelState.IsValid)
            {
                var geoChange = await _context.GeoChanges.FirstOrDefaultAsync(g => g.Id == model.Id);
                if (geoChange == null) return NotFound();

                geoChange.Description = model.Description;
                geoChange.Category = model.Category;
                geoChange.Status = model.Status;
                geoChange.GeoJson = model.GeoJson ?? geoChange.GeoJson;

                _geoChangeService.UpdateGeoChange(geoChange.Id, geoChange.Description, geoChange.GeoJson, geoChange.UserId, geoChange.Status, geoChange.Category);

                return RedirectToAction("MinSide", "Account");
            }

            return View(model);
        }

        // Delete geo change
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            var geoChange = _geoChangeService.GetGeoChangeById(id, userId);
            if (geoChange == null) return NotFound();

            return View(geoChange);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            _geoChangeService.DeleteGeoChange(id, userId);
            return RedirectToAction("MinSide", "Account");
        }

        // View geo change details
        [HttpGet]
        public IActionResult Details(int id)
        {
            var model = _context.GeoChanges.FirstOrDefault(gc => gc.Id == id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }

    }

}