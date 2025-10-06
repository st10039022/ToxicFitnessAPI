using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using ToxicFitnessAPI.Models;

namespace ToxicFitnessAPI.Services
{
    public class UserGoalService
    {
        private readonly IMongoCollection<UserGoal> _userGoals;

        public UserGoalService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            var client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            var database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _userGoals = database.GetCollection<UserGoal>("UserGoals");
        }

        public List<UserGoal> Get() =>
            _userGoals.Find(user => true).ToList();

        public UserGoal GetById(string id) =>
            _userGoals.Find(u => u.Id == id).FirstOrDefault();

        public UserGoal Create(UserGoal userGoal)
        {
            _userGoals.InsertOne(userGoal);
            return userGoal;
        }

        public void Update(string id, UserGoal userGoal) =>
            _userGoals.ReplaceOne(u => u.Id == id, userGoal);
    }
}
