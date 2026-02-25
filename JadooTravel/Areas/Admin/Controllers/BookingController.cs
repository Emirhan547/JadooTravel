

using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.BookingDtos;
using JadooTravel.Entity.Entities.Enums;
using JadooTravel.UI.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IElasticAuditLogger _auditLogger;

        public BookingController(IBookingService bookingService, IElasticAuditLogger auditLogger)
        {
            _bookingService = bookingService;
            _auditLogger = auditLogger;
        }

        public async Task<IActionResult> BookingList()
        {
            var values = await _bookingService.GetAllAsync();
            await _auditLogger.LogAsync("admin.booking.list", "admin", User.Identity?.Name, "list", "booking", null, "success", new { count = values.Count });
            return View(values);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveBooking(string id)
        {
            await SetStatusAsync(id, BookingStatus.Approved, "Rezervasyon admin tarafından onaylandı.");
            return RedirectToAction(nameof(BookingList));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectBooking(string id, string notes)
        {
            await SetStatusAsync(id, BookingStatus.Rejected, string.IsNullOrWhiteSpace(notes) ? "Rezervasyon admin tarafından reddedildi." : notes);
            return RedirectToAction(nameof(BookingList));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassive(string id, string notes)
        {
            await SetStatusAsync(id, BookingStatus.Cancelled, string.IsNullOrWhiteSpace(notes) ? "Rezervasyon pasife çekildi." : notes);
            return RedirectToAction(nameof(BookingList));
        }

        [HttpGet]
        public async Task<IActionResult> BookingDetails(string id)
        {
            var booking = await _bookingService.GetByIdAsync(id);
            await _auditLogger.LogAsync("admin.booking.details", "admin", User.Identity?.Name, "view", "booking", id, "success");
            return View(booking);
        }

        [HttpGet]
        public async Task<IActionResult> PendingBookings()
        {
            var bookings = await _bookingService.GetBookingsByStatusAsync(BookingStatus.Pending);
            await _auditLogger.LogAsync("admin.booking.pending", "admin", User.Identity?.Name, "list", "booking", null, "success", new { count = bookings.Count });
            return View("BookingList", bookings);
        }

        private async Task SetStatusAsync(string id, BookingStatus status, string notes)
        {
            var updateDto = new UpdateBookingStatusDto
            {
                BookingId = id,
                Status = status,
                Notes = notes
            };

            await _bookingService.UpdateBookingStatusAsync(updateDto);
            await _auditLogger.LogAsync("admin.booking.status", "admin", User.Identity?.Name, "update_status", "booking", id, "success", new { status = status.ToString(), notes });
        }
    }
}