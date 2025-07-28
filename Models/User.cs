using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace runSyncBackend.Models
{
    
    public enum UserRole
    {
        Client,
        Coach
    }
     public class User
    {
        [BsonId]
        public string Id { get; set; }

        [BsonElement("firstname")]
        public string FirstName { get; set; } = string.Empty;

        [BsonElement("lastname")]
        public string LastName { get; set; } = string.Empty;

        [BsonElement("Email")]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("PasswordHash")]
        [Required]
        public string PasswordHash { get; set; } = string.Empty;    

        [BsonElement("Role")]
        public UserRole Role { get; set; }

        // Role-specific information
        [BsonElement("ClientInfo")]
        public Athlete ClientInfo { get; set; } = new Athlete(); // Null if the user is a Coach

        [BsonElement("CoachInfo")]
        public Athlete CoachInfo { get; set; } = new Athlete();  // Null if the user is a Client


    }
}

