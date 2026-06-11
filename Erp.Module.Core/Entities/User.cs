using System;
using System.Collections.Generic;
using System.Text;

namespace Erp.Module.Core.Entities
{
    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // 1. Multi-Tenancy Links
        public string TenantId { get; set; } = string.Empty;
        public Tenant? Tenant { get; set; } // <-- Added this

        // Basic Info
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        // 2. Contact Info
        public string? PhoneNumber { get; set; } // <-- Added this

        // Security & Roles
        public bool IsSuperAdmin { get; set; } = false;
        public string Role { get; set; } = "User";
        public bool IsActive { get; set; } = true;

        // 3. Audit Trails
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // <-- Added this
        public DateTime? LastLoginAt { get; set; }                 // <-- Added this
    }
}
