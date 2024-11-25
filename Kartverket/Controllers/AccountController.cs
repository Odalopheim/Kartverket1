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
        private readonly UserService _userService;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<AccountController> logger, ApplicationDbContext context, UserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
            _userService = userService;
        }

        // GET: Loginn side
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Lar brukeren logge inn der den ser også på hvilken rolle brukeren har for å denne han/hun til riktig plass
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
                        userName: user.Email,
                        password: model.Password,
                        isPersistent: false,
                        lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"Login succeeded for user {user.Email}");

                        // Sjekk om brukeren er admin
                        if (await _userManager.IsInRoleAsync(user, "Admin"))
                        {
                            _logger.LogInformation($"User {user.Email} is an Admin.");
                            return RedirectToAction("AdminHjemmeside", "Admin");
                        }

                        // Sjekk om brukerens e-post har @kartverket.no (saksbehandler)
                        if (user.Email.EndsWith("@kartverket.no", StringComparison.OrdinalIgnoreCase))
                        {
                            _logger.LogInformation($"User {user.Email} is from kartverket.no (Saksbehandler).");
                            return RedirectToAction("Saksbehandler", "Saksbehandler");
                        }

                        // Sender brukeren til deres MinSide
                        _logger.LogInformation($"User {user.Email} is not a Saksbehandler.");
                        return RedirectToAction("MinSide", "Account");
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
            return View("Login", model);
        }


        // POST: Logger ut 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        // GET: Registrer
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Registrere bruker
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrerViewModel model)
        {
            if (ModelState.IsValid)
            {

                var user = new IdentityUser { UserName = model.Email, Email = model.Email };


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

                    // Assign the user to a default role
                    var roleResult = await _userManager.AddToRoleAsync(user, "Bruker");

                    if (!roleResult.Succeeded)
                    {
                        foreach (var error in roleResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }

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

        [Authorize]
        [HttpGet]
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

        // GET: Siden med at registreringen var vellykket 
        [HttpGet]
        public IActionResult RegistrationSuccess()
        {
            return View();
        }

        // GET: Viser brukerinformasjon for endring
        [HttpGet]
        public async Task<IActionResult> EditUserInfo()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            var userDetails = await _context.UserDetails.FirstOrDefaultAsync(u => u.UserId == user.Id);
            if (userDetails == null)
            {
                userDetails = new UserDetails { UserId = user.Id, User = user };
                _context.UserDetails.Add(userDetails);
                await _context.SaveChangesAsync();
            }
            else
            {
                userDetails.User = user;
            }
            return View(userDetails);
        }

        // POST: Lar brukeren oppdatere sin informasjon
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUserInfo(UserDetails model)
        {
            ModelState.Remove("UserId");
            ModelState.Remove("User");


            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                var userDetails = await _context.UserDetails.FirstOrDefaultAsync(u => u.UserId == model.UserId);
                if (userDetails == null)
                {
                    _logger.LogWarning("User details not found");
                    return NotFound();
                }

                userDetails.Name = model.Name;
                userDetails.Address = model.Address;
                userDetails.PostNumber = model.PostNumber;
                userDetails.User = user;

                _userService.UpdateUserDetails(
                   userDetails.UserId,
                   userDetails.Name,
                   userDetails.Address,
                   userDetails.PostNumber
                );
                _logger.LogInformation("User details updated successfully");

                return RedirectToAction("MinSide", "Account");
            }
            else
            {
                _logger.LogWarning("ModelState is invalid");
                foreach (var state in ModelState)
                {
                    _logger.LogWarning($"{state.Key}: {string.Join(", ", state.Value.Errors.Select(e => e.ErrorMessage))}");
                    foreach (var error in state.Value.Errors)
                    {
                        _logger.LogWarning($"Error: {error.ErrorMessage}");
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    // Logg ut brukeren etter sletting
                    await _signInManager.SignOutAsync();

                    TempData["SuccessMessage"] = "Profilen din er slettet, og du er logget ut.";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Feil ved sletting
                    TempData["ErrorMessage"] = "Noe gikk galt ved sletting av profilen din.";
                    return RedirectToAction("MinSide");
                }
            }

            TempData["ErrorMessage"] = "Brukeren finnes ikke.";
            return RedirectToAction("MinSide");
        }

    }
}



