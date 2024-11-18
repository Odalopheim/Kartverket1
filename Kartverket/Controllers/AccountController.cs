using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Kartverket.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Kartverket.Models;
using Microsoft.Extensions.Logging;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Kartverket.Data;

namespace Kartverket.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<AccountController> logger, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    // Logg informasjon om brukeren som ble funnet
                    _logger.LogInformation($"User found: {user.Email}, NormalizedEmail: {user.NormalizedEmail}");

                    var result = await _signInManager.PasswordSignInAsync(
                        userName: user.Email, // Bruk e-post som brukernavn
                        password: model.Password,
                        isPersistent: false,
                        lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"Login succeeded for user {user.Email}");
                        return RedirectToAction("MinSide", "Home");
                    }
                    else
                    {
                        // Logg at innlogging mislyktes
                        _logger.LogWarning($"Login failed for user {user.Email}");
                        ModelState.AddModelError("", "Invalid login attempt.");
                    }
                }
                else
                {
                    // Logg at brukeren ikke ble funnet
                    _logger.LogWarning($"User with email {model.Email} not found.");
                    ModelState.AddModelError("", "Invalid login attempt.");
                }
            }
            return View(model);
        }


        

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email }; // Bruk e-post som brukernavn
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var userDetails = new UserDetails
                    {
                        UserId = user.Id,
                        Name = model.Name,
                        Address = model.Address,
                        PostNumber = model.PostNumber
                    };

                    _context.UserDetails.Add(userDetails);
                    await _context.SaveChangesAsync();

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("RegistrationSuccess");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }

        // GET: /Account/RegistrationSuccess
        public IActionResult RegistrationSuccess()
        {
            return View();
        }
    }
}
