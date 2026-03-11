using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace runSyncBackend.Models
{
    public class TrainingResult
    {
        [BsonId]
        [BsonSerializer(typeof(StringOrObjectIdSerializer))]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("trainingId")]
        public string TrainingId { get; set; } = string.Empty;

        [BsonElement("athleteId")]
        public string AthleteId { get; set; } = string.Empty;

        [BsonElement("status")]
        public string Status { get; set; } = "completed"; // completed | skipped | partial

        [BsonElement("completedAt")]
        public DateTime? CompletedAt { get; set; }

        [BsonElement("distance")]
        public double? Distance { get; set; }

        [BsonElement("durationMinutes")]
        public int? DurationMinutes { get; set; }

        [BsonElement("pace")]
        public string? Pace { get; set; }

        [BsonElement("rpe")]
        public int? Rpe { get; set; } // 1-10

        [BsonElement("notes")]
        public string? Notes { get; set; }

        [BsonElement("coachGrade")]
        public int? CoachGrade { get; set; } // 1-10

        [BsonElement("coachNotes")]
        public string? CoachNotes { get; set; }

        [BsonElement("improvementAreas")]
        public string? ImprovementAreas { get; set; }
    }
}
