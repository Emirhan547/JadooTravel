namespace JadooTravel.UI.Logging
{
    public interface IElasticAuditLogger
    {
        Task LogAsync(
            string eventName,
            string actorType,
            string? actorId,
            string action,
            string entityType,
            string? entityId,
            string status,
            object? metadata = null,
            string? actorEmail = null,
            string? message = null);
    }
}