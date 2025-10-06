using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToxicFitnessAPI.Models;

namespace ToxicFitnessAPI.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(string connectionUri, string dbName)
        {
            var client = new MongoClient(connectionUri);
            var db = client.GetDatabase(dbName);

            await SeedExercisesAsync(db);
            await SeedWorkoutsAsync(db);
        }

        // -------------------- EXERCISES --------------------
        private static async Task SeedExercisesAsync(IMongoDatabase db)
        {
            var exCol = db.GetCollection<Exercise>("exercises");
            var count = await exCol.CountDocumentsAsync(FilterDefinition<Exercise>.Empty);
            if (count > 0) return;

            var exercises = new List<Exercise>
            {
                // 🏠 HOME EXERCISES
                new Exercise { Name = "Push-Up", MuscleGroup = "Chest", Equipment = "None", Difficulty = "Beginner", VideoUrl = "https://youtu.be/_l3ySVKYVJ8" },
                new Exercise { Name = "Bodyweight Squat", MuscleGroup = "Legs", Equipment = "None", Difficulty = "Beginner", VideoUrl = "https://youtu.be/aclHkVaku9U" },
                new Exercise { Name = "Lunges", MuscleGroup = "Legs", Equipment = "None", Difficulty = "Beginner", VideoUrl = "https://youtu.be/QOVaHwm-Q6U" },
                new Exercise { Name = "Plank", MuscleGroup = "Core", Equipment = "None", Difficulty = "Beginner", VideoUrl = "https://youtu.be/pSHjTRCQxIw" },
                new Exercise { Name = "Mountain Climbers", MuscleGroup = "Cardio/Core", Equipment = "None", Difficulty = "Beginner", VideoUrl = "https://youtu.be/nmwgirgXLYM" },
                new Exercise { Name = "Chair Dips", MuscleGroup = "Triceps", Equipment = "Chair", Difficulty = "Beginner", VideoUrl = "https://youtu.be/6kALZikXxLc" },
                new Exercise { Name = "Wall Sit", MuscleGroup = "Legs", Equipment = "None", Difficulty = "Beginner", VideoUrl = "https://youtu.be/-cdph8hv0O0" },
                new Exercise { Name = "Burpees", MuscleGroup = "Full Body", Equipment = "None", Difficulty = "Intermediate", VideoUrl = "https://youtu.be/TU8QYVW0gDU" },
                new Exercise { Name = "High Knees", MuscleGroup = "Cardio", Equipment = "None", Difficulty = "Beginner", VideoUrl = "https://youtu.be/OAJ_J3EZkdY" },
                new Exercise { Name = "Crunches", MuscleGroup = "Core", Equipment = "None", Difficulty = "Beginner", VideoUrl = "https://youtu.be/Xyd_fa5zoEU" },
                new Exercise { Name = "Jumping Jacks", MuscleGroup = "Cardio", Equipment = "None", Difficulty = "Beginner", VideoUrl = "https://youtu.be/c4DAnQ6DtF8" },
                new Exercise { Name = "Glute Bridge", MuscleGroup = "Legs", Equipment = "None", Difficulty = "Beginner", VideoUrl = "https://youtu.be/wPM8icPu6H8" },

                // 🏋️ GYM EXERCISES
                new Exercise { Name = "Barbell Bench Press", MuscleGroup = "Chest", Equipment = "Barbell", Difficulty = "Intermediate", VideoUrl = "https://youtu.be/rT7DgCr-3pg" },
                new Exercise { Name = "Incline Dumbbell Press", MuscleGroup = "Chest", Equipment = "Dumbbell", Difficulty = "Intermediate", VideoUrl = "https://youtu.be/8iPEnn-ltC8" },
                new Exercise { Name = "Lat Pulldown", MuscleGroup = "Back", Equipment = "Machine", Difficulty = "Intermediate", VideoUrl = "https://youtu.be/CAwf7n6Luuc" },
                new Exercise { Name = "Barbell Row", MuscleGroup = "Back", Equipment = "Barbell", Difficulty = "Intermediate", VideoUrl = "https://youtu.be/vT2GjY_Umpw" },
                new Exercise { Name = "Bicep Curl", MuscleGroup = "Biceps", Equipment = "Dumbbell", Difficulty = "Beginner", VideoUrl = "https://youtu.be/in7PaeYlhrM" },
                new Exercise { Name = "Tricep Pushdown", MuscleGroup = "Triceps", Equipment = "Cable", Difficulty = "Intermediate", VideoUrl = "https://youtu.be/2-LAMcpzODU" },
                new Exercise { Name = "Squat", MuscleGroup = "Legs", Equipment = "Barbell", Difficulty = "Intermediate", VideoUrl = "https://youtu.be/aclHkVaku9U" },
                new Exercise { Name = "Deadlift", MuscleGroup = "Back/Legs", Equipment = "Barbell", Difficulty = "Advanced", VideoUrl = "https://youtu.be/op9kVnSso6Q" },
                new Exercise { Name = "Leg Press", MuscleGroup = "Legs", Equipment = "Machine", Difficulty = "Intermediate", VideoUrl = "https://youtu.be/IZxyjW7MPJQ" },
                new Exercise { Name = "Shoulder Press", MuscleGroup = "Shoulders", Equipment = "Dumbbell", Difficulty = "Intermediate", VideoUrl = "https://youtu.be/B-aVuyhvLHU" },
                new Exercise { Name = "Cable Crunch", MuscleGroup = "Abs", Equipment = "Cable", Difficulty = "Intermediate", VideoUrl = "https://youtu.be/9bR1W9V1VgE" },
                new Exercise { Name = "Treadmill Run", MuscleGroup = "Cardio", Equipment = "Machine", Difficulty = "Beginner", VideoUrl = "https://youtu.be/Yy8K1HNl6Qg" },
                new Exercise { Name = "Rowing Machine", MuscleGroup = "Cardio", Equipment = "Machine", Difficulty = "Intermediate", VideoUrl = "https://youtu.be/G2eUgey4zEA" }
            };

            await exCol.InsertManyAsync(exercises);
        }

        // -------------------- WORKOUTS --------------------
        private static async Task SeedWorkoutsAsync(IMongoDatabase db)
        {
            var wCol = db.GetCollection<Workout>("Workouts");
            var count = await wCol.CountDocumentsAsync(FilterDefinition<Workout>.Empty);
            if (count > 0) return;

            var workouts = new List<Workout>
            {
                // 💪 GYM PROGRAMS
                new Workout
                {
                    Name = "Push Day (Gym)",
                    Type = "Strength",
                    Difficulty = "Intermediate",
                    DurationMinutes = 60,
                    Exercises = new List<string> {
                        "Barbell Bench Press - 4x10",
                        "Incline Dumbbell Press - 3x12",
                        "Shoulder Press - 3x10",
                        "Tricep Pushdown - 3x12"
                    }
                },
                new Workout
                {
                    Name = "Pull Day (Gym)",
                    Type = "Strength",
                    Difficulty = "Intermediate",
                    DurationMinutes = 60,
                    Exercises = new List<string> {
                        "Barbell Row - 4x10",
                        "Lat Pulldown - 3x12",
                        "Bicep Curl - 3x12",
                        "Deadlift - 4x8"
                    }
                },
                new Workout
                {
                    Name = "Leg Day (Gym)",
                    Type = "Strength",
                    Difficulty = "Advanced",
                    DurationMinutes = 70,
                    Exercises = new List<string> {
                        "Squat - 4x10",
                        "Leg Press - 3x12",
                        "Lunges - 3x12 per leg",
                        "Calf Raises - 4x15"
                    }
                },
                new Workout
                {
                    Name = "Cardio & Core (Gym)",
                    Type = "Cardio",
                    Difficulty = "Intermediate",
                    DurationMinutes = 45,
                    Exercises = new List<string> {
                        "Treadmill Run - 20 mins",
                        "Rowing Machine - 15 mins",
                        "Cable Crunch - 3x20"
                    }
                },

                // 🏠 HOME PROGRAMS
                new Workout
                {
                    Name = "Full Body (Home)",
                    Type = "Bodyweight",
                    Difficulty = "Beginner",
                    DurationMinutes = 40,
                    Exercises = new List<string> {
                        "Push-Up - 3x15",
                        "Bodyweight Squat - 3x20",
                        "Plank - 3x45s",
                        "Lunges - 3x12 per leg",
                        "Mountain Climbers - 3x30s"
                    }
                },
                new Workout
                {
                    Name = "Upper Body (Home)",
                    Type = "Bodyweight",
                    Difficulty = "Beginner",
                    DurationMinutes = 35,
                    Exercises = new List<string> {
                        "Push-Up - 4x12",
                        "Chair Dips - 3x15",
                        "Plank Shoulder Taps - 3x20",
                        "Superman Hold - 3x45s"
                    }
                },
                new Workout
                {
                    Name = "Legs & Core (Home)",
                    Type = "Bodyweight",
                    Difficulty = "Beginner",
                    DurationMinutes = 40,
                    Exercises = new List<string> {
                        "Squats - 4x15",
                        "Wall Sit - 3x45s",
                        "Glute Bridges - 3x20",
                        "Crunches - 3x20"
                    }
                },
                new Workout
                {
                    Name = "Cardio Burn (Home)",
                    Type = "Cardio",
                    Difficulty = "Intermediate",
                    DurationMinutes = 30,
                    Exercises = new List<string> {
                        "Jumping Jacks - 3x50",
                        "High Knees - 3x40s",
                        "Burpees - 3x12",
                        "Jog in Place - 5 mins"
                    }
                }
            };

            await wCol.InsertManyAsync(workouts);
        }
    }
}
