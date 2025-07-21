using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace runSyncBackend.Models
{
    public class Intervals {
    [BsonElement("distance")]
    public string Distance { get; set; } = string.Empty;

    [BsonElement("repetitions")]
    public int Repetitions { get; set; } 

    [BsonElement("rest")]
    public string Rest { get; set; } = string.Empty;

    [BsonElement("targetPace")]
    public string TargetPace { get; set; } = string.Empty;
}
 }