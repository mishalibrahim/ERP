using System;

namespace Erp.Module.Core.Entities
{
    public class PostingGroup
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string TenantId { get; set; } = string.Empty;
        public Tenant? Tenant { get; set; }

        public string GroupName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // e.g., "Customer", "Vendor", "Item"

        public string? ReceivablesAccountId { get; set; }
        public string? PayablesAccountId { get; set; }
        public string? InventoryAccountId { get; set; }
        public string? CogsAccountId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
