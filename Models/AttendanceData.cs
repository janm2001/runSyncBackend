using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace runSyncBackend.Models
{
     public class AttendanceData
    {
        [BsonId]
        [BsonSerializer(typeof(StringOrObjectIdSerializer))]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("value")]
        public int Value { get; set; }

        [BsonElement("color")]
        public string Color { get; set; } = string.Empty;
    }
}