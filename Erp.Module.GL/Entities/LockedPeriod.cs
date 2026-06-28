using Erp.Shared.Entities;
using System;

namespace Erp.Module.GL.Entities
{
    public class LockedPeriod : BaseEntity
    {
        public Guid TenantId { get; set; }

        /// <summary>Year of the locked period, e.g. 2026</summary>
        public int Year { get; set; }

        /// <summary>Month 1–12 of the locked period</summary>
        public int Month { get; set; }

        public bool IsLocked { get; set; }

        public string? LockedBy { get; set; }

        public DateTime? LockedAt { get; set; }
    }
}
