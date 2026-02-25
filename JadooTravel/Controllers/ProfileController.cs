using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.UserDtos;
using JadooTravel.Entity.Entities;
using JadooTravel.UI.Logging;
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

        public ProfileController(
            IUserProfileService userProfileService,
           UserManager<AppUser> userManager,
           ILogger<ProfileController> logger,
           IElasticAuditLogger auditLogger)
        {
            _userProfileService = userProfileService;
            _userManager = userManager;
            _logger = logger;
            _auditLogger = auditLogger;
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
        public async Task<IActionResult> ToggleFavorite(string destinationId, string cityCountry, string imageUrl, decimal price)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "Lütfen giriş yapınız" });

            try
            {
                var existedBefore = (await _userProfileService.GetFavoritesAsync(user.Id)).Any(x => x.DestinationId == destinationId);

                    

                var result = await _userProfileService.ToggleFavoriteAsync(user.Id, destinationId, cityCountry, imageUrl, price);

                if (!result)
                {
                    await _auditLogger.LogAsync("profile.favorite.toggle", "user", user.Id, "toggle_favorite", "destination", destinationId, "failed");
                    return Json(new { success = false, message = "Favori işlemi başarısız" });
                }
                await _auditLogger.LogAsync("profile.favorite.toggle", "user", user.Id, "toggle_favorite", "destination", destinationId, "success", new { added = !existedBefore });
                return Json(new { success = true, isFavorite = !existedBefore });
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ToggleFavorite failed. UserId: {UserId}, DestinationId: {DestinationId}", user.Id, destinationId);
                await _auditLogger.LogAsync("profile.favorite.toggle", "user", user.Id, "toggle_favorite", "destination", destinationId, "error", new { ex.Message });
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateProfileDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
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

        [HttpGet]
        public async Task<IActionResult> MyFavorites()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Auth");

            var favorites = await _userProfileService.GetFavoritesAsync(user.Id);
            await _auditLogger.LogAsync("profile.favorite.list", "user", user.Id, "list", "favorite", null, "success", new { count = favorites.Count });
            return View(favorites);
        }

        [HttpPost]
        public async Task<IActionResult> AddFavorite(string destinationId, string cityCountry, string imageUrl, decimal price)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "Lütfen giriş yapınız" });

            try
            {
                var result = await _userProfileService.AddFavoriteAsync(user.Id, destinationId, cityCountry, imageUrl, price);
                await _auditLogger.LogAsync("profile.favorite.add", "user", user.Id, "add_favorite", "destination", destinationId, result ? "success" : "failed");
                if (result)
                    return Json(new { success = true, message = "Favorilere eklendi" });
                return Json(new { success = false, message = "Bu ürün zaten favorilerde" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddFavorite failed. UserId: {UserId}, DestinationId: {DestinationId}", user.Id, destinationId);
                await _auditLogger.LogAsync("profile.favorite.add", "user", user.Id, "add_favorite", "destination", destinationId, "error", new { ex.Message });
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFavorite(string favoriteId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "Lütfen giriş yapınız" });

            try
            {
                var result = await _userProfileService.RemoveFavoriteAsync(user.Id, favoriteId);
                await _auditLogger.LogAsync("profile.favorite.remove", "user", user.Id, "remove_favorite", "favorite", favoriteId, result ? "success" : "failed");
                if (result)
                    return Json(new { success = true, message = "Favorilerden kaldırıldı" });
                return Json(new { success = false, message = "Favori silinemedi" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RemoveFavorite failed. UserId: {UserId}, FavoriteId: {FavoriteId}", user.Id, favoriteId);
                await _auditLogger.LogAsync("profile.favorite.remove", "user", user.Id, "remove_favorite", "favorite", favoriteId, "error", new { ex.Message });
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}