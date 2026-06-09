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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasQueryFilter(u => _currentUser.IsSuperAdmin || u.TenantId == _currentUser.TenantId);

            modelBuilder.Entity<Tenant>()
                .HasQueryFilter(t => _currentUser.IsSuperAdmin || t.Id == _currentUser.TenantId);

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
