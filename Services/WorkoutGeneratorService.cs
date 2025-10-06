using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ToxicFitnessAPI.Models;

namespace ToxicFitnessAPI.Services
{
    public class WorkoutGeneratorService
    {
        private readonly IMongoCollection<Workout> _workouts;

        public WorkoutGeneratorService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            var client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            var database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _workouts = database.GetCollection<Workout>("Workouts");
        }

        public List<Workout> GetWorkouts() => _workouts.Find(workout => true).ToList();

        public Workout CreateWorkout(Workout workout)
        {
            _workouts.InsertOne(workout);
            return workout;
        }
    }
}
