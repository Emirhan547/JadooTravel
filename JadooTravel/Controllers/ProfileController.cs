using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.UserDtos;
using JadooTravel.Entity.Entities;
using JadooTravel.UI.Logging;
using JadooTravel.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUserProfileService _userProfileService;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<ProfileController> _logger;
        private readonly IElasticAuditLogger _auditLogger;
        private readonly IAwsS3Service _awsS3Service;
        public ProfileController(
            IUserProfileService userProfileService,
           UserManager<AppUser> userManager,
           ILogger<ProfileController> logger,
           IElasticAuditLogger auditLogger,
           IAwsS3Service awsS3Service)
        {
            _userProfileService = userProfileService;
            _userManager = userManager;
            _logger = logger;
            _auditLogger = auditLogger;
            _awsS3Service = awsS3Service;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Auth");

            var profile = await _userProfileService.GetProfileAsync(user.Id);
            if (profile == null)
                return RedirectToAction("Login", "Auth");
            await _auditLogger.LogAsync("profile.view", "user", user.Id, "view", "profile", profile.Id, "success");
            return View(profile);
        }

        [HttpGet]
        public async Task<IActionResult> Update()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Auth");

            var profile = await _userProfileService.GetProfileAsync(user.Id);
            if (profile == null)
                return RedirectToAction("Login", "Auth");
            var updateDto = new UpdateProfileDto
            {
                Id = profile.Id,
                FullName = profile.FullName,
                Email = profile.Email,
                PhoneNumber = profile.PhoneNumber,
                Address = profile.Address,
                City = profile.City,
                Country = profile.Country,
                ProfileImageUrl = profile.ProfileImageUrl
            };

            return View(updateDto);
        }
       
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateProfileDto model, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                if (imageFile is not null)
                    model.ProfileImageUrl = await _awsS3Service.UploadImageAsync(imageFile, "profiles");
                var result = await _userProfileService.UpdateProfileAsync(model);
                if (result)
                {
                    TempData["success"] = "Profil başarıyla güncellendi";
                    await _auditLogger.LogAsync("profile.update", "user", model.Id, "update", "profile", model.Id, "success");
                    return RedirectToAction("Index");

                }
                await _auditLogger.LogAsync("profile.update", "user", model.Id, "update", "profile", model.Id, "failed");
                ModelState.AddModelError("", "Profil güncellenirken bir hata oluştu");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Profile update failed. ProfileId: {ProfileId}", model.Id);
                await _auditLogger.LogAsync("profile.update", "user", model.Id, "update", "profile", model.Id, "error", new { ex.Message });
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Yeni şifreler eşleşmiyor");
                return View(model);
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return RedirectToAction("Login", "Auth");
                var result = await _userProfileService.ChangePasswordAsync(user.Id, model);

                if (result)
                {
                    TempData["success"] = "Şifreniz başarıyla değiştirildi";
                    await _auditLogger.LogAsync("profile.change_password", "user", user.Id, "change_password", "account", user.Id, "success");
                    return RedirectToAction("Index");
                }
                await _auditLogger.LogAsync("profile.change_password", "user", user.Id, "change_password", "account", user.Id, "failed");
                ModelState.AddModelError("", "Mevcut şifreniz yanlış");
                return View(model);
            }
            catch (Exception ex)
            {
                var user = await _userManager.GetUserAsync(User);
                _logger.LogError(ex, "ChangePassword failed. UserId: {UserId}", user?.Id);
                await _auditLogger.LogAsync("profile.change_password", "user", user?.Id, "change_password", "account", user?.Id, "error", new { ex.Message });
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

       
    }
}