using Erp.Module.Core.Entities;
using Erp.Shared.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace Erp.Module.Core.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<CoreDbContext>();
            var passwordHasher = serviceProvider.GetRequiredService<IPasswordHasher<User>>();

            // Check if the database already has any roles seeded.
            // We use IgnoreQueryFilters() because nobody is logged in during startup!
            if (await dbContext.Roles.IgnoreQueryFilters().AnyAsync(r => r.IsSystemRole))
            {
                return; // Database is already seeded, skip this script!
            }

            // ──────────────────────────────────────
            // 1. Seed all Permission records
            // ──────────────────────────────────────
            var allPermissionKeys = Permissions.GetAll();
            var permissionEntities = new List<Permission>();

            foreach (var key in allPermissionKeys)
            {
                var parts = key.Split(':');
                var permission = new Permission
                {
                    Module = parts[0],
                    Action = parts[1],
                    Resource = parts[2],
                    Description = $"{parts[1]} access for {parts[0]} ({parts[2]} scope)"
                };
                permissionEntities.Add(permission);
            }

            dbContext.Permissions.AddRange(permissionEntities);

            // ──────────────────────────────────────
            // 2. Seed system roles
            // ──────────────────────────────────────

            // SuperAdmin — platform owner, gets ALL permissions
            var superAdminRole = new Role
            {
                Name = "SuperAdmin",
                Description = "Platform-level administrator with unrestricted access to all tenants and modules.",
                IsSystemRole = true,
                TenantId = null  // System-wide
            };

            // Admin — tenant administrator, gets all tenant-scoped permissions
            var adminRole = new Role
            {
                Name = "Admin",
                Description = "Tenant administrator with full access within their organization.",
                IsSystemRole = true,
                TenantId = null
            };

            // User — basic user, gets read-only permissions
            var userRole = new Role
            {
                Name = "User",
                Description = "Standard user with basic read access.",
                IsSystemRole = true,
                TenantId = null
            };

            dbContext.Roles.AddRange(superAdminRole, adminRole, userRole);

            // ──────────────────────────────────────
            // 3. Assign permissions to roles
            // ──────────────────────────────────────

            // SuperAdmin gets ALL permissions
            foreach (var permission in permissionEntities)
            {
                dbContext.RolePermissions.Add(new RolePermission
                {
                    RoleId = superAdminRole.Id,
                    PermissionId = permission.Id,
                    GrantedBy = null
                });
            }

            // Admin gets all tenant-scoped permissions (excludes "Own" scope)
            var adminPermissions = permissionEntities
                .Where(p => p.Resource == "Tenant" || p.Resource == "Any")
                .ToList();

            foreach (var permission in adminPermissions)
            {
                dbContext.RolePermissions.Add(new RolePermission
                {
                    RoleId = adminRole.Id,
                    PermissionId = permission.Id,
                    GrantedBy = null
                });
            }

            // User gets only Read permissions
            var userPermissions = permissionEntities
                .Where(p => p.Action == "Read")
                .ToList();

            foreach (var permission in userPermissions)
            {
                dbContext.RolePermissions.Add(new RolePermission
                {
                    RoleId = userRole.Id,
                    PermissionId = permission.Id,
                    GrantedBy = null
                });
            }

            // ──────────────────────────────────────
            // 4. Create the Genesis Company (Tenant)
            // ──────────────────────────────────────
            var genesisTenant = new Tenant
            {
                Id = Guid.NewGuid(),
                CompanyName = "Aegis System Admin",
                CompanyCode = "AEGIS-000",
                LicenseNumber = "INTERNAL-001",
                LicenseType = "Internal",
                RegistrationDate = DateTime.UtcNow,
                Country = "UAE",
                Emirate = "Dubai",
                IsFreeZoneEntity = false
            };

            dbContext.Tenants.Add(genesisTenant);

            // ──────────────────────────────────────
            // 5. Create the Master Account
            // ──────────────────────────────────────
            var superAdmin = new User
            {
                Id = Guid.NewGuid(),
                TenantId = genesisTenant.Id,
                FirstName = "Mishal",
                LastName = "Admin",
                Email = "mishal@aegiserp.com",
                IsActive = true
            };

            superAdmin.PasswordHash = passwordHasher.HashPassword(superAdmin, "Admin@123!");

            dbContext.Users.Add(superAdmin);

            // ──────────────────────────────────────
            // 6. Link the SuperAdmin user to the SuperAdmin role via UserTenantAccess
            // ──────────────────────────────────────
            var superAdminAccess = new UserTenantAccess
            {
                UserId = superAdmin.Id,
                TenantId = genesisTenant.Id,
                RoleId = superAdminRole.Id,
                IsActive = true
            };

            dbContext.UserTenantAccesses.Add(superAdminAccess);

            // ──────────────────────────────────────
            // 7. Commit everything in a single transaction
            // ──────────────────────────────────────
            await dbContext.SaveChangesAsync();
        }
    }
}
