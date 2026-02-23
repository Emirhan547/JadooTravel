using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Dto.Dtos.AnalyticsDtos
{
    public class DailyStatsDto
    {
        public DateTime Date { get; set; }
        public int PageViews { get; set; }
        public int UniqueVisitors { get; set; }
        public int Bookings { get; set; }
        public decimal Revenue { get; set; }
        public double AverageSessionDuration { get; set; }
        public double ConversionRate { get; set; }
    }
}
