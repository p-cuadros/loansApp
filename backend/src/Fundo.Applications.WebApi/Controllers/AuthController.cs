using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Fundo.Applications.WebApi.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        public AuthController(IConfiguration config) => _config = config;

        public record LoginRequest(string Username, string Password);

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            var user = _config["JWT__User"] ?? "admin";
            var pass = _config["JWT__Password"] ?? "admin";
            if (string.IsNullOrWhiteSpace(req?.Username) || string.IsNullOrWhiteSpace(req?.Password))
                return BadRequest(new { error = "bad_request", message = "Missing username or password" });

            if (req.Username != user || req.Password != pass)
                return Unauthorized(new { error = "invalid_credentials", message = "Invalid username or password" });

            var issuer = _config["JWT__Issuer"] ?? "loan-api";
            var audience = _config["JWT__Audience"] ?? "loan-ui";
            var key = _config["JWT__Key"] ?? "dev-secret-key-change";
            var expires = DateTime.UtcNow.AddHours(1);

            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, req.Username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = expires,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(descriptor);
            var jwt = tokenHandler.WriteToken(token);
            return Ok(new { token = jwt, expiresIn = (int)TimeSpan.FromHours(1).TotalSeconds });
        }
    }
}
