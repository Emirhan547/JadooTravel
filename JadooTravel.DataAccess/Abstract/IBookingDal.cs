using JadooTravel.Entity.Entities;
using JadooTravel.Entity.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.DataAccess.Abstract
{
    public interface IBookingDal:IGenericDal<Booking>
    {
        Task<List<Booking>> GetByUserIdAsync(string userId);
        Task<bool> CancelBookingAsync(string bookingId, string userId);
        Task<bool> UpdateBookingStatusAsync(string bookingId, BookingStatus status, string? notes);
        Task<int> GetPendingBookingsCountAsync();
        Task<List<Booking>> GetByStatusAsync(BookingStatus status);
        Task<long> CountByUserIdAsync(string userId);
    }
}
