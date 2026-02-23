using JadooTravel.Dto.Dtos.BookingDtos;
using JadooTravel.Entity.Entities.Enums;


namespace JadooTravel.Business.Abstract
{
    public interface IBookingService : IGenericService<ResultBookingDto, CreateBookingDto, UpdateBookingDto>
    {
        Task<List<UserBookingDto>> GetUserBookingsAsync(string userId);
        Task<ResultBookingDto> GetBookingDetailsAsync(string id);
        Task<bool> CancelBookingAsync(string bookingId, string userId);
        Task<bool> UpdateBookingStatusAsync(UpdateBookingStatusDto updateStatusDto);
        Task<int> GetPendingBookingsCountAsync();
        Task<List<ResultBookingDto>> GetBookingsByStatusAsync(BookingStatus status);
    }
}
