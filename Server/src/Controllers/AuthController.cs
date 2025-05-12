using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using src.Models;

namespace UserAuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserDbContext _context;

        public AuthController(UserDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // Vérification de l'unicité de l'email
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return Conflict("Email already in use");
            }

            // Vérification que le mot de passe et la confirmation correspondent
            if (request.Password != request.ConfirmPassword)
            {
                return BadRequest("Passwords do not match");
            }

            // Création de l'utilisateur
            var user = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password) // Hash du mot de passe
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

            if (dbUser == null || !BCrypt.Net.BCrypt.Verify(user.PasswordHash, dbUser.PasswordHash))
            {
                return Unauthorized("Invalid credentials");
            }

            // Créer la session utilisateur ici (par exemple avec un cookie ou un JWT)
            return Ok(new { message = "Login successful" });
        }
    }
}
