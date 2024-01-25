using Microsoft.AspNetCore.Mvc;
using YourProjectName.Models; // Replace with your actual namespace
using YourProjectName.Data; // Replace with your actual namespace
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

namespace YourProjectName.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(User user)
        {
            var existingUser = await _context.Users.AnyAsync(u => u.Username == user.Username);
            if (existingUser)
            {
                return BadRequest("Username already taken.");
            }

            user.Password = _passwordHasher.HashPassword(user, user.Password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(User login)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == login.Username);

            if (user == null || _passwordHasher.VerifyHashedPassword(user, user.Password, login.Password) != PasswordVerificationResult.Success)
            {
                return Unauthorized();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("TmV3U2VjcmV0S2V5MjAyNFZlcnlTdHJvbmdLZXk="); // Replace with your actual long key
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("edit/{id}")]
        public async Task<IActionResult> EditUser(int id, User updatedUser)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (await _context.Users.AnyAsync(u => u.Username == updatedUser.Username && u.Id != id))
            {
                return BadRequest("Username already taken.");
            }

            if (!string.IsNullOrEmpty(updatedUser.Username))
            {
                user.Username = updatedUser.Username;
            }

            if (!string.IsNullOrEmpty(updatedUser.Password))
            {
                user.Password = _passwordHasher.HashPassword(user, updatedUser.Password);
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("profile")]
        public async Task<ActionResult<User>> GetUserProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _context.Users.FindAsync(Convert.ToInt32(userId));
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }


        [HttpGet("all")]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }
    }
}
