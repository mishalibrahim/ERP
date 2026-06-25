using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Features.CompanySetup.DTOs
{
    public abstract class UpdateCompanyBaseDto
        {
            [Required]
            [MinLength(8, ErrorMessage = "RowVersion is required and must be valid.")]
            public byte[] RowVersion { get; set; } = Array.Empty<byte>();
        }
}

