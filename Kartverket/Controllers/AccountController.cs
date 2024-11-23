using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Kartverket.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Kartverket.Models;
using Microsoft.Extensions.Logging;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Kartverket.Data;
using Microsoft.EntityFrameworkCore;

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
                    _logger.LogInformation($"User found: {user.Email}, NormalizedEmail: {user.NormalizedEmail}");

                    var result = await _signInManager.PasswordSignInAsync(
                        userName: user.Email, // Bruk e-post som brukernavn
                        password: model.Password,
                        isPersistent: false,
                        lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"Login succeeded for user {user.Email}");

                        // Sjekk om brukerens e-post har @kartverket.no (saksbehandler)
                        if (user.Email.EndsWith("@kartverket.no", StringComparison.OrdinalIgnoreCase))
                        {
                            _logger.LogInformation($"User {user.Email} is from kartverket.no (Saksbehandler).");
                            return RedirectToAction("Saksbehandler", "Saksbehandler");  // Omdirigering til saksbehandler-dashboard
                        }

                        // Hvis ikke, kan du omdirigere til en generell bruker-skjerm
                        _logger.LogInformation($"User {user.Email} is not a Saksbehandler.");
                        return RedirectToAction("MinSide", "Account");  // Generell bruker-side
                    }
                    else
                    {
                        _logger.LogWarning($"Login failed for user {user.Email}");
                        ModelState.AddModelError("", "Invalid login attempt.");
                    }
                }
                else
                {
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
                // Create a new IdentityUser with the email as username
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };

                // Create the user with the provided password
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Add additional user details (this assumes you have a UserDetails model and the relevant DbContext)
                    var userDetails = new UserDetails
                    {
                        UserId = user.Id,
                        Name = model.Name,
                        Address = model.Address,
                        PostNumber = model.PostNumber
                    };

                    _context.UserDetails.Add(userDetails);
                    await _context.SaveChangesAsync();

                    // Assign the user to a default role
                    var roleResult = await _userManager.AddToRoleAsync(user, "Bruker"); // or "Saksbehandler", depending on your logic

                    if (!roleResult.Succeeded)
                    {
                        // Log an error if the role assignment fails
                        foreach (var error in roleResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }

                    // Sign in the user after registration
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Redirect to a success page
                    return RedirectToAction("RegistrationSuccess");
                }
                else
                {
                    // If user creation fails, add the errors to the ModelState
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            // If ModelState is invalid, return the view with error messages
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> MinSide()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Bruker ikke funnet");
            }
            var userDetails = await _context.UserDetails.FirstOrDefaultAsync(u => u.UserId == user.Id);
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

        // GET: /Account/RegistrationSuccess
        public IActionResult RegistrationSuccess()
        {
            return View();
        }
    }
}
