using JadooTravel.Dto.Dtos.DestinationDtos;
using JadooTravel.Dto.Dtos.ReviewDtos;

namespace JadooTravel.UI.Models
{
    public class DestinationDetailViewModel
    {
        public UpdateDestinationDto Destination { get; set; }
        public List<ResultReviewDto> Reviews { get; set; } = new();
        public string? CurrentUserId { get; set; }
    }
}
