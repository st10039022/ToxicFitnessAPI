using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ToxicFitnessAPI.Models;
using ToxicFitnessAPI.Services;

namespace ToxicFitnessAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserGoalsController : ControllerBase
    {
        private readonly UserGoalService _userGoalService;

        public UserGoalsController(UserGoalService userGoalService)
        {
            _userGoalService = userGoalService;
        }

        [HttpGet]
        public ActionResult<List<UserGoal>> Get()
        {
            var list = _userGoalService.Get();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public ActionResult<UserGoal> GetById(string id)
        {
            var u = _userGoalService.GetById(id);
            if (u == null) return NotFound();
            return Ok(u);
        }

        [HttpPost]
        public ActionResult<UserGoal> Create([FromBody] UserGoal userGoal)
        {
            var created = _userGoalService.Create(userGoal);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
    }
}
