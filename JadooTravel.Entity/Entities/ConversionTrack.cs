using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Entity.Entities
{
    public class ConversionTrack:BaseEntity
    {   

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        public string ConversionType { get; set; } // "booking", "review", "favorite"
        public string ConversionValue { get; set; } // Destinasyon ID vs

        public decimal Amount { get; set; } // Eğer para ile ilgiliyse

        public DateTime ConvertedAt { get; set; } = DateTime.UtcNow;

        [BsonIgnore]
        public AppUser User { get; set; }
    }
}
