using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Entity.Entities
{
    public class UserFavorite
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string DestinationId { get; set; }

        public string CityCountry { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [BsonIgnore]
        public AppUser User { get; set; }

        [BsonIgnore]
        public Destination Destination { get; set; }
    }
}
