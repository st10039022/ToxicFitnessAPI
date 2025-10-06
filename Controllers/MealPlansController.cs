using Microsoft.AspNetCore.Mvc;
using ToxicFitnessAPI.Models;
using ToxicFitnessAPI.Services;
using System.Collections.Generic;

namespace ToxicFitnessAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MealPlansController : ControllerBase
    {
        private readonly MealPlanService _mealPlanService;

        public MealPlansController(MealPlanService mealPlanService)
        {
            _mealPlanService = mealPlanService;
        }

        [HttpGet]
        public ActionResult<List<MealPlan>> GetAll() =>
            Ok(_mealPlanService.GetAll());

        [HttpGet("{userId}")]
        public ActionResult<MealPlan> GetByUser(string userId)
        {
            var plan = _mealPlanService.GetByUser(userId);
            if (plan == null) return NotFound();
            return Ok(plan);
        }

        [HttpPost]
        public ActionResult<MealPlan> Create([FromBody] MealPlan plan)
        {
            if (string.IsNullOrEmpty(plan.Id))
                plan.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();

            var created = _mealPlanService.Create(plan);
            return CreatedAtAction(nameof(GetByUser), new { userId = created.UserId }, created);
        }

        [HttpPut("{userId}")]
        public ActionResult<MealPlan> Update(string userId, [FromBody] MealPlan plan)
        {
            var updated = _mealPlanService.Update(userId, plan);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{userId}")]
        public IActionResult Delete(string userId)
        {
            var deleted = _mealPlanService.Delete(userId);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpPost("generate")]
        public ActionResult<MealPlan> Generate([FromBody] UserGoal userGoal)
        {
            if (userGoal == null)
                return BadRequest("Missing user goal data.");

            var plan = _mealPlanService.GenerateMealPlanForUser(userGoal);

            if (string.IsNullOrEmpty(plan.Id))
                plan.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();

            return Ok(plan);
        }
    }
}
