using System;
using System.Collections.Generic;
using System.Text;
using Erp.Shared.Entities;

namespace Erp.Module.Core.Entities
{
    public class User : BaseEntity
    {
        // 1. Multi-Tenancy Links (Home Tenant)
        public Guid? TenantId { get; set; }
        public Tenant? Tenant { get; set; } 

        public ICollection<UserTenantAccess> TenantAccesses { get; set; } = new List<UserTenantAccess>();

        // Basic Info
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        // 2. Contact Info
        public string? PhoneNumber { get; set; } // <-- Added this

        // 3. Audit Trails
        public DateTime? LastLoginAt { get; set; }                 // <-- Added this
    }
}
