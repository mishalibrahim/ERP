using System;
using Erp.Shared.Entities;
using Erp.Shared.Enums;

namespace Erp.Module.GL.Entities
{
    public class GlAccount : BaseEntity
    {
        public Guid TenantId { get; set; }
        
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        
        public GlAccountType AccountType { get; set; }
        public GlAccountCategory AccountCategory { get; set; }
        public GlPostingType PostingType { get; set; }
        
        public bool AllowManualEntry { get; set; }
        public bool MandatoryDimensions { get; set; }
        
        public Guid? ParentAccountId { get; set; }
        public GlAccount? ParentAccount { get; set; }
        
        public ICollection<GlAccount> SubAccounts { get; set; } = new List<GlAccount>();

        public Guid? DefaultTaxGroupId { get; set; }
        public TaxGroup? DefaultTaxGroup { get; set; }
    }
}
