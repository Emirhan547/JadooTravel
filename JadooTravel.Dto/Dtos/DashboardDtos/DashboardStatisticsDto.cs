
using JadooTravel.Dto.Dtos.DestinationDtos;
using JadooTravel.Dto.Dtos.UserDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Dto.Dtos.DashboardDtos
{
    public class DashboardStatisticsDto
    {
        public int TotalCategories { get; set; }
        public int TotalDestinations { get; set; }
        public int TotalBookings { get; set; }
        public int TotalTestimonials { get; set; }
        public int TotalPageViews { get; set; }
        public int UniqueVisitors { get; set; }
        public decimal TotalRevenue { get; set; }
        public double ConversionRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<DestinationCapacityDto> DestinationCapacities { get; set; }
        public List<ResultDestinationDto> LatestDestinations { get; set; }
        public List<FavoriteDestinationStatDto> FavoriteDestinationStats { get; set; } = new();

    }
}
