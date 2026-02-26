using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.DashboardDtos;
using JadooTravel.Dto.Dtos.DestinationDtos;
using JadooTravel.Entity.Entities.Enums;
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


        public DashboardController(
             ICategoryService categoryService,
             IDestinationService destinationService,
             IBookingService bookingService,
             ITestimonialService testimonialService,

             IMapper mapper)

        {
            _categoryService = categoryService;
            _destinationService = destinationService;
            _bookingService = bookingService;
            _testimonialService = testimonialService;
            _mapper = mapper;

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
            var destinationDtos = _mapper.Map<List<ResultDestinationDto>>(destinations);
            var filteredBookings = bookings
                .Where(x => x.CreatedDate >= startDate.Value && x.CreatedDate <= endDate.Value.AddDays(1).AddTicks(-1))
                .ToList();

            var approvedBookings = filteredBookings.Count(x => x.Status == BookingStatus.Approved);
            var pendingBookings = filteredBookings.Count(x => x.Status == BookingStatus.Pending);
            var rejectedBookings = filteredBookings.Count(x => x.Status == BookingStatus.Rejected);
            var cancelledBookings = filteredBookings.Count(x => x.Status == BookingStatus.Cancelled);

            var totalRevenue = filteredBookings
                .Where(x => x.Status == BookingStatus.Approved)
                .Sum(x => x.TotalPrice);

            var bookingDays = Math.Max(1, (endDate.Value.Date - startDate.Value.Date).Days + 1);

            var dailyBookingData = Enumerable.Range(0, bookingDays)
                .Select(offset => startDate.Value.Date.AddDays(offset))
                .Select(day => new DailyBookingDataDto
                {
                    Date = day,
                    BookingCount = filteredBookings.Count(x => x.CreatedDate.Date == day),
                    Revenue = filteredBookings
                        .Where(x => x.CreatedDate.Date == day && x.Status == BookingStatus.Approved)
                        .Sum(x => x.TotalPrice)
                })
                .ToList();
            var statistics = new DashboardStatisticsDto
            {
                TotalCategories = categories.Count,
                ActiveCategories = categories.Count(x => x.Status),
                TotalDestinations = destinations.Count,
                TotalBookings = bookings.Count,
                TotalTestimonials = testimonials.Count,
                FilteredBookings = filteredBookings.Count,
                ApprovedBookings = approvedBookings,
                PendingBookings = pendingBookings,
                RejectedBookings = rejectedBookings,
                CancelledBookings = cancelledBookings,
                TotalRevenue = totalRevenue,
                AverageBookingAmount = filteredBookings.Count == 0 ? 0 : filteredBookings.Average(x => x.TotalPrice),
                ApprovalRate = filteredBookings.Count == 0 ? 0 : (double)approvedBookings / filteredBookings.Count * 100,

                StartDate = startDate.Value,
                EndDate = endDate.Value,
                DestinationCapacities = destinationDtos
                    .OrderByDescending(x => x.Capacity)
                    .Take(8)
                    .Select(d => new DestinationCapacityDto
                    {
                        CityCountry = d.CityCountry,
                        Capacity = d.Capacity,
                        Price = d.Price
                    }).ToList(),
                LatestDestinations = destinationDtos.OrderByDescending(d => d.Id).Take(5).ToList(),

                DailyBookingData = dailyBookingData,
                BookingStatusData = new List<BookingStatusDataDto>
                {
                    new() { Status = "Beklemede", Count = pendingBookings },
                    new() { Status = "Onaylandı", Count = approvedBookings },
                    new() { Status = "Reddedildi", Count = rejectedBookings },
                    new() { Status = "İptal", Count = cancelledBookings }
                }

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