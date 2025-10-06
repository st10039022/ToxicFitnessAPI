using Microsoft.AspNetCore.Mvc;
using ToxicFitnessAPI.Models;
using ToxicFitnessAPI.Services;
using System.Collections.Generic;

namespace ToxicFitnessAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MealsController : ControllerBase
    {
        private readonly NutritionService _nutritionService;

        public MealsController(NutritionService nutritionService)
        {
            _nutritionService = nutritionService;
        }

        [HttpGet]
        public ActionResult<List<Meal>> Get() => Ok(_nutritionService.GetMeals());

        [HttpPost]
        public ActionResult<Meal> Create([FromBody] Meal meal)
        {
            var created = _nutritionService.CreateMeal(meal);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public ActionResult<Meal> Update(string id, [FromBody] Meal meal)
        {
            var updated = _nutritionService.UpdateMeal(id, meal);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var deleted = _nutritionService.DeleteMeal(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
