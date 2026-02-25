using Elastic.Clients.Elasticsearch;
using JadooTravel.Entity.Entities;
using JadooTravel.UI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace JadooTravel.UI.Logging
{
    public class ElasticAuditLogger : IElasticAuditLogger
    {
        private readonly ElasticsearchClient _elasticsearchClient;
        private readonly ElasticLoggingOptions _options;
        private readonly ILogger<ElasticAuditLogger> _logger;
        private readonly UserManager<AppUser> _userManager;

        public ElasticAuditLogger(
            ElasticsearchClient elasticsearchClient,
            IOptions<ElasticLoggingOptions> options,
            ILogger<ElasticAuditLogger> logger,
            UserManager<AppUser> userManager)
        {
            _elasticsearchClient = elasticsearchClient;
            _options = options.Value;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task LogAsync(
            string eventName,
            string actorType,
            string? actorId,
            string action,
            string entityType,
            string? entityId,
            string status,
            object? metadata = null,
            string? actorEmail = null,
            string? message = null)
        {
            var indexName = string.Format(_options.IndexFormat, DateTime.UtcNow);
            var resolvedEmail = await ResolveActorEmailAsync(actorId, actorEmail);
            var resolvedMessage = string.IsNullOrWhiteSpace(message)
                ? BuildDefaultTurkishMessage(actorType, action, entityType, status)
                : message;

            var payload = new Dictionary<string, object?>
            {
                ["@timestamp"] = DateTime.UtcNow,
                ["event_name"] = eventName,
                ["actor_type"] = actorType,
                ["actor_id"] = actorId,
                ["actor_email"] = resolvedEmail,
                ["action"] = action,
                ["entity_type"] = entityType,
                ["entity_id"] = entityId,
                ["status"] = status,
                ["message"] = resolvedMessage,
                ["metadata"] = metadata
            };

            try
            {
                var response = await _elasticsearchClient.IndexAsync(payload, idx => idx.Index(indexName));

                if (!response.IsValidResponse)
                {
                    _logger.LogWarning(
                        "Elastic audit log could not be indexed. EventName: {EventName}, DebugInfo: {DebugInfo}",
                        eventName,
                        response.DebugInformation);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Elastic audit log failed unexpectedly. EventName: {EventName}", eventName);
            }
        }

        private async Task<string?> ResolveActorEmailAsync(string? actorId, string? actorEmail)
        {
            if (!string.IsNullOrWhiteSpace(actorEmail))
                return actorEmail;

            if (string.IsNullOrWhiteSpace(actorId))
                return null;

            if (actorId.Contains('@'))
                return actorId;

            try
            {
                var user = await _userManager.FindByIdAsync(actorId);
                return user?.Email;
            }
            catch
            {
                return null;
            }
        }

        private static string BuildDefaultTurkishMessage(string actorType, string action, string entityType, string status)
        {
            var actionTr = action switch
            {
                "create" => "oluşturma",
                "update" => "güncelleme",
                "delete" => "silme",
                "list" => "listeleme",
                "view" => "görüntüleme",
                "login" => "giriş",
                "logout" => "çıkış",
                "register" => "kayıt",
                "approve" => "onay",
                "reject" => "reddetme",
                "cancel" => "iptal",
                "change_password" => "şifre değiştirme",
                _ => action
            };

            var statusTr = status switch
            {
                "success" => "başarılı",
                "failed" => "başarısız",
                "error" => "hata",
                "forbidden" => "yetkisiz",
                "not_found" => "bulunamadı",
                "validation_failed" => "doğrulama hatası",
                _ => status
            };

            return $"{actorType} tarafından {entityType} üzerinde {actionTr} işlemi yapıldı. Sonuç: {statusTr}.";
        }
    }
}