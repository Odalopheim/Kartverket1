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
using Kartverket.ViewModels;
using NuGet.Packaging.Rules;



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

                // Sett standardverdier for status og saksbehandler
                var status = GeoChangeStatus.SendtInn;
                var saksbehandler = string.Empty;

                // Save to the database using Dapper
                _geoChangeService.AddGeoChange(description, geoJson, userId, status, category, saksbehandler);

                return RedirectToAction("MinSide");
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
        public async Task<IActionResult> MinSide()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user==null)
            {
                return NotFound("Bruker ikke funnet");
            }
            var userDetails = await _context.UserDetails.FirstOrDefaultAsync(u=> u.UserId == user.Id);
            var geoChanges = await _context.GeoChanges
                .Where(g => g.UserId == user.Id)
                .ToListAsync();
            var model = new MinSideViewModel
            {
                RegistrerViewModel = new RegistrerViewModel
                {
                    Name = userDetails?.Name,
                    Email = user.Email,
                    Address = userDetails?.Address,
                    PostNumber = userDetails?.PostNumber

                },
                GeoChange = geoChanges
            };
            return View(model);
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
                _geoChangeService.UpdateGeoChange(
                    model.Id,
                    model.Description,
                    model.GeoJson,
                    user.Id,
                    model.Status,
                    model.Category,
                    model.Saksbehandler
                );

                return RedirectToAction("MinSide");
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

        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            _geoChangeService.DeleteGeoChange(id, userId);
            return RedirectToAction("MinSide");
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
                UserName = user?.UserName ?? "Ukjent Bruker"
            };

            return View(model);
        }

      
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
                }

                return RedirectToAction("Saksbehandler");
            }

            return View("GeoChangeDetails", model);
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


    }
}
