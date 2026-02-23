using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Dto.Dtos.ReviewDtos
{
    public class ReviewStatsDto
    {
        public int TotalReviews { get; set; }
        public int ApprovedReviews { get; set; }
        public int PendingReviews { get; set; }
        public double AverageRating { get; set; }
        public int ReviewsThisMonth { get; set; }
        public int DestinationsWithReviews { get; set; }
    }
}
