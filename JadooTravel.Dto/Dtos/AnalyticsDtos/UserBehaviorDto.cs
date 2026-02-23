using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Dto.Dtos.AnalyticsDtos
{
    public class UserBehaviorDto
    {
        public string UserId { get; set; }
        public DateTime FirstVisit { get; set; }
        public DateTime LastVisit { get; set; }
        public int TotalVisits { get; set; }
        public int TotalPageViews { get; set; }
        public double AverageSessionDuration { get; set; }
        public int Bookings { get; set; }
        public decimal TotalSpent { get; set; }
        public string UserSegment { get; set; }
    }
}
