namespace JadooTravel.UI.Areas.Admin.Models
{
    public class AdminAuditLogCategoryViewModel
    {
        public string Key { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Icon { get; set; } = "fa-file-text-o";
        public string Badge { get; set; } = "Normal";
        public List<AdminAuditLogItemViewModel> Entries { get; set; } = [];
    }
}
