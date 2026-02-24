using JadooTravel.DataAccess.Abstract;
using JadooTravel.DataAccess.Context;
using JadooTravel.Entity.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace JadooTravel.DataAccess.Concrete
{
    public class MongoReviewDal : MongoGenericDal<DestinationReview>, IReviewDal
    {
        private readonly IMongoCollection<DestinationReview> _reviewCollection;

        public MongoReviewDal(AppDbContext context, IMongoDatabase mongoDatabase) : base(context)
        {
            _reviewCollection = context.GetCollection<DestinationReview>();
        }

        public async Task<List<DestinationReview>> GetByDestinationAsync(string destinationId, bool onlyApproved = false)
        {
            var filter = Builders<DestinationReview>.Filter.And(
                Builders<DestinationReview>.Filter.Eq(x => x.DestinationId, destinationId),
                Builders<DestinationReview>.Filter.Eq(x => x.IsDeleted, false));

            if (onlyApproved)
            {
                filter = Builders<DestinationReview>.Filter.And(
                    filter,
                    Builders<DestinationReview>.Filter.Eq(x => x.IsApproved, true));
            }

            return await _reviewCollection
                .Find(filter)
                .SortByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<DestinationReview>> GetByUserAsync(string userId)
        {
            var filter = Builders<DestinationReview>.Filter.And(
                Builders<DestinationReview>.Filter.Eq(x => x.UserId, userId),
                Builders<DestinationReview>.Filter.Eq(x => x.IsDeleted, false));

            return await _reviewCollection
                .Find(filter)
                .SortByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<DestinationReview>> GetPendingAsync()
        {
            var filter = Builders<DestinationReview>.Filter.And(
                Builders<DestinationReview>.Filter.Eq(x => x.IsApproved, false),
                Builders<DestinationReview>.Filter.Eq(x => x.IsDeleted, false));

            return await _reviewCollection
                .Find(filter)
                .SortByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<DestinationReview>> GetByDestinationAndRatingAsync(string destinationId, int rating)
        {
            var filter = Builders<DestinationReview>.Filter.And(
                Builders<DestinationReview>.Filter.Eq(x => x.DestinationId, destinationId),
                Builders<DestinationReview>.Filter.Eq(x => x.Rating, rating),
                Builders<DestinationReview>.Filter.Eq(x => x.IsApproved, true),
                Builders<DestinationReview>.Filter.Eq(x => x.IsDeleted, false));

            return await _reviewCollection
                .Find(filter)
                .SortByDescending(x => x.HelpfulCount)
                .ToListAsync();
        }

        public async Task SoftDeleteAsync(string reviewId)
        {
            var update = Builders<DestinationReview>.Update.Set(x => x.IsDeleted, true);
            await _reviewCollection.UpdateOneAsync(x => x.Id == reviewId, update);
        }


        public async Task ApproveAsync(string reviewId, bool approve, string? adminNotes) { var update = Builders<DestinationReview>.Update.Set(x => x.IsApproved, approve).Set(x => x.ApprovedDate, approve ? DateTime.UtcNow : null).Set(x => x.AdminNotes, adminNotes); await _reviewCollection.UpdateOneAsync(x => x.Id == reviewId, update); }

        public async Task RejectAsync(string reviewId, string reason)
        {
            var update = Builders<DestinationReview>.Update
                .Set(x => x.IsDeleted, true)
                .Set(x => x.AdminNotes, $"Reddedildi: {reason}");

            await _reviewCollection.UpdateOneAsync(x => x.Id == reviewId, update);
        }

        public async Task MarkHelpfulAsync(string reviewId)
        {
            var update = Builders<DestinationReview>.Update.Inc(x => x.HelpfulCount, 1);
            await _reviewCollection.UpdateOneAsync(x => x.Id == reviewId, update);
        }
    }
}