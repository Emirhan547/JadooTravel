using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.DataAccess.Abstract;
using JadooTravel.Dto.Dtos.ReviewDtos;
using JadooTravel.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace JadooTravel.Business.Concrete
{
    public class ReviewService : IReviewService
    {

        private readonly IReviewDal _reviewDal;
        private readonly IDestinationDal _destinationDal;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public ReviewService(
          IReviewDal reviewDal,
            IDestinationDal destinationDal,
            UserManager<AppUser> userManager,
            IMapper mapper)
        {
            _reviewDal = reviewDal;
            _destinationDal = destinationDal;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task CreateAsync(CreateReviewDto createReviewDto, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new Exception("Kullanıcı bulunamadı");

            var review = new DestinationReview
            {
                UserId = userId,
                DestinationId = createReviewDto.DestinationId,
                UserName = user.FullName,
                UserEmail = user.Email,
                UserProfileImageUrl = user.ProfileImageUrl,
                Rating = createReviewDto.Rating,
                Title = createReviewDto.Title,
                Comment = createReviewDto.Comment,
                VisitedDays = createReviewDto.VisitedDays,
                Tags = createReviewDto.Tags,
                CreatedDate = DateTime.UtcNow,
                IsApproved = false
            };

            await _reviewDal.CreateAsync(review);
        }

        public async Task UpdateAsync(UpdateReviewDto updateReviewDto)
        {
            var review = await _reviewDal.GetByIdAsync(updateReviewDto.Id)
                 ?? throw new Exception("Yorum bulunamadı");

            review.Rating = updateReviewDto.Rating;
            review.Title = updateReviewDto.Title;
            review.Comment = updateReviewDto.Comment;
            review.VisitedDays = updateReviewDto.VisitedDays;
            review.Tags = updateReviewDto.Tags;
            review.UpdatedDate = DateTime.UtcNow;
            review.IsApproved = false;

            await _reviewDal.UpdateAsync(review);
        }
        public async Task<List<ResultReviewDto>> GetAllReviewsAsync()
        {
            var reviews = await _reviewDal.GetAllAsync();
            var activeReviews = reviews
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedDate)
                .ToList();

            return _mapper.Map<List<ResultReviewDto>>(activeReviews);
        }
        public async Task DeleteAsync(string reviewId) => await _reviewDal.SoftDeleteAsync(reviewId);

        public async Task<ResultReviewDto> GetByIdAsync(string reviewId)
       => _mapper.Map<ResultReviewDto>(await _reviewDal.GetByIdAsync(reviewId));

        public async Task<List<ResultReviewDto>> GetDestinationReviewsAsync(string destinationId)
       => _mapper.Map<List<ResultReviewDto>>(await _reviewDal.GetByDestinationAsync(destinationId));

        public async Task<List<ResultReviewDto>> GetApprovedReviewsAsync(string destinationId)
        => _mapper.Map<List<ResultReviewDto>>(await _reviewDal.GetByDestinationAsync(destinationId, onlyApproved: true));

        public async Task<List<UserReviewDto>> GetUserReviewsAsync(string userId)
       => _mapper.Map<List<UserReviewDto>>(await _reviewDal.GetByUserAsync(userId));

        public async Task<DestinationReviewSummaryDto> GetDestinationReviewSummaryAsync(string destinationId)
        {
            var destination = await _destinationDal.GetByIdAsync(destinationId);
            var allReviews = await _reviewDal.GetByDestinationAsync(destinationId);
            var approvedReviews = allReviews.Where(x => x.IsApproved).ToList();

            var ratingDistribution = Enumerable.Range(1, 5)
                .ToDictionary(rating => rating, rating => approvedReviews.Count(x => x.Rating == rating));

            var topTags = approvedReviews
                .Where(x => x.Tags != null)
                .SelectMany(x => x.Tags!)
                .GroupBy(x => x)
                .OrderByDescending(x => x.Count())
                .Take(5)
                .Select(x => new KeyValuePair<string, int>(x.Key, x.Count()))
                .ToList();

            var latestReviews = approvedReviews
                .OrderByDescending(x => x.CreatedDate)
                .Take(5)
                .ToList();

            return new DestinationReviewSummaryDto
            {
                DestinationId = destinationId,
                CityCountry = destination?.CityCountry ?? "Bilinmiyor",
                AverageRating = approvedReviews.Count > 0 ? Math.Round(approvedReviews.Average(x => x.Rating), 2) : 0,
                TotalReviews = allReviews.Count,
                ApprovedReviews = approvedReviews.Count,
                PendingReviews = allReviews.Count(x => !x.IsApproved),
                RatingDistribution = ratingDistribution,
                TopTags = topTags,
                LatestReviews = _mapper.Map<List<ResultReviewDto>>(latestReviews)
            };
        }

        public async Task<ReviewStatsDto> GetReviewStatsAsync()
        {
            var allReviews = await _reviewDal.GetAllAsync();
            var activeReviews = allReviews.Where(x => !x.IsDeleted).ToList();
            var approvedReviews = activeReviews.Where(x => x.IsApproved).ToList();

            return new ReviewStatsDto
            {
                TotalReviews = activeReviews.Count,
                ApprovedReviews = approvedReviews.Count,
                PendingReviews = activeReviews.Count(x => !x.IsApproved),
                AverageRating = approvedReviews.Count > 0 ? Math.Round(approvedReviews.Average(x => x.Rating), 2) : 0,
                ReviewsThisMonth = activeReviews.Count(x => x.CreatedDate.Month == DateTime.UtcNow.Month && x.CreatedDate.Year == DateTime.UtcNow.Year),
                DestinationsWithReviews = activeReviews.Select(x => x.DestinationId).Distinct().Count()
            };
        }

        public async Task<List<ResultReviewDto>> GetPendingReviewsAsync()
        => _mapper.Map<List<ResultReviewDto>>(await _reviewDal.GetPendingAsync());

        public async Task ApproveReviewAsync(ApproveReviewDto approveDto)
        => await _reviewDal.ApproveAsync(approveDto.ReviewId, approveDto.Approve, approveDto.AdminNotes);

        public async Task RejectReviewAsync(string reviewId, string reason)
        => await _reviewDal.RejectAsync(reviewId, reason);

        public async Task MarkHelpfulAsync(string reviewId)
        => await _reviewDal.MarkHelpfulAsync(reviewId);

        public async Task<List<ResultReviewDto>> GetReviewsByRatingAsync(string destinationId, int rating)
       => _mapper.Map<List<ResultReviewDto>>(await _reviewDal.GetByDestinationAndRatingAsync(destinationId, rating));
    }
}