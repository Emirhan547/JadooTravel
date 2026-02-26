using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.BookingDtos;
using JadooTravel.Entity.Entities;
using JadooTravel.UI.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IDestinationService _destinationService;

        private readonly ILogger<BookingController> _logger;
        private readonly IElasticAuditLogger _auditLogger;
        public BookingController(
             IDestinationService destinationService,
            IBookingService bookingService,
            UserManager<AppUser> userManager,
         
            ILogger<BookingController> logger,
            IElasticAuditLogger auditLogger)
        {
            _bookingService = bookingService;
            _destinationService = destinationService;
            _userManager = userManager;

            _logger = logger;
            _auditLogger = auditLogger;
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
                    await _auditLogger.LogAsync("booking.create", "user", user.Id, "create", "booking", null, "failed", new { createBookingDto.DestinationId, reason = "destination_not_found" });
                    return RedirectToAction("Index", "Default");
                }

                createBookingDto.UserId = user.Id;
                createBookingDto.DestinationCityCountry = destination.CityCountry;
                createBookingDto.DestinationImageUrl = destination.ImageUrl;
                await _bookingService.CreateAsync(createBookingDto);
                await _auditLogger.LogAsync("booking.create", "user", user.Id, "create", "booking", null, "success", new
                {
                    createBookingDto.DestinationId,
                    createBookingDto.StartDate,
                    createBookingDto.EndDate,
                    createBookingDto.PersonCount
                });
                TempData["success"] = "Rezervasyonunuz başarıyla oluşturuldu. Kısa sürede size email gönderilecektir.";
                return RedirectToAction("MyBookings");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Booking create failed unexpectedly. DestinationId: {DestinationId}", createBookingDto.DestinationId);
                await _auditLogger.LogAsync("booking.create", "user", createBookingDto.UserId, "create", "booking", null, "error", new { createBookingDto.DestinationId, ex.Message });
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
                var bookings = await _bookingService.GetUserBookingsAsync(user.Id);
              
                await _auditLogger.LogAsync("booking.list", "user", user.Id, "list", "booking", null, "success", new { count = bookings.Count });
                return View(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MyBookings failed unexpectedly. UserId: {UserId}", user.Id);
                await _auditLogger.LogAsync("booking.list", "user", user.Id, "list", "booking", null, "error", new { ex.Message });
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


                if (booking.UserId != user.Id && !User.IsInRole("Admin"))
                {
                    await _auditLogger.LogAsync("booking.details", "user", user.Id, "view", "booking", id, "forbidden");
                    return Forbid();
                }
                await _auditLogger.LogAsync("booking.details", "user", user.Id, "view", "booking", id, "success");
                return View(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BookingDetails failed unexpectedly. UserId: {UserId}, BookingId: {BookingId}", user.Id, id);
                await _auditLogger.LogAsync("booking.details", "user", user.Id, "view", "booking", id, "error", new { ex.Message });
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
                {
                    await _auditLogger.LogAsync("booking.cancel", "user", user.Id, "cancel", "booking", id, "success");
                    return Json(new { success = true, message = "Rezervasyon başarıyla iptal edildi" });
                }

                await _auditLogger.LogAsync("booking.cancel", "user", user.Id, "cancel", "booking", id, "failed");
                return Json(new { success = false, message = "Rezervasyon iptal edilemedi" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Booking cancel failed unexpectedly. UserId: {UserId}, BookingId: {BookingId}", user.Id, id);
                await _auditLogger.LogAsync("booking.cancel", "user", user.Id, "cancel", "booking", id, "error", new { ex.Message });
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}