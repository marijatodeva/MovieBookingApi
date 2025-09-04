using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MovieApi.Models;
using MovieApi.Models.System;
using MovieApi.Repositories;
using MovieAPI.Repositories;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MovieAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppUserController : ControllerBase
    {

        private readonly IAppUserRepository _userRepository;

        public AppUserController(IAppUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> Get()
        {
            var user = await _userRepository.GetAllUsers();
            if (user == null)
                return NotFound();

            return Ok(user);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> Get(int id)
        {
            var user = await _userRepository.GetUserById(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }


        [HttpPost]
        public async Task<ActionResult> Post([FromBody] RegisterRequest user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _userRepository.CreateUser(user);
            return Ok(id);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Put(long id, [FromBody] AppUser user)
        {
            var existingUser = await _userRepository.GetUserById(id);
            if (existingUser == null)
                return NotFound();

            await _userRepository.UpdateUser(id, user);
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            var existingUser = await _userRepository.GetUserById(id);
            if (existingUser == null)
                return NotFound();

            await _userRepository.DeleteUser(id);
            return NoContent();
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid request");

            var user = await _userRepository.GetUserByUsernameAndPassword(request.Username, request.Password);

            if (user == null)
                return Unauthorized("Invalid username or password");

          
            return Ok(new
            {
                Message = "Logged in successfully",
                User = new
                {
                    user.Id,
                    user.Username,
                    user.FullName,
                    user.Role
                }
            });
        }



        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid request");

            var existingUser = await _userRepository.GetUserByUsername(request.Username);

            if (existingUser != null)
                return Unauthorized("Already Exists USername");

            var userId = await _userRepository.CreateUser(request);
            var response = true;
            return Ok(response);
        }
    }
}
