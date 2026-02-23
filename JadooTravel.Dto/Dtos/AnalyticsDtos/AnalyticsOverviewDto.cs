using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Dto.Dtos.AnalyticsDtos
{
    public class AnalyticsOverviewDto
    {
        public int TotalPageViews { get; set; }
        public int UniqueVisitors { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }

        public double ConversionRate { get; set; }
        public double BounceRate { get; set; }
        public double AverageSessionDuration { get; set; }

        public int NewUsers { get; set; }
        public int ReturningUsers { get; set; }

        public Dictionary<string, int> TopPages { get; set; }
        public List<DailyStatsDto> Last30DaysStats { get; set; }
        public List<DestinationAnalyticsDto> TopDestinations { get; set; }
    }
}
