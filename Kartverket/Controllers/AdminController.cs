using Kartverket.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;

    public AdminController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> AdminHjemmeside()
    {
        var saksbehandlere = await _userManager.GetUsersInRoleAsync("Saksbehandler");
        return View(saksbehandlere);
    }

    [HttpGet]
    public IActionResult CreateSaksbehandler()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSaksbehandler(CreateSaksbehandlerViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new IdentityUser { UserName = model.Email, Email = model.Email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Saksbehandler");
                return RedirectToAction("AdminHjemmeside");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSaksbehandler(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            TempData["ErrorMessage"] = "Ugyldig bruker-ID.";
            return RedirectToAction("AdminHjemmeside");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            TempData["ErrorMessage"] = "Saksbehandler ikke funnet.";
            return RedirectToAction("AdminHjemmeside");
        }

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = $"Saksbehandler {user.Email} ble slettet.";
        }
        else
        {
            TempData["ErrorMessage"] = "Kunne ikke slette saksbehandler.";
        }

        return RedirectToAction("AdminHjemmeside");
    }


    [HttpGet]
    public async Task<IActionResult> ConfirmDelete(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            TempData["ErrorMessage"] = "Ingen bruker valgt for sletting.";
            return RedirectToAction("AdminHjemmeside");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            TempData["ErrorMessage"] = "Saksbehandler ikke funnet.";
            return RedirectToAction("AdminHjemmeside");
        }

        var model = new CreateSaksbehandlerViewModel
        {
            Email = user.Email
        };

        ViewBag.UserId = user.Id;
        return View("DeleteSaksbehandler", model); // Bruker Delete-visningen
    }

}