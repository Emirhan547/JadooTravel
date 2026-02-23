using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Dto.Dtos.AnalyticsDtos
{
    public class MonthlyStatsDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int TotalPageViews { get; set; }
        public int UniqueUsers { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public double AverageConversionRate { get; set; }
    }
}
