using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Dto.Dtos.ReviewDtos
{
    public class UserReviewDto
    {
        public string Id { get; set; }
        public string DestinationName { get; set; }
        public string DestinationImageUrl { get; set; }
        public int Rating { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsApproved { get; set; }
    }
}
