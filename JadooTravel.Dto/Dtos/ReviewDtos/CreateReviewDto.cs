using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Dto.Dtos.ReviewDtos
{
    public class CreateReviewDto
    {
        public string DestinationId { get; set; }
        public int Rating { get; set; } // 1-5
        public string Title { get; set; }
        public string Comment { get; set; }
        public int VisitedDays { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }
}
