

using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.BookingDtos;
using JadooTravel.Entity.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public async Task<IActionResult> BookingList()
        {
            var values = await _bookingService.GetAllAsync();
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
            return View(booking);
        }

        [HttpGet]
        public async Task<IActionResult> PendingBookings()
        {
            var bookings = await _bookingService.GetBookingsByStatusAsync(BookingStatus.Pending);
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
        }
    }
}