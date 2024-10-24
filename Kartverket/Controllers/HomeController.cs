using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using Kartverket.Models;
using Microsoft.AspNetCore.Mvc;
using Kartverket.Services;

namespace Kartverket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IKommuneInfoService _kommuneInfoService;
        private readonly IStedsnavnService _stedsnavnService;

        // definerer en liste sim en in-memory lagring
        private static List<PositionModel> positions = new List<PositionModel>();
        private static List<AreaChange> changes = new List<AreaChange>();
        //private object _kommuneInfoService;


        public HomeController(ILogger<HomeController> logger, IKommuneInfoService kommuneInfoService, IStedsnavnService stedsnavnService)
        {
            _logger = logger;
            _kommuneInfoService = kommuneInfoService;
            _stedsnavnService = stedsnavnService; // Injiser stedsnavn-tjenesten her
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
        public async Task<IActionResult> KommuneInfo(string kommunenummer)
        {
            if(string.IsNullOrEmpty(kommunenummer))
            {
                ViewData["Error"] = "Skriv inn riktig kommune nunmmer.";
                return View("Index");
            }
            var kommuneInfo = await _kommuneInfoService.GetKommuneInfoAsync(kommunenummer);

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
                ViewData["Error"] = $"ikke noe resultat på dette Kommune Mummeret '{kommunenummer}'.";
                return View("Index");
            }
        }
        //håndterer søk for stedsnavn
        [HttpPost]
        public async Task<IActionResult> Stedsnavn(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                ViewData["Error"] = "Please skriv in riktig stedsnavn.";
                return View("Index");
            }

            var stedsnavnResponse = await _stedsnavnService.GetStedsnavnAsync(searchTerm); // Bruk tjenesten her
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
                ViewData["Error"] = $"ikke noe resultat på '{searchTerm}'.";
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

        [HttpPost]
        public IActionResult RegisterAreaChange(string geoJson,string description, string category, string customCategory)
        {
            var finalCategory = category == "Custom" ? customCategory : category;
            var newChange = new AreaChange
            {
                Id = Guid.NewGuid().ToString(),
                GeoJson = geoJson,
                Description = description,
                Category = finalCategory
            };

            changes.Add(newChange);
            return RedirectToAction("AreaChangeOverview");
        }
        [HttpGet]
        public IActionResult AreaChangeOverview()
        {
            return View(changes);
        }
   

        [HttpGet]
        public IActionResult CorrectMap()
        {
           return View();
        }

        [HttpPost]
        public IActionResult CorrectMap(PositionModel model)
        {
            if (ModelState.IsValid)
            {
                positions.Add(model);
                return View("CorrectionOverview", positions);
            }
            return View();
        }

        [HttpGet]
        public IActionResult CorrectionOverview()
        {
            return View(positions);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
