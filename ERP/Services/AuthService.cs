using Azure.Core;
using Erp.Module.Core.Data;
using Erp.Module.Core.Entities;
using Erp.Shared.Exceptions;
using ERP.DTOs.Auth;
using ERP.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ERP.Services
{
    public class AuthService : IAuthService
    {
        private readonly CoreDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<User> _passwordHasher;
        public AuthService(CoreDbContext dbContext, IConfiguration configuration, IPasswordHasher<User> passwordHasher)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
        }
        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _dbContext.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("tenant_id", user.TenantId)
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
            return new AuthResponse(tokenString, user.Email, user.Role, user.TenantId);
        }
        public async Task<UserProfileResponse> GetMyProfileAsync(string userId)
        {
            var user = await _dbContext.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);
            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }
            return new UserProfileResponse
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                TenantId = user.TenantId
            };
        }
    }
}
