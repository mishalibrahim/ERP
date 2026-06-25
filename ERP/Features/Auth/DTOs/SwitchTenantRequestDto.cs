using System;

namespace ERP.Features.Auth.DTOs
{
    public class SwitchTenantRequestDto
    {
        public Guid TargetTenantId { get; set; }
    }
}
