using System;
using System.Collections.Generic;

namespace ERP.Features.JournalEntries.DTOs
{
    public class JournalEntryDto
    {
        public Guid Id { get; set; }
        public string VoucherNo { get; set; } = string.Empty;
        public string JournalName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Currency { get; set; } = "AED";
        public string JournalType { get; set; } = string.Empty; // General, Adjusting, Accrual, Reversing, Opening
        public string? CostCenter { get; set; }
        public string? Department { get; set; }
        public decimal ExchangeRate { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // Draft, Pending Approval, Approved, Posted, Rejected, Reversed
        
        public List<JournalEntryLineDto> Lines { get; set; } = new List<JournalEntryLineDto>();
        public List<JournalVoucherAttachment> Attachments { get; set; } = new List<JournalVoucherAttachment>();
        public string InternalNotes { get; set; } = string.Empty;
        public string ApprovalRemarks { get; set; } = string.Empty;
        public string CurrentApprovalStage { get; set; } = string.Empty; // Initiator, Finance Review, CFO Approve, Posted
        public List<JournalVoucherApprovalHistoryItem> ApprovalHistory { get; set; } = new List<JournalVoucherApprovalHistoryItem>();
        
        public Guid? ReversedVoucherId { get; set; }
        public Guid? ReversingVoucherId { get; set; }
    }
}
