using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.DataAccess.Abstract;
using JadooTravel.Dto.Dtos.BookingDtos;
using JadooTravel.Entity.Entities;
using MongoDB.Bson;

namespace JadooTravel.Business.Concrete
{
    public class BookingManager (IBookingDal _bookingDal, IMapper _mapper) : IBookingService
    {

        public async Task CreateAsync(CreateBookingDto create)
        {
            var booking = _mapper.Map<Booking>(create);
            await _bookingDal.CreateAsync(booking);
        }

        public async Task DeleteAsync(ObjectId id)
        {
            await _bookingDal.DeleteAsync(id);
        }

        public async Task<List<ResultBookingDto>> GetAllAsync()
        {
            var bookings = await _bookingDal.GetAllAsync();
            return _mapper.Map<List<ResultBookingDto>>(bookings);
        }

        public async Task<ResultBookingDto> GetByIdAsync(ObjectId id)
        {
            var booking = await _bookingDal.GetByIdAsync(id);
            return _mapper.Map<ResultBookingDto>(booking);
        }

        public async Task UpdateAsync(UpdateBookingDto update)
        {
            var booking = _mapper.Map<Booking>(update);
            await _bookingDal.UpdateAsync(booking);
        }
    }
}
