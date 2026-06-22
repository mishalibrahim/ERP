using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Features.CompanySetup.DTOs
{
    public class UserTenantAccessDto
        {
            public Guid? Id { get; set; }
            public Guid? UserId { get; set; }
            public string Email { get; set; } = string.Empty;
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? Password { get; set; } // Used for new users
            public Guid RoleId { get; set; }
            public string? RoleName { get; set; }  // Read-only, for display
            public bool IsActive { get; set; } = true;
        }
}

