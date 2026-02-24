using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.BookingDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using JadooTravel.Entity.Entities;

namespace JadooTravel.UI.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IDestinationService _destinationService;
        private readonly IUserProfileService _userProfileService;
        public BookingController(
             IDestinationService destinationService,
            IBookingService bookingService,
            UserManager<AppUser> userManager,
            IUserProfileService userProfileService)
        {
            _bookingService = bookingService;
            _destinationService = destinationService;
            _userManager = userManager;
            _userProfileService = userProfileService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking(CreateBookingDto createBookingDto)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return RedirectToAction("Login", "Auth");

                var destination = await _destinationService.GetByIdAsync(createBookingDto.DestinationId);
                if (destination == null)
                {
                    TempData["error"] = "Seçilen destinasyon bulunamadı.";
                    return RedirectToAction("Index", "Default");
                }

                createBookingDto.UserId = user.Id;
                createBookingDto.DestinationCityCountry = destination.CityCountry;
                createBookingDto.DestinationImageUrl = destination.ImageUrl;
                await _bookingService.CreateAsync(createBookingDto);

                TempData["success"] = "Rezervasyonunuz başarıyla oluşturuldu. Kısa sürede size email gönderilecektir.";
                return RedirectToAction("MyBookings");
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Hata: {ex.Message}";
                return RedirectToAction("Index", "Default");
            }
        }

        [HttpGet]
        public async Task<IActionResult> MyBookings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Auth");

            try
            {
                // Kullanıcıya ait rezervasyonları getir
                var bookings = await _bookingService.GetUserBookingsAsync(user.Id);
                var favorites = await _userProfileService.GetFavoritesAsync(user.Id);
                var favoriteDestinationIds = favorites.Select(x => x.DestinationId).ToHashSet();

                foreach (var booking in bookings)
                    booking.IsFavorite = favoriteDestinationIds.Contains(booking.DestinationId);
                return View(bookings);
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Hata: {ex.Message}";
                return View(new List<UserBookingDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> BookingDetails(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Auth");

            try
            {
                var booking = await _bookingService.GetBookingDetailsAsync(id);

                // Kendi rezervasyonlarını sadece görüntüleyebilsin
                if (booking.UserId != user.Id && !User.IsInRole("Admin"))
                    return Forbid();

                return View(booking);
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Hata: {ex.Message}";
                return RedirectToAction("MyBookings");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBooking(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "Lütfen giriş yapınız" });

            try
            {
                var result = await _bookingService.CancelBookingAsync(id, user.Id);

                if (result)
                    return Json(new { success = true, message = "Rezervasyon başarıyla iptal edildi" });
                else
                    return Json(new { success = false, message = "Rezervasyon iptal edilemedi" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}