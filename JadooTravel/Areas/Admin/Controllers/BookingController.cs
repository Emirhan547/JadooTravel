

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
        public async Task<IActionResult> UpdateStatus(string id, BookingStatus status, string notes)
        {
            try
            {
                var updateDto = new UpdateBookingStatusDto
                {
                    BookingId = id,
                    Status = status,
                    Notes = notes
                };

                var result = await _bookingService.UpdateBookingStatusAsync(updateDto);

                if (result)
                    return Json(new { success = true, message = "Durumu güncellendi" });
                else
                    return Json(new { success = false, message = "Güncellenemedi" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> DeleteBooking(string id)
        {
            await _bookingService.DeleteAsync(id);
            return RedirectToAction("BookingList");
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
            return View(bookings);
        }
    }
}