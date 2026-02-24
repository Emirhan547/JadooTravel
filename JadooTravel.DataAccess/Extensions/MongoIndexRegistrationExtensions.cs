using JadooTravel.Entity.Entities;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace JadooTravel.DataAccess.Extensions
{
    public static class MongoIndexRegistrationExtensions
    {
        public static async Task EnsureMongoIndexesAsync(this IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();

            var bookingCollection = database.GetCollection<Booking>("Booking");
            await bookingCollection.Indexes.CreateManyAsync(new[]
            {
                new CreateIndexModel<Booking>(Builders<Booking>.IndexKeys.Ascending(x => x.UserId).Descending(x => x.CreatedDate)),
                new CreateIndexModel<Booking>(Builders<Booking>.IndexKeys.Ascending(x => x.Status).Descending(x => x.CreatedDate))
            });

            var reviewCollection = database.GetCollection<DestinationReview>("DestinationReviews");
            await reviewCollection.Indexes.CreateOneAsync(
                new CreateIndexModel<DestinationReview>(
                    Builders<DestinationReview>.IndexKeys
                        .Ascending(x => x.DestinationId)
                        .Ascending(x => x.IsApproved)
                        .Ascending(x => x.IsDeleted)
                        .Descending(x => x.CreatedDate)));

            var favoriteCollection = database.GetCollection<UserFavorite>("UserFavorites");
            await favoriteCollection.Indexes.CreateOneAsync(
                new CreateIndexModel<UserFavorite>(
                    Builders<UserFavorite>.IndexKeys
                        .Ascending(x => x.UserId)
                        .Ascending(x => x.DestinationId),
                    new CreateIndexOptions { Unique = true }));

           
        }
    }
}