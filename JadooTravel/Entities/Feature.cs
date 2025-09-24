using MongoDB.Bson.Serialization.Attributes;

namespace JadooTravel.Entities
{
    public class Feature
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string FeatureId { get; set; }
        public string Title { get; set; }
        public string MainTitle { get; set; }
        public string Description { get; set; }
        public string VideoUrl { get; set; }


    }
}
