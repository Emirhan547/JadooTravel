using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Entity.Entities
{
    public class FAQ:BaseEntity
    {
       
        public string Question { get; set; }
        public string Answer { get; set; }
        public string Category { get; set; }

        public int HelpfulCount { get; set; } = 0;
        public int UnhelpfulCount { get; set; } = 0;

        public int Priority { get; set; } = 0

;
        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
    }
}
