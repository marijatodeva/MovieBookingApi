using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieApi.Models; // твојот MovieShowing модел
using MovieApi.Repositories;
using MovieAPI.Models; // ако треба
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieShowingController : ControllerBase
    {
        private readonly IMovieShowingRepository _movieShowingRepository;

        public MovieShowingController(IMovieShowingRepository movieShowingRepository)
        {
            _movieShowingRepository = movieShowingRepository;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieShowing>>> GetShowings(int movieId, DateTime? date)
        {
            try
            {

                var showDate = date ?? DateTime.Today;

                var showings = await _movieShowingRepository.GetShowingsForMovie(movieId, showDate);

                if (showings == null || !showings.Any())
                {
                    return NotFound();
                }

                return Ok(showings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<MovieShowing>>> GetAllShowings(int movieId)
        {
            try
            {

                var showings = await _movieShowingRepository.GetAllShowingsForMovie(movieId);

                if (showings == null || !showings.Any())
                {
                    return NotFound();
                }

                return Ok(showings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }


        [HttpGet("seats")]
        public async Task<ActionResult<IEnumerable<Seat>>> GetSeatsForShowing(int showingId)
        {
            var seats = await _movieShowingRepository.GetSeatsForShowing(showingId);
            if (seats == null || !seats.Any()) return NotFound();
            return Ok(seats);
        }


        [HttpGet("bookedSeats")]
        public async Task<ActionResult<IEnumerable<string>>> GetBookedSeatsForShowing(int showingId)
        {
            var bookedSeats = await _movieShowingRepository.GetBookedSeatsForShowing(showingId);
            if (bookedSeats == null || !bookedSeats.Any()) return NotFound();
            return Ok(bookedSeats);
        }


        [HttpPost]
        public async Task<ActionResult> AddShowing([FromForm] CreateMovieShowing showing)
        {
            if (showing == null) return BadRequest();

            var result = await _movieShowingRepository.AddShowing(showing);

            if (result > 0)
                return Ok(showing);

            return StatusCode(500, "Failed to add showing");
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteShowing(int id)
        {
            var result = await _movieShowingRepository.DeleteShowing(id);

            if (result > 0)
                return NoContent(); 
            else
                return NotFound();
        }
    }
}
