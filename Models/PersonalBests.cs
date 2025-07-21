
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace runSyncBackend.Models
{
    public class PersonalBests
    {
        [BsonElement("5K")]
        public string FiveK { get; set; } = string.Empty;

        [BsonElement("10K")]
        public string TenK { get; set; } = string.Empty;

        [BsonElement("halfMarathon")]
        public string HalfMarathon { get; set; } = string.Empty;

    }
}

