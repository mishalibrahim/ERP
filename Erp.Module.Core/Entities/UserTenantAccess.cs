using System;
using Erp.Shared.Entities;

namespace Erp.Module.Core.Entities
{
    public class UserTenantAccess
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }
        public User? User { get; set; }

        public Guid TenantId { get; set; }
        public Tenant? Tenant { get; set; }

        // FK to the RBAC Role for this tenant
        public Guid RoleId { get; set; }
        public Role? Role { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
