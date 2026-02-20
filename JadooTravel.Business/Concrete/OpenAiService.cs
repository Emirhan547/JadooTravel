using JadooTravel.Business.Abstract;
using JadooTravel.Business.Options;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace JadooTravel.Services;

public class OpenAiService : IAIService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient _httpClient;
    private readonly OpenAiOptions _options;

    public OpenAiService(HttpClient httpClient, IOptions<OpenAiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<string> GetRecommendationsAsync(string cityCountry, string? targetLanguage, CancellationToken cancellationToken)
    {
        var language = string.IsNullOrWhiteSpace(targetLanguage) ? "Turkish" : targetLanguage;
        var payload = new
        {
            model = _options.Model,
            temperature = 0.7,
            messages = new[]
            {
                new
                {
                    role = "system",
                    content = "You are a travel assistant. Provide concise, friendly travel recommendations with headings and bullet points."
                },
                new
                {
                    role = "user",
                    content = $"Destination: {cityCountry}. Respond in {language}. Include best time to visit, must-see spots, local food, and tips."
                }
            }
        };

        return await SendChatRequestAsync(payload, cancellationToken);
    }

    public async Task<IReadOnlyList<string>> TranslateAsync(string targetLanguage, IReadOnlyList<string> texts, CancellationToken cancellationToken)
    {
        if (texts.Count == 0)
        {
            return Array.Empty<string>();
        }

        var payload = new
        {
            model = _options.Model,
            temperature = 0.2,
            response_format = new { type = "json_object" },
            messages = new[]
            {
                new
                {
                    role = "system",
                    content = "You translate UI text. Return only JSON in the shape {\"translations\": [\"...\"]} matching the input order."
                },
                new
                {
                    role = "user",
                    content = $"Target language: {targetLanguage}. Texts: {JsonSerializer.Serialize(texts, JsonOptions)}"
                }
            }
        };

        var responseText = await SendChatRequestAsync(payload, cancellationToken);
        using var document = JsonDocument.Parse(responseText);
        if (!document.RootElement.TryGetProperty("translations", out var translationsElement))
        {
            return texts.ToArray();
        }

        var translations = new List<string>();
        foreach (var item in translationsElement.EnumerateArray())
        {
            translations.Add(item.GetString() ?? string.Empty);
        }

        return translations;
    }

    private async Task<string> SendChatRequestAsync(object payload, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw new InvalidOperationException("OpenAI API key is missing.");
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, _options.BaseUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
        request.Content = new StringContent(JsonSerializer.Serialize(payload, JsonOptions), Encoding.UTF8, "application/json");

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"OpenAI request failed: {responseContent}");
        }

        using var document = JsonDocument.Parse(responseContent);
        var message = document.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
        return message?.Trim() ?? string.Empty;
    }
}