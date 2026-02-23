using JadooTravel.Entity.Entities.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Entity.Entities
{
    public class Booking:BaseEntity 
    {
    

        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string MessageBody { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string DestinationId { get; set; }

        public string DestinationCityCountry { get; set; }
        public string DestinationImageUrl { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        public decimal TotalPrice { get; set; }
        public int PersonCount { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }

        public string AdminNotes { get; set; }

        [BsonIgnore]
        public AppUser User { get; set; }

        [BsonIgnore]
        public Destination Destination { get; set; }
    }
}
