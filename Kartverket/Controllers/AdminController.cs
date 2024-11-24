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

    public async Task<IActionResult> AdminHjemmeside()
    {
        var saksbehandlere = await _userManager.GetUsersInRoleAsync("Saksbehandler");
        return View(saksbehandlere);
    }

    public IActionResult CreateSaksbehandler()
    {
        return View();
    }

    [HttpPost]
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
}