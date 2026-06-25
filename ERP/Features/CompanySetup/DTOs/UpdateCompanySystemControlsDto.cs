using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Features.CompanySetup.DTOs
{
    public class UpdateCompanySystemControlsDto : UpdateCompanyBaseDto
        {
            public bool? MultiCompanyEnable { get; set; }
            public bool? AuditTrailEnable { get; set; }
            public bool? ApprovalWorkflow { get; set; }
            public string? DefaultCostCenterId { get; set; }
            public string? DefaultProjectId { get; set; }
    
            public List<DocumentNumberSeriesDto>? DocumentNumberSeries { get; set; }
            public List<PostingGroupDto>? PostingGroups { get; set; }
        }
}

