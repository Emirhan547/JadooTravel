using JadooTravel.Business.Abstract;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace JadooTravel.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendationsController : ControllerBase
    {
        private readonly IAIService _aiService;
        private readonly ILogger<RecommendationsController> _logger;

        public RecommendationsController(
            IAIService aiService,
            ILogger<RecommendationsController> logger)
        {
            _aiService = aiService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> GetRecommendations([FromBody] RecommendationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { error = "Geçersiz istek" });
            }

            if (string.IsNullOrWhiteSpace(request.CityCountry))
            {
                return BadRequest(new { error = "Şehir/ülke adı boş olamaz" });
            }

            if (request.CityCountry.Length > 100)
            {
                return BadRequest(new { error = "Şehir/ülke adı çok uzun" });
            }

            try
            {
                _logger.LogInformation("API: {CityCountry} için öneriler alınıyor", request.CityCountry);

                var recommendations = await _aiService.GetCityRecommendationsAsync(request.CityCountry.Trim());

                _logger.LogInformation("API: {CityCountry} için öneriler başarıyla alındı", request.CityCountry);

                return Ok(new
                {
                    success = true,
                    cityCountry = request.CityCountry.Trim(),
                    recommendations = recommendations
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API: {CityCountry} için öneri alınırken hata", request.CityCountry);
                return StatusCode(500, new { error = "Öneriler alınırken bir hata oluştu" });
            }
        }
    }

    public class RecommendationRequest
    {
        [Required(ErrorMessage = "Şehir/ülke adı gereklidir")]
        [StringLength(100, ErrorMessage = "Şehir/ülke adı çok uzun")]
        public string CityCountry { get; set; } = string.Empty;
    }
}