using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace ToxicFitnessAPI.Models
{
    public class UserGoal
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string? Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; }
        public int Age { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public string Gender { get; set; }
        public string Goal { get; set; }

        public string ActivityLevel { get; set; } = "moderate";
        public string PreferredLocation { get; set; } = "gym";
        public List<string> Allergies { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
