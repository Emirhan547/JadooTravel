using JadooTravel.Dto.Dtos.ReviewDtos;

namespace JadooTravel.UI.Areas.Admin.Models
{
    public class AdminReviewListViewModel
    {
        public ReviewStatsDto Stats { get; set; } = new();
        public List<ResultReviewDto> Reviews { get; set; } = new();
    }
}