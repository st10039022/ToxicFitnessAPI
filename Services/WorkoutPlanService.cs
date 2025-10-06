using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using ToxicFitnessAPI.Models;

namespace ToxicFitnessAPI.Services
{
    public class WorkoutPlanService
    {
        private readonly IMongoCollection<Workout> _workouts;
        private readonly IMongoCollection<WorkoutPlan> _plans;

        public WorkoutPlanService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            var client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            var database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);

            _workouts = database.GetCollection<Workout>("Workouts");
            _plans = database.GetCollection<WorkoutPlan>("WorkoutPlans");
        }

        // -------------------- GENERATE WORKOUT PLAN --------------------
        public WorkoutPlan GenerateWorkoutPlan(string userId, string goal, string location, string difficulty)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new Exception("UserId is required.");

            var allWorkouts = _workouts.Find(_ => true).ToList();
            if (!allWorkouts.Any())
                throw new Exception("No workouts found in the database.");

            goal = (goal ?? "").Trim().ToLower();
            location = (location ?? "").Trim().ToLower();
            difficulty = (difficulty ?? "").Trim().ToLower();

            // Filter by location
            var byLocation = allWorkouts.Where(w =>
                (location == "home" && w.Name.Contains("(Home)", StringComparison.OrdinalIgnoreCase)) ||
                (location == "gym" && w.Name.Contains("(Gym)", StringComparison.OrdinalIgnoreCase))
            ).ToList();

            if (!byLocation.Any())
                throw new Exception($"No workouts found for location: {location}");

            // Filter by difficulty (fallback)
            var byDifficulty = byLocation
                .Where(w => w.Difficulty.Equals(difficulty, StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (!byDifficulty.Any()) byDifficulty = byLocation;

            // Filter by goal
            List<Workout> goalPreferred = goal switch
            {
                "bulking" or "strength" => byDifficulty.Where(w =>
                    w.Type.Equals("Strength", StringComparison.OrdinalIgnoreCase)).ToList(),

                "cutting" => byDifficulty.Where(w =>
                    w.Type.Equals("Cardio", StringComparison.OrdinalIgnoreCase) ||
                    w.Name.Contains("Full Body", StringComparison.OrdinalIgnoreCase)).ToList(),

                "toning" => byDifficulty.Where(w =>
                    w.Type.Equals("Bodyweight", StringComparison.OrdinalIgnoreCase) ||
                    w.Type.Equals("Cardio", StringComparison.OrdinalIgnoreCase)).ToList(),

                "cardio" => byDifficulty.Where(w =>
                    w.Type.Equals("Cardio", StringComparison.OrdinalIgnoreCase)).ToList(),

                _ => byDifficulty
            };

            if (!goalPreferred.Any()) goalPreferred = byDifficulty;

            var rnd = new Random();
            var picked = goalPreferred.OrderBy(_ => rnd.Next()).Take(5).ToList();

            var templateDays = BuildWeekTemplate(goal);
            var plan = new WorkoutPlan
            {
                Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                UserId = userId,
                GoalType = goal,
                Days = new List<WorkoutDay>()
            };

            for (int i = 0; i < templateDays.Count; i++)
            {
                var chosen = picked.ElementAtOrDefault(i % picked.Count);
                var warmup = GetWarmup(location);
                var cooldown = GetCooldown(location);
                var items = ParseWorkoutItems(chosen, goal);

                plan.Days.Add(new WorkoutDay
                {
                    DayName = templateDays[i],
                    Items = items,
                    Warmup = warmup,
                    Cooldown = cooldown
                });
            }

            var existing = _plans.Find(p => p.UserId == userId).FirstOrDefault();
            if (existing != null)
            {
                plan.Id = existing.Id;
                _plans.ReplaceOne(p => p.UserId == userId, plan);
            }
            else
            {
                _plans.InsertOne(plan);
            }

            return plan;
        }

        // -------------------- GET USER PLAN --------------------
        public WorkoutPlan GetByUser(string userId)
        {
            return _plans.Find(p => p.UserId == userId).FirstOrDefault();
        }

        // -------------------- DELETE USER PLAN --------------------
        public bool DeleteByUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));

            var result = _plans.DeleteOne(p => p.UserId == userId);
            return result.DeletedCount > 0;
        }

        // -------------------- HELPERS --------------------
        private List<string> BuildWeekTemplate(string goal) => goal switch
        {
            "bulking" or "strength" => new() {
                "Monday — Push (Chest/Triceps)",
                "Tuesday — Pull (Back/Biceps)",
                "Wednesday — Legs (Quads/Hamstrings/Glutes)",
                "Thursday — Upper (Compound Focus)",
                "Friday — Lower (Compound Focus)"
            },
            "cutting" => new() {
                "Monday — Full Body Circuit",
                "Tuesday — Cardio Intervals",
                "Wednesday — Core + Conditioning",
                "Thursday — Full Body Circuit",
                "Friday — Cardio Steady State"
            },
            "toning" => new() {
                "Monday — Upper (Bodyweight/Light)",
                "Tuesday — Lower (Bodyweight/Light)",
                "Wednesday — Core + Mobility",
                "Thursday — Upper (Bodyweight/Light)",
                "Friday — Lower (Bodyweight/Light)"
            },
            "cardio" => new() {
                "Monday — Cardio Intervals",
                "Tuesday — Cardio Steady State",
                "Wednesday — Cardio + Core",
                "Thursday — Cardio Intervals",
                "Friday — Cardio Steady State"
            },
            _ => new() {
                "Monday — Full Body",
                "Tuesday — Upper",
                "Wednesday — Lower",
                "Thursday — Full Body",
                "Friday — Cardio/Core"
            }
        };

        private List<string> GetWarmup(string location) =>
            location == "gym"
                ? new() { "Treadmill — 5 min easy", "Band Pull-Aparts — 2x15", "Hip Openers — 2x10/side" }
                : new() { "Jumping Jacks — 60s", "Arm Circles — 2x20s", "Leg Swings — 2x10/side" };

        private List<string> GetCooldown(string _location) =>
            new() { "Static Stretch — 8–10 min", "Deep Breathing — 2 min" };

        private List<WorkoutItem> ParseWorkoutItems(Workout workout, string goal)
        {
            var items = new List<WorkoutItem>();
            var defaultScheme = goal switch
            {
                "bulking" => "4x10",
                "strength" => "5x5",
                "cutting" => "3x15",
                "toning" => "3x12",
                "cardio" => "1x20min",
                _ => "3x12"
            };

            foreach (var ex in workout.Exercises)
            {
                var parts = ex.Split('-', StringSplitOptions.RemoveEmptyEntries);
                var name = parts[0].Trim();
                var schemeRaw = parts.Length > 1 ? parts[1].Trim() : defaultScheme;

                var (sets, reps) = NormalizeScheme(schemeRaw, defaultScheme);

                items.Add(new WorkoutItem
                {
                    ExerciseId = string.Empty,
                    Name = name,
                    Sets = sets,
                    Reps = reps,
                    RestSeconds = GuessRest(goal, name)
                });
            }

            return items;
        }

        private (int sets, string reps) NormalizeScheme(string scheme, string fallback)
        {
            if (string.IsNullOrWhiteSpace(scheme)) scheme = fallback;

            var parts = scheme.ToLower().Split('x', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2 && int.TryParse(parts[0].Trim(), out var s))
                return (s, parts[1].Trim());

            return (1, scheme.Trim());
        }

        private int GuessRest(string goal, string exerciseName)
        {
            var n = exerciseName.ToLower();
            bool compound = n.Contains("squat") || n.Contains("deadlift") ||
                            n.Contains("bench") || n.Contains("row") || n.Contains("press");

            if (goal == "strength") return compound ? 120 : 90;
            if (goal == "bulking") return compound ? 90 : 60;
            if (goal == "cutting" || goal == "toning") return 45;
            if (goal == "cardio") return 30;
            return compound ? 90 : 60;
        }
    }
}
