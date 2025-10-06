using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace ToxicFitnessAPI.Models
{
    public class Workout
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }  //optional

        public string Name { get; set; }
        public string Type { get; set; } // e.g. "Strength", "Cardio", etc.
        public string Difficulty { get; set; } // "Beginner", "Intermediate", "Advanced"
        public int DurationMinutes { get; set; }
        public List<string> Exercises { get; set; } = new();
    }
}
