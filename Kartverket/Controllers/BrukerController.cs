using Microsoft.AspNetCore.Mvc;

namespace Kartverket.Controllers
{
    public class BrukerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
