using Microsoft.AspNetCore.Mvc;
using MovieApi.Models;
using MovieApi.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketRepository _ticketRepository;

        public TicketController(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ticket>>> Get()
        {
            var tickets = await _ticketRepository.GetAllTickets();
            return Ok(tickets);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Ticket>> Get(long id)
        {
            var ticket = await _ticketRepository.GetTicketById(id);
            if (ticket == null) return NotFound();

            return Ok(ticket);
        }


        [HttpPost]
        public async Task<IActionResult> BookTicket([FromBody] CreateTicket ticket)
        {

            var newTicket = await _ticketRepository.CreateTicket(ticket);
            return Ok(newTicket);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            var existingTicket = await _ticketRepository.GetTicketById(id);
            if (existingTicket == null) return NotFound();

            await _ticketRepository.DeleteTicket(id);
            return NoContent();
        }


        [HttpGet("bookedSeats")]
        public async Task<ActionResult<IEnumerable<string>>> GetBookedSeats(int showingId)
        {
            var bookedSeats = await _ticketRepository.GetBookedSeats(showingId);
            return Ok(bookedSeats);
        }



        [HttpGet("GetTicketsForUser")]
        public async Task<ActionResult<IEnumerable<TicketResponse>>> GetTicketsForUser([FromQuery] int userId)
        {
            if (userId==null)
                return BadRequest("UserId is required.");

            var tickets = await _ticketRepository.GetTicketsByUser(userId);

            return Ok(tickets);
        }
    }
}
