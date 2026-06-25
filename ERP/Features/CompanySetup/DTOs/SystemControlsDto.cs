using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Features.CompanySetup.DTOs
{
    public class SystemControlsDto
        {
            public bool? MultiCompanyEnable { get; set; }
            public bool? AuditTrailEnable { get; set; }
            public bool? ApprovalWorkflow { get; set; }
            public string? DefaultCostCenterId { get; set; }
            public string? DefaultProjectId { get; set; }
        }
}

