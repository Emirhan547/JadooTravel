using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.BookingDtos;
using JadooTravel.Entity.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace JadooTravel.UI.Controllers
{
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IMapper _mapper;

        public BookingController(IBookingService bookingService, IMapper mapper)
        {
            _bookingService = bookingService;
            _mapper = mapper;
        }

        public async Task<IActionResult> BookingList()
        {
            var values = await _bookingService.TGetAllListAsync();
            var valueList = _mapper.Map<List<ResultBookingDto>>(values);
            return View(valueList);
        }

        [HttpGet]
        public IActionResult CreateBooking()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking(CreateBookingDto createBookingDto)
        {
            var values = _mapper.Map<Booking>(createBookingDto);
            await _bookingService.TCreateAsync(values);
            TempData["Success"] = "Rezervasyonunuz başarıyla alındı!";
            return RedirectToAction("CategoryList", "Category");
        }

        public async Task<IActionResult> DeleteBooking(ObjectId id)
        {
            await _bookingService.TDeleteAsync(id);
            return RedirectToAction("BookingList");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateBooking(ObjectId id)
        {
            var value = await _bookingService.TGetByIdAsync(id);
            var updateBooking = _mapper.Map<UpdateBookingDto>(value);
            return View(updateBooking);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBooking(UpdateBookingDto updateBookingDto)
        {
            var values = _mapper.Map<Booking>(updateBookingDto);
            await _bookingService.TUpdateAsync(values);
            return RedirectToAction("BookingList");
        }
    }
}