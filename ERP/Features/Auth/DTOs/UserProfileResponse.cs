namespace ERP.Features.Auth.DTOs
{
    public class UserProfileResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public Guid? TenantId { get; set; }
        public List<string> Permissions { get; set; } = [];
    }
}

