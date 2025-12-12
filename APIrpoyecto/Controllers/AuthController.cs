using APIrpoyecto.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace APIrpoyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserService _userService;
        public AuthController(IConfiguration configuration, UserService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }
        //enpoint para loguearse
        [HttpPost("Login")]
        public IActionResult Login([FromBody] UserCredentials credentials)
        {
            //verificar si las credenciales son correctas
            var user = _userService.Authenticate(credentials.Username, credentials.Password);

            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }
            //se generan los claims para la token
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role),

                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: creds
                );
            //se devuelve la token
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });

        }
        //endpoint Register para registrar un nuevo usuario
        [HttpPost("Register")]
        public IActionResult Register([FromBody] UserCredentials credentials)
        {
            //se verifica si el usuario ya existe
            var existingUser = _userService.CheckIfUserExists(credentials.Username);
            if (existingUser)
            {
                return BadRequest("Username already exists");
            }

            //se crea un nuevo usuario con la contraseña hasheada
            var hashedPassword = UserService.HashPassword(credentials.Password);
            var user = _userService.RegisterUser(credentials.Username, hashedPassword, "User");
            //se generan los claims para la token
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, credentials.Username),
                    new Claim(ClaimTypes.Role, "User"),

                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );
            if (user == null)
            {
                return StatusCode(500, "An error occurred while registering the user");
            }

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

    }

    public class UserCredentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
