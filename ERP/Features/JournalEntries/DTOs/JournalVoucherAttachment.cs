using System;

namespace ERP.Features.JournalEntries.DTOs
{
    public class JournalVoucherAttachment
    {
        public string Name { get; set; } = string.Empty;
        public long Size { get; set; }
        public string Type { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }
}
