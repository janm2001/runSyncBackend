using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace runSyncBackend.Models
{
    public class Group
    {
        [BsonId]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("members")]
        public int Members { get; set; }

        [BsonElement("avgPace")]
        public string AvgPace { get; set; } = string.Empty;

        [BsonElement("level")]
        public string Level { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;
    }
}

