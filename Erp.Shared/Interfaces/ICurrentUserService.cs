using System;
using System.Collections.Generic;
using System.Text;

namespace Erp.Shared.Interfaces
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? TenantId { get; }
        bool IsSuperAdmin { get; }
    }
}
