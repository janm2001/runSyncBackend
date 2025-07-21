using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace runSyncBackend.Models
{
     public class MonthlyProgress
    {
        [BsonId]
        public string Id { get; set; }

        [BsonElement("month")]
        public string Month { get; set; } = string.Empty;

        [BsonElement("beginner")]
        public int Beginner { get; set; }

        [BsonElement("intermediate")]
        public int Intermediate { get; set; }

        [BsonElement("advanced")]
        public int Advanced { get; set; }
    }

}