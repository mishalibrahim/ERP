using ERP.DTOs.Auth;

namespace ERP.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest loginRequest);
        Task<UserProfileResponse> GetMyProfileAsync(string userId);
    }
}
