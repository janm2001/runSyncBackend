using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace runSyncBackend.Models
{
     public class Athlete
    {
        [BsonId]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("group")]
        public string Group { get; set; } = string.Empty;

        [BsonElement("joinDate")]
        public string JoinDate { get; set; } = string.Empty;

        [BsonElement("performance")]
        public int Performance { get; set; }

        [BsonElement("improvement")]
        public string Improvement { get; set; } = string.Empty;

        [BsonElement("lastRun")]
        public string LastRun { get; set; } = string.Empty;

        [BsonElement("personalBests")]
        public PersonalBests PersonalBests { get; set; } =  new PersonalBests();

        [BsonElement("attendance")]
        public int Attendance { get; set; }
    }
}

