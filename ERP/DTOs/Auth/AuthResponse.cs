namespace ERP.DTOs.Auth
{
    public record AuthResponse(string Token, string Email, string Role, string TenantId);
}
