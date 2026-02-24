using JadooTravel.Entity.Entities;

namespace JadooTravel.DataAccess.Abstract
{
    public interface IReviewDal : IGenericDal<DestinationReview>
    {
        Task<List<DestinationReview>> GetByDestinationAsync(string destinationId, bool onlyApproved = false);
        Task<List<DestinationReview>> GetByUserAsync(string userId);
        Task<List<DestinationReview>> GetPendingAsync();
        Task SoftDeleteAsync(string reviewId);
        Task ApproveAsync(string reviewId, bool approve, string? adminNotes);
        Task RejectAsync(string reviewId, string reason);
        Task MarkHelpfulAsync(string reviewId);
    }
}