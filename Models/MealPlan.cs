using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace ToxicFitnessAPI.Models
{
    public class Meal
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string? Id { get; set; }

        public string Name { get; set; }
        public int Calories { get; set; }
        public int ProteinG { get; set; }
        public int CarbsG { get; set; }
        public int FatsG { get; set; }
        public List<string> Ingredients { get; set; } = new();
    }

    public class MealDay
    {
        public string DayName { get; set; }
        public List<Meal> Meals { get; set; } = new();
    }

    public class MealPlan
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string? Id { get; set; }

        public string? UserId { get; set; }

        public string Name { get; set; } = "Custom Meal Plan";

        public List<Meal> Meals { get; set; } = new();

        public List<MealDay> Days { get; set; } = new();

        public double DailyCaloriesTarget { get; set; }
        public double TotalCalories { get; set; }
    }
}
