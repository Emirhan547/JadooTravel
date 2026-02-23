using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Dto.Dtos.ReviewDtos
{
    public class ResultReviewDto
    {
        public string Id { get; set; }
        public string DestinationId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserProfileImageUrl { get; set; }
        public int Rating { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
        public bool IsApproved { get; set; }
        public int HelpfulCount { get; set; }
        public int VisitedDays { get; set; }
        public List<string> Tags { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
