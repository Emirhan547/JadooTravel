using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.UserDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using JadooTravel.Entity.Entities;

namespace JadooTravel.UI.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUserProfileService _userProfileService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IBookingService _bookingService;

        public ProfileController(
            IUserProfileService userProfileService,
            UserManager<AppUser> userManager,
            IBookingService bookingService)
        {
            _userProfileService = userProfileService;
            _userManager = userManager;
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Auth");

            var profile = await _userProfileService.GetProfileAsync(user.Id);
            return View(profile);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Auth");

            var profile = await _userProfileService.GetProfileAsync(user.Id);

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
        public async Task<IActionResult> Edit(UpdateProfileDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var result = await _userProfileService.UpdateProfileAsync(model);
                if (result)
                {
                    TempData["success"] = "Profil başarıyla güncellendi";
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("", "Profil güncellenirken bir hata oluştu");
                return View(model);
            }
            catch (Exception ex)
            {
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
                var result = await _userProfileService.ChangePasswordAsync(user.Id, model);

                if (result)
                {
                    TempData["success"] = "Şifreniz başarıyla değiştirildi";
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("", "Mevcut şifreniz yanlış");
                return View(model);
            }
            catch (Exception ex)
            {
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

                if (result)
                    return Json(new { success = true, message = "Favorilere eklendi" });
                else
                    return Json(new { success = false, message = "Bu ürün zaten favorilerde" });
            }
            catch (Exception ex)
            {
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

                if (result)
                    return Json(new { success = true, message = "Favorilerden kaldırıldı" });
                else
                    return Json(new { success = false, message = "Favori silinemedi" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}