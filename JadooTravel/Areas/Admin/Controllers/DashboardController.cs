using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.DashboardDtos;
using JadooTravel.Dto.Dtos.DestinationDtos;
using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IDestinationService _destinationService;
        private readonly IBookingService _bookingService;
        private readonly ITestimonialService _testimonialService;
        private readonly IMapper _mapper;

        public DashboardController(ICategoryService categoryService, IDestinationService destinationService, IBookingService bookingService, ITestimonialService testimonialService, IMapper mapper)
        {
            _categoryService = categoryService;
            _destinationService = destinationService;
            _bookingService = bookingService;
            _testimonialService = testimonialService;
            _mapper = mapper;
        }

        public async Task<IActionResult> AdminDashboard()
        {
            var categories = await _categoryService.GetAllAsync();
            var destinations = await _destinationService.GetAllAsync();
            var bookings = await _bookingService.GetAllAsync();
            var testimonials = await _testimonialService.GetAllAsync();

            var destinationDtos = _mapper.Map<List<ResultDestinationDto>>(destinations);

            var statistics = new DashboardStatisticsDto
            {
                TotalCategories = categories.Count,
                TotalDestinations = destinations.Count,
                TotalBookings = bookings.Count,
                TotalTestimonials = testimonials.Count,
                DestinationCapacities = destinationDtos.Select(d => new DestinationCapacityDto
                {
                    CityCountry = d.CityCountry,
                    Capacity = d.Capacity,
                    Price = d.Price
                }).ToList(),
                LatestDestinations = destinationDtos.OrderByDescending(d => d.Id).Take(5).ToList()
            };

            return View(statistics);
        }

        [HttpGet]
        public async Task<IActionResult> GetDestinationCapacityData()
        {
            var destinations = await _destinationService.GetAllAsync();


            var data = destinations.Select(d => new
            {
                cityCountry = d.CityCountry,
                capacity = d.Capacity
            }).ToList();

            return Json(data);
        }
    }
}
