using JadooTravel.Entity.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Dto.Dtos.BookingDtos
{
    public class UpdateBookingStatusDto
    {
        public string BookingId { get; set; }
        public BookingStatus Status { get; set; }
        public string Notes { get; set; }
    }
}
