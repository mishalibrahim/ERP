using System;
using System.Collections.Generic;
using Erp.Shared.Entities;
using Erp.Shared.Enums;

namespace Erp.Module.GL.Entities
{
    public class JournalEntry : BaseEntity
    {
        public Guid TenantId { get; set; }
        
        public DateTime Date { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        public bool IsPosted { get; set; }

        // Journal Voucher Extended Fields
        public string VoucherNo { get; set; } = string.Empty;
        public string JournalName { get; set; } = string.Empty;
        public string Currency { get; set; } = "AED";
        public JournalVoucherType JournalType { get; set; } = JournalVoucherType.General;
        public string? CostCenter { get; set; }
        public string? Department { get; set; }
        public decimal ExchangeRate { get; set; } = 1.0m;
        public JournalVoucherStatus Status { get; set; } = JournalVoucherStatus.Draft;
        public string? InternalNotes { get; set; }
        public string? ApprovalRemarks { get; set; }
        public JournalVoucherApprovalStage CurrentApprovalStage { get; set; } = JournalVoucherApprovalStage.Initiator;

        // Serialization columns for wide-table performance
        public string ApprovalHistoryJson { get; set; } = "[]";
        public string AttachmentsJson { get; set; } = "[]";

        // Reversal linkage
        public Guid? ReversedVoucherId { get; set; }
        public JournalEntry? ReversedVoucher { get; set; }

        public Guid? ReversingVoucherId { get; set; }
        public JournalEntry? ReversingVoucher { get; set; }
        
        public ICollection<JournalEntryLine> Lines { get; set; } = new List<JournalEntryLine>();
    }
}
