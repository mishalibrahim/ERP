using Erp.Module.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace Erp.Module.Core.Data
{
    public static  class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<CoreDbContext>();
            var passwordHasher = serviceProvider.GetRequiredService<IPasswordHasher<User>>();

            // 2. Check if the database already has a SuperAdmin.
            // We use IgnoreQueryFilters() because nobody is logged in during startup!
            if (await dbContext.Users.IgnoreQueryFilters().AnyAsync(u => u.IsSuperAdmin))
            {
                return; // Database is already seeded, skip this script!
            }
            // 3. Create the Genesis Company (Tenant)
            var genesisTenant = new Tenant
            {
                Id = Guid.NewGuid().ToString(),
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

            // 4. Create your Master Account
            var superAdmin = new User
            {
                Id = Guid.NewGuid().ToString(),
                TenantId = genesisTenant.Id,
                FirstName = "Mishal",
                LastName = "Admin",
                Email = "mishal@aegiserp.com", // You will use this to log in
                Role = "SuperAdmin",
                IsSuperAdmin = true,
                IsActive = true
            };

            // 5. Securely hash the password (We'll use "Admin@123!" for local development)
            superAdmin.PasswordHash = passwordHasher.HashPassword(superAdmin, "Admin@123!");

            dbContext.Users.Add(superAdmin);

            // 6. Commit the changes to SQL Server
            await dbContext.SaveChangesAsync();

        }
    }
}
