namespace JadooTravel.UI.Areas.Admin.Models
{
    public class AdminAuditLogPageViewModel
    {
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
        public List<AdminAuditLogCategoryViewModel> Categories { get; set; } = [];
    }
}
