using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Dto.Dtos.BookingDtos
{
    public class GetBookingByIdDto
    {
        public ObjectId Id { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string MessageBody { get; set; }
    }
}
