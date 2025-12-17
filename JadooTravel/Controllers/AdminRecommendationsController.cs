using JadooTravel.Business.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.UI.Controllers
{
    public class AdminRecommendationsController : Controller
    {
        private readonly IAIService _aiService;
        private readonly ILogger<AdminRecommendationsController> _logger;

        public AdminRecommendationsController(
            IAIService aiService,
            ILogger<AdminRecommendationsController> logger)
        {
            _aiService = aiService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetRecommendations(string cityCountry)
        {
            if (string.IsNullOrWhiteSpace(cityCountry))
            {
                ModelState.AddModelError("cityCountry", "Lütfen bir şehir veya ülke adı girin.");
                return View("Index");
            }

            if (cityCountry.Length > 100)
            {
                ModelState.AddModelError("cityCountry", "Şehir/ülke adı çok uzun.");
                return View("Index");
            }

            try
            {
                _logger.LogInformation("{CityCountry} için öneriler alınıyor", cityCountry);
                var recommendations = await _aiService.GetCityRecommendationsAsync(cityCountry.Trim());

                ViewBag.CityCountry = cityCountry;
                ViewBag.Recommendations = recommendations;

                _logger.LogInformation("{CityCountry} için öneriler başarıyla alındı", cityCountry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{CityCountry} için öneri alınırken hata", cityCountry);
                ViewBag.Error = "Öneriler alınırken bir hata oluştu. Lütfen tekrar deneyin.";
            }

            return View("Index");
        }
    }
}