using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ToxicFitnessAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ToxicFitnessAPI.Services
{
    public class MealPlanService
    {
        private readonly IMongoCollection<MealPlan> _mealPlans;
        private readonly NutritionService _nutritionService;

        public MealPlanService(IOptions<MongoDBSettings> mongoDBSettings, NutritionService nutritionService)
        {
            var client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            var database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _mealPlans = database.GetCollection<MealPlan>("MealPlans");
            _nutritionService = nutritionService;
        }

        // -------------------- CRUD --------------------
        public List<MealPlan> GetAll() =>
            _mealPlans.Find(_ => true).ToList();

        public MealPlan GetByUser(string userId) =>
            _mealPlans.Find(mp => mp.UserId == userId).FirstOrDefault();

        public MealPlan Create(MealPlan plan)
        {
            if (string.IsNullOrEmpty(plan.Id))
                plan.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();

            _mealPlans.InsertOne(plan);
            return plan;
        }

        public MealPlan Update(string userId, MealPlan plan)
        {
            var existing = _mealPlans.Find(mp => mp.UserId == userId).FirstOrDefault();
            if (existing == null) return null;

            plan.Id = existing.Id;
            _mealPlans.ReplaceOne(mp => mp.UserId == userId, plan);
            return plan;
        }

        public bool Delete(string userId)
        {
            var result = _mealPlans.DeleteOne(mp => mp.UserId == userId);
            return result.DeletedCount > 0;
        }

        // -------------------- GENERATE MEAL PLAN --------------------
        public MealPlan GenerateMealPlanForUser(UserGoal userGoal)
        {
            var allMeals = _nutritionService.GetMealsWithoutAllergens(userGoal.Allergies);
            if (!allMeals.Any()) return null;

            // --- Calorie Target ---
            double baseCalories = CalculateCalories(userGoal);
            double dailyCaloriesTarget = userGoal.Goal?.ToLower() switch
            {
                "bulking" => baseCalories + 500,
                "cutting" => baseCalories - 500,
                "toning" => baseCalories,
                "strength" => baseCalories + 300,
                _ => baseCalories
            };

            var plan = new MealPlan
            {
                Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                UserId = userGoal.UserId,
                Name = $"{userGoal.Goal} Meal Plan",
                DailyCaloriesTarget = Math.Round(dailyCaloriesTarget, 2),
                TotalCalories = Math.Round(dailyCaloriesTarget * 7, 2),
                Days = new List<MealDay>()
            };

            var rnd = new Random();
            string[] weekDays = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

            // --- Categorize Meals ---
            var breakfastMeals = allMeals.Where(m => m.Calories <= 400).ToList();
            var lunchMeals = allMeals.Where(m => m.Calories > 400 && m.Calories <= 600).ToList();
            var dinnerMeals = allMeals.Where(m => m.Calories > 500).ToList();
            var snackMeals = allMeals.Where(m => m.Calories <= 300).ToList();

            // --- Build Plan ---
            foreach (var day in weekDays)
            {
                var dayMeals = new List<Meal>();

                if (breakfastMeals.Any()) dayMeals.Add(GetRandomMeal(breakfastMeals, rnd));
                if (lunchMeals.Any()) dayMeals.Add(GetRandomMeal(lunchMeals, rnd));
                if (dinnerMeals.Any()) dayMeals.Add(GetRandomMeal(dinnerMeals, rnd));
                if (snackMeals.Any()) dayMeals.Add(GetRandomMeal(snackMeals, rnd));

                double dayCalories = dayMeals.Sum(m => m.Calories);
                if (dayCalories < dailyCaloriesTarget * 0.8)
                {
                    var extra = allMeals.Where(m => !dayMeals.Contains(m))
                        .OrderBy(x => rnd.Next()).FirstOrDefault();
                    if (extra != null) dayMeals.Add(extra);
                }

                plan.Days.Add(new MealDay
                {
                    DayName = day,
                    Meals = dayMeals
                });
            }

            _mealPlans.InsertOne(plan);
            return plan;
        }

        // -------------------- CALCULATE CALORIES --------------------
        private double CalculateCalories(UserGoal user)
        {
            double bmr;

            if (user.Gender?.ToLower() == "male")
                bmr = 88.36 + (13.4 * user.Weight) + (4.8 * user.Height) - (5.7 * user.Age);
            else
                bmr = 447.6 + (9.2 * user.Weight) + (3.1 * user.Height) - (4.3 * user.Age);

            double multiplier = user.ActivityLevel?.ToLower() switch
            {
                "lightly active" => 1.375,
                "moderately active" => 1.55,
                "very active" => 1.725,
                _ => 1.2
            };

            return bmr * multiplier;
        }

        // -------------------- HELPERS --------------------
        private Meal GetRandomMeal(List<Meal> meals, Random rnd)
        {
            if (meals == null || meals.Count == 0) return null;
            return meals[rnd.Next(meals.Count)];
        }
    }
}
