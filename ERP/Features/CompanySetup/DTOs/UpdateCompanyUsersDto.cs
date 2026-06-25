using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Features.CompanySetup.DTOs
{
    public class UpdateCompanyUsersDto : UpdateCompanyBaseDto
        {
            public string? Status { get; set; }
            public List<UserTenantAccessDto>? UserTenantAccess { get; set; }
        }
}

