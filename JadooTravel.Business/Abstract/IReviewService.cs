using JadooTravel.Dto.Dtos.ReviewDtos;

namespace JadooTravel.Business.Abstract
{
    public interface IReviewService
    {
        // Yorum Oluştur/Düzenle/Sil
        Task CreateAsync(CreateReviewDto createReviewDto, string userId);
        Task UpdateAsync(UpdateReviewDto updateReviewDto);
        Task DeleteAsync(string reviewId);

        // Yorum Getir
        Task<ResultReviewDto> GetByIdAsync(string reviewId);
        Task<List<ResultReviewDto>> GetDestinationReviewsAsync(string destinationId);
        Task<List<ResultReviewDto>> GetApprovedReviewsAsync(string destinationId);
        Task<List<UserReviewDto>> GetUserReviewsAsync(string userId);

        // İstatistikler
        Task<DestinationReviewSummaryDto> GetDestinationReviewSummaryAsync(string destinationId);
        Task<ReviewStatsDto> GetReviewStatsAsync();

        // Admin İşlemleri
        Task<List<ResultReviewDto>> GetPendingReviewsAsync();
        Task ApproveReviewAsync(ApproveReviewDto approveDto);
        Task RejectReviewAsync(string reviewId, string reason);

        // Helpful Count
        Task MarkHelpfulAsync(string reviewId);

        // Rating'e göre ara
        Task<List<ResultReviewDto>> GetReviewsByRatingAsync(string destinationId, int rating);
    }
}