using JadooTravel.Business.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AnalyticsController : Controller
    {
        private readonly IAnalyticsService _analyticsService;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var overview = await _analyticsService.GetOverviewAsync();
            return View(overview);
        }

        [HttpGet]
        public async Task<IActionResult> DailyReport(DateTime? startDate, DateTime? endDate)
        {
            startDate ??= DateTime.UtcNow.AddDays(-30);
            endDate ??= DateTime.UtcNow;

            var dailyStats = await _analyticsService.GetDailyStatsAsync(startDate.Value, endDate.Value);
            return View(dailyStats);
        }

        [HttpGet]
        public async Task<IActionResult> DestinationAnalytics()
        {
            var destinationStats = await _analyticsService.GetDestinationAnalyticsAsync();
            return View(destinationStats);
        }

        [HttpGet]
        public async Task<IActionResult> TopUsers()
        {
            var topUsers = await _analyticsService.GetTopUsersAsync(20);
            return View(topUsers);
        }

        [HttpGet]
        public async Task<IActionResult> TrafficSources()
        {
            var sources = await _analyticsService.GetTrafficSourcesAsync();
            return View(sources);
        }

        // JSON API'ler (Chart.js için)
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
