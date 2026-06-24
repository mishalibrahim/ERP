using System;

namespace ERP.Features.Auth.DTOs
{
    public class TenantListItemDto
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyCode { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }
}
