using JadooTravel.Entity.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Dto.Dtos.BookingDtos
{
    public class UserBookingDto
    {
        public string Id { get; set; }
        public string DestinationCityCountry { get; set; }
        public string DestinationImageUrl { get; set; }
        public string DestinationId { get; set; }
        public bool IsFavorite { get; set; }
        public BookingStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal TotalPrice { get; set; }
        public int PersonCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string StatusDisplay => Status switch
        {
            BookingStatus.Pending => "Beklemede",
            BookingStatus.Approved => "Onaylandı",
            BookingStatus.Rejected => "Reddedildi",
            BookingStatus.Cancelled => "İptal Edildi",
            _ => "Bilinmeyen"
        };

        public string StatusColor => Status switch
        {
            BookingStatus.Pending => "warning",
            BookingStatus.Approved => "success",
            BookingStatus.Rejected => "danger",
            BookingStatus.Cancelled => "secondary",
            _ => "info"
        };
    }
}
