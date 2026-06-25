using System;
using System.Collections.Generic;
using System.Text;

namespace Erp.Shared.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        Guid? TenantId { get; }
        bool IsSuperAdmin { get; }

        // RBAC: resolved permission keys from the JWT
        IReadOnlyList<string> Permissions { get; }
        bool HasPermission(string permissionKey);
    }
}

