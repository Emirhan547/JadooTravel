using JadooTravel.Dto.Dtos.DestinationDtos;
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
        public List<DestinationCapacityDto> DestinationCapacities { get; set; }
        public List<ResultDestinationDto> LatestDestinations { get; set; }
    }
}
