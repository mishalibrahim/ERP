using Erp.Shared.Entities;

namespace Erp.Module.Core.Entities
{
    public class Permission
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // e.g. "Customers", "Invoices", "Reports"
        public string Module { get; set; } = string.Empty;

        // e.g. "Create", "Read", "Update", "Delete", "Approve"
        public string Action { get; set; } = string.Empty;

        // e.g. "Own", "Any", "Tenant"
        public string Resource { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        // Computed key for lookups — not stored in DB
        public string Key => $"{Module}:{Action}:{Resource}";

        // Navigation
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
