using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadooTravel.Entity.Entities
{
    public class PageViewTrack:BaseEntity
    {
     
        public string PageUrl { get; set; }
        public string PageName { get; set; }

        public string UserId { get; set; } // Anonim ise null
        public string SessionId { get; set; }
        public string UserAgent { get; set; }
        public string IpAddress { get; set; }

        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
        public int DurationSeconds { get; set; } // Sayfada ne kadar kaldı

        public string ReferrerUrl { get; set; }
    }
}
