using System.Diagnostics;
using Kartverket.Models;
using Microsoft.AspNetCore.Mvc;
using Kartverket.Services;
using Kartverket.Data;
using Microsoft.EntityFrameworkCore;


namespace Kartverket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IKommuneInfoService _kommuneInfoService;
        private readonly IStedsnavnService _stedsnavnService;
        private readonly ApplicationDbContext _context;

        // definerer en liste sim en in-memory lagring
        private static List<PositionModel> positions = new List<PositionModel>();
        private static List<GeoChange> changes = new List<GeoChange>();
        //private object _kommuneInfoService;


        public HomeController(ILogger<HomeController> logger, IKommuneInfoService kommuneInfoService, IStedsnavnService stedsnavnService, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
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


        // GET: RegistrationForm
        [HttpGet]
        public IActionResult RegistrationForm()
        {
            return View();
        }

        // POST: Opprett Bruker
        [HttpPost]
        public IActionResult OpprettBruker(UserData userData)
        {
            if (ModelState.IsValid)
            {
                // Her kan du implementere logikken for å opprette brukeren i databasen.
                // For eksempel, lagre userData i databasen

                // For demonstrasjon kan vi bare vise de innsatte dataene på en annen side.
                return RedirectToAction("Overview", userData);
            }

            // Hvis modellvalideringen feiler, vis skjemaet igjen med feilmeldinger
            return View("RegistrationForm", userData);
        }

        // GET: Overview
        public IActionResult Overview(UserData userData)
        {
            return View(userData);
        }

        [HttpGet]
        public IActionResult RegisterAreaChange()
        {
            return View();
        }

        [HttpPost]

        public IActionResult RegisterAreaChange(string geoJson, string description, string category, string customCategory, IFormFile fileUpload)
        {
            try
            {
                if (string.IsNullOrEmpty(geoJson) || string.IsNullOrEmpty(description))
                {
                    return BadRequest("Invalid data");
                }

                var finalCategory = category == "Custom" ? customCategory : category;
                var newGeoChange = new GeoChange
                {
                    GeoJson = geoJson,
                    Description = description,
                    Category = finalCategory,
                    Vedlegg = new List<Kartverket.Data.Vedlegg>()
                };

                if (fileUpload != null && fileUpload.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        fileUpload.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        var vedlegg = new Kartverket.Data.Vedlegg
                        {
                            FilNavn = fileUpload.FileName,
                            FilData = fileBytes
                        };
                        newGeoChange.Vedlegg.Add(vedlegg);
                    }
                }

                _context.GeoChange.Add(newGeoChange);
                _context.SaveChanges();

                return RedirectToAction("AreaChangeOverview");
            }
              catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}, Inner Exception: {ex.InnerException?.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet]
        public IActionResult AreaChangeOverview()
        {
            var changes_db = _context.GeoChange.ToList();
            return View(changes_db);
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



        [HttpGet]
        public ViewResult Saksbehandler()
        {
            return View();
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
