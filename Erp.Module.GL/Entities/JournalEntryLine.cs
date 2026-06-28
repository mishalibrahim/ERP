using System;
using Erp.Shared.Entities;
using Erp.Shared.Enums;

namespace Erp.Module.GL.Entities
{
    public class JournalEntryLine : BaseEntity
    {
        public Guid TenantId { get; set; }
        
        public Guid JournalEntryId { get; set; }
        public JournalEntry? JournalEntry { get; set; }
        
        public Guid GlAccountId { get; set; }
        public GlAccount? GlAccount { get; set; }
        
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        
        // Optional links
        public Guid? TaxCodeId { get; set; }
        public TaxGroup? TaxCode { get; set; }
        
        public Guid? DimensionId { get; set; }
        public Dimension? Dimension { get; set; }

        // Extended Line Fields
        public GlAccountType AccountType { get; set; } = GlAccountType.Ledger;
        public string? Description { get; set; }
        public string? CostCenter { get; set; }

        public GlAccountType? OffsetType { get; set; }
        public Guid? OffsetAccountId { get; set; }
        public GlAccount? OffsetAccount { get; set; }
    }
}
