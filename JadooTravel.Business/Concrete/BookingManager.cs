using AutoMapper;
using JadooTravel.Business.Abstract;
using JadooTravel.DataAccess.Abstract;
using JadooTravel.Dto.Dtos.BookingDtos;
using JadooTravel.Entity.Entities;
using JadooTravel.Entity.Entities.Enums;


namespace JadooTravel.Business.Concrete
{
    public class BookingManager(IBookingDal _repository, IMapper _mapper) : IBookingService
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
            var bookings = await _repository.GetByUserIdAsync(userId);
            return _mapper.Map<List<UserBookingDto>>(bookings);
        }

        public async Task<ResultBookingDto> GetBookingDetailsAsync(string id)
        {
            var booking = await _repository.GetByIdAsync(id);
            return _mapper.Map<ResultBookingDto>(booking);
        }

        public async Task<bool> CancelBookingAsync(string bookingId, string userId)
        {
            return await _repository.CancelBookingAsync(bookingId, userId);
        }

        public async Task<bool> UpdateBookingStatusAsync(UpdateBookingStatusDto updateStatusDto)
        {
            return await _repository.UpdateBookingStatusAsync(
                 updateStatusDto.BookingId,
                 updateStatusDto.Status,
                 updateStatusDto.Notes);
        }

        public async Task<int> GetPendingBookingsCountAsync()
        {
            return await _repository.GetPendingBookingsCountAsync();
        }

        public async Task<List<ResultBookingDto>> GetBookingsByStatusAsync(BookingStatus status)
        {
            var bookings = await _repository.GetByStatusAsync(status);
            return _mapper.Map<List<ResultBookingDto>>(bookings);
        }
    }
}