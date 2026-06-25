using Erp.Module.Core.Data;
using Erp.Module.Core.Entities;
using Erp.Shared.Exceptions;
using ERP.Features.Auth.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ERP.Features.Auth
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

            // ──────────────────────────────────────
            // Resolve the user's role and permissions for their home tenant
            // ──────────────────────────────────────
            var tenantAccess = await _dbContext.UserTenantAccesses
                .IgnoreQueryFilters()
                .Include(uta => uta.Role!)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(uta => uta.UserId == user.Id
                                         && uta.TenantId == user.TenantId
                                         && uta.IsActive);

            if (tenantAccess?.Role == null)
            {
                throw new UnauthorizedAccessException("No active role assigned for this tenant.");
            }

            var roleName = tenantAccess.Role.Name;
            var permissionKeys = tenantAccess.Role.RolePermissions
                .Where(rp => rp.Permission != null)
                .Select(rp => rp.Permission!.Key)
                .Distinct()
                .ToList();

            // ──────────────────────────────────────
            // Build JWT claims
            // ──────────────────────────────────────
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, roleName),
                new Claim("tenant_id", user.TenantId?.ToString() ?? string.Empty)
            };

            // Add each permission as a separate claim
            foreach (var permissionKey in permissionKeys)
            {
                claims.Add(new Claim("permission", permissionKey));
            }

            // ──────────────────────────────────────
            // Generate the token
            // ──────────────────────────────────────
            var jwtSecretKey = _configuration["JwtSettings:SecretKey"]
                ?? throw new InvalidOperationException("JWT Secret Key is missing.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiryMinutes = _configuration.GetValue<int>("JwtSettings:ExpiryMinutes", 480);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Update last login timestamp
            user.LastLoginAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return new AuthResponse(tokenString, user.Email, roleName, user.TenantId, permissionKeys);
        }

        public async Task<UserProfileResponse> GetMyProfileAsync(Guid userId)
        {
            var user = await _dbContext.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);

            if (user == null)
            {
                throw new NotFoundException("User not found.");
            }

            // Resolve the user's role for their home tenant
            var tenantAccess = await _dbContext.UserTenantAccesses
                .IgnoreQueryFilters()
                .Include(uta => uta.Role!)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(uta => uta.UserId == user.Id
                                         && uta.TenantId == user.TenantId
                                         && uta.IsActive);

            var roleName = tenantAccess?.Role?.Name ?? "Unknown";
            var permissionKeys = tenantAccess?.Role?.RolePermissions
                .Where(rp => rp.Permission != null)
                .Select(rp => rp.Permission!.Key)
                .Distinct()
                .ToList() ?? [];

            return new UserProfileResponse
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RoleName = roleName,
                TenantId = user.TenantId,
                Permissions = permissionKeys
            };
        }

        public async Task<List<TenantListItemDto>> GetMyTenantsAsync(Guid userId, bool isSuperAdmin)
        {
            if (isSuperAdmin)
            {
                // SuperAdmins can see all active tenants
                var allTenants = await _dbContext.Tenants
                    .IgnoreQueryFilters()
                    .Where(t => t.IsActive)
                    .Select(t => new TenantListItemDto
                    {
                        Id = t.Id,
                        CompanyName = t.CompanyName,
                        CompanyCode = t.CompanyCode,
                        RoleName = "SuperAdmin"
                    })
                    .ToListAsync();
                return allTenants;
            }

            // Normal users only see what's in UserTenantAccess
            var userTenants = await _dbContext.UserTenantAccesses
                .IgnoreQueryFilters()
                .Include(uta => uta.Tenant)
                .Include(uta => uta.Role)
                .Where(uta => uta.UserId == userId && uta.IsActive && uta.Tenant != null && uta.Tenant.IsActive)
                .Select(uta => new TenantListItemDto
                {
                    Id = uta.TenantId,
                    CompanyName = uta.Tenant!.CompanyName,
                    CompanyCode = uta.Tenant.CompanyCode,
                    RoleName = uta.Role != null ? uta.Role.Name : "Unknown"
                })
                .ToListAsync();

            return userTenants;
        }

        public async Task<AuthResponse> SwitchTenantAsync(Guid userId, Guid targetTenantId, bool isSuperAdmin)
        {
            var user = await _dbContext.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);

            if (user == null) throw new UnauthorizedAccessException("User not found or inactive.");

            var targetTenant = await _dbContext.Tenants
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(t => t.Id == targetTenantId && t.IsActive);

            if (targetTenant == null) throw new UnauthorizedAccessException("Target tenant not found or inactive.");

            var tenantAccess = await _dbContext.UserTenantAccesses
                .IgnoreQueryFilters()
                .Include(uta => uta.Role!)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(uta => uta.UserId == user.Id
                                         && uta.TenantId == targetTenantId
                                         && uta.IsActive);

            string roleName;
            List<string> permissionKeys = new List<string>();

            if (tenantAccess != null && tenantAccess.Role != null)
            {
                // User has explicit access
                roleName = tenantAccess.Role.Name;
                permissionKeys = tenantAccess.Role.RolePermissions
                    .Where(rp => rp.Permission != null)
                    .Select(rp => rp.Permission!.Key)
                    .Distinct()
                    .ToList();
            }
            else if (isSuperAdmin)
            {
                // SuperAdmin implicit access
                roleName = "SuperAdmin";
                // SuperAdmin overrides all permissions usually, but we could add a wildcard claim if needed.
                // We'll rely on the Role claim in CurrentUserService.
            }
            else
            {
                throw new UnauthorizedAccessException("You do not have access to this tenant.");
            }

            // Build JWT claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, roleName),
                new Claim("tenant_id", targetTenantId.ToString())
            };

            foreach (var permissionKey in permissionKeys)
            {
                claims.Add(new Claim("permission", permissionKey));
            }

            var jwtSecretKey = _configuration["JwtSettings:SecretKey"]
                ?? throw new InvalidOperationException("JWT Secret Key is missing.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiryMinutes = _configuration.GetValue<int>("JwtSettings:ExpiryMinutes", 480);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthResponse(tokenString, user.Email, roleName, targetTenantId, permissionKeys);
        }
    }
}

