using System;
using Erp.Shared.Entities;
using Erp.Shared.Enums;

namespace Erp.Module.GL.Entities
{
    public class Dimension : BaseEntity
    {
        public Guid TenantId { get; set; }
        
        public DimensionType Type { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
