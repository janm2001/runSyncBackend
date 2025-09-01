using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace runSyncBackend.Models
{
    public class Athlete
    {
        [BsonId] 
        [BsonRepresentation(BsonType.ObjectId)] 
        public string Id { get; set; } = string.Empty;


        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("group")]
        public string Group { get; set; } = string.Empty;

        [BsonElement("joinDate")]
        public string JoinDate { get; set; } = string.Empty;

        [BsonElement("isActiveAthlete")]
        public bool IsActiveAthlete { get; set; }

        [BsonElement("attendance")]
        public int Attendance { get; set; }
        
         [BsonElement("performance")]
        public int? Performance { get; set; }

        [BsonElement("improvement")]
        public string? Improvement { get; set; } = string.Empty;

        [BsonElement("lastRun")]
        public string? LastRun { get; set; } = string.Empty;

        [BsonElement("personalBests")]
        public PersonalBests? PersonalBests { get; set; } =  new PersonalBests();
    }
}

