using Microsoft.AspNetCore.Mvc;
using MovieApi.Repositories;
using System.Threading.Tasks;

namespace MovieApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionRepository _repo;

        public QuestionController(IQuestionRepository repo)
        {
            _repo = repo;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var questions = await _repo.GetAllAsync();
            return Ok(questions);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var question = await _repo.GetByIdAsync(id);
            if (question == null) return NotFound();
            return Ok(question);
        }
    }
}
