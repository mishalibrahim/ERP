using Erp.Shared.Exceptions;
using ERP.DTOs.Auth;
using ERP.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ERP.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<AuthResponse> LoginAsync(LoginRequest loginRequest)
        {
            // Implementation for login logic
            if (loginRequest.Email != "admin@aegis-erp.com" || loginRequest.Password != "password123")
            {
                throw new ValidationException("Invalid email or password.");
            }

            var userId = "usr_001";
            var tenantId = "tenant_AegisFZE";
            var role = "SuperAdmin";
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, loginRequest.Email),
            new Claim(ClaimTypes.Role, role),
            new Claim("tenant_id", tenantId)
        };

            // 3. SECURE THE TOKEN
            var jwtSecretKey = _configuration["JwtSettings:SecretKey"]
                ?? throw new InvalidOperationException("JWT Secret Key is missing.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiryMinutes = _configuration.GetValue<int>("JwtSettings:ExpiryMinutes", 480);

            // 4. GENERATE THE TOKEN
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Return the final data (Notice we use Task.FromResult because we aren't using async/await just yet)
            return (new AuthResponse(tokenString, loginRequest.Email, role, tenantId));
        }
    }
}
