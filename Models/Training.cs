using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace runSyncBackend.Models
{
    public class Training
    {
        [BsonId]
         [BsonSerializer(typeof(StringOrObjectIdSerializer))]
        public string Id { get; set; }

        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;

        [BsonElement("group")]
        public string Group { get; set; } = string.Empty;

        [BsonElement("date")]
        public string Date { get; set; } = string.Empty;

        [BsonElement("duration")]
        public int Duration { get; set; }

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("intervals")]
        public List<Intervals> Intervals { get; set; } = new List<Intervals>();

        [BsonElement("attendance")]
        public List<object> Attendance { get; set; } = new List<object>();

        [BsonElement("completed")]
        public bool Completed { get; set; }

        [BsonElement("distance")]
        public int? Distance { get; set; }
    }
}