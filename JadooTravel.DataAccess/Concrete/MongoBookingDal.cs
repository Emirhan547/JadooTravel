using JadooTravel.DataAccess.Abstract;
using JadooTravel.DataAccess.Context;
using JadooTravel.Entity.Entities;
using JadooTravel.Entity.Entities.Enums;
using MongoDB.Driver;

namespace JadooTravel.DataAccess.Concrete
{
    public class MongoBookingDal
        : MongoGenericDal<Booking>, IBookingDal
    {
        private readonly IMongoCollection<Booking> _bookingCollection;
        public MongoBookingDal(AppDbContext context)
            : base(context)
        {
            _bookingCollection = context.GetCollection<Booking>();
        }

        public async Task<List<Booking>> GetByUserIdAsync(string userId)
        {
            return await _bookingCollection
                .Find(x => x.UserId == userId)
                .SortByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task<bool> CancelBookingAsync(string bookingId, string userId)
        {
            var filter = Builders<Booking>.Filter.And(
                Builders<Booking>.Filter.Eq(x => x.Id, bookingId),
                Builders<Booking>.Filter.Eq(x => x.UserId, userId)
            );

            var update = Builders<Booking>.Update
                .Set(x => x.Status, BookingStatus.Cancelled)
                .Set(x => x.UpdatedDate, DateTime.UtcNow);

            var result = await _bookingCollection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateBookingStatusAsync(string bookingId, BookingStatus status, string? notes)
        {
            var update = Builders<Booking>.Update
                .Set(x => x.Status, status)
                .Set(x => x.UpdatedDate, DateTime.UtcNow)
                .Set(x => x.AdminNotes, notes);

            if (status == BookingStatus.Approved)
                update = update.Set(x => x.ApprovedDate, DateTime.UtcNow);

            var result = await _bookingCollection.UpdateOneAsync(
                Builders<Booking>.Filter.Eq(x => x.Id, bookingId),
                update
            );

            return result.ModifiedCount > 0;
        }

        public async Task<int> GetPendingBookingsCountAsync()
        {
            var filter = Builders<Booking>.Filter.Eq(x => x.Status, BookingStatus.Pending);
            var count = await _bookingCollection.CountDocumentsAsync(filter);
            return (int)count;
        }

        public async Task<List<Booking>> GetByStatusAsync(BookingStatus status)
        {
            return await _bookingCollection
                .Find(x => x.Status == status)
                .SortByDescending(x => x.CreatedDate)
                .ToListAsync();
        }
    }
}