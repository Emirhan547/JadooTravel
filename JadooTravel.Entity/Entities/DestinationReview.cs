using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Entity.Entities
{
    public class DestinationReview:BaseEntity
    {
   

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string DestinationId { get; set; }

        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserProfileImageUrl { get; set; }

        // Rating (1-5 yıldız)
        public int Rating { get; set; }

        // Yorum başlığı
        public string Title { get; set; }

        // Detaylı yorum
        public string Comment { get; set; }

        // Tarafından Onaylandı
        public bool IsApproved { get; set; } = false;
        public DateTime? ApprovedDate { get; set; }

        // Yararlı mı?
        public int HelpfulCount { get; set; } = 0;

        // Ziyaretçi sayısı
        public int VisitedDays { get; set; } // Kaç gün kaldı

        // Etiketler
        public List<string> Tags { get; set; } = new List<string>();
        // Örn: "ailecil", "maceraperest", "lüks", "bütçeli"

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }

        // Admin notları
        public string AdminNotes { get; set; }
        public bool IsDeleted { get; set; } = false;

        [BsonIgnore]
        public AppUser User { get; set; }

        [BsonIgnore]
        public Destination Destination { get; set; }
    }
}
