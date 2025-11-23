using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkConnect.Api.Dtos;
using WorkConnect.Infrastructure.Data;
using WorkConnect.Domain.Entities;

namespace WorkConnect.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly WorkConnectContext _context;

        public UsersController(WorkConnectContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers()
        {
            var users = await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            var dtos = users.Select(u => new UserResponseDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Occupation = u.Occupation,
                Country = u.Country,
                ExperienceLevel = u.ExperienceLevel,
                CreatedAt = u.CreatedAt
            }).ToList();

            return Ok(dtos);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserResponseDto>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound();

            var dto = new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Occupation = user.Occupation,
                Country = user.Country,
                ExperienceLevel = user.ExperienceLevel,
                CreatedAt = user.CreatedAt
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<UserResponseDto>> CreateUser([FromBody] UserCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existsEmail = await _context.Users.AnyAsync(u => u.Email == dto.Email);
            if (existsEmail)
            {
                ModelState.AddModelError("Email", "Email já cadastrado.");
                return BadRequest(ModelState);
            }

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Occupation = dto.Occupation,
                Country = dto.Country,
                ExperienceLevel = dto.ExperienceLevel,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var response = new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Occupation = user.Occupation,
                Country = user.Country,
                ExperienceLevel = user.ExperienceLevel,
                CreatedAt = user.CreatedAt
            };

            return CreatedAtAction(nameof(GetUser), new { id = user.Id, version = "1.0" }, response);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            user.Name = dto.Name;
            user.Occupation = dto.Occupation;
            user.Country = dto.Country;
            user.ExperienceLevel = dto.ExperienceLevel;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
