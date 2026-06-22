using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Features.CompanySetup.DTOs
{
    public class DocumentNumberSeriesDto
        {
            public Guid? Id { get; set; }
            public string DocumentType { get; set; } = string.Empty;
            public string Prefix { get; set; } = string.Empty;
            public long CurrentNumber { get; set; } = 0;
            public string? Suffix { get; set; }
            public bool IsActive { get; set; } = true;
        }
}

