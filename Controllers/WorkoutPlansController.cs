using Microsoft.AspNetCore.Mvc;
using ToxicFitnessAPI.Models;
using ToxicFitnessAPI.Services;

namespace ToxicFitnessAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkoutPlansController : ControllerBase
    {
        private readonly WorkoutPlanService _workoutPlanService;

        public WorkoutPlansController(WorkoutPlanService workoutPlanService)
        {
            _workoutPlanService = workoutPlanService;
        }

        // -------------------- GENERATE --------------------
        [HttpPost("generate")]
        public ActionResult<WorkoutPlan> GeneratePlan([FromBody] GenerateWorkoutRequest request)
        {
            try
            {
                var plan = _workoutPlanService.GenerateWorkoutPlan(
                    request.UserId, request.Goal, request.Location, request.Difficulty
                );
                return Ok(plan);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // -------------------- GET --------------------
        [HttpGet("{userId}")]
        public ActionResult<WorkoutPlan> GetByUser(string userId)
        {
            var plan = _workoutPlanService.GetByUser(userId);
            if (plan == null)
                return NotFound(new { error = "No workout plan found for this user." });
            return Ok(plan);
        }

        // -------------------- DELETE --------------------
        [HttpDelete("{userId}")]
        public IActionResult DeleteByUser(string userId)
        {
            var success = _workoutPlanService.DeleteByUser(userId);
            if (!success)
                return NotFound(new { error = "No workout plan found for this user." });

            return Ok(new { message = "Workout plan deleted successfully." });
        }
    }

    public class GenerateWorkoutRequest
    {
        public string UserId { get; set; }
        public string Goal { get; set; }
        public string Location { get; set; }
        public string Difficulty { get; set; }
    }
}
