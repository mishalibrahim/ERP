using System;
using Erp.Shared.Enums;

namespace ERP.Features.Dimensions.DTOs
{
    public class DimensionDto
    {
        public Guid Id { get; set; }
        public DimensionType Type { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
