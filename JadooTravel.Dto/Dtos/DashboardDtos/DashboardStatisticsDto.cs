
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
        public int ActiveCategories { get; set; }
        public int TotalDestinations { get; set; }
        public int TotalBookings { get; set; }
        public int TotalTestimonials { get; set; }
        public int FilteredBookings { get; set; }
        public int ApprovedBookings { get; set; }
        public int PendingBookings { get; set; }
        public int RejectedBookings { get; set; }
        public int CancelledBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageBookingAmount { get; set; }
        public double ApprovalRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<DestinationCapacityDto> DestinationCapacities { get; set; } = new();
        public List<ResultDestinationDto> LatestDestinations { get; set; } = new();
        public List<DailyBookingDataDto> DailyBookingData { get; set; } = new();
        public List<BookingStatusDataDto> BookingStatusData { get; set; } = new();


    }
}