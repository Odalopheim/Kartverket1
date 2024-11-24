using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Kartverket.Data;
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
       
        // GET: Viser siden der Brukerene kan registrer feil i kart
        [HttpGet]
        public IActionResult RegisterAreaChange() => View();

        //POST: Lar Brukerene sende inn feil for så å komme tilbake til MinSide 
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        // GET: Viser Inmelding på siden for å endre 
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            var geoChange = _geoChangeService.GetGeoChangeById(id, userId);
            if (geoChange == null) return NotFound();

            return View(geoChange);
        }

        //POST: Lar Brukeren endre innemdlingene sine for så å bli omdirigert til MinSide
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(GeoChange model)
        {
            ModelState.Remove("UserId");
            if (ModelState.IsValid)
            {
                var geoChange = await _context.GeoChanges.FirstOrDefaultAsync(g => g.Id == model.Id);

                if (geoChange == null)
                {
                    _logger.LogWarning("Geochange not found");
                    return NotFound();
                }

                _logger.LogInformation("Updating GeoChang with Id: " + model.Id);

                geoChange.Description = model.Description;
                geoChange.Category = model.Category;
                geoChange.Status = model.Status;
                geoChange.GeoJson = model.GeoJson ?? geoChange.GeoJson;

                _geoChangeService.UpdateGeoChange(
                    geoChange.Id, 
                    geoChange.Description, 
                    geoChange.GeoJson, 
                    geoChange.UserId, 
                    geoChange.Status, 
                    geoChange.Category
                    );

                _logger.LogInformation("GeoChange updated succesfully");
                return RedirectToAction("MinSide", "Account");
            }

            else
            {
                _logger.LogWarning("ModelState is invalid");

                foreach(var state in ModelState)
                {
                    _logger.LogWarning($"{state.Key}: {string.Join(", ",state.Value.Errors.Select(e => e.ErrorMessage))}");
                }
            }

            return View(model);
        }

        // GET: Får opp slette innmedlinger siden for brukeren
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            var geoChange = _geoChangeService.GetGeoChangeById(id, userId);
            if (geoChange == null) return NotFound();

            return View(geoChange);
        }

        //POST: Lar brukeren slette innemldingen sin. For så å bli omdirigert til MinSide
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            _geoChangeService.DeleteGeoChange(id, userId);
            return RedirectToAction("MinSide", "Account");
        }

        // GET: Lar brukeren se innmelingene sine
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