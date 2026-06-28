namespace ERP.Features.GeneralLedger.DTOs
{
    public class PeriodLockDto
    {
        public string PeriodValue { get; set; } = string.Empty; // "2026-05"
        public string PeriodLabel { get; set; } = string.Empty; // "May 2026"
        public bool IsLocked { get; set; }
        public string? LockedBy { get; set; }
        public DateTime? LockedAt { get; set; }
    }
}
