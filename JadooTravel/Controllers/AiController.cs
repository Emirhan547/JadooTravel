using JadooTravel.Business.Abstract;
using JadooTravel.Services;
using Microsoft.AspNetCore.Mvc;

namespace JadooTravel.Controllers;

[ApiController]
[Route("api/ai")]
public class AiController : ControllerBase
{
    private readonly IAIService _aiService;

    public AiController(IAIService aiService)
    {
        _aiService = aiService;
    }

    [HttpPost("recommendations")]
    public async Task<IActionResult> GetRecommendations([FromBody] AiRecommendationRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.CityCountry))
        {
            return BadRequest(new { message = "City or country is required." });
        }

        var recommendations = await _aiService.GetRecommendationsAsync(request.CityCountry, request.TargetLanguage, cancellationToken);
        return Ok(new AiRecommendationResponse(recommendations));
    }

    [HttpPost("translate")]
    public async Task<IActionResult> Translate([FromBody] AiTranslateRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.TargetLanguage))
        {
            return BadRequest(new { message = "Target language is required." });
        }

        var texts = request.Texts ?? Array.Empty<string>();
        var translations = await _aiService.TranslateAsync(request.TargetLanguage, texts, cancellationToken);
        return Ok(new AiTranslateResponse(translations));
    }
}

public record AiRecommendationRequest(string CityCountry, string? TargetLanguage);
public record AiRecommendationResponse(string Recommendations);
public record AiTranslateRequest(string TargetLanguage, IReadOnlyList<string>? Texts);
public record AiTranslateResponse(IReadOnlyList<string> Translations);