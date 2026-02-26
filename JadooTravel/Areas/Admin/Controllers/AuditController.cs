using Elastic.Clients.Elasticsearch;
using JadooTravel.UI.Areas.Admin.Models;
using JadooTravel.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace JadooTravel.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AuditController : Controller
    {
        private readonly ElasticsearchClient _elasticsearchClient;
        private readonly ElasticLoggingOptions _elasticOptions;
        private readonly ILogger<AuditController> _logger;

        public AuditController(
            ElasticsearchClient elasticsearchClient,
            IOptions<ElasticLoggingOptions> elasticOptions,
            ILogger<AuditController> logger)
        {
            _elasticsearchClient = elasticsearchClient;
            _elasticOptions = elasticOptions.Value;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new AdminAuditLogPageViewModel
            {
                GeneratedAt = DateTime.Now,
                Categories = BuildCategorySkeleton()
            };

            try
            {
                var indexPattern = ResolveIndexPattern(_elasticOptions.IndexFormat);
                var response = await _elasticsearchClient.SearchAsync<Dictionary<string, object?>>(s => s
                    .Index(indexPattern)
                    .Size(200)
                    .Sort(sort => sort.Field("@timestamp", field => field.Order(Elastic.Clients.Elasticsearch.SortOrder.Desc))));

                if (!response.IsValidResponse || response.Hits.Count == 0)
                {
                    return View(model);
                }

                var logs = response.Hits
                    .Select(hit => hit.Source)
                    .Where(source => source is not null)
                    .Select(MapAuditItem)
                    .Where(item => item is not null)
                    .Cast<AdminAuditLogItemViewModel>()
                    .ToList();

                model.Categories = BuildCategoriesFromLogs(logs);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Admin audit page could not fetch logs from Elasticsearch.");
            }

            return View(model);
        }

        private static List<AdminAuditLogCategoryViewModel> BuildCategorySkeleton()
        {
            return new List<AdminAuditLogCategoryViewModel>
            {
                new() { Key = "system", Title = "Sistem Logları", Icon = "fa-server", Badge = "Canlı" },
                new() { Key = "security", Title = "Güvenlik Logları", Icon = "fa-shield-alt", Badge = "Kritik" },
                new() { Key = "booking", Title = "Rezervasyon Logları", Icon = "fa-calendar-check", Badge = "Yoğun" },
                new() { Key = "activity", Title = "Kullanıcı Aktivite Logları", Icon = "fa-users", Badge = "Güncel" }
            };
        }

        private static List<AdminAuditLogCategoryViewModel> BuildCategoriesFromLogs(List<AdminAuditLogItemViewModel> logs)
        {
            var categories = BuildCategorySkeleton();

            foreach (var category in categories)
            {
                category.Entries = logs
                    .Where(log => ResolveCategory(log) == category.Key)
                    .Take(8)
                    .ToList();
            }

            return categories;
        }

        private static string ResolveCategory(AdminAuditLogItemViewModel item)
        {
            var eventName = item.EventName.ToLowerInvariant();
            var action = item.Action.ToLowerInvariant();
            var entityType = item.EntityType.ToLowerInvariant();

            if (eventName.Contains("auth") || action is "login" or "logout" or "register" || item.Status.Equals("forbidden", StringComparison.OrdinalIgnoreCase))
                return "security";

            if (entityType == "booking" || eventName.Contains("booking"))
                return "booking";

            if (entityType is "review" or "destination" or "favorite" || eventName.Contains("review") || eventName.Contains("destination"))
                return "activity";

            return "system";
        }

        private static AdminAuditLogItemViewModel? MapAuditItem(Dictionary<string, object?> source)
        {
            var timestampValue = GetStringValue(source, "@timestamp");
            if (!DateTime.TryParse(timestampValue, out var timestamp))
            {
                timestamp = DateTime.Now;
            }

            var model = new AdminAuditLogItemViewModel
            {
                Timestamp = timestamp.ToLocalTime(),
                EventName = GetStringValue(source, "event_name"),
                Action = GetStringValue(source, "action"),
                EntityType = GetStringValue(source, "entity_type"),
                Status = GetStringValue(source, "status"),
                Message = GetStringValue(source, "message"),
                ActorEmail = GetStringValue(source, "actor_email")
            };

            return string.IsNullOrWhiteSpace(model.EventName) ? null : model;
        }

        private static string GetStringValue(Dictionary<string, object?> source, string key)
        {
            if (!source.TryGetValue(key, out var value) || value is null)
                return string.Empty;

            if (value is string str)
                return str;

            if (value is JsonElement json)
            {
                return json.ValueKind switch
                {
                    JsonValueKind.String => json.GetString() ?? string.Empty,
                    JsonValueKind.Number => json.GetRawText(),
                    JsonValueKind.True => "true",
                    JsonValueKind.False => "false",
                    _ => json.GetRawText()
                };
            }

            return value.ToString() ?? string.Empty;
        }

        private static string ResolveIndexPattern(string indexFormat)
        {
            if (string.IsNullOrWhiteSpace(indexFormat))
                return "jadootravel-audit-*";

            var formatStart = indexFormat.IndexOf("{0", StringComparison.Ordinal);
            return formatStart > 0
                ? $"{indexFormat[..formatStart]}*"
                : $"{indexFormat}*";
        }
    }
}