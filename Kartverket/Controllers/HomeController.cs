using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Kartverket.Services;
using Kartverket.Data;
using Microsoft.EntityFrameworkCore;
using Kartverket.Models;


namespace Kartverket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IKommuneInfoService _kommuneInfoService;
        private readonly IStedsnavnService _stedsnavnService;
        private readonly ApplicationDbContext _context;
        private readonly GeoChangeService _geoChangeService;
        private readonly UserManager<IdentityUser>_userManager;

        // definerer en liste sim en in-memory lagring
        private static List<PositionModel> positions = new List<PositionModel>();
        private static List<GeoChange> changes = new List<GeoChange>();
        //private object _kommuneInfoService;


        public HomeController(ILogger<HomeController> logger, IKommuneInfoService kommuneInfoService, IStedsnavnService stedsnavnService, ApplicationDbContext context, GeoChangeService geoChangeService, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _kommuneInfoService = kommuneInfoService;
            _stedsnavnService = stedsnavnService; // Injiser stedsnavn-tjenesten her
            _geoChangeService = geoChangeService;
            _userManager = userManager;

        }



        // public HomeController(ILogger<HomeController> logger)
        //{
        //  _logger = logger;
        // }

        public IActionResult Index()
        {
            return View();
        }
        //hånterer søk for Komunneinfo
        [HttpPost]
        public async Task<IActionResult> KommuneInfo(string kommuneNr)
        {
            if(string.IsNullOrEmpty(kommuneNr))
            {
                ViewData["Error"] = "Skriv inn riktig kommune nunmmer.";
                return View("Index");
            }
            var kommuneInfo = await _kommuneInfoService.GetKommuneInfoAsync(kommuneNr);

            if (kommuneInfo != null)
            {
                var viewModel = new KommuneInfoViewModel
                {
                    Kommunenavn = kommuneInfo.Kommunenavn,
                    Kommunenummer = kommuneInfo.Kommunenummer,
                    Fylkesnavn = kommuneInfo.Fylkesnavn,
                    SamiskForvaltningsomrade = kommuneInfo.SamiskForvaltningsomrade
                };
                return View("KommuneInfo", viewModel);
            }
            else
            {
                ViewData["Error"] = $"ikke noe resultat på dette Kommune Mummeret '{kommuneNr}'.";
                return View("Index");
            }
        }
        //håndterer søk for stedsnavn
        [HttpPost]
        public async Task<IActionResult> Stedsnavn(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                ViewData["Error"] = "Please enter a valid place name.";
                return View("Index");
            }

            var stedsnavnResponse = await _stedsnavnService.GetStedsnavnAsync(searchTerm);
            if (stedsnavnResponse?.Navn != null && stedsnavnResponse.Navn.Any())
            {
                var viewModel = stedsnavnResponse.Navn.Select(n => new StedsnavnViewModel
                {
                    Skrivemåte = n.Skrivemåte,
                    Navneobjekttype = n.Navneobjekttype,
                    Språk = n.Språk,
                    Navnestatus = n.Navnestatus
                }).ToList();

                return View("Stedsnavn", viewModel);
            }
            else
            {
                ViewData["Error"] = $"No results found for '{searchTerm}'.";
                return View("Index");
            }
        }


        [HttpGet]
        public ViewResult RegistrationForm()
        {
            return View();
        }

        [HttpPost]
        public ViewResult RegistrationForm(UserData userData)
        {
            return View("Overview", userData);
        }


        [HttpGet]
        public IActionResult RegisterAreaChange()
        {
            return View();
        }

        [Authorize]
        [HttpPost]

        public async Task <IActionResult> RegisterAreaChange(string geoJson, string description, string category)
        {
            try
            {
                if (string.IsNullOrEmpty(geoJson) || string.IsNullOrEmpty(description))
                {
                    return BadRequest("Invalid data");
                }

                var user = await _userManager.GetUserAsync(User);
                var userId = user.Id;
                
                //Save to the database using Dapper
                _geoChangeService.AddGeoChange(description, geoJson, userId);
              

                return RedirectToAction("AreaChangeOverview");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}, Inner Exception: {ex.InnerException?.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


        [Authorize]
        [HttpGet]
        public async Task <IActionResult> AreaChangeOverview()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            var changes = _geoChangeService.GetAllGeoChanges(userId);
            return View(changes);
        }


        //New action methods for UpdateOverview feature
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UpdateOverview()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var userId = user.Id;

                var allChanges = _geoChangeService.GetAllGeoChanges(userId);
                return View(allChanges);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GeoChanges in UpdateOverview.");
                return View("Error");
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

            return View(geoChange);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(GeoChange model)
        {
            // Remove validation for UserId since it is set programmatically
            ModelState.Remove("UserId");

            // Get the current logged-in user
            var user = await _userManager.GetUserAsync(User);

            // Set the UserId programmatically
            model.UserId = user.Id;

            if (ModelState.IsValid)
            {
                _logger.LogInformation("ModelState is valid. Updating GeoChange.");

                // Proceed with updating the geo change
                _geoChangeService.UpdateGeoChange(model.Id, model.Description, model.GeoJson, user.Id);
                return RedirectToAction("UpdateOverview");
            }
            else
            {
                _logger.LogWarning("ModelState is invalid.");
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        _logger.LogWarning(error.ErrorMessage);
                    }
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
            return RedirectToAction("UpdateOverview");
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CorrectionOverview()
        {
            return View(positions);
        }


        [HttpGet]
        public ViewResult Saksbehandler()
        {
            return View();
        }
    }
}
