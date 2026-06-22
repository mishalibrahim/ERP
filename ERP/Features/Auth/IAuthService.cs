using ERP.Features.Auth.DTOs;

namespace ERP.Features.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest loginRequest);
        Task<UserProfileResponse> GetMyProfileAsync(Guid userId);
    }
}
