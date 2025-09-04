using Microsoft.AspNetCore.Mvc;
using MovieApi.Models;
using MovieApi.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieRepository _movieRepository;

        public MoviesController(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> Get()
        {
            var movies = await _movieRepository.GetAllMovies();
            return Ok(movies);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> Get(long id)
        {
            var movie = await _movieRepository.GetMovieById(id);
            if (movie == null)
                return NotFound();

            return Ok(movie);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Movie movie)
        {
            await _movieRepository.CreateMovie(movie);
            return CreatedAtAction(nameof(Get), new { id = movie.Id }, movie);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMovie(long id, [FromBody] Movie updatedMovie)
        {
            var existingMovie = await _movieRepository.GetMovieById(id);
            if (existingMovie == null)
                return NotFound();

            await _movieRepository.UpdateMovie(id, updatedMovie);
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            var existingMovie = await _movieRepository.GetMovieById(id);
            if (existingMovie == null)
                return NotFound();

            await _movieRepository.DeleteMovie(id);
            return NoContent();
        }
    }
}

