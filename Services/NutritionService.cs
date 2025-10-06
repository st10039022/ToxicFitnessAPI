using MongoDB.Driver;
using Microsoft.Extensions.Options;
using ToxicFitnessAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ToxicFitnessAPI.Services
{
    public class NutritionService
    {
        private readonly IMongoCollection<Meal> _meals;

        public NutritionService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            var client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            var database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _meals = database.GetCollection<Meal>("Meals");

            // Seed sample meals if collection empty
            if (!_meals.Find(_ => true).Any())
                SeedMeals();
        }

        public List<Meal> GetMeals() => _meals.Find(_ => true).ToList();

        public Meal CreateMeal(Meal meal)
        {
            _meals.InsertOne(meal);
            return meal;
        }

        public Meal UpdateMeal(string id, Meal meal)
        {
            var existing = _meals.Find(m => m.Id == id).FirstOrDefault();
            if (existing == null) return null;
            meal.Id = existing.Id;
            _meals.ReplaceOne(m => m.Id == id, meal);
            return meal;
        }

        public bool DeleteMeal(string id)
        {
            var result = _meals.DeleteOne(m => m.Id == id);
            return result.DeletedCount > 0;
        }

        // -------------------- FILTER MEALS WITHOUT ALLERGENS --------------------
        public List<Meal> GetMealsWithoutAllergens(List<string> allergies)
        {
            var all = GetMeals();
            if (allergies == null || !allergies.Any()) return all;

            return all.Where(meal =>
                !meal.Ingredients.Any(ing =>
                    allergies.Any(a => ing.Contains(a, StringComparison.OrdinalIgnoreCase))
                )
            ).ToList();
        }

        // -------------------- SEED DEFAULT MEALS --------------------
        private void SeedMeals()
        {
            var meals = new List<Meal>
            {
                // Breakfast
                new Meal { Name = "Oatmeal with Banana", Calories = 350, ProteinG = 10, CarbsG = 60, FatsG = 5, Ingredients = new() { "Oats", "Banana", "Honey" } },
                new Meal { Name = "Scrambled Eggs & Toast", Calories = 400, ProteinG = 25, CarbsG = 30, FatsG = 15, Ingredients = new() { "Eggs", "Bread", "Butter" } },
                new Meal { Name = "Greek Yogurt & Berries", Calories = 250, ProteinG = 20, CarbsG = 25, FatsG = 3, Ingredients = new() { "Yogurt", "Berries", "Honey" } },
                new Meal { Name = "Avocado Toast", Calories = 350, ProteinG = 10, CarbsG = 35, FatsG = 18, Ingredients = new() { "Avocado", "Bread", "Egg" } },
                new Meal { Name = "Protein Smoothie", Calories = 300, ProteinG = 25, CarbsG = 30, FatsG = 5, Ingredients = new() { "Protein Powder", "Milk", "Banana" } },

                // Lunch
                new Meal { Name = "Chicken and Rice Bowl", Calories = 550, ProteinG = 45, CarbsG = 55, FatsG = 10, Ingredients = new() { "Chicken", "Rice", "Peppers" } },
                new Meal { Name = "Beef Wrap", Calories = 550, ProteinG = 40, CarbsG = 45, FatsG = 20, Ingredients = new() { "Beef", "Tortilla", "Lettuce" } },
                new Meal { Name = "Tofu Stir Fry", Calories = 450, ProteinG = 30, CarbsG = 40, FatsG = 15, Ingredients = new() { "Tofu", "Soy Sauce", "Vegetables" } },
                new Meal { Name = "Turkey Sandwich", Calories = 380, ProteinG = 25, CarbsG = 40, FatsG = 8, Ingredients = new() { "Turkey", "Bread", "Lettuce" } },
                new Meal { Name = "Grilled Veggie Wrap", Calories = 420, ProteinG = 15, CarbsG = 55, FatsG = 10, Ingredients = new() { "Tortilla", "Peppers", "Zucchini" } },

                // Dinner
                new Meal { Name = "Grilled Salmon & Veggies", Calories = 600, ProteinG = 50, CarbsG = 20, FatsG = 25, Ingredients = new() { "Salmon", "Broccoli", "Olive Oil" } },
                new Meal { Name = "Pasta with Chicken", Calories = 600, ProteinG = 45, CarbsG = 70, FatsG = 10, Ingredients = new() { "Pasta", "Chicken", "Tomato Sauce" } },
                new Meal { Name = "Vegetable Curry & Rice", Calories = 550, ProteinG = 20, CarbsG = 65, FatsG = 15, Ingredients = new() { "Vegetables", "Coconut Milk", "Rice" } },
                new Meal { Name = "Quinoa Salad", Calories = 400, ProteinG = 15, CarbsG = 50, FatsG = 12, Ingredients = new() { "Quinoa", "Tomato", "Olive Oil" } },
                new Meal { Name = "Baked Fish with Sweet Potato", Calories = 520, ProteinG = 40, CarbsG = 45, FatsG = 10, Ingredients = new() { "Fish", "Sweet Potato", "Garlic" } },

                // Snacks
                new Meal { Name = "Fruit Bowl", Calories = 200, ProteinG = 3, CarbsG = 45, FatsG = 1, Ingredients = new() { "Apple", "Banana", "Grapes" } },
                new Meal { Name = "Trail Mix", Calories = 250, ProteinG = 8, CarbsG = 20, FatsG = 12, Ingredients = new() { "Nuts", "Raisins", "Seeds" } },
                new Meal { Name = "Protein Bar", Calories = 220, ProteinG = 20, CarbsG = 18, FatsG = 5, Ingredients = new() { "Oats", "Protein Powder", "Honey" } },
                new Meal { Name = "Boiled Eggs", Calories = 160, ProteinG = 14, CarbsG = 1, FatsG = 10, Ingredients = new() { "Eggs" } },
                new Meal { Name = "Peanut Butter Sandwich", Calories = 340, ProteinG = 14, CarbsG = 35, FatsG = 18, Ingredients = new() { "Peanut Butter", "Bread" } }
            };

            _meals.InsertMany(meals);
        }
    }
}
