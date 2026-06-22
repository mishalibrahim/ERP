using Erp.Module.Core.Entities;
using Erp.Shared.Entities;
using Erp.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Erp.Module.Core.Data
{
    public class CoreDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUser;

        public CoreDbContext(DbContextOptions<CoreDbContext> options, ICurrentUserService currentUser)
        : base(options)
        {
            _currentUser = currentUser;
        }

        // Existing DbSets
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<TaxGroup> TaxGroups { get; set; }
        public DbSet<TaxRate> TaxRates { get; set; }
        public DbSet<DocumentNumberSeries> DocumentNumberSeries { get; set; }
        public DbSet<PostingGroup> PostingGroups { get; set; }
        public DbSet<UserTenantAccess> UserTenantAccesses { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }

        // RBAC DbSets
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ──────────────────────────────────────
            // RBAC Configuration
            // ──────────────────────────────────────

            // RolePermission: composite PK on the junction table
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            // Permission: unique constraint prevents duplicate "Invoices:Approve:Any"
            modelBuilder.Entity<Permission>()
                .HasIndex(p => new { p.Module, p.Action, p.Resource })
                .IsUnique();

            // Role: tenants see system roles (TenantId == null) + their own custom roles
            modelBuilder.Entity<Role>()
                .HasQueryFilter(r => _currentUser.IsSuperAdmin || r.TenantId == null || r.TenantId == _currentUser.TenantId);

            // ──────────────────────────────────────
            // Existing Multi-Tenancy Query Filters
            // ──────────────────────────────────────

            modelBuilder.Entity<User>()
                .HasQueryFilter(u => _currentUser.IsSuperAdmin || u.TenantId == _currentUser.TenantId);

            modelBuilder.Entity<Tenant>()
                .HasQueryFilter(t => _currentUser.IsSuperAdmin || t.Id == _currentUser.TenantId);

            modelBuilder.Entity<TaxGroup>()
                .HasQueryFilter(x => _currentUser.IsSuperAdmin || x.TenantId == _currentUser.TenantId);
                
            modelBuilder.Entity<DocumentNumberSeries>()
                .HasQueryFilter(x => _currentUser.IsSuperAdmin || x.TenantId == _currentUser.TenantId);
                
            modelBuilder.Entity<PostingGroup>()
                .HasQueryFilter(x => _currentUser.IsSuperAdmin || x.TenantId == _currentUser.TenantId);

            modelBuilder.Entity<UserTenantAccess>()
                .HasQueryFilter(x => _currentUser.IsSuperAdmin || x.TenantId == _currentUser.TenantId);

            modelBuilder.Entity<BankAccount>()
                .HasQueryFilter(x => _currentUser.IsSuperAdmin || x.TenantId == _currentUser.TenantId);

            // ──────────────────────────────────────
            // Existing Indexes & Owned Types
            // ──────────────────────────────────────

            modelBuilder.Entity<Tenant>()
                .HasIndex(t => t.CompanyCode)
                .IsUnique();

            modelBuilder.Entity<Tenant>()
                .OwnsOne(t => t.VatDetails)
                .Property(v => v.TrnNumber)
                .HasMaxLength(15);
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
