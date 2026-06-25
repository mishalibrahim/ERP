using Erp.Shared.Interfaces;
using System.Security.Claims;

namespace ERP.Features.Auth
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Safely extract the UserId from the token's standard NameIdentifier claim
        public Guid? UserId
        {
            get
            {
                var val = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return Guid.TryParse(val, out var guid) ? guid : null;
            }
        }

        // Extract our custom tenant ID claim
        public Guid? TenantId
        {
            get
            {
                var val = _httpContextAccessor.HttpContext?.User?.FindFirstValue("tenant_id");
                return Guid.TryParse(val, out var guid) ? guid : null;
            }
        }

        // SuperAdmin is now derived from the JWT role claim, not a DB column
        public bool IsSuperAdmin =>
            _httpContextAccessor.HttpContext?.User?.IsInRole("SuperAdmin") ?? false;

        // Permissions are embedded in the JWT as individual "permission" claims
        public IReadOnlyList<string> Permissions =>
            _httpContextAccessor.HttpContext?.User?
                .FindAll("permission")
                .Select(c => c.Value)
                .ToList() ?? [];

        public bool HasPermission(string permissionKey) =>
            IsSuperAdmin || Permissions.Contains(permissionKey);
    }
}
