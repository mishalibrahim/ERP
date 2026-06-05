using Erp.Shared.Interfaces;
using System.Security.Claims;

namespace ERP.Services
{
    public class CurrentUserService :ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        // Safely extract the UserId from the token's standard NameIdentifier claim
        public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        // Extract our custom tenant ID claim
        public string? TenantId => _httpContextAccessor.HttpContext?.User?.FindFirstValue("tenant_id");

        // Check if they hold the SuperAdmin role
        public bool IsSuperAdmin => _httpContextAccessor.HttpContext?.User?.IsInRole("SuperAdmin") ?? false;
    }
}
