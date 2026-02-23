using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.Dto.Dtos.ReviewDtos;
using JadooTravel.Entity.Entities;
using MongoDB.Driver;

namespace JadooTravel.Business.Concrete
{
    public class ReviewService : IReviewService
    {
       
        private readonly IMongoDatabase _mongoDatabase;
        private readonly IMapper _mapper;

        public ReviewService(
          IMongoDatabase mongoDatabase,
            IMapper mapper)
        {
            _mongoDatabase = mongoDatabase;
            _mapper = mapper;
        }

        public async Task CreateAsync(CreateReviewDto createReviewDto, string userId)
        {
            try
            {
                var reviewCollection = _mongoDatabase.GetCollection<DestinationReview>("DestinationReviews");
                var userCollection = _mongoDatabase.GetCollection<AppUser>("Users");

                var user = await userCollection.Find(x => x.Id == userId).FirstOrDefaultAsync();
                if (user == null)
                    throw new Exception("Kullanıcı bulunamadı");

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
                    IsApproved = false // Admin onayı gerekli
                };

                await reviewCollection.InsertOneAsync(review);
            }
            catch (Exception ex)
            {
                throw new Exception($"Yorum eklenirken hata oluştu: {ex.Message}");
            }
        }

        public async Task UpdateAsync(UpdateReviewDto updateReviewDto)
        {
            try
            {
                var reviewCollection = _mongoDatabase.GetCollection<DestinationReview>("DestinationReviews");

                var update = Builders<DestinationReview>.Update
                    .Set(x => x.Rating, updateReviewDto.Rating)
                    .Set(x => x.Title, updateReviewDto.Title)
                    .Set(x => x.Comment, updateReviewDto.Comment)
                    .Set(x => x.VisitedDays, updateReviewDto.VisitedDays)
                    .Set(x => x.Tags, updateReviewDto.Tags)
                    .Set(x => x.UpdatedDate, DateTime.UtcNow)
                    .Set(x => x.IsApproved, false); // Tekrar review gerekli

                await reviewCollection.UpdateOneAsync(
                    Builders<DestinationReview>.Filter.Eq(x => x.Id, updateReviewDto.Id),
                    update
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Yorum güncellenirken hata: {ex.Message}");
            }
        }

        public async Task DeleteAsync(string reviewId)
        {
            try
            {
                var reviewCollection = _mongoDatabase.GetCollection<DestinationReview>("DestinationReviews");

                var update = Builders<DestinationReview>.Update
                    .Set(x => x.IsDeleted, true);

                await reviewCollection.UpdateOneAsync(
                    Builders<DestinationReview>.Filter.Eq(x => x.Id, reviewId),
                    update
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Yorum silinirken hata: {ex.Message}");
            }
        }

        public async Task<ResultReviewDto> GetByIdAsync(string reviewId)
        {
            try
            {
                var reviewCollection = _mongoDatabase.GetCollection<DestinationReview>("DestinationReviews");
                var review = await reviewCollection.Find(x => x.Id == reviewId).FirstOrDefaultAsync();
                return _mapper.Map<ResultReviewDto>(review);
            }
            catch (Exception ex)
            {
                throw new Exception($"Yorum getirilirken hata: {ex.Message}");
            }
        }

        public async Task<List<ResultReviewDto>> GetDestinationReviewsAsync(string destinationId)
        {
            try
            {
                var reviewCollection = _mongoDatabase.GetCollection<DestinationReview>("DestinationReviews");
                var filter = Builders<DestinationReview>.Filter.And(
                    Builders<DestinationReview>.Filter.Eq(x => x.DestinationId, destinationId),
                    Builders<DestinationReview>.Filter.Eq(x => x.IsDeleted, false)
                );
                var sort = Builders<DestinationReview>.Sort.Descending(x => x.CreatedDate);

                var reviews = await reviewCollection
                    .Find(filter)
                    .Sort(sort)
                    .ToListAsync();

                return _mapper.Map<List<ResultReviewDto>>(reviews);
            }
            catch (Exception ex)
            {
                throw new Exception($"Yorumlar getirilirken hata: {ex.Message}");
            }
        }

        public async Task<List<ResultReviewDto>> GetApprovedReviewsAsync(string destinationId)
        {
            try
            {
                var reviewCollection = _mongoDatabase.GetCollection<DestinationReview>("DestinationReviews");
                var filter = Builders<DestinationReview>.Filter.And(
                    Builders<DestinationReview>.Filter.Eq(x => x.DestinationId, destinationId),
                    Builders<DestinationReview>.Filter.Eq(x => x.IsApproved, true),
                    Builders<DestinationReview>.Filter.Eq(x => x.IsDeleted, false)
                );
                var sort = Builders<DestinationReview>.Sort.Descending(x => x.CreatedDate);

                var reviews = await reviewCollection
                    .Find(filter)
                    .Sort(sort)
                    .ToListAsync();

                return _mapper.Map<List<ResultReviewDto>>(reviews);
            }
            catch (Exception ex)
            {
                throw new Exception($"Onaylı yorumlar getirilirken hata: {ex.Message}");
            }
        }

        public async Task<List<UserReviewDto>> GetUserReviewsAsync(string userId)
        {
            try
            {
                var reviewCollection = _mongoDatabase.GetCollection<DestinationReview>("DestinationReviews");
                var destinationCollection = _mongoDatabase.GetCollection<Destination>("Destinations");

                var filter = Builders<DestinationReview>.Filter.And(
                    Builders<DestinationReview>.Filter.Eq(x => x.UserId, userId),
                    Builders<DestinationReview>.Filter.Eq(x => x.IsDeleted, false)
                );

                var reviews = await reviewCollection
                    .Find(filter)
                    .SortByDescending(x => x.CreatedDate)
                    .ToListAsync();

                var userReviews = new List<UserReviewDto>();
                foreach (var review in reviews)
                {
                    var destination = await destinationCollection
                        .Find(x => x.Id == review.DestinationId)
                        .FirstOrDefaultAsync();

                    if (destination != null)
                    {
                        userReviews.Add(new UserReviewDto
                        {
                            Id = review.Id,
                            DestinationName = destination.CityCountry,
                            DestinationImageUrl = destination.ImageUrl,
                            Rating = review.Rating,
                            Title = review.Title,
                            Comment = review.Comment,
                            CreatedDate = review.CreatedDate,
                            IsApproved = review.IsApproved
                        });
                    }
                }

                return userReviews;
            }
            catch (Exception ex)
            {
                throw new Exception($"Kullanıcı yorumları getirilirken hata: {ex.Message}");
            }
        }

        public async Task<DestinationReviewSummaryDto> GetDestinationReviewSummaryAsync(string destinationId)
        {
            try
            {
                var reviewCollection = _mongoDatabase.GetCollection<DestinationReview>("DestinationReviews");
                var destinationCollection = _mongoDatabase.GetCollection<Destination>("Destinations");

                var destination = await destinationCollection
                    .Find(x => x.Id == destinationId)
                    .FirstOrDefaultAsync();

                var filter = Builders<DestinationReview>.Filter.And(
                    Builders<DestinationReview>.Filter.Eq(x => x.DestinationId, destinationId),
                    Builders<DestinationReview>.Filter.Eq(x => x.IsDeleted, false)
                );

                var allReviews = await reviewCollection.Find(filter).ToListAsync();
                var approvedReviews = allReviews.Where(x => x.IsApproved).ToList();

                // Rating dağılımı
                var ratingDistribution = new Dictionary<int, int>();
                for (int i = 1; i <= 5; i++)
                {
                    ratingDistribution[i] = approvedReviews.Count(x => x.Rating == i);
                }

                // Ortalama rating
                var averageRating = approvedReviews.Count > 0
                    ? approvedReviews.Average(x => x.Rating)
                    : 0;

                // En sık etiketler
                var topTags = new List<KeyValuePair<string, int>>();
                if (approvedReviews.Any())
                {
                    var allTags = approvedReviews
                        .Where(x => x.Tags != null)
                        .SelectMany(x => x.Tags)
                        .GroupBy(x => x)
                        .OrderByDescending(x => x.Count())
                        .Take(5)
                        .Select(x => new KeyValuePair<string, int>(x.Key, x.Count()))
                        .ToList();
                    topTags = allTags;
                }

                // En son yorumlar
                var latestReviews = approvedReviews
                    .OrderByDescending(x => x.CreatedDate)
                    .Take(5)
                    .ToList();

                return new DestinationReviewSummaryDto
                {
                    DestinationId = destinationId,
                    CityCountry = destination?.CityCountry ?? "Bilinmiyor",
                    AverageRating = Math.Round(averageRating, 2),
                    TotalReviews = allReviews.Count,
                    ApprovedReviews = approvedReviews.Count,
                    PendingReviews = allReviews.Count(x => !x.IsApproved),
                    RatingDistribution = ratingDistribution,
                    TopTags = topTags,
                    LatestReviews = _mapper.Map<List<ResultReviewDto>>(latestReviews)
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Yorum özeti getirilirken hata: {ex.Message}");
            }
        }

        public async Task<ReviewStatsDto> GetReviewStatsAsync()
        {
            try
            {
                var reviewCollection = _mongoDatabase.GetCollection<DestinationReview>("DestinationReviews");

                var filter = Builders<DestinationReview>.Filter.Eq(x => x.IsDeleted, false);
                var allReviews = await reviewCollection.Find(filter).ToListAsync();
                var approvedReviews = allReviews.Where(x => x.IsApproved).ToList();

                var thisMonthReviews = allReviews.Count(x =>
                    x.CreatedDate.Month == DateTime.UtcNow.Month &&
                    x.CreatedDate.Year == DateTime.UtcNow.Year);

                var destinationsWithReviews = allReviews
                    .Select(x => x.DestinationId)
                    .Distinct()
                    .Count();

                return new ReviewStatsDto
                {
                    TotalReviews = allReviews.Count,
                    ApprovedReviews = approvedReviews.Count,
                    PendingReviews = allReviews.Count(x => !x.IsApproved),
                    AverageRating = approvedReviews.Count > 0
                        ? Math.Round(approvedReviews.Average(x => x.Rating), 2)
                        : 0,
                    ReviewsThisMonth = thisMonthReviews,
                    DestinationsWithReviews = destinationsWithReviews
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"İstatistikler getirilirken hata: {ex.Message}");
            }
        }

        public async Task<List<ResultReviewDto>> GetPendingReviewsAsync()
        {
            try
            {
                var reviewCollection = _mongoDatabase.GetCollection<DestinationReview>("DestinationReviews");
                var filter = Builders<DestinationReview>.Filter.And(
                    Builders<DestinationReview>.Filter.Eq(x => x.IsApproved, false),
                    Builders<DestinationReview>.Filter.Eq(x => x.IsDeleted, false)
                );

                var reviews = await reviewCollection
                    .Find(filter)
                    .SortByDescending(x => x.CreatedDate)
                    .ToListAsync();

                return _mapper.Map<List<ResultReviewDto>>(reviews);
            }
            catch (Exception ex)
            {
                throw new Exception($"Bekleyen yorumlar getirilirken hata: {ex.Message}");
            }
        }

        public async Task ApproveReviewAsync(ApproveReviewDto approveDto)
        {
            try
            {
                var reviewCollection = _mongoDatabase.GetCollection<DestinationReview>("DestinationReviews");

                var update = Builders<DestinationReview>.Update
                    .Set(x => x.IsApproved, approveDto.Approve)
                    .Set(x => x.ApprovedDate, approveDto.Approve ? DateTime.UtcNow : null)
                    .Set(x => x.AdminNotes, approveDto.AdminNotes);

                await reviewCollection.UpdateOneAsync(
                    Builders<DestinationReview>.Filter.Eq(x => x.Id, approveDto.ReviewId),
                    update
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Yorum onaylanırken hata: {ex.Message}");
            }
        }

        public async Task RejectReviewAsync(string reviewId, string reason)
        {
            try
            {
                var reviewCollection = _mongoDatabase.GetCollection<DestinationReview>("DestinationReviews");

                var update = Builders<DestinationReview>.Update
                    .Set(x => x.IsDeleted, true)
                    .Set(x => x.AdminNotes, $"Reddedildi: {reason}");

                await reviewCollection.UpdateOneAsync(
                    Builders<DestinationReview>.Filter.Eq(x => x.Id, reviewId),
                    update
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Yorum reddedilirken hata: {ex.Message}");
            }
        }

        public async Task MarkHelpfulAsync(string reviewId)
        {
            try
            {
                var reviewCollection = _mongoDatabase.GetCollection<DestinationReview>("DestinationReviews");

                var update = Builders<DestinationReview>.Update
                    .Inc(x => x.HelpfulCount, 1);

                await reviewCollection.UpdateOneAsync(
                    Builders<DestinationReview>.Filter.Eq(x => x.Id, reviewId),
                    update
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Faydalı işaretlenirken hata: {ex.Message}");
            }
        }

        public async Task<List<ResultReviewDto>> GetReviewsByRatingAsync(string destinationId, int rating)
        {
            try
            {
                var reviewCollection = _mongoDatabase.GetCollection<DestinationReview>("DestinationReviews");
                var filter = Builders<DestinationReview>.Filter.And(
                    Builders<DestinationReview>.Filter.Eq(x => x.DestinationId, destinationId),
                    Builders<DestinationReview>.Filter.Eq(x => x.Rating, rating),
                    Builders<DestinationReview>.Filter.Eq(x => x.IsApproved, true),
                    Builders<DestinationReview>.Filter.Eq(x => x.IsDeleted, false)
                );

                var reviews = await reviewCollection
                    .Find(filter)
                    .SortByDescending(x => x.HelpfulCount)
                    .ToListAsync();

                return _mapper.Map<List<ResultReviewDto>>(reviews);
            }
            catch (Exception ex)
            {
                throw new Exception($"Rating'e göre yorumlar getirilirken hata: {ex.Message}");
            }
        }
    }
}