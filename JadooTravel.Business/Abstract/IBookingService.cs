using JadooTravel.Dto.Dtos.BookingDtos;


namespace JadooTravel.Business.Abstract
{
    public interface IBookingService : IGenericService<ResultBookingDto, CreateBookingDto, UpdateBookingDto>
    {
    }
}
