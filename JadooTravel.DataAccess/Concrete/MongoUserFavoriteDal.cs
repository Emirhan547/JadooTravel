using JadooTravel.DataAccess.Abstract;
using JadooTravel.DataAccess.Context;
using JadooTravel.Entity.Entities;
using MongoDB.Driver;

namespace JadooTravel.DataAccess.Concrete
{
    public class MongoUserFavoriteDal : MongoGenericDal<UserFavorite>, IUserFavoriteDal
    {
        private readonly IMongoCollection<UserFavorite> _favoriteCollection;

        public MongoUserFavoriteDal(AppDbContext context, IMongoDatabase mongoDatabase) : base(context)
        {
            _favoriteCollection = mongoDatabase.GetCollection<UserFavorite>("UserFavorites");
        }

        public async Task<bool> ExistsAsync(string userId, string destinationId)
        {
            var filter = Builders<UserFavorite>.Filter.And(
                Builders<UserFavorite>.Filter.Eq(x => x.UserId, userId),
                Builders<UserFavorite>.Filter.Eq(x => x.DestinationId, destinationId));

            return await _favoriteCollection.Find(filter).AnyAsync();
        }

        public async Task<long> CountByUserIdAsync(string userId)
        {
            return await _favoriteCollection.CountDocumentsAsync(x => x.UserId == userId);
        }

        public async Task<bool> DeleteByIdAndUserIdAsync(string favoriteId, string userId)
        {
            var filter = Builders<UserFavorite>.Filter.And(
                Builders<UserFavorite>.Filter.Eq(x => x.Id, favoriteId),
                Builders<UserFavorite>.Filter.Eq(x => x.UserId, userId));

            var result = await _favoriteCollection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }

        public async Task<List<UserFavorite>> GetByUserIdAsync(string userId)
        {
            return await _favoriteCollection
                .Find(x => x.UserId == userId)
                .SortByDescending(x => x.CreatedDate)
                .ToListAsync();
        }
        public async Task<bool> DeleteByUserIdAndDestinationIdAsync(string userId, string destinationId)
        {
            var filter = Builders<UserFavorite>.Filter.And(
                Builders<UserFavorite>.Filter.Eq(x => x.UserId, userId),
                Builders<UserFavorite>.Filter.Eq(x => x.DestinationId, destinationId));

            var result = await _favoriteCollection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }
    }
}