using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.DashboardDtos;
using JadooTravel.Dto.Dtos.DestinationDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IDestinationService _destinationService;
        private readonly IBookingService _bookingService;
        private readonly ITestimonialService _testimonialService;
        private readonly IMapper _mapper;
        private readonly IAnalyticsService _analyticsService;
        public DashboardController(
             ICategoryService categoryService,
             IDestinationService destinationService,
             IBookingService bookingService,
             ITestimonialService testimonialService,
             IAnalyticsService analyticsService,
             IMapper mapper)
        {
            _categoryService = categoryService;
            _destinationService = destinationService;
            _bookingService = bookingService;
            _testimonialService = testimonialService;
            _mapper = mapper;
            _analyticsService = analyticsService;
        }

        [HttpGet]
        public async Task<IActionResult> AdminDashboard(DateTime? startDate, DateTime? endDate)
        {
            startDate ??= DateTime.UtcNow.AddDays(-30);
            endDate ??= DateTime.UtcNow;
            var categories = await _categoryService.GetAllAsync();
            var destinations = await _destinationService.GetAllAsync();
            var bookings = await _bookingService.GetAllAsync();
            var testimonials = await _testimonialService.GetAllAsync();
            var overview = await _analyticsService.GetOverviewAsync(startDate, endDate);
            var dailyStats = await _analyticsService.GetDailyStatsAsync(startDate.Value, endDate.Value);
            var destinationAnalytics = await _analyticsService.GetDestinationAnalyticsAsync();
            var topUsers = await _analyticsService.GetTopUsersAsync(10);
            var trafficSources = await _analyticsService.GetTrafficSourcesAsync();
            var destinationDtos = _mapper.Map<List<ResultDestinationDto>>(destinations);

            var statistics = new DashboardStatisticsDto
            {
                TotalCategories = categories.Count,
                TotalDestinations = destinations.Count,
                TotalBookings = bookings.Count,
                TotalTestimonials = testimonials.Count,
                TotalPageViews = overview.TotalPageViews,
                UniqueVisitors = overview.UniqueVisitors,
                TotalRevenue = overview.TotalRevenue,
                ConversionRate = overview.ConversionRate,
                StartDate = startDate.Value,
                EndDate = endDate.Value,
                DestinationCapacities = destinationDtos.Select(d => new DestinationCapacityDto
                {
                    CityCountry = d.CityCountry,
                    Capacity = d.Capacity,
                    Price = d.Price
                }).ToList(),
                LatestDestinations = destinationDtos.OrderByDescending(d => d.Id).Take(5).ToList(),
                DailyStats = dailyStats,
                DestinationAnalytics = destinationAnalytics.OrderByDescending(x => x.Bookings).Take(6).ToList(),
                TopUsers = topUsers,
                TrafficSources = trafficSources
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
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetChartData(string type, DateTime? startDate, DateTime? endDate)
        {
            startDate ??= DateTime.UtcNow.AddDays(-30);
            endDate ??= DateTime.UtcNow;

            switch (type)
            {
                case "daily":
                    var dailyStats = await _analyticsService.GetDailyStatsAsync(startDate.Value, endDate.Value);
                    return Json(new
                    {
                        labels = dailyStats.Select(x => x.Date.ToString("dd.MM")),
                        pageViews = dailyStats.Select(x => x.PageViews),
                        bookings = dailyStats.Select(x => x.Bookings),
                        revenue = dailyStats.Select(x => x.Revenue)
                    });

                case "destinations":
                    var destStats = await _analyticsService.GetDestinationAnalyticsAsync();
                    return Json(new
                    {
                        labels = destStats.Select(x => x.CityCountry),
                        bookings = destStats.Select(x => x.Bookings),
                        revenue = destStats.Select(x => x.Revenue)
                    });

                default:
                    return BadRequest();
            }
        }
    }
}
