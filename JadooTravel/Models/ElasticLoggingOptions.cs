namespace JadooTravel.UI.Models
{
    public class ElasticLoggingOptions
    {
        public string Uri { get; set; } = "http://localhost:9200";
        public string IndexFormat { get; set; } = "jadootravel-audit-{0:yyyy.MM}";
    }
}