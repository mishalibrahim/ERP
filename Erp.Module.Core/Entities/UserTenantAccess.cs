using System;

namespace Erp.Module.Core.Entities
{
    public class UserTenantAccess
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string UserId { get; set; } = string.Empty;
        public User? User { get; set; }

        public string TenantId { get; set; } = string.Empty;
        public Tenant? Tenant { get; set; }

        // Specific role for this tenant (e.g., "Admin", "Accountant", "Auditor")
        public string Role { get; set; } = "User";

        public bool IsActive { get; set; } = true;
    }
}
