using Erp.Module.GL.Data;
using Erp.Module.GL.Entities;
using ERP.Features.Dimensions.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Erp.Shared.Interfaces;

namespace ERP.Features.Dimensions
{
    public class DimensionService : IDimensionService
    {
        private readonly GlDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public DimensionService(GlDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<List<DimensionDto>> GetAllAsync()
        {
            return await _context.Dimensions
                .Where(d => d.IsActive)
                .Select(d => new DimensionDto
                {
                    Id = d.Id,
                    Type = d.Type,
                    Code = d.Code,
                    Name = d.Name,
                    IsActive = d.IsActive
                })
                .OrderBy(d => d.Type).ThenBy(d => d.Code)
                .ToListAsync();
        }

        public async Task<DimensionDto?> GetByIdAsync(Guid id)
        {
            var dimension = await _context.Dimensions
                .Where(d => d.Id == id && d.IsActive)
                .FirstOrDefaultAsync();

            if (dimension == null) return null;

            return new DimensionDto
            {
                Id = dimension.Id,
                Type = dimension.Type,
                Code = dimension.Code,
                Name = dimension.Name,
                IsActive = dimension.IsActive
            };
        }

        public async Task<DimensionDto> CreateAsync(CreateDimensionDto dto)
        {
            var tenantId = _currentUserService.TenantId;
            if (tenantId == null) throw new UnauthorizedAccessException("Tenant ID is required.");

            // Check if dimension code already exists for this tenant
            var exists = await _context.Dimensions
                .AnyAsync(d => d.Code == dto.Code);
            if (exists)
                throw new InvalidOperationException($"Dimension code {dto.Code} already exists.");

            var dimension = new Dimension
            {
                TenantId = tenantId.Value,
                Type = dto.Type,
                Code = dto.Code,
                Name = dto.Name,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Dimensions.Add(dimension);
            await _context.SaveChangesAsync();

            return new DimensionDto
            {
                Id = dimension.Id,
                Type = dimension.Type,
                Code = dimension.Code,
                Name = dimension.Name,
                IsActive = dimension.IsActive
            };
        }

        public async Task<DimensionDto?> UpdateAsync(Guid id, UpdateDimensionDto dto)
        {
            var dimension = await _context.Dimensions
                .Where(d => d.Id == id && d.IsActive)
                .FirstOrDefaultAsync();

            if (dimension == null) return null;

            // Check uniqueness if code is changed
            if (dimension.Code != dto.Code)
            {
                var exists = await _context.Dimensions
                    .AnyAsync(d => d.Code == dto.Code);
                if (exists)
                    throw new InvalidOperationException($"Dimension code {dto.Code} already exists.");
            }

            dimension.Type = dto.Type;
            dimension.Code = dto.Code;
            dimension.Name = dto.Name;
            dimension.IsActive = dto.IsActive;
            dimension.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new DimensionDto
            {
                Id = dimension.Id,
                Type = dimension.Type,
                Code = dimension.Code,
                Name = dimension.Name,
                IsActive = dimension.IsActive
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var dimension = await _context.Dimensions
                .Where(d => d.Id == id && d.IsActive)
                .FirstOrDefaultAsync();

            if (dimension == null) return false;

            // Optional: Check if dimension is used in Journal Entries
            
            dimension.IsActive = false;
            dimension.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
