using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Dto.Dtos.ReviewDtos
{
    public class DestinationReviewSummaryDto
    {
        public string DestinationId { get; set; }
        public string CityCountry { get; set; }
        public double AverageRating { get; set; } // 0-5
        public int TotalReviews { get; set; }
        public int ApprovedReviews { get; set; }
        public int PendingReviews { get; set; }
        public Dictionary<int, int> RatingDistribution { get; set; } // Rating -> Count

        // En sık etiketler
        public List<KeyValuePair<string, int>> TopTags { get; set; }

        // En son yorumlar
        public List<ResultReviewDto> LatestReviews { get; set; }
    }
}
