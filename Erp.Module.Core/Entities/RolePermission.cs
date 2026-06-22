using Erp.Shared.Entities;

namespace Erp.Module.Core.Entities
{
    public class RolePermission
    {
        public Guid RoleId { get; set; }
        public Role? Role { get; set; }

        public Guid PermissionId { get; set; }
        public Permission? Permission { get; set; }

        // Audit fields
        public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
        public Guid? GrantedBy { get; set; }  // UserId of whoever assigned this permission
    }
}
