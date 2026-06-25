using Erp.Module.GL.Data;
using Erp.Module.GL.Entities;
using ERP.Features.Taxes.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Erp.Shared.Interfaces;

namespace ERP.Features.Taxes
{
    public class TaxService : ITaxService
    {
        private readonly GlDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public TaxService(GlDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<List<TaxGroupDto>> GetAllAsync()
        {
            return await _context.TaxGroups
                .Include(tg => tg.TaxRates)
                .Where(tg => tg.IsActive)
                .Select(tg => new TaxGroupDto
                {
                    Id = tg.Id,
                    Name = tg.Name,
                    Description = tg.Description,
                    InputVatAccountId = tg.InputVatAccountId,
                    OutputVatAccountId = tg.OutputVatAccountId,
                    IsActive = tg.IsActive,
                    TaxRates = tg.TaxRates.Select(tr => new TaxRateDto
                    {
                        Id = tr.Id,
                        RatePercentage = tr.RatePercentage,
                        EffectiveFrom = tr.EffectiveFrom,
                        EffectiveTo = tr.EffectiveTo,
                        IsActive = tr.IsActive
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<TaxGroupDto?> GetByIdAsync(Guid id)
        {
            var tg = await _context.TaxGroups
                .Include(t => t.TaxRates)
                .Where(t => t.Id == id && t.IsActive)
                .FirstOrDefaultAsync();

            if (tg == null) return null;

            return new TaxGroupDto
            {
                Id = tg.Id,
                Name = tg.Name,
                Description = tg.Description,
                InputVatAccountId = tg.InputVatAccountId,
                OutputVatAccountId = tg.OutputVatAccountId,
                IsActive = tg.IsActive,
                TaxRates = tg.TaxRates.Select(tr => new TaxRateDto
                {
                    Id = tr.Id,
                    RatePercentage = tr.RatePercentage,
                    EffectiveFrom = tr.EffectiveFrom,
                    EffectiveTo = tr.EffectiveTo,
                    IsActive = tr.IsActive
                }).ToList()
            };
        }

        public async Task<TaxGroupDto> CreateAsync(CreateTaxGroupDto dto)
        {
            var tenantId = _currentUserService.TenantId;
            if (tenantId == null) throw new UnauthorizedAccessException("Tenant ID is required.");

            var taxGroup = new TaxGroup
            {
                TenantId = tenantId.Value,
                Name = dto.Name,
                Description = dto.Description,
                InputVatAccountId = dto.InputVatAccountId,
                OutputVatAccountId = dto.OutputVatAccountId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var trDto in dto.TaxRates)
            {
                taxGroup.TaxRates.Add(new TaxRate
                {
                    RatePercentage = trDto.RatePercentage,
                    EffectiveFrom = trDto.EffectiveFrom,
                    EffectiveTo = trDto.EffectiveTo,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            _context.TaxGroups.Add(taxGroup);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(taxGroup.Id) ?? throw new Exception("Failed to retrieve created tax group.");
        }

        public async Task<TaxGroupDto?> UpdateAsync(Guid id, UpdateTaxGroupDto dto)
        {
            var taxGroup = await _context.TaxGroups
                .Include(tg => tg.TaxRates)
                .Where(tg => tg.Id == id && tg.IsActive)
                .FirstOrDefaultAsync();

            if (taxGroup == null) return null;

            taxGroup.Name = dto.Name;
            taxGroup.Description = dto.Description;
            taxGroup.InputVatAccountId = dto.InputVatAccountId;
            taxGroup.OutputVatAccountId = dto.OutputVatAccountId;
            taxGroup.IsActive = dto.IsActive;
            taxGroup.ModifiedAt = DateTime.UtcNow;

            var incomingRateIds = dto.TaxRates.Where(tr => tr.Id.HasValue).Select(tr => tr.Id.Value).ToList();
            var removedRates = taxGroup.TaxRates.Where(tr => !incomingRateIds.Contains(tr.Id)).ToList();
            
            _context.TaxRates.RemoveRange(removedRates);

            foreach (var trDto in dto.TaxRates)
            {
                if (trDto.Id.HasValue)
                {
                    var existingTr = taxGroup.TaxRates.FirstOrDefault(tr => tr.Id == trDto.Id.Value);
                    if (existingTr != null)
                    {
                        existingTr.RatePercentage = trDto.RatePercentage;
                        existingTr.EffectiveFrom = trDto.EffectiveFrom;
                        existingTr.EffectiveTo = trDto.EffectiveTo;
                        existingTr.IsActive = trDto.IsActive;
                        existingTr.ModifiedAt = DateTime.UtcNow;
                    }
                }
                else
                {
                    taxGroup.TaxRates.Add(new TaxRate
                    {
                        RatePercentage = trDto.RatePercentage,
                        EffectiveFrom = trDto.EffectiveFrom,
                        EffectiveTo = trDto.EffectiveTo,
                        IsActive = trDto.IsActive,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();

            return await GetByIdAsync(taxGroup.Id);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var taxGroup = await _context.TaxGroups
                .Where(tg => tg.Id == id && tg.IsActive)
                .FirstOrDefaultAsync();

            if (taxGroup == null) return false;

            taxGroup.IsActive = false;
            taxGroup.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
