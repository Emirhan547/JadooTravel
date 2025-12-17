using JadooTravel.Business.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JadooTravel.Business.Concrete
{
    public class OpenAIManager : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OpenAIManager> _logger;
        private readonly string? _apiKey;

        public OpenAIManager(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<OpenAIManager> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;

            // ÖNCE environment variable'dan oku
            _apiKey = Environment.GetEnvironmentVariable("OpenAI__ApiKey");

            // Environment variable yoksa configuration'dan oku
            if (string.IsNullOrEmpty(_apiKey))
            {
                _apiKey = configuration["OpenAI:ApiKey"]?.Trim();
            }

            // Debug için console'a yaz
            Console.WriteLine($"=== OPENAI API KEY DURUMU ===");
            Console.WriteLine($"Environment Variable: {(!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("OpenAI__ApiKey")) ? "BULUNDU" : "BULUNAMADI")}");
            Console.WriteLine($"Configuration: {(!string.IsNullOrEmpty(configuration["OpenAI:ApiKey"]?.Trim()) ? "BULUNDU" : "BULUNAMADI")}");
            Console.WriteLine($"Kullanılacak API Key: {(!string.IsNullOrEmpty(_apiKey) ? "BULUNDU" : "BULUNAMADI")}");

            if (string.IsNullOrEmpty(_apiKey))
            {
                _logger.LogWarning("🔴 OPENAI API ANAHTARI BULUNAMADI - DEMO MODU AKTİF");
                Console.WriteLine("🔴 DEMO MODU: Gerçek API kullanılamıyor");
            }
            else
            {
                _logger.LogInformation("🟢 OPENAI API ANAHTARI BAŞARIYLA YÜKLENDİ");
                Console.WriteLine($"🟢 GERÇEK API MODU: OpenAI'e bağlanılacak (Key: {_apiKey.Substring(0, Math.Min(10, _apiKey.Length))}...)");
            }

            // HttpClient yapılandırması
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<string> GetCityRecommendationsAsync(string cityCountry)
        {
            if (string.IsNullOrWhiteSpace(cityCountry))
            {
                throw new ArgumentException("Şehir/ülke adı boş olamaz", nameof(cityCountry));
            }

            Console.WriteLine($"🔍 İstek yapılıyor: {cityCountry}");
            Console.WriteLine($"🔍 API Key durumu: {(!string.IsNullOrEmpty(_apiKey) ? "MEVCUT" : "EKSİK")}");

            if (string.IsNullOrEmpty(_apiKey))
            {
                _logger.LogWarning("OpenAI API anahtarı bulunamadı, demo mod kullanılıyor: {CityCountry}", cityCountry);
                return DemoResponse(cityCountry);
            }

            try
            {
                var requestBody = new
                {
                    model = "gpt-4o-mini",
                    messages = new[]
                    {
                        new {
                            role = "system",
                            content = "Sen yardımsever bir seyahat asistanısın. Detaylı seyahat önerileri sağlarsın. Yanıtları Türkçe olarak ver."
                        },
                        new {
                            role = "user",
                            content = $"{cityCountry} şehri/ülkesi için popüler seyahat destinasyonları, aktiviteler ve gizli kalmış yerler öner. Açıklamalı kapsamlı bir liste sağla. Yanıtı Türkçe olarak ver."
                        }
                    },
                    max_tokens = 1000,
                    temperature = 0.7
                };

                var json = JsonSerializer.Serialize(requestBody);
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

                _logger.LogInformation("OpenAI API'ye istek gönderiliyor: {CityCountry}", cityCountry);
                Console.WriteLine($"🚀 OpenAI API'ye istek gönderiliyor...");

                var response = await _httpClient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"📡 OpenAI API yanıt durumu: {response.StatusCode}");
                _logger.LogDebug("OpenAI API yanıt durumu: {StatusCode}", response.StatusCode);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"❌ OpenAI API hatası: {response.StatusCode}");
                    _logger.LogError("OpenAI API hatası: {StatusCode} - {ResponseBody}", response.StatusCode, responseBody);
                    return DemoResponse(cityCountry);
                }

                using var doc = JsonDocument.Parse(responseBody);

                // Geliştirilmiş JSON parsing hata yönetimi
                if (doc.RootElement.TryGetProperty("choices", out var choices) &&
                    choices.GetArrayLength() > 0 &&
                    choices[0].TryGetProperty("message", out var message) &&
                    message.TryGetProperty("content", out var content))
                {
                    var result = content.GetString();
                    Console.WriteLine($"✅ OpenAI'den başarılı yanıt alındı!");
                    return string.IsNullOrWhiteSpace(result) ? DemoResponse(cityCountry) : result.Trim();
                }

                _logger.LogWarning("OpenAI API'den beklenmeyen yanıt formatı");
                Console.WriteLine($"⚠️ OpenAI API'den beklenmeyen yanıt formatı");
                return DemoResponse(cityCountry);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "OpenAI API çağrısında ağ hatası: {CityCountry}", cityCountry);
                Console.WriteLine($"❌ Ağ hatası: {ex.Message}");
                return DemoResponse(cityCountry);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "OpenAI API çağrısında zaman aşımı: {CityCountry}", cityCountry);
                Console.WriteLine($"⏰ Zaman aşımı hatası: {ex.Message}");
                return DemoResponse(cityCountry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OpenAI API çağrısında beklenmeyen hata: {CityCountry}", cityCountry);
                Console.WriteLine($"💥 Beklenmeyen hata: {ex.Message}");
                return DemoResponse(cityCountry);
            }
        }

        private static string DemoResponse(string cityCountry)
        {
            return $"""
            [Demo Modu] {cityCountry} için seyahat önerileri:

            🏛️ Kültürel Mekanlar:
            • Tarihi yerler ve müzeler
            • Yerel kültür merkezleri
            
            🍽️ Yemek & Restoranlar:
            • Geleneksel yerel mutfak
            • Popüler restoranlar ve kafeler
            
            🌳 Açık Hava Aktiviteleri:
            • Parklar ve doğal güzellikler
            • Manzaralı yürüyüş turları
            
            💡 İpucu: Daha detaylı ve kişiselleştirilmiş öneriler için lütfen OpenAI API yapılandırmasını kontrol edin.

            İyi seyahatler! 🌍
            """;
        }
    }
}