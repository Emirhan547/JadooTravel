using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.BookingDtos;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace JadooTravel.UI.Controllers
{
    public class BookingController (IBookingService _bookingService): Controller
    {
        [HttpPost]
        public async Task<IActionResult> CreateBooking(CreateBookingDto createBookingDto)
        {
            await _bookingService.CreateAsync(createBookingDto);
            return RedirectToAction("Index", "Default");
        }
    }
}
