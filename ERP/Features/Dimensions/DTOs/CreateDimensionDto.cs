using Erp.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace ERP.Features.Dimensions.DTOs
{
    public class CreateDimensionDto
    {
        public DimensionType Type { get; set; }
        [Required]
        public string Code { get; set; } = string.Empty;
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
