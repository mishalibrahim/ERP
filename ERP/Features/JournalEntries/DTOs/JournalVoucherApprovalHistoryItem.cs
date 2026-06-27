using System;

namespace ERP.Features.JournalEntries.DTOs
{
    public class JournalVoucherApprovalHistoryItem
    {
        public string Stage { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Actor { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string? Remarks { get; set; }
    }
}
