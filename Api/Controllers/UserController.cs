using Microsoft.AspNetCore.Mvc;
using Api.Models; 
using Api.Data; 
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization; 


namespace Api.Controllers
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
            var key = Encoding.ASCII.GetBytes("TmV3U2VjcmV0S2V5MjAyNFZlcnlTdHJvbmdLZXk=");
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
                return BadRequest("Ky username tashme egziston.");
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

        [HttpPut("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
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

            if (!string.IsNullOrEmpty(request.NewUsername) && request.NewUsername != user.Username)
            {
                if (await _context.Users.AnyAsync(u => u.Username == request.NewUsername))
                {
                    return BadRequest("Ky emer tash me egziston.");
                }

                user.Username = request.NewUsername;
            }

            if (!string.IsNullOrEmpty(request.NewPassword))
            {
                user.Password = _passwordHasher.HashPassword(user, request.NewPassword);
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "profili juaj u rifreskua" });
        }

        public class UpdateProfileRequest
        {
            public string? NewUsername { get; set; } 
            public string? NewPassword { get; set; } 
        }

        [HttpPost("upload-image")]
        [Authorize]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var user = await _context.Users.FindAsync(Convert.ToInt32(userId));
            if (user == null)
            {
                return NotFound();
            }

            var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImages");

            if (!Directory.Exists(uploadsFolderPath))
                Directory.CreateDirectory(uploadsFolderPath);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine("UploadedImages", fileName);
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), filePath);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            user.ImagePath = fileName;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Image uploaded successfully" });
        }


        [HttpGet("all")]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }
    }
}
