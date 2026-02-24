using JadooTravel.Dto.Dtos.ReviewDtos;

namespace JadooTravel.Business.Abstract
{
    public interface IReviewService
    {
        Task CreateAsync(CreateReviewDto createReviewDto, string userId);
        Task UpdateAsync(UpdateReviewDto updateReviewDto);
        Task DeleteAsync(string reviewId);


        Task<ResultReviewDto> GetByIdAsync(string reviewId);
        Task<List<ResultReviewDto>> GetDestinationReviewsAsync(string destinationId);
        Task<List<ResultReviewDto>> GetApprovedReviewsAsync(string destinationId);
        Task<List<UserReviewDto>> GetUserReviewsAsync(string userId);

        Task<DestinationReviewSummaryDto> GetDestinationReviewSummaryAsync(string destinationId);
        Task<ReviewStatsDto> GetReviewStatsAsync();

        Task<List<ResultReviewDto>> GetAllReviewsAsync();
        Task<List<ResultReviewDto>> GetPendingReviewsAsync();
        Task ApproveReviewAsync(ApproveReviewDto approveDto);
        Task RejectReviewAsync(string reviewId, string reason);

        // Helpful Count
        Task MarkHelpfulAsync(string reviewId);

        // Rating'e göre ara
        Task<List<ResultReviewDto>> GetReviewsByRatingAsync(string destinationId, int rating);
    }
}