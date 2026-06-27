using Erp.Module.GL.Entities;
using Erp.Shared.Entities;
using Erp.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Erp.Module.GL.Data
{
    public class GlDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUser;

        public GlDbContext(DbContextOptions<GlDbContext> options, ICurrentUserService currentUser)
            : base(options)
        {
            _currentUser = currentUser;
        }

        public DbSet<GlAccount> GlAccounts { get; set; }
        public DbSet<Dimension> Dimensions { get; set; }
        public DbSet<JournalEntry> JournalEntries { get; set; }
        public DbSet<JournalEntryLine> JournalEntryLines { get; set; }
        public DbSet<TaxGroup> TaxGroups { get; set; }
        public DbSet<TaxRate> TaxRates { get; set; }
        public DbSet<LockedPeriod> LockedPeriods { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ──────────────────────────────────────
            // Multi-Tenancy Query Filters
            // ──────────────────────────────────────

            modelBuilder.Entity<GlAccount>()
                .HasQueryFilter(x => x.TenantId == _currentUser.TenantId);

            modelBuilder.Entity<Dimension>()
                .HasQueryFilter(x => x.TenantId == _currentUser.TenantId);

            modelBuilder.Entity<JournalEntry>()
                .HasQueryFilter(x => x.TenantId == _currentUser.TenantId);

            modelBuilder.Entity<JournalEntryLine>()
                .HasQueryFilter(x => x.TenantId == _currentUser.TenantId);

            modelBuilder.Entity<TaxGroup>()
                .HasQueryFilter(x => x.TenantId == _currentUser.TenantId);

            modelBuilder.Entity<TaxRate>()
                .HasQueryFilter(x => x.TaxGroup != null && x.TaxGroup.TenantId == _currentUser.TenantId);

            modelBuilder.Entity<LockedPeriod>()
                .HasQueryFilter(x => x.TenantId == _currentUser.TenantId);

            modelBuilder.Entity<LockedPeriod>()
                .HasIndex(lp => new { lp.TenantId, lp.Year, lp.Month })
                .IsUnique();
                
            // ──────────────────────────────────────
            // Additional Constraints
            // ──────────────────────────────────────
            
            modelBuilder.Entity<GlAccount>()
                .HasIndex(a => new { a.TenantId, a.AccountNumber })
                .IsUnique();

            modelBuilder.Entity<GlAccount>()
                .HasOne(a => a.ParentAccount)
                .WithMany(a => a.SubAccounts)
                .HasForeignKey(a => a.ParentAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GlAccount>()
                .HasOne(a => a.DefaultTaxGroup)
                .WithMany()
                .HasForeignKey(a => a.DefaultTaxGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<JournalEntryLine>()
                .Property(l => l.Debit)
                .HasPrecision(18, 4);

            modelBuilder.Entity<JournalEntryLine>()
                .Property(l => l.Credit)
                .HasPrecision(18, 4);

            modelBuilder.Entity<JournalEntry>()
                .Property(j => j.ExchangeRate)
                .HasPrecision(18, 4);

            modelBuilder.Entity<JournalEntry>()
                .HasOne(j => j.ReversedVoucher)
                .WithMany()
                .HasForeignKey(j => j.ReversedVoucherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<JournalEntry>()
                .HasOne(j => j.ReversingVoucher)
                .WithMany()
                .HasForeignKey(j => j.ReversingVoucherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<JournalEntryLine>()
                .HasOne(l => l.OffsetAccount)
                .WithMany()
                .HasForeignKey(l => l.OffsetAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaxRate>()
                .Property(r => r.RatePercentage)
                .HasPrecision(18, 4);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var userId = _currentUser.UserId;

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = userId;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.ModifiedAt = DateTime.UtcNow;
                    entry.Entity.ModifiedBy = userId;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
