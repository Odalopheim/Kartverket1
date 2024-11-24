using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Kartverket.Services;
using Kartverket.Models;
using Microsoft.AspNetCore.Identity;

namespace Kartverket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IKommuneInfoService _kommuneInfoService;
        private readonly IStedsnavnService _stedsnavnService;

        public HomeController(ILogger<HomeController> logger, IKommuneInfoService kommuneInfoService, IStedsnavnService stedsnavnService)
        {
            _logger = logger;
            _kommuneInfoService = kommuneInfoService;
            _stedsnavnService = stedsnavnService;
        }

        // GET: Hovedsiden for brukere
        public IActionResult Index()
        {
            return View();
        }

        // POST: Søker opp kommuneInfo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KommuneInfo(string kommuneNr)
        {
            if (string.IsNullOrEmpty(kommuneNr))
            {
                ViewData["Error"] = "Please enter a valid municipality number.";
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

            ViewData["Error"] = $"No result found for municipality number '{kommuneNr}'.";
            return View("Index");
        }

        // POST: Søker opp stedsnavn
        [HttpPost]
        [ValidateAntiForgeryToken]
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

            ViewData["Error"] = $"No results found for '{searchTerm}'.";
            return View("Index");
        }

        // Privacy
        public IActionResult Privacy()
        {
            return View();
        }

        // Error Handling
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
