using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.BookingDtos;
using JadooTravel.Entity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

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

        public async Task<IActionResult>DeleteBooking(string id)
        {
            await _bookingService.DeleteAsync(id);
            return RedirectToAction("BookingList");
        }
    }
}