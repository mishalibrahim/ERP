using System;
using System.Collections.Generic;
using System.Text;
using Erp.Shared.Entities;

namespace Erp.Module.Core.Entities
{
    public class BankAccount
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // This connects the bank account back to the specific Company
        public Guid TenantId { get; set; }
        public Tenant? Tenant { get; set; }

        public bool IsPrimary { get; set; } = false; // "Provide Primary bank" requirement
        public string BankName { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string? Iban { get; set; }
        public string? SwiftCode { get; set; }
        public string Currency { get; set; } = "AED";
    }
}
