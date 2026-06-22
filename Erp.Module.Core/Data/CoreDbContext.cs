using Erp.Module.Core.Entities;
using Erp.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Erp.Module.Core.Data
{
    public class CoreDbContext :DbContext
    {
        private readonly ICurrentUserService _currentUser;

        public CoreDbContext(DbContextOptions<CoreDbContext> options, ICurrentUserService currentUser)
        : base(options)
        {
            _currentUser = currentUser;
        }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<TaxGroup> TaxGroups { get; set; }
        public DbSet<TaxRate> TaxRates { get; set; }
        public DbSet<DocumentNumberSeries> DocumentNumberSeries { get; set; }
        public DbSet<PostingGroup> PostingGroups { get; set; }
        public DbSet<UserTenantAccess> UserTenantAccesses { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasQueryFilter(u => _currentUser.IsSuperAdmin || u.TenantId == _currentUser.TenantId);

            modelBuilder.Entity<Tenant>()
                .HasQueryFilter(t => _currentUser.IsSuperAdmin || t.Id == _currentUser.TenantId);

            // Multi-tenancy Query Filters
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

            modelBuilder.Entity<Tenant>()
                .HasIndex(t => t.CompanyCode)
                .IsUnique();


            modelBuilder.Entity<Tenant>()
                .OwnsOne(t => t.VatDetails)
                .Property(v => v.TrnNumber)
                .HasMaxLength(15);
        }

    }
}
