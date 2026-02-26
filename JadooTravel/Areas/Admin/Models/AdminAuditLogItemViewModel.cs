namespace JadooTravel.UI.Areas.Admin.Models
{
    public class AdminAuditLogItemViewModel
    {
        public DateTime Timestamp { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? ActorEmail { get; set; }
    }
}
