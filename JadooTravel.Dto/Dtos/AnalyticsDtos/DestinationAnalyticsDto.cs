using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Dto.Dtos.AnalyticsDtos
{
    public class DestinationAnalyticsDto
    {
        public string DestinationId { get; set; }
        public string CityCountry { get; set; }
        public int PageViews { get; set; }
        public int Bookings { get; set; }
        public decimal Revenue { get; set; }
        public double ConversionRate { get; set; } // (Bookings / PageViews) * 100
        public int AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}
