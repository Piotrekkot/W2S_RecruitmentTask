using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Way2SendApi.Infrastructure.Models;

namespace Way2SendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// Logowanie użytkownika i generowanie tokenu JWT.
        /// </summary>
        /// <param name="userLogin">Dane logowania użytkownika</param>
        /// <returns>Zwraca token JWT, jeśli logowanie się powiedzie. W przypadku niepowodzenia zwraca status 401 Unauthorized.</returns>
        /// <remarks>
        /// Login i hasło do prawidłowego wygenerowania tokenu to klasyczne, doceniane przez administratorów admin admin  :)
        /// 
        ///     POST /api/auth/login
        ///     {
        ///         "username": "admin",
        ///         "password": "admin"
        ///     }
        ///     
        /// Otrzymany token należy wkleić do Authorize na górze po prawej z przedrostkiem "Bearer "
        /// </remarks>
        /// <response code="200">Zwraca token JWT dla poprawnego logowania</response>
        /// <response code="401">Nieprawidłowe dane logowania</response>
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLogin userLogin)
        {
            if (userLogin.Username == "admin" && userLogin.Password == "admin") // szybka weryfikacja użytkownika ( nie można tak robić w aplikacjach u klienta :D )
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, userLogin.Username)
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["JwtSettings:Issuer"],
                    audience: _configuration["JwtSettings:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(60),
                    signingCredentials: creds);

                return Ok(new
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }

            return Unauthorized();
        }
    }
}


