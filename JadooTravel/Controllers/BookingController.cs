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

        public BookingController(
            IBookingService bookingService,
            UserManager<AppUser> userManager)
        {
            _bookingService = bookingService;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking(CreateBookingDto createBookingDto)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return RedirectToAction("Login", "Auth");

                // Kullanıcı ID'sini ekle
                // Hizmet katmanında işlenecek
                await _bookingService.CreateAsync(createBookingDto);

                TempData["success"] = "Rezervasyonunuz başarıyla oluşturuldu. Kısa sürede size email gönderilecektir.";
                return RedirectToAction("Index", "Default");
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