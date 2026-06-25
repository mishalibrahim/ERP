using Erp.Shared.Entities;

namespace Erp.Module.Core.Entities
{
    public class Role
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // null = system-wide role (e.g. SuperAdmin), set = tenant-custom role
        public Guid? TenantId { get; set; }
        public Tenant? Tenant { get; set; }

        public string Name { get; set; } = string.Empty;         // "SuperAdmin", "Admin", "Accountant"
        public string Description { get; set; } = string.Empty;
        public bool IsSystemRole { get; set; } = false;          // System roles can't be deleted by tenants

        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        public ICollection<UserTenantAccess> UserTenantAccesses { get; set; } = new List<UserTenantAccess>();
    }
}
