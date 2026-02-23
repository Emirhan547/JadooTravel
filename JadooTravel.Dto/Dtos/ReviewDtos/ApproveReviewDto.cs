using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Dto.Dtos.ReviewDtos
{
    public class ApproveReviewDto
    {
        public string ReviewId { get; set; }
        public bool Approve { get; set; }
        public string AdminNotes { get; set; }
    }
}
