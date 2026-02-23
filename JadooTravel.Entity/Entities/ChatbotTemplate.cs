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
    public class ChatbotTemplate:BaseEntity
    {
     

        public string KeywordPattern { get; set; } // Regex pattern
        public string Response { get; set; }
        public ChatCategory Category { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
