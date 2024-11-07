using Kartverk.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kartverk.Mvc.Controllers;

[Authorize] // Requires authentication (any role will do)
public class MapCorrectionsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [Authorize(Roles = "User")] // Requires the user role for the authenticated user
    [HttpPost]
    public IActionResult Save(MapCorrectionModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }
        return View("Index", model);
    }

    [Authorize(Roles = "Administrator")] // Requires the user role for the authenticated user
    [HttpDelete]
    public IActionResult Delete(long id)
    {

        return Index();
    }
}