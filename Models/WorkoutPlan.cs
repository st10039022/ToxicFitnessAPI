using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace ToxicFitnessAPI.Models
{
    [BsonIgnoreExtraElements] // tolerate unknown fields
    public class WorkoutPlan
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string? Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public string GoalType { get; set; } = string.Empty;   // bulking, cutting, toning, strength, etc.
        public List<WorkoutDay> Days { get; set; } = new();
    }

    [BsonIgnoreExtraElements] // tolerate unknown fields
    public class WorkoutDay
    {
        public string DayName { get; set; } = string.Empty;

        // keep legacy name to match existing docs
        public List<WorkoutItem> Items { get; set; } = new();

        // new structured sections (these will be empty on old docs and that's fine)
        public List<string> Warmup { get; set; } = new();
        public List<string> Cooldown { get; set; } = new();
    }

    public class WorkoutItem
    {
        public string ExerciseId { get; set; } = string.Empty; // optional link to Exercise collection
        public string Name { get; set; } = string.Empty;
        public int Sets { get; set; }
        public string Reps { get; set; } = "12";
        public int RestSeconds { get; set; } = 60;
    }
}
