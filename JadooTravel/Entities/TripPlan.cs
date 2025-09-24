using MongoDB.Bson.Serialization.Attributes;

namespace JadooTravel.Entities
{
    public class TripPlan
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string TripPlanId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }

    }
}
