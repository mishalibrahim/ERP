namespace ERP.Features.Auth.DTOs
{
    public record AuthResponse(
        string Token,
        string Email,
        string RoleName,
        Guid? TenantId,
        List<string> Permissions
    );
}

