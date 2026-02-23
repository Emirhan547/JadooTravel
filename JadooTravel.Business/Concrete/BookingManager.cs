

using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.DataAccess.Abstract;
using JadooTravel.Dto.Dtos.BookingDtos;
using JadooTravel.Entity.Entities;
using JadooTravel.Entity.Entities.Enums;
using MongoDB.Driver;

namespace JadooTravel.Business.Concrete
{
    public class BookingManager(IBookingDal _repository, IMapper _mapper, IMongoClient _mongoClient, IMongoDatabase _mongoDatabase) : IBookingService
    {


        public async Task CreateAsync(CreateBookingDto create)
        {
            var booking = _mapper.Map<Booking>(create);
            await _repository.CreateAsync(booking);
        }

        public async Task DeleteAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<List<ResultBookingDto>> GetAllAsync()
        {
            var bookings = await _repository.GetAllAsync();
            return _mapper.Map<List<ResultBookingDto>>(bookings);
        }
        public async Task UpdateAsync(UpdateBookingDto update)
        {
            var booking = _mapper.Map<Booking>(update);
            await _repository.UpdateAsync(booking);
        }
        public async Task<UpdateBookingDto> GetByIdAsync(string id)
        {
            var booking = await _repository.GetByIdAsync(id);
            return _mapper.Map<UpdateBookingDto>(booking);
        }
        public async Task<List<UserBookingDto>> GetUserBookingsAsync(string userId)
        {
            try
            {
                var bookingCollection = _mongoDatabase.GetCollection<Booking>("Bookings");
                var filter = Builders<Booking>.Filter.Eq(x => x.UserId, userId);
                var sort = Builders<Booking>.Sort.Descending(x => x.CreatedDate);

                var bookings = await bookingCollection
                    .Find(filter)
                    .Sort(sort)
                    .ToListAsync();

                return _mapper.Map<List<UserBookingDto>>(bookings);
            }
            catch (Exception ex)
            {
                throw new Exception($"Kullanıcı rezervasyonları yüklenirken hata: {ex.Message}");
            }
        }

        public async Task<ResultBookingDto> GetBookingDetailsAsync(string id)
        {
            var booking = await _repository.GetByIdAsync(id);
            return _mapper.Map<ResultBookingDto>(booking);
        }

        public async Task<bool> CancelBookingAsync(string bookingId, string userId)
        {
            try
            {
                var bookingCollection = _mongoDatabase.GetCollection<Booking>("Bookings");

                var filter = Builders<Booking>.Filter.And(
                    Builders<Booking>.Filter.Eq(x => x.Id, bookingId),
                    Builders<Booking>.Filter.Eq(x => x.UserId, userId)
                );

                var update = Builders<Booking>.Update
                    .Set(x => x.Status, BookingStatus.Cancelled)
                    .Set(x => x.UpdatedDate, DateTime.UtcNow);

                var result = await bookingCollection.UpdateOneAsync(filter, update);
                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Rezervasyon iptal edilirken hata: {ex.Message}");
            }
        }

        public async Task<bool> UpdateBookingStatusAsync(UpdateBookingStatusDto updateStatusDto)
        {
            try
            {
                var bookingCollection = _mongoDatabase.GetCollection<Booking>("Bookings");

                var update = Builders<Booking>.Update
                    .Set(x => x.Status, updateStatusDto.Status)
                    .Set(x => x.UpdatedDate, DateTime.UtcNow)
                    .Set(x => x.AdminNotes, updateStatusDto.Notes);

                if (updateStatusDto.Status == BookingStatus.Approved)
                    update = update.Set(x => x.ApprovedDate, DateTime.UtcNow);

                var result = await bookingCollection.UpdateOneAsync(
                    Builders<Booking>.Filter.Eq(x => x.Id, updateStatusDto.BookingId),
                    update
                );

                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Rezervasyon durumu güncellenirken hata: {ex.Message}");
            }
        }

        public async Task<int> GetPendingBookingsCountAsync()
        {
            try
            {
                var bookingCollection = _mongoDatabase.GetCollection<Booking>("Bookings");
                var filter = Builders<Booking>.Filter.Eq(x => x.Status, BookingStatus.Pending);
                var count = await bookingCollection.CountDocumentsAsync(filter);
                return (int)count;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<List<ResultBookingDto>> GetBookingsByStatusAsync(BookingStatus status)
        {
            try
            {
                var bookingCollection = _mongoDatabase.GetCollection<Booking>("Bookings");
                var filter = Builders<Booking>.Filter.Eq(x => x.Status, status);
                var sort = Builders<Booking>.Sort.Descending(x => x.CreatedDate);

                var bookings = await bookingCollection
                    .Find(filter)
                    .Sort(sort)
                    .ToListAsync();

                return _mapper.Map<List<ResultBookingDto>>(bookings);
            }
            catch (Exception ex)
            {
                throw new Exception($"Duruma göre rezervasyonlar yüklenirken hata: {ex.Message}");
            }
        }
    }
}