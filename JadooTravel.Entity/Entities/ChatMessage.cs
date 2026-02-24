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
    public class ChatMessage:BaseEntity
    {
       

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        public string UserName { get; set; }
        public string UserEmail { get; set; }

        public string Message { get; set; }
        public string Response { get; set; }

        public ChatCategory Category { get; set; }

        public bool IsAutomatic { get; set; } = true;

        // Operatör cevapsa, hangi operatör?
        public string OperatorId { get; set; }
        public string OperatorName { get; set; }

        public int? Satisfaction { get; set; } // 1-5
        public string SatisfactionComment { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? RespondedDate { get; set; }

        [BsonIgnore]
        public AppUser User { get; set; }
    }
}
