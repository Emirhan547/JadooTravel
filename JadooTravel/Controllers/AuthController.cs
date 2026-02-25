using JadooTravel.Dto.Dtos.AuthDtos;
using JadooTravel.Entity.Entities;
using JadooTravel.UI.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace JadooTravel.UI.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<AuthController> _logger;
        private readonly IElasticAuditLogger _auditLogger;
        public AuthController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILogger<AuthController> logger,
            IElasticAuditLogger auditLogger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _auditLogger = auditLogger;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("User login rejected due to invalid model state. Email: {Email}", model.Email);
                await _auditLogger.LogAsync("auth.login", "user", null, "login", "account", null, "validation_failed", new { model.Email });
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.LogWarning("Failed login attempt. Email not found: {Email}", model.Email);
                await _auditLogger.LogAsync("auth.login", "user", null, "login", "account", null, "failed", new { model.Email, reason = "email_not_found" });
                ModelState.AddModelError(string.Empty, "Email veya şifre hatalı");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false
            );

            if (!result.Succeeded)
            {
                _logger.LogWarning("Failed login attempt due to invalid password. UserId: {UserId}, Email: {Email}", user.Id, user.Email);
                await _auditLogger.LogAsync("auth.login", "user", user.Id, "login", "account", user.Id, "failed", new { user.Email, reason = "invalid_password" });
                ModelState.AddModelError(string.Empty, "Email veya şifre hatalı");
                return View(model);
            }


            var isAdmin = user.Roles != null && user.Roles.Contains("Admin");
            _logger.LogInformation("User login successful. UserId: {UserId}, Email: {Email}, IsAdmin: {IsAdmin}", user.Id, user.Email, isAdmin);
            await _auditLogger.LogAsync("auth.login", isAdmin ? "admin" : "user", user.Id, "login", "account", user.Id, "success", new { user.Email, isAdmin });

            if (isAdmin)
            {
                return RedirectToAction("AdminDashboard", "Dashboard", new { area = "Admin" });
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Default");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new AppUser
            {
                Id = ObjectId.GenerateNewId().ToString(),
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName,
          

               
                Roles = new List<string> { "User" }
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                _logger.LogWarning("User registration failed. Email: {Email}", model.Email);
                await _auditLogger.LogAsync("auth.register", "user", null, "register", "account", null, "failed", new { model.Email });

                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("User registration and auto login successful. UserId: {UserId}, Email: {Email}", user.Id, user.Email);
            await _auditLogger.LogAsync("auth.register", "user", user.Id, "register", "account", user.Id, "success", new { user.Email });

            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var userId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logout successful. UserId: {UserId}", userId);
            await _auditLogger.LogAsync("auth.logout", isAdmin ? "admin" : "user", userId, "logout", "account", userId, "success");
            return RedirectToAction("Index", "Default");
        }
    }
}
