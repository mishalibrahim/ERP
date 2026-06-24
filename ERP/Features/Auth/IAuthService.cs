using ERP.Features.Auth.DTOs;

namespace ERP.Features.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<UserProfileResponse> GetMyProfileAsync(Guid userId);
        Task<List<TenantListItemDto>> GetMyTenantsAsync(Guid userId, bool isSuperAdmin);
        Task<AuthResponse> SwitchTenantAsync(Guid userId, Guid targetTenantId, bool isSuperAdmin);
    }
}
