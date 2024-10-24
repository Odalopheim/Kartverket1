using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using Kartverket.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace Kartverket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // definerer en liste sim en in-memory lagring
        private static List<PositionModel> positions = new List<PositionModel>();
        private static List<AreaChange> changes = new List<AreaChange>();
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
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
        public IActionResult RegisterAreaChange(string geoJson,string description, string category, string customCategory, IFormFile fileUpload)
        {
            var finalCategory = category == "Custom" ? customCategory : category;
            var newChange = new AreaChange
            {
                Id = Guid.NewGuid().ToString(),
                GeoJson = geoJson,
                Description = description,
                Category = finalCategory,
                Vedlegg = new List<Vedlegg>()
            };

            if(fileUpload != null && fileUpload.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    fileUpload.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    var vedlegg = new Vedlegg
                    {
                        FilNavn = fileUpload.FileName,
                        FilData = fileBytes
                    };
                    newChange.Vedlegg.Add(vedlegg);
                }
            }

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
