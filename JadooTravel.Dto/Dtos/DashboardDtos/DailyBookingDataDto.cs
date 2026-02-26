using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Dto.Dtos.DashboardDtos
{
    public class DailyBookingDataDto
    {
        public DateTime Date { get; set; }
        public int BookingCount { get; set; }
        public decimal Revenue { get; set; }
    }
}
