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

        // GET: Register Geo Change
        [HttpGet]
        public IActionResult RegisterAreaChange()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RegisterAreaChange(string geoJson, string description, GeoChangeCategory category)
        {
            try
            {
                if (string.IsNullOrEmpty(geoJson) || string.IsNullOrEmpty(description))
                {
                    return BadRequest("Invalid data");
                }

                var user = await _userManager.GetUserAsync(User);
                var userId = user.Id;

                // Sett standardverdier for status 
                var status = GeoChangeStatus.SendtInn;


                // Save to the database using Dapper
                _geoChangeService.AddGeoChange(description, geoJson, userId, status, category);

                return RedirectToAction("MinSide", "Account");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}, Inner Exception: {ex.InnerException?.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation($"Edit GET action called with id={id}");

            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            var geoChange = _geoChangeService.GetGeoChangeById(id, userId);
            if (geoChange == null)
            {
                _logger.LogWarning($"GeoChange with id={id} not found for userId={userId}");
                return NotFound();
            }

            // Logg GeoJson-dataen
            if (string.IsNullOrWhiteSpace(geoChange.GeoJson))
            {
                _logger.LogWarning("GeoJson-data is missing.");
            }
            else
            {
                _logger.LogInformation($"GeoJson data: {geoChange.GeoJson}");
            }

            return View(geoChange);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(GeoChange model)
        {
            // Remove validation for UserId since it can be null
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                var geoChange = await _context.GeoChanges.FirstOrDefaultAsync(g => g.Id == model.Id);

                if (geoChange == null)
                {
                    _logger.LogWarning("GeoChange not found");
                    return NotFound();
                }

                _logger.LogInformation("Updating GeoChange with Id: " + model.Id);

                // Oppdater geoChange med nye verdier
                geoChange.Description = model.Description;
                geoChange.Category = model.Category;
                geoChange.Status = model.Status;

                // Behold den eksisterende GeoJson-dataen
                geoChange.GeoJson = model.GeoJson ?? geoChange.GeoJson;


                // Kall UpdateGeoChange i tjenesten
                _geoChangeService.UpdateGeoChange(
                    geoChange.Id,
                    geoChange.Description,
                    geoChange.GeoJson,
                    geoChange.UserId,
                    geoChange.Status,
                    geoChange.Category
                );

                _logger.LogInformation("GeoChange updated successfully");

                return RedirectToAction("MinSide", "Account");
            }
            else
            {
                _logger.LogWarning("ModelState is invalid");
                foreach (var state in ModelState)
                {
                    _logger.LogWarning($"{state.Key}: {string.Join(", ", state.Value.Errors.Select(e => e.ErrorMessage))}");
                }
            }

            return View(model);
        }



        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            var geoChange = _geoChangeService.GetGeoChangeById(id, userId);
            if (geoChange == null)
            {
                return NotFound();
            }
            return View(geoChange);
        }

        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            _geoChangeService.DeleteGeoChange(id, userId);
            return RedirectToAction("MinSide", "Account");
        }

        //Får opp innmeldingen når du trykker på den 
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

        public async Task<IActionResult> Saksbehandler()
        {
            var geoChanges = await _context.GeoChanges.ToListAsync();
            return View(geoChanges);
        }

        [HttpGet]
        public async Task<IActionResult> GeoChangeDetails(int id)
        {
            var geoChange = await _context.GeoChanges.FirstOrDefaultAsync(g => g.Id == id);

            if (geoChange == null)
            {
                return NotFound();
            }

            // Hent brukerdata basert på UserId
            var user = await _userManager.FindByIdAsync(geoChange.UserId);

            // Pakk inn dataen i en ViewModel for visningen
            var model = new GeoChangeDetailsViewModel
            {
                GeoChange = geoChange,
                UserName = user?.UserName ?? "Ukjent Bruker",
                // Legg til statusen for å vise i visningen
                Status = geoChange.Status.ToString()  // Viktig å sende statusen til visningen
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> SaveChanges(GeoChangeDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Hent eksisterende GeoChange
                var geoChange = await _context.GeoChanges.FirstOrDefaultAsync(g => g.Id == model.GeoChange.Id);

                if (geoChange != null)
                {
                    // Oppdater status og kategori
                    geoChange.Status = model.GeoChange.Status;  // Pass på at statusen blir oppdatert
                    geoChange.Category = model.GeoChange.Category;

                    // Oppdater databasen
                    _context.Update(geoChange);
                    await _context.SaveChangesAsync();

                    // Gå tilbake til oversikten
                    return RedirectToAction("Saksbehandler");
                }
            }

            // Hvis ModelState ikke er gyldig eller noe feilet, last inn dataene på nytt
            var geoChangeData = await _context.GeoChanges
                .FirstOrDefaultAsync(g => g.Id == model.GeoChange.Id);

            if (geoChangeData != null)
            {
                // Fyll inn data som mangler
                model.GeoChange.GeoJson = geoChangeData.GeoJson;
                model.GeoChange.Description = geoChangeData.Description;
                model.UserName = await _context.Users
                    .Where(u => u.Id == geoChangeData.UserId)
                    .Select(u => u.UserName)
                    .FirstOrDefaultAsync();
            }
            else
            {
                // Legg til en feilmelding hvis GeoChange ikke finnes
                ModelState.AddModelError("", "Kunne ikke finne endringen i databasen.");
            }

            // Returner visningen med oppdatert modell
            return View("GeoChangeDetails", model);
        }



        //søkefunskjon for saksbehandlere
        public IActionResult SearchGeoChanges(DateTime? fromDate, DateTime? toDate, GeoChangeCategory? category)
        {
            var geoChanges = _geoChangeService.SearchGeoChanges(fromDate, toDate, category);
            return View("Saksbehandler", geoChanges);
        }
    }
}