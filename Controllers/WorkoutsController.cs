using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ToxicFitnessAPI.Models;

namespace ToxicFitnessAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkoutsController : ControllerBase
    {
        private readonly IMongoCollection<Workout> _workoutsCollection;

        public WorkoutsController(IMongoDatabase database)
        {
            _workoutsCollection = database.GetCollection<Workout>("Workouts");
        }

        [HttpGet]
        public async Task<ActionResult<List<Workout>>> GetAll()
        {
            var workouts = await _workoutsCollection.Find(_ => true).ToListAsync();
            return Ok(workouts);
        }

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Workout>> GetById(string id)
        {
            var workout = await _workoutsCollection.Find(w => w.Id == id).FirstOrDefaultAsync();
            if (workout == null) return NotFound();
            return Ok(workout);
        }

        [HttpPost]
        public async Task<ActionResult<Workout>> Create(Workout workout)
        {
            await _workoutsCollection.InsertOneAsync(workout);
            return CreatedAtAction(nameof(GetById), new { id = workout.Id }, workout);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Workout updatedWorkout)
        {
            // 🔧 Ensure the workout ID is not replaced
            var existingWorkout = await _workoutsCollection.Find(w => w.Id == id).FirstOrDefaultAsync();
            if (existingWorkout == null) return NotFound();

            // Keep the original ID
            updatedWorkout.Id = existingWorkout.Id;

            // Replace the existing document (safe way)
            await _workoutsCollection.ReplaceOneAsync(w => w.Id == id, updatedWorkout);
            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _workoutsCollection.DeleteOneAsync(w => w.Id == id);
            if (result.DeletedCount == 0) return NotFound();
            return NoContent();
        }
    }
}
