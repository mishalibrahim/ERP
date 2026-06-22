using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Features.CompanySetup.DTOs
{
    public class PostingGroupDto
        {
            public Guid? Id { get; set; }
            public string GroupName { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public string? ReceivablesAccountId { get; set; }
            public string? PayablesAccountId { get; set; }
            public string? InventoryAccountId { get; set; }
            public string? CogsAccountId { get; set; }
            public bool IsActive { get; set; } = true;
        }
}

