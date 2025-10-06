using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ToxicFitnessAPI.Models
{
    public class Exercise
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }
        public string MuscleGroup { get; set; }   // chest, back, biceps, legs, shoulders, abs, triceps
        public string Equipment { get; set; }     // none, dumbbell, barbell, machine
        public string Difficulty { get; set; }    // beginner, intermediate, advanced
        public string VideoUrl { get; set; }      // optional
    }
}
